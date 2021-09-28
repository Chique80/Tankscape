using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
Le script EnemyAI.cs est le coeur de l'ai. C'est celui gère l'état de l'ai.
 */

public class EnemyAI : MonoBehaviour 
{
    private EnemyAIValue valueScript;

    [Header("Pathing")]
    private float pathfindingUpdateTimer;
	private WallVector[] _mapWalls;

    [Header("Mouvement")]
    private int _currentWaypointIndex;
    private Path _path;

    [Header("State")]
    UnityEvent exitState;
	
	// Use this for initialization
	void Start () 
	{
        //Get the EnemyAIValue.cs script of this enemy
        valueScript = GetComponent<EnemyAIValue>();
        if(valueScript == null)
        {
            Debug.LogError(this + " doesn't have a EnemyAIValue.cs script!");
        }

		//Get the walls of the map
		if(valueScript.hasWallManager)
		{
			_mapWalls = valueScript.wallManager.getMapVectors();
		}
        else
        {
            _mapWalls = new WallVector[0];
        }

        //Starter les timer
        pathfindingUpdateTimer = valueScript.pathfindingUpdateRate;

        //Initialiser des variables
        exitState = new UnityEvent();
        exitState.AddListener(exitSearchState);
	}
	
	// Update is called once per frame
	void Update () 
	{
        updateState();                                                                              //Update the state
        
        if(valueScript.state == EnemyAIValue.State.Rush)
        {
            //Updater le pathfinding
            if(valueScript.hasPathfindingScript)
            {
                //Updater les timer
                pathfindingUpdateTimer += Time.deltaTime;

                //Chercher un nouveau path, si nécessaire
                if (pathfindingUpdateTimer >= valueScript.pathfindingUpdateRate)
                { 
                    createPath();
                }
                else if(!valueScript.pathfinder.checkWaypoint(waypointTarget, mapWalls) && path!=null)
                {
                    createPath();
                }
                
                updatePath();                                                                       //Vérifier l'état du path actuel
            }
        }   
    }

    // Use this for debuging
    void OnDrawGizmosSelected()
	{
		if(Application.isPlaying)
		{
            Gizmos.color = valueScript.aiGizmosColor;

            if (valueScript.showAIGizmos)
            {
                //Dessiner le path
                if (path != null)
                {
                    path.draw(Gizmos.color);
                }
            }

            //Dessiner les murs
            if (mapWalls != null)
            {
                for (int i = 0; i < mapWalls.Length; i++)
                {
                    mapWalls[i].draw(Gizmos.color);
                }
            }
        }
	}

    /// <summary>
    ///     Réinitialiser le timer de pathfinding.false Permet de créer un nouveau path à la prochaine frame.
    /// </summary>
    public void refreshTimer()
    {
        pathfindingUpdateTimer = valueScript.pathfindingUpdateRate;
    }

    /// <summary>
    ///     Créer un chemin pour se déplacer vers la cible. Le script Pathfinder.cs est appelé pour créer le chemin.false
    /// </summary>
    private void createPath()
    {
        //Créer le meilleur chemin vers le joueur
        _path = valueScript.pathfinder.findPath(valueScript.playerPosition, mapWalls);

        if (path != null)
        {
            _currentWaypointIndex = 0;
        }

        pathfindingUpdateTimer = 0;                                                   
    }

    /// <summary>
    ///     Vérifier la position du tank sur le chemin. Si le tank atteint un waypoint, passé au waypoint suivant.
    /// </summary>
    private void updatePath()
    {
        if (path!=null)
        {
            //Vérifier si le waypoints est passé
            if (Vector3.Distance(waypointTarget, transform.position) <= valueScript.minDistanceToWaypoint)
            {
                _currentWaypointIndex = path.getWaypointIndex(waypointTarget) + 1;

                //Vérifier s'il reste des waypoints dans la liste
                if (currentWaypointIndex >= path.waypoints.Length)
                {
                    createPath();                                                       //Créer un nouveau path
                }
            }
        }
    }

#region STATE
    /*  Toutes les méthodes de cette section gère le changement d'état.
        La méthode updateState() permet de décide l'état que doit prendre le tank. La méthode changeState() effectuer les actions nécessaires 
        au changement de l'état. Les autres méthodes sont appelés par le UnityEvent exitState lorsque le tank entre/sort de l'état correspondant.
    */
     
    private void updateState()
    {
        //Vérifier la position et l'existence du jouer
        if(valueScript.playerTank == null)                                                      //Il n'y a pas de joueur
        {
            changeState(EnemyAIValue.State.Search);                                                     //Mettre le tank en état Search
        }
        else if(valueScript.isPlayerInSight())                                                  //Le joueur n'est pas bloqué par un mur
        {
            //Vérifier la distance au joueur
            if(valueScript.distanceToPlayer <= valueScript.fleeStartRange)                          //Le tank est sous range de fuite
            {
                changeState(EnemyAIValue.State.Flee);
            }
            else if(valueScript.distanceToPlayer <= valueScript.idleStartRange)                 //Le tank est sous la range d'immobilisation
            {
                changeState(EnemyAIValue.State.Idle);                                                   //Mettre le tank en état Idle
            }   
            else                                                                                //Le tank est avant la range d'immobilisation
            {
                changeState(EnemyAIValue.State.Move);                                                   //Mettre le tank en état Move
            }
        }
        else if(valueScript.hasPathfindingScript)                                               //Le joueur est bloqué par un mur
        {
            changeState(EnemyAIValue.State.Rush);                                                       //Mettre le tank en état Rush
        }
        else                                                                                    //Le tank n'a pas de Pathfinding.cs script
        {
            changeState(EnemyAIValue.State.Idle);                                                       //Mettre le tank en état de Idle
        }
    }
    private void changeState(EnemyAIValue.State newState)
    {
        //Vérifier que le tank change bien d'état
        if(valueScript.state != newState)
        {
            valueScript.state = newState;                            

            //Quitter l'état actuel
            exitState.Invoke();
            exitState.RemoveAllListeners();                                    

            //Entrer dans le nouvel état
            switch(newState)
            {
                case EnemyAIValue.State.Search:
                    enterSearchState();
                    exitState.AddListener(exitSearchState);
                    break;

                case EnemyAIValue.State.Rush:
                    enterRushState();
                    exitState.AddListener(exitRushState);
                    break;

                case EnemyAIValue.State.Move:
                    enterMoveState();
                    exitState.AddListener(exitMoveState);
                    break;

                case EnemyAIValue.State.Idle:
                    enterIdleState();
                    exitState.AddListener(exitIdleState);
                    break;

                case EnemyAIValue.State.Flee:
                    enterFleeState();
                    exitState.AddListener(exitFleeState);
                    break;
            }
        }
    }

    private void enterSearchState()
    {

    }
    private void enterRushState()
    {
        createPath();                                                                   //Créer un nouveau path         
    }
    private void enterMoveState()
    {

    }
    private void enterIdleState()
    {
        
    }
    private void enterFleeState()
    {
        //Commencer à déplacer le tank tout de suite
        if(valueScript.hasMouvementScript)
        {
            valueScript.enemyMouvement.refreshTimer();
        }
    }
    private void exitSearchState()
    {
        //Commencer à déplacer le tank tout de suite
        if(valueScript.hasMouvementScript)
        {
            valueScript.enemyMouvement.refreshTimer();
        }

    }
    private void exitRushState()
    {
        //Effacer le path
        _currentWaypointIndex = -1;
        _path = null;
    }
    private void exitMoveState()
    {
        //Commencer à déplacer le tank tout de suite
        if(valueScript.hasMouvementScript)
        {
            valueScript.enemyMouvement.refreshTimer();
        }
    }
    private void exitIdleState()
    {

    }
    private void exitFleeState()
    {

    }
#endregion

#region GETTER/SETTER
    public Vector3 waypointTarget
    {
        get
        {
            if(path!=null && currentWaypointIndex!=-1)
            {
                return path.getWaypointAtIndex(currentWaypointIndex);
            }
            else
            {
                return Vector3.zero;
            }
            
        }
    }
    public int currentWaypointIndex
    {
        get
        {
            return _currentWaypointIndex;
        }
    }
    public Path  path
    {
        get
        {
            return _path;
        }
    }
    public WallVector[] mapWalls
    {
        get
        {
            return _mapWalls;
        }
    }
#endregion
}

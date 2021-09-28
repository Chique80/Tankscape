using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Le script EnemyAIValue.cs contient les variables utilisés par les autres scripts de l'ai. Il contient aussi une référence à chaque autre script faisant
partie de l'AI.
 */

public class EnemyAIValue : MonoBehaviour
{
    public enum State
    {
        Search, 
        Rush, 
        Move, 
        Idle,
        Flee
    }

    public GameObject playerTank;

    [Header("AI")]
    public EnemyAI enemyAI;
    public float minDistanceToWaypoint;

    [Header("Mouvement")]
    public EnemyMouvement enemyMouvement;
    public float mouvementUpdateRate;
    public int nbDirection;
    public float speed;
    public float rotationSpeed;
    
    [Header("Pathfinding")]
    public Pathfinder pathfinder;
    public float pathfindingUpdateRate = 10;
    private float pathfindingUpdateRateMinValue = 10;
    public float waypointsDistanceFromWall;
    public bool useRandomPathing;

    [Header("Wall Detection")]
    public WallDetection wallDetection;
    public float rayonCollision;

    [Header("Projectile Detection")]
    public ProjectileDetection projDetection;
    public float rayonDetectionProjectile;
    public bool predictProjectileCollision;

    [Header("Tank Detection")]
    public TankDetection tankDetection;
    public float rayonDetectionTank;
    public float rayonArret;

    [Header("State & Targeting")]
    public LayerMask terrainLayer;
    public float idleStartRange;
    public float fleeStartRange;
    private State _state;

    [Header("General")]
    public WallManager wallManager;
    public float viewAngle = 25;
    public float viewMinDistance = 1.5f;

    [Header("Toggle Control & Gizmos")]
    public bool showAIGizmos;
    public Color aiGizmosColor;
    public bool showMouvementGizmos;
    public Color mouvementGizmosColor;
    public bool showPathfindingGizmos;
    public Color pathfindingGizmosColor;
    public bool showWallDetectionGizmos;
    public Color wallDetectionGizmosColor;
    public bool showProjectileDetectionGizmos;
    public Color projDetectionGizmosColor;
    public bool showTankDetectionGizmos;
    public Color tankDetectionGizmosColor;


    // Use this for initialization
    void Awake()
    {
    //Vérifier les références aux autres scripts
        //AI
        if(enemyAI == null)
        {
            Debug.LogError(this + " doesn't have a EnemyAI.cs script!");
        }

        //Mouvement
        if(enemyMouvement == null)
        {
            Debug.LogError(this + " doesn't have a EnemyMouvement.cs script!");
        }

        //Pathfinding
        if(pathfinder == null)
        {
            Debug.Log(this + " doesn't have a Pathfinder.cs script!");
        }

        //Wall Detection
        if(wallDetection == null)
        {
            Debug.Log(this + " doesn't have a WallDetection.cs script!");
        }

        //Projectile Detection
        if(projDetection == null)
        {
            Debug.Log(this + " doesn't have a ProjectileDetection.cs script!");
        }

        //Tank Detection
        if(tankDetection == null)
        {
            Debug.Log(this + " doesn't have a TankDetection.cs script!");
        }

        //Wall Manager
        if(wallManager == null)
        {
            if(pathfinder != null)
            {
                Debug.LogError("The WallManager.cs reference is missing. " + this + " requires a WallManager.cs script to function!");
            }
            else
            {
                Debug.Log(this + " doesn't have a reference to a WallManager.cs script!");
            }
        }

    //Corriger les valeurs entrées
        //Pathfinding
        if(pathfindingUpdateRate < pathfindingUpdateRateMinValue)
        {
            pathfindingUpdateRate = pathfindingUpdateRateMinValue;
        }
             
    }

    // Use this for debuging
    void OnDrawGizmosSelected()
    {
        //Draw gizmos for EnemyAI.cs script
        if (showAIGizmos)
        {
            Gizmos.color = aiGizmosColor;

            //Dessiner le rayon de la distance minimal à la cible
            DebugExtension.DrawCircle(this.transform.position, Vector3.up, Gizmos.color, minDistanceToWaypoint);

            //Dessiner les ranges de state
            if(playerTank != null)
            {
                DebugExtension.DrawCircle(playerTank.transform.position, Vector3.up, Color.cyan, idleStartRange);
                DebugExtension.DrawCircle(playerTank.transform.position, Vector3.up, Color.red, fleeStartRange);
            }
        }

        //Draw gizmos for EnemyMouvement.cs script
        if(showMouvementGizmos)
        {
            Gizmos.color = mouvementGizmosColor;
        }

        //Draw gizmos for Pathfinder.cs script
        if(showPathfindingGizmos)
        {
            Gizmos.color = pathfindingGizmosColor;
        }

        //Draw gizmos for WallDetection.cs script
        if (showWallDetectionGizmos)
        {
            Gizmos.color = wallDetectionGizmosColor;

            //Dessiner le rayon de collision
            Vector3 endPoint = positionIn2D;
            endPoint.y = 1f;
            DebugExtension.DrawCylinder(positionIn2D, endPoint, Gizmos.color, rayonCollision);
        }

        //Draw gizmos for ProjectileDetection.cs script
        if(showProjectileDetectionGizmos)
        {
            Gizmos.color = projDetectionGizmosColor;

            //Dessiner le rayon de détection
            DebugExtension.DrawCircle(transform.position, Gizmos.color, rayonDetectionProjectile);
        }

        //Draw gizmos for TankDetection.cs script
        if(showTankDetectionGizmos)
        {
            Gizmos.color = tankDetectionGizmosColor;

            //Dessiner le rayon de détection et d'arrêt
            DebugExtension.DrawCircle(transform.position, Vector3.up, Gizmos.color, rayonDetectionTank);
            DebugExtension.DrawCircle(transform.position, Vector3.up, Gizmos.color, rayonArret);
        }
         

        //Dessiner d'autre truc
        if(playerTank != null)
        {
            //Dessiner un trait vers la cible
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, playerPosition);

            //Dessiner l'angle de vue
            DebugExtension.drawArrow(transform.position, GeneralFunction.rotateVector(playerDirection, viewAngle*Mathf.Deg2Rad)*viewMinDistance, Color.black);
            DebugExtension.drawArrow(transform.position, GeneralFunction.rotateVector(playerDirection, -viewAngle*Mathf.Deg2Rad)*viewMinDistance, Color.black);
        }
    }

#region METHOD
    /// <summary>
    ///     Assinger une nouvelle cible à l'ai. Utiliser principalement durant les niveaux de test.
    /// </summary>
    /// <param name='newPlayerTank'>
    ///     La nouvelle cible de l'ai.
    /// </param>
    public void setNewPlayerTank(GameObject newPlayerTank)
    {
        playerTank = newPlayerTank;
        enemyAI.refreshTimer();
    }

    /// <summary>
    ///     Vérifier si la cible se trouve en vue de l'ai. Pour que la cible soit en vue, aucun mur ne doit se trouve sur
    ///         la ligne entre l'ai et la cible. De plus, si des murs sont trop proche du champ de vision, la cible n'est pas considérée en vue.
    /// </summary>
    /// <returns>
    ///     True si la cible est en vue, false sinon.
    /// </returns>
    public bool isPlayerInSight()
    {
        bool isInSight = false;

        //Vérifier si le joueur est en vue
        if(playerTank != null)
        {
            if(!Physics.Raycast(transform.position, playerDirection, distanceToPlayer, terrainLayer))
            {
                //Vérifier s'il y a des murs dans le champ de vision du tank
                if(!Physics.Raycast(transform.position, GeneralFunction.rotateVector(playerDirection, viewAngle*Mathf.Deg2Rad), viewMinDistance, terrainLayer) &&
                    !Physics.Raycast(transform.position, GeneralFunction.rotateVector(playerDirection, -viewAngle*Mathf.Deg2Rad), viewMinDistance, terrainLayer))
                {
                    isInSight = true;
                }
            }
        }

        return isInSight;
    }

#endregion

#region GETTER/SETTER
    public bool hasAIScript
    {
        get
        {
            return enemyAI != null;
        }
    }
    public bool hasMouvementScript
    {
        get
        {
            return enemyMouvement != null;
        }
    }
    public bool hasWallDetectionScript
    {
        get
        {
            return wallDetection != null;
        }
    }
    public bool hasProjectileDetectionScript
    {
        get
        {
            return projDetection != null;
        }
    }
    public bool hasPathfindingScript
    {
        get
        {
            return pathfinder != null;
        }
    }
    public bool hasWallManager
    {
        get
        {
            return wallManager != null;
        }
    }
    public bool hasTankDetectionScript
    {
        get
        {
            return tankDetection!=null;
        }
    }

    public State state
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;
        }
    }

    public Vector3 positionIn2D
    {
        get
        {
            Vector3 pos = this.transform.position;
            pos.y = 0f;
            return pos;
        }
    }
    public Vector3 playerPosition
    {
        get
        {
            if(playerTank == null)
            {
                return Vector3.zero;
            }
            else
            {
                return playerTank.transform.position;
            }
        }
    }
    public Vector3 playerDirection
    {
        get
        {
            if(playerTank == null)
            {
                return Vector3.zero;
            }
            else
            {
                Vector3 direction = (playerPosition - transform.position).normalized;
                direction.y = 0;
                return direction;
            }
            
        }
    } 
    public float distanceToPlayer
    {
        get
        {
            if(playerTank == null)
            {
                return -1;
            }
            else
            {
                return Vector3.Distance(playerTank.transform.position, transform.position);
            }
        }
    }    

#endregion
}

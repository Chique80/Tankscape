using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDetection : Detection
{
    private const float CLOSE_COLLISION_FLAT_MALUS = 2;
    private const float DISTANT_COLLISION_SCALE_MALUS = 1;
    
    private List<GameObject> detectedWalls;   

    // Use this for initialization
    void Start () 
    {
        //Get the EnemyAIValue.cs script of this enemy
        valueScript = GetComponent<EnemyAIValue>();
        if (valueScript == null)
        {
            Debug.LogError(this + " doesn't have a EnemyAIValue.cs script!");
        }

        //Check if there is an EnemyMouvement.cs script
        if(!valueScript.hasMouvementScript)
        {
            Debug.LogError(this + " requires an EnemyMouvement.cs script!");
        }

        detectedWalls = new List<GameObject>();
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (valueScript.showWallDetectionGizmos)
            {
                Gizmos.color = valueScript.wallDetectionGizmosColor;

                //Show Detected Wall
                for (int i = 0; i < detectedWalls.Count; i++)
                {
                    Gizmos.DrawLine(this.transform.position, detectedWalls[i].transform.position);
                }
            }
        }
    }

#region OVERRIDE
    public override void calculateDirectionsValues()
    { 
        if(valueScript.hasMouvementScript)
        {
            detectedWalls.Clear();

            //Calculer la valeur des directions en fonction des collisions avec les murs
            checkCloseCollision();

            if(valueScript.state==EnemyAIValue.State.Rush || valueScript.state==EnemyAIValue.State.Idle)
            {
                    checkDistantCollision();
            }
            
        }
    }

    public override string ToString()
    {
        return "WallDetection.cs";
    }
#endregion

#region DIRECTION_FUNCTION
    /* Les méthodes dans cette section calcule une valeur pour chaque direction en fonction
        de divers paramètres. Toutes les méthodes sont appelées par calculateDirectionsValues()
     */

    /// <summary>
    ///     Calcule la valeur de chaque direction en fonction des collisions proches avec les murs. La méthode vérifier chaque direction pour
    ///         la distance restant avnt le prochain update du mouvement. La valeur d'une direction est réduite si il mène à un mur.
    /// </summary>
    private void checkCloseCollision()
    {
        Collider[] colliders;                                                                                                           //Liste des collisions pour une direction
        float rayonVerification = valueScript.enemyMouvement.distanceUntilNextUpdate + valueScript.rayonCollision;                      //Rayon de vérification de collision avec les murs

        //Calculer les valeurs pour chaque direction
        for (int i = 0; i<valueScript.nbDirection; i++)
        {
            //Vérifier s'il y a une collision avec un mur dans la direction donnée                                                                   
            colliders = GeneralFunction.projectRectangle(this.transform.position, valueScript.enemyMouvement.directions[i], valueScript.rayonCollision * 2, rayonVerification);
            
            foreach (Collider collider in colliders)
            {
                //Vérifier si la collision est avec un mur
                if (collider.gameObject.GetComponent<Block>() != null)                                                                 
                {
                    //Vérifier si la cible est avant le mur
                    if (!isTargetBeforeWall(collider.gameObject, valueScript.enemyMouvement.directions[i]))                             
                    {
                        directionsValues[i] -= CLOSE_COLLISION_FLAT_MALUS;
                        detectedWalls.Add(collider.gameObject);
                    }
                }
            }
        }
        
    }

    /// <summary>
    ///     Calcule la valeur de chaque direction en fonction des murs éloignés. Chaque direction est réduite d'une valeur inversement proportionel à
    ///         la distance du premier dans cette direction.
    /// </summary>
    private void checkDistantCollision()
    {
        RaycastHit hit;

        for(int i = 0; i<valueScript.nbDirection; i++)
        {
            //Calculer la distance du premier mur dans la direction
            if(Physics.Raycast(transform.position, valueScript.enemyMouvement.directions[i], out hit, Mathf.Infinity, valueScript.terrainLayer))
            {
                float distance = Vector3.Distance(transform.position, hit.point);
                directionsValues[i] -= DISTANT_COLLISION_SCALE_MALUS / distance;
            }

        }
    }
#endregion
 
    /// <summary>
    ///     Vérifier si le prochain mouvement du tank mène à une collision avec un mur
    /// </summary>
    /// <returns>
    ///     True si le prochain mouvement mène à une collision, false sinon.
    /// </returns>
    public bool checkNextMovement()
    {
        bool canMove = true;

        if (valueScript.hasMouvementScript)
        {
            float distanceTravel = (valueScript.speed * Time.fixedDeltaTime) + valueScript.rayonCollision;
            Collider[] colliders = GeneralFunction.projectRectangle(this.transform.position, this.transform.forward, valueScript.rayonCollision * 2, distanceTravel);

            //Vérifier s'il y a une collision avec un mur
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.GetComponent<Block>() != null)
                {
                    canMove = false;
                }
            }
        }

        return canMove;
    }

    /// <summary>
    ///     Vérifier si la cible est avant un mur, pour la direction donnée. Si la cible est avant le mur, le tank peut ignorer la collision
    ///         avec le mur.
    /// </summary>
    /// <param name='wall'>
    ///     Le mur vérifié.
    /// </param>
    /// <param name='movingDirection'>
    ///     La direction vérifié.
    /// </param>
    /// <returns>
    ///     True si la cible est avant le mur, false sinon.
    /// </returns>
    public bool isTargetBeforeWall(GameObject wall, Vector3 movingDirection)
    {
        bool targetIsBeforeWall = false;
        Vector3 target = valueScript.enemyAI.waypointTarget;

        //Vérifier si la cible est dans la direction du déplacement
        Vector3 backRight = this.transform.position + GeneralFunction.rotateVector(movingDirection, Mathf.PI / 2).normalized * valueScript.minDistanceToWaypoint * -1;
        Vector3 backLeft = this.transform.position + GeneralFunction.rotateVector(movingDirection, Mathf.PI / 2).normalized * valueScript.minDistanceToWaypoint;
        Vector3 frontRight = backRight + movingDirection * (valueScript.enemyMouvement.distanceUntilNextUpdate + valueScript.minDistanceToWaypoint);
        Vector3 frontLeft = backLeft + movingDirection * (valueScript.enemyMouvement.distanceUntilNextUpdate + valueScript.minDistanceToWaypoint);
        targetIsBeforeWall = GeneralFunction.isPointWithinPlane(backLeft, backRight, frontLeft, frontRight, target);

        //Vérifier si la target est avant le mur
        if(targetIsBeforeWall)
        {
            float distanceToTarget = Mathf.Abs(Vector3.Distance(target, this.transform.position)) - valueScript.minDistanceToWaypoint;
            float distanceToWall = Mathf.Abs(Vector3.Distance(wall.transform.position, this.transform.position)) - valueScript.rayonCollision - 0.5f;

            if(distanceToTarget > distanceToWall)
            {
                targetIsBeforeWall = false;
            }
        }

        return targetIsBeforeWall;
    }

}

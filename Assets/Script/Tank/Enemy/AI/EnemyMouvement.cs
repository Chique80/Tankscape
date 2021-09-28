using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class EnemyMouvement : MonoBehaviour
{
    private const float RUSH_FORWARD_FLAT_BONUS = 1;
    private const float RUSH_FORWARD_SCALE_BONUS = 0.25f;
    private const float RUSH_PREVIOUS_WAYPOINT_SCALE_MALUS = 0.5f;
    private const float MOVE_FORWARD_SCALE_BONUS = 1;
    private const float IMMOBILE_BASE_VALUE = 1;

    private EnemyAIValue valueScript;

    [Header("Directions")]
    private Vector3 _currentDirection;
    private Vector3[] _directions;
    private float[] _directionsValues;
    private float[] targetDirectionsValues;
    private float _immobileValue;

    [Header("Detection")]
    private List<Detection> detectionScripts;

    [Header("Timer & Updating")]
    private float updateTimer;

    [Header("Toggle Control")]
    public bool canMove;
    public bool showimmobileDirection;

	// Use this for initialization
	void Start ()
    {
        //Get the EnemyAIValue.cs script of this enemy
        valueScript = GetComponent<EnemyAIValue>();
        if (valueScript == null)
        {
            Debug.LogError(this + " doesn't have a EnemyAIValue.cs script!");
        }

        //Get the detection scripts
        detectionScripts = new List<Detection>();
        if (valueScript.hasWallDetectionScript)
        {
            detectionScripts.Add(valueScript.wallDetection);
        }
        if (valueScript.hasProjectileDetectionScript)
        {
            detectionScripts.Add(valueScript.projDetection);
        }
        if(valueScript.hasTankDetectionScript)
        {
            detectionScripts.Add(valueScript.tankDetection);
        }

        //Initialiser directions et valeurs
        _directions = new Vector3[valueScript.nbDirection];
        createDirectionVector();
        initialiseDirectionsValueMatrices();

        //Initialiser la direction actuelle
        _currentDirection = Vector3.zero;

        //Initialiser les timer
        updateTimer = valueScript.mouvementUpdateRate;
    }

    // Update is called once per frame
    private void FixedUpdate()
    { 
        //Update les timer
        if (canMove)
        {
            updateTimer += Time.fixedDeltaTime;
        }

        //Calculer les valeur de chaque direction
        createDirectionVector();                                                    //Dessiner les vecteurs de direction
        calculateDirectionsValues();                                                //Calculer la valeur de chaque direction        

        //Updater la direction du déplacement
        if (updateTimer >= valueScript.mouvementUpdateRate && canMove)
        {
            selectMoveDirection();
            updateTimer = 0;
        }

        //Déplacer le tank
        if (canMove)
        {
            rotate();
            move();
        }
    }

    // Use this for debuging
    void OnDrawGizmosSelected()
    {
        if(Application.isPlaying)
        {
            if (valueScript.showMouvementGizmos)
            {
                Gizmos.color = valueScript.mouvementGizmosColor;

                //Dessiner les directions normales
                for (int i = 0; i < valueScript.nbDirection; i++)
                {
                    if (directionsValues[i] >= 0)
                    {
                        Vector3 arrowDirection = directions[i] * (directionsValues[i] + 1);
                        DebugExtension.drawArrow(this.transform.position, arrowDirection, Color.white);
                    }
                    else
                    {
                        Vector3 arrowDirection = directions[i] * (-directionsValues[i]);
                        DebugExtension.drawArrow(this.transform.position, arrowDirection, Color.red);
                    }
                }

                //Dessiner la direction immobile
                if (showimmobileDirection)
                {
                    if (immobileValue > 0)
                    {
                        DebugExtension.drawArrow(this.transform.position, Vector3.up * immobileValue, Color.white);
                    }
                    else if (immobileValue < 0)
                    {
                        DebugExtension.drawArrow(this.transform.position, Vector3.up * -immobileValue, Color.red);
                    }
                }

                //Dessiner la position du prochain update
                if (distanceUntilNextUpdate != 0)
                {
                    DebugExtension.DrawCircle(this.transform.position, Vector3.up, Gizmos.color, distanceUntilNextUpdate);                          //Draw distance
                    DebugExtension.drawArrow(this.transform.position, this.transform.forward * distanceUntilNextUpdate, Gizmos.color);              //Draw forward direction
                }

                //Draw a ray to the current waypoint
                if (valueScript.enemyAI.waypointTarget != Vector3.zero)
                {
                    Gizmos.DrawLine(this.transform.position, valueScript.enemyAI.waypointTarget);
                }
            }
        }
    }

    /// <summary>
    ///     Réinitialise le timer de l'updateRate du tank. Permet au tank de choisir une nouvelle direction
    ///         à la prochaine frame.
    /// </summary>
    public void refreshTimer()
    {
        updateTimer = valueScript.mouvementUpdateRate;
    }
    
    /// <summary>
    ///     Déplace le tank dans sa direction avant (Transform.foward).
    /// </summary>
    private void move()
    {
        if(currentDirection != Vector3.zero)
        {
            //Vérifier si le mouvement est valide
            bool noWallCollision = true;
            bool noTankCollision = true;

            if(valueScript.hasWallDetectionScript)
            {
                noWallCollision = valueScript.wallDetection.checkNextMovement();
            }
            if(valueScript.hasTankDetectionScript)
            {
                noTankCollision = valueScript.tankDetection.PredictCollision();
            }


            //Move the tank, si le mouvement ne l'emmene pas dans un mur
            if (noWallCollision && noTankCollision)
            {
                transform.Translate(Vector3.forward * valueScript.speed * Time.fixedDeltaTime, Space.Self);
            }
        }
    }

    /// <summary>
    ///     Trouver la base du tank pour qu'elle fasse face à la direction actuel.
    /// </summary>
    private void rotate()
    {        
        Vector3 directionToFace = Vector3.zero;

        //Calculer le vecteur de la direction dans laquel le tank doit faire face
        if(currentDirection != Vector3.zero)
        {
            if(transform.forward != currentDirection)
            {
                directionToFace = currentDirection;
            }
        }
        else if(valueScript.state == EnemyAIValue.State.Idle)
        {
            directionToFace = valueScript.playerDirection;
        }

        //Tourner le tank
        if(directionToFace != Vector3.zero)
        {
            //Calculer les angles polaires des vecteurs de direction
            float angleForward = GeneralFunction.PolarAngleOfVector(transform.forward);
            float angleToFace = GeneralFunction.PolarAngleOfVector(directionToFace);
    
            //Calculer l'angle de rotation
            float angleRotation = Vector3.Angle(transform.forward, directionToFace);
            if (Mathf.Abs(angleRotation) > valueScript.rotationSpeed)
            {
                angleRotation = valueScript.rotationSpeed;
            }

            //Vérifier dans quel direction tourner le tank
            if (angleToFace < angleForward)
            {
                angleRotation *= -1;
            }
            if(Mathf.Abs(angleToFace - angleForward) > 180)
            {
                angleRotation *= -1;
            }        

            //Tourner le tank
            transform.forward = GeneralFunction.rotateVector(transform.forward, angleRotation*Mathf.Deg2Rad);
        }
        
    }

    /// <summary>
    ///     Choisir la direction de déplacement du tank.
    /// </summary>
    private void selectMoveDirection()
    {
        float maxValue = immobileValue;
        int index = -1;

        for(int i = 0; i<directionsValues.Length; i++)
        {
            if(directionsValues[i] > maxValue)
            {
                maxValue = directionsValues[i];
                index = i;
            }
        }

        if(index == -1)
        {
            _currentDirection = Vector3.zero;
        }
        else
        {
            _currentDirection = directions[index];
        }
        
    }

#region DIRECTION FUNCTION
    /// <summary>
    ///     Dessiner les vecteurs de directions du tank. Le premier vecteur est dessiner vers la cible.
    /// </summary>
    private void createDirectionVector()
    {
        Vector3 targetDirection = Vector3.forward;

        //Calculer le vecteur de direction principale
        switch(valueScript.state)
        {
            case EnemyAIValue.State.Rush:
                if(valueScript.hasAIScript && valueScript.enemyAI.waypointTarget!=Vector3.zero)
                {
                    //Vers le waypoint
                    targetDirection = (valueScript.enemyAI.waypointTarget - transform.position).normalized;
                    targetDirection.y = 0;
                }
                break;

            case EnemyAIValue.State.Move:
            case EnemyAIValue.State.Idle:
            case EnemyAIValue.State.Flee:
                //Vers le joueur
                targetDirection = (valueScript.playerPosition - transform.position).normalized;
                targetDirection.y = 0;
                break;

            default:
                targetDirection = Vector3.forward;
                break;
        }

        //Créer les vecteurs de directions
        for (int i = 0; i < directions.Length; i++)
        {
            directions[i] = GeneralFunction.rotateVector(targetDirection, (2 * Mathf.PI * i / valueScript.nbDirection));
        }
    }

    /// <summary>
    ///     Initialiser les tableaux contenant les valeurs de chaque direction. Les tableaux de chaque script de détection sont
    ///         aussi initialisés.
    /// </summary>
    private void initialiseDirectionsValueMatrices()
    {
        _directionsValues = new float[valueScript.nbDirection];
        targetDirectionsValues = new float[valueScript.nbDirection];

        foreach(Detection detectionScript in detectionScripts)
        {
            detectionScript.initialiseDirectionValue(valueScript.nbDirection);
        }
    }

    /// <summary>
    ///     Calculer les valeurs de chaque direction. Appelle aussi les méthodes de chaque script de détection pour calculer les 
    ///         valeurs de directions.
    /// </summary>
    private void calculateDirectionsValues()
    {
        resetDirectionsValues();                                                            //Réinitialiser les valeurs de direction

        //Calcule les valeurs de direction en fonction de la cible
        calulateForTargetDirection();
        if(valueScript.state == EnemyAIValue.State.Rush)
        {
            calculateForPreviousWaypoints();
        }
        

        //Calculer les valeurs de direction pour tous les scripts de détection
        for (int i = 0; i < detectionScripts.Count; i++)
        {
            detectionScripts[i].calculateDirectionsValues();
        }

        //Calculer la valeur de la direction immobile
        switch(valueScript.state)
        {
            case EnemyAIValue.State.Search:
            case EnemyAIValue.State.Idle:
                immobileValue += IMMOBILE_BASE_VALUE;
                break;

            case EnemyAIValue.State.Rush:
                if(valueScript.enemyAI.path != null)
                {
                    immobileValue += IMMOBILE_BASE_VALUE * -2;
                }
                else
                {
                    immobileValue += IMMOBILE_BASE_VALUE;
                }
                
                break;
        }

        //Additionner toutes les valeurs de direction
        for (int i = 0; i < directionsValues.Length; i++)
        {
            _directionsValues[i] = targetDirectionsValues[i];

            for (int j = 0; j < detectionScripts.Count; j++)
            {
                _directionsValues[i] += detectionScripts[j].directionsValues[i];
            }
        }
    }

    /// <summary>
    ///     Réinitiliase la valeur de chaque direction à 0. Appelle aussi la méthode dans les scripts de détection.
    /// </summary>
    private void resetDirectionsValues()
    {
        //Reset targetDirectionsValues
        for (int i = 0; i < targetDirectionsValues.Length; i++)
        {
            targetDirectionsValues[i] = 0f;
        }

        //Reset DirectionsValues dans les scripts de détection
        for (int i = 0; i < detectionScripts.Count; i++)
        {
            detectionScripts[i].resetDirectionValue();
        }

        //Reset la direction immobile
        immobileValue = 0;
    }

    /// <summary>
    ///     Calculer la valeur de chaque direction en fonction de la direction de la cible.
    /// </summary>
    private void calulateForTargetDirection()
    {
        Vector3 targetDirection = Vector3.zero;

        //Caluler la direction de la cible
        switch(valueScript.state)
        {
            case EnemyAIValue.State.Rush:
                if(valueScript.hasAIScript && valueScript.enemyAI.waypointTarget!=Vector3.zero)
                {
                    targetDirection = (valueScript.enemyAI.waypointTarget - transform.position).normalized;
                    targetDirection.y = 0;
                }
                break;

            case EnemyAIValue.State.Move:
            case EnemyAIValue.State.Flee:
                targetDirection = valueScript.playerDirection;
                break;

            default:
                targetDirection = Vector3.zero;
                break;
        }

        //Calculer la valeur des directions
        if(targetDirection != Vector3.zero)
        {
            for(int i = 0; i<directions.Length; i++)
            {
                float angle = Vector3.Angle(targetDirection, directions[i]) * Mathf.Deg2Rad;                    //Angle entre la direction et la cible (en radians)

                switch(valueScript.state)
                {
                    case EnemyAIValue.State.Rush:
                        targetDirectionsValues[i] += RUSH_FORWARD_FLAT_BONUS * Mathf.Sign(Mathf.Cos(angle));
                        targetDirectionsValues[i] += RUSH_FORWARD_SCALE_BONUS * Mathf.Cos(angle);
                        break;

                    case EnemyAIValue.State.Move:
                        targetDirectionsValues[i] += MOVE_FORWARD_SCALE_BONUS * Mathf.Cos(angle);
                        break;

                    case EnemyAIValue.State.Flee:
                        targetDirectionsValues[i] += MOVE_FORWARD_SCALE_BONUS * Mathf.Cos(angle) * -1;
                        break;
                }
            }
        }
    }
    
    /// <summary>
    ///     Calculer la valeur de chaque direction en fonction de la direction du waypoint précédent.
    /// </summary>
    private void calculateForPreviousWaypoints()
    {
        if(valueScript.hasAIScript && valueScript.enemyAI.path != null && valueScript.enemyAI.currentWaypointIndex>=2)
        {
            //Calculer la distance au waypoint précédent
            Vector3 previousWaypoint = valueScript.enemyAI.path.getWaypointAtIndex(valueScript.enemyAI.currentWaypointIndex-1);
            float distanceToPreviousWaypoint = Vector3.Distance(transform.position, previousWaypoint);

            //Vérifier si le tank se trouve sur le waypoint précédent
            if(distanceToPreviousWaypoint <= valueScript.minDistanceToWaypoint)
            {
                //Réduire les directions vers le waypoint précédent au waypoint précédent
                for(int i = 0; i<directions.Length; i++)
                {
                    //Calculer l'angle entre la direction et le waypoint précédent
                    previousWaypoint = valueScript.enemyAI.path.getWaypointAtIndex(valueScript.enemyAI.currentWaypointIndex-2);
                    Vector3 directionToPreviousWaypoint = (previousWaypoint - transform.position).normalized;
                    float angle = Vector3.Angle(directionToPreviousWaypoint, directions[i]) * Mathf.Deg2Rad;                                          

                    //Réduire la valeur de la direction en fonction de l'angle
                    targetDirectionsValues[i] -= RUSH_PREVIOUS_WAYPOINT_SCALE_MALUS * Mathf.Cos(angle);
                        
                }
            }
        }
        
    }
#endregion

#region GETTER/SETTER
    public float distanceUntilNextUpdate
    {
        get
        {
            float remainingTimeUntilNextUpdate = valueScript.mouvementUpdateRate - updateTimer;
            if (remainingTimeUntilNextUpdate <= 0)
            {
                remainingTimeUntilNextUpdate = valueScript.mouvementUpdateRate;                                         //Lors de l'update, le temps restant est calculé pour le prochain update
            }

            return remainingTimeUntilNextUpdate * valueScript.speed;
        }
    }
    public Vector3 currentDirection
    {
        get
        {
            return _currentDirection;
        }
    }
    public Vector3[] directions
    {
        get
        {
            return _directions;
        }
    }
    public float[] directionsValues
    {
        get
        {
            return _directionsValues;
        }
    }
    public float immobileValue
    {
        get
        {
            return _immobileValue;
        }
        set
        {
            _immobileValue = value;
        }
    }
#endregion

}


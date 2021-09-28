using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankDetection : Detection
{
    private const float IMMOBILE_TANK_FLAT_MALUS = 1.5f;
    private List<GameObject> detectedTanks;

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
            Debug.LogError(this + " doesn't have an EnemyMouvement.cs script!");
        }

        detectedTanks = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        detectedTanks.Clear();

		//Find the other tanks in range
        Collider[] colliders = Physics.OverlapCapsule(valueScript.positionIn2D, valueScript.positionIn2D, valueScript.rayonDetectionTank);
        foreach(Collider collider in colliders)
        {
            if(collider.gameObject.GetComponent<EnemyMain>() != null)
            {
                detectedTanks.Add(collider.gameObject);
            }
        }
	}

    void OnDrawGizmosSelected()
    {
        if(Application.isPlaying)
        {
            if(valueScript.showTankDetectionGizmos)
            {
                Gizmos.color = valueScript.tankDetectionGizmosColor;

                //Show tanks
                foreach(GameObject tank in detectedTanks)
                {
                    Gizmos.DrawLine(transform.position, tank.transform.position);

                    //Dessiner le foward du tank
                    DebugExtension.drawArrow(tank.transform.position, tank.transform.forward*3, Gizmos.color);
                }

            }
        }
    }       

#region OVERRIDE
    public override void calculateDirectionsValues()
    {
        if(valueScript.hasMouvementScript)
        {
            checkForCollision();
        }
    }

    public override string ToString()
    {
        return "TankDetection.cs";
    }
#endregion

#region DIRECTION_FUNCTION
    /* Les méthodes dans cette section calcule une valeur pour chaque direction en fonction
        de divers paramètres. Toutes les méthodes sont appelées par calculateDirectionsValues()
    */

    /// <summary>
    ///     Calcule la valeur de chaque direction en fonction de possible collisions avec d'autre tank. La méthode ne prend en compte
    ///         que les tanks immobiles et ceux dont le déplacement est parallèle au tank.
    /// <summary>
    private void checkForCollision()
    {
        Collider[] colliders;

        //Vérifier chaque direction
        for(int i = 0; i<valueScript.nbDirection; i++)
        {
            //Vérifier s'il y a des tanks dans la direction
            colliders = GeneralFunction.projectRectangle(transform.position, valueScript.enemyMouvement.directions[i], valueScript.rayonCollision * 2, valueScript.rayonDetectionTank);
            foreach(Collider collider in colliders)
            {
                if(collider.GetComponent<EnemyMain>()!=null && collider.gameObject!=this.gameObject)
                {
                    EnemyAIValue tankValueScript = collider.GetComponent<EnemyAIValue>();
                    if(tankValueScript != null && tankValueScript.hasMouvementScript)
                    {

                        //Vérifier si le tank est immobile ou s'il se déplace parallèlement à la direction
                        if(tankValueScript.state==EnemyAIValue.State.Idle || tankValueScript.enemyMouvement.currentDirection==Vector3.zero)
                        {
                            directionsValues[i] -= IMMOBILE_TANK_FLAT_MALUS;
                        }
                        //Vérifier si le tank se déplace parallèlement à la direction
                        else
                        {
                            //Calculer l'angle entre la direction du tank et la direction
                            Vector3 selfDirection = valueScript.enemyMouvement.directions[i];
                            Vector3 otherDirection = tankValueScript.enemyMouvement.currentDirection;
                            float angle = Vector3.Angle(selfDirection, otherDirection);

                            if(angle <= 10 || angle>=170)
                            {
                                directionsValues[i] -= IMMOBILE_TANK_FLAT_MALUS;
                            }
                        }
                    }
                }
            }
        }
    }

#endregion
 
    /// <summary>
    ///     Vérifier le prochain mouvement du tank, et les collisions possibles avec d'autre tank.
    /// </summary>
    /// <returns>
    ///     True si il va y avoir une collision, false sinon.
    /// </returns>
    public bool PredictCollision()
    {
        bool canMove = true;

        //Calculer la droite de direction du tank
        Vector2 selfPosition2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 selfFoward = new Vector2(transform.forward.x, transform.forward.z);
        Droite2D droiteSelf = new Droite2D(selfPosition2D, selfFoward);

        //Vérifier les directions de chaque tank
        foreach(GameObject otherTank in detectedTanks)
        {
            Vector3 intersection = Vector3.zero;            

            //Calculer la droite de la direction de l'autre tank
            Vector2 otherPosition2D = new Vector2(otherTank.transform.position.x, otherTank.transform.position.z);
            Vector2 otherFoward = new Vector2(otherTank.transform.forward.x, otherTank.transform.forward.z);
            Droite2D droiteOther = new Droite2D(otherPosition2D, otherFoward);

            //Trouver le point d'intersection
            Vector2 intersection2D;
            if(Droite2D.Intersect(droiteSelf, droiteOther, out intersection2D))
            {
                //Calculer le point d'intersection 3D
                intersection.x = intersection2D.x;
                intersection.y = transform.position.y;
                intersection.z = intersection2D.y;

                //Calculer la distance au point d'intersection
                float distanceSelf = Vector3.Distance(transform.position, intersection);
                float distanceOther = Vector3.Distance(otherTank.transform.position, intersection);

                //Vérifier si le point d'intersection est devant
                Vector3 directionToIntersection = intersection - transform.position;
                if(Vector3.Angle(transform.forward, directionToIntersection) == 0)
                {
                    //Vérifier si la collision est avant le prochain update
                    if(distanceSelf <= valueScript.rayonArret)
                    {

                        //Le tank le plus loin du point de collision doit s'arrêter
                        if(distanceSelf < distanceOther)
                        {
                            canMove = true;
                        }
                        else if(distanceSelf > distanceOther)
                        {
                            canMove = false;
                        }
                    }
                }
            }
        }

        return canMove;
    }
}

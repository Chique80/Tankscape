using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDetection : Detection
{
    private const float DIRECT_COLLISION_SCALE_BONUS = 5;
    private const float IMMOBILE_DIRECTION_COLLISION_SCALE_MALUS = 3;    
    private const float TARGET_LOOK_DIRECTION_FLAT_BONUS = 1.2f;

    private GameObject[] detectedProjs;
   
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
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (valueScript.showProjectileDetectionGizmos)
            {
                Gizmos.color = valueScript.projDetectionGizmosColor;

                foreach (GameObject proj in detectedProjs)
                {
                    if (proj != null)
                    {
                        //Dessiner une line vers le projectile
                        Gizmos.DrawLine(transform.position, proj.transform.position);

                        //Dessiner la direction du projectile
                        Gizmos.DrawRay(proj.transform.position, proj.GetComponent<Projectile_Mouvement>().projectileDirection * 30);
                    }
                }
            }
        }
    }

#region OVERRIDE
    public override void calculateDirectionsValues()
    {
        detectedProjs = detectProjectile();                                 //Get all the projectile within the detection range of the tank

        if(valueScript.hasMouvementScript)
        {
            //Calculer les valeurs des directions en fonction des collisions avec les projectiles
            checkDirectCollision();
        }
        
    }

    public override string ToString()
    {
        return "ProjectileDetection.cs";
    }
#endregion

#region DIRECTION_FUNCTION
    /* Les méthodes dans cette section calcule une valeur pour chaque direction en fonction
        de divers paramètres. Toutes les méthodes sont appelées par calculateDirectionsValues()
     */

    /// <summary>
    ///     Calculer la valeur de chaque direction en fonction des collisions directes avec les projectiles.
    ///         La méthode ne prend en compte que les projectiles qui se dirige droit vers le tank.
    /// </summary>
    private void checkDirectCollision()
    {
        foreach (GameObject proj in detectedProjs)
        {
            //Vérifier si le tank se trouve sur la direction d'un projectile
            if (isOnProjectileDirection(proj))
            {
                Vector3 projectileDirection = proj.GetComponent<Projectile_Mouvement>().projectileDirection;

                //Calculer la distance du projectile
                float distance = Vector3.Distance(transform.position, proj.transform.position);

                //Augmenter la valeur des directions pour éviter le projectile
                for(int i = 0; i<directionsValues.Length; i++)
                {
                    //Calculer l'angle entre la direction du projectile et la direction
                    float angle = Vector3.Angle(-projectileDirection, valueScript.enemyMouvement.directions[i]) * Mathf.Deg2Rad;
                    
                    //Ne pas considérer les angles trop petits
                    if(angle >= (45*Mathf.Deg2Rad) && angle <= (135*Mathf.Deg2Rad))
                    {
                        directionsValues[i] += Mathf.Sin(angle) * (DIRECT_COLLISION_SCALE_BONUS / distance);
                    }
                }

                //Réduire la valeur de la direction immobile
                valueScript.enemyMouvement.immobileValue -= IMMOBILE_DIRECTION_COLLISION_SCALE_MALUS / distance;
            }
        }
    }

#endregion
 
    /// <summary>
    ///     Détecter tous les projectiles proche du tank.
    /// </summary>
    /// <returns>
    ///     La liste de tous les projectiles détectés.
    /// </returns>
    private GameObject[] detectProjectile()
    {
        List<GameObject> projectilesDetected = new List<GameObject>();

        Collider[] colliders = Physics.OverlapCapsule(valueScript.positionIn2D, valueScript.positionIn2D, valueScript.rayonDetectionProjectile);
        foreach(Collider collider in colliders)
        {
            if(collider.gameObject.GetComponent<Projectile_Mouvement>() != null)
            {
                projectilesDetected.Add(collider.gameObject);
            }
        }

        return projectilesDetected.ToArray();
    }

    /// <summary>
    ///     Vérifier si le tank est sur la direction d'un projectile.
    /// </summary>
    /// <param name='proj'>
    ///     Le projectile qui est vérifié.
    /// </param>
    /// <returns>
    ///     True, si le tank est sur la direction du projectile. False, sinon.
    /// </returns>
    private bool isOnProjectileDirection(GameObject proj)
    {
        bool isOnDirection = false;
        RaycastHit hit;

        Vector3 projDirection = proj.GetComponent<Projectile_Mouvement>().projectileDirection.normalized;
        
        if(Physics.Raycast(proj.transform.position, projDirection, out hit, Mathf.Infinity))
        {
            if(hit.collider.gameObject == this.gameObject)
            {
                isOnDirection = true;
            }
        }

        return isOnDirection;
    }
}

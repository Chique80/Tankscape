using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateProjectile : MonoBehaviour {
    
    private Transform body;
    private Transform gun;
    

    void Start ()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>();

        if(transforms == null)
        {
            Debug.LogError(gameObject.name + " n'a pas trouvé les Transforms de ses enfants.");
        }
 
        
        foreach(Transform transform in transforms)
        {
            //Si le transform est le body, initialise la variable body
            if(transform.gameObject.name == "Body" || transform.gameObject.name == "body")
            {
                body = transform;

                Transform[] bodyTransforms = body.GetComponentsInChildren<Transform>();

                if (transforms == null)
                {
                    Debug.LogError(gameObject.name + " n'a pas trouvé les Transforms de ses enfants.");
                }

                foreach (Transform  bodyTransform in bodyTransforms)
                {
                    //Si le transform est le gun, initialise la variable gun
                    if (bodyTransform.gameObject.name == "Gun" || bodyTransform.gameObject.name == "gun")
                    {
                        gun = bodyTransform;
                    }
                }
            }
        }
    }
	

    /// <summary>
    /// Permet de tirer si la méthode est appelée
    /// </summary>
    /// <param name="projectile"> Le prefab du projectile à tirer </param>
    public void tirer(GameObject projectile)
    {
        Collider collider = GetComponent<Collider>();
        float projectileSize;
        float spawnDistance;

        Vector3 max;
        Vector3 gunPos;
        Vector3 spawnPos;
        Vector3 directionToFace;
        

        if (collider != null)
        {
            //Trouver la taille du projectile
            projectileSize = projectile.GetComponent<Collider>().bounds.extents.z;

            if (projectile.GetComponent<Collider>() == null)
            {
                Debug.LogError(projectile.name + " n'a pas de collider.");
            }
            else
            {
                //Trouver la position la plus éloignée
                max = collider.bounds.max;
                max.y = gun.position.y;

                //Trouver la position du gun
                gunPos = transform.position;
                gunPos.y = gun.position.y;

                //Calculer la position de création
                spawnDistance = Vector3.Distance(max, gunPos) + projectileSize + 0.1f;                                   //Caluler la distance de création
                spawnPos = gunPos - (body.forward * spawnDistance);                                                     //Calculer la position de création

                directionToFace = body.position - spawnPos;                                                             //Calcul du vecteur direction du projectile

                //Créer le projectile
                projectile = Instantiate(projectile, spawnPos, Quaternion.LookRotation(directionToFace));
            }

            
        }
    }

    
    /// <summary>
    /// Gizmos pour debug
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if(Application.isPlaying)
        {
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                //Trouver la position la plus éloignée
                Vector3 max = collider.bounds.max;
                max.y = gun.position.y;

                //Trouver la position du gun
                Vector3 gunPos = transform.position;
                gunPos.y = gun.position.y;

                //Calculer la position de création
                float spawnDistance = Vector3.Distance(max, gunPos) + 0.1f;                                             //Distance de création
                Vector3 spawnPos = gunPos - (body.forward * spawnDistance);                                             //Position de création
                
                DebugExtension.DrawPoint(spawnPos, Color.black);

            }
            
        }
    }
}



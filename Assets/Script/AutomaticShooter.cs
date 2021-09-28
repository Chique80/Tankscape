using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Cette classe est utilisé pour teste l'ai des tanks. Elle permet de créer un objet qui tire automatiquement sur
    une cible, si la cible est en vue de l'objet.
 */

public class AutomaticShooter : MonoBehaviour
{

    [Header("Stats")]
    public float fireRate;
    private float fireTimer;

    [Header("Component")]
    public GameObject projectile;
    public GameObject target;


	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(target != null)
        {
            //Update les timers
            fireTimer += Time.fixedDeltaTime; 
            
            if(fireTimer >= fireRate)
            {
                if(shoot())
                {
                    fireTimer = 0;
                }
            }
        }
        else
        {
            fireTimer = 0f;
        }
        
	}

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        //Draw a circle
        DebugExtension.DrawCircle(transform.position, Vector3.up, Color.white);

        if (target != null)
        {
            //Draw a ray to the target
            Gizmos.DrawLine(transform.position, target.transform.position);

            //Draw a circle around the target
            DebugExtension.DrawCircle(target.transform.position, Vector3.up, Color.white);
        }
    }

    private bool shoot()
    {
        bool hasShoot = false;

        if(projectile != null)
        {
            if(GetComponent<CreateProjectile>() == null)
            {
                if(isTargetInSight())
                {
                    Vector3 directionToTarget = transform.position - target.transform.position;
                    Instantiate(projectile, transform.position, Quaternion.LookRotation(directionToTarget));

                    hasShoot = true;
                }
            }
            else
            {
                if(isTargetInSight())
                {
                    GetComponent<CreateProjectile>().tirer(projectile);
                    hasShoot = true;
                }
                
            }
        }

        return hasShoot;
    }

    private bool isTargetInSight()
    {
        bool isInSight = false;

        RaycastHit hit;
        Vector3 directionToTarget = target.transform.position - transform.position;

        if(Physics.Raycast(transform.position, directionToTarget, out hit, directionToTarget.magnitude))
        {
            isInSight = hit.collider.gameObject == target;
        }

        return isInSight;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingTank : EnemyBehavior
{
    protected EnemyAIValue valueScript;
    protected GameObject target;

    [Header("Shooting")]
    public float fireRate;
    protected float fireTimer;

    [Header("Component")]
    public GameObject projectile;
    protected CreateProjectile shoot;
    protected GameObject turret;

    [Header("Toggle & Control")]
    public bool canShoot = true;


    // Use this for initialization
    void Start ()
    {
        //Find the CreateProjectile.cs script
        shoot = GetComponent<CreateProjectile>();
        if(shoot == null)
        {
            Debug.LogError(this + " requires a CreateProjectile.cs script to function!");
        }

        //Find the EnemyAIValue.cs script
        valueScript = GetComponent<EnemyAIValue>();
        if(valueScript == null)
        {
            Debug.LogError(this + " doesn't have a EnemyAIValue.cs script!");
        }

        //Trouver la tourelle du tank
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach (Transform transform in transforms)
        {
            if (transform.gameObject.name == "body" || transform.gameObject.name == "Body")
            {
                turret = transform.gameObject;
            }
        }
        if(turret == null)
        {
            Debug.Log("Le tank ne possède pas de tourelle!");
        }

        //Vérifier que le script possède un prefab de projectile
        if(projectile == null)
        {
            Debug.LogError(this + " requires a GameObject projectile to shoot!");
        }

        //Initialiser le timer
        fireTimer = 0;
	}

    void OnDrawGizmosSelected()
    {
        //Calculer le point d'origine du ray
        Vector3 boundsMax = GetComponent<Collider>().bounds.max;
        boundsMax.y = turret.transform.position.y;
        boundsMax = boundsMax - turret.transform.position;
        Vector3 rayOrigin = turret.transform.position + (targetDirection.normalized * boundsMax.magnitude);
        float viewDistance = valueScript.viewMinDistance - Vector3.Distance(turret.transform.position, rayOrigin);

        DebugExtension.DrawCapsule(rayOrigin, rayOrigin, Color.yellow, viewDistance);
    }

#region OVERRIDE
    public override void enable()
    {
        this.enabled = true;
    }

    public override void disable()
    {
        this.enabled = false;
    }
#endregion

    /// <summary>
    ///     Tirer sur la cible. La méthode vérifier si le cible est en vue avant de tirer.
    /// </summary>
    /// <returns>
    ///     True si le tank a tiré, false sinon.
    /// </returns>
    protected bool shootTarget()
    {
        bool hasShoot = false;

        if (canShoot && isTargetInSight())
        {
            Debug.Log("Target is in sight");
            shoot.tirer(projectile);
            hasShoot = true;
        }
        else
        {
            Debug.Log("Target is not in sight");
        }

        return hasShoot;
    }

    /// <summary>
    ///     Tourner la tourelle du tank pour qu'elle fasse face à la cible. Ne modifier l'orientation verticale de la tourelle.
    /// </summary>
    protected virtual void rotateTurret()
    {
        turret.transform.forward = -targetDirection;
    }
 
    /// <summary>
    ///     Vérifier si la cible est en vue. Une cible est en vue si aucun mur ne se trouve sur la direction entre le cible et
    ///         et le tank.
    /// </summary>
    /// <returns>
    ///     True si la cible est en vue, false sinon.
    /// </returns>
    protected bool isTargetInSight()
    {
        bool isTargetInSight = true;

        if(valueScript != null && target != null && turret != null)
        {
            RaycastHit hit;

            //Calculer le point d'origine du ray
            Vector3 boundsMax = GetComponent<Collider>().bounds.max;
            boundsMax.y = turret.transform.position.y;
            boundsMax = boundsMax - turret.transform.position;
            Vector3 rayOrigin = turret.transform.position + (targetDirection.normalized * boundsMax.magnitude);

            //Vérifier si la target est en vue
            if(Physics.Raycast(rayOrigin, targetDirection, out hit, Vector3.Distance(rayOrigin, targetPosition)))
            {
                //Vérifier qu'il n'y a pas de collision avec le projectile
                while(hit.collider.GetComponent<Projectile_Mouvement>()!=null && hit.collider.gameObject!=target)
                {
                    //Calculer le point d'origine d'un ray à partir du projectile
                    Vector3 projBoundsMax = hit.collider.bounds.max;
                    projBoundsMax.y = hit.collider.transform.position.y;
                    projBoundsMax = projBoundsMax - hit.collider.transform.position;
                    Vector3 projRayOrigin = hit.transform.position + (targetDirection.normalized * projBoundsMax.magnitude);

                    Physics.Raycast(projRayOrigin, targetDirection, out hit, Vector3.Distance(projRayOrigin, targetPosition));
                }
            
                if(hit.collider.gameObject != target)
                {
                    Debug.Log(hit.collider.gameObject + " is in front of target");
                    isTargetInSight = false;
                }
        /*      
                else
                {
                    Debug.Log("Check view range");
                    float viewDistance = valueScript.viewMinDistance - Vector3.Distance(turret.transform.position, rayOrigin);

                    //Vérifier la présence d'autre objet dans le cone de vision
               
                    if(Physics.Raycast(rayOrigin, GeneralFunction.rotateVector(targetDirection, valueScript.viewAngle*Mathf.Deg2Rad), out hit, viewDistance))
                    {
                        if((hit.collider.gameObject.GetComponent<Block>()!=null || hit.collider.gameObject.GetComponent<TankMain>()!=null) &&
                            (hit.collider.gameObject!=target && hit.collider.gameObject!=this.gameObject))
                        {
                            Debug.Log(hit.collider.gameObject + "is in view range");
                            isTargetInSight = false;
                        }
                    }
                    if(Physics.Raycast(rayOrigin, GeneralFunction.rotateVector(targetDirection, -valueScript.viewAngle*Mathf.Deg2Rad), out hit, viewDistance))
                    {
                        if((hit.collider.gameObject.GetComponent<Block>()!=null || hit.collider.gameObject.GetComponent<TankMain>()!=null) &&
                            (hit.collider.gameObject!=target && hit.collider.gameObject!=this.gameObject))
                        {
                            Debug.Log(hit.collider.gameObject + "is in view range");
                            isTargetInSight = false;
                        }
                    }
            
                    Collider[] colliders = GeneralFunction.OverlapCone(rayOrigin, targetDirection, viewDistance, valueScript.viewAngle*2);
                    foreach(Collider collider in colliders)
                    {
                        if(collider.gameObject != target && collider.gameObject != this.gameObject)
                        {
                            if(collider.gameObject.GetComponent<TankMain>() != null)
                            {
                                Debug.Log(collider.gameObject + "is in view range");
                                isTargetInSight = false;
                            }
                            else if(collider.gameObject.GetComponent<Block>() != null)
                            {
                                if(collider.gameObject.GetComponent<Block>().blocksProjectile)
                                {
                                    Debug.Log(collider.gameObject + "is in view range");
                                    isTargetInSight = false;
                                }
                            }
                        }
                    }
                  
                }
        //*/
            }
            else
            {
                isTargetInSight = false;
            }
        }
        else
        {
            isTargetInSight = false;
        }

        return isTargetInSight;
    }

#region GETTER/SETTER
    public Vector3 targetPosition
    {
        get
        {
            Vector3 targetPosition = Vector3.zero;

            if(target != null)
            {
                targetPosition = target.transform.position;
            }
            
            return targetPosition;
        }
    }
    public Vector3 targetDirection
    {
        get
        {
            Vector3 targetDirection = transform.forward;

            if(targetPosition != Vector3.zero)
            {
                targetDirection = targetPosition - turret.transform.position;
                targetDirection.y = 0;
            }

            return targetDirection;
        }
    }
#endregion
}

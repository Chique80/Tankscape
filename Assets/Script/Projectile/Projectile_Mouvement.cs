using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Mouvement : MonoBehaviour {

    /*VARIALBES*/
    /*PUBLIC*/
    [Header("Info")]
    public float speedProjectile;
    public int nombreRebond;

    [Header("Components")]
    public GameObject bulletImpact;
    public GameObject explosion;

    /*PRIVATE*/
    private Rigidbody rb;
    private Vector3 newRotation;
    private Vector3 _projectileDirection;
    private bool estPossibleRebondir = true;
    
 
    void Start () {
        
        //Initialise les variables
        rb = transform.GetComponent<Rigidbody>();

        //Rotate le projectile pour aller dans la bonne direction
        newRotation = transform.localEulerAngles;               
        newRotation.x -= 90;
        transform.localEulerAngles = newRotation;

        
        if(rb != null)
        {
            
            rb.constraints = RigidbodyConstraints.FreezeRotation;   //Empêche de rotater
            _projectileDirection = rb.transform.up;                 //Initialise la direction du projectile
        }
        else
        {
            Debug.Log(gameObject.name + " has no RigidBody");
        }
        


       

    }

    void FixedUpdate ()
    {
        //Bouge le projectile
        ChangePosition();
    }

    /// <summary>
    /// Lorsqu'une collision est détecté
    /// </summary>
    /// 
    /// <param name="collision"> L'information sur le point de collision </param>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Block>())
        {
            CollisionBlock(collision);
        }
        else if (collision.gameObject.GetComponent<Projectile_Mouvement>())
        {
            CollisionProjectile();
        }
        else if (collision.gameObject.GetComponent<PlayerMain>() || collision.gameObject.GetComponent<EnemyMain>() || collision.gameObject.GetComponent<TankMain>())
        {
            CollisionTank();
        }
        else
        {
            Debug.LogError("La collision s'est fait avec un collider inconnu.");
        }
    }
    
    /// <summary>
    /// Permet de bouger le projectile dans la direction qu'il regarde
    /// </summary>
    /// 
    private void ChangePosition()
    {
        //Valeur à ajouter au vecteur position
        float valeurProjectionX;
        float valeurProjectionZ;

        //Angle par rapport au axe du World
        float angleX;   //Axe des x
        float angleZ;   //Axe des z

        //Position finale à assigner au projectile
        Vector3 position;

        //Calcul des angles
        angleX = Vector3.Angle(Vector3.right, projectileDirection);
        angleZ = Vector3.Angle(Vector3.forward, projectileDirection);

        //Pour enlever les angles du 3e et 4e quadrant en trouvant son équivalent dans le 1er et 2e quadrant
        if (angleZ > 90)
        {
            angleX = -angleX;
        }

        //Transforme les angles en radians
        angleX = Mathf.Deg2Rad * angleX;
        angleZ = Mathf.Deg2Rad * angleZ;
        
        //Calcul les valeurs des projections
        valeurProjectionX = Mathf.Cos(angleX);
        valeurProjectionZ = Mathf.Sin(angleX);

        //Trouve la position dans l'espace
        position = transform.position;
        position.x += valeurProjectionX * speedProjectile * Time.deltaTime;
        position.z += valeurProjectionZ * speedProjectile * Time.deltaTime;

        //Assigne la position au projectile
        transform.position = position;

    }
    
#region REBOND
    /// <summary>
    /// Fait rebondir le projectile lorsquil y a un contact avec un mur
    /// </summary>
    /// 
    /// <param name="collision"> Information sur la collision </param>
    private void Rebondir(Collision collision)
    {
        //Nouveau vecteur après rebond
        Vector3 nouveauVecteur;

        //Variable avant la collision (si le projectile à un rebond perpendiculaire)
        Vector3 positionDebutRebond = transform.position;
        Vector3 projectileDirectionInitial = _projectileDirection;


        //Information sur le hit du Raycast vers le mur
        RaycastHit hitInfo = findHitPointWithRay(positionDebutRebond, projectileDirectionInitial, collision.contacts[0].point, collision.contacts[0].normal);

        //Calcul du nouveau vecteur
        nouveauVecteur = CalculNouveauVecteur(hitInfo.normal, -_projectileDirection);
        

        
        if (Vector3.zero == nouveauVecteur)                                     //Verifie si la collision s'est bien fait (que le projectile n'a pas spawn dans un bloc)
        {
            Destroy(gameObject);
        }
        else if (verifierSiRebondPerpendiculaire(hitInfo, nouveauVecteur))      //Vérifie si le projectile rebondi dans un coin
        {
            repositionner(-projectileDirectionInitial, positionDebutRebond);
        }
        else                                                                    //C'est un rebond normal
        {

            //Reposition le projectile sur le point de contact
            repositionner(nouveauVecteur, collision.contacts[0].point);
            
            //Sort le projectile du mur
            deplacerHorsMur(collision.collider);
        }

        //Réinitialise la possibilité de rebondir
        estPossibleRebondir = true;
    }

    /// <summary>
    /// Trouve le point de contact du projectile
    /// </summary>
    /// 
    /// <param name="positionDebutRebond"> Position au début du rebond </param>
    /// <param name="projectileDirectionInitial"> Direction au début du rebond </param>
    /// <param name="pointContactInitial"> Position du point de contact trouvé par la collision initiale </param>
    /// <param name="directionNormalCollision"> Direction normal trouvé par la collision initiale </param>
    /// <returns>  Retourne l'information sur ce point de contact </returns>
    private RaycastHit findHitPointWithRay(Vector3 positionDebutRebond,Vector3 projectileDirectionInitial,Vector3 pointContactInitial,Vector3 directionNormalCollision)
    {
        RaycastHit hitInfo;

        //Si le projectile n'a pas de bloc devant lui, mais il y a contact
        if(!Physics.Raycast(positionDebutRebond, projectileDirectionInitial, out hitInfo, 1f, LayerMask.GetMask("Terrain")))
        {
            //Trace un ray dans la direction de la collision initiale
            Physics.Raycast(positionDebutRebond, (pointContactInitial - positionDebutRebond), out hitInfo, 100f, LayerMask.GetMask("Terrain"));

            //Redéfini la normal puisque dans certain cas la normal trouvé n'est pas bonne (ne donne pas de rebond réaliste)
            hitInfo.normal = directionNormalCollision;
            
        }
        

        return hitInfo;
    }
    
    /// <summary>
    /// Regarde si le rebond est dans un coin
    /// </summary>
    /// 
    /// <param name="hitInfo"> L'information trouvé par le ray </param>
    /// <param name="vecteurDirection"> La nouvelle direction du projectile après le rebond </param>
    /// <returns></returns>
    private bool verifierSiRebondPerpendiculaire(RaycastHit hitInfo, Vector3 vecteurDirection)
    {

        bool siPerpendiculaire;
        Ray ray = new Ray(hitInfo.point, vecteurDirection);

        siPerpendiculaire = Physics.Raycast(ray, 0.2f, LayerMask.GetMask("Terrain"));   //Regarde s'il y a un mur juste en avant
        

        return siPerpendiculaire;
    }

    /// <summary>
    /// Repositionne le projectile avec la nouvelle directione et la nouvelle position
    /// </summary>
    /// <param name="nouveauVecteur"> Le vecteur direction après le rebond </param>
    /// <param name="position"> La position du contact </param>
    private void repositionner(Vector3 nouveauVecteur, Vector3 position)
    {

         //Rotation
        transform.rotation = Quaternion.LookRotation(nouveauVecteur);
        newRotation = transform.localEulerAngles;
        newRotation.x += 90;
        transform.localEulerAngles = newRotation;               


        //Repositionne
        _projectileDirection = rb.transform.up;                
        transform.position = position;
    }

   /// <summary>
   /// Déplace le projectile hors du mur
   /// </summary>
   /// <param name="colliderMur"> Le collider du mur en contact</param>
    private void deplacerHorsMur( Collider colliderMur)
    {
        
        bool siMurDansListe = false;
        bool siContactAvecMur = true;

        //Variable pour vérifier les collisions au cercle
        float radiusSphereCheck;
        Vector3 positionSphereCheck = Vector3.zero;
        Collider[] tabCollider;

        //Initialise le rayon du cercle à vérifier
        radiusSphereCheck = GetComponent<CapsuleCollider>().radius * gameObject.transform.localScale.y;
        do
        {
            siMurDansListe = false;                             //Réinitialise la condition

            //Assigne la position de la vérification du cercle
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).transform.name == "PositionEndCollider")
                {
                    positionSphereCheck = transform.GetChild(i).transform.position;
                }
            }

            //Assigne les colliders au tab[] qui entre en contact avec le cercle
            tabCollider = Physics.OverlapSphere(positionSphereCheck, radiusSphereCheck);


            foreach (Collider col in tabCollider)
            {
                //Vérifie si le projectile est encore en contact avec le mur initial
                if (col == colliderMur)
                {
                    siMurDansListe = true;
                }
            }

            //Vérifie s'il y a contacts multiples avec le projectile
            if (siMurDansListe)                                //Il entre toujours en collision avec lui-même et le hitground
            {
                ChangePosition();
            }
            else
            {
                siContactAvecMur = false;
            }

        } while (siContactAvecMur);
    }
    
    #endregion

    #region COLLISION
    /// <summary>
    /// Collision avec un bloc
    /// </summary>
    /// <param name="collision"> L'information sur la collision</param>
    private void CollisionBlock(Collision collision)
    {
        //S'il peut rebondir
        if (estPossibleRebondir)
        {
            estPossibleRebondir = false;

            //S'il n'y a plus de rebonds
            if (nombreRebond == 0)
            {
                Destroy(gameObject);
                if (explosion != null)
                {
                    Instantiate(explosion, transform.position, Quaternion.identity);

                }
            }
            else
            {
                nombreRebond -= 1;      //Enregistre le rebond
                Rebondir(collision);    //Rebondi le projectile

                //Spawn le bullet Impact (particle system)
                bulletImpact = Instantiate(bulletImpact, transform.position, Quaternion.LookRotation(collision.contacts[0].normal));
                
            }
        }
    }

    /// <summary>
    /// Collision avec un projectile
    /// </summary>
    private void CollisionProjectile()
    {
        //Détruit le projectile
        Destroy(gameObject);
        if (explosion != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);

        }
        else
        {
            Debug.LogError(gameObject.name + " has not found the explosion");
        }
    }

    /// <summary>
    /// Si la collision se fait avec un tank
    /// </summary>
    private void CollisionTank()
    {
        //Détruit le projectile
        Destroy(gameObject);
        if (explosion != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);

        }
        else
        {
            Debug.LogError(gameObject.name + " has not found the explosion");
        }
    }
#endregion

#region UTIL
    /// <summary>
    /// Effectue une transformation linéaire pour trouver la réflexion
    /// du vecteur direction par rapport au vecteur unitaire.
    /// </summary>
    /// <param name="unitaire"> La droite de réflexion </param>
    /// <param name="direction"> Le vecteur qu'on désire effectuer la réflexion </param>
    /// <returns> Le nouveau vecteur calculé après la transformation linéaire </returns>
    private Vector3 CalculNouveauVecteur(Vector3 unitaire, Vector3 direction)
    {
        float[,] matRotation;
        Vector3 nouveauVecteur = direction;

        //Calcul la nouvelle matrice
        matRotation = GeneralFunction.CalculMatriceReflexion(unitaire);

        //Calcul le nouveau vecteur
        nouveauVecteur.x = matRotation[0, 0] * direction.x + matRotation[0, 1] * direction.z;
        nouveauVecteur.z = matRotation[1, 0] * direction.x + matRotation[1, 1] * direction.z;
        
        return nouveauVecteur.normalized;
    }
    
#endregion

#region GETTER/SETTER
    public Vector3 projectileDirection
    {
        get
        {
            return _projectileDirection;
        }
    }
    #endregion

#region GIZMOS
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, _projectileDirection.normalized * 100);
            Gizmos.DrawRay(transform.position, -_projectileDirection.normalized * 100);
        }
    }
#endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouvementPlayer : MonoBehaviour 
{ 

    [Header("Mouvement")]
    public float speedMouvement = 2f;
    public float speedRotate = 0.5f;

    [Header("Bouger Base")]
    private bool isGoingUp;
    private bool isGoingDown;
    private bool isGoingRight;
    private bool isGoingLeft;

    private bool isVectorUp;
    private bool isVectorDown;
    private bool isVectorLeft;
    private bool isVectorRight;

    private Quaternion targetRotation;
    private Vector3 directionBase;
    private Vector3 targetDirection;
    
    private BoxCollider colliderTank;

    private bool isRotating;

    [SerializeField]
    private float speedRotation = 1;
    private float lerpTime = 0;

    [Header("Bouger Tourelle")]
    public LayerMask clickMask;

    private Vector3 positionMouse;
    private Vector3 directionToFace;


    private void Start()
    {
        //Initialise la direction du joueur et son collider
        directionBase = transform.forward;
        colliderTank = transform.GetComponentInParent<BoxCollider>();

        //Message d'erreur
        if (transform.GetComponentInParent<BoxCollider>() == null)
        {
            Debug.LogError(transform.name + " n'a pas de BoxCollidier.");
        }
    }
     
    void FixedUpdate()
    {
        directionBase = transform.forward;
        RotateBase();
        rotaterTourelle();
    }

    /// <summary>
    /// Rotate la tourelle selon l'endroit de la souris
    /// </summary>
    private void rotaterTourelle()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>();

        //Message d'erreur
        if(GetComponentsInChildren<Transform>() == null)
        {
            Debug.LogError(transform.name + " n'a pas de component Transform dans ses enfants.");
        }

        //Trouve la position du mouse dans l'espace
        positionMouse = GeneralFunction.PositionMouse(clickMask);

        foreach (Transform transform in transforms)
        {
            //Trouve le component Transform du Body
            if (transform.gameObject.name == "body" || transform.gameObject.name == "Body")
            {
                positionMouse.y = transform.position.y;                         //Assigne la même hauteur que l'horizon
                directionToFace = -(positionMouse - transform.position);        //Trouve le vecteur direction
                transform.rotation = Quaternion.LookRotation(directionToFace);  //Change sa rotation
            }
        }
        
    }

    /// <summary>
    /// Rotate la base selon les commandes de bouger du joueur
    /// </summary>
    private void RotateBase()
    {
        //Trouve la direction des inputs du joueur
        targetDirection = trouverTargetDirection();

        //S'il a la même direction que la direction de la base ou il n'y  a pas de direction
        if (targetDirection.Equals(Vector3.zero) || targetDirection.normalized.Equals(directionBase.normalized))
        {
            isRotating = false;
        }
        else
        {
            targetRotation = Quaternion.LookRotation(targetDirection);      //Assigne la rotation voulu
            isRotating = true;
            lerpTime = 0;

        }

        //S'il doit rotater
        if (isRotating)
        {
            lerpTime += Time.deltaTime * speedRotation;                                         //Le temps de rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpTime); //Rotate le transform
            colliderTank.transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpTime);    //Rotate le collider

        }
    }

    /// <summary>
    /// Trouve le vecteur target selon les inputs du joueurs
    /// </summary>
    /// <returns> Le vecteur target</returns>
    private Vector3 trouverTargetDirection()
    {
        Vector3 targetDirection = Vector3.zero;

        //S'il va seulement à gauche
        if (isGoingLeft && !isGoingRight)
        {
            isVectorLeft = true;
            isVectorRight = false;
        }
        //S'il va seulement à droite
        else if (isGoingRight && !isGoingLeft)
        {
            isVectorLeft = false;
            isVectorRight = true;
        }
        else
        {
            isVectorLeft = false;
            isVectorRight = false;
        }

        //S'il va seulement en avant
        if (isGoingUp && !isGoingDown)
        {
            isVectorUp = true;
            isVectorDown = false;
        }
        //S'il va seulement en arrière
        else if (isGoingDown && !isGoingUp)
        {
            isVectorUp = false;
            isVectorDown = true;
        }
        else
        {
            isVectorUp = false;
            isVectorDown = false;
        }

        //S'il veut aller en avant
        if (isVectorUp)
        {
            targetDirection.z += 1f;
        }
        //S'il veut aller en arrière
        else if (isVectorDown)
        {
            targetDirection.z += -1f;
        }

        //S'il veut aller à droite
        if (isVectorRight)
        {
            targetDirection.x += 1f;
        }
        //S'il veut aller à gauche
        else if (isVectorLeft)
        {
            targetDirection.x += -1f;
        }
        
        return targetDirection;
    }

    /// <summary>
    /// Si le input left a été rentré, bouge le joueur dans cette direction
    /// </summary>
    public void moveLeft()
    {
        if(this.enabled)
        {
            transform.Translate(Vector3.left*speedMouvement*Time.deltaTime, Space.World);
            isGoingLeft = true;
        }
    }

    /// <summary>
    /// Si le input right a été rentré, bouge le joueur dans cette direction
    /// </summary>
    public void moveRight()
    {
        if(this.enabled)
        {
            transform.Translate(Vector3.right * speedMouvement * Time.deltaTime, Space.World);
            isGoingRight = true;
        }
    }

    /// <summary>
    /// Si le input avant a été rentré, bouge le joueur dans cette direction
    /// </summary>
    public void moveUp()
    {
        if(this.enabled)
        {
            transform.Translate(Vector3.forward * speedMouvement * Time.deltaTime, Space.World);
            isGoingUp = true;
        }
    }

    /// <summary>
    /// Si le input arrière a été rentré, bouge le joueur dans cette direction
    /// </summary>
    public void moveDown()
    {
        if(this.enabled)
        {
            transform.Translate(Vector3.back * speedMouvement * Time.deltaTime, Space.World);
            isGoingDown = true;
        }
    }

    /// <summary>
    /// S'il relâche left
    /// </summary>
    public void stopLeft()
    {
        if(this.enabled)
        {
            isGoingLeft = false;
        }
    }

    /// <summary>
    /// S'il relâche right
    /// </summary>
    public void stopRight()
    {
        if(this.enabled)
        {
            isGoingRight = false;
        }
    }

    /// <summary>
    /// S'il relâche avant
    /// </summary>
    public void stopUp()
    {
        if(this.enabled)
        {
            isGoingUp = false;
        }
    }

    /// <summary>
    /// S'il relâche arrière
    /// </summary>
    public void stopDown()
    {
        if(this.enabled)
        {
            isGoingDown = false;
        }
    }
}

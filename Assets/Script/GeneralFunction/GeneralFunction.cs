using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Ce script contient plusieurs fonctions générales utilisées par divers autres scripts.
 */
public static class GeneralFunction 
{
    /// <summary>
    ///     Projecter un rectangle dans une direction pour détecter tous les gameobjects qui s'y trouve.
    /// </summary>
    /// <param name='startPoint'>
    ///     Le point de départ du rectangle.
    /// </param>
    /// <param name='direction'>
    ///     La direction dans laquel projetter le rectangle, en fonction de la position de départ.
    /// </param>
    /// <param name='width'>
    ///     La largeur du rectangle.
    /// </param>
    /// <param name='length'>
    ///     La longeur du rectangle.
    /// </param>
    /// <param name='height'>
    ///     La hauteur du rectangle.
    /// </param>
    /// <returns>
    ///     Une liste des colliders de tous les gameobjects présents dans le rectangle.
    /// </returns>
	public static Collider[] projectRectangle(Vector3 startPoint, Vector3 direction, float width, float length, float height = 1f)
	{
		Collider[] colliders;

		//Calculer la taille de la boite
		Vector3 size = new Vector3(width/2, height/2, length/2);

		//Calculer le centre de la boite
		Vector3 center = startPoint + direction.normalized*(length/2);
		center.y = height/2 + 0.1f;

		//Calculer l'angle d'orientation de la boite
		float angle = Vector3.Angle(Vector3.forward, direction.normalized);
		Quaternion orientation = Quaternion.AngleAxis(angle, Vector3.up);

		colliders = Physics.OverlapBox(center, size, orientation);
		return colliders;
	}

    public static Collider[] OverlapCone(Vector3 origin, Vector3 direction, float range, float angle, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        List<Collider> colliders = new List<Collider>();

        //Trouver tous les objects autour du point d'origin
        Collider[] sphereOverlaps = Physics.OverlapCapsule(origin, origin, range, layerMask, queryTriggerInteraction);
        foreach(Collider collider in sphereOverlaps)
        {
            Vector3 directionToCollider = collider.transform.position - origin;                                 //Calculer la direction vers le collider
            float angleToCollider = Vector3.Angle(direction, directionToCollider);                              //Calculer l'angle du collider par rapport à la direction central

            //Vérifier si le collider se trouve dans l'angel
            if(angleToCollider <= angle/2)
            {
                colliders.Add(collider);
            }
        }

        return colliders.ToArray();
    }
    public static Collider[] OverlapCone(Vector3 origin, Vector3 direction, float range, float angle, int layerMask)
    {
        List<Collider> colliders = new List<Collider>();

        //Trouver tous les objects autour du point d'origin
        Collider[] sphereOverlaps = Physics.OverlapCapsule(origin, origin, range, layerMask);
        foreach(Collider collider in sphereOverlaps)
        {
            Vector3 directionToCollider = collider.transform.position - origin;                                 //Calculer la direction vers le collider
            float angleToCollider = Vector3.Angle(direction, directionToCollider);                              //Calculer l'angle du collider par rapport à la direction central

            //Vérifier si le collider se trouve dans l'angel
            if(angleToCollider <= angle/2)
            {
                colliders.Add(collider);
            }
        }

        return colliders.ToArray();
    }
    public static Collider[] OverlapCone(Vector3 origin, Vector3 direction, float range, float angle)
    {
        List<Collider> colliders = new List<Collider>();

        //Trouver tous les objects autour du point d'origin
        Collider[] sphereOverlaps = Physics.OverlapCapsule(origin, origin, range);
        foreach(Collider collider in sphereOverlaps)
        {
            Vector3 directionToCollider = collider.transform.position - origin;                                 //Calculer la direction vers le collider
            float angleToCollider = Vector3.Angle(direction, directionToCollider);                              //Calculer l'angle du collider par rapport à la direction central

            //Vérifier si le collider se trouve dans l'angel
            if(angleToCollider <= angle/2)
            {
                colliders.Add(collider);
            }
        }

        return colliders.ToArray();
    }

    /// <summary>
    ///     Vérifier si un point se trouve à l'intérieur d'un plan. Le plan est un rectangle délimité par 4 coins.
    /// </summary>
    /// <param name='cornerBackLeft'>
    ///     Le coin arrière gauche du plan.
    /// </param>
    /// <param name='cornerBackRight'>
    ///     Le coin arrière droite du plan.
    /// </param>
    /// <param name='cornerFrontLeft'>
    ///     Le coin avant gauche du plan.
    /// </param>
    /// <param name='cornerFrontRight'>
    ///     Le coin avant droit du plan.
    /// </param>
    /// <param name='point'>
    ///     Le point à vérifier.
    /// </param>
    /// <returns>
    ///     True si le point est dans le plan, false sinon.
    /// </returns>
    public static bool isPointWithinPlane(Vector3 cornerBackLeft, Vector3 cornerBackRight, Vector3 cornerFrontLeft, Vector3 cornerFrontRight, Vector3 point)
    {
        bool pointIsWithinPlane = true;

        Vector3 relativePoint = point - cornerBackLeft;
        if (relativePoint.x < 0f && relativePoint.z < 0f)
        {
            pointIsWithinPlane = false;
        }

        relativePoint = point - cornerBackRight;
        if (relativePoint.x > 0f && relativePoint.z < 0f)
        {
            pointIsWithinPlane = false;
        }

        relativePoint = point - cornerFrontLeft;
        if (relativePoint.x < 0f && relativePoint.z > 0f)
        {
            pointIsWithinPlane = false;
        }

        relativePoint = point - cornerFrontRight;
        if (relativePoint.x > 0f && relativePoint.z > 0f)
        {
            pointIsWithinPlane = false;
        }

        return pointIsWithinPlane;
    }

    /// <summary>
    ///     Créer un vecteur qui correspond à la rotation d'un vecteur donné d'un angle donné.
    /// </summary>
    /// <param name='vector'>
    ///     Le vecteur à tourner.
    /// </param>
    /// <param name='angle'>
    ///     L'angle de rotation, en radians.
    /// </param>
    /// <returns>
    ///     Le vecteur tourné.
    /// </returns>
    public static Vector3 rotateVector(Vector3 vector, float angle)
    {
        Vector3 rotatedVector = Vector3.zero;

        rotatedVector.z = vector.x * Mathf.Sin(angle) + vector.z * Mathf.Cos(angle);
        rotatedVector.x = vector.x * Mathf.Cos(angle) - vector.z * Mathf.Sin(angle);
        rotatedVector.y = vector.y;

        return rotatedVector;
    }

    /// <summary>
    ///     Calcule l'angle polaire d'un vecteur. L'angle est calculé à partir du vecteur de référence Vector3.right = (1,0,0)
    /// </summary>
    /// <param name='vector'>
    ///     Le vecteur.
    /// </param>
    /// <returns>
    ///     L'angle polaire du vecteur, en degré.
    /// </returns>
    public static float PolarAngleOfVector(Vector3 vector)
    {
        float angle;

        if(vector.z >= 0)
        {
            angle = Vector3.Angle(Vector3.right, vector);
        }
        else
        {
            angle = 360 - Vector3.Angle(Vector3.right, vector);
        }

        return angle;
    }

    /// <summary>
    ///     Créer une matrice de réflexion d'un vecteur autour du vecteur donné.
    /// </summary>
    /// <param name='unitaire'>
    ///     Le vecteur unitaire qui sert de droite de réflextion.
    /// </param>
    /// <returns>
    ///     La matrice permettant la rotation d'un vecteur autour du vecteur donné.
    /// </returns>
    public static float[,] CalculMatriceReflexion(Vector3 unitaire)
    {
        /*Vecteur de comparaison*/
        Vector3 vectorX = Vector3.right;
        Vector3 vectorZ = Vector3.forward;

        float angleUniAxeX;
        float angleUniAxeZ;

        float[,] matRot = new float[2, 2];

        angleUniAxeX = Vector3.Angle(vectorX, unitaire);
        angleUniAxeZ = Vector3.Angle(vectorZ, unitaire);

        if (angleUniAxeZ > 90)
        {
            angleUniAxeX = -angleUniAxeX;
        }

        angleUniAxeX = Mathf.PI * angleUniAxeX / 180;


        matRot[0, 0] = Mathf.Cos(2 * angleUniAxeX);
        matRot[0, 1] = Mathf.Sin(2 * angleUniAxeX);
        matRot[1, 0] = Mathf.Sin(2 * angleUniAxeX);
        matRot[1, 1] = -1 * Mathf.Cos(2 * angleUniAxeX);

        return matRot;
    }

    /// <summary>
    ///     Trouver la position de la souris sur la map.
    /// </summary>
    /// <param name='clickMask'>
    ///     La layer à considérer pour le raycasting.
    /// </param>
    /// <returns>
    ///     La position de la souris sur la map.
    /// </returns>
    public static Vector3 PositionMouse(LayerMask clickMask)
    {
        Vector3 positionMouse = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 1000f, clickMask))
        {
            positionMouse = hitInfo.point;
        }

        return positionMouse;

    }

    /// <summary>
    ///     Arrondir un nombre à un nombre de décimals donné.
    /// </summary>
    /// <param name='nb'>
    ///     Le nombre à arrondir.
    /// </param>
    /// <param name='decimals'>
    ///     Le nombre de décimals à garder.
    /// </param>
    /// <returns>
    ///     Le nombre arrondi.
    /// </returns>
    public static float roundFloat(float nb, int decimals)
	{
		float roundedNb = nb * (Mathf.Pow(10f, decimals));
		roundedNb = Mathf.Round(roundedNb);
		roundedNb = roundedNb / (Mathf.Pow(10f, decimals));

		return roundedNb;
	}
}  

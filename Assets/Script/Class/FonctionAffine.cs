using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Cette classe représente une fonction affine sur le plan XZ du niveau.
 */

public class FonctionAffine {

	private float _pente;
	private float _ordonne;

	private bool _isSegment;
	private Vector3 _startPoint;
	private Vector3 _endPoint;

	public FonctionAffine(Vector3 startPoint, Vector3 endPoint)
	{
		_startPoint = startPoint;
		_endPoint = endPoint;

		_pente = (endPoint.z - startPoint.z)/(endPoint.x - startPoint.x);
		_ordonne = startPoint.z - (_pente * startPoint.x);		

		//Round shit up
		_startPoint.x = roundFloat(_startPoint.x);
		_startPoint.y = roundFloat(_startPoint.y);
		_startPoint.z = roundFloat(_startPoint.z);
		_endPoint.x = roundFloat(_endPoint.x);
		_endPoint.y = roundFloat(_endPoint.y);
		_endPoint.z = roundFloat(_endPoint.z);
		_pente = roundFloat(_pente);
		_ordonne = roundFloat(_ordonne);

		_isSegment = true;
	}
	public FonctionAffine(float pente, float ordonne)
	{
		_pente = pente;
		_ordonne = ordonne;

		//Round shit up
		_pente = roundFloat(pente);
		_ordonne = roundFloat(ordonne);

		_isSegment = false;
		_startPoint = Vector3.zero;
		_endPoint = Vector3.zero;
	}

	/// <summary>
	///		Calculer l'image d'une coordonnée en abscisse donnée.
	/// </summary>
	/// <param name='abscisse'>
	///		L'abscisse du point.
	///	</param>
	/// <returns>
	///		L'image de l'abscisse donnée
	///	</returns>
	public float getImage(float abscisse)
	{
		return roundFloat((pente * abscisse) + ordonne);
	}

	/// <summary>
	///		Calculer l'abscisse d'une image donnée.
	/// </summary>
	/// <param name='image'>
	///		L'image du point.
	///	</param>
	/// <returns>
	///		L'abscisse de l'image donnée
	///	</returns>
	public float getAbscisse(float image)
	{
		return (image - ordonne) / pente;
	}
	
	/// <summary>
	/// 	Vérifier si un point est sur la fonction affine.
	///	</summary>
	/// <param name='point'>
	///		Le point a vérifier.
	///	</param>
	/// <returns>
	///		True si le point est sur la fonction, false sinon.
	/// </returns>	
	public bool isPointOnFonction(Vector3 point)
	{
		bool isOnFonction = false;

		//Round up the point
		Vector3 roundedPoint = new Vector3(roundFloat(point.x), roundFloat(point.y), roundFloat(point.z));

		//Vérifier si le point est un point de la fonction
		if(getImage(point.x) == roundedPoint.z)
		{
			//Si la fonction a des limites, vérifier si le point se trouve dans ces limites
			if(isSegment)
			{
				if((roundedPoint.x>=startPoint.x && roundedPoint.x<=endPoint.x) ||
					(roundedPoint.x<=startPoint.x && roundedPoint.x>=endPoint.x))
				{
					if((roundedPoint.z>=startPoint.z && roundedPoint.z<=endPoint.z) ||
						(roundedPoint.z<=startPoint.z && roundedPoint.z>=endPoint.z))
					{
						isOnFonction = true;
					}
				}
			}
			else
			{
				isOnFonction = true;
			}
		}

        return isOnFonction;
	}

#region STATIC
	/// <summary>
	/// 	Trouver le point d'intersection entre deux droites.
	///	</summary>
	/// <param name='fonctionA'>
	///		L'un des deux fonctions
	///	</param>
	/// <param name='fonctionB'>
	///		L'un des deux fonctions
	///	</param>
	/// <param name='intersection'>
	///		Le point d'intersection entre les deux droites. S'il n'y a pas de point d'intersection, la valeur est Vector.zero.
	///	</param>
	/// <returns>
	///		True s'il y a un point d'intersection. False, sinon.
	/// </returns>	
	public static bool Intersect(FonctionAffine fonctionA, FonctionAffine fonctionB, out Vector3 intersection)
	{
		bool areIntersecting = false;
		
		intersection = Vector3.zero;

		//Vérifier si les pentes sont parallèles
		if(fonctionA.pente != fonctionB.pente)
		{
			//Trouver l'intersection
			intersection.x = (fonctionB.ordonne - fonctionA.ordonne) / (fonctionA.pente - fonctionB.pente);
			intersection.z = (fonctionA.pente * intersection.x) + fonctionA.ordonne;

			areIntersecting = true;
		}
		else
		{
			//Les pentes doivent se superposent pour que les fonctions s'intersectionne
			if(fonctionA.ordonne == fonctionB.ordonne)
			{
				//Assigner l'ordonnée à l'orginne comme intersection
				intersection.x = 0;
				intersection.z = fonctionA.ordonne;

				areIntersecting = true;
			}
		}

		return areIntersecting;
	}

#endregion

#region UTIL
	/// <summary>
	///		Arrondir au nombre à deux décimals
	/// </summary>
	/// <param name='nb'>
	/// 	Le nombre à arrondir
	///	</param>
	/// <returns>
	/// 	Le nombre arrondie
	/// <return>
	private float roundFloat(float nb)
	{
		float roundedNb = nb * (Mathf.Pow(10f, 2f));
		roundedNb = Mathf.Round(roundedNb);
		roundedNb = roundedNb / (Mathf.Pow(10f, 2f));

		return roundedNb;
	}
#endregion

#region GETTER/SETTER
	public bool isSegment
	{
		get
		{
			return _isSegment;
		}
	}
	public Vector3 startPoint
	{
		get
		{
			return _startPoint;
		}
	}
	public Vector3 endPoint
	{
		get
		{
			return _endPoint;
		}
	}
	public float pente
	{
		get
		{
			return _pente;
		}
	}
	public float ordonne
	{
		get
		{
			return _ordonne;
		}
	}
#endregion
}
 
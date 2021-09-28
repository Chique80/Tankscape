using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Cette classe représente une droite en 2 dimension.
 */

public class Droite2D
{
	private Vector2 _pointA;
	private Vector2 _direction;

	private bool _hasBorne;
	private Vector2 _borneA;
	private Vector2 _borneB;

	public Droite2D(Vector2 point, Vector2 direction)
	{
		_pointA = point;
		_direction = direction;

		_hasBorne = false;
		_borneA = Vector2.zero;
		_borneB = Vector2.zero;
	}
	public Droite2D(Vector2 borneStart, Vector2 borneEnd, bool useBorne)
	{
		_hasBorne = useBorne;
		_borneA = borneStart;
		_borneB = borneEnd;

		_pointA = borneStart;
		_direction = borneStart - borneEnd;
	}
	public Droite2D(Vector3 point, Vector3 direction, bool useY)
	{
		if(useY)
		{
			_pointA = new Vector2(point.x, point.y);
			_direction = new Vector2(direction.x, direction.y);
		}
		else
		{
			_pointA = new Vector2(point.x, point.z);
			_direction = new Vector2(direction.x, direction.z);
		}

		_hasBorne = false;
		_borneA = Vector2.zero;
		_borneB = Vector2.zero;
	}
	public Droite2D(Vector3 borneStart, Vector3 borneEnd, bool useBorne, bool useY)
	{
		_hasBorne = useBorne;

		if(useY)
		{
			_borneA = new Vector2(borneStart.x, borneStart.y);
			_borneB = new Vector2(borneEnd.x, borneEnd.y);
		}
		else
		{
			_borneA = new Vector2(borneStart.x, borneStart.z);
			_borneB = new Vector2(borneEnd.x, borneEnd.z);
		}

		_pointA = borneStart;
		_direction = borneStart - borneEnd;
	}


	/// <summary>
	/// 	Vérifier si un point est à l'intérieur des bornes de la droite. La méthode vérifie aussi si le point est sur la droite.
	///			Si la droite n'a pas de bornes. La valeur retournée est la même que Droite2D.isPointOnDroite(Vector3 point).
	/// </summary>
	/// <param name='point'>
	/// 	Le point à vérifier.
	///	</param>
	/// <returns>
	/// 	True si le point est entre les bornes et sur la droite. False sinon.
	///	</returns>
	public bool isPointWithinBorders(Vector2 point)
	{
		bool isValid = false;

		if(isPointOnDroite(point) && hasBorne)
		{
			if((roundFloat(point.x)>=roundFloat(borneA.x) && roundFloat(point.x)<=roundFloat(borneB.x)) || 
				(roundFloat(point.x)<=roundFloat(borneA.x) && roundFloat(point.x)>=roundFloat(borneB.x)))
			{
				if((roundFloat(point.y)>=roundFloat(borneA.y) && roundFloat(point.y)<=roundFloat(borneB.y)) || 
					(roundFloat(point.y)<=roundFloat(borneA.y) && roundFloat(point.y)>=roundFloat(borneB.y)))
				{
					isValid = true;
				}
			}
		}

		return isValid;
	}

	/// <summary>
	/// 	Vérifier si un point est sur la droite. Si la droite possède des bornes, la méthode ne les prend pas en compte.
	/// </summary>
	/// <param name='point'>
	/// 	Le point à vérifier.
	///	</param>
	/// <returns>
	/// 	True si le point est sur la droite. False sinon.
	///	</returns>
	public bool isPointOnDroite(Vector2 point)
	{
		bool isValid = false;

		float constantFromX = (point.x - pointA.x) * direction.y;
		float constantFromY = (point.y - pointA.y) * direction.x;

		if(roundFloat(constantFromX) == roundFloat(constantFromY))
		{
			isValid = true;
		}

		return isValid;
	}

#region OVERRIDE
public override string ToString()
{
	string name = "D:(x,y) = " + pointA + " + k" + direction;

	if(hasBorne)
	{
		name += " for (" + borneA + " ; " + borneB + ")";
	}

	return name;
}

#endregion

#region STATIC
	/// <summary>
	/// 	Trouver le point d'intersection entre deux droites.
	/// </summary>
	/// <param name='droiteA'>
	/// 	L'une des droites
	///	</param>
	///	<param name='droiteB'>
	///		L'une des droites
	///	</param>
	///	<param name='intersection'>
	///		Le point d'intersection entre les deux droites. S'il n'y a pas d'intersection, la valeur retournée est Vector3.zero;
	/// </param>
	/// <returns>
	/// 	True s'il y a un point d'intersection. False sinon.
	///	</returns>
	public static bool Intersect(Droite2D droiteA, Droite2D droiteB, out Vector2 intersection)
	{
		bool isIntersecting = false;

		if(!areParallele(droiteA, droiteB))
		{
			//Calculer les valeurs de k et t
			float determinant = (droiteB.direction.x*droiteA.direction.y) - (droiteB.direction.y*droiteA.direction.x);
			float constantX = droiteB.pointA.x - droiteA.pointA.x;
			float constantY = droiteB.pointA.y - droiteA.pointA.y;

			float k = ((droiteB.direction.x*constantY) - (droiteB.direction.y*constantX)) / determinant;
			float t = ((droiteA.direction.x*constantY) - (droiteA.direction.y*constantX)) / determinant;

			//Trouver le point d'intersection pour la droite A
			Vector2 intersectionA = Vector2.zero;
			intersectionA.x = droiteA.pointA.x + k*droiteA.direction.x;
			intersectionA.y = droiteA.pointA.y + k*droiteA.direction.y;

			//Trouver le point d'intersection pour la droite B
			Vector2 intersectionB = Vector2.zero;
			intersectionB.x = droiteB.pointA.x + t*droiteB.direction.x;
			intersectionB.y = droiteB.pointA.y + t*droiteB.direction.y;

			//Vérifier le point d'intersection avec la droite B
			if(intersectionA == intersectionB)
			{
				isIntersecting = true;
				intersection = intersectionA;
			}
			else
			{
				intersection = Vector2.zero;
			}
		}
		else
		{
			intersection = Vector2.zero;
		}

		return isIntersecting;
	}

	/// <summary>
	/// 	Vérifier si deux droites sont parallèles.
	/// </summary>
	/// <param name='droiteA'>
	/// 	L'une des droites
	///	</param>
	///	<param name='droiteB'>
	///		L'une des droites
	///	</param>
	/// <returns>
	/// 	True si les droites sont parallèles. False sinon.
	///	</returns>
	public static bool areParallele(Droite2D droiteA, Droite2D droiteB)
	{
		bool areParallele = true;

		float constantFromX = droiteA.direction.x * droiteB.direction.y;
		float constantFromY = droiteA.direction.y * droiteB.direction.x;

		if(constantFromX != constantFromY)
		{
			areParallele = false;
		}

		return areParallele;
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
	public Vector2 direction
	{ 
		get
		{
			return _direction;
		}
	}
	public Vector2 pointA
	{
		get
		{
			return _pointA;
		}
	}
	public Vector2 borneA
	{
		get
		{
			return _borneA;
		}
	}
	public Vector2 borneB
	{
		get
		{
			return _borneB;
		}
	}
	public bool hasBorne
	{
		get
		{
			return _hasBorne;
		}
	}
#endregion
}

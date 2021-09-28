using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* La classe représente un mur du niveau sous forme de vecteur. La classe est principalement utilisé pour
	le pathfinding.
 */
 //

public class WallVector 
{
	private Vector3 _startPoint;
	private Vector3 _endPoint;
	private List<Vector3> _points;

	private float pointsDrawingSize = 0.1f;


	public WallVector(Vector3 startPoint, Vector3 endPoint)
	{
		_startPoint = startPoint;
		_endPoint = endPoint;

		_points = new List<Vector3>();

		_points.Add(startPoint);
		addPoint(endPoint);
	}

	/// <summary>
	/// 	Vérifier si un point est sur le mur.
	/// </summary>
	/// <param name='point'>
	/// 	Le point a vérifier.
	///	</param>
	/// <returns>
	/// 	True si le point est sur le vecteur, false sinon.
	///	</returns>
	public bool isPointOnVector(Vector3 point)
	{
		bool isOnWall = false;

		Vector3 pointFromStart = point - startPoint;
		if(Vector3.Angle(pointFromStart, vector) == 0f)
		{
			if(pointFromStart.magnitude < vector.magnitude)
			{
				isOnWall = true;
			}
		}
		
		return isOnWall;
	}

	/// <summary>
	/// 	Vérifier si un point est sur la direction du vecteur.
	/// </summary>
	/// <param name='point'>
	/// 	Le point a vérifier.
	///	</param>
	/// <returns>
	/// 	True si le point est sur la direction du vecteur, false sinon.
	///	</returns>
	public bool isPointOnDirection(Vector3 point)
	{
		bool isOnDir = false;

		Vector3 pointFromStart = point - startPoint;
		if(Vector3.Angle(pointFromStart, vector)==0f || Vector3.Angle(pointFromStart, vector)==180f)
		{
			isOnDir = true;
		}

		return isOnDir;
	}

	/// <summary>
	/// 	Ajouter un point au vecteur. Le point doit être sur le vecteur pour pouvoir être ajouté.
	/// </summary>
	/// <param name='point'>
	/// 	Le point a ajouter.
	///	</param>
	/// <returns>
	/// 	True si le point a été ajouté, false sinon.
	///	</returns>
	public bool addPoint(Vector3 point)
	{
		bool isAdded = false;

		Vector3 pointFromStart = point - startPoint;									//Position du point relative au début du mur

		//Vérifier que le point ne fait pas partie de la liste
		if(!_points.Contains(point))
		{
			//Vérifier que le point est sur l'axe du mur
			if(isPointOnDirection(point))
			{
				_points.Add(point);												//Ajouter le point au mur

				//Vérifier si le point est avant ou après le début du mur
				if(Vector3.Angle(vector, pointFromStart) == 180f)
				{
					_startPoint = point;											//Redéfinir le début du mur car le point se trouve avant
				}
				else if(pointFromStart.magnitude > vector.magnitude)
				{
					_endPoint = point;												//Redéfinir la fin du mur car le point se trouve après
				}

				isAdded = true;
			}
		}

		return isAdded;
	}

	/// <summary>
	/// 	Vérifier le WallVecteur donné à ce WallVecteur. Les deux vecteurs doivent avoir la même direction et partager
	///			au moins un point d'intersection pour être fusionné.
	/// </summary>
	/// <param name='wall'>
	/// 	Le WallVector à fusionner.
	///	</param>
	/// <returns>
	/// 	True si le point a été fusionné, false sinon.
	///	</returns>
	public bool fusionner(WallVector wall)
	{
		bool hasFuse = false;

		//Vérifier que le début où la fin du mur à fusionné se trouve sur ce mur
		if(this.isPointOnVector(wall.startPoint) || this.isPointOnVector(wall.endPoint))
		{
			//Vérifier que le mur à fusionné à la même direction que ce mur
			if(Vector3.Angle(this.vector, wall.vector)==0f || Vector3.Angle(this.vector, wall.vector)==180f)
			{
				foreach(Vector3 point in wall.points)
				{
					this.addPoint(point);
				}

				hasFuse = true;
			}
		}

		return hasFuse;
	}

	/// <summary>
	/// 	Trouver le point du vectuer le plus proche d'un point donné.
	/// </summary>
	/// <param name='point'>
	/// 	Le point de référence.
	///	</param>
	/// <returns>
	/// 	Le point le plus proche.
	///	</returns>
	public Vector3 getClosestPoint(Vector3 point)
	{
		Vector3 closestPoint = Vector3.zero;
		float minDistance = Mathf.Infinity;

		foreach(Vector3 wallPoint in points)
		{
			//Calculer la distance entre le point et le point du mur
			float distance = Vector3.Distance(point, wallPoint);
			distance = Mathf.Abs(distance);

			//Regarder si la distance est la plus petite à date
			if(distance < minDistance)
			{
				minDistance = distance;
				closestPoint = wallPoint;
			}
		}

		return closestPoint;
	}
	
	/// <summary>
	/// 	Calculer la position d'un waypoint à partir du point de départ du vecteur.
	/// </summary>
	/// <param name='distance'>
	/// 	La distance entre le waypoint et le point de départ.
	///	</param>
	/// <returns>
	/// 	Le waypoint.
	///	</returns>
	public Vector3 getWaypointFromStart(float distance)
	{
		Vector3 waypoint = startPoint;

		if(startPoint.x == endPoint.x)
        {
            if(startPoint.z < endPoint.z)
			{
				waypoint.z -= distance;
			}
			else
			{
				waypoint.z += distance;
			}
        }
		else
		{
			FonctionAffine fonction = new FonctionAffine(startPoint, endPoint);
			float ecart = Mathf.Sqrt(Mathf.Pow(distance, 2) / (1 + Mathf.Pow(fonction.pente, 2)));

			if(startPoint.x < endPoint.x)
			{
				waypoint.x -= ecart;
				waypoint.z = fonction.getImage(waypoint.x);
			}
			else
			{
				waypoint.x += ecart;
				waypoint.z = fonction.getImage(waypoint.x);
			}
		}
        
		return waypoint;
	}

	/// <summary>
	/// 	Calculer la position d'un waypoint à partir du point de fin du vecteur.
	/// </summary>
	/// <param name='distance'>
	/// 	La distance entre le waypoint et le point de fin.
	///	</param>
	/// <returns>
	/// 	Le waypoint.
	///	</returns>
	public Vector3 getWaypointFromEnd(float distance)
	{
		Vector3 waypoint = endPoint;

		if(startPoint.x == endPoint.x)
        {
            if(startPoint.z < endPoint.z)
			{
				waypoint.z += distance;
			}
			else
			{
				waypoint.z -= distance;
			} 
        }
		else
		{
			FonctionAffine fonction = new FonctionAffine(startPoint, endPoint);
			float ecart = Mathf.Sqrt(Mathf.Pow(distance, 2) / (1 + Mathf.Pow(fonction.pente, 2)));

			if(startPoint.x < endPoint.x)
			{
				waypoint.x += ecart;
				waypoint.z = fonction.getImage(waypoint.x);
			}
			else
			{
				waypoint.x -= ecart;
				waypoint.z = fonction.getImage(waypoint.x);
			}
		}
        
		return waypoint;
	}

	/// <summary>
	///		Dessiner le WallVectors sur la map. Utilisé pour le debug.
	///	</summary>
	public void draw(Color color)
	{
		Color oldColor = Gizmos.color;
		Gizmos.color = color;

		//Draw the vector
		if(startPoint != endPoint)
		{
			DebugExtension.drawArrow(startPoint, endPoint-startPoint, Gizmos.color);
		}

		Gizmos.DrawSphere(startPoint, pointsDrawingSize);
		Gizmos.DrawSphere(endPoint, pointsDrawingSize);

		Gizmos.color = oldColor;
	}

#region OVERRIDE
	// override object.Equals()
	public override bool Equals(object obj)
	{
		bool isEqual = true;
		
		if (obj == null || GetType() != obj.GetType())
		{
			isEqual = false;
		}
		else
		{
			WallVector wall = (WallVector) obj;

			if(wall.startPoint != startPoint)
			{
				isEqual = false;
			}
			else if(wall.endPoint != endPoint)
			{
				isEqual = false;
			}
		}

		return isEqual;
	}
	
	// override object.GetHashCode
	public override int GetHashCode()
	{
		// TODO: write your implementation of GetHashCode() here
		return base.GetHashCode();
	}

	// override object.ToString()
	public override string ToString()
	{
		return "(" + startPoint + ";" + endPoint + ")";
	}
#endregion

#region GETTER/SETTER
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
	public Vector3 vector
	{
		get
		{
			return endPoint - startPoint;
		}
	}
	public Vector3[] points
	{
		get
		{
			return _points.ToArray();
		}
	}
	public string toString
	{
		get
		{
			return "(" + startPoint + ";" + endPoint + ")";
		}
	}
#endregion
}

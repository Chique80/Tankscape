using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Classe représentant un mur dans un niveau.
 */
 //

public class Block : MonoBehaviour 
{
	private float pointsDrawingSize = 0.1f;

	[Header("Collision")]
	public bool blocksProjectile;

	[Header("Wall")]
	public List<Block> wallEnds;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.7f, 0.7f, 0.7f);
	
		foreach(Block block in wallEnds)
		{
			if(block != null)
			{
				Vector3 start = center;
				Vector3 end = block.center;
				start.y = 2f;
				end.y = 2f;

				DebugExtension.drawArrow(start, end-start, Gizmos.color, 0.6f, 30);
				Gizmos.DrawSphere(start, pointsDrawingSize);
				Gizmos.DrawSphere(end, pointsDrawingSize);
			}
		}
		
	}

	/// <summary>
	/// 	Vérifier si le bloc est sur un vecteur donnée. Pour être sur le vecteur, le centre du bloc doit se trouver sur
	///			sur le vecteur.
	///	</summary>
	/// <returns>
	///		True si le bloc est sur le vecteur, false sinon.
	///	</returns>
	public bool isOnVector(Vector3 vector, Vector3 vectorStartPoint)
	{
		bool isOnVector = true;

		Vector3 centerFromStartPoint = bounds.center - vectorStartPoint;

		if(Vector3.Angle(centerFromStartPoint, vector) != 0f)
		{
			isOnVector = false;
		}

		if(isOnVector && centerFromStartPoint.magnitude > vector.magnitude)
		{
			isOnVector = false;
		}

		return isOnVector;
	}
 
#region OVERRIDE
	// override object.Equals()
	public override bool Equals(object obj)
{
	bool isEqual = true;

	if (obj == null || GetType() != obj.GetType())
	{
		return false;
	}
	else
	{
		Block block = (Block) obj;

		if(!block.transform.Equals(this.transform))
		{
			isEqual = false;
		}
		else if(!block.wallEnds.Equals(this.wallEnds))
		{
			isEqual = false;
		}
	}

	return isEqual;
}

	// override object.GetHashCode
	public override int GetHashCode()
{
	return base.GetHashCode();
}

#endregion

#region GETTER/SETTER
	public Bounds bounds
	{
		get
		{
			return this.gameObject.GetComponent<Renderer>().bounds;
		}
	}
	public float size
	{
		get
		{
			return (bounds.size.x);
		}
	}
	public float extent
	{
		get
		{
			return (bounds.extents.x);
		}
	}
	public Vector3 center
	{
		get
		{
			return bounds.center;
		}
	}
	public bool isWall
	{
		get
		{
			bool isWall = false;
			
			if(wallEnds.Capacity != 0)
			{
				isWall = true;

				//Lancer une erreur si une fin de mur n'est pas assignée
				int index = 0;
				while(isWall && index<wallEnds.Capacity)
				{
					
					if(wallEnds[index] == null)
					{
						Debug.LogError("Une fin de mur n'est pas assignee");
					}
					index++;
				}
			}

			return isWall;
		}
	}
#endregion

}

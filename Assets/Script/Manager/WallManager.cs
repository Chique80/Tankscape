using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 */
public class WallManager : MonoBehaviour
{
	private List<WallVector> sceneWalls;
	private List<WallVector> singleBlockWalls;
	private List<Block> blocks;

	// Use this for initialization
	void Awake()
	{
		sceneWalls = new List<WallVector>();
		singleBlockWalls = new List<WallVector>();
		blocks = new List<Block>();

		createMapVectors();
	}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawRay(Vector3.zero, Vector3.back * 1000);
        Gizmos.color = Color.red;
        float pente = 1000 * -1;
        float x = 10;
        float z = (pente * x);
        Vector3 point = new Vector3(x, 0, z);
        Gizmos.DrawRay(Vector3.zero, point);
    }

	/// <summary>
	/// 	Créer un liste de WallVector représentant les murs du niveau.
	/// </summary>
    private void createMapVectors()
	{
        List<Block> points = new List<Block>();                   //Mur composé d'un seul bloc

		//Chercher tous les blocs de la scène
		GameObject[] gameObjects = (GameObject[]) GameObject.FindObjectsOfType(typeof(GameObject));
		foreach(GameObject gameObj in gameObjects)
		{
			if(gameObj.GetComponent<Block>() != null)
			{
				Block block = gameObj.GetComponent<Block>();
				blocks.Add(block);

				//Créer les murs
				if(block.isWall)
				{
					foreach(Block wallEnd in block.wallEnds)
					{
						if(wallEnd != null)
						{
							sceneWalls.Add(new WallVector(block.center, wallEnd.center));
						}
					}
				}
                else
                {
                    points.Add(block);
                }
			}
		}

/* */
		//Assigner les points au murs
		foreach(Block point in points)
		{
            bool isPartOfWall = false;                                         //Indique si le point fait partie d'au moins un mur

            //Ajouter le point au murs dont il fait partie
			foreach(WallVector wall in sceneWalls)
            {
                if(wall.isPointOnVector(point.center))
                {
                    wall.addPoint(point.center);
                    isPartOfWall = true;
                }
            }

            //Créer des murs avec les points non assignés
            if(!isPartOfWall)
            {
                sceneWalls.Add(new WallVector(point.center, point.center));
            }
		}
	}
 
	/// <summary>
	/// 	Retourne la liste des WallVector du niveau. Cette méthode est normalement appelée par un tank.
	/// </summary>
    public WallVector[] getMapVectors()
    {
		List<WallVector> wallVectors = sceneWalls;
	   
	   	return wallVectors.ToArray();
    }
}

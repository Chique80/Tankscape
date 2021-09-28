using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    private EnemyAIValue valueScript;
    private Path[] paths;
    private List<WallVector> obstacles;
    

	// Use this for initialization
	void Start ()
    {
        //Get the EnemyAIValue.cs script of this enemy
        valueScript = GetComponent<EnemyAIValue>();
        if (valueScript == null)
        {
            Debug.LogError(this + " doesn't have a EnemyAIValue.cs script");
        }

        obstacles = new List<WallVector>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	}

    void OnDrawGizmosSelected()
    {
        if(Application.isPlaying && valueScript.showPathfindingGizmos)
        {
            if (valueScript.showPathfindingGizmos)
            {
                Gizmos.color = valueScript.pathfindingGizmosColor;

                if (paths != null)
                {
                    foreach (Path path in paths)
                    {
                        path.draw(Gizmos.color);
                    }
                }
                if (obstacles != null)
                {
                    foreach (WallVector wall in obstacles)
                    {
                        wall.draw(Gizmos.color);
                    }
                }
            }
        }
    }

    public bool checkWaypoint(Vector3 waypoint, WallVector[] walls)
    {
        bool isWaypointValid = true;

        if(valueScript.enemyAI.path != null)
        {
            WallVector obstacle = findFirstObstacle(transform.position, waypoint, walls);
            if(obstacle != null)
            {
                isWaypointValid = false;
            }
        }

        return isWaypointValid;
    }

#region PATHING
    /// <summary>
    ///     Trouver un chemin entre le tank et la cible. Le chemin peut soit être le meileur chemin, soit un chemin
    ///     aléatoire parmis ceux valides.
    /// </summary>
    /// <param name='target'>
    ///     La cible vers laquellle le tank doit se déplacer
    /// </param>
    /// <param name='walls'>
    ///     Les WallVectors du niveau. Ce sont les murs que le tank doit éviter.
    /// </param>
    /// <returns>
    ///     Le path choisi
    /// </returns>
    public Path findPath(Vector3 target, WallVector[] walls)
    {
        Path bestPath = null;
        obstacles.Clear();

        //Créer tout les chemins
        paths = createPathBetweenPoints(transform.position, target, walls);                            

        //Choisir le chemin
        if(paths.Length > 0)
        {
            //Sélection aléatoire
            if(valueScript.useRandomPathing)                                                
            {
                int index = Random.Range(0, paths.Length);
                bestPath = paths[index];
            }

            //Choisir le chemin le plus court
            else                                                                           
            {
                if(paths.Length > 0)
                {
                    float minDistance = Mathf.Infinity;
                    foreach (Path path in paths)
                    {
                        if(path.distance < minDistance)
                        {
                            minDistance = path.distance;
                            bestPath = path;
                        }
                    }
                }
            }
        }

        return bestPath;
    }

    /// <summary>
    ///     Créer tous les chemins possibles entrent deux points donnés
    /// </summary>
    /// <param name='startPoint'>
    ///     La position de départ du chemin
    /// </param>
    /// <param name='endPoint'>
    ///     La position de fin du chemin
    /// </param>
    /// <param name='walls'>
    ///     Les WallVectors du niveau. Ce sont les murs que le tank doit éviter.
    /// </param>
    /// <returns>
    ///     Tous les chemins créés
    /// </returns>
    private Path[] createPathBetweenPoints(Vector3 startPoint, Vector3 endPoint, WallVector[] walls)
    {
        List<Path> possiblePaths = new List<Path>();
        Path basePath = new Path(startPoint, endPoint);

        //Trouver le premier obstacle qui bloque le chemin, s'il y en a un
        WallVector obstacle = findFirstObstacle(startPoint, endPoint, walls);                           
        if(obstacle != null)
        {
            if(!obstacles.Contains(obstacle))
            {
                Path[] createdPaths;

                //Trouver les deux waypoints pour éviter l'obstacle FIXME
                Vector3 firstWaypoint = obstacle.getWaypointFromStart(valueScript.waypointsDistanceFromWall);
                Vector3 secondWaypoint = obstacle.getWaypointFromEnd(valueScript.waypointsDistanceFromWall);

                obstacles.Add(obstacle);                                                                   //Ajouter l'obstacle à liste des obstacles

                //Créer les paths pour le premier waypoint
                createdPaths = createPathToAndFromWaypoint(startPoint, endPoint, firstWaypoint, walls);
                if(createdPaths.Length > 0)
                {
                    foreach(Path path in createdPaths)
                    {
                        possiblePaths.Add(path);
                    }
                }

                createdPaths = null;

                //Créer les paths pour le second waypoint
                createdPaths = createPathToAndFromWaypoint(startPoint, endPoint, secondWaypoint, walls);
                if (createdPaths.Length > 0)
                {
                    foreach (Path path in createdPaths)
                    {
                        possiblePaths.Add(path);
                    }
                }

                obstacles.Remove(obstacle);
            }
        }
        else
        {
            possiblePaths.Add(basePath);
        }

        return possiblePaths.ToArray();
    }

    /// <summary>
    ///     Créer tous les chemins possibles entrent deux points donnés, en passant par un waypoin
    /// </summary>
    /// <param name='startPoint'>
    ///     La position de départ du chemins
    /// </param>
    /// <param name='endPoint'>
    ///     La position de fin du chemin
    /// </param>
    /// <param name='waypoint'>
    ///     Le waypoint que le tank doit emprunter
    /// </param>
    /// <param name='walls'>
    ///     Les WallVectors du niveau. Ce sont les murs que le tank doit éviter.
    /// </param>
    /// <returns>
    ///     Tous les chemins créés
    /// </returns>
    private Path[] createPathToAndFromWaypoint(Vector3 startPoint, Vector3 endPoint, Vector3 waypoint, WallVector[] walls)
    {
        List<Path> paths = new List<Path>();

        Path[] pathsToWaypoint = createPathBetweenPoints(startPoint, waypoint, walls);                      //Créer les paths vers le waypoint
        Path[] pathsFromWaypoint = createPathBetweenPoints(waypoint, endPoint, walls);                      //Créer les paths vers endPoint à partir du waypoint
        Path fusedPath;

        //Vérifier s'il y a des chemins valides
        if (pathsToWaypoint.Length > 0 && pathsFromWaypoint.Length > 0)
        {
            //Fusionner les paths créés
            foreach (Path path in pathsToWaypoint)
            {
                foreach (Path pathToFuse in pathsFromWaypoint)
                {
                    //Créer un nouveau path  qui est la fusion des deux autres
                    fusedPath = new Path(path);
                    fusedPath.fuseFromIndex(pathToFuse, fusedPath.size - 1);
                    paths.Add(fusedPath);
                }
            }
        } 

        return paths.ToArray();
    }
#endregion

    /// <summary>
    ///     Trouver le WallVector la plus proche qui agit commme obstacle entre les deux positions données
    /// </summary>
    /// <param name='startPoint'>
    ///     La position de départ du chemin. C'est à partir de se point qu'est calculé la distance avec les obstacles.
    /// </param>
    /// <param name='endPoint'>
    ///     La position de fin du chemin
    /// </param>
    /// <param name='walls'>
    ///     Les WallVectors du niveau. Ce sont les murs que le tank doit éviter.
    /// </param>
    /// <returns>
    ///     Le WallVector le plus proche
    /// </returns>
    private WallVector findFirstObstacle(Vector3 startPoint, Vector3 endPoint, WallVector[] walls)
    {
        WallVector closestObstacle = null;                                                                          //Le mur obstacle le plus proche
        float minDistance = Mathf.Infinity;                                                                         //Distance du mur obstacle le plus proche

        //Créer une droite entre le point et la cible
        Vector2 startPoint2D = new Vector2(startPoint.x, startPoint.z);
        Vector2 endPoint2D = new Vector2(endPoint.x, endPoint.z);
        Droite2D droiteToTarget = new Droite2D(startPoint2D, endPoint2D, true);                                     

        //Vérifier les points d'intersections avec tous les murs de la scène
        foreach (WallVector wall in walls)
        {
            if (wall.startPoint != wall.endPoint)
            {
                //Créer la droite du mur
                Vector2 wallStartPoint2D = new Vector2(wall.startPoint.x, wall.startPoint.z);
                Vector2 wallEndPoint2D = new Vector2(wall.endPoint.x, wall.endPoint.z);
                Droite2D droiteMur = new Droite2D(wallStartPoint2D, wallEndPoint2D, true);                                

                //Trouver le point d'intersection
                Vector3 intersection;
                if(findIntersection(droiteToTarget, droiteMur, out intersection))
                {
                    //Vérifier cet obstacle est plus proche que le précédent obstacle le plus proche
                    if(Vector3.Distance(startPoint, intersection) < minDistance)
                    {
                        minDistance = Vector3.Distance(startPoint, intersection);
                        closestObstacle = wall;
                    }
                }
            }
        }
        
        return closestObstacle;
    }

    /// <summary>
    ///     Calculer le point d'intersection entre deux droites.
    /// </summary>
    /// <param name='droiteTarget'>
    ///     La droite qui représente le direction d'un point à une cible
    /// </param>
    /// <param name='droiteWall'>
    ///     La droite qui représente un WallVector
    /// </param>
    /// <param name='intersection'>
    ///     Le point d'intersection des deux droites. S'il n'y a pas d'intersection, la valeur est Vector3.zero.
    /// </param>
    /// <returns>
    ///     True s'il y a un point d'intersection. False sinon.
    /// </returns>
    private bool findIntersection(Droite2D droiteTarget, Droite2D droiteWall, out Vector3 intersection)
    {
        bool hasIntersection = false;
        intersection = Vector3.zero;

        //Trouver le point d'intersection entre les deux droites
        Vector2 intersection2D;
        if(Droite2D.Intersect(droiteTarget, droiteWall, out intersection2D))
        {
            //Vérifier si le point d'intersection est entre les bornes
            if(droiteTarget.isPointWithinBorders(intersection2D) && droiteWall.isPointWithinBorders(intersection2D))
            {
                intersection = new Vector3(intersection2D.x, 0.5f, intersection2D.y);
                hasIntersection = true;
            }
        } 

        return hasIntersection;
    }

}

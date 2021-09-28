using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Cette classe représente le chemin d'un tank à travers le niveau. Elle contient la liste
    des waypoints que le tank doit emprunter.
 */

public class Path
{
    private List<Vector3> _waypoints;
    private float pointDrawingSize = 0.1f;

    public Path(List<Vector3> waypoints)
    {
        _waypoints = waypoints;
    }
    public Path(Path path)
    {
        _waypoints = new List<Vector3>();
        foreach (Vector3 waypoint in path.waypoints)
        {
            _waypoints.Add(waypoint);
        }
    }
    public Path(Vector3 start, Vector3 end)
    {
        _waypoints = new List<Vector3>();
        _waypoints.Add(start);
        _waypoints.Add(end);
    }

    /// <summary>
    /// Fusionne deux path. Le nouveau path est ajouté à partir de l'index donné. Le nouveau path doit commencer par le même
    /// waypoint que celui à l'index donné.
    /// </summary>
    /// <param name="path">Le path a ajouter</param>
    /// <param name="index">L'index du waypoint à partir du quel ajouter le nouveau path</param>
    /// <returns>True si la fusion a réussi, false sinon</returns>
    public bool fuseFromIndex(Path path, int index)
    {
        bool hasFuse = false;

        if (path.waypoints[0] == waypoints[index])
        {
            for (int i = 1; i < path.size; i++)
            {
                _waypoints.Insert(index + i, path.waypoints[i]);
            }

            hasFuse = true;
        }

        return hasFuse;
    }

    
    /// <summary>
    ///     Dessiner le chemin sur la map. Utiliser pour le debug.
    /// </summary>
    public void draw(Color color)
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = color;

        Vector3 precedentWaypoint = Vector3.zero;
        foreach(Vector3 waypoint in waypoints)
        {
            //Dessiner une ligne entre les waypoints
            if(precedentWaypoint != Vector3.zero)
            {
                DebugExtension.drawArrow(precedentWaypoint, waypoint - precedentWaypoint, Gizmos.color);
            }
            precedentWaypoint = waypoint;

            //Dessiner le waypoint
            Gizmos.DrawSphere(waypoint, pointDrawingSize);
        }

        Gizmos.color = oldColor;
    }

#region LIST METHOD
    /// <summary>
    ///     Ajouter un waypoint au path.
    /// </summary>
    /// <param name='waypoint'>
    ///     Le waypoint à ajouter.
    /// </param>
    /// <param name='index'>
    ///     L'index auquel ajouter le waypoint dans la liste.
    /// </param>
    public void addWaypoint(Vector3 waypoint, int index)
    {
        _waypoints.Insert(index, waypoint);
    }

    /// <summary>
    ///     Enlever un waypoint au path.
    /// </summary>
    /// <param name='waypoint'>
    ///     Le waypoint à enlever.
    /// </param>
    public void removeWaypoint(Vector3 waypoint)
    {
        _waypoints.Remove(waypoint);
    }

    /// <summary>
    ///     Ajouter un waypoint au path.
    /// </summary>
    /// <param name='index'>
    ///     L'index du waypoint à enlever.
    /// </param>
    public void removeWaypointAtIndex(int index)
    {
        _waypoints.RemoveAt(index);
    }

    /// <summary>
    ///     Vérifier si le path contient un waypoint.
    /// </summary>
    /// <param name='waypoint'>
    ///     Le waypoint à vérifier.
    /// </param>
    /// <returns>
    ///     True si le path contient le waypoint, false sinon.
    /// </returns>
    public bool contains(Vector3 waypoint)
    {
        return _waypoints.Contains(waypoint);
    }

    /// <summary>
    ///     Retourner l'index d'un waypoint.
    /// </summary>
    /// <param name='waypoint'>
    ///     Le waypoint dont on cherche l'index.
    /// </param>
    /// <returns>
    ///     L'index du waypoint. -1 si le waypoint ne fait pas partie de la liste.
    /// </returns>
    public int getWaypointIndex(Vector3 waypoint)
    {
        bool hasFound = false;
        int index = 0;

        while(!hasFound && index<waypoints.Length)
        {
            if(waypoint.Equals(waypoints[index]))
            {
                hasFound = true;
            }
            else
            {
                index++;
            }
        }

        if(!hasFound)
        {
            index = -1;
        }
        return index;
    }

    /// <summary>
    ///     Retourner le waypoint à l'index donné.
    /// </summary>
    /// <param name='index'>
    ///     L'index du waypoint.
    /// </param>
    /// <returns>
    ///     Le waypoint à l'index donné. Vector3.zero s'il n'y a pas de waypoint à l'index donné.
    /// </returns>
    public Vector3 getWaypointAtIndex(int index)
    {
        return waypoints[index];
    }
#endregion

#region GETTER/SETTER
    public Vector3[] waypoints
	{
		get
		{
			return _waypoints.ToArray();
		}
	}
    public int size
    {
        get
        {
            return waypoints.Length;
        }
    } 
    public float distance
    {
        get
        {
            float distance = 0;

            for(int i = 0; i<waypoints.Length-1; i++)
            {
                distance += Vector3.Distance(waypoints[i], waypoints[i + 1]);
            }

            return distance;
        }
    }
#endregion
}

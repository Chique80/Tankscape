using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Ce script est utilisé pour créer des objets permettant de teste l'AI des tanks. Le script permet à un tank de se déplacer
	automatiquement sur un chemin composer de plusieurs waypoints.
 */

public class AutomaticMovingTank : MonoBehaviour 
{
	[Header("Targets")]
	public bool useReversePathing = false;
	public float distanceToTarget;
	public List<GameObject> targets;
	private int currentTargetIndex;
	private bool isPathingBack;

	[Header("Component")]
	public EnemyAIValue valueScript;

	// Use this for initialization
	void Start () 
	{
		currentTargetIndex = 0;
		isPathingBack = false;

		changeTarget();
	}
	 
	// Update is called once per frame
	void Update () 
	{
		//Vérifier la distance à la cible
		if(Vector3.Distance(transform.position, currentTargetPosition) <= distanceToTarget)
		{
			changeTarget();
		}
	}

	void OnDrawGizmosSelected()
	{
		if(distanceToTarget > 0)
		{
			DebugExtension.DrawCircle(transform.position, Vector3.up, Color.white, distanceToTarget);
		}
	}

	private void changeTarget()
	{
		//Changer la cible
		if(isPathingBack)
		{
			currentTargetIndex--;
		}
		else
		{
			currentTargetIndex++;
		}

		if(currentTargetIndex >= targets.Count)
		{
			if(useReversePathing)
			{
				currentTargetIndex = targets.Count - 1;
				isPathingBack = true;
			}
			currentTargetIndex = 0;
		}
		else if(currentTargetIndex < 0 && useReversePathing)
		{
			currentTargetIndex = 1;
			isPathingBack = false;
		}

		//Asigner la cible à l'ai
		if(valueScript != null)
		{
			valueScript.playerTank = currentTarget;
		}
	}

#region GETTER/SETTER
	public GameObject currentTarget
	{
		get
		{
			return targets[currentTargetIndex];
		}
	}
	public Vector3 currentTargetPosition
	{
		get
		{
			return currentTarget.transform.position;
		}
	}
	
#endregion
}

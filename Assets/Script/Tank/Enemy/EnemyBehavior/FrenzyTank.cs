using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 */
public class FrenzyTank : AutomaticShootingTank 
{

	[Header("Frenzy")]
	public float deltaAngle = 25;
	public float rotateTurretRate = 0.2f;
	private float rotateTurretTimer;
 
	void OnDrawGizmosSelected()
	{
		Vector3 directionA = GeneralFunction.rotateVector(transform.forward, -deltaAngle * Mathf.Deg2Rad);
		Vector3 directionB = GeneralFunction.rotateVector(transform.forward, deltaAngle * Mathf.Deg2Rad);

		DebugExtension.drawArrow(transform.position, directionA.normalized*2, Color.green);
		DebugExtension.drawArrow(transform.position, directionB.normalized*2, Color.green);
	}

	/// <summary>
	///		Tourner la tourelle pour qu'elle fasse face à une direction aléatoire. La direction aléatoire est choisit parmi un écart
	///			d'angle centré sur le joueur.
	///	</summary>
	protected override void rotateTurret()
	{
		rotateTurretTimer += Time.fixedDeltaTime;			//Update timer

		//Tourner la tourelle
		if(rotateTurretTimer >= rotateTurretRate)
		{
			//Générer une direction aléatoire
			float rndAngle = Random.Range(-deltaAngle, deltaAngle) * Mathf.Deg2Rad;
			Vector3 direction = GeneralFunction.rotateVector(-targetDirection, rndAngle);
					
			turret.transform.forward = direction;			
			
			rotateTurretTimer = 0;
		}
	}


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 */
public class ArtilleryTank : EnemyBehavior 
{
	private EnemyAIValue valueScript;
	public GameObject cannon;

	[Header("Shooting")]
	public float fireRate;
	private float fireTimer;
	public float shootingRange;
	public float minShootingRange;
	public GameObject projectile;

	[Header("Cannon Ball Trajectory")]
	public float maxFlyTime;
	public float minFlyTime;
	public float spawnHeight;
	private float spawnDistance;


	// Use this for initialization
	void Start ()
	{
		//Find the EnemyAIValue.cs script
        valueScript = GetComponent<EnemyAIValue>();
        if(valueScript == null)
        {
            Debug.LogError(this + " doesn't have a EnemyAIValue.cs script!");
        }

		//Make sure the script has a cannon
		if(cannon == null)
		{
			Debug.LogError(this + " is missing a reference to a cannon object!");
		}

		//Calculer la distance à la laquelle créer le projectile
		if(cannon != null)
		{
			if(cannon.GetComponentInChildren<Transform>() != null)
			{
				spawnDistance = cannon.GetComponentInChildren<Transform>().localScale.z + cannon.transform.localScale.z + projectile.transform.localScale.z;
			}
		}
		else
		{
			spawnDistance = 0;
		}

		//Initialiser le timer
		fireTimer = fireRate/2;
	}
	
	// Update is called once per frame
	void Update () 
	{
		fireTimer += Time.fixedDeltaTime;

		if(valueScript != null && valueScript.state != EnemyAIValue.State.Search)
		{
			//Orienter le canon
			rotateCannon();

			//Tirer à la position du joueur
			if(fireTimer >= fireRate && valueScript.distanceToPlayer <= shootingRange && valueScript.distanceToPlayer >= minShootingRange)
			{
				fireCannon();
				fireTimer = 0;
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		//Dessiner la portée de tir
		DebugExtension.DrawCircle(transform.position, Vector3.up, Color.green, shootingRange);
		DebugExtension.DrawCircle(transform.position, Vector3.up, Color.red, minShootingRange);

		//Dessiner spawnPos et foward
		if(cannon != null)
		{
			DebugExtension.drawArrow(cannon.transform.position, cannon.transform.forward, Color.white);
		}
	}
 
#region OVERRIDE
	public override void enable()
	{
		this.enabled = true;
	}
	public override void disable()
	{
		this.enabled = false;
	}
#endregion

	/// <summary>
	/// 	Trouver le canon du tank pour qu'il fasse face à au jour. L'ortientation verticale du tank est aussi modifiée.
	/// </summary>
	private void rotateCannon()
	{
		if(cannon != null)
		{
			Vector3 cannonDirection = valueScript.playerDirection;
			cannonDirection.y = spawnHeight;
			cannonDirection.Normalize();

			cannon.transform.forward = cannonDirection;
		}
		
	}

	/// <summary>
	/// 	Tirer un projectile sur le joueur.
	/// </summary>
	private void fireCannon()
	{
		if(cannon != null)
		{
			//Calculer la position de création du projectile
			Vector3 spawnPos = cannon.transform.position + (cannon.transform.forward * spawnDistance);

			//Calculer le temps de vol du projectile
			float flyTime = maxFlyTime * valueScript.distanceToPlayer / shootingRange;
			if(flyTime < minFlyTime)
			{
				flyTime = minFlyTime;
			}

			//Créer le projectile et lui assigner sa point d'impact et sa durée de vol
			GameObject cannonBall = Instantiate(projectile, spawnPos, Quaternion.identity);
			if(cannonBall.GetComponent<CannonBallMouvement>() != null)
			{
				cannonBall.GetComponent<CannonBallMouvement>().landPosition = valueScript.playerPosition;
				cannonBall.GetComponent<CannonBallMouvement>().flyTime = flyTime;
			}
		}
	}
}

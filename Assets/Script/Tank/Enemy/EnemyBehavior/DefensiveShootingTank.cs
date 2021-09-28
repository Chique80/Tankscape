using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveShootingTank : ShootingTank 
{
	[Header("Defensive Shooting")]
	public float shootingAngle;
	public float shootingRange;
	public float detectionRange;
	
	// Update is called once per frame
	void Update () 
	{
		fireTimer += Time.fixedDeltaTime;								//Update le timer

		//Trouver une cible
		if(target==null || fireTimer>=fireRate)
		{
			findTarget();
		}

		//Orienter la tourelle
		rotateTurret();

		//Tirer
		if (fireTimer >= fireRate)
        {
            if(shootTarget())
            {
                fireTimer = 0;
            }
        }
	}

	void OnDrawGizmosSelected()
	{
		Vector3 directionA = Vector3.zero;
		Vector3 directionB = Vector3.zero;

		//Calculer les deux vecteurs représentants l'angle de tir
		if(Application.isPlaying)
		{
			directionA = GeneralFunction.rotateVector(targetDirection.normalized, shootingAngle*Mathf.Deg2Rad);
			directionB = GeneralFunction.rotateVector(targetDirection.normalized, -shootingAngle*Mathf.Deg2Rad);
		}
		else
		{
			directionA = GeneralFunction.rotateVector(transform.forward, shootingAngle*Mathf.Deg2Rad);
			directionB = GeneralFunction.rotateVector(transform.forward, -shootingAngle*Mathf.Deg2Rad);
		}

		//Dessiner la portée de tir
		Gizmos.color = Color.yellow;
		DebugExtension.DrawCircle(transform.position, Vector3.up, Gizmos.color, shootingRange);
		
		//Dessiner l'angle de tir
		Gizmos.color = Color.gray;
		Gizmos.DrawRay(transform.position, directionA * detectionRange);
		Gizmos.DrawRay(transform.position, directionB * detectionRange);

		//Tracer un trait vers la cible
		if(targetPosition != Vector3.zero)
		{
			Gizmos.DrawLine(transform.position, targetPosition);
		}
	}

	/// <summary>
	/// 	Trouver une cible à tirer. La cible par défaut est le joueur. Si un projectile se trouve proche du tank, le tank le
	///			prend pour cible.
	/// </summary>
	private void findTarget()
	{
		if(valueScript != null && valueScript.state != EnemyAIValue.State.Search)
		{
			target = valueScript.playerTank;												//Assigner le joueur comme cible initiale

			GameObject[] projectiles = detectProjectilesInShootingRange();					//Trouver tous les projectiles à portée de tir

			//Vérifier s'il y a des projectiles
			if(projectiles.Length > 0)
			{
				float minDistance = Vector3.Distance(targetPosition, transform.position);			//Assigner la distance entre le joueur et le tank comme distance initiale

				//Trouver le projectile le plus proche
				foreach(GameObject projectile in projectiles)
				{
					float distance = Vector3.Distance(projectile.transform.position, transform.position);
					if(distance < minDistance)
					{
						minDistance = distance;
						target = projectile;
					}
				}
			}
			else
			{
				//Vérifier s'il y a des projectiles qui se dirige vers le joueur
				projectiles = detectProjectilesInDetectionRange();
				if(projectiles.Length > 0)
				{
					Debug.Log("A projectile is too close!");
					target = null;																			//Ne pas tirer
				}
			}

		}
	}
 
	/// <summary>
	/// 	Détecter les projectiles qui sont à proximité du tank. La méthode ne détecte que les projectiles qui sont dans
	///			un certain cone de vision.
	/// </summary>
	///	<returns>
	///		La liste des projectiles détectés.
	/// </returns>
	private GameObject[] detectProjectilesInShootingRange()
	{
		List<GameObject> projectiles = new List<GameObject>();

		//Trouver tous les projectiles dans la portée de tir
		Collider[] colliders = Physics.OverlapSphere(transform.position, shootingRange);
		foreach(Collider collider in colliders)
		{
			if(collider.GetComponent<Projectile_Mouvement>() != null)
			{
				//Calculer la direction vers le projectile
				Vector3 directionToProjectile = collider.transform.position - transform.position;
				directionToProjectile.y = 0;

				//Vérifier si le projectile se trouve dans l'angle de tir
				if(Vector3.Angle(targetDirection, directionToProjectile) <= shootingAngle)
				{
					projectiles.Add(collider.gameObject);
				}
			}
		}

		return projectiles.ToArray();
	}

	/// <summary>
	/// 	Détecter les projectiles qui sont à une certaines distance devant le tank. La méthode ne détecte que les projectiles qui sont dans
	///			un certain cone de vision et qui se dirige vers le tank.
	/// </summary>
	///	<returns>
	///		La liste des projectiles détectés.
	/// </returns>
	private GameObject[] detectProjectilesInDetectionRange()
	{
		List<GameObject> projectiles = new List<GameObject>();

		//Trouver tous les projectiles dans la portée de tir
		Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
		foreach(Collider collider in colliders)
		{
			if(collider.GetComponent<Projectile_Mouvement>() != null)
			{
				//Calculer la direction vers le projectile
				Vector3 directionToProjectile = collider.transform.position - transform.position;
				directionToProjectile.y = 0;

				//Vérifier si le projectile se trouve dans l'angle de tir
				if(Vector3.Angle(targetDirection, directionToProjectile) <= shootingAngle)
				{
					Vector3 projectileDirection = collider.GetComponent<Projectile_Mouvement>().projectileDirection;
					float distance = Vector3.Distance(transform.position, collider.transform.position);

					//Vérifier si le projectile se dirige vers le joueur
					RaycastHit hit;
					if(Physics.Raycast(collider.transform.position, projectileDirection, out hit, distance))
					{
						if(hit.collider.gameObject == this.gameObject)
						{
							projectiles.Add(collider.gameObject);
						}
					}				
				}
			}
		}

		return projectiles.ToArray();
	}
}

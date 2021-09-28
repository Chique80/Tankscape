using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallMouvement : MonoBehaviour 
{
	[Header("Explosion")]
	public float explosionRange;
	public GameObject explosionEffect;
	public GameObject groundTargetPrefab;
	private GameObject groundTarget;

	[Header("Mouvement")]
	public float flyTime;
	public Vector3 landPosition = Vector3.zero;
    private Vector3 velocity;
	private bool hasTrajectory;

	// Use this for initialization
	void Start () 
	{
		hasTrajectory = false;

		//Calculer la vitesse du boulet
		if(landPosition != Vector3.zero)
		{
			calculateVelocity();

			//Créer la cible
			if(groundTargetPrefab != null)
			{
				groundTarget = Instantiate(groundTargetPrefab, landPosition, Quaternion.Euler(90,0,0));
				groundTarget.transform.localScale = new Vector3(explosionRange, explosionRange, 1);
			}
		}
	}
	 
	// Update is called once per frame
	void FixedUpdate () 
	{
		//Calculer la vitesse du boulet
		if(landPosition != Vector3.zero && !hasTrajectory)
		{
			calculateVelocity();

			//Créer la cible
			if(groundTargetPrefab != null)
			{
				groundTarget = Instantiate(groundTargetPrefab, landPosition, Quaternion.Euler(90,0,0));
			}
		}

		if(hasTrajectory)
		{
            move();
			
			if(transform.position.y <= 0)
			{
				explode();
			}
		}	
	}

	void OnDrawGizmosSelected()
	{
		//Dessiner la zone d'explosion
		if(landPosition != Vector3.zero)
		{
			DebugExtension.DrawCircle(landPosition, Vector3.up, Color.green, explosionRange);
		}
	}

    private void calculateVelocity()
	{
		//Calculer la vitesse horizontal
		float horizontalSpeed = horizontalTravelDistance / flyTime;

		//Calculer la vitesse vertical
		float deplacementVertical = -transform.position.y;
		float verticalSpeed = (deplacementVertical - 0.5f*Physics.gravity.y*Mathf.Pow(flyTime, 2)) / flyTime;

		//Calculer le vecteur de vélocité horizontale
		Vector3 directionToLanding = landPosition - transform.position;
		directionToLanding.y = 0;
		directionToLanding = directionToLanding.normalized * horizontalSpeed;

		//Calculer le vecteur de vélocité verticale
		velocity = new Vector3(directionToLanding.x, verticalSpeed, directionToLanding.z);
		hasTrajectory = true;
	}

    private void move()
    {
        //Update la velocité
        velocity.y = velocity.y + (Physics.gravity.y * Time.fixedDeltaTime);

        //Bouger le projectile
        transform.Translate(new Vector3(velocity.x, 0, velocity.z) * Time.fixedDeltaTime, Space.World);
        transform.Translate(Vector3.up * velocity.y * Time.fixedDeltaTime, Space.World);
    }

    private void explode()
	{
		//Créer l'effet d'explosion
		if(explosionEffect != null)
		{
			GameObject explosion = Instantiate(explosionEffect, landPosition, Quaternion.identity);
			explosion.transform.localScale *= explosionRange;
		}

		//Tuer le joueur
		Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange);
		foreach(Collider collider in colliders)
		{
			if(collider.GetComponent<PlayerMain>() != null)
			{
				collider.GetComponent<PlayerMain>().kill();
			}
		}

		//Détruire la cible
		if(groundTarget != null)
		{
			Destroy(groundTarget);
		}

		//Détruire le boulet
		Destroy(this.gameObject);
	}

#region GETTER/SETTER
	public float horizontalTravelDistance
	{
		get
		{
			float distance = 0;

			if(landPosition != Vector3.zero)
			{
				Vector3 positionIn2D = new Vector3(transform.position.x, 0, transform.position.z);
				Vector3 landPositionIn2D = new Vector3(landPosition.x, 0, landPosition.z);
				distance = Vector3.Distance(positionIn2D, landPositionIn2D);
			}

			return distance;
		}
	}
#endregion
}

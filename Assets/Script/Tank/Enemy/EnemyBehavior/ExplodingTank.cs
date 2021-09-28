using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingTank : EnemyBehavior 
{
	private EnemyAIValue valueScript;

	[Header("Explosion")]
	public float explosionRange;
	public float delay;
	public bool cancelExplosion;
	private float explosionTimer;
	private bool isExploding;

	[Header("Component")]
	public GameObject explosion;
	public GameObject overchargingEffectPrefab;
	private GameObject overchargingEffectObject;
	public GameObject sparkEffect;


	// Use this for initialization
	void Start () 
	{
		//Find the EnemyAIValue.sc script
		valueScript = GetComponent<EnemyAIValue>();
		if(valueScript == null)
		{
			Debug.Log(this + " doesn't have a EnemyAIValue.cs script!");
		}

		isExploding = false;
		explosionTimer = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(valueScript != null && valueScript.state != EnemyAIValue.State.Search)
		{
			//Vérifier si le joueur est assez proche
			if(valueScript.distanceToPlayer <= explosionRange)
			{
				//Commmencer le chargement de l'explosion
				isExploding = true;

				if(overchargingEffectPrefab != null && overchargingEffectObject == null)
				{
					overchargingEffectObject = Instantiate(overchargingEffectPrefab, transform.position, Quaternion.Euler(-90,0,0));
					overchargingEffectObject.transform.parent = gameObject.transform;
					overchargingEffectObject.transform.position = new Vector3(overchargingEffectObject.transform.position.x,
						overchargingEffectObject.transform.position.y - 1,
						overchargingEffectObject.transform.position.z);
				}
					
			}
			//Annuler l'explosion
			else if(isExploding && cancelExplosion)
			{
				isExploding = false;
				explosionTimer = 0;

				if(overchargingEffectObject != null)
				{
					Destroy(overchargingEffectObject);
				}
			}
			
			//Check the detonation timer
			if(isExploding)
			{
				explosionTimer += Time.fixedDeltaTime;
				if(explosionTimer >= delay)
				{
					explode();
				}
			}
		}
		//Annuler l'explosion
		else if(isExploding && cancelExplosion)
		{
			isExploding = false;
			explosionTimer = 0;

			if(overchargingEffectObject != null)
			{
				Destroy(overchargingEffectObject);
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		if(explosionRange > 0)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, explosionRange);
		}
	}

	// Called when the gameobject is destroyed
    void OnDestroy()
    {
        if(overchargingEffectObject != null)
        {
            Destroy(overchargingEffectObject);
        }
    }

#region OVERRIDE
	public override void enable()
	{
		this.enabled = true;

		//Activer l'effet d'overcharging
        if (overchargingEffectObject != null)
        {
            overchargingEffectObject.GetComponent<ParticleSystem>().Play();
        }

        //Désactiver les sparks
        if (sparkEffect != null)
        {
            sparkEffect.GetComponent<ParticleSystem>().Play();
        }
	}

	public override void disable()
	{
		this.enabled = false;

		//Désactiver l'effet d'overcharging
        if (overchargingEffectObject != null)
        {
            overchargingEffectObject.GetComponent<ParticleSystem>().Pause();
        }

        //Désactiver les sparks
        if (sparkEffect != null)
        {
            sparkEffect.GetComponent<ParticleSystem>().Pause();
        }
	}
#endregion

	/// <summary>
	///		Détruire le tank en le faisant exploser. Tuer le joueur s'il se trouve à portée.
	/// </summary>
    protected void explode()
	{
		if(explosion != null)
		{
			Instantiate(explosion, transform.position, Quaternion.identity);
			
			isExploding = false;
			explosionTimer = 0;

			if(overchargingEffectObject != null)
			{
				Destroy(overchargingEffectObject);
			}
		}

		//Check if the player is in the explosion
		if(valueScript != null && valueScript.playerTank != null)
		{
			if(valueScript.distanceToPlayer <= explosionRange)
			{
				valueScript.playerTank.GetComponent<PlayerMain>().kill();
			}
		}
		
		GetComponent<EnemyMain>().kill();
	}
   
}

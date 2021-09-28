using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 */
public class RocketTank : EnemyBehavior 
{
	private EnemyAIValue valueScript;

	[Header("Speed Burst")]
	public int bonusSpeed;
	public float burstDuration;
	public int nbOfUse;
	private float burstStartDelay = 0.5f;
	private bool isInBurst;

	[Header("Component")]
	public GameObject burstEffect;

	// Use this for initialization
	void Start () 
	{
		//Find the EnemyAIValue.cs script
        valueScript = GetComponent<EnemyAIValue>();
        if(valueScript == null)
        {
            Debug.LogError(this + " doesn't have a EnemyAIValue.cs script!");
        }

		//Désactiver l'effet de burst
		if(burstEffect != null)
		{
			burstEffect.SetActive(false);
		}
		isInBurst = false;	
	}
	 
	// Update is called once per frame
	void Update () 
	{
		if(valueScript != null)
		{
			//Lorsque le tank est en burst
			if(isInBurst)
			{
				burstDuration -= Time.fixedDeltaTime;

				if(burstDuration <= 0)
				{
					//End the burst
					isInBurst = false;
					valueScript.speed -= bonusSpeed;
                    valueScript.mouvementUpdateRate *= 2;

					//End the effect
					if(burstEffect != null)
					{
						burstEffect.SetActive(false);
					}
				}
			}

			//Lorsque le tank n'est pas en burst
			else if(nbOfUse > 0)
			{
				//Vérifier si le joueur est en vue
				if(valueScript.state == EnemyAIValue.State.Move)
				{
					burstStartDelay -= Time.fixedDeltaTime;
					if(burstStartDelay <= 0)
					{
						//Start the burst
						isInBurst = true;
						valueScript.speed += bonusSpeed;
                        valueScript.mouvementUpdateRate /= 2;
                        nbOfUse--;
						burstStartDelay = 0.5f;

						//Start the effect
						if(burstEffect != null)
						{
							burstEffect.SetActive(true);
							burstEffect.GetComponent<ParticleSystem>().Play();
						}
					}
				}
				else
				{
					burstStartDelay = 0.5f;
				}
			}
		}
	}

#region OVERRIDE
	public override void enable()
	{
		this.enabled = true;

		//Activer l'effet de burst
		if(isInBurst == true && burstEffect!=null)
		{
			burstEffect.SetActive(true);
		}
	}

	public override void disable()
	{
		this.enabled = false;

		//Désactiver l'effet de burst
		if(isInBurst == true && burstEffect!=null)
		{
			burstEffect.SetActive(false);
		}
	}
#endregion
}

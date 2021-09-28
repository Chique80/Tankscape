using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingBlock : MonoBehaviour 
{
	[Header("Healing")]
	public int healing = 2;
	public GameObject healingEffect;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.GetComponent<PlayerMain>() != null)
		{
			//Trouver le GameManagerScript.cs script
			GameObject manager = GameObject.FindGameObjectWithTag("GameManager");
			if(manager != null)
			{
				GameManagerScript gameManager = manager.GetComponent<GameManagerScript>();
				if(gameManager != null)
				{
					gameManager.playerHp += healing;																//Redonner des points de vie au joueur

					if(healingEffect != null)
					{
						Instantiate(healingEffect, transform.position, Quaternion.identity);
					}

					Destroy(this.gameObject);																		//Détruire le cube
				}
			} 
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMain : MonoBehaviour
{
    protected bool isPaused;
    public LevelManager levelManager;

    [Header("Stats")]
    public bool isImmortal;
    public int maxHp;
    protected int hp;

    [Header("Component")]
    public GameObject destructionEffect;


	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<Projectile_Mouvement>() != null)
        {
            Debug.Log(this + " has been hit by a projectile");
            reduceHP(1);
        }
    }
 
#region VIRTUAL_METHOD
    public virtual void pause()
    {
        isPaused = true;
    }
    public virtual void play()
    {
        isPaused = false;
    }

#endregion

    public void reduceHP(int damage)
    {
        if(!isImmortal)
        {
            hp -= damage;

            if(hp <= 0)
            {
                if(levelManager != null)
                {
                    levelManager.removeTank(this.gameObject);
                }

                //Détruire le tank
                if(destructionEffect != null)
                {
                    Instantiate(destructionEffect, transform.position, Quaternion.identity);
                }
                Destroy(this.gameObject);
            }
        }
    }

    public void kill()
    {
        reduceHP(maxHp);
    }
}
    
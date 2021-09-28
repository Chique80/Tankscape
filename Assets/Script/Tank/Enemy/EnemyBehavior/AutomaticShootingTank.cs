using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticShootingTank : ShootingTank 
{
	 
	// Update is called once per frame
	void Update () 
	{
		fireTimer += Time.fixedDeltaTime;                                                       //Update le timer

		//Trouver la cible
		if(valueScript != null)
        {
            if (valueScript.state != EnemyAIValue.State.Search)
            {
                target = valueScript.playerTank;
                isTargetInSight();
            }
        }

		//Orienter la tourelle
		rotateTurret();																	

        //Vérifier si le tank doit tirer sur sa cible
        if (fireTimer >= fireRate)
        {
            if(shootTarget())
            {
                fireTimer = 0;
            }
        }
	}
}

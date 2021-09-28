using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 */
public class EnemyMain : TankMain 
{
    [Header("Pointage")]
    public int point;

	// Use this for initialization
	void Start () 
	{
		//Initialiser les points de vie
        hp = maxHp;
	}

#region OVERRIDE
    public override void pause()
    {
        //Empêcher le tank de bouget
        EnemyAIValue valueScript = GetComponent<EnemyAIValue>();
        if(valueScript != null)
        {
            if(valueScript.hasMouvementScript)
            {
                valueScript.enemyMouvement.canMove = false;
            }
        }
        
        //Désactiver les scripts de comportement
        EnemyBehavior[] behaviorScripts = GetComponents<EnemyBehavior>();
        foreach(EnemyBehavior behaviorScript in behaviorScripts)
        {
            behaviorScript.disable();
        }
    }
 
    public override void play()
    {
        //Permettre au tank de bouger
        EnemyAIValue valueScript = GetComponent<EnemyAIValue>();
        if(valueScript != null)
        {
            if(valueScript.hasMouvementScript)
            {
                valueScript.enemyMouvement.canMove = true;
            }
        }

        //Activer les scripts de comportement
        EnemyBehavior[] behaviorScripts = GetComponents<EnemyBehavior>();
        foreach(EnemyBehavior behaviorScript in behaviorScripts)
        {
            behaviorScript.enable();
        }
    }
#endregion
}

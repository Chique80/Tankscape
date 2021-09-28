using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Ce script est utilisé dans les niveaux de test de l'AI. Il contient des inputs personnalisés au niveau de test.
 */

public class TestEnemyManager : MonoBehaviour {

    public LayerMask groundLayer;

    [Header("Shooting")]
    public GameObject projectile;
    private Vector3 projectileSpawnPosition;
    private Vector3 projectileEndPosition;

	[Header("Component")]
	public GameObject enemyTarget;
	public GameObject pin;
	public GameObject enemyTank;

	// Use this for initialization
	void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update () 
	{
		manageInput();
	}

	private void manageInput()
	{
		if(Input.GetMouseButtonDown(1))
		{
			moveEnemyTarget();
		}

        if(Input.GetMouseButtonDown(0))
        {
            shootPlayer();
        }
	}

	private void moveEnemyTarget()
	{
		Vector3 spawnPos = GeneralFunction.PositionMouse(groundLayer);
        spawnPos.y = enemyTank.transform.position.y;

        if(enemyTank.GetComponent<EnemyAIValue>().playerTank != null)
        {
            GameObject.Destroy(enemyTank.GetComponent<EnemyAIValue>().playerTank);
        }
        enemyTank.GetComponent<EnemyAIValue>().setNewPlayerTank(Instantiate(enemyTarget, spawnPos, Quaternion.identity));
	}
    
    private void shootPlayer()
    {
        if (enemyTank != null && projectile != null)
        {
            projectileSpawnPosition = GeneralFunction.PositionMouse(groundLayer);
            projectileSpawnPosition.y = 0.5f;

            projectileEndPosition = enemyTank.transform.position;
            shoot();
        }
    }
 
    private void shoot()
    {
        Vector3 direction = projectileSpawnPosition - projectileEndPosition;
        Instantiate(projectile, projectileSpawnPosition, Quaternion.LookRotation(direction));

        projectileSpawnPosition = Vector3.zero;
        projectileEndPosition = Vector3.zero;
    }

}

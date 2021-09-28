using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAutoTarget : MonoBehaviour 
{
	[Header("Info")]
	public float rotationSpeed;
	private Vector3 directionToTarget;	


	[Header("Component")]
	public GameObject target;
	private GameObject turret;


	// Use this for initialization
	void Start () 
	{
        //Trouver la tourelle du tank
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach (Transform transform in transforms)
        {
            if (transform.gameObject.name == "body" || transform.gameObject.name == "Body")
            {
                turret = transform.gameObject;
            }
        }
	}
	 
	// Update is called once per frame
	void Update () 
	{
		//Calculer la direction vers la cible
		if(target != null)
		{
			directionToTarget = (target.transform.position - transform.position).normalized;
		}
		else
		{
			directionToTarget = Vector3.zero;
		}

        rotateTurret();																//Tourner la tourelle
	}

	private void rotateTurret()
	{
		if(turret != null && directionToTarget != Vector3.zero)
		{
			turret.transform.forward = -directionToTarget;
		}
	}
}

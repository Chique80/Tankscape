using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMainTest : TankMain
{
    public Vector3 tankDirection;
    public float speed;

	// Use this for initialization
	void Start ()
    {
        Debug.Log(GetComponent<TankMain>() == null);	
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
 
    private void OnDrawGizmosSelected()
    {
        if(tankDirection != Vector3.zero && speed != 0)
        {
            DebugExtension.drawArrow(transform.position, tankDirection.normalized * speed, Color.white);
            DebugExtension.DrawCircle(transform.position, Vector3.up, Color.white, speed);
        }
    }
}

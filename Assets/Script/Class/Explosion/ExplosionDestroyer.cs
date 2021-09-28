using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDestroyer : MonoBehaviour {

    [Header("Temps de vie")]
    public float tempsdeVieExplosion;

    [Header("Component")]
    public GameObject cloudSmoke;

    private float time;
    private bool siCloudSpawn = false;

    private Vector3 spawnPos;
    private Quaternion spawnRot;
   
 
    // Use this for initialization
    void Start () {

        cloudSmoke = Resources.Load("Smoke/Smoke") as GameObject;
    }
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;

        //Créer le cloud qui l'accompagne
        if (time > tempsdeVieExplosion/5 && !siCloudSpawn)
        {
            spawnerCloud();
            siCloudSpawn = true;
        }
        //Se détruit si son temps de vie est fini
        else if (time > tempsdeVieExplosion)
        {
            Destroy(gameObject);

        }
    }

    /// <summary>
    /// Place l'explosion dans l'espace
    /// </summary>
    /// <param name="pos"> La positon de l'explosion</param>
    /// <param name="rot"> La rotation de l'explosion</param>
    public void setValue(Vector3 pos, Quaternion rot)
    {
        spawnPos = pos;
        spawnRot = rot;
    }

    /// <summary>
    /// Créer le cloud qui vient avec lui
    /// </summary>
    public void spawnerCloud()
    {
        Instantiate(cloudSmoke, spawnPos, Quaternion.LookRotation(-transform.forward)); ;
    }
}

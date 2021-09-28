using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSpawner : MonoBehaviour {

    [Header("Explosion system")]
    public GameObject explosion;
    

    private Vector3 spawnPos;
    private Quaternion spawnRot;

    // Use this for initialization
    void Start () {

        //Load l'explosion
        explosion = Resources.Load("Explosion/Explosion") as GameObject;
        
        explosion = spawnerExplosion();           //Spawn l'explosion
        explosion.GetComponent<ExplosionDestroyer>().setValue(transform.position,transform.rotation);

        if (explosion.GetComponent<ExplosionDestroyer>() == null)
        {
            Debug.LogError(gameObject.name + " n'a pas pu trouvé l'explosion.");
        } 
    }

    /// <summary>
    /// Spawn l'explosion
    /// </summary>
    /// <returns> Le gameObject de l'explosion</returns>
    public GameObject spawnerExplosion()
    {
       return Instantiate(explosion, spawnPos, spawnRot);

    }


}

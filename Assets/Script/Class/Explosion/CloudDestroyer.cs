using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudDestroyer : MonoBehaviour {

    [Header("Temps de vie")]
    public float tempsdeVieCloud;

    private float time;
    


    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        //Détruit le cloud après la fin de son temps de vie
        if (time > tempsdeVieCloud)
        {
            Destroy(gameObject);
 
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : TankMain 
{
    [Header("Shooting")]
    public float fireRate;
    private float fireTimer;
    public GameObject projectile;
    private CreateProjectile createProjectile;

	// Use this for initialization
	void Start () 
    {
        createProjectile = gameObject.GetComponent<CreateProjectile>();

        //Initialiser des variables
        hp = maxHp;
        fireTimer = 0;
	}
	
	// Update is called once per frame
	void Update () 
    {
        fireTimer += Time.fixedDeltaTime;

        //Changer l'image du curseur
        if(fireTimer>=fireRate && fireRate<fireRate+Time.deltaTime)
        {
            
            if(levelManager.GetComponent<UIManager>())
            {
                Texture2D cursorEnabled = levelManager.GetComponent<UIManager>().cursorImageEnabled;
                Cursor.SetCursor(cursorEnabled, new Vector2(cursorEnabled.width/2, cursorEnabled.height/2), CursorMode.Auto);
            }
        }
	}
 
#region OVERRIDE
    public override void pause()
    {
        isPaused = true;

        GetComponent<MouvementPlayer>().enabled = false;
    }

    public override void play()
    {
        isPaused = false;

        GetComponent<MouvementPlayer>().enabled = true;
    }   

#endregion

    public void playerShoot()
    {
        if(fireTimer >= fireRate && !isPaused)
        {
            createProjectile.tirer(projectile);
            fireTimer = 0;

            //Changer l'image du curseur
            if(levelManager.GetComponent<UIManager>())
            {
                Texture2D cursorDisabled = levelManager.GetComponent<UIManager>().cursorImageDisable;
                Cursor.SetCursor(cursorDisabled, new Vector2(cursorDisabled.width/2, cursorDisabled.height/2), CursorMode.Auto);
            }
        }
    }
}

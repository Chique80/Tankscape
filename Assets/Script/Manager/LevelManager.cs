using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 */
public class LevelManager : MonoBehaviour 
{
    public enum LevelStatus
    {
        Loading,
        Playing,
        Completed,
        Failed,
        Paused
    }
    
	private GameManagerScript gameManager;

    [Header("Level state")]
    private LevelStatus _status;

    [Header("Loading")]
	public float delayAtStart = 3.5f;
    public float delayAtEnd = 3;

	[Header("Tank")]
	public GameObject player;
	public List<GameObject> enemies;

    //Pour empêcher de tirer après le unpause
    private bool siVientUnpause = false;
    private float timeDelaiFrameUnpause;
    private float tempsDattenteAvantUnpause = 1;
    

    // Use this for initialization
    void Start () 
	{
        _status = LevelStatus.Loading;

		//Trouver le GameManager.cs
		GameObject manager = GameObject.FindGameObjectWithTag("GameManager");
		if(manager != null)
		{
			gameManager = manager.GetComponent<GameManagerScript>();
		}

        //Désactiver tous les tanks
        player.GetComponent<PlayerMain>().pause();
		foreach(GameObject enemy in enemies)
		{
			enemy.GetComponent<EnemyMain>().pause();
		}
        
    }
	 
	// Update is called once per frame
	void Update () 
	{
		//Wait at the start of the level
		if(levelStatus == LevelStatus.Loading)
		{
			delayAtStart -= Time.fixedDeltaTime;
			if(delayAtStart <= 0)
			{
                startLevel();
			}
		}
		
        if(levelStatus==LevelStatus.Failed || levelStatus==LevelStatus.Completed)
        {
            delayAtEnd -= Time.fixedDeltaTime;
            if(delayAtEnd <= 0)
            {
                endLevel();
            }
            
        }
        
	}

    /// <summary>
    ///     Méthode appelé lorsqu'un tank est détruire. Enlève le tank de la liste si c'est un ennemi ou
    ///     termine le niveau si c'est le joueur. Le score du joueur est aussi ajuster en fonction du tank enlevé.
    /// </summary>
    /// <param name='tank'>
    ///     Le tank a enlevé.
    /// </param>
	public void removeTank(GameObject tank)
	{
        //Remove player tank
		if(tank == player && levelStatus == LevelStatus.Playing)
		{
			Debug.Log("Player killed");
            _status = LevelStatus.Failed;

            //Désactiver les tanks
            foreach(GameObject enemy in enemies)
            {
                enemy.GetComponent<EnemyMain>().pause();
            }

            //Détruire tous les projectiles
            GameObject[] gameObjects = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.GetComponent<Projectile_Mouvement>() != null)
                {
                    Destroy(gameObject);
                }
            }
        }

        //Remove enemy tank
		else if(tank.GetComponent<EnemyMain>() != null && levelStatus == LevelStatus.Playing)
		{
            Debug.Log("Enemy killed");
            enemies.Remove(tank);
            gameManager.augmenterPointage(tank.GetComponent<EnemyMain>().point);

            //Vérifier s'il reste des ennemies
            if(enemies.Count == 0)
            {
                _status = LevelStatus.Completed;
                player.GetComponent<PlayerMain>().pause();

                //Détruire tous les projectiles
                GameObject[] gameObjects = GameObject.FindObjectsOfType<GameObject>();
                foreach(GameObject gameObject in gameObjects)
                {
                    if(gameObject.GetComponent<Projectile_Mouvement>() != null)
                    {
                        Destroy(gameObject);
                    }
                }
            }
		}
	}

    /// <summary>
    ///     Mettre le niveau en pause. Si le niveau est déjà en pause, arrêter la pause.
    /// </summary>
    public void pauseLevel()
    {
        Debug.Log("PAUSE LEVEL!");

        //Unpaused level, activer toutes les entités
        if(levelStatus == LevelStatus.Paused)
        {
            //Joueur
            player.GetComponent<PlayerMain>().play();

            //Ennemies
            foreach(GameObject enemy in enemies)
            {
                enemy.GetComponent<EnemyMain>().play();
            }

            //Autres objects de la scène
            GameObject[] gameObjects = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject gameObject in gameObjects)
            {
                //Les projectiles
                if (gameObject.GetComponent<Projectile_Mouvement>() != null)
                {
                    gameObject.GetComponent<Projectile_Mouvement>().enabled = true;
                    gameObject.GetComponentInChildren<ParticleSystem>().Play();
                }

                //Les bouelets de cannon
                else if (gameObject.GetComponent<CannonBallMouvement>() != null)
                {
                    gameObject.GetComponent<CannonBallMouvement>().enabled = true;
                }

                //Les effets
                else if(gameObject.GetComponent<ParticleSystem>() != null)
                {
                    gameObject.GetComponent<ParticleSystem>().Play();
                }
            }

            _status = LevelStatus.Playing;
        }

        //Paused level, désactiver toutes les entités
        else if(levelStatus!=LevelStatus.Completed && levelStatus!=LevelStatus.Failed)
        {
            //Joueur
            player.GetComponent<PlayerMain>().pause();
            player.GetComponent<CreateProjectile>().enabled = false;

            //Ennemies
            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<EnemyMain>().pause();
            }

            //Autres objects
            GameObject[] gameObjects = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject gameObject in gameObjects)
            {
                //Les projectiles
                if (gameObject.GetComponent<Projectile_Mouvement>() != null)
                {
                    gameObject.GetComponent<Projectile_Mouvement>().enabled = false;
                    gameObject.GetComponentInChildren<ParticleSystem>().Pause();
                }

                //Les boulets de canon
                else if (gameObject.GetComponent<CannonBallMouvement>() != null)
                {
                    gameObject.GetComponent<CannonBallMouvement>().enabled = false;
                }

                //Les effets
                else if (gameObject.GetComponent<ParticleSystem>() != null)
                {
                    gameObject.GetComponent<ParticleSystem>().Pause();
                }
            }

            _status = LevelStatus.Paused;

            //Réinitialise le timeDelaiFrameUnpause
            timeDelaiFrameUnpause = 0;
        }
        
    }

    /// <summary>
    ///     Exécuter les actions nécessaires au début du niveau. Notamment, activer tous les tanks.
    /// </summary>
	private void startLevel()
	{
		Debug.Log("START LEVEL!");
        _status = LevelStatus.Playing;

		//Activer le joueur
		player.GetComponent<PlayerMain>().play();

		//Assigner le joueur comme cible des ennemis
		foreach(GameObject enemy in enemies)
		{
			enemy.GetComponent<EnemyMain>().play();
		}
	}

    /// <summary>
    ///     Terminer le niveau. Si le joueur est encore en vie, passer au niveau suivant. Si le joueur est mort, recommencer
    ///     le niveau ou terminer la partie.
    /// </summary>
    private void endLevel()
    {
        if(levelStatus == LevelStatus.Completed)
        {
            gameManager.nextLevel();
        }
        else if(levelStatus == LevelStatus.Failed)
        {
            gameManager.endGame();
        }
    }

#region GETTER/SETTER
    public LevelStatus levelStatus
    {
        get
        {
            return _status;
        }
    }
#endregion

}

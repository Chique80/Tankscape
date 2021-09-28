using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour 
{

	UnityEvent shootButtonPressed;
    UnityEvent rightClickPressed;
    UnityEvent pauseButtonPressed;

    [Header("Player Event")]
    UnityEvent playerFowardPressed;
	UnityEvent playerBackwardPressed;
	UnityEvent playerLeftPressed;
	UnityEvent playerRightPressed;

    UnityEvent playerFowardUnpressed;
    UnityEvent playerBackwardUnpressed;
    UnityEvent playerLeftUnpressed;
    UnityEvent playerRightUnpressed;   

	[Header ("Input")]
	public KeyCode playerFoward;
	public KeyCode playerBackward;
	public KeyCode playerRight;
	public KeyCode playerLeft;
	public KeyCode shootButton;
    public KeyCode pauseButton;


	// Use this for initialization
	void Start () 
	{
		playerFowardPressed = new UnityEvent();
		playerBackwardPressed = new UnityEvent();
		playerRightPressed = new UnityEvent();
		playerLeftPressed = new UnityEvent();

        playerFowardUnpressed = new UnityEvent();
        playerBackwardUnpressed = new UnityEvent();
        playerRightUnpressed = new UnityEvent();
        playerLeftUnpressed = new UnityEvent();

        rightClickPressed = new UnityEvent();
        shootButtonPressed = new UnityEvent();
        pauseButtonPressed = new UnityEvent();
    }
	
    // Called once per frame
	void FixedUpdate()
	{
		//Touche pour avancer
		if(Input.GetKey(playerFoward))      //Enfoncée
		{
			playerFowardPressed.Invoke();
		}
        else                                //Relâchée
        {
            playerFowardUnpressed.Invoke();
        }

        //Touche pour reculer
        if (Input.GetKey(playerBackward))    //Enfoncée
		{
			playerBackwardPressed.Invoke();
        }
        else                                //Relâchée
        {
            playerBackwardUnpressed.Invoke();
        }

        //Touche pour aller à droite
        if (Input.GetKey(playerRight))       //Enfoncée
		{
			playerRightPressed.Invoke();
        }
        else                                //Relâchée
        {
            playerRightUnpressed.Invoke();
        }

        //Touche pour aller à gauche
        if (Input.GetKey(playerLeft))        //Enfoncée
		{
			playerLeftPressed.Invoke();
        }
        else                                //Relâchée
        {
            playerLeftUnpressed.Invoke();
        }

        //Le touche pour tirer
        if (Input.GetKeyDown(shootButton))
        {
            
            shootButtonPressed.Invoke();
        }

        //Right click est enfoncé
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            rightClickPressed.Invoke();
        }

        //Toucher pour pauser
        if (Input.GetKeyDown(pauseButton))
        {
            pauseButtonPressed.Invoke();
        }

        //Touche pour avancer d'un niveau (TOREMOVE)
        if (Input.GetKey(KeyCode.Escape) && Input.GetKeyDown(KeyCode.Space))
        {
            GameManagerScript gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManagerScript>();
            if (gameManager != null)
            {
                gameManager.nextLevel();
            }
        }
    }
 
    /// <summary>
    ///     Initialiser les inputs du niveau.
    /// </summary>
	public void setLevelInput()
	{
        //Enlever les inputs précédents
        removeListeners();

        //Assigner les inputs du UI
        GameObject sceneManager = GameObject.FindGameObjectWithTag("SceneManager");
        if(sceneManager != null)
        {
            //Trouver les scripts
            UIManager ui = sceneManager.GetComponent<LevelUIManager>();
            LevelManager level = sceneManager.GetComponent<LevelManager>();

            //Assigner les listeners
            if (level != null)
            {
                pauseButtonPressed.AddListener(level.pauseLevel);
            }
            if(ui != null)
            {
                rightClickPressed.AddListener(ui.checkUIButton);
            }
        }

        //Assigner les inputs du joueur
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            //Trouver les scripts
            PlayerMain playerMain = player.GetComponent<PlayerMain>();
            MouvementPlayer playerMov = player.GetComponent<MouvementPlayer>();
  
            //Assigner les listeners
            playerFowardPressed.AddListener(playerMov.moveUp);
            playerBackwardPressed.AddListener(playerMov.moveDown);
            playerRightPressed.AddListener(playerMov.moveRight);
            playerLeftPressed.AddListener(playerMov.moveLeft);

            playerFowardUnpressed.AddListener(playerMov.stopUp);
            playerBackwardUnpressed.AddListener(playerMov.stopDown);
            playerRightUnpressed.AddListener(playerMov.stopRight);
            playerLeftUnpressed.AddListener(playerMov.stopLeft);

            shootButtonPressed.AddListener(playerMain.playerShoot);
        }
    }

    /// <summary>
    ///     Initialiser les inputs de l'écran titre.
    /// </summary>
    public void setTitleScreenInput()
    {
        removeListeners();

        //Assignation de component
        UIManager ui = GameObject.FindGameObjectWithTag("UIManager").GetComponent<TitleScreenUIManager>();
        if (ui != null)
        {
            //Ajouter les novueaux listeners
            rightClickPressed.AddListener(ui.checkUIButton);
        }
    }

    /// <summary>
    ///     Initialiser les inputs de l'écran de fin de partie.
    /// </summary>
    public void setEndGameScreenInput()
    {
        removeListeners();

        //Assignation de component
        UIManager ui = GameObject.FindGameObjectWithTag("UIManager").GetComponent<EndGameScreenUIManager>();
        if (ui != null)
        {
            //Ajouter les novueaux listeners
            rightClickPressed.AddListener(ui.checkUIButton);
        }
    }

    /// <summary>
    ///     Initialiser les inputs de l'écran de score.
    /// </summary>
    public void setScoreScreenInput()
    {
        removeListeners();

        //Assignation de component
        UIManager ui = GameObject.FindGameObjectWithTag("UIManager").GetComponent<ScoreScreenUIManager>();
        if (ui != null)
        {
            //Ajouter les novueaux listeners
            rightClickPressed.AddListener(ui.checkUIButton);
        }
    }

    /// <summary>
    ///     Enlever tous les listeners dans event des inputs. Utilisé avant l'initialisation des input d'une scène.
    /// </summary>
	private void removeListeners()
	{
		shootButtonPressed.RemoveAllListeners();
        rightClickPressed.RemoveAllListeners();
        pauseButtonPressed.RemoveAllListeners();

        playerFowardPressed.RemoveAllListeners();
		playerBackwardPressed.RemoveAllListeners();
		playerLeftPressed.RemoveAllListeners();
		playerRightPressed.RemoveAllListeners();

        playerFowardUnpressed.RemoveAllListeners();
        playerBackwardUnpressed.RemoveAllListeners();
        playerLeftUnpressed.RemoveAllListeners();
        playerRightUnpressed.RemoveAllListeners();
    }
}

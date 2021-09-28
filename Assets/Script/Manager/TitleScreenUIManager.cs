using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

/*
 */
public class TitleScreenUIManager : UIManager
{
    private GameManagerScript gameManager;

    [Header("UI Element")]
    public ButtonScript btnPlay;
    public ButtonScript btnQuit;
    public ButtonScript btnScore;

	// Use this for initialization
	void Start ()
    {
        //Trouver le GameManager.cs
        GameObject manager = GameObject.FindGameObjectWithTag("GameManager");
        if (manager == null)
        {
            Debug.LogError(this + " requires a GameManager.cs script|");
        }
        else
        {
            gameManager = manager.GetComponent<GameManagerScript>();
            if (gameManager == null)
            {
                Debug.LogError(this + " requires a GameManager.cs script|");
            }
        }

        //Trouver le Raycaster et l'EventSystem
        initialiserComponent();

        //Initialiser le reste
        initialiseButtonEvent();
    }
 
    /// <summary>
    ///     Initialiser les buttons du iu.
    /// </summary>
    private void initialiseButtonEvent()
    {
        if (gameManager != null)
        {
            //PlayButton
            if (btnPlay != null)
            {
                btnPlay.initialiseClickedEvent();
                btnPlay.onClickedEvent.AddListener(gameManager.startGame);
            }

            //QuitButton
            if (btnQuit != null)
            {
                btnQuit.initialiseClickedEvent();
                btnQuit.onClickedEvent.AddListener(gameManager.quitGame);
            }

            //Button pour afficher l'écran de score
            if(btnScore != null)
            {
                btnScore.initialiseClickedEvent();
                btnScore.onClickedEvent.AddListener(gameManager.loadScoreScreen);
            }
        }
    }
}

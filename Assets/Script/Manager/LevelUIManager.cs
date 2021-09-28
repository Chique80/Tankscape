using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIManager : UIManager {

    [Header("UI Element")]
    public Text textePointage;
    public Text texteVie;
    public Text texteStart;
    public Text texteLoading;
    public Text texteInstruction;
    public Text texteOutcomeMission;
    public Text textePause;
    public ButtonScript btnResume;
    public ButtonScript btnQuit;
    public Image pauseScreen;

    [Header("Texte")]
    public string texteComplete = "Mission\nCompleted";
    public string texteFailed = "Mission\nFailed";

    [Header("Timer")]
    private float decompte;
    private float delaiAffichageInstruction;                         

    [Header("Component")]
    private GameManagerScript gameManager;
    private LevelManager levelManager;


    // Use this for initialization
    void Start()
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

        //Trouver le LevelManager.cs
        levelManager = GetComponent<LevelManager>();
        if(levelManager == null)
        {
            Debug.LogError(this + " requires a LevelManager.cs script!");
        }

        //Trouver le raycaster et l'eventSystem
        initialiserComponent();

        //Initialiser les buttons
        initialiserButtonScript();

        //Initialiser les variables
        if(gameManager != null)
        {
            delaiAffichageInstruction = 0.5f;
            textePointage.text = gameManager.pointageTotal.ToString();
        }

        //Cacher les éléments du ui
        hideUIElement();
    }

    // Update is called once per frame
    void Update()
    {
        //Update le timer d'affichage des instructions
        if (levelManager.levelStatus == LevelManager.LevelStatus.Playing)
        {
            delaiAffichageInstruction -= Time.deltaTime;
        }

        //Updater le pointage
        if(gameManager != null)
        {
            textePointage.text = gameManager.pointageTotal.ToString();
            texteVie.text = gameManager.playerHp.ToString();
        }

        //Update le ui
        updateUI();
    }

    /// <summary>
    ///     Updater le ui pour afficher les bons éléments.
    /// </summary>
    private void updateUI()
    {
        hideUIElement();

        //Afficher les éléments du UI en fonction de l'état du niveau
        switch (levelManager.levelStatus)
        {
            case LevelManager.LevelStatus.Loading:
                texteInstruction.gameObject.SetActive(true);
                texteLoading.gameObject.SetActive(true);

                decompte = GeneralFunction.roundFloat(levelManager.delayAtStart, 0);
                texteLoading.text = decompte.ToString();

                //Affiche le texte Start
                if (levelManager.delayAtStart <= 0.2)
                {
                    texteLoading.gameObject.SetActive(false);
                    texteStart.gameObject.SetActive(true);
                }
                break;

            case LevelManager.LevelStatus.Completed:
                texteOutcomeMission.gameObject.SetActive(true);
                texteOutcomeMission.text = texteComplete;
                break;

            case LevelManager.LevelStatus.Failed:
                texteOutcomeMission.gameObject.SetActive(true);
                texteOutcomeMission.text = texteFailed;
                break;

            case LevelManager.LevelStatus.Paused:

                textePause.gameObject.SetActive(true);
                btnQuit.gameObject.SetActive(true);
                btnResume.gameObject.SetActive(true);
                pauseScreen.gameObject.SetActive(true);
                break;

            case LevelManager.LevelStatus.Playing:
                if (delaiAffichageInstruction > 0)
                {
                    texteStart.gameObject.SetActive(true);
                }
                break;

            default:
                Debug.LogError("STATE NOT DEFINIED IN UI");
                break;
        }
    }
 
    /// <summary>
    ///     Cacher les éléments du ui. La méthode est appelé à chaque frame pour être sur d'afficher
    ///     les bons éléments sur le ui.
    /// </summary>
    private void hideUIElement()
    {
        texteStart.gameObject.SetActive(false);
        texteLoading.gameObject.SetActive(false);
        texteInstruction.gameObject.SetActive(false);
        texteOutcomeMission.gameObject.SetActive(false);
        textePause.gameObject.SetActive(false);
        pauseScreen.gameObject.SetActive(false);

        btnQuit.gameObject.SetActive(false);
        btnResume.gameObject.SetActive(false);
    }

    /// <summary>
    ///     Initialiser les buttons du ui.
    /// </summary>
    private void initialiserButtonScript()
    {
        if (gameManager != null && levelManager != null)
        {
            //Button pour quitter
            if (btnQuit != null)
            {
                //Ajouter les events listeners
                btnQuit.initialiseClickedEvent();
                btnQuit.onClickedEvent.AddListener(gameManager.returnToStart);

                //Cacher le button
                btnQuit.gameObject.SetActive(false);
            }

            //Button pour continuer
            if(btnResume != null)
            {
                //Ajouter les events listeners
                btnResume.initialiseClickedEvent();
                btnResume.onClickedEvent.AddListener(levelManager.pauseLevel);

                //Cacher le button
                btnResume.gameObject.SetActive(false);
            }
        }
    }

}
    



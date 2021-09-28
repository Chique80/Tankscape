using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 */
public class GameManagerScript : MonoBehaviour 
{
    private const string SCORE_FILE_PATH = "ScoreFile.txt";
 
    private InputManager inputManager;
    private int _pointageTotal;
    private List<PlayerEntry> entries;

    [Header("Other Scene")]
    public string nameTitleScreen;
    public string nameEndGameScreen;
    public string nameScoreScreen;

	[Header ("Level")]
    public List<string> levelsName;
    private int currentLevel = -1;

    [Header("Player")]
    public string playerName;
    public int playerMaxHp;
    public int playerHp;
  

    // Use this for initialization
    void Start () 
	{
        //Assigner les différents component
        inputManager = GetComponent<InputManager>();
        if(inputManager == null)
        {
            Debug.LogError("An InputManager.cs script is missing!");
        }

        //Créer les entrées de joueurs
        entries = new List<PlayerEntry>();
        loadPlayerEntries();

        SceneManager.sceneLoaded += OnSceneLoaded;                                            //Méthode a call lorsqu'une scene est load

        //Load l'écran titre
        unloadAllScene();
        SceneManager.LoadSceneAsync(nameTitleScreen, LoadSceneMode.Additive);                 
	}

    void Update ()
    {
    }
    
#region SCORE_MANAGEMENT
    /// <summary>
    ///     Augmenter le pointage du joueur.
    /// </summary>
    /// <param name='point'>
    ///     Les points à ajouter au pointage.
    /// </param>
    public void augmenterPointage(int point)
    {
        _pointageTotal += point;
    }
    
    /// <summary>
    ///     Enregistrer le pointage du joueur. Le pointage du joueur est enregistrer dans un PlayerEntry.
    /// </summary>
    public void savePlayerScore()
    {
        PlayerEntry entry = new PlayerEntry(playerName, pointageTotal);
        entries.Add(entry);
    }
    
    /// <summary>
    ///     Enregistrer toutes les PlayerEntry dans un fichier texte.
    /// </summary>
    public void savePlayerEntries()
    {
        try
        {
            //Regarder si un fichier existe déjà
            if(!File.Exists(SCORE_FILE_PATH))
            {
                FileStream tempFile = File.Create(SCORE_FILE_PATH);
                tempFile.Close();
            }

            //Enregistrer les entrées des joueurs dans le fichier
            StreamWriter scoreFile = new StreamWriter(SCORE_FILE_PATH, false);
            foreach(PlayerEntry entry in entries)
            {
                scoreFile.WriteLine(entry.ToString());
            }

            scoreFile.Close();
        }
        catch(System.Exception e)
        {
            Debug.Log(e);
        }

    }

    /// <summary>
    ///     Loader les PlayerEntry à partie d'un fichier texte. S'il n'y a pas de fichier de texte, rien n'est loader.
    /// </summary>
    public void loadPlayerEntries()
    {
        if(File.Exists(SCORE_FILE_PATH))
        {
            try
            {
                StreamReader scoreFile = new StreamReader(SCORE_FILE_PATH);
                
                //Lire le fichier
                string line = scoreFile.ReadLine();
                while(line != null)
                {
                    //Créer une entrée
                    string[] texts = line.Split(PlayerEntry.SEPARATEUR.ToCharArray());
                    try
                    {
                        PlayerEntry entry = new PlayerEntry(texts[0], int.Parse(texts[1].Trim()));
                        entries.Add(entry);
                    }
                    catch(System.Exception e)
                    {
                        Debug.LogError(e);
                    }

                    line = scoreFile.ReadLine();
                }

                scoreFile.Close();
            }
            catch(System.Exception e)
            {
                Debug.LogError(e);
            }
        }
    }

#endregion

#region LEVEL_MANAGEMENT
    /// <summary>
    ///     Commencer une partie en loadant le premier niveau.
    /// </summary>
    public void startGame()
    {
        playerHp =  playerMaxHp;

        //Load le premier niveau 
        currentLevel = 0;
        loadLevel();
    }

    /// <summary>
    ///     Loader le niveau suivante.
    /// </summary>
    public void nextLevel()
    {
        //Load le niveau suivant
        currentLevel++;
        loadLevel();
    }

    /// <summary>
    ///     Terminer la partie en loadant l'écran de fin de partie.
    /// </summary>
    public void endGame()
    {
        playerHp--;                                               

        //Afficher l'écran de game over, si le joueur n'a plus de vie
        if(playerHp <= 0)
        {
            currentLevel = levelsName.Count + 1;
        }

        loadLevel();                                                    
    }

    /// <summary>
    ///     Loader l'écran titre
    /// </summary>
    public void returnToStart()
    {
        //Afficher l'écran titre
        currentLevel = -1;
        loadLevel();

        //Réinitialise le pointage
        _pointageTotal = 0;
    }

    /// <summary>
    ///     Fermer le jeu
    /// </summary>
    public void quitGame()
    {
        unloadAllScene();

        savePlayerEntries();

        if(Application.isEditor)
        {
            //UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }
#endregion

#region SCENE_LOADING
    /// <summary>
    ///     Loader l'écran de score
    /// </summary>
    public void loadScoreScreen()
    {
        unloadAllScene();
        SceneManager.LoadSceneAsync(nameScoreScreen, LoadSceneMode.Additive);
    }

    /// <summary>
    ///     Loader une scène. La scène a loader est déterminée par une variable.
    /// </summary>
    private void loadLevel()
    {
        //Load/Unload scene
        unloadAllScene();

        //Load l'écran titre
        if(currentLevel < 0)
        {
            SceneManager.LoadSceneAsync(nameTitleScreen, LoadSceneMode.Additive);                 
        }

        //Load l'écran de game over
        else if(currentLevel >= levelsName.Count)
        {
            SceneManager.LoadSceneAsync(nameEndGameScreen, LoadSceneMode.Additive);
        }

        //Load un niveau
        else
        {
            string levelName = levelsName[currentLevel];
            SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);           
        }
    }

    /// <summary>
    ///     Méthode appelée lorsqu'une scène est loader. Permet d'ajuster les inputs à la scène qui vient d'être loadée.
    /// </summary>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Load Scene: " + scene.name);
        SceneManager.SetActiveScene(scene);

        if (scene.name == nameTitleScreen)
        {
            inputManager.setTitleScreenInput();
        }
        else if(scene.name == nameEndGameScreen)
        {
            inputManager.setEndGameScreenInput();
        }
        else if(scene.name == nameScoreScreen)
        {
            inputManager.setScoreScreenInput();
        }
        else if(levelsName.Contains(scene.name))
        {
            inputManager.setLevelInput();
        }
    }

    /// <summary>
    ///     Fermer toutes les scènes exceptées la scène principale.
    /// </summary>
    private void unloadAllScene()
    {
        for(int i = 0; i<SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            if(scene.name != "DoNotDelete")
            {
                Debug.Log("Unload scene: " + scene.name);
                SceneManager.UnloadSceneAsync(scene);
            }
        }
    }
#endregion
    
#region GETTER/SETTER
    public int pointageTotal
    {
        get
        {
            return _pointageTotal;
        }
    }
    public PlayerEntry[] PlayerEntries
    {
        get
        {
            return entries.ToArray();
        }
    }
#endregion

}

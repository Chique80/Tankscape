using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 */
public class EndGameScreenUIManager : UIManager 
{
	[Header("UIElement")]
	public Text title;
	public InputField nameField;
	public Text scoreText;
	public ButtonScript btnSave;
	public ButtonScript btnQuit;

	[Header("Texte")]
	public string GameOverTexte;
	public string VictoryTexte;

	[Header("Component")]
	private GameManagerScript gameManager;

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

		//Trouver le GraphicRaycaster et le EventSystem
		initialiserComponent();
 
		//Initialiser les éléments du UI
		if(gameManager != null)
		{
			scoreText.text = gameManager.pointageTotal.ToString();
			if(gameManager.playerHp > 0)
			{
				title.text = VictoryTexte;
			}
			else
			{
				title.text = GameOverTexte;
			}
			
			initialiserButtonScript();
		}
	}

	/// <summary>
	///		Initialiser les buttons du ui.
	/// </summary>
	private void initialiserButtonScript()
	{
		//Button pour sauvegarder
		if(btnSave != null)
		{
			//Ajouter les events listeners
			btnSave.initialiseClickedEvent();
			btnSave.onClickedEvent.AddListener(changePlayerName);
			btnSave.onClickedEvent.AddListener(gameManager.savePlayerScore);
			btnSave.onClickedEvent.AddListener(gameManager.returnToStart);
		}

		//Button pour quitter
		if(btnQuit != null)
		{
			//Ajouter les events listeners
			btnQuit.initialiseClickedEvent();
			btnQuit.onClickedEvent.AddListener(gameManager.returnToStart);
		}
	}

	/// <summary>
	///		Changer le nom du joueur, en fonction de ce qu'il a entré.
	/// </summary>
	private void changePlayerName()
	{
		if(gameManager != null)
		{
			gameManager.playerName = nameField.text;
		}
	} 
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 */
public class ScoreScreenUIManager : UIManager 
{
	[Header("UI Element")]
	public MaskableGraphic entryZone;
	public ButtonScript btnQuit;
	public Scrollbar scoreZoneBar;

	[Header("Score Entry")]
	public GameObject scoreEntryPefab;
	public Vector3 firstEntryPos;
	public float ecartVertical = -40;
	public int maxEntry = 7;
	private int firstEntryIndex;

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

		//Trouver le Raycaster et l'EventSystem
		initialiserComponent();

		//Initialiser le reste
		initialiserButtonScript();
		firstEntryIndex = 0;

		//Afficher les scores
		showScoreEntry();
	}

	/// <summary>
	///		Afficher les entrées de score dans la zone de score.
	/// </summary>
	private void showScoreEntry()
	{
		if(gameManager != null)
		{
			if(entryZone != null)
			{
				PlayerEntry[] entries = gameManager.PlayerEntries;
				quicksortPlayerEntryByScore(entries, 0, entries.Length-1);

				for(int i = 0; i < maxEntry; i++)
				{
					
					//Créer l'élément
					GameObject scoreEntry = Instantiate(scoreEntryPefab, entryZone.rectTransform);
					scoreEntry.transform.localPosition = new Vector3(firstEntryPos.x, firstEntryPos.y + (i * ecartVertical), firstEntryPos.z);

					//Mettre les informations de l'entrée dans l'élément
					Text[] childs = scoreEntry.GetComponentsInChildren<Text>();
					if(i + firstEntryIndex < entries.Length)
					{
						childs[0].text = entries[i+firstEntryIndex].Name;
						childs[1].text = entries[i+firstEntryIndex].Score.ToString();
					}
					else
					{
						childs[0].text = " ";
						childs[1].text = " ";
					}
					
				}
			}
			else
			{
				Debug.Log(this + " is missing a area to create the score entry!");
			}
		}
	}

	/// <summary>
	/// 	Métode appelée lorsque la scrollBar est changée. Le méthode appelle à son tour showScoreEntry() pour
	///			modifier la zone de score.
	/// </summary>
	public void changeScrollBar()
	{
		if(scoreZoneBar != null)
		{
			//Calculer l'index de la première entrée à afficher
			int step = (int) Mathf.Round(scoreZoneBar.value / scoreZoneBar.size);
			firstEntryIndex = maxEntry * step;

			//Update l'écran de score
			showScoreEntry();
		}
	}

	/// <summary>
	/// 	Initialiser les buttons du ui.
	/// </summary>
	private void initialiserButtonScript()
	{
		if(gameManager != null)
		{
			//Button quitter
			if(btnQuit != null)
			{
				btnQuit.initialiseClickedEvent();
				btnQuit.onClickedEvent.AddListener(gameManager.returnToStart);
			}
		}
	}
 
#region UTILS
	private void quicksortPlayerEntryByScore(PlayerEntry[] entries, int start, int end)
	{
		int i;
		if(start < end)
		{
			i = partitionPlayerEntryByScore(entries, start, end);

			quicksortPlayerEntryByScore(entries, start, i - 1);
			quicksortPlayerEntryByScore(entries, i + 1, end);
		}
	}
	private int partitionPlayerEntryByScore(PlayerEntry[] entries, int start, int end)
	{
		PlayerEntry temp;
		PlayerEntry pivot = entries[end];
		int i = start - 1;

		for(int j = start; j <= end-1; j++)
		{
			if(entries[j].Score >= pivot.Score)
			{
				i++;
				temp = entries[i];
				entries[i] = entries[j]; 
				entries[j] = temp;
			}
		}

		temp = entries[i+1];
		entries[i+1] = entries[end];
		entries[end] = temp;

		return i + 1;
	}
#endregion
	
}

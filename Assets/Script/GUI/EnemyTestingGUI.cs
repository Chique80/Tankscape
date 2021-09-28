using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Ce script sert à afficher un ui personnalisé pour les scènes de test de l'ai.
 */ 

public class EnemyTestingGUI : MonoBehaviour
{
	private int enemyDirectionsWindowID = 1;
	private int menuWindowID = 2;
	private int gizmosWindowID = 3;
    private int aiControlWindowID = 4;

	[Header("Component")]
	public GameObject enemy;

	[Header("Enemy Direction Window")]
	public float enDirX;
	public float enDirY;

	[Header("Menu Window")]
	public float mnuX;
	public float mnuY;
	public bool tglEnemyDirection;
	public bool tglEnemyView;
	public bool tglGizmosWindow;

    [Header("AI Control Window")]
    public float aiX;
    public float aiY;

	[Header("Gizmos Window")]
	public float gizX;
	public float gizY;
	

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

    void OnGUI()
    {
		if(enemy != null)
		{
			//Draw menu
			GUILayout.Window(menuWindowID, new Rect(Screen.width * mnuX, Screen.height * mnuY, 0, 0), menuWindow, "Menu");

			//Draw AI Control window
			if(enemy.GetComponent<EnemyAIValue>() != null)
			{
				GUILayout.Window(aiControlWindowID, new Rect(Screen.width * aiX, Screen.height * aiY, 0, 0), aiControlWindow, "AI Control Window");
			}

			//Draw gizmos window
			if (tglGizmosWindow)
			{
				GUILayout.Window(gizmosWindowID, new Rect(Screen.width * gizX, Screen.height * gizY, 0, 0), gizmosWindow, "Gizmos Menu");
			}

			//Draw enemy directions value
			if (enemy.GetComponent <EnemyAIValue>().hasMouvementScript)
			{
				if (tglEnemyDirection)
				{
					GUILayout.Window(enemyDirectionsWindowID, new Rect(Screen.width * enDirX, Screen.height * enDirY, 0, 0), ennemyDirectionsValuesWindow, "Directions Values");
				}
			}
		}
	}

	void menuWindow(int windowID)
	{
		//Toggle button to show enemy direction window
		tglEnemyDirection = GUILayout.Toggle(tglEnemyDirection, "Directions Window");

		//Toggle button to show gizmos menu
		tglGizmosWindow = GUILayout.Toggle(tglGizmosWindow, "Gizmos Menu");

		//Toggle button to show enemy view
		if(GUILayout.Toggle(tglEnemyView, "Enemy View"))
		{
			//Show enemy view
			if(!tglEnemyView)
			{
				tglEnemyView = true;
				enemyView(true);
			}
		}
		else
		{
			//Hide enemy view
			if(tglEnemyView)
			{
				tglEnemyView = false;
				enemyView(false);
			}
		}
	}

	void gizmosWindow(int windowID)
	{
		//Toggle button to show EnemyAI.cs gizmos
        enemy.GetComponent<EnemyAIValue>().showAIGizmos = GUILayout.Toggle(enemy.GetComponent<EnemyAIValue>().showAIGizmos, "EnemyAI");
		
		//Toggle button to show EnemyMouvement.cs gizmos
        enemy.GetComponent<EnemyAIValue>().showMouvementGizmos = GUILayout.Toggle(enemy.GetComponent<EnemyAIValue>().showMouvementGizmos, "EnemyMouvement");

		//Toggle button to show WallDetection.cs gizmos
        enemy.GetComponent<EnemyAIValue>().showWallDetectionGizmos = GUILayout.Toggle(enemy.GetComponent<EnemyAIValue>().showWallDetectionGizmos, "WallDetection");

		//Toggle button to show Pathfinder.cs gizmos
        enemy.GetComponent<EnemyAIValue>().showPathfindingGizmos = GUILayout.Toggle(enemy.GetComponent<EnemyAIValue>().showPathfindingGizmos, "Pathfinder");

        //Toggle button to show ProjectileDetection.cs gizmos
        enemy.GetComponent<EnemyAIValue>().showProjectileDetectionGizmos = GUILayout.Toggle(enemy.GetComponent<EnemyAIValue>().showProjectileDetectionGizmos, "ProjetileDetection");

        //Toggle button to show TankDetection.cs gizmos
        enemy.GetComponent<EnemyAIValue>().showTankDetectionGizmos = GUILayout.Toggle(enemy.GetComponent<EnemyAIValue>().showTankDetectionGizmos, "TankDetection");

    }

	void ennemyDirectionsValuesWindow(int windowID)
	{
		//Get the value
		Vector3[] enemyDirections = enemy.GetComponent<EnemyAIValue>().enemyMouvement.directions;
		float[] enemyDirectionsValues = enemy.GetComponent<EnemyAIValue>().enemyMouvement.directionsValues;

		//Show directions values
		GUILayout.BeginHorizontal();

		//Draw half
		GUILayout.BeginVertical();
		for(int i = 0; i<enemyDirections.Length/2; i++)
		{
			//Calculer l'angle polaire (en fonction de la direction principale) de la direction
			float angle = Vector3.Angle(enemyDirections[i], enemyDirections[0]);
			angle = Mathf.Round(angle);

			//Enregistrer la valeur arrondie de la direction
			float value = GeneralFunction.roundFloat(enemyDirectionsValues[i], 2);

			GUILayout.BeginHorizontal();
			GUILayout.Label(normalizedString(angle.ToString()+"\u00B0", 5));
			GUILayout.Space(4);
			GUILayout.Label(value.ToString());
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();

		//Draw a separation
		GUILayout.BeginVertical();
		for(int i = 0; i<enemyDirections.Length/2; i++)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(3);
			GUILayout.Label("|");
			GUILayout.Space(7);
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();

		//Draw the other half
		GUILayout.BeginVertical();
		for(int i = enemyDirections.Length/2; i<enemyDirections.Length; i++)
		{
			//Calculer l'angle polaire (en fonction de la direction principale) de la direction
			float angle = Vector3.Angle(enemyDirections[i], -enemyDirections[0]) + 180;
			angle = Mathf.Round(angle);

			//Enregistrer la valeur arrondie de la direction
			float value = GeneralFunction.roundFloat(enemyDirectionsValues[i], 2);

			GUILayout.BeginHorizontal();
			GUILayout.Label(normalizedString(angle.ToString()+"\u00B0", 5));
			GUILayout.Space(4);
			GUILayout.Label(value.ToString());
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();

		GUILayout.EndHorizontal();
	}

    void aiControlWindow(int windowID)
    {
        if(enemy.GetComponent<EnemyAIValue>().hasMouvementScript)
        {
			//Can move
            enemy.GetComponent<EnemyAIValue>().enemyMouvement.canMove = GUILayout.Toggle(enemy.GetComponent<EnemyAIValue>().enemyMouvement.canMove, "Can Move");

			//Immobile direction
			GUILayout.BeginHorizontal();
            enemy.GetComponent<EnemyAIValue>().enemyMouvement.showimmobileDirection = GUILayout.Toggle(enemy.GetComponent<EnemyAIValue>().enemyMouvement.showimmobileDirection, "Show Immobile");
 			GUILayout.Label(GeneralFunction.roundFloat(enemy.GetComponent<EnemyAIValue>().enemyMouvement.immobileValue, 2).ToString());
			GUILayout.EndHorizontal();

			//Show State
			GUILayout.Label("State: " + enemy.GetComponent<EnemyAIValue>().state.ToString());
        }
    }

	private void enemyView(bool show)
	{
		GameObject[] gameObjects = (GameObject[]) GameObject.FindObjectsOfType(typeof(GameObject));			//Find all the game object in the scene

		//Disable the renderer for each block
		foreach(GameObject gameObj in gameObjects)
		{
			//Désactiver le renderer des blocks
			if(gameObj.GetComponent<Block>() != null)
			{
				gameObj.GetComponent<MeshRenderer>().enabled = !show;
			}
		}

        //Disable le renderer du tank
        MeshRenderer mainRenderer = enemy.GetComponent<MeshRenderer>();
        if (mainRenderer != null)
        {
            mainRenderer.enabled = !show;
        }

        MeshRenderer[] renderers = enemy.GetComponentsInChildren<MeshRenderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = !show;
        }
    }

#region Util
	private string normalizedString(string text, int size)
	{
		string newText = text.Trim();

		if(text.Length < size)
		{
			for(int i = 0; i<size - text.Length; i++)
			{
				newText += " ";
			}
		}

		return newText;
	}
#endregion
}

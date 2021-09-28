using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Cette classe représente une entrée de score d'un joueur. Elle contient le score du joueur et son nom
 */
 //

public class PlayerEntry 
{
	public static string SEPARATEUR = ":";

	private string _name;
	private int _score;

	public PlayerEntry(string name, int score)
	{
		_name = name;
		_score = score;
	}

#region OVERRIDE
	public override string ToString()
	{
		return Name + SEPARATEUR + " " + Score.ToString();
	}
#endregion

#region GETTER/SETTER
	public string Name
	{ 
		get
		{
			return _name;
		}
	}
	public int Score
	{
		get
		{
			return _score;
		}
	}
#endregion
	
}

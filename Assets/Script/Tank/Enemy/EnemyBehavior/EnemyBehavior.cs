using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehavior : MonoBehaviour 
{

#region ABSTRACT
	/// <summary>
	///		Méthode appelé pour activer le script de comportement en cours de partie. La méthode effectue les actions nécessaires à l'activation du
	///			du script.
	/// </summary>
	public abstract void enable();
 
	/// <summary>
	///		Méthode appelé pour désactiver le script de comportement en cours de partie. La méthode effectue les actions nécessaires à la désactivation du
	///			du script.
	/// </summary>
	public abstract void disable();
#endregion
}

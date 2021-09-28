using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
 */
public abstract class Detection : MonoBehaviour {

    protected EnemyAIValue valueScript;
    protected float[] _directionsValues; 

    /// <summary>
    ///     Calculer la valeur de chaque direction.
    /// </summary>
    public abstract void calculateDirectionsValues();

    /// <summary>
    ///     Initialiser le tableaux contenant la valeur de chaque direction.false
    /// </summary>
    public void initialiseDirectionValue(int nbDirection)
    {
        _directionsValues = new float[nbDirection];
        for (int i = 0; i < nbDirection; i++)
        {
            directionsValues[i] = 0;
        }
    }
    
    /// <summary>
    ///     Réinitialiser la valeur de chaque direction à 0.
    /// </summary>
    public void resetDirectionValue()
    {
        for (int i = 0; i < directionsValues.Length; i++)
        {
            directionsValues[i] = 0;
        }
    }
 
 
#region GETTER/SETTER
    public float[] directionsValues
    {
        get
        {
            return _directionsValues;  
        }
    }
#endregion
}

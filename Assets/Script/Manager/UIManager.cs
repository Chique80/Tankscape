using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Cursor")]
    public Texture2D cursorImageEnabled;
    public Texture2D cursorImageDisable;

    [Header("Event Management")]
    protected GraphicRaycaster raycaster;
    protected EventSystem eventSystem;

    /// <summary>
    ///     Trouver le GraphicRaycaster et le EventSystem de la scène. Initialiser aussi l'image du curseur.
    /// </summary>
    protected void initialiserComponent()
    {
        //Trouver un raycaster
        raycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
        if(raycaster == null)
        {
            Debug.LogError(this + " requires a GraphicRaycaster!");
        }

        //Trouver l'event system
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        if(eventSystem == null)
        {
            Debug.LogError(this + " requires an EventSystem!");
        }

        //Changer l'image du cursor
        if(cursorImageEnabled != null)
        {
            Cursor.SetCursor(cursorImageEnabled, new Vector2(cursorImageEnabled.width/2,cursorImageEnabled.height/2), CursorMode.Auto);
        }
    }
 
    /// <summary>
    ///     Vérifier si le joueur clique sur un button du ui. Si le joueur clique sur un boutton, la méthode appelle l'action du boutton.
    /// </summary>
    public void checkUIButton()
    {
        RaycastResult[] results = checkMousePosition();

        int index = 0;
        bool hasClickButton = false;
        while (index < results.Length && !hasClickButton)
        {
            RaycastResult hit = results[index];

            if (hit.gameObject.GetComponent<ButtonScript>() != null)
            {
                hasClickButton = true;
                hit.gameObject.GetComponent<ButtonScript>().clicked();
            }

            index++;
        }
    }

    /// <summary>
    ///     Vérifier la position de la souris sur le ui.
    /// </summary>
    /// <returns>
    ///     La liste des éléments du ui qui se trouve sous la souris.
    /// </returns>
    public RaycastResult[] checkMousePosition()
    {
        List<RaycastResult> results = new List<RaycastResult>();

        if (raycaster!=null && eventSystem!=null)
        {
            PointerEventData pointerEvent = new PointerEventData(eventSystem);
            pointerEvent.position = Input.mousePosition;

            raycaster.Raycast(pointerEvent, results);
        }
        else
        {
            Debug.Log(this + " cannot check mouse position because it is missing a reference to an EventSystem and/or a GraphicRaycaster!");
        }
        

        return results.ToArray();
    }
    
}


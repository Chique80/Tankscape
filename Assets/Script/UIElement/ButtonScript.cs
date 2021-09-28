using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    private Image image;

    private bool isClicked;
    private bool isEnable;

    public UnityEvent onClickedEvent;

    //Timer
    private float clickTimer;

    [Header("Sprites")]
    public Sprite sprDefault;                       //Le sprite de base du button
    public Sprite sprDisable;                       //Le sprite lorsque le button est désactivé
    public Sprite sprActivated;                     //Le sprite lorsque le button est enfoncé

    [Header("Other")]
    public float clickDuration;                     //Le temps que le button reste enfoncé

    // Use this for initialization
    void Start ()
    {
        //Assigner les components
        image = this.GetComponent<Image>();

        image.sprite = sprDefault;
        isClicked = false;
        isEnable = true;
        clickTimer = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(isClicked && isEnable)
        {
            clickTimer += Time.deltaTime;
            if(clickTimer >= clickDuration)
            {
                image.sprite = sprDefault;
                isClicked = false;
                clickTimer = 0;
            }
        }
	}

    // Called when the gameobject is disabled
    void OnDisable()
    {
        image.sprite = sprDefault;
        isClicked = false;
        clickTimer = 0;
    }

    /// <summary>
    ///     Méthode appelé pour indique le bouton a été cliqué. La méthode appelé le onClickedEvent pour effectuer l'action
    ///     appropriée.
    /// </summary>
    public void clicked()
    {
        if(!isClicked && isEnable)
        {
            image.sprite = sprActivated;
            isClicked = true;

            onClickedEvent.Invoke();
        }
    }

    /// <summary>
    ///     Désactiver le bouton pour empêcher le joueur de cliquer dessus.
    /// </summary>
    public void disable()
    {
        image.sprite = sprDisable;
        isEnable = false;
    }
 
    /// <summary>
    ///     Activer le bouton pour permettre au joueur de cliquer dessus.
    /// </summary>
    public void enable()
    {
        image.sprite = sprDefault;
        isEnable = true;
        isClicked = false; 
    }

    /// <summary>
    ///     Initialiser le onClickedEvent
    /// </summary>
    public void initialiseClickedEvent()
    {
        onClickedEvent = new UnityEvent();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPage 
{
    public UIPage prevPage;    // previous page of this page    
    private GameObject holder; // the gameobject that holds the GUI elements of this page
    private string tag;        // tag of the page's gameobject

    /// <summary>
    /// Methods to register a new page
    /// </summary>
    /// <param name="holder">the gameobject that holds the GUI elements of the new page</param>    
    public UIPage(GameObject holder)
    {
        this.holder = holder;
        this.tag = holder.tag;
    }

    /// <summary>
    /// Methods to register a new page
    /// </summary>
    /// <param name="holder">the gameobject that holds the GUI elements of the new page</param>    
    /// <param name="prevPage">the previous page of the new page</param>
    public UIPage(GameObject holder, UIPage prevPage)
    {
        this.holder = holder;
        this.prevPage = prevPage;
        this.tag = holder.tag;
    }

    /// <summary>
    /// Display / Hide the window based on the given value
    /// </summary>
    /// <param name="value">True to display, false to hide</param>
    public void Display(bool value)
    {
        if (tag == "Buttons")
        {
            SetButtonsState(value);
        }
        else if (tag == "CharacterSelection")
        {
            MonoBehaviour.FindObjectOfType<CharacterSelection>().SetActive(value);
        }
        else
        {
            holder.SetActive(value);
        }
    }

    /// <summary>
    /// Enable / Disable buttons based on the given value
    /// </summary>
    /// <param name="value"></param>
    void SetButtonsState(bool value)
    {
        foreach (var btn in holder.GetComponentsInChildren<Button>())
        {
            btn.enabled = value;
            btn.image.sprite = value ?
                btn.spriteState.pressedSprite :
                btn.spriteState.disabledSprite;
        }
    }
}

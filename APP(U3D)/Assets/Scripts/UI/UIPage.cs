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
    /// <param name="flag">True to display, false to hide</param>
    public void Display(bool flag)
    {
        if (tag == "Buttons")
        {
            SetButtonsState(flag);
            SetGameState(flag);
        }
        else if (tag == "CharacterSelection")
        {
            MonoBehaviour.FindObjectOfType<CharacterSelection>().SetActive(flag);
        }
        else if (tag == "Instruction")
        {
            if (flag)
                MonoBehaviour.FindObjectOfType<Instruction>().Reset();

            holder.SetActive(flag);    
        }
        else
        {
            holder.SetActive(flag);
        }
    }

    /// <summary>
    /// Enable / Disable buttons based on the given value
    /// </summary>
    /// <param name="flag"></param>
    void SetButtonsState(bool flag)
    {
        foreach (var btn in holder.GetComponentsInChildren<Button>())
        {
            btn.enabled = flag;
            btn.image.sprite = flag ?
                btn.spriteState.pressedSprite :
                btn.spriteState.disabledSprite;
        }
    }
    
    /// <summary>
    /// Pause / Resume the game when some buttons are pressed
    /// </summary>
    /// <param name="flag"></param>
    void SetGameState(bool flag)
    {
        // first, find the ui manager from the scene
        var uiManager = MonoBehaviour.FindObjectOfType<UIManager>();

        // if uiManager is not found or it is not an InGame scene, return
        if (uiManager == null || uiManager.sceneType != SceneType.InGame)
            return;

        // pause / resume the game
        uiManager.SetInteractableVisibility(flag);
        Time.timeScale = flag ? 1f : 0f;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Option")]
    [Tooltip("The genre of this scene")]
    public SceneType sceneType;
    [Tooltip("Gameobjects that are not displayed by default")]
    public GameObject[] initialHide;

    [Header("UI Widgets - for common")]
    [Tooltip("The initial buttons in homepage")]
    public GameObject initButtons;
    [Tooltip("A sprite that is used to fade in or out when switch scenes")]
    public Image fadeSprite;
    [Tooltip("A board that shows player's name and states")]
    public PlayerBoard playerBoard;
    [Tooltip("A game object that holds the sub-loading bar UI")]
    public GameObject subLoaderBar;
    
    [Header("UI Widget - for some games")]
    [Tooltip("a button for starting the game")]
    public Button readyBtn;
    [Tooltip("a script that handles betting decision in the game")]
    public GameObject playerAction;
    [Tooltip("a script that handles all the UI-text objects in the scene")]
    public GameObject labelController;

    [HideInInspector]
    public List<GameObject> interactable; // a list to store all interactable UI objects from the scene
                                          // interactable objects will be hidden when the game is paused
    private UIPage initPage;              // the background window
    private UIPage currentPage;           // the current displayed window

    // Start is called before the first frame update
    void Start()
    {        
        // initialize the background page
        switch (sceneType)
        {
            case SceneType.Homepage:
                initPage = new UIPage(initButtons);
                break;
            case SceneType.InCasino:
                initPage = new UIPage(initButtons);
                break;
            case SceneType.InGame:
                initPage = new UIPage(initButtons);
                break;
            default:
                break;
        }

        // set current page to be the init page
        currentPage = initPage;

        // initialize interactable object list
        interactable = new List<GameObject>();

        // hide some objects that should not be displayed by default
        SetDefaultObjectVisibility(false);
    }

    // Update is called once per frame
    void Update() { OnESC(); }

    // Terminate the program
    public void Terminate() { Application.Quit(); }

    /// <summary>
    /// Method to pop up a new window and hide the old window
    /// </summary>
    /// <param name="holder">the popped up window</param>
    public void CreatePage(GameObject holder)
    {
        currentPage = new UIPage(holder, currentPage);
        currentPage.prevPage?.Display(false);
        currentPage.Display(true);
    }

    /// <summary>
    /// Method to close the current window by clicking 
    /// on the terminate button in the interface
    /// </summary>
    public void ClosePage()
    {
        currentPage.Display(false);
        currentPage = currentPage.prevPage;
        currentPage.Display(true);
    }

    /// <summary>
    /// Method that allows the user to close the current
    /// window by pressing the ESC key
    /// </summary>
    void OnESC()
    {
        // return if ESC key is not pressed
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        // close current page if it has a previous page
        if (currentPage.prevPage != null)
            ClosePage();
    }

    /// <summary>
    /// Method to modify visibility for default UI objects
    /// </summary>
    public void SetDefaultObjectVisibility(bool flag)
    {
        for (int i = 0; i < initialHide.Length; i++)
            initialHide[i].SetActive(flag);
    }

    /// <summary>
    /// Method to modify the init buttons visibility
    /// </summary>
    public void SetInitButtonVisbility(bool flag)
    {
        initButtons.SetActive(flag);
    }

    /// <summary>
    /// Method to modify the interactable object's visibiilty
    /// </summary>
    public void SetInteractableVisibility(bool flag)
    {
        for (int i = 0; i < interactable.Count; i++)
            interactable[i].SetActive(flag);
    }
}

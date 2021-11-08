using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public bool lockPage;                 // determine whether or not the user can go back to previous page
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
                Blackboard.LockCursor(false);
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

        // initialize audio scene
        InitializeAudioScene();
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
    /// Method to pop up a new window without closing the old one
    /// </summary>
    /// <param name="holder">the popped up window</param>
    public void CreatePageAdditive(GameObject holder)
    {
        currentPage = new UIPage(holder, currentPage);
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
        // return if the page is lock
        if (lockPage)
            return;

        // return if there's no previous page of the current page
        if (currentPage.prevPage == null)
            return;

        // return if ESC key is not pressed
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        // find all buttons that have "Cancel Button" tag with it and 
        // invoke its on click method
        foreach (var btn in GameObject.FindGameObjectsWithTag("CancelButton"))
            btn.GetComponent<Button>().onClick.Invoke();        
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

    /// <summary>
    /// Method to initialize the audio scene
    /// </summary>
    public void InitializeAudioScene()
    {
        // return if the audio scene is already loaded 
        if (SceneManager.GetSceneByName(Const.SCENE_AUDIO).isLoaded)
            return;

        // otherwise load the audio scene
        SceneManager.LoadScene(Const.SCENE_AUDIO, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Methods to play mouse enter & click buton sound effect
    /// </summary>
    public void PlayMouseEnterSound() => Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipMouseEnterBtn, AudioType.UI);
    public void PlayMouseClickSound() => Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipMouseClickBtn, AudioType.UI);
}

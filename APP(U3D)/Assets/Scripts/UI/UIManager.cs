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

    [Header("UI Widgets")]
    [Tooltip("The initial buttons in homepage")]
    public GameObject initButtons;

    private UIPage initPage;           // the background window
    private UIPage currentPage;        // the current displayed window
    private GameObject[] interactable; // an array to store all interactable UI objects from the scene

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
                break;
            case SceneType.InGame:
                initPage = new UIPage(initButtons);
                break;
            default:
                break;
        }

        // set current page to be the init page
        currentPage = initPage;

        // find the interactable object in the scene
        FindInteractable();
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
    /// Method to modify the init buttons visibility
    /// </summary>
    public void SetInitButtonVisbility(bool flag)
    {
        initButtons.SetActive(flag);
    }

    /// <summary>
    /// Method to get all interactable UI objects in the scene
    /// </summary>
    private void FindInteractable()
    {
        interactable = GameObject.FindGameObjectsWithTag("Interactable");
    }

    /// <summary>
    /// Method to modify the interactable object's visibiilty
    /// </summary>
    public void SetInteractableVisibility(bool flag)
    {
        for (int i = 0; i < interactable.Length; i++)
        {
            interactable[i].SetActive(flag);
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;
using Archive;
using TMPro;

public class Entering : MonoBehaviour
{
    [Header("Register Panel")]
    [Tooltip("A game object that shows user registration panel")]
    public GameObject register;
    [Tooltip("Button to confirm entered username")]
    public Button nextBtn;
    [Tooltip("Where player enters his username")]
    public TMP_InputField inputField;

    [Header("Dependencies")]
    [Tooltip("A script to handle all UI functionalities")]
    public UIManager uiManager;
    [Tooltip("A game object contains start a new or load panel")]
    public GameObject startOrLoad;
    [Tooltip("A script to deal with archive system")]
    public PanelManager archiveManager;

    private string newName;    // a name that the player is going to use

    /// <summary>
    /// Method to call when the player clicks the 'Play' button, go through each archive
    /// panel and see if there is any archive record.
    /// case0: find any archive record => pop up [Start a new Game / Load a saved game] panel
    /// case1: no record found => pop up [register] panel
    /// </summary>
    public void AttempToPlay()
    {
        // scan through archive panels and determine whether or not record exists
        var hasRecord = false;
        for (int i = 0; i < archiveManager.archives.Length; i++)
        {
            if (archiveManager.archives[i].HasRecord())
            {
                hasRecord = true;
                break;
            }
        }

        // pop up either save/load panel or register panel
        if (hasRecord)
            uiManager.CreatePage(startOrLoad);
        else
            uiManager.CreatePage(register);
    }

    /// <summary>
    /// Method to call when the inputfiled has been changed
    /// </summary>
    /// <param name="newName"></param>
    public void InputChange(string newName)
    {
        // record the newName
        this.newName = newName;

        // if the character length is 0, disable the 'next' button
        if (newName.Length == 0 && nextBtn.enabled)
            nextBtn.Switch(false);
        // enable the 'next' button as long as the user has entered anything
        else if (newName.Length > 0 && !nextBtn.enabled)
            nextBtn.Switch(true);

        // play input change sound effect
        Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipInputFieldChange, AudioType.UI);
    }

    /// <summary>
    /// Method to reset input field from register panel
    /// </summary>
    public void ResetInput() => inputField.text = "";

    /// <summary>
    /// Method to set the last edited name as the local player name
    /// </summary>
    public void UpdateLocalPlayerName() => Storage.SaveString(Const.LOCAL_PLAYER, StorageType.Name, newName);
}
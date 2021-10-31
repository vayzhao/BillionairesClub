﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalTagManager : MonoBehaviour
{
    [Tooltip("A script that handles all UI operation")]
    public UIManager uiManager;
    [Tooltip("All portal in this scene")]
    public PortalTag[] portalTags;
    [Tooltip("A script that handles poker chip exchange methods")]
    public ExchangeChip exchangeChip;    
    [Tooltip("A game object to display confirmation window when exiting the casino")]
    public GameObject exitConfirmation;

    private bool isRunning;   // determine whether or not this script is running
    private int triggerIndex; // an index to indicate which portal the player is near to
    private Transform player; // a variable to record the player

    private void Update()
    {
        CheckSpaceBarInput();
    }

    /// <summary>
    /// Method to setup the manager, by creating all the portal objects in the scene
    /// and bind player transform to the script, player transform will be used to 
    /// measure distance between player and portal in runtime
    /// </summary>
    /// <param name="player">the player</param>
    public void Setup(Transform player)
    {
        // bind player to this script
        this.player = player;

        // spawn portal fxs
        for (int i = 0; i < portalTags.Length; i++)
            portalTags[i].Spawn();

        // start the range-check coroutine
        StartCoroutine(CheckPlayerPosition());
    }

    /// <summary>
    /// Method to stop measuring distance between player and portals, also
    /// hidding the current displayed portal tooltip
    /// </summary>
    public void Stop()
    {
        // if a portal tooltip is displayed, hide it
        if (triggerIndex >= 0)
            portalTags[triggerIndex].tag.SetActive(false);

        // stop range-check coroutine
        isRunning = false;
        StopCoroutine(CheckPlayerPosition());
    }

    /// <summary>
    /// Method to resume range-check coroutine
    /// </summary>
    public void Resume()
    {
        StartCoroutine(CheckPlayerPosition());
    }

    /// <summary>
    /// An IEnumerator to check distance between player and portals everyframe
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckPlayerPosition()
    {
        // initialize runner bool
        isRunning = true;

        // initialize trigger index
        triggerIndex = -1;

        while (isRunning)
        {
            // wait for a frame before execution
            yield return new WaitForSeconds(Time.deltaTime);

            // get player's position in this frame
            var pos = player.position;

            // if the player is near to a portal, do nothing
            if (triggerIndex >= 0 && portalTags[triggerIndex].IsPlayerInRange(pos))
                continue;

            // otherwise, hide the portal tooltip and reset trigger index
            if (triggerIndex != -1)
                portalTags[triggerIndex].tag.SetActive(false);
            triggerIndex = -1;

            // and find if there is any portal near to the player
            for (int i = 0; i < portalTags.Length; i++)
            {
                if (portalTags[i].IsPlayerInRange(pos))
                {
                    triggerIndex = i;
                    portalTags[triggerIndex].tag.SetActive(true);
                    continue;
                }
            }            
        }
    }

    /// <summary>
    /// A method to check player's space-bar input
    /// </summary>
    void CheckSpaceBarInput()
    {
        // return if the script is not running
        if (!isRunning)
            return;

        // return if the player is not near to any portal
        if (triggerIndex == -1)
            return;

        // return if the trigger index is greater than 1
        // 0: back to home scene
        // 1: exchange poker chip
        // >1: game
        if (triggerIndex > 1)
            return;

        // return if the player is not pressing Space Bar
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        // check which portal the player is using
        switch (triggerIndex)
        {
            case 0:
                BackToHomepage();
                break;
            case 1:
                exchangeChip.PopUp();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// A method to pop up 'back to homepage' window
    /// </summary>
    void BackToHomepage()
    {
        // stop portals range-check coroutine
        Stop();

        // pop up confirmation window
        uiManager.CreatePage(exitConfirmation);
    }
    
    /// <summary>
    /// A method to confirm leaving the casino, taking the 
    /// user back to home page
    /// </summary>
    public void ConfirmExit()
    {
        // close current page
        uiManager.ClosePage();

        // load back to home page scene
        SceneManager.LoadScene(Const.SCENE_HOMEPAGE);
    }
}
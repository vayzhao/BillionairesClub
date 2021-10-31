﻿using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExchangeChip: MonoBehaviour
{
    [Header("Core Objects")]
    [Tooltip("A game object that holds the window for chip exchange")]
    public GameObject window;
    [Tooltip("A script that handles all UI operation")]
    public UIManager uiManager;
    [Tooltip("A script that holds portal operation")]
    public PortalTagManager portals;

    [Header("Second Panel")]
    public GameObject secondPanel;
    public GameObject warningPanel;
    public GameObject confirmPanel;
    public GameObject congratPanel;
    public TextMeshProUGUI confirmText;

    [HideInInspector]
    public bool isHolding;

    private int purchaseItemIndex;
    private int[] chipAmount = new int[5] { 1000, 2500, 5000, 10000, 20000 };
    private int[] gemRequire = new int[5] { 50, 110, 200, 350, 600 };

    /// <summary>
    /// A method to pop up the panel for chip exchange
    /// </summary>
    public void PopUp()
    {
        // stop portals range-check coroutine
        portals.Stop();

        // pop up the panel
        uiManager.CreatePage(window);

        // force the user to focus on the window
        Blackboard.FocusOnWindow(true);
    }

    /// <summary>
    /// A method to close the chip exchange panel
    /// </summary>
    public void Close()
    {
        // resume portals range-check coroutine
        portals.Resume();

        // release user from the chip exchange panel
        Blackboard.FocusOnWindow(false);
    }

    /// <summary>
    /// Method to pop up a window when the user is trying to exchange poker chip
    /// pop up warning window when the gem is insufficient
    /// pop up confirming window when the gem is enough
    /// </summary>
    /// <param name="itemIndex"></param>
    public void AttemptToPurchase(int itemIndex)
    {
        // switch isHolding to be true to prevent UI manager from closing 
        // the cashier panel when poping up a secondary panel
        isHolding = true;

        // pop up the secondary panel
        uiManager.CreatePage(secondPanel);

        // initially hide all second panels 
        warningPanel.SetActive(false);
        confirmPanel.SetActive(false);
        congratPanel.SetActive(false);

        // check to see if the player has enough gem to purchase
        if (Blackboard.localPlayer.gem >= gemRequire[itemIndex]) 
        {
            // store puchase item index
            purchaseItemIndex = itemIndex;

            // if the player has enough gem, pop up the confirmation panel
            confirmPanel.SetActive(true);

            // modify the text in confirmPanel
            confirmText.text = $"Change  <sprite name=\"Chip\"><color=\"green\">{chipAmount[itemIndex]:C0}</color>" +
                $" with  <sprite name=\"Gem\"><color=\"green\">{gemRequire[itemIndex]}</color>";
        }
        else
        {
            // otherwise pop up a warning panel
            warningPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Method to confirm purchase
    /// </summary>
    public void ConfirmPuchase()
    {
        // swap confirm panel to congrats panel
        confirmPanel.SetActive(false);
        congratPanel.SetActive(true);

        // exchange player's resource
        Blackboard.localPlayer.ExchangePokerChip(gemRequire[purchaseItemIndex], chipAmount[purchaseItemIndex]);
    }
}
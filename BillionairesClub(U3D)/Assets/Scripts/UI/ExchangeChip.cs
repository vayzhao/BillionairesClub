using System;
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

    [Header("First Panel")]
    public Image playerPortrait;
    [Tooltip("The text that says hello to the player")]
    public TextMeshProUGUI greetingText;

    [Header("Second Panel")]
    [Tooltip("The game object that holds the warning panel, confirm panel and congrat panel")]
    public GameObject secondPanel;
    [Tooltip("The panel that warns the player when attempting to purchase something not affordable")]
    public GameObject warningPanel;
    [Tooltip("The panel for the user to confirm when purchasing")]
    public GameObject confirmPanel;
    [Tooltip("The panel for notificating the player that the purchase was successful")]
    public GameObject congratPanel;
    [Tooltip("The text for the confirm panel")]
    public TextMeshProUGUI confirmText;

    private int purchaseItemIndex;                                            // index of the exchange type    
    private int[] chipAmount = new int[5] { 1000, 2500, 5000, 10000, 20000 }; // chip amount for each exchange type    
    private int[] gemRequire = new int[5] { 50, 110, 200, 350, 600 };         // gem requirement for each exchange type

    /// <summary>
    /// A method to pop up the panel for chip exchange
    /// </summary>
    public void PopUp()
    {
        // stop portals range-check coroutine
        portals.Stop();

        // update the greeting text and player's protrait
        playerPortrait.sprite = Blackboard.GetPortraitPrefab(Blackboard.localPlayer.modelIndex);
        greetingText.text = $"Hi, {Blackboard.localPlayer.name}. How can I help?";

        // pop up the panel
        uiManager.CreatePage(window);

        // play greetings sound effect
        Blackboard.audioManager.PlayAudio(Blackboard.audioManager.cashierGreetings, AudioType.UI);
    }

    /// <summary>
    /// A method to close the chip exchange panel
    /// </summary>
    public void Close()
    {
        // return if the second panel is displayed 
        if (secondPanel.activeSelf)
            return;

        // close the page
        uiManager.ClosePage();

        // resume portals range-check coroutine
        portals.Resume();
    }

    /// <summary>
    /// Method to pop up a window when the user is trying to exchange poker chip
    /// pop up warning window when the gem is insufficient
    /// pop up confirming window when the gem is enough
    /// </summary>
    /// <param name="itemIndex"></param>
    public void AttemptToPurchase(int itemIndex)
    {
        // pop up the secondary panel
        uiManager.CreatePageAdditive(secondPanel);

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

            // play warning sound effect
            Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipWarning, AudioType.UI);
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

        // play purchase successful sound effect
        Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipPurchaseSuccessful, AudioType.UI);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerBoard : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("A image component that shows the player's protrait")]
    public Image portrait;
    [Tooltip("A text component that shows player's remaining chip")]
    public TextMeshProUGUI chipText;
    [Tooltip("A text component that shows player's remaining gem")]
    public TextMeshProUGUI gemText;

    private Player player; // the player that this player board refers to 

    /// <summary>
    /// Method to bind the player's information to this playerboard
    /// </summary>
    /// <param name="player"></param>
    public void BindToPlayer(Player player)
    {
        // bind player to this script
        this.player = player;

        // bind this playerboard to the player
        player.playerBoard = this;

        // update portrait sprite 
        portrait.sprite = Blackboard.GetPortraitPrefab(player.modelIndex);

        // update text's value
        UpdateValue();
    }

    /// <summary>
    /// Method to update playerboard's remaining chip and gem
    /// </summary>
    public void UpdateValue()
    {
        chipText.text = player.chip.ToString("C0");
        gemText.text = player.gem.ToString();
    }
}
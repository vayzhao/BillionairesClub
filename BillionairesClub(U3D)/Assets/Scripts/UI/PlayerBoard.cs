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
    [Tooltip("A text component that shows player's name")]
    public TextMeshProUGUI nameText;
    [Tooltip("A text component that shows player's current description")]
    public TextMeshProUGUI descriptionText;
    [Tooltip("A text component that shows player's remaining chip")]
    public TextMeshProUGUI chipText;
    [Tooltip("A text component that shows player's remaining gem")]
    public TextMeshProUGUI gemText;
    [Tooltip("A component that allows the user to edit playerboard description")]
    public TMP_InputField inputField;

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

        // update player's name & description
        nameText.text = player.name;
        inputField.text = player.description;

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

    /// <summary>
    /// Method to lock the keyboard when the user is trying to
    /// modify the player board description. We do that to prevent the 
    /// character from moving when the user is trying to type white space
    /// </summary>
    public void StartEditing() => CursorController.isTypingText = true;

    /// <summary>
    /// Method to play input change sound effect when the player is editing the input field
    /// </summary>
    public void InputChange() => Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipInputFieldChange, AudioType.UI);

    /// <summary>
    /// Method to unlock the keyboard when the user finished modifying
    /// player board description
    /// </summary>
    public void EndEditing(string message)
    {
        // unlock keyboard
        CursorController.isTypingText = false;

        // immediatly disable and re-enable the input field so the 
        // player will not re-select the input field unintentionally 
        // when pressing the space bar again
        inputField.interactable = false;
        inputField.interactable = true;

        // update this description text to the player
        player.description = message;
    }
}
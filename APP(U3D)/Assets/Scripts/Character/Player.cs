﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Main")]
    [Tooltip("Determine whether or not the player is a NPC")]
    public bool isNPC;

    [Header("Property")]
    [Tooltip("The amount of money this player has")]
    public int chip;
    [Tooltip("The amount of chip this player has")]
    public int gem;

    [HideInInspector]
    public int modelIndex;          // index of player's model
    [HideInInspector]
    public PlayerBoard playerBoard; // the player board binded to this player

    /// <summary>
    /// Method to increase or decrease a player's chip amount
    /// update playerboard data if this player is a user
    /// </summary>
    /// <param name="amount">the amount of chip change</param>
    public void EditPlayerChip(int amount)
    {
        // modify chip amount
        chip += amount;

        // update playerboard information
        if (!isNPC)
            playerBoard.UpdateValue();
    }

}
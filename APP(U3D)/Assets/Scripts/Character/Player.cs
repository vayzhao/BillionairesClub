using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isNPC;
    public int chip;
    public int gem;

    [HideInInspector]
    public int modelIndex;
    [HideInInspector]
    public PlayerBoard playerBoard;

    private int chipRed;
    private int chipBlue;
    private int chipYellow;
    private int chipPink;
    private int chipBlack;
    
    public int GetTotalChip()
    {
        return chipRed * 5
            + chipBlue * 10
            + chipYellow * 20
            + chipPink * 50
            + chipBlack * 100;
    }

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
using System;
using System.Collections;
using UnityEngine;

public struct Bet
{
    public int bonusWager;  // amount of money the player bets on bouns
    public int anteWager;   // amount of money the player bets on ante
    public int flopWager;   // amount of money the player bets on flop
    public int turnWager;   // amount of money the player bets on turn
    public int riverWager;  // amount of money the player bets on river
    public bool hasFolded;  // determine whether or not the player has folded in this round

    // reset all types of wager to initial
    public void Reset()
    {
        bonusWager = 0;
        anteWager = 0;
        flopWager = 0;
        turnWager = 0;
        riverWager = 0;
        hasFolded = false;
    }

    /// <summary>
    /// Method to sum the amount of bet in ante, flop, turn and river
    /// </summary>
    /// <returns></returns>
    public int GetTotal() { return anteWager + flopWager + turnWager + riverWager; }



}
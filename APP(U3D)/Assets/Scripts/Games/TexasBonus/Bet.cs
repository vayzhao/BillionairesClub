using System;
using System.Collections;
using UnityEngine;

public struct Bet
{
    public int bonusWager;
    public int anteWager;
    public int flopWager;
    public int turnWager;
    public int riverWager;
    public bool hasFolded;

    public void Reset()
    {
        bonusWager = 0;
        anteWager = 0;
        flopWager = 0;
        turnWager = 0;
        riverWager = 0;
        hasFolded = false;
    }

    public int GetTotal()
    {
        return anteWager + flopWager + turnWager + riverWager;
    }




}
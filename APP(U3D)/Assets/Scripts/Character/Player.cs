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



}
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeforeBet : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI wagerText;
    public Button btn_reset;
    public Button btn_bet;
    public Button btn_fold;
    public GameObject wagerPanel;
    
    [Header("Options")]
    public Toggle isSkipDrag;
    public Toggle isRightClickBet;

    [Header("Wager Options")]
    public DraggableChip[] chips;
    [HideInInspector]
    public int[] chipValues;

    [HideInInspector]
    public Blackboard.VoidDel methodFold;
    [HideInInspector]
    public Blackboard.VoidDel methodCheck;
    [HideInInspector]
    public Blackboard.VoidDel methodBet;

    [HideInInspector]
    public int totalBet;
    private int remaining;
    private int remainingTemp;

    void Start()
    {
        InitializeValues();
    }

    void Update()
    {
        RightClickDetect();
    }

    /// <summary>
    /// Method to setup an array of all poker chip values
    /// </summary>
    void InitializeValues()
    {
        chipValues = new int[chips.Length];
        for (int i = 0; i < chipValues.Length; i++)
            chipValues[i] = chips[i].value;
    }

    void RightClickDetect()
    {
        // return if right click bet is off
        if (!isRightClickBet.isOn)
            return;

        // return if the user is not pressing right-click
        if (!Input.GetMouseButtonDown(1))
            return;

        // determine which action to take
        if (totalBet == 0)
            Fold();
        else
            Bet();
    }

    void RefreshChipValidity(int value)
    {
        for (int i = 0; i < chips.Length; i++)
            chips[i].Enable(chips[i].value <= value);
    }

    public void DisplayWagerPanel(string titleString, int remaining)
    {
        this.remaining = remaining;

        wagerPanel.SetActive(true);

        titleText.text = titleString;

        ResetChip();
    }

    public void AddChip(int value)
    {
        if (totalBet == 0)
        {
            btn_reset.Switch(true);
            btn_bet.gameObject.SetActive(true);
            btn_fold.gameObject.SetActive(false);
        }
        
        totalBet += value;
        remainingTemp -= value;
        wagerText.text = $"{totalBet:C0}";

        RefreshChipValidity(remainingTemp);
    }

    public void ResetChip()
    {
        totalBet = 0;
        remainingTemp = remaining;
        btn_reset.Switch(false);
        btn_bet.gameObject.SetActive(false);
        btn_fold.gameObject.SetActive(true);
        wagerText.text = "";

        RefreshChipValidity(remainingTemp);
    }


    /// <summary>
    /// Delegate methods for fold/check/bet
    /// </summary>
    public void Bet() => methodBet.Invoke();
    public void Fold() => methodFold.Invoke();
    public void Check() => methodCheck.Invoke();
}
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeforeBet : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI wagerText;

    public int total;
    public bool isWaiting;

    public Button btn_reset;
    public Button btn_bet;
    public Button btn_fold;

    [Header("Options")]
    public Toggle isSkipDrag;
    public Toggle isRightClickBet;

    public delegate void Method();
    public Method methodFold;
    public Method methodCheck;
    public Method methodBet;

    void Start()
    {
        Reset();
    }

    void Update()
    {
        RightClickDetect();
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
        if (total == 0)
            Fold();
        else
            Bet();
    }

    public void AddChip(int value)
    {
        if (total == 0)
        {
            btn_reset.Switch(true);
            btn_bet.gameObject.SetActive(true);
            btn_fold.gameObject.SetActive(false);
        }
        
        total += value;
        wagerText.text = $"{total:C0}";
    }

    public void Reset()
    {
        total = 0;
        btn_reset.Switch(false);
        btn_bet.gameObject.SetActive(false);
        btn_fold.gameObject.SetActive(true);
        wagerText.text = "";
    }


    /// <summary>
    /// Delegate methods for fold/check/bet
    /// </summary>
    public void Bet() => methodBet.Invoke();
    public void Fold() => methodFold.Invoke();
    public void Check() => methodCheck.Invoke();
}
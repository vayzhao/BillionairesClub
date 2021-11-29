using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeforeBet : MonoBehaviour
{

    public int total;

    public Button btn_reset;
    public Button btn_bet;
    public Button btn_fold;
    public TextMeshProUGUI betText;

    public Toggle isSkipDrag;
    public Toggle isRightClickBet;

    private void Start()
    {
        Reset();
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
        betText.text = $"{total:C0}";
    }

    public void Reset()
    {
        total = 0;
        btn_reset.Switch(false);
        btn_bet.gameObject.SetActive(false);
        btn_fold.gameObject.SetActive(true);
        betText.text = "";
    }

    public void Bet()
    {

    }

    public void Fold()
    {

    }
}
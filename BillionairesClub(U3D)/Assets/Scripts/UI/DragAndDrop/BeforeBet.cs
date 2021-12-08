using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Blackboard;

public class BeforeBet : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("A text component used to display name of the panel")]
    public TextMeshProUGUI titleText;
    [Tooltip("A text component used to display amount of wager the player bets")]
    public TextMeshProUGUI wagerText;
    [Tooltip("A button to clear up wagers")]
    public Button btn_reset;
    [Tooltip("The bet button")]
    public Button btn_bet;
    [Tooltip("The skip button")]
    public Button btn_skip;
    [Tooltip("A game object that holds the wager panel")]
    public GameObject wagerPanel;
    
    [Header("Options")]
    [Tooltip("An option that allows the player to add wager by left-clicking the chip")]
    public Toggle isSkipDrag;
    [Tooltip("An option that allows the player to bet by right-clicking the mouse")]
    public Toggle isRightClickBet;

    [Header("Wager Options")]
    [Tooltip("Button for all draggable chips")]
    public DraggableChip[] chips;
    [HideInInspector]
    public int[] chipValues;    // values for all draggable chips

    [HideInInspector]
    public int playerIndex;     // determine whose turn it is
    [HideInInspector]
    public int totalWager;      // amount of wager the player wants to bet in this round
    [HideInInspector]
    public int remaining;       // amounf of wager the player had when this round started
    [HideInInspector]
    public int remainingTemp;   // amount of wager the player has now
    [HideInInspector]
    public BetType betType;     // which type of bet it is
    [HideInInspector]
    public bool isWaiting;      // determine whether or not this script is waiting for a player to make decision

    public VoidDel methodFold;  // method for folding
    public VoidDel methodCheck; // method for checking
    public VoidDel methodBet;   // method for betting
    public VoidDel methodClear; // method for clearing chip
    public VoidDelI methodAdd;  // method for adding chip

    

    void Start() => InitializeValues();

    /// <summary>
    /// Method to setup an array of all poker chip values
    /// </summary>
    void InitializeValues()
    {
        chipValues = new int[chips.Length];
        for (int i = 0; i < chipValues.Length; i++)
            chipValues[i] = chips[i].value;
    }
    
    /// <summary>
    /// An ienumerator to detect whether or not the user is 
    /// trying to bet with right-click
    /// </summary>
    /// <returns></returns>
    public IEnumerator RightClickDetect()
    {
        // start an infinite while loop
        while (true)
        {
            yield return new WaitForSeconds(Time.deltaTime);

            // skip this iteration in the following cases
            // case0: right button is not pressed
            // case1: right click bet option is activated
            // case2: the bet button is not available
            if (!Input.GetMouseButton(1)
                || !isRightClickBet.isOn
                || !btn_bet.isActiveAndEnabled)
                continue;

            // otherwise, bet and break the loop
            Bet();
            break;            
        }
    }

    /// <summary>
    /// Method to refresh chip validity, for example, if the player has only
    /// $20 remaining, all chips with value more than $20 will turn gray
    /// </summary>
    /// <param name="value"></param>
    public void RefreshChipValidity(int value)
    {
        for (int i = 0; i < chips.Length; i++)
            chips[i].Enable(chips[i].value <= value);
    }

    /// <summary>
    /// Method to display wager panels, also store player's remaining chip
    /// into the script, the reason to do that is because we need to know 
    /// how much money the player originally has, so we can reset it when need
    /// </summary>
    /// <param name="titleString">name of the panel</param>
    /// <param name="remaining">player's remaining poker chip</param>
    public void DisplayWagerPanel(string titleString, int remaining)
    {
        // store the remaining value
        this.remaining = remaining;

        // pop up the panel and rename it
        wagerPanel.SetActive(true);
        titleText.text = titleString;
        wagerText.text = "";

        // start right click detection coroutine 
        StartCoroutine(RightClickDetect());

        // initial the wager value
        Clear();
    }

    /// <summary>
    /// Delegate methods for fold/check/bet
    /// </summary>
    public void Bet() => methodBet.Invoke();
    public void Fold() => methodFold.Invoke();
    public void Check() => methodCheck.Invoke();
    public void Clear() => methodClear.Invoke();
    public void Add(int i) => methodAdd.Invoke(i);
}
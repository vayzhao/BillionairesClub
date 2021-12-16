using System;
using UnityEngine;
using UnityEngine.UI;
using static Blackboard;

namespace Blackjack
{
    public class PlayerAction : BeforeBet
    {
        [Header("Decision Panel")]
        [Tooltip("A game object that holds hit button")]
        public Button btn_hit;
        [Tooltip("A game object that holds stand button")]
        public Button btn_stand;
        [Tooltip("A game object that holds double down button")]
        public Button btn_double;
        [Tooltip("A game object that holds split button")]
        public Button btn_split;
        [Tooltip("A game object that holds hit/stand/double/split buttons")]
        public GameObject decisionPanel;

        [HideInInspector]
        public Bet[] bets;                      // bet data for players
        [HideInInspector]
        public GameManager gameManager;         // the game manager script
        [HideInInspector]
        public LabelController labelController; // the table controller script
        [HideInInspector]
        public TableController tableController; // the label controller script
        
        public void SetUp()
        {
            // initialize bet data for all players
            bets = new Bet[gameManager.players.Length];

            // register delegate method for decisions            
            methodAdd = Add;
            methodBet = Bet;
            methodClear = Clear;
            methodCheck = Check;
        }

        /// <summary>
        /// Method to reset bet data for all players, it is called 
        /// when a new round starts
        /// </summary>
        public void ResetBet()
        {
            // reset bet array's data
            for (int i = 0; i < bets.Length; i++)
                bets[i].Reset();

            // remove all poker chip models from the table
            tableController.ClearWagerStackForAll();
        }

        /// <summary>
        /// Method to display a decision panel for a non-npc player
        /// </summary>
        /// <param name="betType">the bet type</param>
        /// <param name="playerIndex">whose term is it</param>
        public void DisplayBetPanel(BetType betType, int playerIndex)
        {
            // switch waiting state to be true and record the playerIndex and bet type
            this.isWaiting = true;
            this.betType = betType;
            this.playerIndex = playerIndex;

            // display asscoated decision panel to the player, depends on the bet type
            switch (betType)
            {
                case BetType.PerfectPair:
                    DisplayWagerPanel("Perfect Pair Wager", gameManager.players[playerIndex].chip);
                    break;
                case BetType.AnteWager:
                    DisplayWagerPanel("Ante Wager", gameManager.players[playerIndex].chip);
                    break;
                case BetType.Insurance:
                    DisplayWagerPanel("Insurance Wager", 
                        Mathf.Min(gameManager.players[playerIndex].chip, bets[playerIndex].anteWager / 2));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Method to display decision panel, includes hit, stand,
        /// double down and split buttons
        /// </summary>
        public void DisplayDecisionPanel(int playerIndex)
        {
            // switch waiting state to be true and record the playerIndex and bet type
            this.isWaiting = true;
            this.playerIndex = playerIndex;

            // display decision panel
            decisionPanel.SetActive(true);

            // check double down and split button validity
            var hand = tableController.GetPlayerHand(playerIndex);
            btn_double.Switch(hand.GetRank() <= 11);
            btn_split.Switch(hand.IsPairSameValue());
        }

        public void DoubleDown()
        {

        }

        public void Hit()
        {
            tableController.OnPlayerHit(playerIndex, 0);
        }

        public void Stand()
        {

            bets[playerIndex].stand = true;
            FinishTurn();
            audioManager.PlayAudio(audioManager.clipCheck, AudioType.Sfx);
        }

        public void Split()
        {

        }

        /// <summary>
        /// Method to finish a turn for a player, switch isWaiting to false
        /// so that the coroutine can continue to move to the next player
        /// also, hide the wager panel & decision panel
        /// </summary>
        void FinishTurn()
        {
            isWaiting = false;
            wagerPanel.SetActive(false);
            decisionPanel.SetActive(false);
            StopCoroutine(RightClickDetect());
        }

        /// <summary>
        /// Method for players to bet on ante wager, it is called from
        /// the button click event in the Game UI
        /// </summary>
        /// <param name="value">the bet wager value</param>
        void BetAnteWager(int value)
        {
            // store value into bet data
            bets[playerIndex].anteWager = value;

            // consume player's chip
            gameManager.players[playerIndex].EditPlayerChip(-value);
            labelController.SetBetLabel(playerIndex, bets[playerIndex].anteWager);
        }

        /// <summary>
        /// Method for players to bet on perfect pair wager, it is called 
        /// from the button click event in the Game UI
        /// </summary>
        /// <param name="value">the bet wager value</param>
        void BetPerfectPair(int value)
        {
            // store value into the bet data
            bets[playerIndex].perfectPairWager = value;

            // consume player's chip
            gameManager.players[playerIndex].EditPlayerChip(-value);
            labelController.SetBetLabel(playerIndex, bets[playerIndex].anteWager, bets[playerIndex].perfectPairWager);
        }

        /// <summary>
        /// Method for players to bet on insurance wager, it is called 
        /// from the button click event in the Game UI
        /// </summary>
        /// <param name="value">the bet wager value</param>
        void BetInsuranceWager(int value)
        {
            // store value into the bet data
            bets[playerIndex].insuranceWager = value;

            // consume player's chip
            gameManager.players[playerIndex].EditPlayerChip(-value);
            labelController.insuranceBets[playerIndex].Display($"{bets[playerIndex].insuranceWager:C0}");
        }

        /// <summary>
        /// Method for players to bet
        /// </summary>
        new void Bet()
        {
            // finish this turn
            FinishTurn();

            // determine which bet turn it is
            switch (betType)
            {
                case BetType.PerfectPair:
                    BetPerfectPair(totalWager);
                    break;
                case BetType.AnteWager:
                    BetAnteWager(totalWager);
                    break;
                case BetType.Insurance:
                    BetInsuranceWager(totalWager);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Method for players to add a single chip into the wager pool
        /// </summary>
        /// <param name="value">value of the added chip</param>
        new void Add(int value)
        {
            // if this is the first chip being added, swithc on reset button
            if (totalWager == 0)
            {
                btn_reset.Switch(true);

                // if this is a ante wager bet, switch on bet button
                if (betType == BetType.AnteWager)
                    btn_bet.Switch(true);

                // if this is a bonud wager bet, display bet button 
                // and hide skip button
                else if (betType == BetType.PerfectPair || betType == BetType.Insurance)
                {
                    btn_bet.gameObject.SetActive(true);
                    btn_skip.gameObject.SetActive(false);
                }
            }

            // update the wager and remaining info
            totalWager += value;
            remainingTemp -= value;
            wagerText.text = $"{totalWager:C0}";

            // add single wager model to the associate slot
            if (betType == BetType.AnteWager)
                tableController.AddWagerModel(playerIndex, GameManager.WAGER_INDEX_ANTE, value);
            else if (betType == BetType.PerfectPair)
                tableController.AddWagerModel(playerIndex, GameManager.WAGER_INDEX_BONUS_SPLITE_WAGER, value);
            else if (betType == BetType.Insurance)
                tableController.AddWagerModel(playerIndex, GameManager.WAGER_INDEX_INSURANCE, value);

            // refresh chip's validity
            RefreshChipValidity(remainingTemp);
        }

        /// <summary>
        /// Method for the player to reset wager in 'before bet' section
        /// </summary>
        new void Clear()
        {
            // reset total wager & remaining
            totalWager = 0;
            remainingTemp = remaining;

            // switch off reset button and hide other button
            btn_reset.Switch(false);
            btn_bet.gameObject.SetActive(false);
            btn_skip.gameObject.SetActive(false);

            // if it is ante wager bet
            if (betType == BetType.AnteWager)
            {
                // reset bet button
                btn_bet.gameObject.SetActive(true);
                btn_bet.Switch(false);
                wagerText.text = "";

                // clear all wagers in ante wager slot
                tableController.ClearWagerStackForSingleSlot(playerIndex, GameManager.WAGER_INDEX_ANTE);
            }
            // if it is a bonud wager bet
            else if (betType == BetType.PerfectPair)
            {
                // display skip button
                btn_skip.gameObject.SetActive(true);

                // clear all wagers in bonus wager slot
                tableController.ClearWagerStackForSingleSlot(playerIndex, GameManager.WAGER_INDEX_BONUS_SPLITE_WAGER);
            }
            // if it is an insurance wager bet
            else if (betType == BetType.Insurance)
            {
                // display skip button
                btn_skip.gameObject.SetActive(true);

                // clear all wagers in insurance wager slot
                tableController.ClearWagerStackForSingleSlot(playerIndex, GameManager.WAGER_INDEX_INSURANCE);
            }

            // refresh chip's validity
            RefreshChipValidity(remainingTemp);
        }

        /// <summary>
        /// Method for players to skip
        /// </summary>
        new void Check()
        {
            // finish this turn
            FinishTurn();

            // play check sound effect
            Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipCheck, AudioType.Sfx);
        }
    }
}
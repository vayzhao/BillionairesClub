﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace TexasBonus
{
    public class PlayerAction : MonoBehaviour
    {
        [Header("UI Components")]
        [Tooltip("Panel that shows bonus wager buttons")]
        public GameObject group_bonusWager;
        [Tooltip("Panel that shows ante wager buttons")]
        public GameObject group_anteWager;
        [Tooltip("Panel that shows bet / fold buttons")]
        public GameObject group_betOrFold;
        [Tooltip("Panel that shows bet / check buttons")]
        public GameObject group_betOrCheck;

        [HideInInspector]
        public bool isWaiting;                  // determine whether or not this script is waiting for a player to make decision
        [HideInInspector]
        public Bet[] bets;                      // bet data for players
        [HideInInspector]
        public GameManager gameManager;         // the game manager script
        [HideInInspector]
        public TableController tableController; // the table controller script
        [HideInInspector]
        public LabelController labelController; // the label controller script

        private BetType betType;                // used in bet method, to determine whether it is flop bet, turn bet or river bet
        private int playerIndex;               // whose term is it

        public void Setup() 
        {
            bets = new Bet[gameManager.players.Length];
        }

        /// <summary>
        /// Method to reset bet data from all player, it is called when a new 
        /// round starts
        /// </summary>
        public void ResetBet()
        {
            // reset bet array's data
            for (int i = 0; i < bets.Length; i++)
                bets[i].Reset();

            // remove all poker chip models from the table
            tableController.RemoveWagerModel();
        }

        /// <summary>
        /// Method to display a decision panel for a non-npc player
        /// </summary>
        /// <param name="betType">the bet type of this decision</param>
        /// <param name="playerIndex">whose term is it</param>
        public void DisplayBetPanel(BetType betType, int playerIndex)
        {
            // switch waiting state to be true and record the playerIndex and bet type
            isWaiting = true;
            this.betType = betType;
            this.playerIndex = playerIndex;

            // display associated decision panel to the player, depends on the bet type
            switch (betType)
            {
                case BetType.BonusWager:
                    group_bonusWager.SetActive(true);
                    break;
                case BetType.AnteWager:
                    group_anteWager.SetActive(true);
                    break;
                case BetType.Flop:
                    group_betOrFold.SetActive(true);
                    break;
                case BetType.Turn:
                    group_betOrCheck.SetActive(true);
                    break;
                case BetType.River:
                    group_betOrCheck.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Method for the player to bet on bonus wager, it is called
        /// from the button click event in the game UI
        /// </summary>
        /// <param name="value">bonus wager amount</param>
        public void BetBonusWager(int value)
        {
            // switch waiting state to be false and hide the decision panel
            isWaiting = false;
            group_bonusWager.SetActive(false);

            // store value into bet data
            bets[playerIndex].bonusWager = value;

            // create some poker chip models to repersent the player's wager
            tableController.CreateWagerModel(playerIndex, 4, value);

            // consume player's chip 
            gameManager.players[playerIndex].EditPlayerChip(-value);
        }
        public void BetBonusWager_AI(int playerIndex)
        {
            // assume the value to be 5
            var value = 5;

            // repeat excatly how it functions in player's version
            this.playerIndex = playerIndex;
            BetBonusWager(value);
        }

        /// <summary>
        /// Method for the player to bet on ante wager, it is called
        /// from the button click event in the game UI
        /// </summary>
        /// <param name="value">ante wager amount</param>
        public void BetAnteWager(int value)
        {
            // switch waiting state to be false and hide the decision panel
            isWaiting = false;
            group_anteWager.SetActive(false);

            // store value into bet data
            bets[playerIndex].anteWager = value;

            // create some poker chip models to repersent the player's wager
            // and also update the text label to show how much money the player
            // has currently bet on this round
            tableController.CreateWagerModel(playerIndex, 0, value);
            labelController.SetBetLabel(playerIndex, bets[playerIndex].GetTotal());

            // consume player's chip 
            gameManager.players[playerIndex].EditPlayerChip(-value);            
        }
        public void BetAnteWager_AI(int playerIndex)
        {
            // assume the value to be 15
            var value = 15;

            // repeat excatly how it functions in player's version
            this.playerIndex = playerIndex;
            BetAnteWager(value);
        }

        /// <summary>
        /// Method for the player to bet on flop/turn/river wager, it
        /// is called from the button click event in the game UI
        /// </summary>
        /// <param name="value">wager amount</param>
        public void Bet()
        {
            // switch waiting state to be false and hide the decision panel
            isWaiting = false;
            group_betOrFold.SetActive(false);
            group_betOrCheck.SetActive(false);

            // check which type of bet it is 
            switch (betType)
            {
                case BetType.Flop:
                    bets[playerIndex].flopWager = bets[playerIndex].anteWager * 2;
                    tableController.CreateWagerModel(playerIndex, 1, bets[playerIndex].anteWager * 2);
                    gameManager.players[playerIndex].EditPlayerChip(bets[playerIndex].anteWager * -2);
                    break;
                case BetType.Turn:
                    bets[playerIndex].turnWager = bets[playerIndex].anteWager * 1;
                    tableController.CreateWagerModel(playerIndex, 2, bets[playerIndex].anteWager * 1);
                    gameManager.players[playerIndex].EditPlayerChip(bets[playerIndex].anteWager * -1);
                    break;
                case BetType.River:
                    bets[playerIndex].riverWager = bets[playerIndex].anteWager * 1;
                    tableController.CreateWagerModel(playerIndex, 3, bets[playerIndex].anteWager * 1);
                    gameManager.players[playerIndex].EditPlayerChip(bets[playerIndex].anteWager * -1);
                    break;
                default:
                    break;
            }

            // update the text label to show how much money the player
            // has currently bet on this round
            labelController.SetBetLabel(playerIndex, bets[playerIndex].GetTotal());
        }
        public void Bet_AI(BetType betType, int playerIndex)
        {
            // repeat excatly how it functions in player's version
            this.betType = betType;
            this.playerIndex = playerIndex;
            Bet();
        }
        
        /// <summary>
        /// Method for a player to fold the cards
        /// </summary>
        public void Fold()
        {
            // switch waiting state to be false and hide the decision panel
            isWaiting = false;
            group_anteWager.SetActive(false);
            group_betOrFold.SetActive(false);

            // update bet data, set hand to be folded
            bets[playerIndex].hasFolded = true;

            // display the player's card models face-down on the table 
            tableController.playerCardsObj[playerIndex].SetActive(true);

            // hide the hand-rank panel if the player is a user player
            if (!gameManager.players[playerIndex].isNPC)
                labelController.SetLocalHandRankPanelVisibility(false);
        }

        /// <summary>
        /// Method for a player to check
        /// </summary>
        public void Check()
        {
            // switch waiting state to be false and hide the decision panel
            isWaiting = false;
            group_betOrCheck.SetActive(false);
        }
    }
}
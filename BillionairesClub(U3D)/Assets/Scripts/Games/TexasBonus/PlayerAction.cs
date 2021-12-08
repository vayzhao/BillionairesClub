using System;
using UnityEngine;
using UnityEngine.UI;

namespace TexasBonus
{
    public class PlayerAction : BeforeBet
    {
        [Header("Decision Panel")]
        [Tooltip("A game object that holds fold button")]
        public GameObject obj_fold;
        [Tooltip("A game object that holds check button")]
        public GameObject obj_check;
        [Tooltip("A game object that holds bet button")]
        public GameObject obj_bet;
        [Tooltip("A game object that holds fold/check/bet buttons")]
        public GameObject decisionPanel;

        [HideInInspector]
        public Bet[] bets;                      // bet data for players
        [HideInInspector]
        public GameManager gameManager;         // the game manager script
        [HideInInspector]
        public TableController tableController; // the table controller script
        [HideInInspector]
        public LabelController labelController; // the label controller script

        public void Setup() 
        {
            // initialize bet data for all players
            bets = new Bet[gameManager.players.Length];

            // register delegate method for fold/check/bet
            methodFold = Fold;
            methodCheck = Check;
            methodBet = Bet;
            methodAdd = Add;
            methodClear = Clear;
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
            tableController.ClearWagerStackForAll();
        }

        /// <summary>
        /// Method to display a decision panel for a non-npc player
        /// </summary>
        /// <param name="betType">the bet type of this decision</param>
        /// <param name="playerIndex">whose term is it</param>
        public void DisplayBetPanel(BetType betType, int playerIndex)
        {
            // switch waiting state to be true and record the playerIndex and bet type
            this.isWaiting = true;
            this.betType = betType;
            this.playerIndex = playerIndex;

            // display associated decision panel to the player, depends on the bet type
            switch (betType)
            {
                // display bonus wager panel, the player has to have enough money to bet
                // on flop, that's why the remaining chip is assume to be (total-anteWager*2)
                case BetType.BonusWager:
                    DisplayWagerPanel("Bonus Wager", 
                        gameManager.players[playerIndex].chip - bets[playerIndex].anteWager * 2);
                    break;
                
                // display ante wager panel, the player cannot bet more 1/3 of his total chip
                // because he needs to have at least 2 times of ante wager to start the flop turn
                case BetType.AnteWager:
                    DisplayWagerPanel("Ante Wager", gameManager.players[playerIndex].chip / 3);
                    break;

                default:
                    DisplayDecisionPanel();
                    break;
            }
        }

        /// <summary>
        /// Method to display fold/check/bet panel for a non-npc player
        /// </summary>
        void DisplayDecisionPanel()
        {
            // display decision panel
            decisionPanel.SetActive(true);

            // in flop, the player will be asked to decide whether fold or bet
            if (betType == BetType.Flop)
            {
                obj_fold.SetActive(true);
                obj_check.SetActive(false);
                obj_bet.GetComponent<Button>().Switch(true);
            }
            // otherwise (turn OR river), the player will be asked to decided 
            // whether to check or bet
            else
            {
                obj_check.SetActive(true);
                obj_fold.SetActive(false);
                obj_bet.GetComponent<Button>().Switch(gameManager.players[playerIndex].chip >= bets[playerIndex].anteWager);
            }
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
                case BetType.BonusWager:
                    BetBonusWager(totalWager);
                    break;
                case BetType.AnteWager:
                    BetAnteWager(totalWager);
                    break;
                default:
                    BetFlopTurnRiver();
                    break;
            }
        }

        /// <summary>
        /// Method for players to check
        /// </summary>
        new void Check()
        {
            // finish this turn
            FinishTurn();

            // play check sound effect
            Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipCheck, AudioType.Sfx);
        }

        /// <summary>
        /// Method for players to fold cards
        /// </summary>
        new void Fold()
        {
            // finish this turn
            FinishTurn();

            // update bet data, set hand to be folded
            bets[playerIndex].hasFolded = true;

            // display player's card models face-down on the table
            tableController.playerCardsObj[playerIndex].SetActive(true);

            // play fold sound effect
            Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipFold, AudioType.Sfx);

            // hide the hand-rank panel if the player is a user player
            if (!gameManager.players[playerIndex].isNPC)
                labelController.SetLocalHandRankPanelVisibility(false);
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
        /// Methods for players to bet on ante wager, it is called from 
        /// the button click event in the Game UI
        /// </summary>
        /// <param name="value"></param>
        void BetAnteWager(int value)
        {
            // store value into bet data
            bets[playerIndex].anteWager = value;
            bets[playerIndex].EditAmountChange(-value);

            // consume player's chip
            gameManager.players[playerIndex].EditPlayerChip(-value);
            labelController.SetBetLabel(playerIndex, bets[playerIndex].GetTotal());
        }
        public void BetAnteWagerAI(int playerIndex)
        {
            // randomly select an value for ante wager ($10~$300)
            var value = chipValues[0] * UnityEngine.Random.Range(2, 60);

            // create some poker chip models to represent the bot's wager
            tableController.InstantiateWagerStack(playerIndex, 0, value);

            // repeat excatly how it functions in player's version
            this.playerIndex = playerIndex;
            BetAnteWager(value);
        }

        /// <summary>
        /// Methods for players to bet on bonus wager, it is called from
        /// the button click event in the Game UI
        /// </summary>
        /// <param name="value"></param>
        void BetBonusWager(int value)
        {
            // store value into the bet data
            bets[playerIndex].bonusWager = value;
            bets[playerIndex].EditAmountChange(-value);

            // consume player's chip
            gameManager.players[playerIndex].EditPlayerChip(-value);
            labelController.SetBetLabel(playerIndex, bets[playerIndex].GetTotal(), bets[playerIndex].bonusWager);
        }
        public void BetBonusWagerAI(int playerIndex)
        {
            // randomly select an value for ante wager ($5~$150)
            var value = chipValues[0] * UnityEngine.Random.Range(1, 30);

            // create some poker chip models to represent the bot's wager
            tableController.InstantiateWagerStack(playerIndex, 4, value);

            // repeat excatly how it functions in player's version
            this.playerIndex = playerIndex;
            BetBonusWager(value);
        }

        /// <summary>
        /// Method for the players to bet on flop/turn/river wager, it
        /// is called from the button click event in the game UI
        /// </summary>
        void BetFlopTurnRiver()
        {
            // check which bet turn it is 
            switch (betType)
            {
                case BetType.Flop:
                    bets[playerIndex].flopWager = bets[playerIndex].anteWager * 2;
                    bets[playerIndex].EditAmountChange(-bets[playerIndex].flopWager);
                    tableController.CloneAnteWagerStack(playerIndex, 1, true);
                    gameManager.players[playerIndex].EditPlayerChip(-bets[playerIndex].flopWager);
                    break;
                case BetType.Turn:
                    bets[playerIndex].turnWager = bets[playerIndex].anteWager * 1;
                    bets[playerIndex].EditAmountChange(-bets[playerIndex].turnWager);
                    tableController.CloneAnteWagerStack(playerIndex, 2, false);
                    gameManager.players[playerIndex].EditPlayerChip(-bets[playerIndex].turnWager);
                    break;
                case BetType.River:
                    bets[playerIndex].riverWager = bets[playerIndex].anteWager * 1;
                    bets[playerIndex].EditAmountChange(-bets[playerIndex].riverWager);
                    tableController.CloneAnteWagerStack(playerIndex, 3, false);
                    gameManager.players[playerIndex].EditPlayerChip(-bets[playerIndex].riverWager);
                    break;
                default:
                    break;
            }

            // update the text label to show how much money the player
            // has currently bet on this round
            labelController.SetBetLabel(playerIndex, bets[playerIndex].GetTotal(), bets[playerIndex].bonusWager);
        }
        public void BetFlopTurnRiverAI(BetType betType, int playerIndex)
        {
            // repeat excatly how it functions in player's version
            this.betType = betType;
            this.playerIndex = playerIndex;

            // get bot's current hand strength
            var handStrength = tableController.GetHandStrength(playerIndex);

            // bet if the bot has at least one pair
            if (handStrength.rank >= Rank.OnePair)
            {
                Bet();
                return;
            }

            // if the bot's hand has at least eight-high in flop, bet
            if (betType == BetType.Flop && handStrength.GetValue()>=Value.EIGHT)
            {
                Bet();
                return;
            }

            // otherwise it can be only fold or check
            if (betType == BetType.Flop)
                Fold();
            else
                Check();
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
                else if (betType == BetType.BonusWager)
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
                tableController.AddWagerModel(playerIndex, 0, value);
            else if (betType == BetType.BonusWager)
                tableController.AddWagerModel(playerIndex, 4, value);

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
                tableController.ClearWagerStackForSingleSlot(playerIndex, 0);
            }
            // if it is a bonud wager bet
            else if (betType == BetType.BonusWager)
            {
                // display skip button
                btn_skip.gameObject.SetActive(true);

                // clear all wagers in bonus wager slot
                tableController.ClearWagerStackForSingleSlot(playerIndex, 4);
            }
            
            // refresh chip's validity
            RefreshChipValidity(remainingTemp);
        }
    }
}

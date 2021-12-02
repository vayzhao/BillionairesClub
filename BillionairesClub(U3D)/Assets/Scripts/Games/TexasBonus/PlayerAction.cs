using System;
using UnityEngine;
using UnityEngine.UI;

namespace TexasBonus
{
    public class PlayerAction : BeforeBet
    {
        [Header("Decision Panel")]
        public GameObject obj_fold;
        public GameObject obj_check;
        public GameObject obj_bet;
        public GameObject decisionPanel;

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
        private int playerIndex;                // whose term is it

        public void Setup() 
        {
            // initialize bet data for all players
            bets = new Bet[gameManager.players.Length];

            // register delegate method for fold/check/bet
            methodFold = Fold;
            methodCheck = Check;
            methodBet = Bet;
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
                    DisplayWagerPanel("Bonus Wager", 
                        gameManager.players[playerIndex].chip - bets[playerIndex].anteWager * 2);
                    break;
                case BetType.AnteWager:
                    DisplayWagerPanel("Ante Wager", gameManager.players[playerIndex].chip / 3);
                    break;
                case BetType.Flop:
                case BetType.Turn:
                case BetType.River:
                    DisplayDecisionPanel();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Method to display fold/check/bet panel for a non-npc player
        /// </summary>
        void DisplayDecisionPanel()
        {
            decisionPanel.SetActive(true);

            switch (betType)
            {
                case BetType.Flop:
                    obj_fold.SetActive(true);
                    obj_check.SetActive(false);
                    break;
                case BetType.Turn:
                case BetType.River:
                    obj_check.SetActive(true);
                    obj_fold.SetActive(false);
                    obj_bet.GetComponent<Button>().Switch(gameManager.players[playerIndex].chip >= bets[playerIndex].anteWager);
                    break;
                default:
                    break;
            }
        }

        new void Bet()
        {
            // finish this turn
            FinishTurn();

            switch (betType)
            {
                case BetType.BonusWager:
                    break;
                case BetType.AnteWager:
                    BetAnteWager(totalBet);
                    break;
                case BetType.Flop:
                    break;
                case BetType.Turn:
                    break;
                case BetType.River:
                    break;
                default:
                    break;
            }
        }

        new void Check()
        {
            // finish this turn
            FinishTurn();
        }

        new void Fold()
        {
            // finish this turn
            FinishTurn();
        }

        void BetAnteWager(int value)
        {
            // store value into bet data
            bets[playerIndex].anteWager = value;
            bets[playerIndex].EditAmountChange(-value);

            // create some poker chip models to represent the player's wager
            // and also update the text label to show how much money the player
            // has bet on this round
            tableController.CreateWagerModel(playerIndex, 0, value);
            labelController.SetBetLabel(playerIndex, bets[playerIndex].GetTotal());

            // consume player's chip
            gameManager.players[playerIndex].EditPlayerChip(-value);
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
        }

        ///// <summary>
        ///// Method for the player to bet on bonus wager, it is called
        ///// from the button click event in the game UI
        ///// </summary>
        ///// <param name="value">bonus wager amount</param>
        //public void BetBonusWager(int value)
        //{
        //    // switch waiting state to be false and hide the decision panel
        //    isWaiting = false;
        //    wagerPanel.SetActive(false);

        //    // store value into bet data
        //    bets[playerIndex].bonusWager = value;
        //    bets[playerIndex].EditAmountChange(-value);

        //    // create some poker chip models to repersent the player's wager
        //    tableController.CreateWagerModel(playerIndex, 4, value);

        //    // consume player's chip 
        //    gameManager.players[playerIndex].EditPlayerChip(-value);
        //    labelController.SetBetLabel(playerIndex, bets[playerIndex].GetTotal(), bets[playerIndex].bonusWager);
        //}
        //public void BetBonusWager_AI(int playerIndex)
        //{
        //    // randomly select an value for bonus wager
        //    var value = chipValues[UnityEngine.Random.Range(0, chipValues.Length)];

        //    // repeat excatly how it functions in player's version
        //    this.playerIndex = playerIndex;
        //    BetBonusWager(value);
        //}

        /// <summary>
        /// Method for the player to bet on ante wager, it is called
        /// from the button click event in the game UI
        /// </summary>
        /// <param name="value">ante wager amount</param>
        //public void BetAnteWager(int value)
        //{
        //     switch waiting state to be false and hide the decision panel
        //    isWaiting = false;
        //    wagerPanel.SetActive(false);

        //     store value into bet data
        //    bets[playerIndex].anteWager = value;
        //    bets[playerIndex].EditAmountChange(-value);

        //     create some poker chip models to repersent the player's wager
        //     and also update the text label to show how much money the player
        //     has currently bet on this round
        //    tableController.CreateWagerModel(playerIndex, 0, value);
        //    labelController.SetBetLabel(playerIndex, bets[playerIndex].GetTotal());

        //     consume player's chip 
        //    gameManager.players[playerIndex].EditPlayerChip(-value);
        //}
        //public void BetAnteWager_AI(int playerIndex)
        //{
        //     randomly select an value for ante wager
        //    var value = chipValues[UnityEngine.Random.Range(0, chipValues.Length)];

        //     repeat excatly how it functions in player's version
        //    this.playerIndex = playerIndex;
        //    BetAnteWager(value);
        //}

        ///// <summary>
        ///// Method for the player to bet on flop/turn/river wager, it
        ///// is called from the button click event in the game UI
        ///// </summary>
        ///// <param name="value">wager amount</param>
        //public void Bet2()
        //{
        //    // switch waiting state to be false and hide the decision panel
        //    isWaiting = false;
        //    decisionPanel.SetActive(false);

        //    // check which type of bet it is 
        //    switch (betType)
        //    {
        //        case BetType.Flop:
        //            bets[playerIndex].flopWager = bets[playerIndex].anteWager * 2;
        //            bets[playerIndex].EditAmountChange(-bets[playerIndex].flopWager);
        //            tableController.CreateWagerModel(playerIndex, 1, bets[playerIndex].anteWager * 2);
        //            gameManager.players[playerIndex].EditPlayerChip(bets[playerIndex].anteWager * -2);
        //            break;
        //        case BetType.Turn:
        //            bets[playerIndex].turnWager = bets[playerIndex].anteWager * 1;
        //            bets[playerIndex].EditAmountChange(-bets[playerIndex].turnWager);
        //            tableController.CreateWagerModel(playerIndex, 2, bets[playerIndex].anteWager * 1);
        //            gameManager.players[playerIndex].EditPlayerChip(bets[playerIndex].anteWager * -1);
        //            break;
        //        case BetType.River:
        //            bets[playerIndex].riverWager = bets[playerIndex].anteWager * 1;
        //            bets[playerIndex].EditAmountChange(-bets[playerIndex].riverWager);
        //            tableController.CreateWagerModel(playerIndex, 3, bets[playerIndex].anteWager * 1);
        //            gameManager.players[playerIndex].EditPlayerChip(bets[playerIndex].anteWager * -1);
        //            break;
        //        default:
        //            break;
        //    }

        //    // update the text label to show how much money the player
        //    // has currently bet on this round
        //    labelController.SetBetLabel(playerIndex, bets[playerIndex].GetTotal(), bets[playerIndex].bonusWager);
        //}
        //public void Bet_AI(BetType betType, int playerIndex)
        //{
        //    // repeat excatly how it functions in player's version
        //    this.betType = betType;
        //    this.playerIndex = playerIndex;

        //    // get bot's current hand strength
        //    var handStrength = tableController.GetHandStrength(playerIndex);

        //    // bet if the bot has at least one pair
        //    if (handStrength.rank >= Rank.OnePair)
        //    {
        //        Bet();
        //        return;
        //    }

        //    // if the bot has a value of at least eight in flop, also bet
        //    if (betType == BetType.Flop && handStrength.GetValue() >= Value.EIGHT) 
        //    {
        //        Bet();
        //        return;
        //    }

        //    // otherwise it can be only fold or check
        //    if (betType == BetType.Flop) Fold(); else Check();
        //}

        ///// <summary>
        ///// Method for a player to fold the cards
        ///// </summary>
        //public new void Fold()
        //{
        //    // switch waiting state to be false and hide the decision panel
        //    isWaiting = false;
        //    wagerPanel.SetActive(false);
        //    decisionPanel.SetActive(false);

        //    // update bet data, set hand to be folded
        //    bets[playerIndex].hasFolded = true;

        //    // display the player's card models face-down on the table 
        //    tableController.playerCardsObj[playerIndex].SetActive(true);

        //    // play fold sound effect
        //    Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipFold, AudioType.Sfx);

        //    // hide the hand-rank panel if the player is a user player
        //    if (!gameManager.players[playerIndex].isNPC)
        //        labelController.SetLocalHandRankPanelVisibility(false);
        //}

        ///// <summary>
        ///// Method for a player to check
        ///// </summary>
        //public new void Check()
        //{
        //    // switch waiting state to be false and hide the decision panel
        //    isWaiting = false;
        //    decisionPanel.SetActive(false);

        //    // play check sound effect
        //    Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipCheck, AudioType.Sfx);
        //}
    }
}

using System;
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

        [Header("Decision Buttons")]
        [Tooltip("Buttons for betting ante wager")]
        public Button[] anteWagerBtns;
        [Tooltip("Button for betting bonus wager")]
        public Button[] bonusWagerBtns;
        [Tooltip("Button for betting in bet or check panel")]
        public Button betBtn;

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
        [HideInInspector]
        public BeforeBet beforeBet;             // the drag and drop betting component

        private BetType betType;                // used in bet method, to determine whether it is flop bet, turn bet or river bet
        private int playerIndex;                // whose term is it
        private int[] anteWagerOptions;         // optional betting values of ante wager 
        private int[] bonusWagerOptions;        // optional betting values of bonus wager
        private int[] wagerOptions;             // values for all chips

        private void Start()
        {
            beforeBet = GetComponent<BeforeBet>();
            beforeBet.methodFold = TestFold;
            beforeBet.methodCheck = TestCheck;
            beforeBet.methodBet = TestBet;
        }

        public void Setup() 
        {
            bets = new Bet[gameManager.players.Length];
            anteWagerOptions = new int[4] { 15, 30, 45, 60 };
            bonusWagerOptions = new int[5] { 5, 10, 25, 50, 100 };
            wagerOptions = new int[5] { 5, 10, 25, 50, 100 };

            
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
                    DisplayBonusWagerPanel(playerIndex);
                    break;
                case BetType.AnteWager:
                    DisplayAnteWagerPanel(playerIndex);
                    break;
                case BetType.Flop:
                    group_betOrFold.SetActive(true);
                    break;
                case BetType.Turn:
                case BetType.River:
                    DisplayBetOrCheck(playerIndex);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Method to display betting panel for ante wager, will disable options
        /// that have value greater than 2/3 of player's remaining poker chip
        /// </summary>
        /// <param name="playerIndex"></param>
        private void DisplayAnteWagerPanel(int playerIndex)
        {
            // run through each ante wager option
            // the player cannot bet more than 1/3 of his total poker chip
            // because the player will be required to bet 2 times of the ante wager in FLOP
            for (int i = 0; i < anteWagerOptions.Length; i++)
            {
                // the player cannot bet more than 1/3 of his total poker chip
                var validity = gameManager.players[playerIndex].chip >= anteWagerOptions[i] * 3;

                // modify button state and cross sprite visibility
                anteWagerBtns[i].enabled = validity;
                anteWagerBtns[i].transform.GetChild(0).GetComponent<Image>().enabled = !validity;
            }

            // finally display the decision panel
            group_anteWager.SetActive(true);
        }

        /// <summary>
        /// Method to display betting panel for bonus wager, will disable options
        /// that have value greater than (player's remaning poker chip - anteWager * 2)
        /// </summary>
        /// <param name="playerIndex"></param>
        private void DisplayBonusWagerPanel(int playerIndex)
        {
            // run through each bonus wager option
            // the player cannot bet more than (player's remaning poker chip - anteWager * 2)
            // because the player will be required to bet 2 times of the ante wager in FLOP
            for (int i = 0; i < bonusWagerOptions.Length; i++)
            {
                // the player cannot bet more than(player's remaning poker chip - anteWager * 2)
                var validity = (gameManager.players[playerIndex].chip - bets[playerIndex].anteWager * 2) >= bonusWagerOptions[i];

                // modify button state and cross sprite visibility
                bonusWagerBtns[i].enabled = validity;
                bonusWagerBtns[i].transform.GetChild(0).GetComponent<Image>().enabled = !validity;
            }

            // finally display the decision panel
            group_bonusWager.SetActive(true);
        }

        /// <summary>
        /// Method to display betting panel for TURN and RIVER, will disable bet button when
        /// the player's remaning poker chip is less than ante wager
        /// </summary>
        /// <param name="playerIndex"></param>
        private void DisplayBetOrCheck(int playerIndex)
        {
            // the player cannon bet if the remaning chip is less than ante wager
            var validity = gameManager.players[playerIndex].chip >= bets[playerIndex].anteWager;

            // modify button state 
            betBtn.Switch(validity);

            // finally display the decision panel
            group_betOrCheck.SetActive(true);
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
            bets[playerIndex].EditAmountChange(-value);

            // create some poker chip models to repersent the player's wager
            tableController.CreateWagerModel(playerIndex, 4, value);

            // consume player's chip 
            gameManager.players[playerIndex].EditPlayerChip(-value);
            labelController.SetBetLabel(playerIndex, bets[playerIndex].GetTotal(), bets[playerIndex].bonusWager);
        }
        public void BetBonusWager_AI(int playerIndex)
        {
            // randomly select an value for bonus wager
            var value = bonusWagerOptions[UnityEngine.Random.Range(0, bonusWagerOptions.Length)];

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
            bets[playerIndex].EditAmountChange(-value);

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
            // randomly select an value for ante wager
            var value = anteWagerOptions[UnityEngine.Random.Range(0, anteWagerOptions.Length)];

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
                    bets[playerIndex].EditAmountChange(-bets[playerIndex].flopWager);
                    tableController.CreateWagerModel(playerIndex, 1, bets[playerIndex].anteWager * 2);
                    gameManager.players[playerIndex].EditPlayerChip(bets[playerIndex].anteWager * -2);
                    break;
                case BetType.Turn:
                    bets[playerIndex].turnWager = bets[playerIndex].anteWager * 1;
                    bets[playerIndex].EditAmountChange(-bets[playerIndex].turnWager);
                    tableController.CreateWagerModel(playerIndex, 2, bets[playerIndex].anteWager * 1);
                    gameManager.players[playerIndex].EditPlayerChip(bets[playerIndex].anteWager * -1);
                    break;
                case BetType.River:
                    bets[playerIndex].riverWager = bets[playerIndex].anteWager * 1;
                    bets[playerIndex].EditAmountChange(-bets[playerIndex].riverWager);
                    tableController.CreateWagerModel(playerIndex, 3, bets[playerIndex].anteWager * 1);
                    gameManager.players[playerIndex].EditPlayerChip(bets[playerIndex].anteWager * -1);
                    break;
                default:
                    break;
            }

            // update the text label to show how much money the player
            // has currently bet on this round
            labelController.SetBetLabel(playerIndex, bets[playerIndex].GetTotal(), bets[playerIndex].bonusWager);
        }
        public void Bet_AI(BetType betType, int playerIndex)
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

            // if the bot has a value of at least eight in flop, also bet
            if (betType == BetType.Flop && handStrength.GetValue() >= Value.EIGHT) 
            {
                Bet();
                return;
            }

            // otherwise it can be only fold or check
            if (betType == BetType.Flop) Fold(); else Check();
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

            // play fold sound effect
            Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipFold, AudioType.Sfx);

            // hide the hand-rank panel if the player is a user player
            if (!gameManager.players[playerIndex].isNPC)
                labelController.SetLocalHandRankPanelVisibility(false);
        }
        void TestBet()
        {
            Debug.Log("TestBet");
        }
        void TestFold()
        {
            Debug.Log("TestFold");
        }
        void TestCheck()
        {
            Debug.Log("TestCheck");
        }

        /// <summary>
        /// Method for a player to check
        /// </summary>
        public void Check()
        {
            // switch waiting state to be false and hide the decision panel
            isWaiting = false;
            group_betOrCheck.SetActive(false);

            // play check sound effect
            Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipCheck, AudioType.Sfx);
        }
    }
}

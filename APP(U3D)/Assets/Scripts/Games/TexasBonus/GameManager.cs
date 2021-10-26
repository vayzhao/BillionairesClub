using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TexasBonus
{
    public class GameManager : MonoBehaviour
    {
        [Header("Prefab objects")]
        [Tooltip("A prefab object that holds the player action script")]
        public GameObject pref_PlayerAction;
        [Tooltip("A prefab object that holds the UI script")]
        public GameObject pref_labelController;
        [Tooltip("A prefab object that holds the ready button")]
        public GameObject pref_readyBtn;
        [Tooltip("A prefab object that holds the UI manager")]
        public GameObject pref_uiManager;

        [HideInInspector]
        public Player[] players;                  // data for all players

        private GameObject obj_readyBtn;          // a button that allows you to enter the game
        private TableInformation tableInfo;       // a script that handles information of the players 
        private TableController tableController;  // a script that handles all the 3D models used on the table 
        private PlayerAction playerAction;        // a script that handles betting decision in the game
        private LabelController labelController;  // a script that handles all the UI-text objects in the scene
        private UIManager uiManager;              // a script that handles all interactable UI components
        /// <summary>
        /// Method to setup the game manager for texas bonus
        /// </summary>
        /// <param name="canvas"></param>
        public void Setup(Transform canvas) 
        {
            // create game object for player action
            playerAction = Instantiate(pref_PlayerAction, canvas).GetComponent<PlayerAction>();

            // find core components for table
            tableInfo = FindObjectOfType<TableInformation>();
            tableController = FindObjectOfType<TableController>();

            // create game object for player hand
            labelController = Instantiate(pref_labelController, canvas).GetComponent<LabelController>();
            labelController.Setup();
            tableController.labelController = labelController;

            // bind relative script to each other
            playerAction.gameManager = this;
            playerAction.labelController = labelController;
            playerAction.tableController = tableController;
            tableController.gameManager = this;
            tableController.playerAction = playerAction;

            // create ready button in hidden form
            obj_readyBtn = Instantiate(pref_readyBtn, canvas);
            obj_readyBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => GameStart());
            obj_readyBtn.SetActive(false);

            // create the UI manager and register event for stand up button
            uiManager = Instantiate(pref_uiManager, canvas).GetComponent<UIManager>();
        }

        /// <summary>
        /// Method to initialize all components and get ready to start the game
        /// </summary>
        public void FinishedLoading()
        {
            // access player data
            players = tableInfo.players;

            // method to run start method for playerAction & tableController
            playerAction.Setup();
            tableController.Setup();

            // display ready button
            obj_readyBtn.SetActive(true);
        }

        /// <summary>
        /// Method to start the game officially
        /// </summary>
        void GameStart()
        {
            // destroy the ready button
            Destroy(obj_readyBtn.gameObject);

            // display labels on the table
            tableController.DisplayTableLabel();

            // enable the intractable UI components
            uiManager.SetInitButtonVisbility(true);

            // start the game loop coroutine
            StartCoroutine(Round());
        }

        /// <summary>
        /// Core Game Loop function
        /// An infinite whileloop to keep the game going
        /// </summary>
        /// <returns></returns>
        IEnumerator Round()
        {
            // initialize round number
            var round = 0;
            while (true)
            {
                // increment round
                round++;

                // reset the game
                ResetGame();

                // deal 2 dealers card and 5 community card (in face-down)
                yield return tableController.DealInitialCards();          
                
                // ask for the bonus wager
                yield return BonusWagerBet();

                // deal 2 cards for each player (in face-down)
                yield return tableController.DealPlayerCards();

                // ask for the ante wager and reveal player's start hand
                yield return AnteWagerBet();
                tableController.RevealPlayerCards();

                // ask for decision on flop bet
                yield return FlopBet();

                // take away folded player's card and chip
                yield return tableController.DetermineFoldedPlayer();

                // skip this game if all player has folded
                if (tableController.allPlayerFolded)
                    continue;

                // otherwise reveal the flop cards
                yield return tableController.RevealCommunityCard(0, false);
                yield return tableController.RevealCommunityCard(1, false);
                yield return tableController.RevealCommunityCard(2, true);

                // ask for decision on turn bet and reveal the turn card
                yield return TurnBet();               
                yield return tableController.RevealCommunityCard(3, true);

                // ask for decision on river bet and reveal the river card
                yield return RiverBet();
                yield return tableController.RevealCommunityCard(4, true);

                // reveal dealers hand and compute its hand-rank
                yield return tableController.RevealDealerHand();

                // compare player's hand-rank and dealer's hand-rank
                yield return Comparing();                
            }
        }

        /// <summary>
        /// Method to reset the game, it is called everytime when a new 
        /// round starts
        /// </summary>
        void ResetGame()
        {
            // reset player action's data
            playerAction.ResetBet();

            // reset table controller's data
            tableController.ResetTable();
        }

        /// <summary>
        /// Method to scan through player array and ask for bonus wager
        /// </summary>
        /// <returns></returns>
        IEnumerator BonusWagerBet()
        {
            // initialize checkIndex
            var checkIndex = -1;
            while (checkIndex < players.Length - 1)
            {
                // increment checkIndex
                checkIndex++;

                // skip this iteration if the 'n' player is empty
                if (players[checkIndex] == null)
                    continue;

                // bet automatically if the 'n' player is a NPC
                if (players[checkIndex].isNPC)
                {
                    playerAction.BetBonusWager_AI(checkIndex);
                    yield return new WaitForSeconds(Const.WAIT_TIME_DECISION);
                    continue;
                }

                // otherwise, pop up a decision window to the player
                playerAction.DisplayBetPanel(BetType.BonusWager, checkIndex);
                while (playerAction.isWaiting)
                    yield return new WaitForSeconds(Const.WAIT_TIME_DECISION);
            }
        }

        /// <summary>
        /// Method to scan through player array and ask for ante wager
        /// </summary>
        /// <returns></returns>
        IEnumerator AnteWagerBet()
        {
            var checkIndex = -1;
            while (checkIndex < players.Length - 1)
            {
                // increment checkIndex
                checkIndex++;

                // skip this iteration if the 'n' player is empty
                if (players[checkIndex] == null)
                    continue;

                // bet automatically if the 'n' player is a NPC
                if (players[checkIndex].isNPC)
                {
                    playerAction.BetAnteWager_AI(checkIndex);
                    yield return new WaitForSeconds(Const.WAIT_TIME_DECISION);
                    continue;
                }

                // otherwise, pop up a decision window to the player
                playerAction.DisplayBetPanel(BetType.AnteWager, checkIndex);
                while (playerAction.isWaiting)
                    yield return new WaitForSeconds(Const.WAIT_TIME_DECISION);
            }
        }

        /// <summary>
        /// Method to scan through player array and ask for flop wager
        /// </summary>
        /// <returns></returns>
        IEnumerator FlopBet()
        {
            var checkIndex = -1;
            while (checkIndex < players.Length - 1)
            {
                // increment checkIndex
                checkIndex++;

                // skip this iteration if the 'n' player is empty
                if (players[checkIndex] == null)
                    continue;

                // bet automatically if the 'n' player is a NPC
                if (players[checkIndex].isNPC)
                {
                    playerAction.Bet_AI(BetType.Flop, checkIndex);
                    yield return new WaitForSeconds(Const.WAIT_TIME_DECISION);
                    continue;
                }

                // otherwise, pop up a decision window to the player
                playerAction.DisplayBetPanel(BetType.Flop, checkIndex);
                while (playerAction.isWaiting)
                    yield return new WaitForSeconds(Const.WAIT_TIME_DECISION);
            }
        }

        /// <summary>
        /// Method to scan through player array and ask for turn wager
        /// </summary>
        /// <returns></returns>
        IEnumerator TurnBet()
        {
            var checkIndex = -1;
            while (checkIndex < players.Length - 1)
            {
                // increment checkIndex
                checkIndex++;

                // skip this iteration if the 'n' player is empty
                if (players[checkIndex] == null || playerAction.bets[checkIndex].hasFolded)
                    continue;

                // bet automatically if the 'n' player is a NPC
                if (players[checkIndex].isNPC)
                {
                    playerAction.Bet_AI(BetType.Turn, checkIndex);
                    yield return new WaitForSeconds(Const.WAIT_TIME_DECISION);
                    continue;
                }

                // otherwise, pop up a decision window to the player
                playerAction.DisplayBetPanel(BetType.Turn, checkIndex);
                while (playerAction.isWaiting)
                    yield return new WaitForSeconds(Const.WAIT_TIME_DECISION);
            }
        }

        /// <summary>
        /// Method to scan through player array and ask for river wager
        /// </summary>
        /// <returns></returns>
        IEnumerator RiverBet()
        {
            var checkIndex = -1;
            while (checkIndex < players.Length - 1)
            {
                // increment checkIndex
                checkIndex++;

                // skip this iteration if the 'n' player is empty
                if (players[checkIndex] == null || playerAction.bets[checkIndex].hasFolded)
                    continue;

                // bet automatically if the 'n' player is a NPC
                if (players[checkIndex].isNPC)
                {
                    playerAction.Bet_AI(BetType.River, checkIndex);
                    yield return new WaitForSeconds(Const.WAIT_TIME_DECISION);
                    continue;
                }

                // otherwise, pop up a decision window to the player
                playerAction.DisplayBetPanel(BetType.River, checkIndex);
                while (playerAction.isWaiting)
                    yield return new WaitForSeconds(Const.WAIT_TIME_DECISION);
            }
        }

        /// <summary>
        /// Method to scan through player array and compare hand-rank between the player
        /// and the dealer
        /// </summary>
        /// <returns></returns>
        IEnumerator Comparing()
        {
            var checkIndex = -1;
            while (checkIndex < players.Length - 1)
            {
                // increment checkIndex
                checkIndex++;

                // hide everything for the previous compared player
                tableController.HidePlayerCards(checkIndex - 1);

                // skip this iteration if the 'n' player is empty or folded
                if (players[checkIndex] == null || playerAction.bets[checkIndex].hasFolded)
                    continue;

                // compare player's hand strength
                tableController.BonusReward(checkIndex);
                tableController.Compare(checkIndex);                
                yield return new WaitForSeconds(Const.WAIT_TIME_COMPARE);
            }

            // hide everything from the last compared player
            tableController.HidePlayerCards(checkIndex);
        }
    }
}

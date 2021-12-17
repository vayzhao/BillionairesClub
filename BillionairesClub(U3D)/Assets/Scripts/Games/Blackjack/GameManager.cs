using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blackjack
{
    public class GameManager : MonoBehaviour
    {
        public const int CARD_DECK_COUNT = 8;
        public const int WAGER_INDEX_ANTE = 0;
        public const int WAGER_INDEX_BONUS_SPLITE_WAGER = 1;
        public const int WAGER_INDEX_DOUBLE = 2;
        public const int WAGER_INDEX_SPLIT_DOUBLE = 3;
        public const int WAGER_INDEX_INSURANCE = 4;

        [HideInInspector]
        public Player[] players;                 // data for all players
                
        private UIManager uiManager;             // a script that handles all the UI components
        private PlayerAction playerAction;       // a script that handles betting decision in the game
        private TableInformation tableInfo;      // a script that handles information of the players
        private TableController tableController; // a script that handles all the 3D models used on the table
        private LabelController labelController; // a script that handles all the UI-text objects in the scene

        /// <summary>
        /// Method to setup the game manager for blackjack
        /// </summary>
        /// <param name="canvas"></param>
        public void Setup(Transform canvas)
        {
            // find ui manager from the canvas
            uiManager = canvas.GetComponentInChildren<UIManager>();

            // create game object for player action
            playerAction = uiManager.playerAction.GetComponent<PlayerAction>();

            // find table information
            tableInfo = FindObjectOfType<TableInformation>();
            tableController = FindObjectOfType<TableController>();

            // create UI object to display local player's hand
            labelController = uiManager.labelController.GetComponent<LabelController>();
            labelController.cardDeckLabel.Switch(false);
            labelController.Reset();

            // bind relative script to each other
            tableController.gameManager = this;
            tableController.playerAction = playerAction;
            tableController.labelController = labelController;
            playerAction.gameManager = this;
            playerAction.labelController = labelController;
            playerAction.tableController = tableController;

            // register clicking event for ready button
            uiManager.readyBtn.onClick.AddListener(() => GameStart());

            // set player action & ready button as interactable object
            uiManager.interactable.Add(playerAction.gameObject);
            uiManager.interactable.Add(uiManager.readyBtn.gameObject);
        }

        /// <summary>
        /// Method to initialize all components and get ready to start the game
        /// </summary>
        public void FinishedLoading()
        {
            // access player data
            players = tableInfo.players;

            // method to run start method for playerAction & tableController
            playerAction.SetUp();
            tableController.Setup();
        }

        /// <summary>
        /// Method to start the game officially
        /// </summary>
        void GameStart()
        {
            // remove ready button from the interactable object list and destroy it
            uiManager.interactable.Remove(uiManager.readyBtn.gameObject);
            Destroy(uiManager.readyBtn.gameObject);

            // display labels on the table
            tableController.DisplayTableLabel();

            // start the game loop coroutine
            StartCoroutine(Round());
        }
        
        /// <summary>
        /// Core Game Loop Function
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

                // ask for the ante wager and perfect pair wager
                yield return AnteWagerBet();
                yield return PerfectPairBet();

                // deal first two cards to players and dealer
                yield return tableController.DealInitialCards();

                // ask for the insurance wager if the insurance trigger is on
                if (tableController.insuranceTriggered)
                    yield return InsuranceBet();

                // apply perfect pair reward / loss
                yield return tableController.CheckPerfectPair();

                // go through players and ask for decisions
                yield return MakeDecision();

                // TODO: Reveal dealer's second card and give insurance reward

                // TODO: dealer's hit

                // TODO: compare results

                break;
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
                    // TODO: call AI betting method
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
        /// Method to scan through player array and ask for perfect pair wager
        /// </summary>
        /// <returns></returns>
        IEnumerator PerfectPairBet()
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
                    // TODO: call AI betting method
                    yield return new WaitForSeconds(Const.WAIT_TIME_DECISION);
                    continue;
                }

                // otherwise, pop up a decision window to the player
                playerAction.DisplayBetPanel(BetType.PerfectPair, checkIndex);
                while (playerAction.isWaiting)
                    yield return new WaitForSeconds(Const.WAIT_TIME_DECISION);
            }
        }

        /// <summary>
        /// Method to scan through player array and ask for insurance wager
        /// </summary>
        /// <returns></returns>
        IEnumerator InsuranceBet()
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
                    // TODO: call AI betting method
                    yield return new WaitForSeconds(Const.WAIT_TIME_DECISION);
                    continue;
                }

                // otherwise, pop up a decision window to the player
                playerAction.DisplayBetPanel(BetType.Insurance, checkIndex);
                while (playerAction.isWaiting)
                    yield return new WaitForSeconds(Const.WAIT_TIME_DECISION);
            }
        }

        /// <summary>
        /// Method to scan through player array and ask for decision, decisions
        /// include double down, hit, stand and split
        /// </summary>
        /// <returns></returns>
        IEnumerator MakeDecision()
        {
            var checkIndex = -1;
            while (checkIndex < players.Length - 1)
            {
                // increment checkIndex
                checkIndex++;

                // skip this iteration if the 'n' player is empty
                if (players[checkIndex] == null)
                    continue;

                // making decision for AI players
                if (players[checkIndex].isNPC)
                {
                    // TODO: call AI betting method
                    yield return new WaitForSeconds(Const.WAIT_TIME_DECISION);
                    continue;
                }

                // otherwise, pop up a decision window to the player
                yield return playerAction.Deciding(checkIndex);
                while (playerAction.isWaiting)
                    yield return new WaitForSeconds(Const.WAIT_TIME_DECISION);
            }
        }
    }
}


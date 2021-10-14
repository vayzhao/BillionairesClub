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

        [HideInInspector]
        public Player[] players;

        private GameObject obj_readyBtn;          // a button that allows you to enter the game
        private TableInformation tableInfo;       // a script that handles information of the players 
        private TableController tableController;  // a script that handles all the 3D models used on the table 
        private PlayerAction playerAction;        // a script that handles betting decision in the game
        private LabelController labelController;  // a script that handles all the UI-text objects in the scene

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

            // start a round
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

                playerAction.ResetBet();
                tableController.Shuffle();
                tableController.HidePlayerHand();
                tableController.TurnCardsBack();
                yield return tableController.DealInitialCards();                
                yield return BonusWagerBet();
                yield return tableController.DealPlayerCards();
                yield return AnteWagerBet();
                tableController.RevealPlayerCards();
                yield return FlopBet();
                yield return tableController.DetermineFoldedPlayer();

                if (tableController.allPlayerFolded)
                    continue;
                                    
                yield return tableController.RevealCommunityCard(0, false);
                yield return tableController.RevealCommunityCard(1, false);
                yield return tableController.RevealCommunityCard(2, true);
                yield return TurnBet();
                yield return tableController.RevealCommunityCard(3, true);
                yield return RiverBet();
                yield return tableController.RevealCommunityCard(4, true);
                yield return tableController.RevealDealerHand(0);
                yield return tableController.RevealDealerHand(1);
                tableController.ComputeDealerHandStrength();
                yield return Comparing();                
            }
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
                if (players[checkIndex] == null) {
                    //Debug.Log($"Player{checkIndex} is empty");                    
                    yield return new WaitForSeconds(Blackboard.WAIT_TIME_EMPTY);
                    continue;
                }

                if (players[checkIndex].isNPC)
                {
                    //Debug.Log($"Player{checkIndex} has made a decision");
                    playerAction.BetBonusWager_AI(checkIndex);
                    yield return new WaitForSeconds(Blackboard.WAIT_TIME_DECISION);
                    continue;
                }

                playerAction.DisplayBetPanel(BetType.BonusWager, checkIndex);
                while (playerAction.isWaiting)
                {
                    //Debug.Log("Wait for player to make decision");
                    yield return new WaitForSeconds(Blackboard.WAIT_TIME_DECISION);
                }
            }

            //Debug.Log("BonusWager");
            yield return new WaitForSeconds(Time.deltaTime);
        }

        IEnumerator AnteWagerBet()
        {
            var checkIndex = -1;
            while (checkIndex < players.Length - 1)
            {
                // increment checkIndex
                checkIndex++;

                // skip this iteration if the 'n' player is empty
                if (players[checkIndex] == null)
                {
                    //Debug.Log($"Player{checkIndex} is empty");
                    yield return new WaitForSeconds(Blackboard.WAIT_TIME_EMPTY);
                    continue;
                }

                if (players[checkIndex].isNPC)
                {
                    //Debug.Log($"Player{checkIndex} has made a decision");
                    playerAction.BetAnteWager_AI(checkIndex);
                    yield return new WaitForSeconds(Blackboard.WAIT_TIME_DECISION);
                    continue;
                }

                playerAction.DisplayBetPanel(BetType.AnteWager, checkIndex);
                while (playerAction.isWaiting)
                {
                    //Debug.Log("Wait for player to make decision");
                    yield return new WaitForSeconds(Blackboard.WAIT_TIME_DECISION);
                }
            }

            //Debug.Log("AnteWager");
            yield return new WaitForSeconds(Time.deltaTime);
        }

        IEnumerator FlopBet()
        {
            var checkIndex = -1;
            while (checkIndex < players.Length - 1)
            {
                // increment checkIndex
                checkIndex++;

                // skip this iteration if the 'n' player is empty
                if (players[checkIndex] == null)
                {
                    //Debug.Log($"Player{checkIndex} is empty");
                    yield return new WaitForSeconds(Blackboard.WAIT_TIME_EMPTY);
                    continue;
                }

                if (players[checkIndex].isNPC)
                {
                    //Debug.Log($"Player{checkIndex} has made a decision");
                    playerAction.Bet_AI(BetType.Flop, checkIndex);
                    //playerAction.Fold_AI(checkIndex);
                    yield return new WaitForSeconds(Blackboard.WAIT_TIME_DECISION);
                    continue;
                }

                playerAction.DisplayBetPanel(BetType.Flop, checkIndex);
                while (playerAction.isWaiting)
                {
                    //Debug.Log("Wait for player to make decision");
                    yield return new WaitForSeconds(Blackboard.WAIT_TIME_DECISION);
                }
            }

            //Debug.Log("Flop");
            yield return new WaitForSeconds(Time.deltaTime);
        }

        IEnumerator TurnBet()
        {
            var checkIndex = -1;
            while (checkIndex < players.Length - 1)
            {
                // increment checkIndex
                checkIndex++;

                // skip this iteration if the 'n' player is empty
                if (players[checkIndex] == null || playerAction.PlayerHasFolded(checkIndex))
                {
                    //Debug.Log($"Player{checkIndex} is empty");
                    yield return new WaitForSeconds(Blackboard.WAIT_TIME_EMPTY);
                    continue;
                }

                if (players[checkIndex].isNPC)
                {
                    //Debug.Log($"Player{checkIndex} has made a decision");
                    playerAction.Bet_AI(BetType.Turn, checkIndex);
                    yield return new WaitForSeconds(Blackboard.WAIT_TIME_DECISION);
                    continue;
                }

                playerAction.DisplayBetPanel(BetType.Turn, checkIndex);
                while (playerAction.isWaiting)
                {
                    //Debug.Log("Wait for player to make decision");
                    yield return new WaitForSeconds(Blackboard.WAIT_TIME_DECISION);
                }
            }

            //Debug.Log("Turn");
            yield return new WaitForSeconds(Time.deltaTime);
        }

        IEnumerator RiverBet()
        {
            var checkIndex = -1;
            while (checkIndex < players.Length - 1)
            {
                // increment checkIndex
                checkIndex++;

                // skip this iteration if the 'n' player is empty
                if (players[checkIndex] == null || playerAction.PlayerHasFolded(checkIndex))
                {
                    //Debug.Log($"Player{checkIndex} is empty");
                    yield return new WaitForSeconds(Blackboard.WAIT_TIME_EMPTY);
                    continue;
                }

                if (players[checkIndex].isNPC)
                {
                    //Debug.Log($"Player{checkIndex} has made a decision");
                    playerAction.Bet_AI(BetType.River, checkIndex);
                    yield return new WaitForSeconds(Blackboard.WAIT_TIME_DECISION);
                    continue;
                }

                playerAction.DisplayBetPanel(BetType.River, checkIndex);
                while (playerAction.isWaiting)
                {
                    //Debug.Log("Wait for player to make decision");
                    yield return new WaitForSeconds(Blackboard.WAIT_TIME_DECISION);
                }
            }
            //Debug.Log("River");
            yield return new WaitForSeconds(Time.deltaTime);
        }

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
                {
                    //Debug.Log($"Player{checkIndex} is empty");
                    yield return new WaitForSeconds(Blackboard.WAIT_TIME_EMPTY);
                    continue;
                }

                // compare player's hand strength
                tableController.BonusReward(checkIndex);
                tableController.Compare(checkIndex);                
                yield return new WaitForSeconds(Blackboard.WAIT_TIME_COMPARE);
            }

            // hide everything from the last compared player
            tableController.HidePlayerCards(checkIndex);

            //Debug.Log("Compare");
        }























    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Blackboard;
using static Const;

namespace Blackjack
{
    public class TableController : WagerBehaviour
    {
        [Header("Player's asset")]
        [Tooltip("A circle sprite around the wager stack")]
        public GameObject[] marks;
        [Tooltip("Player's poker chip")]
        public GameObject[] chips;

        [Header("Poker Models")]
        [Tooltip("Models represent dealer's card")]
        public GameObject[] dealerCardsObj;      
        [Tooltip("Models represent player's card")]
        public GameObject[] playerCardGroups;

        [HideInInspector]
        public bool insuranceTriggered;          // whether or not the dealer's first card is an ace
        [HideInInspector]
        public GameManager gameManager;          // the game manager script
        [HideInInspector]       
        public PlayerAction playerAction;        // the player action script
        [HideInInspector]
        public LabelController labelController;  // the label controller script
        
        private CardDeck cardDeck;               // card deck used in the game
        private Image[][] labelMarks;            // circle label around wager stacks
        private GameObject[][][] playerCardsObj; // objects to represent player's cards
        private Hand dealerHand;                 // hand evaluator for dealer
        private Hand[] playerHands;              // hand evaluator for players

        public void Setup()
        {
            InitializeCards();
            InitializeWagerPos();
        }

        /// <summary>
        /// Method to initialize the card deck and player's card objects
        /// </summary>
        void InitializeCards()
        {
            // setup card deck (notice that blackjack uses 6~8 decks of cards)
            cardDeck = new CardDeck(GameManager.CARD_DECK_COUNT);
            cardDeck.Shuffle();
            //cardDeck.DebugDeck();

            // setup hand evaluator for dealer and players
            dealerHand = new Hand();
            playerHands = new Hand[gameManager.players.Length];

            // setup player card objs
            playerCardsObj = new GameObject[gameManager.players.Length][][];
            for (int i = 0; i < playerCardsObj.Length; i++)
            {
                playerHands[i] = new Hand();
                playerCardsObj[i] = new GameObject[2][];
                for (int j = 0; j < playerCardsObj[i].Length; j++)
                {
                    playerCardsObj[i][j] = new GameObject[5];
                    for (int k = 0; k < playerCardsObj[i][j].Length; k++)
                    {
                        playerCardsObj[i][j][k] = playerCardGroups[i].transform.GetChild(j).GetChild(k).gameObject;
                    }
                }
            }
        }

        /// <summary>
        /// Method to record wager placing position
        /// </summary>
        void InitializeWagerPos()
        {
            // initialize associated variables
            wagerPos = new Vector3[gameManager.players.Length][];
            wagerStacks = new GameObject[gameManager.players.Length][];
            labelMarks = new Image[gameManager.players.Length][];

            // initialize wager positions
            for (int i = 0; i < wagerPos.Length; i++)
            {
                wagerPos[i] = new Vector3[6];
                wagerStacks[i] = new GameObject[6];
                labelMarks[i] = new Image[6];
                for (int j = 0; j < wagerPos[i].Length; j++)
                {
                    wagerPos[i][j] = wager[i].transform.GetChild(j).transform.position;
                    labelMarks[i][j] = marks[i].transform.GetChild(j).GetComponent<Image>();
                }   
            }
        }

        /// <summary>
        /// Method to reset a player's wager label mark
        /// </summary>
        /// <param name="playerId">index of the player</param>
        void ResetLabelMark(int playerId)
        {
            labelMarks[playerId][GameManager.WAGER_INDEX_DOUBLE].enabled = false;
            labelMarks[playerId][GameManager.WAGER_INDEX_SPLIT_ANTE].enabled = false;
            labelMarks[playerId][GameManager.WAGER_INDEX_SPLIT_DOUBLE].enabled = false;
            labelMarks[playerId][GameManager.WAGER_INDEX_INSURANCE].enabled = false;
        }

        /// <summary>
        /// Method to display labels on the table, containing player's asset.
        /// It is called when the start button is pressed
        /// </summary>
        public void DisplayTableLabel()
        {
            // setup card deck label
            labelController.cardDeckLabel.Display($"Remaining Card:{cardDeck.GetRemaining()}");

            // set player's asset to be visible, depends on the player's status
            for (int i = 0; i < gameManager.players.Length; i++)
            {
                // skip if the 'n' player does not exist
                if (gameManager.players[i] == null)
                    continue;

                // otherwise, display the 'n' player's poker chip and label marks
                chips[i].SetActive(true);
                marks[i].SetActive(true);
                ResetLabelMark(i);
            }
        }

        /// <summary>
        /// Method to reset the table, includes hiding card models, reseting labels
        /// and player's hand 
        /// </summary>
        public void ResetTable()
        {
            // reset insurance trigger
            insuranceTriggered = false;

            // hide dealer card objects 
            for (int i = 0; i < dealerCardsObj.Length; i++)
                dealerCardsObj[i].SetActive(false);

            // hide all player card objects
            for (int i = 0; i < playerCardsObj.Length; i++)
                for (int j = 0; j < playerCardsObj[i].Length; j++)
                    for (int k = 0; k < playerCardsObj[i][j].Length; k++)
                        playerCardsObj[i][j][k].SetActive(false);

            // reset table labels
            labelController.Reset();
        }

        /// <summary>
        /// Alternate version of draw a card, first to draw a card then check to see
        /// if the card deck has any remaining card, if it doesn't, shuffle the deck
        /// also update remaining card number in card deck label
        /// </summary>
        /// <returns></returns>
        public Card DrawACard()
        {
            // draw a card
            var card = cardDeck.DrawACard();

            // shuffle the card deck when it is running out
            if (cardDeck.GetRemaining() == 0)
                cardDeck.Shuffle();

            // update card deck label
            labelController.cardDeckLabel.tmp.text = $"Remaining Card:{cardDeck.GetRemaining()}";

            return card;
        }

        /// <summary>
        /// Method to deal first two cards for dealer and players
        /// </summary>
        /// <returns></returns>
        public IEnumerator DealInitialCards()
        {
            Card card;

            for (int i = 0; i < 2; i++)
            {
                // dealing a card to each player
                for (int j = 0; j < gameManager.players.Length; j++)
                {
                    // check to see if the player exists
                    if (gameManager.players[j] != null)
                    {
                        // draw a card
                        card = DrawACard();
                        playerHands[j].AddCard(card);
                        playerCardsObj[j][0][i].SetCard(card);
                        audioManager.PlayAudio(audioManager.clipDealCards, AudioType.Sfx);

                        // get point
                        var point = playerHands[j].GetRank();

                        // display global player hand label
                        labelController.playerHandLabel[j].Display($"{point}");

                        // display local player hand label if this player is a non-npc player
                        if (i == 0 && !gameManager.players[j].isNPC)
                            labelController.localHandLabels[0].Display("");

                        // update card sprite from local hand panel
                        if (!gameManager.players[j].isNPC)
                        {
                            labelController.RevealACard(GetCardSprite(card.GetCardIndex()), i);
                            labelController.localHandLabels[0].tmp.text = $"{playerHands[j].GetRank()}";
                        }

                        yield return new WaitForSeconds(WAIT_TIME_DEAL);
                    }
                }

                // and deal a card for dealer
                card = DrawACard();
                dealerHand.AddCard(card);
                dealerCardsObj[i].SetCard(card);
                audioManager.PlayAudio(audioManager.clipDealCards, AudioType.Sfx);

                // in the first iteration, display dealer's hand label
                // and detect insurance trigger
                if (i == 0)
                {
                    CheckInsuranceTrigger(card.value == Value.ACE);
                    labelController.dealerHandLabel.Display($"{dealerHand.GetRank()}");
                }
                // in the second iteration, make dealer's second card face-down
                else if (i == 1)
                {
                    dealerCardsObj[i].transform.localEulerAngles = new Vector3(0f, 0f, 180f);
                }

                yield return new WaitForSeconds(WAIT_TIME_DEAL);
            }            
        }

        /// <summary>
        /// Method to check insurance trigger, the flag will be switch
        /// on when the dealer's first card is an ACE
        /// </summary>
        /// <param name="flag"></param>
        void CheckInsuranceTrigger(bool flag)
        {
            // return when the flag is false
            if (!flag)
                return;

            // otherwise set insurance trigger to be true
            insuranceTriggered = true;

            // display insurance label marks on the table
            for (int i = 0; i < labelMarks.Length; i++)
                if (gameManager.players[i] != null)
                    labelMarks[i][GameManager.WAGER_INDEX_INSURANCE].enabled = true;
        }
    }
}
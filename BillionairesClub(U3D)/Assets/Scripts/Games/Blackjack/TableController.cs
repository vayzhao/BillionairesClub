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

        /// <summary>
        /// Method to access a single player's hand
        /// </summary>
        /// <param name="playerId">index of the player</param>
        /// <returns></returns>
        public Hand GetPlayerHand(int playerId) => playerHands[playerId];

        public void Setup()
        {
            InitializeCards();
            InitializeWagerPos();
            HideInitialObjects();
        }

        /// <summary>
        /// Method to initialize the card deck and player's card objects
        /// </summary>
        void InitializeCards()
        {
            // setup card deck (notice that blackjack uses 6~8 decks of cards)
            cardDeck = new CardDeck(GameManager.CARD_DECK_COUNT);
            cardDeck.Shuffle();
            //cardDeck.DebugDeck_Blackjack();

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
                wagerPos[i] = new Vector3[5];
                wagerStacks[i] = new GameObject[5];
                labelMarks[i] = new Image[5];
                for (int j = 0; j < wagerPos[i].Length; j++)
                {
                    wagerPos[i][j] = wager[i].transform.GetChild(j).transform.position;
                    labelMarks[i][j] = marks[i].transform.GetChild(j).GetComponent<Image>();
                }   
            }
        }

        /// <summary>
        /// Method to hide all players assets on the table, also dealer's cards
        /// </summary>
        void HideInitialObjects()
        {
            // hide all players asset
            for (int i = 0; i < gameManager.players.Length; i++)
            {
                chips[i].SetActive(false);
                marks[i].SetActive(false);
                wager[i].SetActive(false);
                playerCardGroups[i].SetActive(false);
            }

            // hide all dealer card objects
            for (int i = 0; i < dealerCardsObj.Length; i++)
                dealerCardsObj[i].SetActive(false);
        }

        /// <summary>
        /// Method to reset a player's wager label mark
        /// </summary>
        /// <param name="playerId">index of the player</param>
        void ResetLabelMark(int playerId)
        {
            labelMarks[playerId][GameManager.WAGER_INDEX_BONUS_SPLITE_WAGER].enabled = true;
            labelMarks[playerId][GameManager.WAGER_INDEX_DOUBLE].enabled = false;
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
                playerCardGroups[i].SetActive(true);
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

                        // initialize label text
                        var labelText = "";

                        // check to see if the player has blackjack
                        if (playerHands[j].HasBlackjack())
                        {
                            labelText = "Blackjack";
                        }
                        else
                        {
                            // otherwise, calculate player's hand rank
                            var rank = playerHands[j].GetRank();
                            var rankSoft = playerHands[j].GetRankSoft();
                            labelText = $"{rank}" + (rankSoft > 0 ? $"/{rankSoft}" : "");
                        }

                        // display global player hand label
                        labelController.playerHandLabel[j].Display(labelText);

                        // display local player hand label if this player is a non-npc player
                        if (i == 0 && !gameManager.players[j].isNPC)
                            labelController.localHandLabels[0].Display("");

                        // update card sprite from local hand panel
                        if (!gameManager.players[j].isNPC)
                        {
                            labelController.RevealACard(GetCardSprite(card.GetCardIndex()), i);
                            labelController.localHandLabels[0].tmp.text = labelText;
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
                    labelController.dealerHandLabel.Display($"{dealerHand.GetRank()}" + 
                        (dealerHand.GetRankSoft() > 0 ? $"/{dealerHand.GetRankSoft()}" : ""));
                }
                // in the second iteration, make dealer's second card face-down
                else if (i == 1)
                {
                    dealerCardsObj[i].transform.localEulerAngles = new Vector3(0f, 0f, 180f);
                }

                yield return new WaitForSeconds(WAIT_TIME_DEAL);
            }

            // check if anyone has a pair to start with
            DetectPairHand();
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

        public void OnPlayerHit(int playerIndex, int handIndex)
        {
            // draw a card
            var card = DrawACard();
            var cardIndex = playerHands[playerIndex].GetCardCount(handIndex);
            playerHands[playerIndex].AddCard(card, handIndex);
            playerCardsObj[playerIndex][handIndex][cardIndex].SetCard(card);
            audioManager.PlayAudio(audioManager.clipDealCards, AudioType.Sfx);

            // get point
            var rank = playerHands[playerIndex].GetRank();
            var rankSoft = playerHands[playerIndex].GetRankSoft();
            var labelText = $"{rank}" + (rankSoft > 0 ? $"/{rankSoft}" : "");

            // display global player hand label
            labelController.playerHandLabel[playerIndex].tmp.text = labelText;

            // update card sprite from local hand panel
            if (!gameManager.players[playerIndex].isNPC)
            {
                labelController.RevealACard(GetCardSprite(card.GetCardIndex()), cardIndex, handIndex);
                labelController.localHandLabels[handIndex].tmp.text = labelText;
            }
        }

        /// <summary>
        /// Method to pre-calculate player's perfect pair reward multipiler, players who have a pair
        /// to start with, will change his hand panel background color to be green
        /// </summary>
        void DetectPairHand()
        {
            // quickly check through all players hand, change hand panel to green color
            // if the 'n' player has a pair, also pre-calculate its reward multipier
            for (int i = 0; i < gameManager.players.Length; i++)
            {
                // skip this iteration if 'n' player does not exist
                if (gameManager.players[i] == null)
                    continue;

                // skip this iteration if 'n' player did not bet on perfect pair
                if (playerAction.bets[i].perfectPairWager == 0)
                    continue;

                // if the 'n' player has a pair 
                if (playerHands[i].IsPairSameValue())
                {
                    // set hand panel color to be green
                    labelController.playerHandLabel[i].bg.sprite = labelController.labelSpriteGreen;

                    // calculate its perfect pair reward multipier
                    playerHands[i].CalculatePerfectPairReward();

                    // display perfect pair reward panel
                    if (!gameManager.players[i].isNPC)
                        labelController.SetPerfectPairLabel(playerHands[i].GetPerfectPairMultiplier());
                }
                else
                {
                    // otherwise set hand panel color to be red
                    labelController.playerHandLabel[i].bg.sprite = labelController.labelSpriteRed;
                }
            }
        }

        /// <summary>
        /// Methdo to calculate perfect pair win & loss for a single playerr
        /// </summary>
        /// <param name="index">index of the player</param>
        void PerfectPairResult(int index)
        {
            // get perfect pair wager & multiplier
            var message = "";
            var perfectPairBet = playerAction.bets[index].perfectPairWager;
            var multiplier = playerHands[index].GetPerfectPairMultiplier();

            // if the player doesn't win from the perfect pair, take his 
            // perfect pair wagers alway
            if (multiplier == 0)
            {
                message = $"<color=\"red\">-{perfectPairBet}</color>";
                TakingChipsAway(index, GameManager.WAGER_INDEX_BONUS_SPLITE_WAGER);
            }
            else
            {
                message = $"<color=\"green\">+{perfectPairBet * multiplier}</color>";
                gameManager.players[index].EditPlayerChip(perfectPairBet * (multiplier + 1));
                MultiplyWagerModel(index, GameManager.WAGER_INDEX_BONUS_SPLITE_WAGER, multiplier) ;
            }

            // display the result with floating text
            labelController.FloatText(message, labelController.betLabels[index].transform.position, 60f, 3f, 0.3f);
        }

        /// <summary>
        /// An IEnumerator that runs through all players and apply perfect pair
        /// reward / loss for each player
        /// </summary>
        /// <returns></returns>
        public IEnumerator CheckPerfectPair()
        {
            // applying perfect pair reward / loss
            for (int i = 0; i < gameManager.players.Length; i++)
            {
                // skip this iteration if 'n' player does not exist
                if (gameManager.players[i] == null)
                    continue;

                // skip this iteration if 'n' player did not bet on perfect pair
                if (playerAction.bets[i].perfectPairWager == 0)
                    continue;

                // calculate 'n' player's win & loss and play wager animation
                PerfectPairResult(i);
                wagerAnimator.Play();

                yield return new WaitForSeconds(WAIT_TIME_COMPARE);
            }

            // clear perfect pair relevent objects and UI
            for (int i = 0; i < gameManager.players.Length; i++)
            {
                // skip this iteration if 'n' player does not exist
                if (gameManager.players[i] == null)
                    continue;

                // remove perfect pair wager number from bet label text
                labelController.SetBetLabel(i, playerAction.bets[i].anteWager);

                // reset player hand label color and hide perfect pair wager label
                labelMarks[i][GameManager.WAGER_INDEX_BONUS_SPLITE_WAGER].enabled = false;
                labelController.playerHandLabel[i].bg.sprite = labelController.labelSpriteTransparent;

                // remove perfect pair bonus label
                labelController.perfectPairLabel.Switch(false);

                // remove wager stack in perfect pair slot
                ClearWagerStackForSingleSlot(i, GameManager.WAGER_INDEX_BONUS_SPLITE_WAGER);
            }
        }
    }
}
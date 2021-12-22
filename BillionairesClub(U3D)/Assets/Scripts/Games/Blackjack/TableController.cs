using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Blackboard;
using static Const;

namespace Blackjack
{
    using static Para;
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
        private bool hasInsuranceBet;            // determine if there are insurance bet on the table
        private bool hasUnclearPlayer;           // determine if there are players waiting to compare

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
            cardDeck = new CardDeck(CARD_DECK_COUNT);
            //cardDeck.Shuffle();
            cardDeck.DebugDeck_Blackjack();

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
            labelMarks[playerId][WAGER_INDEX_BONUS_SPLITE_WAGER].enabled = true;
            labelMarks[playerId][WAGER_INDEX_DOUBLE].enabled = false;
            labelMarks[playerId][WAGER_INDEX_SPLIT_DOUBLE].enabled = false;
            labelMarks[playerId][WAGER_INDEX_INSURANCE].enabled = false;
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
            // reset triggers
            hasInsuranceBet = false;
            hasUnclearPlayer = false;
            insuranceTriggered = false;

            // hide dealer card objects 
            for (int i = 0; i < dealerCardsObj.Length; i++)
                dealerCardsObj[i].SetActive(false);

            // hide all player card objects
            for (int i = 0; i < playerCardsObj.Length; i++)
                for (int j = 0; j < playerCardsObj[i].Length; j++)
                    for (int k = 0; k < playerCardsObj[i][j].Length; k++)
                        playerCardsObj[i][j][k].SetActive(false);

            // reset all player's hand
            dealerHand.Reset();
            for (int i = 0; i < playerHands.Length; i++)
            {
                ResetLabelMark(i);
                playerHands[i].Reset();
                labelController.playerHandLabel[i].bg.sprite = labelController.labelSpriteTransparent;
            }

            // reset table labels
            labelController.Reset();

            // reset dealer's hand label
            labelController.dealerHandLabel.bg.sprite = labelController.labelSpriteTransparent;
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

                        // display global player hand label
                        var labelText = playerHands[j].ToString();
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

            // otherwise set insuranceTriggered to be true
            insuranceTriggered = true;

            // display insurance label marks on the table
            for (int i = 0; i < labelMarks.Length; i++)
                if (gameManager.players[i] != null)
                    labelMarks[i][WAGER_INDEX_INSURANCE].enabled = true;
        }

        /// <summary>
        /// Method to reward or tak away players chip in insurance bet section
        /// </summary>
        /// <returns></returns>
        IEnumerator InsuranceResult()
        {
            // run through each player and see determine win & lose
            for (int i = 0; i < gameManager.players.Length; i++)
            {
                // skip this iteration if 'n' player does not exist
                if (gameManager.players[i] == null)
                    continue;

                // skip this iteration if 'n' player does not have insurance bet
                var insuranceBet = playerAction.bets[i].insuranceWager;
                if (insuranceBet == 0)
                    continue;

                // otherwise, the player either win or lose insurance wager
                var message = "";
                if (dealerHand.HasBlackjack())
                {
                    message = $"<color=\"green\">+{insuranceBet * REWARD_INSURANCE:C0}</color>";
                    gameManager.players[i].EditPlayerChip(insuranceBet * (REWARD_INSURANCE + 1));
                    MultiplyWagerModel(i, WAGER_INDEX_INSURANCE, REWARD_INSURANCE);
                }
                else
                {
                    message = $"<color=\"red\">-{insuranceBet:C0}</color>";
                    TakingChipsAway(i, WAGER_INDEX_INSURANCE);
                }

                // play wager animation and display the result with a floating text
                wagerAnimator.Play();
                labelController.insuranceBets[i].Switch(false);
                labelController.FloatText(message, labelController.insuranceBets[i].transform.position, 60f, 3f, 0.3f);

                yield return new WaitForSeconds(WAIT_TIME_COMPARE);
            }

            // clear insurance relevent objects and UI
            for (int i = 0; i < gameManager.players.Length; i++)
            {
                // skip this iteration if 'n' player does not exist
                if (gameManager.players[i] == null)
                    continue;

                // otherwise, hide insurance wager stack 
                ClearWagerStackForSingleSlot(i, WAGER_INDEX_INSURANCE);
            }
        }

        /// <summary>
        /// Method for the dealer to continue draw cards until its hand reaches 17
        /// </summary>
        /// <returns></returns>
        IEnumerator DealTill17()
        {
            // dealer keep drawing cards
            while (dealerHand.GetHighestRank() < 17 && !dealerHand.HasFiveCardCharlie())
            {
                yield return new WaitForSeconds(WAIT_TIME_DEAL * 4);

                // deal a card for dealer
                var card = DrawACard();
                dealerHand.AddCard(card);
                dealerCardsObj[dealerHand.GetCardCount() - 1].SetCard(card);
                audioManager.PlayAudio(audioManager.clipDealCards, AudioType.Sfx);
                labelController.dealerHandLabel.tmp.text = dealerHand.ToString();

                // if the dealer busts, change dealer label background color to red
                if (dealerHand.HasBust())
                    labelController.dealerHandLabel.bg.sprite = labelController.labelSpriteRed;
                else if (dealerHand.HasFiveCardCharlie())
                    labelController.dealerHandLabel.bg.sprite = labelController.labelSpriteGreen;
            }

            // at the end if the dealer's hand is value ranked, display the highest value
            if (dealerHand.handRank[0] == HandRank.Value) 
            {
                dealerHand.stand[0] = true;
                labelController.dealerHandLabel.tmp.text = dealerHand.ToString();
            }
        }

        /// <summary>
        /// Method for the dealer to compare his hand with every individual player
        /// </summary>
        /// <returns></returns>
        IEnumerator Comparing()
        {
            // run through each player and compare result
            for (int i = 0; i < gameManager.players.Length; i++)
            {
                // skip this iteration if 'n' player does not exist
                if (gameManager.players[i] == null)
                    continue;

                // skip this iteration if 'n' player is already clear
                if (playerAction.bets[i].isClear)
                    continue;

                // otherwise run through player hands,
                // each player has up to 2 hands
                for (int j = 0; j < MAX_HAND; j++)
                {
                    // skip this iteration if 'n' hand is not stood
                    if (!playerHands[i].stand[j])
                        continue;

                    // compre the 'n' hand to dealer's hand
                    var result = playerHands[i].CompareToDealer(j, dealerHand);
                    switch (result)
                    {
                        case Result.Win:
                            PlayerWins(i, j, playerAction.bets[i]);
                            break;
                        case Result.Lose:
                            PlayerLoses(i, j, playerAction.bets[i]);
                            break;
                        case Result.Standoff:
                            Standoff(i, j, playerAction.bets[i]);
                            break;
                        default:
                            break;
                    }

                    // change the hand label background image to indicate who win and who lose
                    labelController.SetHandRankLabelColor(i, result);
                    yield return new WaitForSeconds(WAIT_TIME_COMPARE);
                }

                // clean the player after comparison
                ClearSinglePlayer(i);
            }
        }

        /// <summary>
        /// Method to reward a player when he wins
        /// </summary>
        /// <param name="playerIndex">index of the player</param>
        /// <param name="handIndex">index of the hand</param>
        /// /// <param name="bet">bet data from the player</param>
        void PlayerWins(int playerIndex, int handIndex, Bet bet)
        {
            // calculate the reward multipiler 
            var multipiler = 0f;
            switch (playerHands[playerIndex].handRank[handIndex])
            {
                case HandRank.Value:
                    multipiler = REWARD_NORMAL;
                    break;
                case HandRank.Blackjack:
                    multipiler = REWARD_BLACKJACK;
                    break;
                case HandRank.FiveCardCharlie:
                    multipiler = REWARD_FIVECARDS;
                    break;
                default:
                    break;
            }

            // calculate the actual reward
            var reward = 0f;
            var rewardDisplay = 0f;
            if (handIndex == 0)
            {
                if (bet.anteWager > 0)
                {
                    reward += bet.anteWager * (1 + multipiler);
                    rewardDisplay += bet.anteWager * multipiler;
                    MultiplyWagerModel(playerIndex, WAGER_INDEX_ANTE, multipiler);
                }
                if (bet.doubleWager > 0)
                {
                    reward += bet.doubleWager * (1 + multipiler);
                    rewardDisplay += bet.doubleWager * multipiler;
                    MultiplyWagerModel(playerIndex, WAGER_INDEX_DOUBLE, multipiler);
                }
            }
            else if (handIndex == 1)
            {
                if (bet.anteWagerSplit > 0)
                {
                    reward += bet.anteWagerSplit * (1 + multipiler);
                    rewardDisplay += bet.anteWagerSplit * multipiler;
                    MultiplyWagerModel(playerIndex, WAGER_INDEX_BONUS_SPLITE_WAGER, multipiler);
                }
                if (bet.doubleWagerSplit > 0)
                {
                    reward += bet.doubleWagerSplit * (1 + multipiler);
                    rewardDisplay += bet.doubleWagerSplit * multipiler;
                    MultiplyWagerModel(playerIndex, WAGER_INDEX_SPLIT_DOUBLE, multipiler);
                }
            }

            // modify player's chip amount
            gameManager.players[playerIndex].EditPlayerChip(reward);

            // play the wager animation and display float text
            wagerAnimator.Play();
            labelController.FloatText($"<color=\"green\">+{rewardDisplay:C0}</color>",
                labelController.betLabels[playerIndex].transform.position, 60f, 3f, 0.3f);
        }

        /// <summary>
        /// Method to take away players chip when he loses 
        /// </summary>
        /// <param name="playerIndex">index of the player</param>
        /// <param name="handIndex">index of the hand</param>
        /// /// <param name="bet">bet data from the player</param>
        void PlayerLoses(int playerIndex, int handIndex, Bet bet)
        {
            // calculate the loss
            var loss = 0f;
            if (handIndex == 0)
            {
                if (bet.anteWager > 0)
                {
                    loss += bet.anteWager;
                    TakingChipsAway(playerIndex, WAGER_INDEX_ANTE);
                }
                if (bet.doubleWager > 0)
                {
                    loss += bet.doubleWager;
                    TakingChipsAway(playerIndex, WAGER_INDEX_DOUBLE);
                }
            }
            else if (handIndex == 1)
            {
                if (bet.anteWagerSplit > 0)
                {
                    loss += bet.anteWagerSplit;
                    TakingChipsAway(playerIndex, WAGER_INDEX_BONUS_SPLITE_WAGER);
                }
                if (bet.doubleWagerSplit > 0)
                {
                    loss += bet.doubleWagerSplit;
                    TakingChipsAway(playerIndex, WAGER_INDEX_SPLIT_DOUBLE);
                }
            }

            // play the wager animation and display float text
            wagerAnimator.Play();
            labelController.FloatText($"<color=\"red\">+{loss:C0}</color>",
                labelController.betLabels[playerIndex].transform.position, 60f, 3f, 0.3f);
        }

        /// <summary>
        /// Method to return player poker chip when it is standoff
        /// </summary>
        /// <param name="playerIndex">index of the player</param>
        /// <param name="handIndex">index of the hand</param>
        /// <param name="bet">bet data from the player</param>
        void Standoff(int playerIndex, int handIndex, Bet bet)
        {
            // calculate refund
            var refund = 0f;
            if (handIndex == 0)
                refund = bet.anteWager + bet.doubleWager;
            else if (handIndex == 1)
                refund = bet.anteWagerSplit + bet.doubleWagerSplit;

            // give money back to the player
            gameManager.players[playerIndex].EditPlayerChip(refund);
        }

        /// <summary>
        /// Method for the dealer to decide whether or not to continue the game.
        /// (e.g if all the players bust and no one bet on the insurance, the 
        /// dealer just end the game)
        /// </summary>
        /// <returns></returns>
        public IEnumerator DealerDecision()
        {
            // check whether or not the game needs to continue
            CheckGameState();

            // if there is no insurance bet and all players are cleared
            // we can safely break this coroutine
            if (!hasInsuranceBet && !hasUnclearPlayer)
                yield break;

            // otherwise, reveal dealer's second card
            dealerCardsObj[1].transform.localEulerAngles = Vector3.zero;
            labelController.dealerHandLabel.tmp.text = dealerHand.ToString();

            // compare player's win & lose in insurance section
            if (hasInsuranceBet)
                yield return InsuranceResult();

            // if there is no unclear player, end the round
            if (!hasUnclearPlayer)
                yield break;

            // keep drawing card until reach 17
            yield return DealTill17();

            // compare dealer's hand and existing players hand
            yield return Comparing();
        }

        /// <summary>
        /// Method to detect whether or nor the game needs to continue
        /// by running through all players and check their insurance bet
        /// and state. 
        /// </summary>
        void CheckGameState()
        {
            // run through each player and see if they have insurance bet or 
            // they are waiting to compare
            for (int i = 0; i < gameManager.players.Length; i++)
            {
                // skip this iteration if 'n' player does not exist
                if (gameManager.players[i] == null)
                    continue;

                if (playerAction.bets[i].insuranceWager > 0)
                    hasInsuranceBet = true;

                if (!playerAction.bets[i].isClear)
                    hasUnclearPlayer = true;
            }
        }

        /// <summary>
        /// Method for the player to draw additional cards
        /// </summary>
        /// <param name="playerIndex">index of the player</param>
        /// <param name="handIndex">index of the player's current hand</param>
        public void OnPlayerHit(int playerIndex, int handIndex)
        {
            // draw a card
            var card = DrawACard();
            var cardIndex = playerHands[playerIndex].GetCardCount(handIndex);
            playerHands[playerIndex].AddCard(card, handIndex);
            playerCardsObj[playerIndex][handIndex][cardIndex].SetCard(card);
            audioManager.PlayAudio(audioManager.clipDealCards, AudioType.Sfx);

            // get hand string
            var labelText = playerHands[playerIndex].ToString();

            // display global player hand label
            labelController.playerHandLabel[playerIndex].tmp.text = labelText;

            // update card sprite from local hand panel
            if (!gameManager.players[playerIndex].isNPC)
            {
                labelController.RevealACard(GetCardSprite(card.GetCardIndex()), 
                    cardIndex, handIndex);
                labelController.localHandLabels[handIndex].tmp.text = 
                    playerHands[playerIndex].ToString(handIndex);
            }
        }

        public void OnPlayerSplit(int playerIndex)
        {
            // split player's hand
            playerHands[playerIndex].SplitHand();

            // update card objects display on the table
            var card = playerHands[playerIndex].GetCard(1, 0);
            playerCardsObj[playerIndex][0][1].SetActive(false);
            playerCardsObj[playerIndex][1][0].SetCard(card);



            // update label display
            labelController.SplitHand(playerIndex, playerHands[playerIndex], 
                GetCardSprite(card.GetCardIndex()));
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
                message = $"<color=\"red\">-{perfectPairBet:C0}</color>";
                TakingChipsAway(index, WAGER_INDEX_BONUS_SPLITE_WAGER);
            }
            else
            {
                message = $"<color=\"green\">+{perfectPairBet * multiplier:C0}</color>";
                gameManager.players[index].EditPlayerChip(perfectPairBet * (multiplier + 1));
                MultiplyWagerModel(index, WAGER_INDEX_BONUS_SPLITE_WAGER, multiplier) ;
            }

            // play wager animation and display the result with a floating text
            wagerAnimator.Play();
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

                // calculate 'n' player's win & loss
                PerfectPairResult(i);
                
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

                // reset player hand label color
                labelController.playerHandLabel[i].bg.sprite = labelController.labelSpriteTransparent;

                // remove perfect pair bonus label
                labelController.perfectPairLabel.Switch(false);

                // remove wager stack in perfect pair slot
                ClearWagerStackForSingleSlot(i, WAGER_INDEX_BONUS_SPLITE_WAGER);
            }
        }

        /// <summary>
        /// Method for a single player's bust, dealer collects all chips from the 
        /// specific wager stacks
        /// </summary>
        /// <param name="playerIndex">index of the player</param>
        /// <param name="handIndex">index of the hand</param>
        public void Bust(int playerIndex, int handIndex = 0)
        {
            var message = "";
            var bet = playerAction.bets[playerIndex];

            if (handIndex == 0)
            {
                message = $"<color=\"red\">-{bet.anteWager:C0}</color>";
                TakingChipsAway(playerIndex, WAGER_INDEX_ANTE);
            }
            else if (handIndex == 1)
            {
                message = $"<color=\"red\">-{bet.anteWagerSplit:C0}</color>";
                TakingChipsAway(playerIndex, WAGER_INDEX_BONUS_SPLITE_WAGER);
            }

            // display the result with floating text
            labelController.FloatText(message, labelController.betLabels[playerIndex].transform.position, 60f, 3f, 0.3f);

            // hide the bet label
            labelController.betLabels[playerIndex].Switch(false);

            // play the wager animator
            wagerAnimator.Play();
        }

        /// <summary>
        /// Method for a single player's five card charlie, instant win
        /// </summary>
        /// <param name="playerIndex">index of the player</param>
        /// <param name="handIndex">index of the hnad</param>
        public void FiveCardCharlie(int playerIndex, int handIndex = 0)
        {
            var message = "";
            var bet = playerAction.bets[playerIndex];

            if (handIndex == 0) 
            {
                message = $"<color=\"green\">+{bet.anteWager * REWARD_FIVECARDS:C0}</color>";
                MultiplyWagerModel(playerIndex, WAGER_INDEX_ANTE, REWARD_FIVECARDS);
                gameManager.players[playerIndex].EditPlayerChip(bet.anteWager * (REWARD_FIVECARDS + 1));
            }
            else if (handIndex == 1)
            {
                message = $"<color=\"green\">+{bet.anteWagerSplit * REWARD_FIVECARDS:C0}</color>";
                MultiplyWagerModel(playerIndex, WAGER_INDEX_DOUBLE, REWARD_FIVECARDS);
                gameManager.players[playerIndex].EditPlayerChip(bet.anteWagerSplit * (REWARD_FIVECARDS + 1));
            }

            // display the result with floating text
            labelController.FloatText(message, labelController.betLabels[playerIndex].transform.position, 60f, 3f, 0.3f);
            
            // hide the bet label
            labelController.betLabels[playerIndex].Switch(false);

            // play the wager animator
            wagerAnimator.Play();
        }

        /// <summary>
        /// Method to clear one single player out of the table
        /// </summary>
        /// <param name="playerIndex">index of the player</param>
        public void ClearSinglePlayer(int playerIndex)
        {
            // set isClear to be true
            playerAction.bets[playerIndex].isClear = true;

            // hide player's bet label and local hand panel
            labelController.betLabels[playerIndex].Switch(false);
            
            // if the player is an actual player, hide its hand rank panel
            if (!gameManager.players[playerIndex].isNPC)
            {
                labelController.playerHandLabel[playerIndex].Switch(false);
                labelController.ResetLocalHandVisbility();
            }

            // hide player's card objects
            for (int i = 0; i < playerCardsObj[playerIndex].Length; i++)
                for (int j = 0; j < playerCardsObj[playerIndex][i].Length; j++)
                    playerCardsObj[playerIndex][i][j].SetActive(false);

            // hide player's wager stacks (exclude insurance slot)
            ClearWagerStackForSingleSlot(playerIndex, WAGER_INDEX_ANTE);
            ClearWagerStackForSingleSlot(playerIndex, WAGER_INDEX_BONUS_SPLITE_WAGER);
            ClearWagerStackForSingleSlot(playerIndex, WAGER_INDEX_DOUBLE);
            ClearWagerStackForSingleSlot(playerIndex, WAGER_INDEX_SPLIT_DOUBLE);
        }
    }
}
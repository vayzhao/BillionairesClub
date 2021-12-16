using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Blackboard;

namespace TexasBonus
{
    public class TableController : WagerBehaviour
    {
        [Header("UI Components")]
        [Tooltip("A game object that holds the table labels")]
        public GameObject group_slots; 

        [Header("Player's asset")]
        [Tooltip("Poker chips")]
        public GameObject[] chips;
        [Tooltip("Objects that hold label marks for the wager")]
        public GameObject[] marks;

        [Header("Poker Models")]
        [Tooltip("Models to repersent dealer's cards")]
        public GameObject[] dealerCardsObj;
        [Tooltip("Models to repersent community cards")]
        public GameObject[] communityCardsObj;
        [Tooltip("Models to repersent player's cards")]
        public GameObject[] playerCardsObj;

        [HideInInspector]
        public GameManager gameManager;          // the game manager script
        [HideInInspector]
        public PlayerAction playerAction;        // the player action script
        [HideInInspector]
        public LabelController labelController;  // the label controller script
        [HideInInspector]
        public bool allPlayerFolded;             // a state to determine whether or not all the players have folded thier cards
        
        private CardDeck cardDeck;               // card deck used in the game
        private Card[] dealerHand;               // card data for dealer
        private Card[] communityCards;           // card data for community cards
        private Card[][] playerCards;            // card data for players
        private HandStrength dealerHandStrengh;  // hand evaluator for dealer
        private HandStrength[] handStrengths;    // hand evaluator for players
        
        public void Setup()
        {
            InitializeCards();
            InitializeWagerPos();
            HideInitialObjects();
        }
        
        /// <summary>
        /// Method to initialize the card deck and hand evaluators
        /// </summary>
        void InitializeCards()
        {
            // setup card deck
            cardDeck = new CardDeck();

            // setup hand & hand evaluator for the dealer
            dealerHand = new Card[2];
            dealerHandStrengh = new HandStrength();

            // setup hand & hand evaluator for the players
            playerCards = new Card[gameManager.players.Length][];
            handStrengths = new HandStrength[gameManager.players.Length];
            for (int i = 0; i < playerCards.Length; i++)
            {
                playerCards[i] = new Card[2];
                handStrengths[i] = new HandStrength();
            }

            // setup community cards
            communityCards = new Card[5];
        }

        /// <summary>
        /// Method to record placing position for wager model
        /// </summary>
        void InitializeWagerPos()
        {
            // initialize associated variables
            wagerPos = new Vector3[gameManager.players.Length][];
            wagerStacks = new GameObject[gameManager.players.Length][];

            // initialize wager positions
            for (int i = 0; i < wagerPos.Length; i++)
            {
                wagerPos[i] = new Vector3[5];
                wagerStacks[i] = new GameObject[5];
                for (int j = 0; j < wagerPos[i].Length; j++)
                    wagerPos[i][j] = wager[i].transform.GetChild(j).transform.position;
            }
        }

        /// <summary>
        /// Method to hide all players assets on the table, also dealer's cards
        /// </summary>
        void HideInitialObjects()
        {
            // hide card slots
            group_slots.SetActive(false);

            // hide all players asset
            for (int i = 0; i < gameManager.players.Length; i++)
            {
                chips[i].SetActive(false);
                marks[i].SetActive(false);
                wager[i].SetActive(false);
                playerCardsObj[i].SetActive(false);
            }

            // hide all dealer card objects
            for (int i = 0; i < dealerCardsObj.Length; i++)
                dealerCardsObj[i].SetActive(false);

            // hide all community card objects
            for (int i = 0; i < communityCardsObj.Length; i++)
                communityCardsObj[i].SetActive(false);
        }

        /// <summary>
        /// Method to get current hand strength of a single player
        /// </summary>
        /// <param name="index">index of the player</param>
        /// <returns></returns>
        public HandStrength GetHandStrength(int index) => handStrengths[index];

        /// <summary>
        /// Methdo to reset the table, includes hiding card models, reseting labels,
        /// hand evluators and card deck shuffling
        /// </summary>
        public void ResetTable()
        {
            // hide dealer card objects and turn it to face-down
            for (int i = 0; i < dealerCardsObj.Length; i++)
            {
                dealerCardsObj[i].SetActive(false);
                dealerCardsObj[i].transform.localEulerAngles = new Vector3(-90f, 0f, 180f);
            }                
            for (int i = 0; i < communityCardsObj.Length; i++)
            {
                communityCardsObj[i].SetActive(false);
                communityCardsObj[i].transform.localEulerAngles = new Vector3(-90f, 0f, 180f);
            }

            // reset table labels
            labelController.Reset();

            // reset hand strength datas
            dealerHandStrengh.Reset();
            for (int i = 0; i < handStrengths.Length; i++)
                handStrengths[i].Reset();

            // shuffle the card deck
            // cardDeck.DebugDeck();
            cardDeck.Shuffle();
        }

        /// <summary>
        /// Method to display labels on the table, it is called when the 
        /// start button is pressed
        /// </summary>
        public void DisplayTableLabel()
        {
            // set table label to be visible
            group_slots.SetActive(true);

            // set player label to be visible, depends on the player status
            for (int i = 0; i < gameManager.players.Length; i++)
            {
                // skip if the 'n' player does not exist
                if (gameManager.players[i] == null)
                    continue;

                // otherwise, display the 'n' player's poker chip and label mark
                chips[i].SetActive(true);
                marks[i].SetActive(true);
            }
        }

        /// <summary>
        /// Method to hide a player's wager models, cards and label, it is 
        /// called when the player has lost
        /// </summary>
        /// <param name="index"></param>
        private void HidePlayerBetAndCards(int index)
        {
            // hide card models and bet label
            playerCardsObj[index].HideCard();
            labelController.betLabels[index].Switch(false);

            // remove all chips from this player
            ClearWagerStackForSinglePlayer(index);
        }

        /// <summary>
        /// Method to scan through each player and find out who has folded
        /// </summary>
        /// <returns></returns>
        public IEnumerator DetermineFoldedPlayer()
        {
            // assume all player has folded
            allPlayerFolded = true;

            // run through all players
            for (int i = 0; i < gameManager.players.Length; i++)
            {
                // skip if the 'n' player does not exist 
                if (gameManager.players[i] == null)
                    continue;

                // if the 'n' player has folded, take away his wager and cards
                if (playerAction.bets[i].hasFolded)
                {
                    TakingChipsAway(i, 0);
                    TakingChipsAway(i, 4);
                    wagerAnimator.Play();
                    labelController.DisplayBetResult(i, playerAction.bets[i].GetAmountChange());
                    yield return new WaitForSeconds(Const.WAIT_TIME_CHIP_TOTAL + Const.WAIT_TIME_CHIP_TRAVEL);
                    HidePlayerBetAndCards(i);
                }
                // otherwise, as long as at least one player has not folded,
                // set allPlayerFolded to be false
                else
                {
                    allPlayerFolded = false;
                }                
            }            
        }

        /// <summary>
        /// Method to deal dealer's cards and community cards
        /// </summary>
        /// <returns></returns>
        public IEnumerator DealInitialCards()
        {
            // dealing dealer's cards
            for (int i = 0; i < dealerCardsObj.Length; i++)
            {
                dealerHand[i] = cardDeck.DrawACard();
                dealerCardsObj[i].SetCard(dealerHand[i]);
                audioManager.PlayAudio(audioManager.clipDealCards, AudioType.Sfx);
                yield return new WaitForSeconds(Const.WAIT_TIME_DEAL);
            }

            // dealing community cards
            for (int i = 0; i < 5; i++)
            {
                communityCards[i] = cardDeck.DrawACard();
                communityCardsObj[i].SetCard(communityCards[i]);
                audioManager.PlayAudio(audioManager.clipDealCards, AudioType.Sfx);
                yield return new WaitForSeconds(Const.WAIT_TIME_DEAL);                
            }
        }

        /// <summary>
        /// Method to deal player's cards
        /// </summary>
        /// <returns></returns>
        public IEnumerator DealPlayerCards()
        {
            // first of all set local hand-rank panel to be visible
            labelController.SetLocalHandRankPanelVisibility(true);

            // each player will receive 2 cards initially
            for (int i = 0; i < 2; i++)
            {
                // run though each player
                for (int j = 0; j < gameManager.players.Length; j++)
                {
                    // skip when the 'n' player does not exist
                    if (gameManager.players[j] == null)
                        continue;

                    // deal a card for this player
                    playerCards[j][i] = cardDeck.DrawACard();
                    playerCardsObj[j].transform.GetChild(i).gameObject.SetCard(playerCards[j][i]);
                    
                    // if this player is a user-player, update its cardTexture visibility in hand-rank panel
                    if (!gameManager.players[j].isNPC)
                        labelController.cardTexture[i].enabled = true;

                    yield return new WaitForSeconds(Const.WAIT_TIME_EMPTY);
                }
            }
        }

        /// <summary>
        /// Method to reveal player's cards
        /// </summary>
        public void RevealPlayerCards()
        {
            // run through each player
            for (int i = 0; i < gameManager.players.Length; i++)
            {
                // skip when the 'n' player does not exist
                if (gameManager.players[i] == null)
                    continue;

                // otherwise, reveal the cards
                for (int j = 0; j < 2; j++)
                {
                    // add this card to the player's hand evaluator
                    handStrengths[i].AddCard(playerCards[i][j]);

                    // update card texture in hand-rank panel if this player is a user-player
                    if (!gameManager.players[i].isNPC)
                        labelController.cardTexture[j].sprite = GetCardSprite(playerCards[i][j].GetCardIndex());
                }

                // recompute the hand-rank
                handStrengths[i].Recompute();

                // if this player is a user-player, update its hand-rank panel 
                if (!gameManager.players[i].isNPC)
                {
                    labelController.localHandLabel.tmp.text = handStrengths[i].GetInitialHandString();
                    labelController.SetBonusLabel(handStrengths[i].GetBonusMultiplier());
                    labelController.cardTexture[0].sprite = GetCardSprite(playerCards[i][0].GetCardIndex());
                    labelController.cardTexture[1].sprite = GetCardSprite(playerCards[i][1].GetCardIndex());
                    audioManager.PlayAudio(audioManager.clipDealCards, AudioType.Sfx);
                }
            }
        }

        /// <summary>
        /// Method to reveal a community card
        /// </summary>
        /// <param name="index">index of the revealing card</param>
        /// <param name="computeHandStrengh">determine whether or not to recompute hand-rank for all players</param>
        /// <returns></returns>
        public IEnumerator RevealCommunityCard(int index, bool computeHandStrengh)
        {
            // set the community card to be face-up
            communityCardsObj[index].transform.localEulerAngles = new Vector3(-90, 0, 0);

            // add this card to all hand evaluators
            for (int i = 0; i < handStrengths.Length; i++)
                handStrengths[i].AddCard(communityCards[index]);
            dealerHandStrengh.AddCard(communityCards[index]);

            // play reveal card sound effect
            audioManager.PlayAudio(audioManager.clipDealCards, AudioType.Sfx);

            // determine whether or not to recompute hand-rank for all players
            if (computeHandStrengh)
            {
                // run through all hand evaluators
                for (int i = 0; i < handStrengths.Length; i++)
                {
                    // skip if the 'n' player does not exisit or has folded
                    if (gameManager.players[i] == null || playerAction.bets[i].hasFolded)
                        continue;

                    // recompute hand-rank
                    handStrengths[i].Recompute();

                    // update hand-rank panel's text if this player is a user-player
                    if (!gameManager.players[i].isNPC)
                        labelController.localHandLabel.tmp.text = handStrengths[i].ToString();
                }
            }           
            yield return new WaitForSeconds(Const.WAIT_TIME_DEAL);
        }

        /// <summary>
        /// Method to reveal dealer's card 
        /// </summary>
        /// <returns></returns>
        public IEnumerator RevealDealerHand()
        {
            // run through dealer's cards
            for (int i = 0; i < 2; i++)
            {
                // add this card to dealer's hand evaluator
                dealerHandStrengh.AddCard(dealerHand[i]);

                // set the card to be face-up
                dealerCardsObj[i].transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
                yield return new WaitForSeconds(Const.WAIT_TIME_DEAL);
            }

            // recompute dealer's hand-rank
            dealerHandStrengh.Recompute();

            // play reveal card sound effect
            audioManager.PlayAudio(audioManager.clipDealCards, AudioType.Sfx);

            // display dealer's hand-rank label
            labelController.dealerHandRankLabel.Display(dealerHandStrengh.ToString("<color=#00FF01><size=80%>"));
        }

        /// <summary>
        /// Method to check if a player wins in bonus section
        /// </summary>
        /// <param name="index">index of the player</param>
        public void BonusReward(int index)
        {
            // get bonus wager & multiplier
            var bonusWager = playerAction.bets[index].bonusWager;
            var multiplier = handStrengths[index].GetBonusMultiplier();

            // only continue when the player has bet on bonus and trigger bonus reward
            if (!(bonusWager > 0 && multiplier > 0))
            {
                TakingChipsAway(index, 4);
                return;
            }

            // multiply bonus wager
            MultiplyWagerModel(index, 4, multiplier);

            // reward the player
            playerAction.bets[index].EditAmountChange(bonusWager * (multiplier + 1));
            gameManager.players[index].EditPlayerChip(bonusWager * (multiplier + 1));
        }

        /// <summary>
        /// Method to compare a player's hand to dealer's hand and find out the winner
        /// </summary>
        /// <param name="index"></param>
        public void Compare(int index)
        {
            // get the result of comparison
            var result = HandStrength.Compare(handStrengths[index], dealerHandStrengh);

            // display the card models
            playerCardsObj[index].ShowCard();

            // display the hand rank panel
            labelController.SetHandRankLabelColor(index, result);            
            labelController.handRankLabel[index].Display(handStrengths[index].ToString("<color=#00FF01><size=80%>"));

            // if the player is an actual player, hide its hand rank panel UI
            if (!gameManager.players[index].isNPC)
                labelController.SetLocalHandRankPanelVisibility(false);

            // play fold sound effect
            audioManager.PlayAudio(audioManager.clipFold, AudioType.Sfx);

            // enum player's result
            switch (result)
            {
                case Result.Win:
                    ResultWin(index);
                    break;
                case Result.Lose:
                    ResultLose(index);
                    break;
                case Result.Standoff:
                    ResultStandoff(index);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Method to reward the player when win
        /// </summary>
        /// <param name="index">index of the player</param>
        private void ResultWin(int index)
        {
            // initialize the reward
            var reward = playerAction.bets[index].anteWager;

            // the player wins ante wager if he has hand-rank equal to flush or above
            if (handStrengths[index].rank >= Rank.Flush)
            {
                MultiplyWagerModel(index, 0, 1);
                reward += playerAction.bets[index].anteWager;
            }
            if (playerAction.bets[index].flopWager > 0)
            {
                MultiplyWagerModel(index, 1, 1);
                reward += playerAction.bets[index].flopWager * 2;
            }
            if (playerAction.bets[index].turnWager > 0)
            {
                MultiplyWagerModel(index, 2, 1);
                reward += playerAction.bets[index].turnWager * 2;
            }
            if (playerAction.bets[index].riverWager > 0)
            {
                MultiplyWagerModel(index, 3, 1);
                reward += playerAction.bets[index].riverWager * 2;
            }

            // modify player's chip amount
            gameManager.players[index].EditPlayerChip(reward);
            playerAction.bets[index].EditAmountChange(reward);
        }

        /// <summary>
        /// Method to take away chips from the player when lose
        /// </summary>
        /// <param name="index">index of the player</param>
        private void ResultLose(int index)
        {
            // run through each chip slot and take away chips
            for (int i = 0; i < 4; i++)
                TakingChipsAway(index, i);
        }

        /// <summary>
        /// Method to return chips to the player when standoff
        /// </summary>
        /// <param name="index">index of the player</param>
        private void ResultStandoff(int index)
        {
            // return wagers to the player
            gameManager.players[index].EditPlayerChip(playerAction.bets[index].GetTotal());
            playerAction.bets[index].EditAmountChange(playerAction.bets[index].GetTotal());
        }

        /// <summary>
        /// Method to hide a player's wager and cards
        /// </summary>
        /// <param name="index"></param>
        public void HidePlayerCards(int index)
        {
            // return the the player does not exist 
            if (index < 0 || gameManager.players[index] == null)
                return;

            // hide wager, bet label and hand-rank label
            HidePlayerBetAndCards(index);
            labelController.handRankLabel[index].Switch(false);
        }

        /// <summary>
        /// Method to player the chip animation player
        /// </summary>
        /// <param name="index"></param>
        public void PlayChipAnimation(int index)
        {
            // play the animation player
            wagerAnimator.Play();

            // display chip amount change for this player
            labelController.DisplayBetResult(index, playerAction.bets[index].GetAmountChange());
        }
    }
}
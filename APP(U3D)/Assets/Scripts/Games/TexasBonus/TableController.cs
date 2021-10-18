using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TexasBonus
{
    public class TableController : MonoBehaviour
    {
        [Header("UI Components")]
        [Tooltip("A game object that holds the table labels")]
        public GameObject group_slots; 

        [Header("Player's asset")]
        [Tooltip("Poker chips")]
        public GameObject[] chips;
        [Tooltip("Objects that contain placing position for wager")]
        public GameObject[] wager;
        [Tooltip("Objects that hold label marks for the wager")]
        public GameObject[] marks;

        [Header("Poker Models")]
        [Tooltip("Models to repersent dealer's cards")]
        public GameObject[] dealerCardsObj;
        [Tooltip("Models to repersent community cards")]
        public GameObject[] communityCardsObj;
        [Tooltip("Models to repersent player's cards")]
        public GameObject[] playerCardsObj;

        [Header("Prefabs")]
        [Tooltip("Prefab model for red chip")]
        public GameObject pref_redChip;
        [Tooltip("Prefab model for blue chip")]
        public GameObject pref_blueChip;
        [Tooltip("Prefab model for yellow chip")]
        public GameObject pref_yellowChip;
        [Tooltip("Prefab model for pink chip")]
        public GameObject pref_pinkChip;
        [Tooltip("Prefab model for black chip")]
        public GameObject pref_blackChip;

        [HideInInspector]
        public CardDeck cardDeck;                // card deck used in the game
        [HideInInspector]
        public GameManager gameManager;          // the game manager script
        [HideInInspector]
        public PlayerAction playerAction;        // the player action script
        [HideInInspector]
        public LabelController labelController;  // the label controller script
        [HideInInspector]
        public bool allPlayerFolded;             // a state to determine whether or not all the players have folded thier cards

        private Vector3[][] wagerPos;            // contains placing positions for all wager
        private GameObject[][] wagerObj;         // parent object that holds all poker chips model for a specific player
        private Card[] dealerHand;               // card data for dealer
        private Card[] communityCards;           // card data for community cards
        private Card[][] playerCards;            // card data for players
        private HandStrength dealerHandStrengh;  // hand evaluator for dealer
        private HandStrength[] handStrengths;    // hand evaluator for players

        public void Setup()
        {
            InitializeCards();
            InitializeWagerPos();
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
            wagerObj = new GameObject[gameManager.players.Length][];

            // initialize wager positions
            for (int i = 0; i < wagerPos.Length; i++)
            {
                wagerPos[i] = new Vector3[5];
                wagerObj[i] = new GameObject[5];
                for (int j = 0; j < wagerPos.Length; j++)
                    wagerPos[i][j] = wager[i].transform.GetChild(j).transform.position;
            }
        }

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
        /// Methdo to create wager model for a player, wager model repersent the 
        /// amount of money the player bet on a specific part (BONUS/ANTE/FLOP/TURN/RIVER)
        /// </summary>
        /// <param name="playerId">the player id</param>
        /// <param name="wagerIndex">the index of the specific part</param>
        /// <param name="amount">amount of money</param>
        public void CreateWagerModel(int playerId, int wagerIndex, int amount)
        {
            // first of all, create an empty game object that holds the wager models
            wagerObj[playerId][wagerIndex] = new GameObject("Chip");
            wagerObj[playerId][wagerIndex].transform.parent = Blackboard.spawnHolder;
            wagerObj[playerId][wagerIndex].transform.localScale = Vector3.one * 1.75f;
            wagerObj[playerId][wagerIndex].transform.position = wagerPos[playerId][wagerIndex];

            // initialize the spawning data
            var remaining = amount;
            var height = 0f;
            var angle = 0f;

            // keep spawning wager model as long as the remaining amount is greater than 0
            while (remaining > 0)
            {
                // the spawning object
                GameObject obj;

                // case1: black chip
                if (remaining >= 100)
                {
                    remaining -= 100;
                    obj = Instantiate(pref_blackChip, wagerObj[playerId][wagerIndex].transform);
                }
                // case2: pink chip
                else if (remaining >= 50)
                {
                    remaining -= 50;
                    obj = Instantiate(pref_pinkChip, wagerObj[playerId][wagerIndex].transform);
                }
                // case3: yellow chip
                else if (remaining >= 20)
                {
                    remaining -= 20;
                    obj = Instantiate(pref_yellowChip, wagerObj[playerId][wagerIndex].transform);
                }
                // case4: blue chip
                else if (remaining >= 10)
                {
                    remaining -= 10;
                    obj = Instantiate(pref_blueChip, wagerObj[playerId][wagerIndex].transform);
                }
                // default case: red chip
                else
                {
                    remaining = 0;
                    obj = Instantiate(pref_redChip, wagerObj[playerId][wagerIndex].transform);
                }

                // adjust spawned object's position and rotation
                obj.transform.localPosition = new Vector3(0f, height, 0f);
                obj.transform.localEulerAngles = new Vector3(-90f, angle, 0f);

                // increment height & angle
                height += 0.01f;
                angle += 20f;
            }
        }

        /// <summary>
        /// Method to multiply wager on a specific part (BONUS/ANTE/FLOP/TURN/RIVER), it is called
        /// when a player wins
        /// </summary>
        /// <param name="playerId">the player id</param>
        /// <param name="wagerIndex">index of the specific part</param>
        /// <param name="multiplier">multiplier</param>
        public void MultiplyWagerModel(int playerId, int wagerIndex, int multiplier)
        {
            // find the parent object that holds wager models in this specific part
            var parent = wagerObj[playerId][wagerIndex].transform;
            
            // find the childcount so that we know the height of the parent object
            var childCount = parent.transform.childCount;
            var height = parent.GetChild(childCount - 1).localPosition.y;
            var angle = parent.GetChild(childCount - 1).localEulerAngles.y;

            // spawn 'n' times
            for (int i = 0; i < multiplier; i++)
            {
                for (int j = 0; j < childCount; j++)
                {
                    // increment height & angle
                    height += 0.01f;
                    angle += 20f;

                    // spawn a wager model above 
                    var obj = Instantiate(parent.GetChild(j), parent);
                    obj.transform.localPosition = new Vector3(0f, height, 0f);
                    obj.transform.localEulerAngles = new Vector3(-90f, angle, 0f);
                }
            }            
        }

        /// <summary>
        /// Method to remove all wager models, it is called when a round 
        /// is about to reset
        /// </summary>
        public void RemoveWagerModel()
        {
            for (int i = 0; i < wagerObj.Length; i++)
            {
                for (int j = 0; j < wagerObj[i].Length; j++)
                {
                    if (wagerObj[i][j] != null)
                        Destroy(wagerObj[i][j]);
                }
            }
        }

        /// <summary>
        /// Method to hide a player's wager models, cards and label, it is 
        /// called when the player has lost
        /// </summary>
        /// <param name="index"></param>
        private void HidePlayerBetAndCards(int index)
        {
            // remove all chips from this player
            for (int i = 0; i < wagerObj[index].Length; i++)
            {
                if (wagerObj[index][i] != null)
                    Destroy(wagerObj[index][i]);
            }

            // hide card models and bet label
            playerCardsObj[index].HideCard();
            labelController.SetBetLabel(index);
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
                    yield return new WaitForSeconds(Blackboard.WAIT_TIME_DEAL);
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
                yield return new WaitForSeconds(Blackboard.WAIT_TIME_DEAL);
            }

            // dealing community cards
            for (int i = 0; i < 5; i++)
            {
                communityCards[i] = cardDeck.DrawACard();
                communityCardsObj[i].SetCard(communityCards[i]);
                yield return new WaitForSeconds(Blackboard.WAIT_TIME_DEAL);                
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

                    yield return new WaitForSeconds(Blackboard.WAIT_TIME_EMPTY);
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
                        labelController.cardTexture[j].sprite = Blackboard.GetCardSprite(playerCards[i][j].GetCardIndex());
                }

                // recompute the hand-rank
                handStrengths[i].Recompute();

                // if this player is a user-player, update its hand-rank panel 
                if (!gameManager.players[i].isNPC)
                {
                    labelController.title.text = handStrengths[i].GetInitialHandString();
                    labelController.cardTexture[0].sprite = Blackboard.GetCardSprite(playerCards[i][0].GetCardIndex());
                    labelController.cardTexture[1].sprite = Blackboard.GetCardSprite(playerCards[i][1].GetCardIndex());
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
                        labelController.title.text = handStrengths[i].ToString();
                }
            }           
            yield return new WaitForSeconds(Blackboard.WAIT_TIME_DEAL);
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
                yield return new WaitForSeconds(Blackboard.WAIT_TIME_DEAL);
            }

            // recompute dealer's hand-rank
            dealerHandStrengh.Recompute();

            // display dealer's hand-rank label
            labelController.SetHandRankLabelForDealer(true, dealerHandStrengh.ToString("<color=#00FF01><size=80%>"));
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
                return;

            // multiply bonus wager
            MultiplyWagerModel(index, 4, multiplier);

            // reward the player
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
            labelController.SetHandRankLabel(index, true, handStrengths[index].ToString("<color=#00FF01><size=80%>"));

            // if the player is an actual player, hide its hand rank panel UI
            if (!gameManager.players[index].isNPC)
                labelController.SetLocalHandRankPanelVisibility(false);

            // if the player wins
            if (result == Result.Win)
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
            }
            // if the player standoff
            else if (result == Result.Standoff)
            {
                // return wagers to the player
                gameManager.players[index].EditPlayerChip(
                    playerAction.bets[index].anteWager +
                    playerAction.bets[index].flopWager +
                    playerAction.bets[index].turnWager +
                    playerAction.bets[index].riverWager);
            }   
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
            labelController.SetHandRankLabel(index, false);
        }        
    }
}
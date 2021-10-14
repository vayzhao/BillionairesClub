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
        public GameObject group_tags;
        public GameObject group_slots;

        [Header("Player's asset")]
        public GameObject[] chips;
        public GameObject[] wager;
        public GameObject[] hands;
        public GameObject[] marks;

        [Header("Dealer & Community Cards")]
        public GameObject[] dealerCardsObj;
        public GameObject[] communityCardsObj;
        public GameObject[] playerCardsObj;

        [Header("Prefabs")]
        public GameObject pref_redChip;
        public GameObject pref_blueChip;
        public GameObject pref_yellowChip;
        public GameObject pref_pinkChip;
        public GameObject pref_blackChip;

        private Vector3[][] wagerPos;

        private Card[] dealerHand;
        private Card[] communityCards;
        private Card[][] playerCards;
        private HandStrength dealerHandStrengh;
        private HandStrength[] handStrengths;
        private GameObject[][] wagerObj;

        public CardDeck cardDeck;
        public GameManager gameManager;
        public PlayerAction playerAction;
        public LabelController labelController;
        public bool allPlayerFolded;

        public void Setup()
        {
            InitializeCards();
            InitializeWagerPos();
        }

        void InitializeWagerPos()
        {
            wagerPos = new Vector3[gameManager.players.Length][];
            wagerObj = new GameObject[gameManager.players.Length][];
            for (int i = 0; i < wagerPos.Length; i++)
            {
                wagerPos[i] = new Vector3[5];
                wagerObj[i] = new GameObject[5];
                for (int j = 0; j < wagerPos.Length; j++)
                {
                    wagerPos[i][j] = wager[i].transform.GetChild(j).transform.position;
                }
            }
        }

        void InitializeCards()
        {
            cardDeck = new CardDeck();

            dealerHand = new Card[2];
            dealerHandStrengh = new HandStrength();

            playerCards = new Card[gameManager.players.Length][];
            handStrengths = new HandStrength[gameManager.players.Length];
            for (int i = 0; i < playerCards.Length; i++)
            {
                playerCards[i] = new Card[2];
                handStrengths[i] = new HandStrength();
            }
                

            communityCards = new Card[5];
        }

        public void Shuffle()
        {
            for (int i = 0; i < dealerCardsObj.Length; i++)
            {
                dealerCardsObj[i].SetActive(false);
            }
            for (int i = 0; i < communityCardsObj.Length; i++)
            {
                communityCardsObj[i].SetActive(false);
            }


            cardDeck.Shuffle();
            //cardDeck.DebugDeck();
        }

        public void HidePlayerHand()
        {
            labelController.Reset();
            dealerHandStrengh.Reset();
            for (int i = 0; i < handStrengths.Length; i++)
            {
                handStrengths[i].Reset();
            }
        }
        
        public void TurnCardsBack()
        {
            for (int i = 0; i < dealerCardsObj.Length; i++)
            {
                dealerCardsObj[i].transform.localEulerAngles = new Vector3(-90f, 0f, 180f);
            }
            for (int i = 0; i < communityCardsObj.Length; i++)
            {
                communityCardsObj[i].transform.localEulerAngles = new Vector3(-90f, 0f, 180f);
            }
        }

        public void DisplayTableLabel()
        {
            group_tags.SetActive(true);
            group_slots.SetActive(true);

            for (int i = 0; i < gameManager.players.Length; i++)
            {
                if (gameManager.players[i] != null)
                {
                    chips[i].SetActive(true);
                    marks[i].SetActive(true);                    
                }
            }
            
        }

        public void CreateWagerModel(int playerId, int wagerIndex, int amount)
        {
            wagerObj[playerId][wagerIndex] = new GameObject("Chip");
            wagerObj[playerId][wagerIndex].transform.parent = Blackboard.spawnHolder;
            wagerObj[playerId][wagerIndex].transform.localScale = Vector3.one * 1.75f;
            wagerObj[playerId][wagerIndex].transform.position = wagerPos[playerId][wagerIndex];

            var remaining = amount;
            var height = 0f;
            var angle = 0f;
            while (remaining > 0)
            {
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

                obj.transform.localPosition = new Vector3(0f, height, 0f);
                obj.transform.localEulerAngles = new Vector3(-90f, angle, 0f);

                height += 0.01f;
                angle += 20f;
            }
        }
        public void MultiplyWagerModel(int playerId, int wagerIndex, int multiplier)
        {
            var parent = wagerObj[playerId][wagerIndex].transform;
            var childCount = parent.transform.childCount;
            var height = parent.GetChild(childCount - 1).localPosition.y;
            var angle = parent.GetChild(childCount - 1).localEulerAngles.y;

            for (int i = 0; i < multiplier; i++)
            {
                for (int j = 0; j < childCount; j++)
                {
                    height += 0.01f;
                    angle += 20f;

                    var obj = Instantiate(parent.GetChild(j), parent);
                    obj.transform.localPosition = new Vector3(0f, height, 0f);
                    obj.transform.localEulerAngles = new Vector3(-90f, angle, 0f);
                }
            }            
        }


        public void RemoveWagerModel()
        {
            for (int i = 0; i < wagerObj.Length; i++)
            {
                for (int j = 0; j < wagerObj[i].Length; j++)
                {
                    if (wagerObj[i][j] != null)
                    {
                        Destroy(wagerObj[i][j]);
                    }
                }
            }
        }

        private void HidePlayerBetAndCards(int index)
        {
            // remove all chips
            for (int i = 0; i < wagerObj[index].Length; i++)
            {
                if (wagerObj[index][i] != null)
                    Destroy(wagerObj[index][i]);
            }

            // hide cards
            playerCardsObj[index].HideCard();

            // hide bet label
            labelController.SetBetLabel(index);
        }

        public IEnumerator DetermineFoldedPlayer()
        {
            allPlayerFolded = true;

            for (int i = 0; i < gameManager.players.Length; i++)
            {
                if (gameManager.players[i] == null)
                    continue;

                if (playerAction.bets[i].hasFolded)
                {
                    yield return new WaitForSeconds(Blackboard.WAIT_TIME_DEAL);
                    HidePlayerBetAndCards(i);
                }
                else
                {
                    allPlayerFolded = false;
                }
            }
        }


        public IEnumerator DealInitialCards()
        {
            for (int i = 0; i < dealerCardsObj.Length; i++)
            {
                dealerHand[i] = cardDeck.DrawACard();
                dealerCardsObj[i].SetCard(dealerHand[i]);
                yield return new WaitForSeconds(Blackboard.WAIT_TIME_DEAL);
            }

            for (int i = 0; i < 5; i++)
            {
                communityCards[i] = cardDeck.DrawACard();
                communityCardsObj[i].SetCard(communityCards[i]);
                yield return new WaitForSeconds(Blackboard.WAIT_TIME_DEAL);                
            }
        }

        public IEnumerator DealPlayerCards()
        {
            labelController.Display();

            // deal first card
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < gameManager.players.Length; j++)
                {
                    if (gameManager.players[j] != null) 
                    {
                        playerCards[j][i] = cardDeck.DrawACard();
                        playerCardsObj[j].transform.GetChild(i).gameObject.SetCard(playerCards[j][i]);

                        if (!gameManager.players[j].isNPC)
                            labelController.cardTexture[i].enabled = true;

                        yield return new WaitForSeconds(Blackboard.WAIT_TIME_EMPTY);
                    }
                }
            }
        }

        public void RevealPlayerCards()
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < gameManager.players.Length; j++)
                {
                    if (gameManager.players[j] == null)
                        continue;

                    var index = playerCards[j][i].GetCardIndex();
                    handStrengths[j].AddCard(playerCards[j][i]);

                    if (!gameManager.players[j].isNPC) 
                        labelController.cardTexture[i].sprite = Blackboard.GetCardSprite(index);

                }
            }

            for (int i = 0; i < gameManager.players.Length; i++)
            {
                if (gameManager.players[i] == null)
                    continue;

                handStrengths[i].Recompute();

                if (!gameManager.players[i].isNPC)
                    labelController.title.text = handStrengths[i].GetInitialHandString();
                    //labelController.title.text = handStrengths[i].ToString();
            }
        }

        public IEnumerator RevealCommunityCard(int index, bool computeHandStrengh)
        {
            communityCardsObj[index].transform.localEulerAngles = new Vector3(-90, 0, 0);

            for (int i = 0; i < handStrengths.Length; i++)
            {
                handStrengths[i].AddCard(communityCards[index]);
            }

            if (computeHandStrengh)
            {
                for (int i = 0; i < handStrengths.Length; i++)
                {
                    if (gameManager.players[i] == null || playerAction.bets[i].hasFolded)
                        continue;

                    handStrengths[i].Recompute();

                    if (!gameManager.players[i].isNPC)
                        labelController.title.text = handStrengths[i].ToString();

                }
            }           

            yield return new WaitForSeconds(Blackboard.WAIT_TIME_DEAL);
        }

        public IEnumerator RevealDealerHand(int index)
        {
            dealerCardsObj[index].transform.localEulerAngles = new Vector3(-90, 0, 0);
            yield return new WaitForSeconds(Blackboard.WAIT_TIME_DEAL);
        }

        public void ComputeDealerHandStrength()
        {
            dealerHandStrengh.AddCard(dealerHand[0]);
            dealerHandStrengh.AddCard(dealerHand[1]);
            dealerHandStrengh.AddCard(communityCards[0]);
            dealerHandStrengh.AddCard(communityCards[1]);
            dealerHandStrengh.AddCard(communityCards[2]);
            dealerHandStrengh.AddCard(communityCards[3]);
            dealerHandStrengh.AddCard(communityCards[4]);
            dealerHandStrengh.Recompute();
            labelController.SetHandRankLabelForDealer(true, dealerHandStrengh.ToString("<color=#00FF01><size=80%>"));
        }

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
                labelController.HideHandRankPanel();

            if (result == Result.Win)
            {
                var reward = playerAction.bets[index].anteWager;

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
            else if (result == Result.Standoff)
            {
                gameManager.players[index].EditPlayerChip(
                    playerAction.bets[index].anteWager +
                    playerAction.bets[index].flopWager +
                    playerAction.bets[index].turnWager +
                    playerAction.bets[index].riverWager);
            }
                
        }

        public void HidePlayerCards(int index)
        {
            if (index < 0 || gameManager.players[index] == null)
                return;

            HidePlayerBetAndCards(index);
            labelController.SetHandRankLabel(index, false);
        }
        
    }
}
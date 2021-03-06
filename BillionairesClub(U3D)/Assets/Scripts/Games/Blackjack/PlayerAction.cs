using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Blackboard;
using static Const;

namespace Blackjack
{
    using static Para;
    public class PlayerAction : BeforeBet
    {
        [Header("Decision Panel")]
        [Tooltip("A game object that holds hit button")]
        public Button btn_hit;
        [Tooltip("A game object that holds stand button")]
        public Button btn_stand;
        [Tooltip("A game object that holds double down button")]
        public Button btn_double;
        [Tooltip("A game object that holds split button")]
        public Button btn_split;
        [Tooltip("A game object that holds hit/stand/double/split buttons")]
        public GameObject decisionPanel;

        [HideInInspector]
        public Bet[] bets;                      // bet data for players
        [HideInInspector]
        public GameManager gameManager;         // the game manager script
        [HideInInspector]
        public LabelController labelController; // the table controller script
        [HideInInspector]
        public TableController tableController; // the label controller script

        private int handIndex;                  // player's current hand index
        private bool splitted;                  // determine if the player has splitted once
        private bool triggerHit;                // a switchable trigger for hit function
        private bool triggerStand;              // a switchable trigger for stand function
        private bool triggerDouble;             // a switchable trigger for double function
        private bool triggerSplit;              // a switchable trigger for split function
        private bool triggerBust;               // a switchable trigger for bust function
        private bool triggerFiveCards;          // a switchable trigger for fiveCard function

        public void SetUp()
        {
            // initialize bet data for all players
            bets = new Bet[gameManager.players.Length];

            // register delegate method for decisions            
            methodAdd = Add;
            methodBet = Bet;
            methodClear = Clear;
            methodCheck = Check;
        }

        /// <summary>
        /// Method to reset bet data for all players, it is called 
        /// when a new round starts
        /// </summary>
        public void ResetBet()
        {
            // reset bet array's data
            for (int i = 0; i < bets.Length; i++)
                bets[i].Reset();

            // remove all poker chip models from the table
            tableController.ClearWagerStackForAll();
        }

        /// <summary>
        /// Method to display the wager betting panel for a non-npc player
        /// </summary>
        /// <param name="betType">the bet type</param>
        /// <param name="playerIndex">whose term is it</param>
        public void DisplayBetPanel(BetType betType, int playerIndex)
        {
            // switch waiting state to be true and record the playerIndex and bet type
            this.isWaiting = true;
            this.betType = betType;
            this.playerIndex = playerIndex;

            // display asscoated decision panel to the player, depends on the bet type
            switch (betType)
            {
                case BetType.PerfectPair:
                    DisplayWagerPanel("Perfect Pair Wager", gameManager.players[playerIndex].chip);
                    break;
                case BetType.AnteWager:
                    DisplayWagerPanel("Ante Wager", gameManager.players[playerIndex].chip);
                    break;
                case BetType.Insurance:
                    DisplayWagerPanel("Insurance Wager", 
                        Mathf.Min(gameManager.players[playerIndex].chip, bets[playerIndex].anteWager / 2));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Method to display decision panel in the first place, includes hit, 
        /// stand and detect validity for double down and split buttons
        /// </summary>
        void DisplayDecisionPanel(int playerIndex)
        {
            // switch waiting state to be true and record the playerIndex and bet type
            this.isWaiting = true;
            this.splitted = false;
            this.playerIndex = playerIndex;

            // if the player has blackjack, then automatically stand
            if (tableController.GetPlayerHand(playerIndex).HasBlackjack())
            {
                triggerStand = true;
                return;
            }

            // otherwise, display decision panel
            decisionPanel.SetActive(true);

            // check if the player has enough amount of chip to do double down or split
            if (gameManager.players[playerIndex].chip >= bets[playerIndex].anteWager)
            {
                // check double down and split button validity
                var hand = tableController.GetPlayerHand(playerIndex);
                var rank = hand.GetSum();
                btn_double.Switch(rank >= 9 && rank <= 11);
                btn_split.Switch(hand.IsPairSameValue() && !splitted);
            }
        }

        /// <summary>
        /// Method to display/hide decision panel. When a button in the decision panel
        /// is clicked, switch trigger to true and hide the decision panel for a short 
        /// amount of time. When the decision panel shows up again, switch trigger
        /// to false in order to detect player's next input.
        /// </summary>
        /// <param name="trigger">always opposite to panel visibility</param>
        /// <param name="flag">true to display, false to hide</param>
        void SetDecisionPanelVisibility(ref bool trigger, bool flag)
        {
            // switch trigger and panel's visibility
            trigger = !flag;
            decisionPanel.SetActive(flag);

            // when displaying the panel
            if (flag)
            {
                // switch off split button
                btn_split.Switch(false);

                // check to see if double down button should be enabled
                btn_double.Switch(IsDoubleDownable(
                    tableController.GetPlayerHand(playerIndex), handIndex));
            }
        }

        /// <summary>
        /// detect whether or not the player need to stop hitting, the player
        /// stops hitting when the hand is bust, blackjack or five card charlie
        /// charlie
        /// </summary>
        void DetectBreaker()
        {
            // get the player current hand
            var hand = tableController.GetPlayerHand(playerIndex);

            if (hand.HasBust(handIndex)) 
            {
                SetDecisionPanelVisibility(ref triggerBust, false);
                labelController.LocalPanelTransparency(handIndex, TRANSPARENCE_STAND);
            }
            else if (hand.HasFiveCardCharlie(handIndex))
            {
                SetDecisionPanelVisibility(ref triggerFiveCards, false);
                labelController.LocalPanelTransparency(handIndex, TRANSPARENCE_STAND);                
            }
            else if (hand.HasBlackjack(handIndex))
            {
                SetDecisionPanelVisibility(ref triggerStand, false);
            }
        }

        /// <summary>
        /// Method to determine whether or not the player should finish his turn,
        /// usually, the player will immediately finishes his turn when he gets 
        /// bust, blackjack or five card charlie, except he has two hands to play
        /// </summary>
        void DetermineFinisher()
        {
            // clear current hand 
            tableController.ClearSinglePlayerSingleHand(playerIndex, handIndex);

            // get player's hand data
            var hand = tableController.GetPlayerHand(playerIndex);

            // reset player's global hand label background
            labelController.playerHandLabel[playerIndex].bg.sprite =
                labelController.labelSpriteTransparent;

            // if player's hands are all cleared or stood, finish the turn
            if (hand.HasAllClear() || hand.HasAllStand())
                FinishTurn();
            else
            {
                // otherwise, set handIndex to be the remaining hand
                handIndex = !hand.stand[0] ? 0 : 1;

                // display decision panel and update local hand panel's transparency
                if (!gameManager.players[playerIndex].isNPC) 
                {
                    SetDecisionPanelVisibility(ref triggerStand, true);
                    labelController.LocalPanelTransparency(handIndex, TRANSPARENCE_NORMAL);
                }
            }
        }

        /// <summary>
        /// Method to determine whether or not a specific hand is able to double down
        /// </summary>
        /// <param name="hand">hand data</param>
        /// <param name="handIndex">hand index</param>
        /// <returns></returns>
        bool IsDoubleDownable(Hand hand, int handIndex)
        {
            var sum = hand.GetSum(handIndex);
            var sumSoft = hand.GetSumSoft(handIndex);
            var count = hand.GetCardCount(handIndex);
            return count == 2 && ((sum >= 9 && sum <= 11) || (sumSoft >= 9 && sumSoft <= 11));
        }

        /// <summary>
        /// Decision function for double down button
        /// </summary>
        void DoubleDown()
        {
            // add one card to the player
            tableController.OnPlayerHit(playerIndex, 0);
            audioManager.PlayAudio(audioManager.clipPlaceWager, AudioType.Sfx);

            // cost player's wager
            bets[playerIndex].doubleWager = bets[playerIndex].anteWager;
            tableController.CloneAnteWagerStack(playerIndex, WAGER_INDEX_DOUBLE, false);
            gameManager.players[playerIndex].EditPlayerChip(-bets[playerIndex].doubleWager);
            labelController.UpdateBetLabel(playerIndex, bets[playerIndex]);

            // if the player is an actual player, run stand trigger, 
            // otherwise run stand code
            if (!gameManager.players[playerIndex].isNPC)
                StandTrigger();
            else
                Stand();
        }
        public void DoubleDownTrigger()
        {
            // this part is function when the player clicks the button
            if (!triggerDouble)
            {
                SetDecisionPanelVisibility(ref triggerDouble, false);
                return;
            }

            // after that trigger will be switched off 
            // and execute the following codes
            triggerDouble = false;

            // execute double down code
            DoubleDown();
        }

        /// <summary>
        /// Decision function for hit button
        /// </summary>
        void Hit()
        {
            // add one card to the player
            tableController.OnPlayerHit(playerIndex, handIndex);
        }
        public void HitTrigger()
        {
            // this part is functioning when the player clicks the button
            if (!triggerHit)
            {
                // hide decision panel and switch on trigger
                SetDecisionPanelVisibility(ref triggerHit, false);
                return;
            }

            // execute hit code
            Hit();

            // switch off the trigger and display the panel again
            SetDecisionPanelVisibility(ref triggerHit, true);

            // detect whether or not the player need to stop hitting
            DetectBreaker();
        }

        /// <summary>
        /// Decision function for stand button
        /// </summary>
        void Stand()
        {
            // set player statu to be stand and play check sound effect
            tableController.GetPlayerHand(playerIndex).stand[handIndex] = true;
            audioManager.PlayAudio(audioManager.clipCheck, AudioType.Sfx);

            // update hand rank panel text
            labelController.playerHandLabel[playerIndex].tmp.text =
                tableController.GetPlayerHand(playerIndex).ToString();

            // if the player stands the first hand, finish the turn
            if (handIndex == 0)
                FinishTurn();
            else
            {
                // otherwise, set handIndex to be 0 and the player 
                // start playing his second hand
                handIndex = 0;
            }
        }
        public void StandTrigger()
        {
            // this part is functioning when the player clicks the button
            if (!triggerStand)
            {
                // hide decision panel and switch on trigger
                SetDecisionPanelVisibility(ref triggerStand, false);
                return;
            }

            // after that trigger will be switched off 
            // and execute the following codes
            triggerStand = false;

            // set local hand panel to be half-transparent
            labelController.LocalPanelTransparency(handIndex, TRANSPARENCE_STAND);

            // execute stand code
            Stand();

            // if the players turn has not finished yet and the handIndex is 0
            // display decision panel again and set first hand label(local) to
            // be transparent
            if (isWaiting && handIndex == 0)
            {
                SetDecisionPanelVisibility(ref triggerStand, true);
                labelController.LocalPanelTransparency(0, TRANSPARENCE_NORMAL);
            }
        }
        
        /// <summary>
        /// Decision function for split button
        /// </summary>
        void Split()
        {
            // split player's hand
            tableController.OnPlayerSplit(playerIndex);

            // cost player's wager
            bets[playerIndex].anteWagerSplit = bets[playerIndex].anteWager;
            tableController.CloneAnteWagerStack(playerIndex, WAGER_INDEX_BONUS_SPLITE_WAGER, false);
            gameManager.players[playerIndex].EditPlayerChip(-bets[playerIndex].anteWagerSplit);

            // update player's bet label and hand label
            labelController.UpdateBetLabel(playerIndex, bets[playerIndex]);

            // play split sound effect
            audioManager.PlayAudio(audioManager.clipDealCards, AudioType.Sfx);

            // set handIndex to be 1 
            handIndex = 1;
            splitted = true;
        }
        public void SplitTrigger()
        {
            // this part is functioning when the player clicks the button
            if (!triggerSplit)
            {
                // hide decision panel and switch on trigger
                SetDecisionPanelVisibility(ref triggerSplit, false);
                return;
            }

            // after that trigger will be switched off and display
            // decision panel again
            SetDecisionPanelVisibility(ref triggerSplit, true);

            // execute split code
            Split();
        }

        /// <summary>
        /// An IEnumerator that keeps running while player is making his decisions
        /// </summary>
        /// <param name="playerIndex">index of the player</param>
        /// <returns></returns>
        public IEnumerator Deciding(int playerIndex)
        {
            // initialize handIndex
            handIndex = 0;

            // display decision panel
            DisplayDecisionPanel(playerIndex);

            while (isWaiting)
            {
                if (triggerHit)
                {
                    yield return new WaitForSeconds(WAIT_TIME_DECISION);
                    HitTrigger();
                }

                if (triggerStand)
                {
                    yield return new WaitForSeconds(WAIT_TIME_DECISION);
                    StandTrigger();
                }

                if (triggerDouble)
                {
                    yield return new WaitForSeconds(WAIT_TIME_DECISION);
                    DoubleDownTrigger();
                }

                if (triggerSplit)
                {
                    yield return new WaitForSeconds(WAIT_TIME_DECISION);
                    SplitTrigger();
                }

                if (triggerBust)
                {
                    yield return new WaitForSeconds(WAIT_TIME_DECISION);
                    tableController.Bust(playerIndex, handIndex);
                    triggerBust = false;            
                    yield return new WaitForSeconds(WAIT_TIME_COMPARE_FAST);
                    DetermineFinisher();    
                }

                if (triggerFiveCards)
                {
                    yield return new WaitForSeconds(WAIT_TIME_DECISION);
                    tableController.FiveCardCharlie(playerIndex, handIndex);
                    triggerFiveCards = false;
                    yield return new WaitForSeconds(WAIT_TIME_COMPARE_FAST);
                    DetermineFinisher();                    
                }

                yield return new WaitForSeconds(Time.deltaTime);
            }            
        }

        /// <summary>
        /// An IEnumerator that keeps running while NPC is making his decisions
        /// </summary>
        /// <param name="playerIndex">index of the player</param>
        /// <returns></returns>
        public IEnumerator DecidingAI(int playerIndex)
        {
            // initialize playerIndex & handIndex
            handIndex = 0;
            this.isWaiting = true;
            this.splitted = false;
            this.playerIndex = playerIndex;

            // get the AI player's hand
            var hand = tableController.GetPlayerHand(playerIndex);

            while (isWaiting)
            {
                // check to see if the bot has blackjack, five card charlie
                // or bust that will possibily terminate the turn
                if (hand.HasBlackjack(handIndex))
                {
                    Stand();
                    continue;
                }
                else if (hand.HasFiveCardCharlie(handIndex))
                {
                    tableController.FiveCardCharlie(playerIndex, handIndex);
                    yield return new WaitForSeconds(WAIT_TIME_COMPARE_FAST);
                    DetermineFinisher();
                    continue;
                }
                else if (hand.HasBust(handIndex))
                {
                    tableController.Bust(playerIndex, handIndex);
                    yield return new WaitForSeconds(WAIT_TIME_COMPARE_FAST);
                    DetermineFinisher();
                    continue;
                }

                yield return new WaitForSeconds(WAIT_TIME_SHORTPAUSE);

                // if not, here's the Ai decision logic starts, first find the 
                // sum and card count of bot's current hand
                // AI decision logic, in this part, bot has a hand with values
                var sum = hand.GetSum(handIndex);
                var count = hand.GetCardCount(handIndex);

                // enum behaviour possibilities
                var lookForFiveCard = count == 4 && sum <= 18;
                var doubleDownAble = count == 2 && sum >= 9 && sum <= 11;
                var splitAble = count == 2 && !splitted && hand.IsPairSameValue();

                // double down when the sum is 10 or 11
                if (doubleDownAble && (sum >= 10))
                    DoubleDown();
                // split when the start pair is 1/1 - 9/9
                else if (splitAble && (sum <= 18))
                    Split();
                // hit when the sum is lessthan 16 or
                // wait for 1 more card to get five card charlie
                else if (sum < 16 || lookForFiveCard)
                    Hit();           
                // otherwise stand
                else
                    Stand();
            }
        }

        /// <summary>
        /// Method to finish a turn for a player, switch isWaiting to false
        /// so that the coroutine can continue to move to the next player
        /// also, hide the wager panel & decision panel
        /// </summary>
        void FinishTurn()
        {
            isWaiting = false;            
            wagerPanel.SetActive(false);
            decisionPanel.SetActive(false);
            StopCoroutine(RightClickDetect());
        }

        /// <summary>
        /// Method for players to bet on ante wager, it is called from
        /// the button click event in the Game UI
        /// </summary>
        /// <param name="value">the bet wager value</param>
        void BetAnteWager(int value)
        {
            // store value into bet data
            bets[playerIndex].anteWager = value;

            // consume player's chip
            gameManager.players[playerIndex].EditPlayerChip(-value);
            labelController.betLabels[playerIndex].Switch(true);
            labelController.UpdateBetLabel(playerIndex, bets[playerIndex]);
        }
        public void BetAnteWagerAI(int playerIndex)
        {
            // randomly select an value for ante wager ($10~$300)
            var value = chipValues[0] * UnityEngine.Random.Range(2, 60);

            // create some poker chip models to represent the bot's wager
            tableController.InstantiateWagerStack(playerIndex, WAGER_INDEX_ANTE, value);

            // repeat excatly how it functions in player's version
            this.playerIndex = playerIndex;
            BetAnteWager(value);
        }

        /// <summary>
        /// Method for players to bet on perfect pair wager, it is called 
        /// from the button click event in the Game UI
        /// </summary>
        /// <param name="value">the bet wager value</param>
        void BetPerfectPair(int value)
        {
            // store value into the bet data
            bets[playerIndex].perfectPairWager = value;

            // consume player's chip
            gameManager.players[playerIndex].EditPlayerChip(-value);
            labelController.UpdateBetLabel(playerIndex, bets[playerIndex]);
        }
        public void BetPerfectPairAI(int playerIndex)
        {
            // randomly select an value for perfect pair wager ($5~$150)
            var value = chipValues[0] * UnityEngine.Random.Range(1, 30);

            // create some poker chip models to represent the bot's wager
            tableController.InstantiateWagerStack(playerIndex, WAGER_INDEX_BONUS_SPLITE_WAGER, value);

            // repeat excatly how it functions in player's version
            this.playerIndex = playerIndex;
            BetPerfectPair(value);
        }

        /// <summary>
        /// Method for players to bet on insurance wager, it is called 
        /// from the button click event in the Game UI
        /// </summary>
        /// <param name="value">the bet wager value</param>
        void BetInsuranceWager(int value)
        {
            // store value into the bet data
            bets[playerIndex].insuranceWager = value;

            // consume player's chip
            gameManager.players[playerIndex].EditPlayerChip(-value);
            labelController.insuranceBets[playerIndex].Display($"{bets[playerIndex].insuranceWager:C0}");
        }
        public void BetInsuranceWagerAI(int playerIndex)
        {
            // randomly select an value for insurance wager (up to half of ante)
            var maxQty = bets[playerIndex].anteWager / chipValues[0] / 2;
            var value = chipValues[0] * UnityEngine.Random.Range(1, maxQty);

            // create some poker chip models to represent the bot's wager
            tableController.InstantiateWagerStack(playerIndex, WAGER_INDEX_INSURANCE, value);

            // repeat excatly how it functions in player's version
            this.playerIndex = playerIndex;
            BetInsuranceWager(value);
        }

        /// <summary>
        /// Method for players to bet
        /// </summary>
        new void Bet()
        {
            // finish this turn
            FinishTurn();

            // determine which bet turn it is
            switch (betType)
            {
                case BetType.PerfectPair:
                    BetPerfectPair(totalWager);
                    break;
                case BetType.AnteWager:
                    BetAnteWager(totalWager);
                    break;
                case BetType.Insurance:
                    BetInsuranceWager(totalWager);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Method for players to add a single chip into the wager pool
        /// </summary>
        /// <param name="value">value of the added chip</param>
        new void Add(int value)
        {
            // if this is the first chip being added, swithc on reset button
            if (totalWager == 0)
            {
                btn_reset.Switch(true);

                // if this is a ante wager bet, switch on bet button
                if (betType == BetType.AnteWager)
                    btn_bet.Switch(true);

                // if this is a bonud wager bet, display bet button 
                // and hide skip button
                else if (betType == BetType.PerfectPair || betType == BetType.Insurance)
                {
                    btn_bet.gameObject.SetActive(true);
                    btn_skip.gameObject.SetActive(false);
                }
            }

            // update the wager and remaining info
            totalWager += value;
            remainingTemp -= value;
            wagerText.text = $"{totalWager:C0}";

            // add single wager model to the associate slot
            if (betType == BetType.AnteWager)
                tableController.AddWagerModel(playerIndex, WAGER_INDEX_ANTE, value);
            else if (betType == BetType.PerfectPair)
                tableController.AddWagerModel(playerIndex, WAGER_INDEX_BONUS_SPLITE_WAGER, value);
            else if (betType == BetType.Insurance)
                tableController.AddWagerModel(playerIndex, WAGER_INDEX_INSURANCE, value);

            // refresh chip's validity
            RefreshChipValidity(remainingTemp);
        }

        /// <summary>
        /// Method for the player to reset wager in 'before bet' section
        /// </summary>
        new void Clear()
        {
            // reset total wager & remaining
            totalWager = 0;
            remainingTemp = remaining;

            // switch off reset button and hide other button
            btn_reset.Switch(false);
            btn_bet.gameObject.SetActive(false);
            btn_skip.gameObject.SetActive(false);

            // if it is ante wager bet
            if (betType == BetType.AnteWager)
            {
                // reset bet button
                btn_bet.gameObject.SetActive(true);
                btn_bet.Switch(false);
                wagerText.text = "";

                // clear all wagers in ante wager slot
                tableController.ClearWagerStackForSingleSlot(playerIndex, WAGER_INDEX_ANTE);
            }
            // if it is a bonud wager bet
            else if (betType == BetType.PerfectPair)
            {
                // display skip button
                btn_skip.gameObject.SetActive(true);

                // clear all wagers in bonus wager slot
                tableController.ClearWagerStackForSingleSlot(playerIndex, WAGER_INDEX_BONUS_SPLITE_WAGER);
            }
            // if it is an insurance wager bet
            else if (betType == BetType.Insurance)
            {
                // display skip button
                btn_skip.gameObject.SetActive(true);

                // clear all wagers in insurance wager slot
                tableController.ClearWagerStackForSingleSlot(playerIndex, WAGER_INDEX_INSURANCE);
            }

            // refresh chip's validity
            RefreshChipValidity(remainingTemp);
        }

        /// <summary>
        /// Method for players to skip
        /// </summary>
        new void Check()
        {
            // finish this turn
            FinishTurn();

            // play check sound effect
            Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipCheck, AudioType.Sfx);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blackjack
{
    using static Para;
    public class Hand 
    {
        public bool[] stand;                 // determine whether or not the player has stood
        public bool[] clear;                 // determine whether or not the player has been cleared
        public HandRank[] rank;              // rank of a hand
        private int[] sum;                   // sum of a hand
        private int[] sumSoft;               // sum of a hand (with ace to be 11)
        private int[] cardCount;             // card count of a hand
        private Card[][] cards;              // cards data of a hand
        private float perfectPairMultiplier; // reward multipier for perfect pair
        
        public Hand()
        {
            // initialize essential variables
            sum = new int[MAX_HAND];
            sumSoft = new int[MAX_HAND];
            cardCount = new int[MAX_HAND];
            stand = new bool[MAX_HAND];
            clear = new bool[MAX_HAND];
            rank = new HandRank[MAX_HAND];
            cards = new Card[MAX_HAND][];
            for (int i = 0; i < cards.Length; i++)
                cards[i] = new Card[MAX_CARD_PER_HAND];
        }
        
        /// <summary>
        /// Method to reset hand, it is called at the start of 
        /// the round
        /// </summary>
        public void Reset()
        {
            // reset all the essential variables
            for (int i = 0; i < MAX_HAND; i++)
            {
                sum[i] = 0;
                cardCount[i] = 0;
                sumSoft[i] = 0;
                stand[i] = false;
                clear[i] = false;
                rank[i] = HandRank.Value;
            }

            // set the second hand to be clear and stand by default
            clear[1] = true;
            stand[1] = true;

            // reset perfect pair reward multipiler
            perfectPairMultiplier = 0;

            // clean cache for all cards
            for (int i = 0; i < MAX_HAND; i++)
                for (int j = 0; j < cards[i].Length; j++)
                    cards[i][j] = null;
        }

        /// <summary>
        /// Method to insert a revealed card into the card array
        /// </summary>
        /// <param name="newCard">the adding card</param>
        /// <param name="handIndex">index of the hand</param>
        public void AddCard(Card newCard, int handIndex = 0)
        {
            cards[handIndex][cardCount[handIndex]] = newCard;
            cardCount[handIndex] += 1;

            Recompute(handIndex);
        }

        /// <summary>
        /// Method for the player to split his hand
        /// </summary>
        public void SplitHand()
        {
            // move second card of first hand to first 
            // card of second hand
            cards[1][0] = cards[0][1];
            cards[0][1] = null;

            // set the second hand to be unclear and no stand
            clear[1] = false;
            stand[1] = false;

            // reset card count for both hand and recompute
            for (int i = 0; i < MAX_HAND; i++)
            {
                cardCount[i] = 1;
                Recompute(i);
            }
        }

        /// <summary>
        /// Method to obtain the strength for a specific hand
        /// </summary>
        /// <param name="handIndex">index of the hand</param>
        /// <returns></returns>
        public int GetSum(int handIndex = 0) => sum[handIndex];
        public int GetSumSoft(int handIndex = 0) => sumSoft[handIndex];
        public int GetHighestSum(int handIndex = 0) => Mathf.Max(sum[handIndex], sumSoft[handIndex]);
        public bool HasBlackjack(int handIndex = 0) => rank[handIndex] == HandRank.Blackjack;
        public bool HasBust(int handIndex = 0) => rank[handIndex] == HandRank.Bust;
        public bool HasFiveCardCharlie(int handIndex = 0) => rank[handIndex] == HandRank.FiveCardCharlie;
        public Card GetCard(int handIndex, int cardIndex) => cards[handIndex][cardIndex];
        public bool HasAllClear() => clear[0] && clear[1];
        public bool HasAllStand() => stand[0] && stand[1];

        /// <summary>
        /// Method to obtain the card count in a specific hand
        /// </summary>
        /// <param name="handIndex">index of the hand</param>
        /// <returns></returns>
        public int GetCardCount(int handIndex = 0) => cardCount[handIndex];

        /// <summary>
        /// Method to compute the hand-rank for a hand
        /// </summary>
        /// <param name="handIndex">index of the hand</param>
        public void Recompute(int handIndex = 0)
        {
            // detect blackjack if the hand currently has 2 cards only
            if (cardCount[handIndex] == 2)
                DetectBlackjack(handIndex);

            // skip the recomputation if the player already has blackjack
            if (HasBlackjack(handIndex))
                return;

            // otherwise, sum the player hand's cards
            sum[handIndex] = 0;
            for (int i = 0; i < cardCount[handIndex]; i++)
                sum[handIndex] += cards[handIndex][i].value.ToInt();

            // check to see if this is a soft hand, if it is, add an
            // alternate option to the player
            if (IsSoftHand(handIndex))
                sumSoft[handIndex] = sum[handIndex] + 10;
            else
                sumSoft[handIndex] = 0;

            // if the rank is over 21, the hand is bust
            if (sum[handIndex] > 21)
                rank[handIndex] = HandRank.Bust;

            // otherwise, if the player has 5 cards without exceeding 21
            // the player gets a five card charlie
            else if (cardCount[handIndex] == 5)
                rank[handIndex] = HandRank.FiveCardCharlie;
        }

        /// <summary>
        /// Method to detect whether or not the player has blackjack by
        /// finding an ace and a picture from the first two cards
        /// </summary>
        /// <param name="handIndex"></param>
        void DetectBlackjack(int handIndex = 0)
        {
            // run through the first two cards and see if there
            // is an ace and a picture
            var hasAce = false;
            var hasPicture = false;
            for (int i = 0; i < cardCount[handIndex]; i++)
            {
                switch (cards[handIndex][i].value)
                {
                    case Value.ACE:
                        hasAce = true;
                        break;
                    case Value.JACK:
                    case Value.QUEEN:
                    case Value.KING:
                        hasPicture = true;
                        break;
                    default:
                        break;
                }
            }

            // if the player has blackjack, set rank to be 21
            if (hasAce && hasPicture)
            {
                sum[handIndex] = 21;
                rank[handIndex] = HandRank.Blackjack;
            }
        }

        /// <summary>
        /// Method to detect if the player's hand is a soft hand
        /// e.g: a soft hand contains an ace without the other card(s)
        /// totaling 10 or more
        /// </summary>
        /// <param name="handIndex">index of the hand</param>
        /// <returns></returns>
        bool IsSoftHand(int handIndex)
        {
            // run through all cards in the hand and sum their values,
            // also check if there is an ace in the hand
            var total = 0;
            var hasAce = false;
            for (int i = 0; i < cardCount[handIndex]; i++)
            {
                if (cards[handIndex][i].value == Value.ACE && !hasAce)
                    hasAce = true;
                else
                    total += cards[handIndex][i].value.ToInt();
            }
            return hasAce && total <= 10;
        }

        /// <summary>
        /// Method to determine whether or not the player has a pair
        /// </summary>
        /// <returns></returns>
        public bool IsPairSameValue() => cards[0][0].SameValue(cards[0][1]);
        public bool IsSameSuit() => cards[0][0].SameSuit(cards[0][1]);
        public bool IsSameColor() => cards[0][0].SameColor(cards[0][1]);

        /// <summary>
        /// Method to setup calculate & fetch perfect pair reward mutiplier
        /// </summary>
        public void CalculatePerfectPairReward()=> perfectPairMultiplier = IsSameSuit() ? REWARD_PP_SAMESUIT : IsSameColor() ? REWARD_PP_SAMECOLOR : REWARD_PP_NORMAL;
        public int GetPerfectPairMultiplier() => (int)perfectPairMultiplier;

        /// <summary>
        /// Method for a player to compare his hand to the dealer's hand
        /// if one's hand rank is greater than the others, return result
        /// if their hand ranks are equal, compare their highest value
        /// otherwise, standoff
        /// </summary>
        /// <param name="handIndex">index of the hand</param>
        /// <param name="dealer">dealer's hand</param>
        /// <returns></returns>
        public Result CompareToDealer(int handIndex, Hand dealer)
        {
            // dealer has 1 hand only
            var dealerHandIndex = 0;

            // when two hands are not equal, return either win or lose
            if (rank[handIndex] != dealer.rank[dealerHandIndex])
                return rank[handIndex] > dealer.rank[dealerHandIndex] ? Result.Win : Result.Lose;

            // if both hands are Rank.value, who ever has the higher value, wins
            if (rank[handIndex] == HandRank.Value &&
                dealer.rank[dealerHandIndex] == HandRank.Value)
            {
                var playerValue = GetHighestSum(handIndex);
                var dealerValue = dealer.GetHighestSum(0);
                return playerValue > dealerValue ? Result.Win : playerValue < dealerValue ? Result.Lose : Result.Standoff;
            }

            // if all the comparison above are passed, then the result can be standoff only
            return Result.Standoff;
        }

        /// <summary>
        /// Conver player's hand to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "";

            // run through player's hands
            for (int i = 0; i < MAX_HAND; i++)
            {
                // in the second iteration, check to see if the 
                // player has a second hand, it he does, add a prefix
                // to the result, otherwise break the loop
                if (i == 1)
                    if (!clear[i])
                        result += "-";
                    else
                        break;
                result += ToString(i);
            }
            return result;
        }
        public string ToString(int handIndex)
        {
            // check what combination the player has
            if (HasBlackjack(handIndex))
                return "Blackjack";
            else if (HasBust(handIndex))
                return "Bust";
            else if (HasFiveCardCharlie(handIndex))
                return "5 Cards";
            else
            {
                // if this hand is value rank, check if it is stood.
                // when stood, only return the highest value
                if (stand[handIndex])
                    return $"{GetHighestSum(handIndex)}";
                else
                    return sumSoft[handIndex] > 0 ? $"{sum[handIndex]}/{sumSoft[handIndex]}" : $"{sum[handIndex]}";
            }
        }
    }
}
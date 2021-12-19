using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blackjack
{
    using static Para;
    public class Hand 
    {
        public bool[] stand;               // 
        public HandRank[] handRank;        //
        private int[] rank;                //
        private int[] rankSoft;            //
        private int[] cardCount;           //
        private Card[][] cards;            //
        private int perfectPairMultiplier; //
        
        public Hand()
        {
            rank = new int[MAX_HAND];
            rankSoft = new int[MAX_HAND];
            cardCount = new int[MAX_HAND];
            cards = new Card[MAX_HAND][];
            stand = new bool[MAX_HAND];
            handRank = new HandRank[MAX_HAND];

            for (int i = 0; i < cards.Length; i++)
                cards[i] = new Card[MAX_CARD_PER_HAND];
        }
        
        /// <summary>
        /// Method to reset hand, it is called at the start of 
        /// the round
        /// </summary>
        public void Reset()
        {
            perfectPairMultiplier = 0;

            for (int i = 0; i < MAX_HAND; i++)
            {
                rank[i] = 0;
                cardCount[i] = 0;
                rankSoft[i] = 0;
                stand[i] = false;
                handRank[i] = HandRank.Value;
            }

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
        /// Method to obtain the strength for a specific hand
        /// </summary>
        /// <param name="handIndex">index of the hand</param>
        /// <returns></returns>
        public int GetRank(int handIndex = 0) => rank[handIndex];
        public int GetRankSoft(int handIndex = 0) => rankSoft[handIndex];
        public int GetHighestRank(int handIndex = 0) => Mathf.Max(rank[handIndex], rankSoft[handIndex]);
        public bool HasBlackjack(int handIndex = 0) => handRank[handIndex] == HandRank.Blackjack;
        public bool HasBust(int handIndex = 0) => handRank[handIndex] == HandRank.Bust;
        public bool HasFiveCardCharlie(int handIndex = 0) => handRank[handIndex] == HandRank.FiveCardCharlie;

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
                DetectBlackjack();

            // skip the recomputation if the player already has blackjack
            if (HasBlackjack(handIndex))
                return;

            // otherwise, sum the player hand's cards
            rank[handIndex] = 0;
            for (int i = 0; i < cardCount[handIndex]; i++)
                rank[handIndex] += GetPoint(cards[handIndex][i].value);

            // check to see if this is a soft hand, if it is, add an
            // alternate option to the player
            if (IsSoftHand(handIndex))
                rankSoft[handIndex] = rank[handIndex] + 10;
            else
                rankSoft[handIndex] = 0;

            // if the rank is over 21, the hand is bust
            if (rank[handIndex] > 21)
                handRank[handIndex] = HandRank.Bust;

            // otherwise, if the player has 5 cards without exceeding 21
            // the player gets a five card charlie
            else if (cardCount[handIndex] == 5)
                handRank[handIndex] = HandRank.FiveCardCharlie;
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
                rank[handIndex] = 21;
                handRank[handIndex] = HandRank.Blackjack;
            }
        }

        /// <summary>
        /// Method to detect if the player's hand is a soft hand
        /// e.g: a soft hand contains an ace without the other card(s)
        /// totaling 10 or more
        /// </summary>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        bool IsSoftHand(int handIndex)
        {
            int total = 0;
            bool hasAce = false;

            for (int i = 0; i < cardCount[handIndex]; i++)
            {
                if (cards[handIndex][i].value == Value.ACE && !hasAce)
                {
                    hasAce = true;
                }
                else
                {
                    total += GetPoint(cards[handIndex][i].value);
                }
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
        public void CalculatePerfectPairReward()=> perfectPairMultiplier = IsSameSuit() ? 30 : IsSameColor() ? 10 : 5;
        public int GetPerfectPairMultiplier() => perfectPairMultiplier;

        /// <summary>
        /// Method to translate a card's value to point
        /// </summary>
        /// <param name="value">card's value</param>
        /// <returns></returns>
        int GetPoint(Value value)
        {
            switch (value)
            {
                case Value.JACK:
                case Value.QUEEN:
                case Value.KING:
                    return 10;
                case Value.ACE:
                    return 1;
                default:
                    return ((int)value) + 2;
            }
        }

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
            if (handRank[handIndex] != dealer.handRank[dealerHandIndex])
                return handRank[handIndex] > dealer.handRank[dealerHandIndex] ? Result.Win : Result.Lose;

            // if both hands are Rank.value, who ever has the higher value, wins
            if (handRank[handIndex] == HandRank.Value &&
                dealer.handRank[dealerHandIndex] == HandRank.Value)
            {
                var playerValue = GetHighestRank(handIndex);
                var dealerValue = dealer.GetHighestRank(handIndex);
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
                    if (cardCount[i] > 0)
                        result += "-";
                    else
                        break;

                // check what combination the player has
                if (HasBlackjack(i))
                    result += "Blackjack";
                else if (HasBust(i))
                    result += "Bust";
                else if (HasFiveCardCharlie(i))
                    result += "5 Cards";
                else
                {
                    // if this hand is value rank, check if it is stood.
                    // when stood, only return the highest value
                    if (stand[i])
                    {
                        result += $"{GetHighestRank(i)}";
                    }
                    else
                    {
                        result += $"{rank[i]}";
                        if (rankSoft[i] > 0)
                            result += $"/{rankSoft[i]}";
                    }
                }
            }
            return result;
        }
    }
}
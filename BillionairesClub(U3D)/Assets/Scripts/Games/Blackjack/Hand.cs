using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blackjack
{
    public class Hand 
    {
        private int[] rank;
        private int[] pointer;
        private Card[][] cards;
        private int perfectPairMultiplier;

        public Hand()
        {
            rank = new int[2];
            pointer = new int[2];
            cards = new Card[2][];

            for (int i = 0; i < cards.Length; i++)
                cards[i] = new Card[5];
        }

        /// <summary>
        /// Method to reset hand, it is called at the start of 
        /// the round
        /// </summary>
        public void Reset()
        {
            rank[0] = 0;
            rank[1] = 0;
            pointer[0] = 0;
            pointer[1] = 0;

            for (int i = 0; i < cards.Length; i++)
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
            cards[handIndex][pointer[handIndex]] = newCard;
            pointer[handIndex] += 1;

            Recompute(handIndex);
        }

        /// <summary>
        /// Method to obtain the value from this hand strength
        /// </summary>
        /// <param name="handIndex">index of the hand</param>
        /// <returns></returns>
        public int GetRank(int handIndex = 0) => rank[handIndex];

        /// <summary>
        /// Method to compute the hand-rank for a hand
        /// </summary>
        /// <param name="handIndex">index of the hand</param>
        public void Recompute(int handIndex = 0)
        {
            rank[handIndex] = 0;
            for (int i = 0; i < cards[handIndex].Length; i++)
            {
                if (cards[handIndex][i] != null)
                    rank[handIndex] += GetPoint(cards[handIndex][i].value);
            }
        }

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
    }
}
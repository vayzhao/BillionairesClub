using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TexasBonus
{
    public class HandStrength
    {
        private List<Card> cards;
        private Rank rank;
        private Value value;
        private Value subValue;
        private Suit suit;
        private Card[] bestHand;

        public HandStrength()
        {
            cards = new List<Card>();
            bestHand = new Card[5];
        }

        public void Reset()
        {
            // remove all cards cache
            cards.Clear();

            // reset hand rank
            rank = Rank.HighHand;
            
            // set best hand to be initial
            for (int i = 0; i < bestHand.Length; i++)
                bestHand[i] = null;
        }

        /// <summary>
        /// Method to insert a revealed card into the card list
        /// </summary>
        /// <param name="newCard"></param>
        public void AddCard(Card newCard)
        {
            cards.Add(newCard);
        }

        /// <summary>
        /// Method to compute the hand-rank for the player
        /// </summary>
        public void Recompute()
        {
            if (StartHand()) 
                return;
            if (rank <= Rank.OnePair) 
                CheckOnePair();
            if (rank <= Rank.TwoPairs) 
                CheckTwoPairs();
            if (rank <= Rank.ThreeOfAKind) 
                CheckThreeOfAKind();
            if (rank <= Rank.Straight) 
                CheckStraight();
            if (rank <= Rank.Flush) 
                CheckFlush();
            if (rank <= Rank.FullHouse) 
                CheckFullHouse();
            if (rank <= Rank.FourOfAKind) 
                CheckFourOfAKind();
            if (rank <= Rank.StraightFlush) 
                CheckStraightFlush();
            FindBestHand();
        }

        /// <summary>
        /// Method to determine if the player has a straight flush
        /// </summary>
        private void CheckStraightFlush()
        {
            // it is impossible to have straight flush when the player's hand is fullhouse or four of a kind
            if (rank == Rank.FullHouse || rank == Rank.FourOfAKind)
                return;

            // find if the player has the same suit of card * 5
            var flush = cards.GroupBy(x => x.suit).Where(x => x.Count() >= 5);

            // if a flush is found, try tof find a straight then
            if (flush.Any())
            {
                // get the flush cards
                suit = flush.Single().Key;
                var flushCards = cards.Select(x => x).Where(x => x.suit == suit).OrderByDescending(x => x.value).ToArray();

                // initialize straight count & value
                var straightCount = 1;
                var straightValue = flushCards[0].value;

                // run through each element in the flush cards and see if they are straight
                for (int i = 0; i < flushCards.Length; i++)
                {
                    // check to see if the 'n' element is excatly 1 geater than the next element
                    if (flushCards[i].value - flushCards[i + 1].value == 1)
                    {
                        // increment straight count
                        straightCount++;

                        // when straight count is equal to 5, the player gets a straight
                        if (straightCount == 5)
                        {
                            value = straightValue;
                            rank = value == Value.ACE ? Rank.RoyalFlush : Rank.StraightFlush;
                            break;
                        }
                    }
                    // otherwise the straight breaks, check to see if the array has at least 5 remaining cards that are unchecked,
                    // if it does, reset straight count and straight value
                    else if (flushCards.Length - (i + 1) >= 5)
                    {
                        straightCount = 1;
                        straightValue = flushCards[i + 1].value;
                    }
                    // otherwise break the loop
                    else
                    {                        
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Method to determine if the player has a quads
        /// </summary>
        private void CheckFourOfAKind() 
        {
            // it is impossible to have quads when the player already has full house, straight or flush
            if (rank >= Rank.Straight)
                return;

            // find quads in the cards
            var quads = cards.GroupBy(x => x.value).Where(x => x.Count() > 3);

            // when a trips is found, store the biggest value of the trips
            if (quads.Any())
            {
                rank = Rank.FourOfAKind;
                value = quads.Max(x => x.Key);
            }
        }

        /// <summary>
        /// Method to determine if the player has a full house
        /// </summary>
        private void CheckFullHouse() 
        {
            // it is impossible to get a full house if the hand strengh is less than three of a kind
            if (rank < Rank.ThreeOfAKind)
                return;

            // find pairs within the cards
            var pairs = cards.GroupBy(x => x.value).Where(x => x.Count() > 1);

            // it is also not possible to have a full house if the hand does not have at least two pairs
            if (pairs.Count() < 2) 
                return;

            // if there is any trips within the two pairs, then we get a full house
            if (pairs.Any(x => x.Count() >= 3)) 
            {
                rank = Rank.FullHouse;
                value = pairs.Where(x => x.Count() >= 3).Max(x => x.Key);
                subValue = pairs.Where(x => x.Key != value).Max(x => x.Key);
            }
        }

        /// <summary>
        /// Method to determine if the player has a flush
        /// </summary>
        private void CheckFlush() 
        {
            // find if the player has the same suit of card * 5
            var flushes = cards.GroupBy(x => x.suit).Where(x => x.Count() >= 5);

            // when a flush is found, store the biggest value
            if (flushes.Any())
            {
                rank = Rank.Flush;
                suit = flushes.Single().Key;
                value = cards.Where(x => x.suit == suit).Max(x => x.value);
            }
        }

        /// <summary>
        /// Method to determine if the player has a straight
        /// </summary>
        private void CheckStraight() 
        {
            // first, sort the cards by descending order and convert it to an array
            var sortCards = cards.OrderByDescending(x => x.value).ToArray();

            // initialize straight count & straight value
            var straightCount = 1;
            var straightValue = sortCards[0].value;

            // run from the first element to the second last element in the sortCard array
            for (int i = 0; i < sortCards.Length - 1; i++)
            {
                // check to see if this element's value is exactly 1 greater than the next 
                if (((int)(sortCards[i].value - sortCards[i + 1].value)) == 1) 
                {
                    // increment straight count
                    straightCount++;
                    
                    // when straight count is equal to 5, the player gets a straight
                    if (straightCount == 5)
                    {
                        rank = Rank.Straight;
                        value = straightValue;
                        break;
                    }
                }
                // otherwise the straight breaks, check to see if the array has at least 5 remaining cards that are unchecked,
                // if it does, reset straight count and straight value
                else if (sortCards.Length - (i + 1) >= 5)
                {
                    straightCount = 1;
                    straightValue = sortCards[i + 1].value;
                }
                // otherwise break the for loop
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Method to determine if the player has a trips
        /// </summary>
        private void CheckThreeOfAKind() 
        {
            // it is impossible to get a trips if the hand rank is less than one pair
            if (rank < Rank.OnePair)
                return;

            // find trips in the cards
            var trips = cards.GroupBy(x => x.value).Where(x => x.Count() > 2);

            // when a trips is found, store the biggest value of the trips
            if (trips.Any())
            {
                rank = Rank.ThreeOfAKind;
                value = trips.Max(x => x.Key);
            }
        }

        /// <summary>
        /// Method to determine if the player has two pairs
        /// </summary>
        private void CheckTwoPairs() 
        {
            // find pairs in the cards
            var pairs = cards.GroupBy(x => x.value).Where(x => x.Count() > 1);

            // when 2 or more than 2 pairs are found, store the value of the biggest pairs
            if (pairs.Count() >= 2) 
            {
                rank = Rank.TwoPairs;
                value = pairs.Max(x => x.Key);
                subValue = pairs.Where(x => x.Key != value).Max(x => x.Key);
            }

        }

        /// <summary>
        /// Method to determine if the player has one pair
        /// </summary>
        private void CheckOnePair() 
        {
            // find pairs in the cards
            var pairs = cards.GroupBy(x => x.value).Where(x => x.Count() > 1);

            // when any pairs found, store the value of the biggest pair
            if (pairs.Any())
            {
                rank = Rank.OnePair;
                value = pairs.Max(x => x.Key);
            }
        }


        private void FindBestHand()
        {
            switch (rank)
            {
                case Rank.HighHand:
                    BestHandHighHand();
                    break;
                case Rank.OnePair:
                    BestHandOnePair();
                    break;
                case Rank.TwoPairs:
                    BestHandTwoPair();
                    break;
                case Rank.ThreeOfAKind:
                    BestHandThreeOfAKind();
                    break;
                case Rank.Straight:
                    BestHandStraight();
                    break;
                case Rank.Flush:
                    BestHandFlush();
                    break;
                case Rank.FullHouse:
                    BestHandFullHouse();
                    break;
                case Rank.FourOfAKind:
                    BestHandFourOfAKind();
                    break;
                case Rank.StraightFlush:
                    BestHandStraightFlush();
                    break;
                case Rank.RoyalFlush:
                    BestHandStraightFlush();
                    break;
                default:
                    break;
            }
        }
        private void BestHandStraightFlush()
        {
            var straightFlush = cards.Select(x => x).Where(x => x.suit == suit && x.value <= value).OrderByDescending(x => x.value).Distinct().Take(5).ToArray();
            bestHand[0] = straightFlush[0];
            bestHand[1] = straightFlush[1];
            bestHand[2] = straightFlush[2];
            bestHand[3] = straightFlush[3];
            bestHand[4] = straightFlush[4];
        }
        private void BestHandFourOfAKind()
        {
            var quads = cards.Select(x => x).Where(x => x.value == value).ToArray();
            var rest = cards.Select(x => x).Where(x => x.value != value).OrderByDescending(x => x.value).First();
            bestHand[0] = quads[0];
            bestHand[1] = quads[1];
            bestHand[2] = quads[2];
            bestHand[3] = quads[3];
            bestHand[4] = rest;
        }
        private void BestHandFullHouse()
        {
            var trips = cards.Select(x => x).Where(x => x.value == value).ToArray();
            var pair = cards.Select(x => x).Where(x => x.value == subValue).ToArray();
            bestHand[0] = trips[0];
            bestHand[1] = trips[1];
            bestHand[2] = trips[2];
            bestHand[3] = pair[0];
            bestHand[4] = pair[1];
        }
        private void BestHandFlush()
        {
            var flush = cards.Select(x => x).Where(x => x.suit == suit).OrderByDescending(x => x.value).Take(5).ToArray();
            bestHand[0] = flush[0];
            bestHand[1] = flush[1];
            bestHand[2] = flush[2];
            bestHand[3] = flush[3];
            bestHand[4] = flush[4];
        }
        private void BestHandStraight()
        {
            var straight = cards.Select(x => x).Where(x => x.value <= value).OrderByDescending(x => x.value).Distinct().Take(5).ToArray();
            bestHand[0] = straight[0];
            bestHand[1] = straight[1];
            bestHand[2] = straight[2];
            bestHand[3] = straight[3];
            bestHand[4] = straight[4];
        }
        private void BestHandThreeOfAKind()
        {
            var trips = cards.Select(x => x).Where(x => x.value == value).ToArray();
            var rest = cards.Select(x => x).Where(x => x.value != value).OrderByDescending(x => x.value).Take(2).ToArray();
            bestHand[0] = trips[0];
            bestHand[1] = trips[1];
            bestHand[2] = trips[2];
            bestHand[3] = rest[0];
            bestHand[4] = rest[1];
        }
        private void BestHandTwoPair()
        {
            var pair = cards.Select(x => x).Where(x => x.value == value).ToArray();
            var subPair = cards.Select(x => x).Where(x => x.value == subValue).ToArray();
            var rest = cards.Select(x => x).Where(x => x.value != value && x.value != subValue).OrderByDescending(x => x.value).First();
            bestHand[0] = pair[0];
            bestHand[1] = pair[1];
            bestHand[2] = subPair[0];
            bestHand[3] = subPair[1];
            bestHand[4] = rest;
        }
        private void BestHandOnePair()
        {
            var pair = cards.Select(x => x).Where(x => x.value == value).ToArray();
            var rest = cards.Select(x => x).Where(x => x.value != value).OrderByDescending(x => x.value).Take(3).ToArray();
            bestHand[0] = pair[0];
            bestHand[1] = pair[1];
            bestHand[2] = rest[0];
            bestHand[3] = rest[1];
            bestHand[4] = rest[2];
        }
        private void BestHandHighHand()
        {
            bestHand = cards.OrderByDescending(x => x.value).Take(5).ToArray();
        }

        /// <summary>
        /// Method to determine player's starting hand strengh
        /// </summary>
        /// <returns></returns>
        private bool StartHand() 
        {
            // return false if the card count is greater than 2
            if (cards.Count > 2)
                return false;

            // determine the starting hand strengh
            rank = cards[0].SameValue(cards[1]) ? Rank.OnePair : Rank.HighHand;

            // set value to be the max value in start hand
            value = Card.Max(cards);

            return true; 
        }

        /// <summary>
        /// Method to convert hand strengh into a friendly string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return rank.GetName() + "\n" + GetBestHandString();
        }
        public string ToString(string color)
        {
            return rank.GetName() + "\n" + color + GetBestHandString();
        }
        public string GetInitialHandString()
        {
            switch (rank)
            {
                case Rank.OnePair:
                    return rank.GetName() + $"({value.GetName()})";
                default:
                    return rank.GetName() + $"({value.GetName()})";
            }
        }
        public string GetBestHandString()
        {
            return $"({bestHand[0].value.GetName(true)}" +
                $"+{bestHand[1].value.GetName(true)}" +
                $"+{bestHand[2].value.GetName(true)}" +
                $"+{bestHand[3].value.GetName(true)}" +
                $"+{bestHand[4].value.GetName(true)})";
        }
    }
}
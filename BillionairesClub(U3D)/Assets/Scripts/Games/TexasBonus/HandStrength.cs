using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TexasBonus
{
    public class HandStrength
    {
        public Rank rank;            // determine the level of this hand 
        private Suit suit;           // suit of the combination (it is only used when the hand-rank is flush or straight flush)
        private Value value;         // the biggest value of this hand
        private Value subValue;      // the second biggest value of this hand
        private Card[] bestHand;     // the 5 best card out of 7 cards
        private List<Card> cards;    // the 7 cards (2 player's hand + 5 community cards)
        private int bonusMultiplier; // the bonus multiplier 

        public HandStrength()
        {
            cards = new List<Card>();
            bestHand = new Card[5];
        }

        /// <summary>
        /// Method to reset hand strength calculate, it is called at
        /// the start of every round
        /// </summary>
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
        /// Method to obtain the value from this hand strength
        /// </summary>
        public Value GetValue() => value;

        /// <summary>
        /// Method to compute the hand-rank for the player
        /// </summary>
        public void Recompute()
        {
            // return if the player has 2 cards only (in this case community cards are not revealed)
            if (StartHand()) 
                return;

            // find the highest hand-rank within current hand
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

            // when highest hand-rank is found, find the best 5 cards within current hand
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

            // if a flush is found, check to see if there is a straight within the flush
            if (flush.Any())
            {
                // get the flush cards and sort them in ascending order
                suit = flush.Single().Key;
                var flushCards = cards.Select(x => x).Where(x => x.suit == suit).OrderBy(x => x.value).ToArray();

                // initialize straight count & value
                var straightCount = 1;
                var straightValue = flushCards[0].value;

                // run from the first element to the second last element in the flushCards and see if there is any straight
                for (int i = 0; i < flushCards.Length - 1; i++)
                {
                    // check to see if the 'n' element is excatly 1 less than the next element
                    if ((flushCards[i + 1].value - flushCards[i].value) == 1)
                    {
                        // if it is, increase the straight count and update the straight value
                        straightCount++;
                        straightValue = flushCards[i + 1].value;
                        
                        // when reaches the end, return the straight flush
                        if (i == flushCards.Length - 2)
                        {
                            value = straightValue;
                            rank = value == Value.ACE ? Rank.RoyalFlush : Rank.StraightFlush;
                            break;
                        }
                    }
                    // otherwise, check to see if the straight count is equal or greater than 5
                    else if (straightCount >= 5)
                    {
                        value = straightValue;
                        rank = value == Value.ACE ? Rank.RoyalFlush : Rank.StraightFlush;
                        break;
                    }
                    // otherwise, check to see if the straight count is 4 and the last value is 5
                    else if (straightCount == 4 && straightValue == Value.FIVE)
                    {
                        // run through the remaining cards and check if there is any ACE
                        for (int j = i + 1; j < flushCards.Length; j++)
                        {
                            if (flushCards[j].value == Value.ACE)
                            {
                                rank = Rank.StraightFlush;
                                value = straightValue;
                                break;
                            }
                        }
                        // if the player has 2-3-4-5 but does not have ACE, it is impossible to get another straight
                        return;
                    }
                    // otherwise, check if there are more than 5 unchecked cards remaining
                    // if yes, reset the straight count and straight value
                    else if (flushCards.Length - (i + 1) >= 5) 
                    {
                        straightCount = 1;
                        straightValue = flushCards[i + 1].value;
                    }
                    // otherwise break the for loop as it is impossible to get a straight
                    else
                    {
                        break;
                    }
                }
            }
        }
        private void BestHandStraightFlush()
        {
            // find all cards which has a lower value and same suit with the straight value, and sort then by descending order
            // if the straight value is 5, take the first 4 of them
            // otherwise, take the first 5 of them
            var straightFlush = cards.Select(x => x).Where(x => x.suit == suit && x.value <= value).OrderByDescending(x => x.value).Distinct().ToArray();

            // setup the first best 4 cards
            bestHand[0] = straightFlush[0];
            bestHand[1] = straightFlush[1];
            bestHand[2] = straightFlush[2];
            bestHand[3] = straightFlush[3];

            // if the straight value is 5, set the last element to be an ACE
            if (value == Value.FIVE)
                bestHand[4] = cards.Select(x => x).Where(x => x.value == Value.ACE).First();
            else
                bestHand[4] = straightFlush[4];
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
        private void BestHandFlush()
        {
            var flush = cards.Select(x => x).Where(x => x.suit == suit).OrderByDescending(x => x.value).Take(5).ToArray();
            bestHand[0] = flush[0];
            bestHand[1] = flush[1];
            bestHand[2] = flush[2];
            bestHand[3] = flush[3];
            bestHand[4] = flush[4];
        }

        /// <summary>
        /// Method to determine if the player has a straight
        /// </summary>
        private void CheckStraight()
        {
            // first, sort the cards by ascending order and distince
            var sortCards = cards.OrderBy(x => x.value).Distinct().ToArray();

            // initialize straight count & straight value
            var straightCount = 1;
            var straightValue = sortCards[0].value;

            // run from the first element to the second last element in the sortCard array
            for (int i = 0; i < sortCards.Length - 1; i++)
            {
                // check to see if this element's value is excatly 1 less than the next                 
                if ((sortCards[i + 1].value - sortCards[i].value) == 1)
                {
                    // if it is, increase the straight count and update the straight value
                    straightCount++;
                    straightValue = sortCards[i + 1].value;

                    // when reaches the end, return the straight
                    if (i == sortCards.Length - 2)
                    {
                        rank = Rank.Straight;
                        value = straightValue;
                        return;
                    }
                }
                // otherwise, check to see if the straight count is equal or greater than 5
                else if (straightCount >= 5)
                {
                    rank = Rank.Straight;
                    value = straightValue;
                    break;
                }
                // otherwise, check to see if the straight count is 4 and the last value is 5
                else if (straightCount == 4 && straightValue == Value.FIVE)
                {
                    // if it is, run through the remaining cards and check if there is any ACE
                    for (int j = i + 1; j < sortCards.Length; j++)
                    {
                        if (sortCards[j].value == Value.ACE)
                        {
                            rank = Rank.Straight;
                            value = straightValue;
                            break;
                        }
                    }
                    // if the player has 2-3-4-5 but does not have ACE, it is impossible to get another straight
                    return;
                }
                // otherwise, check if there are more than 5 unchecked cards remain
                // if yes, reset the straight count and straight value
                else if (sortCards.Length - (i + 1) >= 5) 
                {
                    straightCount = 1;
                    straightValue = sortCards[i + 1].value;
                }
                // otherwise break the for loop as it is impossible to get a straight
                else
                {
                    break;
                }
            }
        }
        private void BestHandStraight()
        {
            // find all cards which has a lower value than the straight value and sort then by descending order
            // if the straight value is 5, take the first 4 of them
            // otherwise, take the first 5 of them
            var straight = cards.Select(x => x).Where(x => x.value <= value).OrderByDescending(x => x.value).Distinct().ToArray();

            // setup the first best 4 cards
            bestHand[0] = straight[0];
            bestHand[1] = straight[1];
            bestHand[2] = straight[2];
            bestHand[3] = straight[3];

            // if the straight value is 5, set the last element to be an ACE
            if (value == Value.FIVE)
                bestHand[4] = cards.Select(x => x).Where(x => x.value == Value.ACE).First();
            // otherwise the last best card will be the 5th element in 'straight'
            else
                bestHand[4] = straight[4];
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

            // find the value and sub value
            var sortedHand = cards.OrderByDescending(x => x.value).ToArray();
            value = sortedHand[0].value;
            subValue = sortedHand[1].value;

            // calculate bonus mulitplier
            CalculateBonus();

            return true; 
        }

        /// <summary>
        /// Method to calculate player's bonus multiplier, it is called
        /// when the first 2 cards are revealed
        /// </summary>
        private void CalculateBonus()
        {
            // initialize multiplier
            bonusMultiplier = 0;

            // high-hand bonus (Ace + King/Queen/Jack)
            if (rank == Rank.HighHand && value == Value.ACE)
            {
                // determine whether or not the first 2 cards are same suit
                var isSameSuit = cards[0].suit == cards[1].suit;

                // ACE + KING
                if (subValue == Value.KING)
                {
                    bonusMultiplier = isSameSuit ? 25 : 15;
                }
                // ACE + QUEEN / JACK
                else if (subValue == Value.QUEEN || subValue == Value.JACK) 
                {
                    bonusMultiplier = isSameSuit ? 20 : 5;
                }
            }
            // pair bonus
            else if (rank == Rank.OnePair)
            {
                // pair of ACE
                if (value == Value.ACE)
                {
                    bonusMultiplier = 30;
                }
                // pair of KING / QUEEN / JACK
                else if (value == Value.KING || value == Value.QUEEN || value == Value.JACK)
                {
                    bonusMultiplier = 10;
                }
                // any pair, other than a pair of aces, kings, queens or jacks
                else
                {
                    bonusMultiplier = 3;
                }
            }
        }
        public int GetBonusMultiplier() { return bonusMultiplier; }

        /// <summary>
        /// Method to compare player's hand and dealer's hand,
        /// if one's hand rank is greater than the others, return result
        /// if their hand-ranks are equal, compare the value of the bestHand
        /// if their bestHand have the same value, return standoff
        /// </summary>
        /// <param name="player"></param>
        /// <param name="dealer"></param>
        /// <returns></returns>
        public static Result Compare(HandStrength player, HandStrength dealer)
        {
            // when two hands are not equal, return either win or lose
            if (player.rank != dealer.rank)
                return player.rank > dealer.rank ? Result.Win : Result.Lose;

            // in this case, two hands are equal, we need to check through all
            // the elements in bestHand to determine which hand is greater than 
            // the other
            for (int i = 0; i < 5; i++)
            {
                // skip when the 'n' element's value is equal
                if (player.bestHand[i].value == dealer.bestHand[i].value)
                    continue;

                // otherwise, return either win or lose
                return player.bestHand[i].value > dealer.bestHand[i].value ? Result.Win : Result.Lose;
            }

            // if all the comparison above are passed, then the result can be standoff only
            return Result.Standoff;
        }

        /// <summary>
        /// Method to convert hand strengh into a friendly string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return rank.GetName() + "\n" + GetBestHandString();
        }
        public string ToString(string prefix)
        {
            return rank.GetName() + "\n" + prefix + GetBestHandString();
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
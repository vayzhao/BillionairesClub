using System;
using System.Collections.Generic;
using UnityEngine;

public class Card : IEquatable<Card>
{
    public Suit suit;   // suit of this card
    public Value value; // value of this card

    public Card(Suit suit, Value value)
    {
        this.suit = suit;
        this.value = value;
    }

    public int GetCardIndex() { return ((int)suit * 13) + (int)value; }

    public bool SameValue(Card other) { return this.value == other.value; }

    public bool SameSuit(Card other) { return this.suit == other.suit; }

    public bool SameColor(Card other) { return this.suit.GetColor() == other.suit.GetColor(); }

    /// <summary>
    /// Method to find the card that has the maximum value within a list of cards
    /// </summary>
    /// <param name="cards">the list of cards</param>
    /// <returns></returns>
    public static Value Max(List<Card> cards)
    {
        // initialize index & max value
        var index = 0;
        var maxValue = -1;
        for (int i = 0; i < cards.Count; i++)
        {
            if ((int)cards[i].value > maxValue)
            {
                index = i;
                maxValue = (int)cards[i].value;                
            }
        }
        return cards[index].value;
    }

    public bool Equals(Card other) => value.Equals(other.value);
    public override int GetHashCode() => value.GetHashCode();

    public override string ToString() => $"{suit.ToString()}-{value.ToString()}";
}
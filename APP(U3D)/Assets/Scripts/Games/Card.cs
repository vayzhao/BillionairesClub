using System;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public Suit suit;
    public Value value;
    public GameObject gameObj;

    public Card(Suit suit, Value value)
    {
        this.suit = suit;
        this.value = value;
    }

    public int GetCardIndex()
    {
        return ((int)suit * 13) + (int)value;
    }
    
    public bool SameValue(Card other)
    {
        return this.value == other.value;
    }

    public bool SameSuit(Card other)
    {
        return this.suit == other.suit;
    }

    public static Value Max(List<Card> cards)
    {
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

}
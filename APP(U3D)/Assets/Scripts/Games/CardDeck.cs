using System;
using System.Collections.Generic;
using UnityEngine;

public class CardDeck
{
    private List<Card> cards;
    private List<Card> usedCards;

    public CardDeck()
    {
        cards = new List<Card>();
        usedCards = new List<Card>();

        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            foreach (Value value in Enum.GetValues(typeof(Value)))
            {
                cards.Add(new Card(suit, value));
            }
        }
    }

    public void DebugDeck()
    {
        cards = new List<Card>();
        usedCards = new List<Card>();

        cards.Add(new Card(Suit.Diamond,Value.TWO));
        cards.Add(new Card(Suit.Diamond, Value.THREE));
        cards.Add(new Card(Suit.Diamond, Value.TEN));
        cards.Add(new Card(Suit.Diamond, Value.JACK));
        cards.Add(new Card(Suit.Diamond, Value.QUEEN));
        cards.Add(new Card(Suit.Diamond, Value.KING));
        cards.Add(new Card(Suit.Diamond, Value.ACE));
        cards.Add(new Card(Suit.Diamond, Value.EIGHT));
        cards.Add(new Card(Suit.Diamond, Value.NINE));
        cards.Add(new Card(Suit.Diamond, Value.TWO));
        cards.Add(new Card(Suit.Diamond, Value.THREE));
        cards.Add(new Card(Suit.Diamond, Value.TEN));
        cards.Add(new Card(Suit.Diamond, Value.JACK));
        cards.Add(new Card(Suit.Diamond, Value.QUEEN));
        cards.Add(new Card(Suit.Diamond, Value.KING));
        cards.Add(new Card(Suit.Diamond, Value.ACE));
        cards.Add(new Card(Suit.Diamond, Value.EIGHT));
        cards.Add(new Card(Suit.Diamond, Value.NINE));
    }

    public void Shuffle()
    {
        var temp = new List<Card>();


        for (int i = 0; i < usedCards.Count; i++)
        {
            cards.Add(usedCards[0]);
            usedCards.RemoveAt(0);
        }

        while (cards.Count > 0)
        {
            var card = cards[UnityEngine.Random.Range(0, cards.Count)];
            cards.Remove(card);
            temp.Add(card);
        }

        cards = temp;
    }

    public void PrintCardDeck()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Debug.Log($"{i} card{cards[i].suit} with {cards[i].value}");
        }
    }

    public Card DrawACard()
    {
        var card = cards[0];
        cards.Remove(card);
        usedCards.Add(card);
        return card;
    }

}
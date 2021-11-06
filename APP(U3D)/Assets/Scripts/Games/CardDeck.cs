using System;
using System.Collections.Generic;
using UnityEngine;

public class CardDeck
{ 
    private List<Card> cards;     // list of card that repersent the card-deck
    private List<Card> usedCards; // list of card that has been dealt

    public CardDeck()
    {
        cards = new List<Card>();
        usedCards = new List<Card>();

        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            foreach (Value value in Enum.GetValues(typeof(Value)))
                cards.Add(new Card(suit, value));

        Shuffle();
    }

    /// <summary>
    /// Method for setting up a card deck for debug purpose
    /// </summary>
    public void DebugDeck()
    {
        cards = new List<Card>();
        usedCards = new List<Card>();

        cards.Add(new Card(Suit.Heart, Value.KING));
        cards.Add(new Card(Suit.Heart, Value.QUEEN));
        cards.Add(new Card(Suit.Heart, Value.EIGHT));
        cards.Add(new Card(Suit.Heart, Value.JACK));
        cards.Add(new Card(Suit.Heart, Value.QUEEN));
        cards.Add(new Card(Suit.Heart, Value.KING));
        cards.Add(new Card(Suit.Heart, Value.ACE));
        cards.Add(new Card(Suit.Heart, Value.TEN));
        cards.Add(new Card(Suit.Heart, Value.NINE));
    }

    /// <summary>
    /// Method for shuffling a card deck
    /// </summary>
    public void Shuffle()
    {
        // first of all, put every card into the usedCard list
        for (int i = 0; i < cards.Count; i++)
        {
            usedCards.Add(cards[0]);
            cards.RemoveAt(0);
        }

        // then, randomly pick a card from usedCard list and put it back to card list
        while (usedCards.Count > 0)
        {
            // randomly select a card
            var card = usedCards[UnityEngine.Random.Range(0, usedCards.Count)];

            // add it to card list & remove it from usedCard list
            cards.Add(card);
            usedCards.Remove(card);
        }
    }

    /// <summary>
    /// Method to draw the first card from the card-deck
    /// </summary>
    /// <returns></returns>
    public Card DrawACard()
    {
        // find the first card 
        var card = cards[0];

        // revmoe it from the card list & add it to usedCard list
        cards.Remove(card);
        usedCards.Add(card);

        return card;
    }
}
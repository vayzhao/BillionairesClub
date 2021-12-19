using System;
using System.Collections.Generic;
using UnityEngine;

public class CardDeck
{ 
    private List<Card> cards;     // list of card that repersent the card-deck
    private int pointerIndex;     // the index of current drawing card
    private System.Random rd;     // the random generator for card shuffling
    
    public CardDeck()
    {
        cards = new List<Card>();
        rd = new System.Random();

        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            foreach (Value value in Enum.GetValues(typeof(Value)))
                cards.Add(new Card(suit, value));
    }

    public CardDeck(int multiplier)
    {
        cards = new List<Card>();
        rd = new System.Random();

        for (int i = 0; i < multiplier; i++)
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                foreach (Value value in Enum.GetValues(typeof(Value)))
                    cards.Add(new Card(suit, value));
    }

    /// <summary>
    /// Method for setting up a card deck for debug purpose
    /// </summary>
    public void DebugDeck_TexasBonus()
    {
        cards = new List<Card>();
        pointerIndex = 0;
        cards.Add(new Card(Suit.Heart, Value.ACE)); // dealer hand 1
        cards.Add(new Card(Suit.Heart, Value.ACE)); // dealer hand 2
        cards.Add(new Card(Suit.Heart, Value.ACE)); // flop1
        cards.Add(new Card(Suit.Spade, Value.THREE)); // flop2
        cards.Add(new Card(Suit.Diamond, Value.FOUR)); // flop3
        cards.Add(new Card(Suit.Spade, Value.FIVE)); // turn
        cards.Add(new Card(Suit.Diamond, Value.SIX)); // river
        cards.Add(new Card(Suit.Heart, Value.SEVEN)); // player's hand 1
        cards.Add(new Card(Suit.Heart, Value.ACE)); // player's hand 2
    }
    public void DebugDeck_Blackjack()
    {
        cards = new List<Card>();
        pointerIndex = 0;
        for (int i = 0; i < 20; i++)
        {
            cards.Add(new Card(Suit.Heart, Value.ACE)); // player
            cards.Add(new Card(Suit.Heart, Value.ACE)); // dealer
            cards.Add(new Card(Suit.Heart, Value.ACE)); // player 
            cards.Add(new Card(Suit.Spade, Value.KING)); // dealer
            cards.Add(new Card(Suit.Diamond, Value.KING)); // player or dealer
            cards.Add(new Card(Suit.Spade, Value.TWO)); // player or dealer
            cards.Add(new Card(Suit.Diamond, Value.TWO)); // player or dealer
            cards.Add(new Card(Suit.Heart, Value.TWO)); // player or dealer
            cards.Add(new Card(Suit.Heart, Value.ACE)); // player or dealer
            cards.Add(new Card(Suit.Heart, Value.ACE)); // player or dealer
        }
        
    }

    /// <summary>
    /// Method for shuffling a card deck
    /// </summary>
    public void Shuffle()
    {
        // reset pointer index
        pointerIndex = 0;

        // play shuffle sound effect
        Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipShuffling, AudioType.Sfx);

        // shuffle the card deck
        for (int i = cards.Count - 1; i > 0; i--)
        {
            // randomly pick a card
            var j = rd.Next(0, i);
            Card c = cards[j];

            // and swap the selected card with i card
            cards[j] = cards[i];
            cards[i] = c;
        }
    }

    /// <summary>
    /// Method to draw the 'n' card from the card-deck
    /// </summary>
    /// <returns></returns>
    public Card DrawACard()
    {
        // find the 'n' card 
        var card = cards[pointerIndex];

        // increment the pointer index
        pointerIndex++;

        return card;
    }

    /// <summary>
    /// Method to get number of remaining cards in the card deck
    /// </summary>
    /// <returns></returns>
    public int GetRemaining() => cards.Count - pointerIndex;
}
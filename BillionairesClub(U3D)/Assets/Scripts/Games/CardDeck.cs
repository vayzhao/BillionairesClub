﻿using System;
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

    /// <summary>
    /// Method for setting up a card deck for debug purpose
    /// </summary>
    public void DebugDeck()
    {
        cards = new List<Card>();
        cards.Add(new Card(Suit.Heart, Value.KING));
        cards.Add(new Card(Suit.Heart, Value.QUEEN));
        cards.Add(new Card(Suit.Heart, Value.FIVE));
        cards.Add(new Card(Suit.Spade, Value.SIX));
        cards.Add(new Card(Suit.Diamond, Value.SEVEN));
        cards.Add(new Card(Suit.Spade, Value.SEVEN));
        cards.Add(new Card(Suit.Diamond, Value.SEVEN));
        cards.Add(new Card(Suit.Heart, Value.FOUR));
        cards.Add(new Card(Suit.Heart, Value.THREE));
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
}
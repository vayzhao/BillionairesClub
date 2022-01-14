using System;
using System.Collections.Generic;
using UnityEngine;

public class CardDeck
{
    const int CARD_SUIT_COUNT = 4;      // number of suit in a card deck
    const int CARD_VALUE_COUNT = 13;    // number of value in a suit
    const int CARD_COUNT_PER_DECK = 52; // total number of cards in a card deck

    private Card[] cards;               // array of card that repersent the card-deck
    private int cardIndex;              // the index of current drawing card
    private System.Random rd;           // the random generator for card shuffling

    public CardDeck()
    {
        // initialize the card deck array and random generator
        cards = new Card[CARD_COUNT_PER_DECK];
        rd = new System.Random();

        // setting up every elemenet in the card deck
        for (int i = 0; i < CARD_SUIT_COUNT; i++)
            for (int j = 0; j < CARD_VALUE_COUNT; j++)
                cards[i * CARD_VALUE_COUNT + j] = new Card((Suit)i, (Value)j);
    }

    public CardDeck(int multiplier)
    {
        // initialize the card deck array and random generator
        cards = new Card[CARD_COUNT_PER_DECK * multiplier];
        rd = new System.Random();

        // setting up every elemenet in the card deck
        for (int i = 0; i < multiplier; i++) 
            for (int j = 0; j < CARD_SUIT_COUNT; j++)
                for (int k = 0; k < CARD_VALUE_COUNT; k++)
                    cards[j * CARD_VALUE_COUNT + k] = new Card((Suit)j, (Value)k);
    }

    /// <summary>
    /// Method for setting up a card deck for debug purpose
    /// </summary>
    public void DebugDeck_TexasBonus()
    {
        cards = new Card[9];
        cardIndex = 0;
        cards[0] = new Card(Suit.Heart, Value.ACE); // dealer's first card
        cards[1] = new Card(Suit.Heart, Value.ACE); // dealer's second card
        cards[2] = new Card(Suit.Heart, Value.ACE); // 1st community card
        cards[3] = new Card(Suit.Heart, Value.ACE); // 2nd community card
        cards[4] = new Card(Suit.Heart, Value.ACE); // 3rd community card
        cards[5] = new Card(Suit.Heart, Value.ACE); // 4th community card
        cards[6] = new Card(Suit.Heart, Value.ACE); // 5th community card
        cards[7] = new Card(Suit.Heart, Value.ACE); // player's first card
        cards[8] = new Card(Suit.Heart, Value.ACE); // player's second card
    }
    public void DebugDeck_Blackjack()
    {
        cards = new Card[9];
        cardIndex = 0;
        cards[0] = new Card(Suit.Heart, Value.ACE); // player's first card
        cards[1] = new Card(Suit.Heart, Value.ACE); // dealer's second card
        cards[2] = new Card(Suit.Heart, Value.ACE); // player's second card
        cards[3] = new Card(Suit.Heart, Value.ACE); // player or dealer's card
        cards[4] = new Card(Suit.Heart, Value.ACE); // player or dealer's card
        cards[5] = new Card(Suit.Heart, Value.ACE); // player or dealer's card
        cards[6] = new Card(Suit.Heart, Value.ACE); // player or dealer's card
        cards[7] = new Card(Suit.Heart, Value.ACE); // player or dealer's card
        cards[8] = new Card(Suit.Heart, Value.ACE); // player or dealer's card
    }

    /// <summary>
    /// Method for shuffling a card deck
    /// </summary>
    public void Shuffle()
    {
        // reset card index
        cardIndex = 0;

        // play shuffle sound effect
        Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipShuffling, AudioType.Sfx);

        // shuffle the card deck
        for (int i = cards.Length - 1; i > 0; i--)
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
        // increment the card index
        cardIndex++;

        // return the card 
        return cards[cardIndex - 1];
    }

    /// <summary>
    /// Method to get number of remaining cards in the card deck
    /// </summary>
    /// <returns></returns>
    public int GetRemaining() => cards.Length - cardIndex;
}
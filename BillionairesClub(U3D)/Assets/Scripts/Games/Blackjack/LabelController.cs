using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Blackjack
{
    public class LabelController : LabelBehaviour
    {
        [Header("Local Player Hand")]
        [Tooltip("Labels that display the local player's hand")]
        public Label[] localHandLabels;
        [Tooltip("A label that display the perfect pair reward multiplier")]
        public Label perfectPairLabel;

        [Header("Other")]
        [Tooltip("A label object that shows dealer's hand")]
        public Label dealerHandLabel;
        [Tooltip("A label object that shows remaining cards in the card deck")]
        public Label cardDeckLabel;
        [Tooltip("Player's hand image(origin)")]
        public Image[] cardTextureOrigin;
        [Tooltip("Player's hand image(split)")]
        public Image[] cardTextureSplit;
        [Tooltip("Label objects that show the amount of money the player bets")]
        public Label[] betLabels;
        [Tooltip("Label objects taht show the amount of money the player bets on insurance")]
        public Label[] insuranceBets;
        [Tooltip("Label objects that show player's hand")]
        public Label[] playerHandLabel;

        /// <summary>
        /// Method to reset labels, it is called when a round finished
        /// </summary>
        public void Reset()
        {
            // hide labels 
            HideOtherLabels();
            ResetLocalHandVisbility();
        }

        /// <summary>
        /// Method to hide all labels in the game
        /// </summary>
        void HideOtherLabels()
        {
            // hide local player hand labels
            for (int i = 0; i < localHandLabels.Length; i++)
                localHandLabels[i].Switch(false);

            // hide bet labels
            for (int i = 0; i < betLabels.Length; i++)
                betLabels[i].Switch(false);

            // hide insurance bet labels
            for (int i = 0; i < insuranceBets.Length; i++)
                insuranceBets[i].Switch(false);

            // hide player hand labels
            for (int i = 0; i < playerHandLabel.Length; i++)
                playerHandLabel[i].Switch(false);

            // hide dealer's hand label
            dealerHandLabel.Switch(false);
        }

        /// <summary>
        /// Method to reset local player's hand label and its
        /// card images
        /// </summary>
        public void ResetLocalHandVisbility()
        {
            // hide all cards in local hand labels
            for (int i = 0; i < cardTextureOrigin.Length; i++)
            {
                cardTextureOrigin[i].enabled = false;
                cardTextureSplit[i].enabled = false;
            }

            // also hide perfect pair label
            perfectPairLabel.Switch(false);
        }

        /// <summary>
        /// Method to activate bet label component and modify its text
        /// </summary>
        /// <param name="index">index of the player</param>
        /// <param name="amount">amount of wagers</param>
        /// <param name="perfectPair">amount of perfect pair wagers</param>
        public void SetBetLabel(int index, int amount, int perfectPair = 0)
        {
            betLabels[index].Switch(true);
            betLabels[index].tmp.text = $"{amount:C0}" + (perfectPair > 0 ? $"<color=\"yellow\">({perfectPair:C0})</color>" : "");
        }

        /// <summary>
        /// Method to reveal a card in local player hand panel
        /// </summary>
        /// <param name="cardSprite">sprite source</param>
        /// <param name="cardIndex">index of the card</param>
        /// <param name="handIndex">index of the hand</param>
        public void RevealACard(Sprite cardSprite, int cardIndex, int handIndex = 0)
        {
            if (handIndex == 0)
            {
                cardTextureOrigin[cardIndex].enabled = true;
                cardTextureOrigin[cardIndex].sprite = cardSprite;
            }
            else
            {
                cardTextureSplit[cardIndex].enabled = true;
                cardTextureSplit[cardIndex].sprite = cardSprite;
            }
        }
    }
}
﻿using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TexasBonus
{
    public class LabelController : MonoBehaviour
    {
        [Header("Local Player Hand")]
        [Tooltip("A image to display the background of player hand")]
        public Image panel;
        [Tooltip("A text to display what combination the player has")]
        public TextMeshProUGUI title;
        [Tooltip("Default sprite for the card back")]
        public Sprite defaultTexture;
        [Tooltip("Player's hand image")]
        public Image[] cardTexture;
        [Space(25f)]        
        [Tooltip("Text objects that show the amount of money that the players bet")]
        public TextMeshProUGUI[] betLabels;
        [Tooltip("Text objects that show the hand-rank of the players hand")]
        public GameObject[] handRankLabel;
        [Tooltip("")]
        public GameObject dealerHandRankLabel;
        [Header("Sprite Asset")]
        public Sprite labelSpriteForWin;
        public Sprite labelSpriteForLose;
        public Sprite labelSpriteForStandoff;

        private Image dealerLabelBg;
        private Image[] playerLabelBg;
        private TextMeshProUGUI dealerLabelText;
        private TextMeshProUGUI[] playerLabelText;

        /// <summary>
        /// Method to setup the label controllers
        /// </summary>
        public void Setup()
        {
            // get player amount
            var playerCount = handRankLabel.Length;

            // find the label background image and text components
            playerLabelBg = new Image[playerCount];
            playerLabelText = new TextMeshProUGUI[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                playerLabelBg[i] = handRankLabel[i].GetComponent<Image>();
                playerLabelText[i] = handRankLabel[i].GetComponentInChildren<TextMeshProUGUI>();
            }

            // find the label background image and text components for the dealer
            dealerLabelBg = dealerHandRankLabel.GetComponent<Image>();
            dealerLabelText = dealerHandRankLabel.GetComponentInChildren<TextMeshProUGUI>();

            // reset the label controller
            Reset();
        }

        /// <summary>
        /// Method to reset labels, it is called then a round finished
        /// </summary>
        public void Reset()
        {
            // hide the panel object & title
            HideOtherLabels();
            HideHandRankPanel();
        }

        /// <summary>
        /// Method to display player hand-rank pannel, it is called after the player receive cards
        /// </summary>
        public void Display()
        {
            // display the panel & title
            panel.enabled = true;
            title.enabled = true;
            cardTexture[0].sprite = defaultTexture;
            cardTexture[1].sprite = defaultTexture;

            // reset title text
            title.text = "";
        }

        /// <summary>
        /// Method to hide local player's hadn-rank panel
        /// </summary>
        public void HideHandRankPanel()
        {
            panel.enabled = false;
            title.enabled = false;
            cardTexture[0].enabled = false;
            cardTexture[1].enabled = false;
        }

        /// <summary>
        /// Method to hide all labels in the game
        /// </summary>
        private void HideOtherLabels()
        {
            // hide all bet labels
            for (int i = 0; i < betLabels.Length; i++)
                SetBetLabel(i);

            // hide all player hand rank label
            for (int i = 0; i < handRankLabel.Length; i++)
                SetHandRankLabel(i, false);

            // hide dealer's hand rank labels
            SetHandRankLabelForDealer(false);
        }


        /// <summary>
        /// Method to display / hide the bet chip label for players
        /// </summary>
        /// <param name="index">the player index</param>
        /// <param name="amount">the amount of money</param>
        public void SetBetLabel(int index, int amount = 0)
        {
            betLabels[index].text = amount > 0 ? amount.ToString("C0") : "";
        }

        /// <summary>
        /// Method to display / hide the hand rank label for players
        /// </summary>
        /// <param name="index">the player index</param>
        /// <param name="status">true to display false to hide</param>
        /// <param name="message">message to display at the label</param>
        public void SetHandRankLabel(int index, bool status, string message = "")
        {
            playerLabelText[index].text = message;
            handRankLabel[index].SetActive(status);
        }

        /// <summary>
        /// Method to display / hide the hand rank label for the dealer
        /// </summary>
        /// <param name="status">true to display false to hide</param>
        /// <param name="message">message to display at the label</param>
        public void SetHandRankLabelForDealer(bool status, string message = "")
        {
            dealerLabelText.text = message;
            dealerHandRankLabel.SetActive(status);
        }


        /// <summary>
        /// Method to switch player and dealer's hand-rank panel, the sprite used
        /// to display the panel based on the given result
        /// Green for win, Red for lose, Purple for standoff
        /// </summary>
        /// <param name="playerIndex">index of the compared player</param>
        /// <param name="result">result of the comparison</param>
        public void SetHandRankLabelColor(int playerIndex, Result result)
        {
            switch (result)
            {
                case Result.Win:
                    dealerLabelBg.sprite = labelSpriteForLose;
                    playerLabelBg[playerIndex].sprite = labelSpriteForWin;
                    break;
                case Result.Lose:
                    dealerLabelBg.sprite = labelSpriteForWin;
                    playerLabelBg[playerIndex].sprite = labelSpriteForLose;
                    break;
                case Result.Standoff:
                    dealerLabelBg.sprite = labelSpriteForStandoff;
                    playerLabelBg[playerIndex].sprite = labelSpriteForStandoff;
                    break;
                default:
                    break;
            }
        }
    }
}

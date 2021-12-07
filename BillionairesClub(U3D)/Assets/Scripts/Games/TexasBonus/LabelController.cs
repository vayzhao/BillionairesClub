using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TexasBonus
{
    public class LabelController : LabelBehaviour
    {
        [Header("Local Player Hand")]
        [Tooltip("A image to display the background of player hand")]
        public Image panel;
        [Tooltip("A text to display what combination the player has")]
        public TextMeshProUGUI title;
        [Tooltip("A game object that holds the bonus panel")]
        public GameObject bonusPanel;
        [Tooltip("A text to display bonus reward multiplier")]
        public TextMeshProUGUI bonusText;

        [Header("Other")]
        [Tooltip("Default sprite for the card back")]
        public Sprite defaultTexture;
        [Tooltip("Text object that shows the hand-rank of the dealer")]
        public GameObject dealerHandRankLabel;
        [Tooltip("Player's hand image")]
        public Image[] cardTexture;        
        [Tooltip("Text objects that show the amount of money that the players bet")]
        public TextMeshProUGUI[] betLabels;
        [Tooltip("Text objects that show the hand-rank of the players")]
        public GameObject[] handRankLabel;

        private Image dealerLabelBg;               // image to display hand-rank label's background 
        private Image[] playerLabelBg;             // image to display hand-rank label's background 
        private TextMeshProUGUI dealerLabelText;   // text to display hand-rank label's text 
        private TextMeshProUGUI[] playerLabelText; // text to display hand-rank label's text

        /// <summary>
        /// Method to setup the label controller
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
        /// Method to reset labels, it is called when a round finished
        /// </summary>
        public void Reset()
        {
            // hide the panel object & title
            HideOtherLabels();
            SetLocalHandRankPanelVisibility(false);
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
        /// Method to modify hand-rank panel's visibility
        /// </summary>
        /// <param name="state">true to display, false to hide</param>
        public void SetLocalHandRankPanelVisibility(bool state)
        {
            // set states for hand-rank panel components
            panel.enabled = state;
            title.enabled = state;
            cardTexture[0].enabled = state;
            cardTexture[1].enabled = state;

            // when enabling, reset sprite for cardTexture and title text
            if (state)
            {
                title.text = "";
                cardTexture[0].sprite = defaultTexture;
                cardTexture[1].sprite = defaultTexture;
            }
            // when disabling, reset the bonus panel
            else
            {
                SetBonusLabel(0);
            }
        }

        /// <summary>
        /// Method to display / hide the bet chip label for players
        /// </summary>
        /// <param name="index">the player index</param>
        /// <param name="amount">the amount of money</param>
        public void SetBetLabel(int index, int amount = 0, int bonus = 0)
        {
            if (amount == 0)
            {
                betLabels[index].text = "";
                return;
            }

            if (bonus == 0)
                betLabels[index].text = $"{amount:C0}";
            else
                betLabels[index].text = $"{amount:C0}<color=\"yellow\">({bonus:C0})</color>";
        }

        /// <summary>
        /// Method to display betting result to players, it is called when 
        /// calculating player's profit and loss
        /// </summary>
        /// <param name="index">index of the player</param>
        /// <param name="amountChange">amount of profit/loss</param>
        public void DisplayBetResult(int index, int amountChange)
        {
            // first of all, hide the initial bet text
            SetBetLabel(index);

            // return if the bet amount hasn't change
            if (amountChange == 0)
                return;

            // otherwise, setup a text for bet result
            var message = amountChange > 0 ?
                $"<color=\"green\">+{amountChange:C0}</color>" :
                $"<color=\"red\">{amountChange:C0}</color>";

            // display the text
            FloatText(message, betLabels[index].transform.position, 60f, 3f, 0.3f);
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
                    dealerLabelBg.sprite = labelSpriteRed;
                    playerLabelBg[playerIndex].sprite = labelSpriteGreen;
                    break;
                case Result.Lose:
                    dealerLabelBg.sprite = labelSpriteGreen;
                    playerLabelBg[playerIndex].sprite = labelSpriteRed;
                    break;
                case Result.Standoff:
                    dealerLabelBg.sprite = labelSpritePurple;
                    playerLabelBg[playerIndex].sprite = labelSpritePurple;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Method to update bonus panel and its text
        /// </summary>
        /// <param name="multiplier"></param>
        public void SetBonusLabel(int multiplier)
        {
            // hide the bonus panel and return if the multipier is 0
            if (multiplier == 0)
            {
                bonusPanel.SetActive(false);
                return;
            }

            // otherwise, display the bonus and update its text
            bonusPanel.SetActive(true);
            bonusText.text = $"Bonus *{multiplier}";

            // play bonus sound effect
            Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipBonusPopup, AudioType.Sfx);
        }
    }
}

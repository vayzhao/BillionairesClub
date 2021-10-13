using System;
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
        
        public TextMeshProUGUI[] betLabels;
        public TextMeshProUGUI[] handRankLabel;
        public TextMeshProUGUI dealerHandRankLabel;

        /// <summary>
        /// Method to reset labels, it is called then a round finished
        /// </summary>
        public void Reset()
        {
            // hide the panel object & title
            panel.enabled = false;
            title.enabled = false;

            // reset card textures and disable it
            cardTexture[0].sprite = defaultTexture;
            cardTexture[1].sprite = defaultTexture;
            cardTexture[0].enabled = false;
            cardTexture[1].enabled = false;

            // hide all labels
            for (int i = 0; i < betLabels.Length; i++)
                betLabels[i].text = "";
            for (int i = 0; i < handRankLabel.Length; i++)
                handRankLabel[i].text = "";
            dealerHandRankLabel.text = "";
        }

        /// <summary>
        /// Method to display player hand pannel, it is called after the player receive cards
        /// </summary>
        public void Display()
        {
            // display the panel & title
            panel.enabled = true;
            title.enabled = true;

            // reset title text
            title.text = "";
        }

        public void HideHandPanel()
        {
            panel.enabled = false;
            title.enabled = false;
            cardTexture[0].enabled = false;
            cardTexture[1].enabled = false;
        }

        public void ShowBet(int index, int amount)
        {
            betLabels[index].text = amount.ToString("C0");
        }

        public void HideBet(int index)
        {
            betLabels[index].text = "";
        }

        public void ShowHandRank(int index, string message)
        {
            handRankLabel[index].text = message;
        }

        public void HideHandRank(int index)
        {
            handRankLabel[index].text = "";
        }

        public void ShowDealerHankRank(string message)
        {
            dealerHandRankLabel.text = message;
        }

        public void HideDealerHandRank()
        {
            dealerHandRankLabel.text = "";
        }
    }
}

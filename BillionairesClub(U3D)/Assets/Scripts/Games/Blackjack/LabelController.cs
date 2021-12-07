using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Blackjack
{
    public class LabelController : LabelBehaviour
    {
        [Header("Local Player Hand")]
        public Image[] panels;
        public TextMeshProUGUI[] titles;
        public GameObject perfectPairPanel;
        public TextMeshProUGUI perfectPairText;



        [Tooltip("Text object that shows dealer's hand")]
        public GameObject dealerHandLabel;
        [Tooltip("Text objects that show player's hand")]
        public GameObject[] playerHandLabel;

        private Image dealerLabelBg;
        private Image[] playerLabelBg;
        private TextMeshProUGUI dealerLabelText;
        private TextMeshProUGUI[] playerLabelText;

        /// <summary>
        /// Method to setup the label controller
        /// </summary>
        public void Setup()
        {
            // get player amount
            var playerCount = playerHandLabel.Length;

            // find the label background image and text components
            playerLabelBg = new Image[playerCount];
            playerLabelText = new TextMeshProUGUI[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                playerLabelBg[i] = playerHandLabel[i].GetComponent<Image>();
                playerLabelText[i] = playerHandLabel[i].GetComponentInChildren<TextMeshProUGUI>();
            }

            // find the label background image text components for the dealer
            dealerLabelBg = dealerHandLabel.GetComponent<Image>();
            dealerLabelText = dealerHandLabel.GetComponentInChildren<TextMeshProUGUI>();

            // reset the label controller
            Reset();
        }

        public void DisplayTableLabel()
        {

        }

        public void Reset()
        {
            
        }

        void HideOtherLabels()
        {

        }

    }
}
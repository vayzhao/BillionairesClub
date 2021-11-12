using System;
using System.Collections;
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
        [Tooltip("A game object that holds the bonus panel")]
        public GameObject bonusPanel;
        [Tooltip("A text to display bonus reward multiplier")]
        public TextMeshProUGUI bonusText;
        [Tooltip("Default sprite for the card back")]
        public Sprite defaultTexture;
        [Tooltip("Player's hand image")]
        public Image[] cardTexture;
        [Space(15)]        
        [Tooltip("Text objects that show the amount of money that the players bet")]
        public TextMeshProUGUI[] betLabels;
        [Tooltip("Text objects that show the hand-rank of the players")]
        public GameObject[] handRankLabel;
        [Tooltip("Text object that shows the hand-rank of the dealer")]
        public GameObject dealerHandRankLabel;
        [Header("Sprite Asset")]
        [Tooltip("Background sprite for hand-rank label when win")]
        public Sprite labelSpriteForWin;
        [Tooltip("Background sprite for hand-rank label when lose")]
        public Sprite labelSpriteForLose;
        [Tooltip("Background sprite for hand-rank label when standoff")]
        public Sprite labelSpriteForStandoff;

        private Image dealerLabelBg;               // image to display hand-rank label's background 
        private Image[] playerLabelBg;             // image to display hand-rank label's background 
        private TextMeshProUGUI dealerLabelText;   // text to display hand-rank label's text 
        private TextMeshProUGUI[] playerLabelText; // text to display hand-rank label's text

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
        /// Method to display the betting result in the bet label
        /// </summary>
        /// <param name="index">index of the player</param>
        /// <param name="amountChange">chip amount change for this round</param>
        public void SetBetLabelResult(int index, int amountChange)
        {
            // if the amount has not change, reset bet label and return
            if (amountChange == 0)
            {
                betLabels[index].text = "";
                return;
            }

            // otherwise, the player either win or lose
            if (amountChange > 0)
                betLabels[index].text = $"<color=\"green\">+{amountChange:C0}</color>";
            else
                betLabels[index].text = $"<color=\"red\">{amountChange:C0}</color>";

            // start floating the text and fade it
            StartCoroutine(BetLabelFloating(index));
        }
        IEnumerator BetLabelFloating(int index)
        {
            // initialize progress and record the origin position & color
            var progress = 0f;
            var originPos = betLabels[index].transform.position;
            var originCor = betLabels[index].color;
            
            // playing the text animation
            while (progress < 1f)
            {
                // update the progress
                var progressChange = Time.deltaTime / Const.WAIT_TIME_LABEL_FLOAT;
                progress += progressChange;

                // update the position and alpha color
                betLabels[index].transform.position += Vector3.up * 0.3f;
                betLabels[index].color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, progress));

                yield return new WaitForSeconds(Time.deltaTime);
            }

            // reset the bet label text
            betLabels[index].text = "";
            betLabels[index].color = originCor;
            betLabels[index].transform.position = originPos;
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

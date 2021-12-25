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
        [Tooltip("A label that display the local player's hand")]
        public Label localHandLabel;
        [Tooltip("A label that display the bonus reward multiplier")]
        public Label bonusLabel;

        [Header("Other")]        
        [Tooltip("Text object that shows the hand-rank of the dealer")]
        public Label dealerHandRankLabel;
        [Tooltip("Player's hand image")]
        public Image[] cardTexture;        
        [Tooltip("Text objects that show the amount of money that the players bet")]
        public Label[] betLabels;
        [Tooltip("Text objects that show the hand-rank of the players")]
        public Label[] handRankLabel;

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
                betLabels[i].Switch(false);

            // hide all player hand rank label
            for (int i = 0; i < handRankLabel.Length; i++)
                handRankLabel[i].Switch(false);

            // hide dealer's hand rank labels
            dealerHandRankLabel.Switch(false);
        }

        /// <summary>
        /// Method to modify hand-rank panel's visibility
        /// </summary>
        /// <param name="state">true to display, false to hide</param>
        public void SetLocalHandRankPanelVisibility(bool state)
        {
            // set states for hand-rank panel components
            localHandLabel.Switch(state);
            cardTexture[0].enabled = state;
            cardTexture[1].enabled = state;

            // when enabling, reset sprite for cardTexture and title text
            if (state)
            {
                localHandLabel.tmp.text = "";
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
        /// Method to activate bet label component and modify its text
        /// </summary>
        /// <param name="index">index of the player</param>
        /// <param name="amount">amount of wagers</param>
        /// <param name="bonus">amount of perfect pair wagers</param>
        public void SetBetLabel(int index, int amount, int bonus = 0)
        {
            betLabels[index].Switch(true);
            betLabels[index].tmp.text = $"{amount:C0}" + (bonus > 0 ? $"<color=\"yellow\">({bonus:C0})</color>" : "");
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
            betLabels[index].Switch(false);

            // return if the bet amount hasn't change
            if (amountChange == 0)
                return;

            // otherwise, setup a text for bet result
            var message = amountChange > 0 ?
                $"<color=\"green\">+{amountChange:C0}</color>" :
                $"<color=\"red\">{amountChange:C0}</color>";

            // display the text
            FloatText(message, betLabels[index].transform.position);
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
                    dealerHandRankLabel.bg.sprite = labelSpriteRed;
                    handRankLabel[playerIndex].bg.sprite = labelSpriteGreen;
                    break;
                case Result.Lose:
                    dealerHandRankLabel.bg.sprite = labelSpriteGreen;
                    handRankLabel[playerIndex].bg.sprite = labelSpriteRed;
                    break;
                case Result.Standoff:
                    dealerHandRankLabel.bg.sprite = labelSpritePurple;
                    handRankLabel[playerIndex].bg.sprite = labelSpritePurple;
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
            // return if the multiplier is 0
            if (multiplier == 0)
            {
                bonusLabel.Switch(false);
                return;
            }

            // otherwise, display the bonus and update its text
            bonusLabel.Switch(true);
            bonusLabel.tmp.text = $"Bonus *{multiplier}";

            // play bonus sound effect
            Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipBonusPopup, AudioType.Sfx);
        }
    }
}

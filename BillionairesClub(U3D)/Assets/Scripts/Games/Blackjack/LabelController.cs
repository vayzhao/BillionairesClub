using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Blackjack
{
    using static Para;
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
            // hide all local hand panels
            for (int i = 0; i < MAX_HAND; i++)
                ResetLocalHandVisibilityForSingle(i);

            // also hide perfect pair label
            perfectPairLabel.Switch(false);
        }
        public void ResetLocalHandVisibilityForSingle(int handIndex)
        {
            // hide the panel background
            localHandLabels[handIndex].Switch(false);
            LocalPanelTransparency(handIndex, TRANSPARENCE_NORMAL);

            // hide each card sprite
            if (handIndex == 0)
                for (int i = 0; i < cardTextureOrigin.Length; i++)
                    cardTextureOrigin[i].enabled = false;
            else if (handIndex == 1)
                for (int i = 0; i < cardTextureSplit.Length; i++)
                    cardTextureSplit[i].enabled = false;
        }

        /// <summary>
        /// Method to reset a player's global hand label and bet label
        /// </summary>
        /// <param name="playerIndex">index of the player</param>
        public void HideBetAndHandLabelForSingle(int playerIndex)
        {
            betLabels[playerIndex].Switch(false);
            playerHandLabel[playerIndex].Switch(false);
        }

        /// <summary>
        /// Method to update player's bet label during the game,
        /// this function might be called when player decides to 
        /// double down or split
        /// </summary>
        /// <param name="playerIndex">index of the player</param>
        /// <param name="bet">bet data from the player</param>
        public void UpdateBetLabel(int playerIndex, Bet bet)
        {
            // initialize text 
            var text = $"{ bet.anteWager + bet.doubleWager:C0}";

            // expand the text if the player has second hand
            if (bet.anteWagerSplit > 0)
                text += $"<color=\"yellow\">(" +
                    $"{bet.anteWagerSplit + bet.doubleWagerSplit:C0})</color>";
            else if (bet.perfectPairWager > 0)
                text += $"<color=\"yellow\">({bet.perfectPairWager:C0})</color>";

            // apply changes to the text component 
            betLabels[playerIndex].tmp.text = text;
        }

        /// <summary>
        /// Method to update player's bet & hand label, it is often called after player gets 
        /// a bust or five cards charlie when he has two hands. we use this update function 
        /// to remain bet & hand label for a hand that has not been cleared
        /// </summary>
        /// <param name="playerIndex">index of the player</param>
        /// <param name="handIndex">index of the hand</param>
        /// <param name="hand">the hand data</param>
        /// <param name="bet">the bet data</param>
        public void UpdateBetLabel(int playerIndex, int handIndex, Hand hand, Bet bet)
        {
            // update player's hand panel text
            playerHandLabel[playerIndex].tmp.text = hand.ToString(handIndex);

            // update player's bet label text
            betLabels[playerIndex].tmp.text = handIndex == 0 ?
                $"{bet.anteWager + bet.doubleWager:C0}" :
                $"{bet.anteWagerSplit + bet.doubleWagerSplit:C0}";
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

        /// <summary>
        /// Method to update perfect pair panel and its text
        /// </summary>
        /// <param name="multiplier"></param>
        public void SetPerfectPairLabel(int multiplier)
        {
            // return if the multiplier is 0
            if (multiplier == 0)
            {
                perfectPairLabel.Switch(false);
                return;
            }

            // otherwise, display the bonus and update its text
            perfectPairLabel.Switch(true);
            perfectPairLabel.tmp.text = $"Bonus *{multiplier}";

            // play bonus sound effect
            Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipBonusPopup, AudioType.Sfx);
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
                    dealerHandLabel.bg.sprite = labelSpriteRed;
                    playerHandLabel[playerIndex].bg.sprite = labelSpriteGreen;
                    break;
                case Result.Lose:
                    dealerHandLabel.bg.sprite = labelSpriteGreen;
                    playerHandLabel[playerIndex].bg.sprite = labelSpriteRed;
                    break;
                case Result.Standoff:
                    dealerHandLabel.bg.sprite = labelSpritePurple;
                    playerHandLabel[playerIndex].bg.sprite = labelSpritePurple;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Method for a player to split his hand
        /// </summary>
        /// <param name="playerIndex">index of the player</param>
        /// <param name="hand">player's current hand</param>
        /// <param name="splittedCardSprite">the card being splitted to the second hand</param>
        public void SplitHand(int playerIndex, Hand hand, Sprite splittedCardSprite)
        {
            // update local hand panel for the player
            localHandLabels[0].tmp.text = hand.ToString(0);
            localHandLabels[1].Display(hand.ToString(1));

            // set the first hand panel to be half-transparent
            LocalPanelTransparency(0, TRANSPARENCE_STAND);

            // disable the second card in first hand panel
            cardTextureOrigin[1].enabled = false;

            // reveal the second card in the first hand 
            RevealACard(splittedCardSprite, 0, 1);
        }

        /// <summary>
        /// Method to adjust local hand panel's transparency
        /// </summary>
        /// <param name="handIndex">index of the player</param>
        /// <param name="alphaValue">transparence value</param>
        public void LocalPanelTransparency(int handIndex, float alphaValue)
        {
            localHandLabels[handIndex].bg.color = new Color(1f, 1f, 1f, alphaValue);
        }

        
    }
}
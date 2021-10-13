using System;
using UnityEngine;
using UnityEngine.UI;

namespace TexasBonus
{
    public class PlayerAction : MonoBehaviour
    {
        [Header("UI Components")]
        public GameObject group_bonusWager;
        public GameObject group_anteWager;
        public GameObject group_betOrFold;
        public GameObject group_betOrCheck;

        public bool isWaiting;
        public Bet[] bets;

        private BetType betType;
        private int currentPlayerIndex;

        public GameManager gameManager;
        public TableController tableController;
        public LabelController labelController;

        public void Setup()
        {
            bets = new Bet[gameManager.players.Length];
        }

        public void ResetBet()
        {
            for (int i = 0; i < bets.Length; i++)
                bets[i].Reset();

            tableController.RemoveWagerModel();
        }

        public bool PlayerHasFolded(int playerIndex)
        {
            return bets[playerIndex].hasFolded;
        }

        public void DisplayBetPanel(BetType betType, int playerIndex)
        {
            isWaiting = true;
            currentPlayerIndex = playerIndex;
            this.betType = betType;

            switch (betType)
            {
                case BetType.BonusWager:
                    DisplayBonusWager();
                    break;
                case BetType.AnteWager:
                    DisplayAnteWager();
                    break;
                case BetType.Flop:
                    DisplayBetOrFlop();
                    break;
                case BetType.Turn:
                    DisplayBetOrCheck();
                    break;
                case BetType.River:
                    DisplayBetOrCheck();
                    break;
                default:
                    break;
            }
        }

        void DisplayBonusWager()
        {
            group_bonusWager.SetActive(true);
        }

        void DisplayAnteWager()
        {
            group_anteWager.SetActive(true);
        }

        void DisplayBetOrFlop()
        {
            group_betOrFold.SetActive(true);
        }

        void DisplayBetOrCheck()
        {
            group_betOrCheck.SetActive(true);
        }

        public void BetBonusWager(int value)
        {
            isWaiting = false;
            group_bonusWager.SetActive(false);

            bets[currentPlayerIndex].bonusWager = value;

            tableController.CreateWagerModel(currentPlayerIndex, 4, value);

        }

        public void BetBonusWager_AI(int playerIndex)
        {
            bets[playerIndex].bonusWager = 5;
            tableController.CreateWagerModel(playerIndex, 4, 5);
        }

        public void BetAnteWager(int value)
        {
            isWaiting = false;
            group_anteWager.SetActive(false);

            bets[currentPlayerIndex].anteWager = value;

            tableController.CreateWagerModel(currentPlayerIndex, 0, value);

            labelController.ShowBet(currentPlayerIndex, bets[currentPlayerIndex].GetTotal());
        }

        public void BetAnteWager_AI(int playerIndex)
        {
            var value = 15;

            bets[playerIndex].anteWager = value;
            tableController.CreateWagerModel(playerIndex, 0, value);

            labelController.ShowBet(playerIndex, bets[playerIndex].GetTotal());
        }

        public void Bet()
        {
            isWaiting = false;
            group_betOrFold.SetActive(false);
            group_betOrCheck.SetActive(false);

            switch (betType)
            {
                case BetType.Flop:
                    bets[currentPlayerIndex].flopWager = bets[currentPlayerIndex].anteWager * 2;
                    tableController.CreateWagerModel(currentPlayerIndex, 1, bets[currentPlayerIndex].anteWager * 2);                    
                    break;
                case BetType.Turn:
                    bets[currentPlayerIndex].turnWager = bets[currentPlayerIndex].anteWager * 1;
                    tableController.CreateWagerModel(currentPlayerIndex, 2, bets[currentPlayerIndex].anteWager * 1);
                    break;
                case BetType.River:
                    bets[currentPlayerIndex].riverWager = bets[currentPlayerIndex].anteWager * 1;
                    tableController.CreateWagerModel(currentPlayerIndex, 3, bets[currentPlayerIndex].anteWager * 1);
                    break;
                default:
                    break;
            }

            labelController.ShowBet(currentPlayerIndex, bets[currentPlayerIndex].GetTotal());
        }

        public void Bet_AI(BetType betType, int playerIndex)
        {
            switch (betType)
            {
                case BetType.Flop:
                    bets[playerIndex].flopWager = bets[playerIndex].anteWager * 2;
                    tableController.CreateWagerModel(playerIndex, 1, bets[playerIndex].anteWager * 2);
                    break;
                case BetType.Turn:
                    bets[playerIndex].turnWager = bets[playerIndex].anteWager * 1;
                    tableController.CreateWagerModel(playerIndex, 2, bets[playerIndex].anteWager * 1);
                    break;
                case BetType.River:
                    bets[playerIndex].riverWager = bets[playerIndex].anteWager * 1;
                    tableController.CreateWagerModel(playerIndex, 3, bets[playerIndex].anteWager * 1);
                    break;
                default:
                    break;
            }

            labelController.ShowBet(playerIndex, bets[playerIndex].GetTotal());
        }
        
        public void Fold()
        {
            isWaiting = false;
            bets[currentPlayerIndex].hasFolded = true;
            group_anteWager.SetActive(false);
            group_betOrFold.SetActive(false);

            labelController.HideHandPanel();
            tableController.playerCardsObj[currentPlayerIndex].SetActive(true);
        }

        public void Fold_AI(int playerIndex)
        {
            bets[playerIndex].hasFolded = true;
            tableController.playerCardsObj[playerIndex].SetActive(true);
        }

        public void Check()
        {
            isWaiting = false;
            group_betOrCheck.SetActive(false);
        }

        public void Check_AI(int playerIndex)
        {

        }


    }
}

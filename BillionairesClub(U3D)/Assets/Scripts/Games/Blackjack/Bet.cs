namespace Blackjack
{
    public struct Bet
    {
        public int anteWager;
        public int perfectPairWager;
        public int insuranceWager;
        public int amountChange;
        public bool stand;

        public void Reset()
        {
            anteWager = 0;
            perfectPairWager = 0;
            insuranceWager = 0;
            amountChange = 0;
            stand = false;
        }

        /// <summary>
        /// Method to edit and get the amount change in this game
        /// </summary>
        /// <param name="change"></param>
        public void EditAmountChange(int change) => amountChange += change;
        public int GetAmountChange() => amountChange;
    }
}
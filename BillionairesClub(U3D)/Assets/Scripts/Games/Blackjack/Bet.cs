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
            stand = false;
        }

    }
}
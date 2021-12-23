namespace Blackjack
{
    public struct Bet
    {
        public int anteWager;        
        public int doubleWager;
        public int anteWagerSplit;
        public int doubleWagerSplit;
        public int perfectPairWager;
        public int insuranceWager;

        public void Reset()
        {
            anteWager = 0;
            doubleWager = 0;
            anteWagerSplit = 0;
            doubleWagerSplit = 0;
            perfectPairWager = 0;
            insuranceWager = 0;
        }

    }
}
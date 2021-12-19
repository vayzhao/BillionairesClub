namespace Blackjack
{
    public static class Para
    {
        public const int MAX_HAND = 2;
        public const int MAX_CARD_PER_HAND = 5;

        public const int CARD_DECK_COUNT = 8;

        public const int WAGER_INDEX_ANTE = 0;
        public const int WAGER_INDEX_BONUS_SPLITE_WAGER = 1;
        public const int WAGER_INDEX_DOUBLE = 2;
        public const int WAGER_INDEX_SPLIT_DOUBLE = 3;
        public const int WAGER_INDEX_INSURANCE = 4;

        public const float REWARD_PP_SAMESUIT = 30f;
        public const float REWARD_PP_SAMECOLOR = 10f;
        public const float REWARD_PP_NORMAL = 5f;
        public const float REWARD_NORMAL = 1f;
        public const float REWARD_BLACKJACK = 1.5f;
        public const float REWARD_FIVECARDS = 5f;
        public const float REWARD_INSURANCE = 2f;
    }

    public enum HandRank
    {
        Bust,
        Value,
        Blackjack,
        FiveCardCharlie
    }
}
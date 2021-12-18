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

        public const int REWARD_PP_SAMESUIT = 30;
        public const int REWARD_PP_SAMECOLOR = 10;
        public const int REWARD_PP_NORMAL = 5;
        public const int REWARD_BLACKJACK = 3;
        public const int REWARD_FIVECARDS = 5;
        public const int REWARD_INSURANCE = 2;
    }
}
public enum GameType
{
    None,
    TexasBonus,
    Blackjack
}

public enum SeatAvailability
{
    All,
    UserOnly,
    DealerOnly
}

public enum SceneType
{
    Homepage,
    InCasino,
    InGame
}

public enum BetType
{
    BonusWager,
    AnteWager,
    Flop,
    Turn,
    River,
    PerfectPair,
    Insurance
}

public enum Suit
{
    Diamond,
    Club,
    Heart,
    Spade
}

public enum Value
{
    TWO,
    THREE,
    FOUR,
    FIVE,
    SIX,
    SEVEN,
    EIGHT,
    NINE,
    TEN,
    JACK,
    QUEEN,
    KING,
    ACE
}

public enum SuitColor
{
    Red,
    Black
}

public enum Rank
{
    HighHand,
    OnePair,
    TwoPairs,
    ThreeOfAKind,
    Straight,
    Flush,
    FullHouse,
    FourOfAKind,
    StraightFlush,
    RoyalFlush
}

public enum Result
{
    Win,
    Lose,
    Standoff
}

public enum StorageType
{
    Chip,
    Gem,
    ModelIndex,
    Progress,
    PlayerState,
    Position,
    Rotation,
    HasRecord,
    Name,
    Description
}

public enum AudioType
{
    Sfx,
    UI
}

public enum PlatformType
{
    Window,
    iOS,
    Android
}
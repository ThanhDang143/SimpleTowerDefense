public static class GloblaConstants
{
    public static class Default
    {
        public const string LEVEL = "LVL001";
    }

    public static class Noti
    {
        public const string ON_UPDATE_WAVE = "ON_UPDATE_WAVE";
        public const string ON_UPDATE_GAME_STATE = "ON_UPDATE_GAME_STATE";
        public const string NOT_ENOUGH_COIN = "NOT_ENOUGH_COIN";
        public const string ON_LOAD_NEXT_LEVEL = "ON_LOAD_NEXT_LEVEL";
    }

}


public enum GameState
{
    NONE,
    LOADING,
    PREPARE,
    PLAYING,
    PAUSE,
}

public enum MapCellType
{
    EMPTY,
    BLOCK,
    ROAD,
    START_POINT,
    END_POINT,
}

public enum Stat
{
    HP, // = value * PowerMultiplier 
    DMG, // = value * PowerMultiplier
    ATK_RANGE, // = value * PowerMultiplier
    ATK_SPEED, // = value / PowerMultiplier
    MOVE_SPEED, // = value * PowerMultiplier
}

public enum ResourcesKey
{
    COIN,
    LEVEL_HP
}
namespace StarDefense.Core
{
    #region Hero
    public enum HeroRarity
    {
        Common,
        Rare,
        Epic,
        Unique,
        Legend
    }

    public enum HeroTribe
    {
        Human,
        Alien
    }
    #endregion

    #region Commander
    public enum AttackStrategyType
    {
        Single,
        Splash
    }
    #endregion

    #region UI
    public enum ActiveUI 
    { 
        None,
        Summon,
        Repair,
        Upgrade,
        TranscendButton,
        TranscendSelecte
    }
    #endregion
}

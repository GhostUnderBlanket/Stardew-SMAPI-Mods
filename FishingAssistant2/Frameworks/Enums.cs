namespace FishingAssistant2.Frameworks
{
    internal enum TreasureChance
    {
        Default,
        Always,
        Never
    }

    internal enum FishQuality
    {
        Default,
        None = 0,
        Silver = 1,
        Gold = 2,
        Iridium = 4
    }

    internal enum AutoActionResponse
    {
        OnDelay,
        CanDoAction,
        CantDoAction,
        InventoryFull,
        LowStamina
    }
}

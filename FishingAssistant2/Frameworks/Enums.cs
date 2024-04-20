namespace ChibiKyu.StardewMods.FishingAssistant2.Frameworks
{
    internal enum HudPosition { Left, Right }

    internal enum TreasureChance { Default, Always, Never }

    internal enum ActionOnInventoryFull { Stop, Drop, Discard }

    internal enum FishQuality { Any = -1, None = 0, Silver = 1, Gold = 2, Iridium = 4 }

    internal enum PauseFishingBehaviour { Off, WarnOnly, WarnAndPause }

    internal enum SkipFishingMiniGame { Off, SkipAll, SkipOnlyCaught }
}

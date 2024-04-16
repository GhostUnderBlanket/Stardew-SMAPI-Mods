using StardewModdingAPI;

namespace ChibiKyu.StardewMods.FishingAssistant2
{
    internal class ModConfig
    {
        /// KEY BINDING ///
        /// <summary>Button for toggling automation of this mod</summary>
        public SButton EnableAutomationButton { get; set; } = SButton.F5;

        /// <summary>Button for toggling catch or ignore treasure in fishing mini-game</summary>
        public SButton CatchTreasureButton { get; set; } = SButton.F6;
        
        /// <summary>Button for quickly open Fishing assistant2 GMCM in-game config menu</summary>
        public SButton OpenConfigMenuButton { get; set; } = SButton.None;

        /// HUD ///
        /// <summary>Position to display mod status</summary>
        public string ModStatusPosition { get; set; } = "Left";
        
        /// AUTOMATION ///
        /// <summary>Toggle for auto-casting fishing rod</summary>
        public bool AutoCastFishingRod { get; set; } = true;

        /// <summary>Toggle for auto-hooking fish</summary>
        public bool AutoHookFish { get; set; } = true;

        /// <summary>Toggle for automatically playing fishing mini-game</summary>
        public bool AutoPlayMiniGame { get; set; } = true;

        /// <summary>Toggle for automatically closing fish popup</summary>
        public bool AutoClosePopup { get; set; } = true;

        /// <summary>Toggle for automatically looting treasure</summary>
        public bool AutoLootTreasure { get; set; } = true;
        
        /// <summary>Action to take if inventory is full while looting</summary>
        public string ActionIfInventoryFull { get; set; } = "Stop";
        
        /// <summary>Toggle for automatically trashing junk items when obtaining new items</summary>
        public bool AutoTrashJunk { get; set; } = false;
        
        /// <summary>Items priced less than or equal to this are considered junk</summary>
        public int JunkHighestPrice { get; set; } = 0;
        
        /// <summary>Toggle for allowing trashing of fish with a sale price corresponding to 'JunkHighestPrice'</summary>
        public bool AllowTrashFish { get; set; } = false;
        
        /// <summary>Should mod auto-pause fishing during nighttime</summary>
        public string AutoPauseFishing { get; set; } = "WarnAndPause";

        /// <summary>Time to trigger fishing pause</summary>
        public int TimeToPause { get; set; } = 24;
        
        /// <summary>Number of warnings the mod will give if you continue fishing after reaching 'TimeToPause'</summary>
        public int WarnCount { get; set; } = 1;

        /// <summary>Toggle to automatically eating food in inventory</summary>
        public bool AutoEatFood { get; set; } = false;
        
        /// <summary>Energy percent to consider as low energy and find something to eat</summary>
        public int EnergyPercentToEat { get; set; } = 5;

        /// <summary>Toggle for allowing eating of caught fish</summary>
        public bool AllowEatingFish { get; set; } = false;
        
        /// <summary>Toggle for auto attaching bait if possible</summary>
        public bool AutoAttachBait { get; set; } = false;
        
        /// <summary>Preference for bait type</summary>
        public string PreferredBait { get; set; } = "Any";
        
        /// <summary>Toggle for spawning bait if none is available</summary>
        public bool SpawnBaitIfDontHave { get; set; } = false;
        
        /// <summary>Amount of bait to spawn</summary>
        public int BaitAmountToSpawn { get; set; } = 10;
        
        /// <summary>Toggle for automatically attaching tackles if possible</summary>
        public bool AutoAttachTackles { get; set; } = false;
        
        /// <summary>Preference for tackle type in the first slot</summary>
        public string PreferredTackle { get; set; } = "Any";
        
        /// <summary>Preference for tackle type in the second slot of the Adv. iridium rod</summary>
        public string PreferredAdvIridiumTackle { get; set; } = "Any";
        
        /// <summary>Toggle for spawning tackle if none are available</summary>
        public bool SpawnTackleIfDontHave { get; set; } = false;
        
        /// FISHING ///
        /// <summary>Toggle to instantly skip fishing mini-game</summary>
        public string SkipFishingMiniGame { get; set; } = "Off";

        /// <summary>Toggle for making fish bite instantly</summary>
        public bool InstantFishBite { get; set; } = false;
        
        /// <summary>Preference for the amount of fish pulled from water</summary>
        public int PreferFishAmount { get; set; } = 1;
        
        /// <summary>Quality of caught fish</summary>
        public string PreferFishQuality { get; set; } = "Any";

        /// <summary>Whether to consider every catch as perfect</summary>
        public bool AlwaysPerfect { get; set; } = false;
        
        /// <summary>Toggle to always catch fish at maximum size</summary>
        public bool AlwaysMaxFishSize { get; set; } = false;
        
        /// <summary>Multiplier applied to fish difficulty</summary>
        public float FishDifficultyMultiplier { get; set; } = 1;

        /// <summary>Value added to fish difficulty</summary>
        public int FishDifficultyAdditive { get; set; } = 0;

        /// <summary>Instantly catch treasure when it appears</summary>
        public bool InstantCatchTreasure { get; set; } = false;
        
        /// <summary>Chance of finding treasure while fishing</summary>
        public string TreasureChance { get; set; } = "Default";
        
        /// <summary>Chance of finding golden treasure while fishing</summary>
        public string GoldenTreasureChance { get; set; } = "Default";
        
        /// FISH PREVIEW ///
        /// <summary>Toggle for displaying fish info while catching fish</summary>
        public bool DisplayFishPreview { get; set; } = true;

        /// <summary>Toggle for showing fish name with fish info</summary>
        public bool ShowFishName { get; set; } = true;

        /// <summary>Toggle for showing treasure with fish info</summary>
        public bool ShowTreasure { get; set; } = true;

        /// <summary>Show a preview for all fish species, even those you have never caught</summary>
        public bool ShowUncaughtFish { get; set; } = false;

        /// <summary>Toggle for showing fish info if the current fish is legendary</summary>
        public bool ShowLegendaryFish { get; set; } = false;

        /// FISHING ROD ///
        /// <summary>Start the game at day 1 with fishing rod</summary>
        public string StartWithFishingRod { get; set; } = "None";
        
        /// <summary>Allow player to unlock and adjust casting power by holding left mouse for a second and at the next cast mod will use this cast power instead of 'Cast Power Percent'</summary>
        public bool UseSmartCastPower { get; set; } = true;
        
        /// <summary>Preference for casting power</summary>
        public int CastPowerPercent { get; set; } = 100;
        
        /// <summary>Make your fishing bait last indefinitely</summary>
        public bool InfiniteBait { get; set; } = false;
        
        /// <summary>Make your tackle last indefinitely</summary>
        public bool InfiniteTackle { get; set; } = false;

        /// Enchantment ///
        /// <summary>Toggle for adding Auto-Hook enchantment to fishing rod</summary>
        public bool AddAutoHookEnchantment { get; set; } = false;

        /// <summary>Toggle for adding Efficient enchantment to fishing rod</summary>
        public bool AddEfficientEnchantment { get; set; } = false;

        /// <summary>Toggle for adding Master enchantment to fishing rod</summary>
        public bool AddMasterEnchantment { get; set; } = false;

        /// <summary>Toggle for adding Preserving enchantment to fishing rod</summary>
        public bool AddPreservingEnchantment { get; set; } = false;

        /// <summary>Toggle for removing enchantment when fishing rod is unequipped</summary>
        public bool RemoveWhenUnequipped { get; set; } = true;
    }
}

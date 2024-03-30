using StardewModdingAPI;

namespace ChibiKyu.StardewMods.FishingAssistant2
{
    internal class ModConfig
    {
        /// <summary>Button for toggle max or free fishing rod cast power</summary>
        internal SButton EnableModButton { get; set; } = SButton.F5;

        /// <summary>Button for toggle catch or ignore treasure in fishing mini-game</summary>
        internal SButton CatchTreasureButton { get; set; } = SButton.F6;

        /// <summary>Position to display fish info when playing fishing mini-game</summary>
        internal string ModStatusPosition { get; set; } = "Left";
        
        internal bool MaxCastPower { get; set; } = true;

        /// <summary>Make fish to bite instantly.</summary>
        internal bool InstantFishBite { get; set; } = false;
        
        internal string TreasureChance { get; set; } = "Default";
        
        internal string PreferFishQuality { get; set; } = "Any";

        /// <summary>Whether the game should consider every catch to be perfectly executed, even if it wasn't.</summary>
        internal bool AlwaysPerfect { get; set; } = false;
        
        internal bool AlwaysMaxFishSize { get; set; } = false;
        
        /// <summary>A multiplier applied to the fish difficulty. This can a number between 0 and 1 to lower difficulty, or more than 1 to increase it.</summary>
        internal float FishDifficultyMultiplier { get; set; } = 1;

        /// <summary>A value added to the fish difficulty. This can be less than 0 to decrease difficulty, or more than 0 to increase it.</summary>
        internal int FishDifficultyAdditive { get; set; } = 0;

        /// <summary>Whether to catch fish instantly.</summary>
        internal bool InstantCatchFish { get; set; } = false;

        /// <summary>Whether to catch treasure instantly.</summary>
        internal bool InstantCatchTreasure { get; set; } = false;
        
        /// <summary>Let Fishing Assistant 2 auto cast fishing rod</summary>
        internal bool AutoCastFishingRod { get; set; } = true;

        /// <summary>Let Fishing Assistant 2 auto hook fish rod</summary>
        internal bool AutoHookFish { get; set; } = true;

        /// <summary>Let Fishing Assistant 2 auto play fishing mini-game</summary>
        internal bool AutoPlayMiniGame { get; set; } = true;

        /// <summary>Let Fishing Assistant 2 auto close fish popup</summary>
        internal bool AutoClosePopup { get; set; } = true;

        /// <summary>Let Fishing Assistant 2 auto loot treasure</summary>
        internal bool AutoLootTreasure { get; set; } = true;
        
        internal string ActionIfInventoryFull { get; set; } = "Stop Loot";
        
        /// <summary>Should mod auto attach bait if possible.</summary>
        internal bool AutoAttachBait { get; set; } = false;
        
        internal string PreferBait { get; set; } = "Any";
        
        /// <summary>Whether fishing bait lasts forever.</summary>
        internal bool InfiniteBait { get; set; } = false;
        
        internal bool SpawnBaitIfDontHave { get; set; } = false;
        
        /// <summary>Should mod auto attach tackles if possible.</summary>
        internal bool AutoAttachTackles { get; set; } = false;
        
        internal string PreferTackle { get; set; } = "Any";
        
        /// <summary>Whether fishing tackles last forever.</summary>
        internal bool InfiniteTackle { get; set; } = false;
        
        internal bool SpawnTackleIfDontHave { get; set; } = false;
        
        /// <summary>Whether to pause fishing on given time</summary>
        internal string AutoPauseFishing { get; set; } = "WarnAndPause";

        /// <summary>Time to stop fishing</summary>
        internal int PauseFishingTime { get; set; } = 24;
        
        internal int NumToWarn { get; set; } = 1;

        /// <summary>Whether to eat some food if need</summary>
        internal bool AutoEatFood { get; set; } = false;

        /// <summary>Amount of energy in percent to find food to eat</summary>
        internal int EnergyPercentToEat { get; set; } = 5;

        /// <summary>Allow to eat fish that you caught</summary>
        internal bool AllowEatingFish { get; set; } = false;

        /// <summary>Should mod show fish info while catching fish?</summary>
        internal bool DisplayFishInfo { get; set; } = true;

        /// <summary>Position to display fish info when playing fishing mini-game</summary>
        internal string FishInfoDisplayPosition { get; set; } = "UpperRight";

        /// <summary>Show fish name with fish info</summary>
        internal bool ShowFishName { get; set; } = true;

        /// <summary>Show treasure with fish info</summary>
        internal bool ShowTreasure { get; set; } = true;

        /// <summary>Show fish info whether uncaught or not</summary>
        internal bool ShowUncaughtFishSpecies { get; set; } = false;

        /// <summary>Always show fish info if current fish is legendary</summary>
        internal bool AlwaysShowLegendaryFish { get; set; } = false;

        /// <summary>Add auto-Hook enchantment to fishing rod</summary>
        internal bool AddAutoHookEnchantment { get; set; } = false;

        /// <summary>Add Efficient enchantment to fishing rod</summary>
        internal bool AddEfficientEnchantment { get; set; } = false;

        /// <summary>Add Master enchantment to fishing rod</summary>
        internal bool AddMasterEnchantment { get; set; } = false;

        /// <summary>Add Preserving enchantment to fishing rod</summary>
        internal bool AddPreservingEnchantment { get; set; } = false;

        /// <summary>Add Preserving enchantment to fishing rod</summary>
        internal bool RemoveEnchantmentWhenUnequipped { get; set; } = true;
    }
}
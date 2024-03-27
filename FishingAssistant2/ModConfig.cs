using StardewModdingAPI;

namespace ChibiKyu.StardewMods.FishingAssistant2
{
    internal class ModConfig
    {
        /// <summary>Button for toggle max or free fishing rod cast power</summary>
        public SButton EnableModButton { get; set; } = SButton.F5;

        /// <summary>Button for toggle catch or ignore treasure in fishing mini-game</summary>
        public SButton CatchTreasureButton { get; set; } = SButton.F6;

        /// <summary>Position to display fish info when playing fishing mini-game</summary>
        public string ModStatusPosition { get; set; } = "Left";
        
        public bool MaxCastPower { get; set; } = true;

        /// <summary>Make fish to bite instantly.</summary>
        public bool InstantFishBite { get; set; } = false;
        
        public string TreasureChance { get; set; } = "Default";

        /// <summary>Whether the game should consider every catch to be perfectly executed, even if it wasn't.</summary>
        public bool AlwaysPerfect { get; set; } = false;
        
        /// <summary>A multiplier applied to the fish difficulty. This can a number between 0 and 1 to lower difficulty, or more than 1 to increase it.</summary>
        public float FishDifficultyMultiplier { get; set; } = 1;

        /// <summary>A value added to the fish difficulty. This can be less than 0 to decrease difficulty, or more than 0 to increase it.</summary>
        public int FishDifficultyAdditive { get; set; } = 0;

        /// <summary>Whether to catch fish instantly.</summary>
        public bool InstantCatchFish { get; set; } = false;

        /// <summary>Whether to catch treasure instantly.</summary>
        public bool InstantCatchTreasure { get; set; } = false;
        
        /// <summary>Let Fishing Assistant 2 auto cast fishing rod</summary>
        public bool AutoCastFishingRod { get; set; } = true;

        /// <summary>Let Fishing Assistant 2 auto hook fish rod</summary>
        public bool AutoHookFish { get; set; } = true;

        /// <summary>Let Fishing Assistant 2 auto play fishing mini-game</summary>
        public bool AutoPlayMiniGame { get; set; } = true;

        /// <summary>Let Fishing Assistant 2 auto close fish popup</summary>
        public bool AutoClosePopup { get; set; } = true;

        /// <summary>Let Fishing Assistant 2 auto loot treasure</summary>
        public bool AutoLootTreasure { get; set; } = true;
        
        public string ActionIfInventoryFull { get; set; } = "Stop Loot";
        
        /// <summary>Should mod auto attach bait if possible.</summary>
        public bool AutoAttachBait { get; set; } = false;
        
        public string PreferBait { get; set; } = "Any";
        
        /// <summary>Whether fishing bait lasts forever.</summary>
        public bool InfiniteBait { get; set; } = false;
        
        public bool SpawnBaitIfDontHave { get; set; } = false;
        
        /// <summary>Should mod auto attach tackles if possible.</summary>
        public bool AutoAttachTackles { get; set; } = false;
        
        public string PreferTackle { get; set; } = "Any";
        
        /// <summary>Whether fishing tackles last forever.</summary>
        public bool InfiniteTackle { get; set; } = false;
        
        public bool SpawnTackleIfDontHave { get; set; } = false;
        
        /// <summary>Whether to pause fishing on given time</summary>
        public bool AutoPauseFishing { get; set; } = true;

        /// <summary>Time to stop fishing</summary>
        public int PauseFishingTime { get; set; } = 24;

        /// <summary>Whether to eat some food if need</summary>
        public bool AutoEatFood { get; set; } = false;

        /// <summary>Amount of energy in percent to find food to eat</summary>
        public int EnergyPercentToEat { get; set; } = 5;

        /// <summary>Allow to eat fish that you caught</summary>
        public bool AllowEatingFish { get; set; } = false;

        /// <summary>Should mod show fish info while catching fish?</summary>
        public bool DisplayFishInfo { get; set; } = true;

        /// <summary>Position to display fish info when playing fishing mini-game</summary>
        public string FishInfoDisplayPosition { get; set; } = "UpperRight";

        /// <summary>Show fish name with fish info</summary>
        public bool ShowFishName { get; set; } = true;

        /// <summary>Show treasure with fish info</summary>
        public bool ShowTreasure { get; set; } = true;

        /// <summary>Show fish info whether uncaught or not</summary>
        public bool ShowUncaughtFishSpecies { get; set; } = false;

        /// <summary>Always show fish info if current fish is legendary</summary>
        public bool AlwaysShowLegendaryFish { get; set; } = false;

        /// <summary>Add auto-Hook enchantment to fishing rod</summary>
        public bool AddAutoHookEnchantment { get; set; } = false;

        /// <summary>Add Efficient enchantment to fishing rod</summary>
        public bool AddEfficientEnchantment { get; set; } = false;

        /// <summary>Add Master enchantment to fishing rod</summary>
        public bool AddMasterEnchantment { get; set; } = false;

        /// <summary>Add Preserving enchantment to fishing rod</summary>
        public bool AddPreservingEnchantment { get; set; } = false;

        /// <summary>Add Preserving enchantment to fishing rod</summary>
        public bool RemoveEnchantmentWhenUnequipped { get; set; } = true;
    }
}
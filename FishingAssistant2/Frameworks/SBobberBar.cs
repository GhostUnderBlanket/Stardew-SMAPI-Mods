using FishingAssistant2.Frameworks;
using StardewValley;
using StardewValley.Menus;

namespace ChibiKyu.StardewMods.FishingAssistant2.Frameworks
{
    internal class SBobberBar(BobberBar bobberBar)
    {
        public BobberBar Instance { get; set; } = bobberBar;
        
        public void OverrideFishDifficult(float difficultyMultiplier, float difficultyAdditive)
        {
            Instance.difficulty *= difficultyMultiplier;
            Instance.difficulty += difficultyAdditive;
            if (Instance.difficulty < 0) Instance.difficulty = 0;
        }

        public void OverrideTreasureChance(string treasureChance)
        {
            if (Game1.isFestival()) return;
            
            if (treasureChance == "Always")
                Instance.treasure = true;
            else if (treasureChance == "Never") 
                Instance.treasure = false;
        }

        public void InstantCatchTreasure(bool catchTreasure)
        {
            if (Instance.treasure) Instance.treasureCaught = catchTreasure;
        }
        
        public void InstantCatchFish()
        {
            Instance.distanceFromCatching = 1.0f;
        }

        public void AlwaysPerfect()
        {
            Instance.perfect = true;
        }
    }
}
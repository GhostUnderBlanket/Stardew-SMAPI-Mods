using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;

namespace ChibiKyu.StardewMods.FishingAssistant2.Frameworks
{
    internal class SBobberBar(BobberBar bobberBar)
    {
        internal BobberBar Instance { get; set; } = bobberBar;

        internal void OverrideFishDifficult(float difficultyMultiplier, float difficultyAdditive)
        {
            Instance.difficulty *= difficultyMultiplier;
            Instance.difficulty += difficultyAdditive;
            if (Instance.difficulty < 0) Instance.difficulty = 0;
        }

        internal void OverrideTreasureChance(string treasureChance, string goldenTreasureChance)
        {
            if (Game1.isFestival()) return;

            if (treasureChance == TreasureChance.Always.ToString())
                Instance.treasure = true;
            else if (treasureChance == TreasureChance.Never.ToString())
                Instance.treasure = false;

            if (!Instance.treasure) return;

            if (goldenTreasureChance == TreasureChance.Always.ToString())
                Instance.goldenTreasure = true;
            else if (goldenTreasureChance == TreasureChance.Never.ToString())
                Instance.goldenTreasure = false;
        }

        internal void InstantCatchTreasure(bool catchTreasure)
        {
            if (Instance.treasure) Instance.treasureCaught = catchTreasure;
        }

        internal void InstantCatchFish()
        {
            Instance.distanceFromCatching = 1.0f;
        }

        internal void AlwaysPerfect(bool alwaysPerfect)
        {
            if (alwaysPerfect)
            {
                Instance.perfect = true;
                Instance.fishShake = Vector2.Zero;

                if (Game1.isFestival()) Game1.CurrentEvent.perfectFishing();
            }
        }

        internal void OverrideFishQuality(string preferFishQuality)
        {
            if (preferFishQuality != FishQuality.Any.ToString()) Instance.fishQuality = (int)Enum.Parse(typeof(FishQuality), preferFishQuality);
        }

        internal void OverrideFishSize(bool alwaysMaxFishSize)
        {
            if (alwaysMaxFishSize) Instance.fishSize = Instance.maxFishSize;
        }

        internal void HandleFishPreview(bool displayFishPreview)
        {
            if (displayFishPreview && Instance.challengeBaitFishes > -1)
            {
                if (!Instance.bobbers.Contains("(O)SonarBobber"))
                    Instance.bobbers.Add("(O)SonarBobber");
            }
        }

        internal bool AlreadyCaughtFish(int minCaught = 1)
        {
            if (Instance == null) return false;

            ItemMetadata? metadata = ItemRegistry.GetMetadata(Instance.whichFish);
            Game1.player.fishCaught.TryGetValue(metadata.QualifiedItemId, out int[] numArray);
            bool caughtSpecies = numArray != null && numArray.Length > 0 && numArray[0] >= minCaught;

            return caughtSpecies;
        }
    }
}

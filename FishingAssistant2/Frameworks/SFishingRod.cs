using ChibiKyu.StardewMods.Common;
using FishingAssistant2;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Menus;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace ChibiKyu.StardewMods.FishingAssistant2.Frameworks
{
    internal class SFishingRod(FishingRod instance, Func<ModConfig> modConfig)
    {
        private readonly List<BaseEnchantment> _addedEnchantments = new();

        internal float SmartCastPower;

        internal bool SmartCastPowerSaved;

        internal float UnlockCastPowerTimer = modConfig().ParsedUnlockCastPowerTime;

        internal FishingRod Instance { get; set; } = instance;

        internal void ResetSmartCastPower()
        {
            UnlockCastPowerTimer = modConfig().ParsedUnlockCastPowerTime;
            SmartCastPower = 0;
            SmartCastPowerSaved = false;
        }

        #region Automation

        internal void AutoAttachBait()
        {
            if (!IsRodNotInUse() || Game1.isFestival()) return;

            IList<Item> items = Game1.player.Items;

            if (Instance.UpgradeLevel < 2) return;

            // Check the bait slot. Case where there is already bait attached. We stack the same type of bait onto the existing bait attached to the fishing rod.
            if (Instance.attachments[0] != null && Instance.attachments[0].Stack != Instance.attachments[0].maximumStackSize())
            {
                foreach (Item item in items)
                {
                    if (item?.Category != Object.baitCategory || !item.Name.Equals(Instance.attachments[0].Name)) continue;

                    int stackAdd = Math.Min(Instance.attachments[0].getRemainingStackSpace(), item.Stack);
                    Instance.attachments[0].Stack += stackAdd;
                    item.Stack -= stackAdd;

                    if (item.Stack == 0) Game1.player.removeItemFromInventory(item);

                    CommonHelper.PushWarning(Instance, I18n.HudMessage_AutoAttach(), item.DisplayName, Instance.DisplayName);
                }
            }
            // Case where there is no bait attached. We simply attach the first instance of bait we see in the inventory onto the fishing rod.
            else if (Instance.attachments[0] == null)
            {
                foreach (Item item in items)
                {
                    if (item?.Category != Object.baitCategory || (modConfig().PreferredBait != "Any" && item.QualifiedItemId != modConfig().PreferredBait)) continue;

                    Instance.attachments[0] = (Object)item;
                    Game1.player.removeItemFromInventory(item);
                    CommonHelper.PushWarning(Instance, I18n.HudMessage_AutoAttach(), item.DisplayName, Instance.DisplayName);

                    break;
                }

                if (!modConfig().SpawnBaitIfDontHave || Instance.attachments[0] != null) return;

                //Fallback if user doesn't set prefer bait
                if (modConfig().PreferredBait == "Any") modConfig().PreferredBait = "(O)685";
                Object baits = ItemRegistry.Create<Object>(modConfig().PreferredBait, modConfig().BaitAmountToSpawn);
                Instance.attachments[0] = baits;
                CommonHelper.PushWarning(Instance, I18n.HudMessage_AutoAttachSpawned(), baits.DisplayName, Instance.DisplayName);
            }
        }

        internal void AutoAttachTackles()
        {
            if (!IsRodNotInUse() || Game1.isFestival()) return;

            IList<Item> items = Game1.player.Items;

            // Check the tackle slot.
            if (Instance.UpgradeLevel >= 3) AttachTackleAtSlot(1, modConfig().PreferredTackle);

            if (Instance.UpgradeLevel == 4) AttachTackleAtSlot(2, modConfig().PreferredAdvIridiumTackle);

            return;

            void AttachTackleAtSlot(int attachmentSlot, string preferredTackle = "Any", string fallbackTackle = "(O)686")
            {
                if (Instance.attachments[attachmentSlot] != null) return;

                foreach (Item item in items)
                {
                    if (item?.Category != Object.tackleCategory || (preferredTackle != "Any" && item.QualifiedItemId != preferredTackle)) continue;

                    Instance.attachments[attachmentSlot] = (Object)item;
                    Game1.player.removeItemFromInventory(item);
                    CommonHelper.PushWarning(Instance, I18n.HudMessage_AutoAttach(), item.DisplayName, Instance.DisplayName);

                    break;
                }

                if (!modConfig().SpawnTackleIfDontHave || Instance.attachments[attachmentSlot] != null) return;

                //Fallback if user doesn't set prefer tackle
                if (preferredTackle == "Any") preferredTackle = fallbackTackle;

                Object tackle = ItemRegistry.Create<Object>(preferredTackle);
                Instance.attachments[attachmentSlot] = tackle;
                CommonHelper.PushWarning(Instance, I18n.HudMessage_AutoAttachSpawned(), tackle.DisplayName, Instance.DisplayName);
            }
        }

        internal void AutoHook()
        {
            if (!IsRodCanHook()) return;

            Instance.timePerBobberBob = 1f;
            Instance.timeUntilFishingNibbleDone = FishingRod.maxTimeToNibble;
            Instance.DoFunction(Game1.player.currentLocation, (int)Instance.bobber.X, (int)Instance.bobber.Y, 1, Game1.player);
            Rumble.rumble(0.95f, 200f);
        }

        #endregion

        #region Override

        internal void OverrideCastPower()
        {
            if (Instance.isTimingCast && UnlockCastPowerTimer-- <= 0 && modConfig().UnlockCastPowerTime < 3f)
            {
                UnlockCastPowerTimer = 0;
                SmartCastPower = Instance.castingPower;
                SmartCastPowerSaved = true;
            }

            if (Instance.castedButBobberStillInAir) UnlockCastPowerTimer = modConfig().ParsedUnlockCastPowerTime;

            Instance.castingPower = SmartCastPowerSaved ? SmartCastPower : modConfig().DefaultCastPower / 100.0f + 0.01f;
        }

        internal void OverrideTimeUntilFishBite()
        {
            if (Instance.timeUntilFishingBite > 0) Instance.timeUntilFishingBite = 0f;
        }

        internal void OverrideGoldenTreasureChance()
        {
            if (modConfig().GoldenTreasureChance == TreasureChance.Always.ToString())
                Instance.goldenTreasure = true;
            else if (modConfig().GoldenTreasureChance == TreasureChance.Never.ToString())
                Instance.goldenTreasure = false;
        }

        internal void OverrideNumberOfFishCaught(int numberOfFishCaught, BobberBar bar)
        {
            bool isLucky = Instance.GetBait()?.QualifiedItemId == "(O)774" && Game1.random.NextDouble() < 0.25 + Game1.player.DailyLuck / 2.0;

            numberOfFishCaught = Game1.isFestival() || bar.bossFish ? 1 : isLucky ? 2 : numberOfFishCaught;

            if (bar.challengeBaitFishes > 0) numberOfFishCaught = bar.challengeBaitFishes;

            Instance.numberOfFishCaught = numberOfFishCaught;
        }

        internal void InfiniteBait()
        {
            if (Instance.attachmentSlots() >= 1 && Instance.attachments[0] != null) Instance.attachments[0].Stack = Instance.attachments[0].maximumStackSize();
        }

        internal void InfiniteTackles()
        {
            if (Instance.attachmentSlots() >= 2 && Instance.attachments[1] != null) Instance.attachments[1].uses.Value = 0;
            if (Instance.attachmentSlots() >= 3 && Instance.attachments[2] != null) Instance.attachments[2].uses.Value = 0;
        }

        #endregion

        #region Enchantment

        internal void AddEnchantment(BaseEnchantment enchantment)
        {
            _addedEnchantments.Add(enchantment);
            Instance.enchantments.Add(enchantment);
        }

        internal void ClearAddedEnchantments()
        {
            foreach (BaseEnchantment enchantment in _addedEnchantments)
                Instance.enchantments.Remove(enchantment);
        }

        #endregion

        #region Conditional

        internal bool IsRodNotInUse()
        {
            return Context.CanPlayerMove && !Instance.inUse();
        }

        internal bool IsRodCanCast()
        {
            return IsRodNotInUse() && !Game1.player.isMoving() && !Game1.player.isRidingHorse();
        }

        private bool IsRodCanHook()
        {
            return Instance is
            {
                isNibbling: true,
                hit: false,
                isReeling: false,
                pullingOutOfWater: false,
                fishCaught: false,
                showingTreasure: false
            } && !Instance.hasEnchantmentOfType<AutoHookEnchantment>();
        }

        internal bool IsRodShowingFish()
        {
            return !Context.CanPlayerMove && Instance.fishCaught && Instance.inUse() && Instance is
            {
                isCasting: false,
                isTimingCast: false,
                isReeling: false,
                pullingOutOfWater: false,
                showingTreasure: false
            };
        }

        #endregion
    }
}
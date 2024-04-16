using ChibiKyu.StardewMods.Common;
using FishingAssistant2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Menus;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace ChibiKyu.StardewMods.FishingAssistant2.Frameworks
{
    internal class SFishingRod(FishingRod instance)
    {
        internal FishingRod Instance { get; set; } = instance;
        
        private readonly List<BaseEnchantment> _addedEnchantments = new List<BaseEnchantment>();

        private float _smartCastPower;
        private float _discardSmartCastTimer = 60.0f;
        private bool _smartCastPowerSaved;

        #region Automation

        internal void AutoAttachBait(string preferBait, bool infiniteBait, bool spawnBaitIfDontHave, int baitAmountToSpawn)
        {
            if (IsRodNotInUse() && !Game1.isFestival())
            {
                IList<Item> items = Game1.player.Items;
                
                if (Instance.UpgradeLevel >= 2)
                {
                    // Check the bait slot. Case where there is already bait attached. We stack the same type of bait onto the existing bait attached to the fishing rod.
                    if (Instance.attachments[0] != null && Instance.attachments[0].Stack != Instance.attachments[0].maximumStackSize())
                    {
                        foreach (Item item in items)
                        {
                            if (item?.Category == Object.baitCategory && item.Name.Equals(Instance.attachments[0].Name))
                            {
                                int stackAdd = Math.Min(Instance.attachments[0].getRemainingStackSpace(), item.Stack);
                                Instance.attachments[0].Stack += stackAdd;
                                item.Stack -= stackAdd;

                                if (item.Stack == 0) Game1.player.removeItemFromInventory(item);

                                CommonHelper.PushNotification(HUDMessage.newQuest_type, I18n.HudMessage_AutoAttach(), item.DisplayName, Instance.DisplayName);
                            }
                        }
                    }
                    // Case where there is no bait attached. We simply attach the first instance of bait we see in the inventory onto the fishing rod.
                    else if (Instance.attachments[0] == null)
                    {
                        foreach (Item item in items)
                        {
                            if (item?.Category == Object.baitCategory && (preferBait == "Any" || item.QualifiedItemId == preferBait))
                            {
                                Instance.attachments[0] = (Object)item;
                                Game1.player.removeItemFromInventory(item);
                                CommonHelper.PushNotification(HUDMessage.newQuest_type, I18n.HudMessage_AutoAttach(), item.DisplayName, Instance.DisplayName);
                                break;
                            }
                        }

                        if (spawnBaitIfDontHave && Instance.attachments[0] == null)
                        {
                            //Fallback if user doesn't set prefer bait
                            if (preferBait == "Any") preferBait = "(O)685";
                            Object baits = ItemRegistry.Create<Object>(preferBait, baitAmountToSpawn);
                            Instance.attachments[0] = baits;
                            CommonHelper.PushNotification(HUDMessage.newQuest_type, I18n.HudMessage_AutoAttachSpawned(), baits.DisplayName, Instance.DisplayName);
                        }
                    }

                    if (Instance.attachments[0] != null && infiniteBait)
                    {
                        Instance.attachments[0].Stack = Instance.attachments[0].maximumStackSize();
                    }
                }
            }
        }

        internal void AutoAttachTackles(string[] preferTackle, bool infiniteTackle, bool spawnTackleIfDontHave)
        {
            if (IsRodNotInUse() && !Game1.isFestival())
            {
                IList<Item> items = Game1.player.Items;
                
                // Check the tackle slot.
                if (Instance.UpgradeLevel >= 3)
                {
                    if (Instance.attachments[1] == null)
                    {
                        foreach (Item item in items)
                        {
                            if (item?.Category == Object.tackleCategory && (preferTackle[0] == "Any" || item.QualifiedItemId == preferTackle[0]))
                            {
                                Instance.attachments[1] = (Object)item;
                                Game1.player.removeItemFromInventory(item);
                                CommonHelper.PushNotification(HUDMessage.newQuest_type, I18n.HudMessage_AutoAttach(), item.DisplayName, Instance.DisplayName);
                                break;
                            }
                        }

                        if (spawnTackleIfDontHave && Instance.attachments[1] == null)
                        {
                            //Fallback if user doesn't set prefer tackle
                            if (preferTackle[0] == "Any") preferTackle[0] = "(O)686";
                            Object tackle = ItemRegistry.Create<Object>(preferTackle[0]);
                            Instance.attachments[1] = tackle;
                            CommonHelper.PushNotification(HUDMessage.newQuest_type, I18n.HudMessage_AutoAttachSpawned(), tackle.DisplayName, Instance.DisplayName);
                        }
                    }
                    
                    if (Instance.attachments[1] != null && infiniteTackle) Instance.attachments[1].uses.Value = 0;
                }
                
                if (Instance.UpgradeLevel == 4)
                {
                    if (Instance.attachments[2] == null)
                    {
                        foreach (Item item in items)
                        {
                            if (item?.Category == Object.tackleCategory && (preferTackle[1] == "Any" || item.QualifiedItemId == preferTackle[1]))
                            {
                                Instance.attachments[2] = (Object)item;
                                Game1.player.removeItemFromInventory(item);
                                CommonHelper.PushNotification(HUDMessage.newQuest_type, I18n.HudMessage_AutoAttach(), item.DisplayName, Instance.DisplayName);
                                break;
                            }
                        }

                        if (spawnTackleIfDontHave && Instance.attachments[2] == null)
                        {
                            //Fallback if user doesn't set prefer tackle
                            if (preferTackle[1] == "Any") preferTackle[1] = "(O)686";
                            var tackle2 = ItemRegistry.Create<Object>(preferTackle[1]);
                            Instance.attachments[2] = tackle2;
                            CommonHelper.PushNotification(HUDMessage.newQuest_type, I18n.HudMessage_AutoAttachSpawned(), tackle2.DisplayName, Instance.DisplayName);
                        }
                    }
                    
                    if (Instance.attachments[2] != null && infiniteTackle) Instance.attachments[2].uses.Value = 0;
                }
            }
        }
        
        internal void AutoHook()
        {
            if (IsRodCanHook())
            {
                Instance.timePerBobberBob = 1f;
                Instance.timeUntilFishingNibbleDone = FishingRod.maxTimeToNibble;
                Instance.DoFunction(Game1.player.currentLocation, (int)Instance.bobber.X, (int)Instance.bobber.Y, 1, Game1.player);
                Rumble.rumble(0.95f, 200f);
            }
        }

        #endregion

        #region Override

        internal void OverrideCastPower(bool useSmartCastPower, int castPower)
        {
            if (useSmartCastPower && Instance.isTimingCast && _discardSmartCastTimer-- <= 0)
            {
                _smartCastPower = Instance.castingPower;
                _smartCastPowerSaved = true;
            }
            
            if (!useSmartCastPower && _smartCastPowerSaved)
            {
                ResetSmartCastPower();
            }

            if (Instance.castedButBobberStillInAir)
            {
                _discardSmartCastTimer = 60;
            }
            
            Instance.castingPower = _smartCastPowerSaved ? _smartCastPower : (castPower / 100.0f) + 0.01f;
        }
        
        internal void ResetSmartCastPower()
        {
            _smartCastPower = 0.0f;
            _smartCastPowerSaved = false;
        }
        
        internal void OverrideTimeUntilFishBite()
        {
            if (Instance.timeUntilFishingBite > 0)
                Instance.timeUntilFishingBite = 0f;
        }
        
        internal void OverrideGoldenTreasureChance(string goldenTreasureChance)
        {
            if (goldenTreasureChance == TreasureChance.Always.ToString())
                Instance.goldenTreasure = true;
            else if (goldenTreasureChance == TreasureChance.Never.ToString()) 
                Instance.goldenTreasure = false;
        }

        internal void OverrideNumberOfFishCaught(int numberOfFishCaught, BobberBar bar)
        {
            bool isLucky = Instance.GetBait()?.QualifiedItemId == "(O)774" && Game1.random.NextDouble() < 0.25 + Game1.player.DailyLuck / 2.0;

            numberOfFishCaught = (Game1.isFestival() || bar.bossFish) ? 1 : isLucky ? 2 : numberOfFishCaught;
            
            if (bar.challengeBaitFishes > 0) numberOfFishCaught = bar.challengeBaitFishes;

            Instance.numberOfFishCaught = numberOfFishCaught;
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
            return !Context.CanPlayerMove 
                   && Instance.fishCaught 
                   && Instance.inUse() 
                   && Instance is
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
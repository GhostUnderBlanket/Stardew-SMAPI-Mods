using ChibiKyu.StardewMods.Common;
using FishingAssistant2;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace ChibiKyu.StardewMods.FishingAssistant2.Frameworks
{
    internal class SFishingRod(FishingRod instance)
    {
        internal FishingRod Instance { get; set; } = instance;
        
        private readonly List<BaseEnchantment> _addedEnchantments = new List<BaseEnchantment>();
        
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
                            // Category value for bait is -21. Source: https://github.com/veywrn/StardewValley/blob/master/StardewValley/Item.cs
                            if (item?.Category == -21 && item.Name.Equals(Instance.attachments[0].Name))
                            {
                                int stackAdd = Math.Min(Instance.attachments[0].getRemainingStackSpace(), item.Stack);
                                Instance.attachments[0].Stack += stackAdd;
                                item.Stack -= stackAdd;

                                if (item.Stack == 0) Game1.player.removeItemFromInventory(item);

                                CommonHelper.PushNotification(HUDMessage.newQuest_type, I18n.HudMessage_AutoAttach(), ItemRegistry.GetData(item.QualifiedItemId).DisplayName, ItemRegistry.GetData(Instance.QualifiedItemId).DisplayName);
                            }
                        }
                    }
                    // Case where there is no bait attached. We simply attach the first instance of bait we see in the inventory onto the fishing rod.
                    else if (Instance.attachments[0] == null)
                    {
                        foreach (Item item in items)
                        {
                            if (item?.Category == -21 && (preferBait == "Any" || item.QualifiedItemId == preferBait))
                            {
                                Instance.attachments[0] = (Object)item;
                                Game1.player.removeItemFromInventory(item);
                                CommonHelper.PushNotification(HUDMessage.newQuest_type, I18n.HudMessage_AutoAttach(), ItemRegistry.GetData(item.QualifiedItemId).DisplayName, ItemRegistry.GetData(Instance.QualifiedItemId).DisplayName);
                                break;
                            }
                        }

                        if (spawnBaitIfDontHave && preferBait != "Any") Instance.attachments[0] = ItemRegistry.Create<Object>(preferBait, baitAmountToSpawn);
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
                if (Instance.UpgradeLevel >= 3 && Instance.attachments[1] == null)
                {
                    foreach (Item item in items)
                    {
                        // Category value for tackle is -22.
                        // Source: https://github.com/veywrn/StardewValley/blob/master/StardewValley/Item.cs
                        if (item?.Category == -22 && (preferTackle[0] == "Any" || item.QualifiedItemId == preferTackle[0]))
                        {
                            Instance.attachments[1] = (Object)item;
                            Game1.player.removeItemFromInventory(item);
                            CommonHelper.PushNotification(HUDMessage.newQuest_type, I18n.HudMessage_AutoAttach(), ItemRegistry.GetData(item.QualifiedItemId).DisplayName, ItemRegistry.GetData(Instance.QualifiedItemId).DisplayName);
                            break;
                        }
                    }

                    if (spawnTackleIfDontHave && preferTackle[0] != "Any") Instance.attachments[1] = ItemRegistry.Create<Object>(preferTackle[0]);
                    
                    if (Instance.attachments[1] != null && infiniteTackle) Instance.attachments[1].uses.Value = 0;
                }
                
                if (Instance.UpgradeLevel == 4 && Instance.attachments[2] == null)
                {
                    foreach (Item item in items)
                    {
                        // Category value for tackle is -22.
                        // Source: https://github.com/veywrn/StardewValley/blob/master/StardewValley/Item.cs
                        if (item?.Category == -22 && (preferTackle[1] == "Any" || item.QualifiedItemId == preferTackle[1]))
                        {
                            Instance.attachments[2] = (Object)item;
                            Game1.player.removeItemFromInventory(item);
                            CommonHelper.PushNotification(HUDMessage.newQuest_type, I18n.HudMessage_AutoAttach(), ItemRegistry.GetData(item.QualifiedItemId).DisplayName, ItemRegistry.GetData(Instance.QualifiedItemId).DisplayName);
                            break;
                        }
                    }

                    if (spawnTackleIfDontHave && preferTackle[1] != "Any") Instance.attachments[2] = ItemRegistry.Create<Object>(preferTackle[1]);
                    
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
        
        internal void InstantFishBite()
        {
            if (Instance.timeUntilFishingBite > 0)
                Instance.timeUntilFishingBite = 0f;
        }
        
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
        
        internal bool IsRodNotInUse()
        {
            return Context.CanPlayerMove && Game1.activeClickableMenu == null && !Instance.inUse();
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
            return !Context.CanPlayerMove && Instance.fishCaught && Instance.inUse() && !Instance.isCasting && !Instance.isTimingCast && !Instance.isReeling && !Instance.pullingOutOfWater && !Instance.showingTreasure;
        }
    }
}
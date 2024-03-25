using FishingAssistant2.Frameworks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Menus;
using StardewValley.SpecialOrders;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace ChibiKyu.StardewMods.FishingAssistant2.Frameworks
{
    internal class SFishingRod(FishingRod instance)
    {
        public FishingRod Instance { get; set; } = instance;
        
        private readonly int _defaultNumAttachmentSlots = instance.numAttachmentSlots.Value;
        private readonly Object _defaultBait = instance.GetBait();
        private readonly List<Object> _defaultTackles = instance.GetTackle();
        private readonly List<BaseEnchantment> _addedEnchantments = new List<BaseEnchantment>();
        
        private int _autoCastDelay = 60;
        private int _autoClosePopupDelay = 30;
        
        public AutoActionResponse DoAutoCastFishingRod(int playerFacingDirection, int standingPosX, int standingPosY)
        {
            if (_autoCastDelay-- > 0)
                return AutoActionResponse.OnDelay;
            _autoCastDelay = 60;
            
            if (!IsRodNotInUse() || Game1.player.isRidingHorse() || Game1.isFestival())
                return AutoActionResponse.CantDoAction;
        
            bool hasEnoughStamina = Game1.player.stamina > 8.0 - Game1.player.FishingLevel * 0.1;
            bool hasEfficientEnchantment = Instance.hasEnchantmentOfType<EfficientToolEnchantment>();
            bool isInventoryFull = Game1.player.isInventoryFull();

            if (isInventoryFull)
                return AutoActionResponse.InventoryFull;
            if (!hasEnoughStamina && !hasEfficientEnchantment)
                return AutoActionResponse.LowStamina;
            
            Game1.player.faceDirection(playerFacingDirection);
            Instance.beginUsing(Game1.currentLocation, standingPosX, standingPosY, Game1.player);
            
            return AutoActionResponse.CanDoAction;
        }

        public void AutoHook()
        {
            if (IsRodCanHook())
            {
                Instance.timePerBobberBob = 1f;
                Instance.timeUntilFishingNibbleDone = FishingRod.maxTimeToNibble;
                Instance.DoFunction(Game1.player.currentLocation, (int)Instance.bobber.X, (int)Instance.bobber.Y, 1, Game1.player);
                Rumble.rumble(0.95f, 200f);
            }
        }

        public void AutoCloseFishPopup()
        {
            if (IsRodShowingFish() && !Game1.isFestival())
            {
                if (_autoClosePopupDelay-- > 0)
                    return;
                _autoClosePopupDelay = 30;
                
                DecompiledClosePopup();
            }
        }
        
        public void InstantFishBite()
        {
            if (Instance.timeUntilFishingBite > 0)
                Instance.timeUntilFishingBite = 0f;
        }
        
        public void AddEnchantment(BaseEnchantment enchantment)
        {
            _addedEnchantments.Add(enchantment);
            Instance.enchantments.Add(enchantment);
        }

        public void ClearAddedEnchantments()
        {
            foreach (BaseEnchantment enchantment in _addedEnchantments)
                Instance.enchantments.Remove(enchantment);
        }
        
        private bool IsRodNotInUse()
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
        
        private bool IsRodShowingFish()
        {
            return !Context.CanPlayerMove && Instance.fishCaught && Instance.inUse() && !Instance.isCasting && !Instance.isTimingCast && !Instance.isReeling && !Instance.pullingOutOfWater && !Instance.showingTreasure;
        }

        #region Decompiled

        private void DecompiledClosePopup()
        {
            /// This code is copied from decompile Stardew Valley FishingRod.cs
            Game1.player.playNearbySoundLocal("coin");
            if (!Instance.fromFishPond && Game1.IsSummer && Instance.whichFish.QualifiedItemId == "(O)138" &&
                Game1.dayOfMonth >= 20 && Game1.dayOfMonth <= 21 &&
                Game1.random.NextDouble() < 0.33 * (double)Instance.numberOfFishCaught)
                Instance.gotTroutDerbyTag = true;
            if (!Instance.treasureCaught && !Instance.gotTroutDerbyTag)
            {
                Instance.recastTimerMs = 200;
                Item obj = CreateFish();
                bool flag = obj.GetItemTypeId() == "(O)";
                if ((obj.Category == -4 || obj.HasContextTag("counts_as_fish_catch")) && !Instance.fromFishPond)
                {
                    int num = (int)Game1.player.stats.Increment("PreciseFishCaught",
                        Math.Max(1, Instance.numberOfFishCaught));
                }

                if (obj.QualifiedItemId == "(O)79" || obj.QualifiedItemId == "(O)842")
                {
                    obj = (Item)Game1.player.currentLocation.tryToCreateUnseenSecretNote(Game1.player);
                    if (obj == null)
                        return;
                }

                bool fromFishPond = Instance.fromFishPond;
                Game1.player.completelyStopAnimatingOrDoingAction();
                Instance.doneFishing(Game1.player, !fromFishPond);
                if (((Game1.isFestival() ? 0 : (!fromFishPond ? 1 : 0)) & (flag ? 1 : 0)) != 0 &&
                    Game1.player.team.specialOrders != null)
                {
                    foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
                    {
                        Action<Farmer, Item> onFishCaught = specialOrder.onFishCaught;
                        if (onFishCaught != null)
                            onFishCaught(Game1.player, obj);
                    }
                }

                if (Game1.isFestival() || Game1.player.addItemToInventoryBool(obj))
                    return;
                Game1.activeClickableMenu = (IClickableMenu)new ItemGrabMenu((IList<Item>)new List<Item>()
                {
                    obj
                }, (object)Instance).setEssential(true);
            }
            else
            {
                Instance.fishCaught = false;
                Instance.showingTreasure = true;
                Game1.player.UsingTool = true;
                Item fish = CreateFish();
                if ((fish.Category == -4 || fish.HasContextTag("counts_as_fish_catch")) && !Instance.fromFishPond)
                {
                    int num = (int)Game1.player.stats.Increment("PreciseFishCaught",
                        Math.Max(1, Instance.numberOfFishCaught));
                }

                if (Game1.player.team.specialOrders != null)
                {
                    foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
                    {
                        Action<Farmer, Item> onFishCaught = specialOrder.onFishCaught;
                        if (onFishCaught != null)
                            onFishCaught(Game1.player, fish);
                    }
                }

                bool inventoryBool = Game1.player.addItemToInventoryBool(fish);
                if (Instance.treasureCaught)
                {
                    Instance.animations.Add(new TemporaryAnimatedSprite(
                        Instance.goldenTreasure ? "LooseSprites\\Cursors_1_6" : "LooseSprites\\Cursors",
                        Instance.goldenTreasure ? new Rectangle(256, 75, 32, 32) : new Rectangle(64, 1920, 32, 32),
                        500f, 1,
                        0, Game1.player.Position + new Vector2(-32f, -160f), false, false,
                        (float)((double)Game1.player.StandingPixel.Y / 10000.0 + 1.0 / 1000.0), 0.0f, Color.White,
                        4f, 0.0f,
                        0.0f, 0.0f)
                    {
                        motion = new Vector2(0.0f, -0.128f),
                        timeBasedMotion = true,
                        endFunction = new TemporaryAnimatedSprite.endBehavior(Instance.openChestEndFunction),
                        extraInfoForEndBehavior = inventoryBool ? 0 : fish.Stack,
                        alpha = 0.0f,
                        alphaFade = -1f / 500f
                    });
                }
                else
                {
                    if (!Instance.gotTroutDerbyTag)
                        return;
                    Instance.animations.Add(new TemporaryAnimatedSprite("TileSheets\\Objects_2",
                        new Rectangle(80, 16, 16, 16), 500f, 1, 0,
                        Game1.player.Position + new Vector2(-8f, (float)sbyte.MinValue), false, false,
                        (float)((double)Game1.player.StandingPixel.Y / 10000.0 + 1.0 / 1000.0), 0.0f, Color.White,
                        4f, 0.0f,
                        0.0f, 0.0f)
                    {
                        motion = new Vector2(0.0f, -0.128f),
                        timeBasedMotion = true,
                        endFunction = new TemporaryAnimatedSprite.endBehavior(Instance.openChestEndFunction),
                        extraInfoForEndBehavior = inventoryBool ? 0 : fish.Stack,
                        alpha = 0.0f,
                        alphaFade = -1f / 500f,
                        id = 1074
                    });
                }
            }
        }
        
        private Item CreateFish()
        {
            Item itemOrErrorItem = Instance.whichFish.CreateItemOrErrorItem(quality: Instance.fishQuality);
            itemOrErrorItem.SetFlagOnPickup = Instance.setFlagOnCatch;
            if (itemOrErrorItem.GetItemTypeId() =="(O)")
            {
                if (itemOrErrorItem.QualifiedItemId == GameLocation.CAROLINES_NECKLACE_ITEM_QID)
                {
                    if (itemOrErrorItem is Object @object)
                        @object.questItem.Value = true;
                }
                else if (Instance.numberOfFishCaught > 1 && itemOrErrorItem.QualifiedItemId != "(O)79" && itemOrErrorItem.QualifiedItemId != "(O)842")
                    itemOrErrorItem.Stack = Instance.numberOfFishCaught;
            }
            return itemOrErrorItem;
        }

        #endregion
    }
}
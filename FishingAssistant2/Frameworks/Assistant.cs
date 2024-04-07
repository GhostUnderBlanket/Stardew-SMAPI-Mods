using ChibiKyu.StardewMods.Common;
using FishingAssistant2;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Menus;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace ChibiKyu.StardewMods.FishingAssistant2.Frameworks
{
    internal class Assistant(Func<ModEntry> modEntry, Func<ModConfig> modConfig)
    {
        internal int NumWarnThisDay = 0;
        internal bool IsInFishingMiniGame;
        private bool _isInTreasureChestMenu;
        
        private readonly ModConfig _config = modConfig();
        private readonly ModEntry _modEntry = modEntry();
        
        private SBobberBar? _bobberBar;
        private SFishingRod? _fishingRod;
        private ItemGrabMenu? _treasureChestMenu;
        private IList<Item> excludeItems = new List<Item>();
        
        private int _autoCastDelay = 60;
        private int _autoClosePopupDelay = 30;
        private int _autoLootDelay = 30;
        private bool _catchingTreasure;
        
        private readonly IReflectedField<MouseState>? _currentMouseState = modEntry().Helper.Reflection.GetField<MouseState>(Game1.input, "_currentMouseState");
        
        #region FishingRod

        public void OnEquipFishingRod(FishingRod fishingRod, bool isModEnable)
        {
            if (_fishingRod == null)
            {
                OverrideFishingRod(fishingRod);
            }
            else if (_fishingRod.Instance != fishingRod)
            {
                OnUnEquipFishingRod();
                OverrideFishingRod(fishingRod);
            }

            if (isModEnable) _modEntry.CachePlayerPosition();
            else _modEntry.ForgetPlayerPosition();
            
            DoFishingRodAssistantTask();
        }

        public void OnUnEquipFishingRod()
        {
            if (_fishingRod == null) return;
            
            _modEntry.ForgetPlayerPosition();
            
            if (_config.RemoveEnchantmentWhenUnequipped) _fishingRod.ClearAddedEnchantments();
            
            _fishingRod = null;
        }

        private void OverrideFishingRod(FishingRod fishingRod)
        {
            _fishingRod = new SFishingRod(fishingRod);
            
            if (_config.AddAutoHookEnchantment && !fishingRod.hasEnchantmentOfType<AutoHookEnchantment>())
                _fishingRod.AddEnchantment(new AutoHookEnchantment());
 
            if (_config.AddEfficientEnchantment && !fishingRod.hasEnchantmentOfType<EfficientToolEnchantment>())
                _fishingRod.AddEnchantment(new EfficientToolEnchantment());

            if (_config.AddMasterEnchantment && !fishingRod.hasEnchantmentOfType<MasterEnchantment>())
                _fishingRod.AddEnchantment(new MasterEnchantment());

            if (_config.AddPreservingEnchantment && !fishingRod.hasEnchantmentOfType<PreservingEnchantment>())
                _fishingRod.AddEnchantment(new PreservingEnchantment());
        }

        private void DoFishingRodAssistantTask()
        {
            if (_config.MaxCastPower) _fishingRod.Instance.castingPower = 1.01f;
            
            if (_config.InstantFishBite) _fishingRod.InstantFishBite();
            
            if (_config.AutoAttachBait) _fishingRod?.AutoAttachBait(_config.PreferBait, _config.InfiniteBait, _config.SpawnBaitIfDontHave, _config.BaitAmountToSpawn);
            
            if (_config.AutoAttachTackles) _fishingRod?.AutoAttachTackles(new []{_config.PreferTackle, _config.PreferAdvIridiumTackle} , _config.InfiniteTackle, _config.SpawnTackleIfDontHave);

            if (_bobberBar != null && _bobberBar.Instance.treasure) _fishingRod?.OverrideGoldenTreasureChance(_config.GoldenTreasureChance);
            
            if (!_modEntry.ModEnable) return;
            
            if (_config.AutoCastFishingRod) AutoCastFishingRod(_modEntry.FacingDirection);
            
            if (_config.AutoHookFish) _fishingRod.AutoHook();
        }

        private void AutoCastFishingRod(int playerFacingDirection)
        {
            if (!_fishingRod.IsRodNotInUse() || Game1.player.isRidingHorse() || Game1.isFestival())
                return;
            
            if (_autoCastDelay-- > 0)
                return;
            _autoCastDelay = 60;
            
            bool lowStamina = Game1.player.Stamina <= Game1.player.MaxStamina * (float)(_config.EnergyPercentToEat / 100f);
            bool hasEnchantment = _fishingRod.Instance.hasEnchantmentOfType<EfficientToolEnchantment>();
            bool isInventoryFull = Game1.player.isInventoryFull();
            
            if (lowStamina && !hasEnchantment)
            {
                if (!_config.AutoEatFood || _config.AutoEatFood && !TryAutoEatFood())
                {
                    CommonHelper.PushErrorNotification(I18n.HudMessage_LowStamina());
                    _modEntry.ForceDisable();
                }
            }
            
            if (isInventoryFull)
            {
                CommonHelper.PushErrorNotification(I18n.HudMessage_InventoryFull());
                _modEntry.ForceDisable();
            }
            
            if ((!lowStamina || hasEnchantment) && !isInventoryFull)
            {
                Game1.player.faceDirection(playerFacingDirection);
                Game1.pressUseToolButton();
            }
        }
        
        private bool TryAutoEatFood()
        {
            var player = Game1.player;
            if (!player.isEating && _fishingRod.IsRodNotInUse())
            {
                IEnumerable<Object> items = player.Items.OfType<Object>();
                Object bestItem = null;
                
                foreach (var item in items)
                {
                    if (item.Edibility <= 0 || (item.Category == -4 && !_config.AllowEatingFish))
                        continue;

                    if (bestItem == null || bestItem.Edibility / bestItem.salePrice() < item.Edibility / item.salePrice())
                        bestItem = item;
                }

                if (bestItem != null)
                {
                    player.eatObject(bestItem);
                    bestItem.Stack--;
                    if (bestItem.Stack == 0) player.removeItemFromInventory(bestItem);

                    CommonHelper.PushWarnNotification(I18n.HudMessage_AutoEatFood(), ItemRegistry.GetData(bestItem.QualifiedItemId).DisplayName);
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Bobber Bar

        internal void OnFishingMiniGameStart(BobberBar bobberBar)
        {
            IsInFishingMiniGame = true;
            _bobberBar = new SBobberBar(bobberBar);

            DoBobberBarAssistantTask();
        }
        
        internal void OnFishingMiniGameEnd()
        {
            IsInFishingMiniGame = false;
            _bobberBar = null;
            _catchingTreasure = false;
        }

        private void DoBobberBarAssistantTask()
        {
            _bobberBar.HandleFishPreview(_config.DisplayFishPreview);
            
            _bobberBar.OverrideFishDifficult(_config.FishDifficultyMultiplier, _config.FishDifficultyAdditive);
            
            _bobberBar.OverrideTreasureChance(_config.TreasureChance, _config.GoldenTreasureChance);
            
            if (_config.InstantCatchTreasure || _config.InstantCatchFish) 
                _bobberBar.InstantCatchTreasure(_modEntry.CatchTreasure);
            
            if (_config.InstantCatchFish) _bobberBar.InstantCatchFish();
        }

        #endregion

        #region Update

        internal void DoOnUpdateAssistantTask()
        {
            if (!_modEntry.ModEnable) return;
            
            if (IsInFishingMiniGame && _config.AutoPlayMiniGame) AutoPlayFishingMiniGame();
            
            if (_config.AutoClosePopup) AutoCloseFishPopup();

            if (_isInTreasureChestMenu && _config.AutoLootTreasure) AutoLootTreasure();
        }

        private void AutoPlayFishingMiniGame()
        {
            var bar = _bobberBar?.Instance;
            var rod = _fishingRod?.Instance;
            
            float fishPos = bar.bobberPosition;
            int bobberBarCenter = (bar.bobberBarHeight / 2);
            
            if (bar is { fadeOut: true, scale: <= 0.1f })
            {
                _bobberBar?.AlwaysPerfect(_config.AlwaysPerfect);
                _bobberBar?.OverrideFishQuality(_config.PreferFishQuality);
                _bobberBar?.OverrideFishSize(_config.AlwaysMaxFishSize);
                
                bar.scale = 0.0f;
                bar.fadeOut = false;
                string qualifiedItemId = Game1.player.CurrentTool is FishingRod currentTool ? currentTool.GetBait()?.QualifiedItemId : (string) null;
                int numCaught = bar.bossFish ? 1 : qualifiedItemId == "(O)774" && Game1.random.NextDouble() < 0.25 + Game1.player.DailyLuck / 2.0 ? 2 : _config.PreferFishAmount;
                if (bar.challengeBaitFishes > 0)
                    numCaught = bar.challengeBaitFishes;
                if (bar.distanceFromCatching > 0.8999999761581421 && rod != null)
                {
                    rod.pullFishFromWater(
                        bar.whichFish,
                        bar.fishSize,
                        bar.fishQuality,
                        (int) bar.difficulty,
                        bar.treasureCaught,
                        bar.perfect,
                        bar.fromFishPond,
                        bar.setFlagOnCatch,
                        bar.bossFish,
                        numCaught);
                }
                else
                {
                    Game1.player.completelyStopAnimatingOrDoingAction();
                    rod?.doneFishing(Game1.player, true);
                }
                Game1.exitActiveMenu();
                Game1.setRichPresence("location", Game1.currentLocation.Name);
            }
            
            if (_catchingTreasure && bar.distanceFromCatching < 0.2f)
            {
                _catchingTreasure = false;
                fishPos = bar.bobberPosition;
            }
            else if (_modEntry.CatchTreasure && bar.treasure && !bar.treasureCaught && (bar.distanceFromCatching > 0.8f || _catchingTreasure))
            {
                _catchingTreasure = true; 
                fishPos = bar.treasurePosition;
            }
            
            fishPos += 25;
            bar.bobberBarSpeed = (fishPos - bar.bobberBarPos - bobberBarCenter) / 2;
        }

        private void AutoCloseFishPopup()
        {
            if (_fishingRod != null && _fishingRod.IsRodShowingFish() && !Game1.isFestival())
            {
                if (_autoClosePopupDelay-- > 0)
                    return;
                _autoClosePopupDelay = 30;
                
                SimulateLeftMouseClick();
                _fishingRod.Instance.tickUpdate(Game1.currentGameTime, Game1.player);
                _currentMouseState?.SetValue(new MouseState());
                Game1.oldKBState = new KeyboardState(); 
            }

            return;

            void SimulateLeftMouseClick()
            {
                if (_currentMouseState == null) return;
            
                foreach (var button in Game1.options.useToolButton)
                {
                    if (button.key != Keys.None) 
                    {
                        Game1.oldKBState = new KeyboardState(button.key); 
                    }
                    
                    if (button.mouseLeft)
                    {
                        _currentMouseState.SetValue(new MouseState(
                            Game1.viewport.X / 2, 
                            Game1.viewport.Y / 2, 
                            0, 
                            ButtonState.Pressed,
                            ButtonState.Released, 
                            ButtonState.Released,
                            ButtonState.Released,
                            ButtonState.Released,
                            0));
                    }
                }
            }
        }

        internal void OnTreasureMenuOpen(ItemGrabMenu itemGrabMenu)
        {
            _treasureChestMenu = itemGrabMenu;
            _isInTreasureChestMenu = true;
        }
        
        internal void OnTreasureMenuClose()
        {
            _isInTreasureChestMenu = false;
            _treasureChestMenu = null;
        }

        private void AutoLootTreasure()
        {
            if (_autoLootDelay-- > 0) return;
            _autoLootDelay = 30;

            IList<Item> actualInventory = _treasureChestMenu.ItemsToGrabMenu.actualInventory;
            
            if (actualInventory.Count == excludeItems.Count)
            {
                excludeItems.Clear();
                
                if (actualInventory.Count == 0)
                {
                    _treasureChestMenu.exitThisMenu();
                    _treasureChestMenu = null;
                }
                else
                {
                    if (_config.ActionIfInventoryFull == ActionOnInventoryFull.Drop.ToString())
                    {
                        _treasureChestMenu.DropRemainingItems();
                        _treasureChestMenu.exitThisMenu();
                    }
                    else if (_config.ActionIfInventoryFull == ActionOnInventoryFull.Discard.ToString())
                    {
                        _treasureChestMenu.exitThisMenu();
                    }

                    _treasureChestMenu = null;
                    CommonHelper.PushErrorNotification(I18n.HudMessage_InventoryFull());
                    _modEntry.ForceDisable();
                }
            }
            else
            {
                Item obj = actualInventory[excludeItems.Count];
                if (obj != null)
                {
                    if (obj.QualifiedItemId == "(O)102")
                    {
                        Game1.player.foundArtifact(obj.QualifiedItemId, 1);
                        Game1.playSound("fireball");
                        actualInventory.RemoveAt(excludeItems.Count);
                    }
                    else
                    {
                        if (Game1.player.addItemToInventoryBool(obj))
                        {
                            Game1.playSound("coin");
                            actualInventory.RemoveAt(excludeItems.Count);
                        }
                        else
                        {
                            excludeItems.Add(obj);
                        }
                    }
                    
                    _autoLootDelay = 10;
                }
            }
        }

        #endregion

        #region On Time Changed

        internal void DoOnTimeChangedAssistantTask()
        {
            if (!_modEntry.ModEnable) return;
            
            if (_config.AutoPauseFishing != PauseFishingBehaviour.Off.ToString()) AutoPauseFishing(_config.AutoPauseFishing, _config.NumToWarn);
        }
        
        private void AutoPauseFishing(string autoPauseFishing, int numToWarn)
        {
            if (Game1.timeOfDay >= _config.PauseFishingTime * 100 && NumWarnThisDay < numToWarn)
            {
                NumWarnThisDay++;
                CommonHelper.PushErrorNotification(I18n.HudMessage_AutoDisable(), Game1.getTimeOfDayString(Game1.timeOfDay));
                if (autoPauseFishing == PauseFishingBehaviour.WarnAndPause.ToString())
                {
                    _modEntry.ForceDisable();
                    if (!Game1.IsMultiplayer) Game1.activeClickableMenu = new GameMenu();
                }
            }
        }

        #endregion

        #region On Inventory Changed

        internal void AutoTrashJunk(InventoryChangedEventArgs e)
        {
            if (!_modEntry.ModEnable || !_config.AutoTrashJunk) return;
            
            List<Item> changedItems = new List<Item>();
            changedItems.AddRange(e.Added);
            changedItems.AddRange(e.QuantityChanged.Select(newStacks => newStacks.Item));
            
            foreach (Item newItem in changedItems)
            {
                int sellToStorePrice = Utility.getSellToStorePriceOfItem(newItem, false);
                
                // Category value for trash is -20. Source: https://github.com/veywrn/StardewValley/blob/master/StardewValley/Item.cs
                if (newItem.canBeTrashed() && (newItem.Category == -20 || sellToStorePrice < _config.JunkHighestPrice))
                {
                    Utility.trashItem(newItem);
                    e.Player.removeItemFromInventory(newItem);
                    CommonHelper.PushWarnNotification(I18n.HudMessage_AutoTrashJunk(), newItem.DisplayName, newItem.Stack);
                }
            }
        }

        #endregion
    }
}
using ChibiKyu.StardewMods.Common;
using FishingAssistant2;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace ChibiKyu.StardewMods.FishingAssistant2.Frameworks
{
    internal class Assistant(Func<ModEntry> modEntry, Func<ModConfig> modConfig)
    {
        private readonly IList<Item> _excludeItems = new List<Item>();

        private int _autoCastDelay = 60;

        private int _autoClosePopupDelay = 30;

        private int _autoLootDelay = 30;

        private SBobberBar? _bobberBar;

        private bool _catchingTreasure;

        private int _facingDirection;

        private bool _facingDirectionCached;

        private SFishingRod? _fishingRod;

        private bool _isInTreasureChestMenu;

        private ItemGrabMenu? _treasureChestMenu;

        internal bool IsInFishingMiniGame;

        internal int NumWarnThisDay;

        #region On Inventory Changed

        internal void AutoTrashJunk(InventoryChangedEventArgs e)
        {
            if (!modEntry().AutomationEnable || !modConfig().AutoTrashJunk) return;

            List<Item> changedItems = new();
            changedItems.AddRange(e.Added);
            changedItems.AddRange(e.QuantityChanged.Select(newStacks => newStacks.Item));

            foreach (Item newItem in changedItems)
            {
                int sellToStorePrice = Utility.getSellToStorePriceOfItem(newItem, false);

                if (newItem.canBeTrashed() && (newItem.Category == Object.junkCategory ||
                                               (newItem.Category == Object.FishCategory && sellToStorePrice <= modConfig().JunkHighestPrice && modConfig().AllowTrashFish) ||
                                               (newItem.Category != Object.FishCategory && sellToStorePrice <= modConfig().JunkHighestPrice && SaveToTrashed(newItem))))
                {
                    Utility.trashItem(newItem);
                    e.Player.removeItemFromInventory(newItem);
                    CommonHelper.PushWarnNotification(I18n.HudMessage_AutoTrashJunk(), newItem.DisplayName, newItem.Stack);
                }
            }

            return;

            bool SaveToTrashed(Item itemToTrash)
            {
                return itemToTrash is not (MeleeWeapon or FishingRod or Pan or Slingshot);
            }
        }

        #endregion

        #region FishingRod

        public void OnEquipFishingRod(FishingRod fishingRod, bool isModEnable)
        {
            if (_fishingRod == null)
                OverrideFishingRod(fishingRod);
            else if (_fishingRod.Instance != fishingRod)
            {
                OnUnEquipFishingRod();
                OverrideFishingRod(fishingRod);
            }

            if (isModEnable && !Game1.player.isMoving())
                CachePlayerPosition();
            else
                ForgetPlayerPosition();

            DoFishingRodAssistantTask();
        }

        public void OnUnEquipFishingRod()
        {
            ForgetPlayerPosition();

            if (_fishingRod == null) return;

            if (modConfig().RemoveWhenUnequipped) _fishingRod.ClearAddedEnchantments();

            _fishingRod = null;
        }

        private void OverrideFishingRod(FishingRod fishingRod)
        {
            _fishingRod = new SFishingRod(fishingRod, modConfig);

            if (modConfig().AddAutoHookEnchantment && !_fishingRod.Instance.hasEnchantmentOfType<AutoHookEnchantment>())
                _fishingRod.AddEnchantment(new AutoHookEnchantment());

            if (modConfig().AddEfficientEnchantment && !_fishingRod.Instance.hasEnchantmentOfType<EfficientToolEnchantment>())
                _fishingRod.AddEnchantment(new EfficientToolEnchantment());

            if (modConfig().AddMasterEnchantment && !_fishingRod.Instance.hasEnchantmentOfType<MasterEnchantment>())
                _fishingRod.AddEnchantment(new MasterEnchantment());

            if (modConfig().AddPreservingEnchantment && !_fishingRod.Instance.hasEnchantmentOfType<PreservingEnchantment>())
                _fishingRod.AddEnchantment(new PreservingEnchantment());
        }

        private void DoFishingRodAssistantTask()
        {
            if (_fishingRod == null) return;

            _fishingRod.OverrideCastPower();

            if (modConfig().InstantFishBite) _fishingRod.OverrideTimeUntilFishBite();

            if (modConfig().AutoAttachBait) _fishingRod.AutoAttachBait();

            if (modConfig().AutoAttachTackles) _fishingRod.AutoAttachTackles();

            if (modConfig().InfiniteBait) _fishingRod.InfiniteBait();

            if (modConfig().InfiniteTackle) _fishingRod.InfiniteTackles();

            if (_bobberBar is { Instance.treasure: true }) _fishingRod.OverrideGoldenTreasureChance();

            if (!modEntry().AutomationEnable) return;

            if (modConfig().AutoCastFishingRod) AutoCastFishingRod();

            if (modConfig().AutoHookFish) _fishingRod.AutoHook();
        }

        private void AutoCastFishingRod()
        {
            if (_fishingRod != null && (!_fishingRod.IsRodCanCast() || Game1.isFestival()))
                return;

            if (_autoCastDelay-- > 0) return;

            _autoCastDelay = 60;

            bool lowStamina = modConfig().AutoEatFood
                ? Game1.player.Stamina <= Game1.player.MaxStamina * (modConfig().EnergyPercentToEat / 100f)
                : Game1.player.Stamina <= 8.0 - Game1.player.FishingLevel * 0.1;

            bool hasEnchantment = _fishingRod.Instance.hasEnchantmentOfType<EfficientToolEnchantment>();
            bool isInventoryFull = Game1.player.isInventoryFull();

            if (lowStamina && !hasEnchantment)
            {
                if (!modConfig().AutoEatFood || (modConfig().AutoEatFood && !TryAutoEatFood()))
                {
                    CommonHelper.PushErrorNotification(I18n.HudMessage_LowStamina());
                    modEntry().ForceDisable();
                }
            }

            if (isInventoryFull)
            {
                CommonHelper.PushErrorNotification(I18n.HudMessage_InventoryFull());
                modEntry().ForceDisable();
            }

            if ((lowStamina && !hasEnchantment) || isInventoryFull) return;

            Game1.player.faceDirection(_facingDirection);
            Game1.pressUseToolButton();

            return;

            bool TryAutoEatFood()
            {
                Farmer? player = Game1.player;

                if (player.isEating || _fishingRod == null || !_fishingRod.IsRodNotInUse()) return false;

                IEnumerable<Object> items = player.Items.OfType<Object>();
                Object? bestItem = null;

                foreach (Object item in items)
                {
                    if (item.Edibility <= 0 || (item.Category == Object.FishCategory && !modConfig().AllowEatingFish))
                        continue;

                    if (bestItem == null || bestItem.Edibility / bestItem.salePrice() < item.Edibility / item.salePrice())
                        bestItem = item;
                }

                if (bestItem == null) return false;

                player.eatObject(bestItem);
                bestItem.Stack--;
                if (bestItem.Stack == 0) player.removeItemFromInventory(bestItem);

                CommonHelper.PushWarnNotification(I18n.HudMessage_AutoEatFood(), ItemRegistry.GetData(bestItem.QualifiedItemId).DisplayName);

                return true;
            }
        }

        internal void GiveStarterFishingRod(string startWithFishingRod)
        {
            if (Game1.dayOfMonth != 1 || !ShouldGivePlayerFishingRod())
                return;

            ParsedItemData? parsedItem = ItemRegistry.GetDataOrErrorItem(startWithFishingRod);
            Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create(parsedItem.QualifiedItemId));

            return;

            bool ShouldGivePlayerFishingRod()
            {
                return !Game1.player.Items.OfType<FishingRod>().Any() && startWithFishingRod != "None";
            }
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
            if (_bobberBar == null) return;

            _bobberBar.HandleFishPreview(modConfig().DisplayFishPreview);

            _bobberBar.OverrideFishDifficult(modConfig().FishDifficultyMultiplier, modConfig().FishDifficultyAdditive);

            _bobberBar.OverrideTreasureChance(modConfig().TreasureChance, modConfig().GoldenTreasureChance);
        }

        #endregion

        #region On Update Ticked

        internal void DoOnUpdateAssistantTask()
        {
            if (_bobberBar != null && _fishingRod != null)
            {
                BobberBar bar = _bobberBar.Instance;
                FishingRod rod = _fishingRod.Instance;

                _bobberBar.AlwaysPerfect(modConfig().AlwaysPerfect);

                _bobberBar.OverrideFishQuality(modConfig().PreferFishQuality);

                _bobberBar.OverrideFishSize(modConfig().AlwaysMaxFishSize);

                if (modConfig().InstantCatchTreasure && bar.treasureScale >= 1.0f) _bobberBar.InstantCatchTreasure(modConfig().AutoPlayMiniGame, modEntry().CatchTreasure);

                _fishingRod.OverrideNumberOfFishCaught(modConfig().PreferFishAmount, _bobberBar.Instance);

                if (bar is { fadeOut: true, scale: <= 0.1f } || ShouldSkipMiniGame())
                {
                    bar.scale = 0.0f;
                    bar.fadeOut = false;

                    if (ShouldSkipMiniGame())
                    {
                        _bobberBar.InstantCatchFish();
                        _bobberBar.InstantCatchTreasure(true, modEntry().CatchTreasure);
                    }

                    if (bar.distanceFromCatching >= 0.8999f)
                    {
                        rod.pullFishFromWater(bar.whichFish, bar.fishSize, bar.fishQuality, (int)bar.difficulty, bar.treasureCaught, bar.perfect, bar.fromFishPond, bar.setFlagOnCatch, bar.bossFish,
                            rod.numberOfFishCaught);
                    }
                    else
                    {
                        Game1.player.completelyStopAnimatingOrDoingAction();
                        rod.doneFishing(Game1.player, true);
                    }

                    Game1.exitActiveMenu();
                    Game1.setRichPresence("location", Game1.currentLocation.Name);
                }
            }

            if (!modEntry().AutomationEnable) return;

            if (IsInFishingMiniGame && modConfig().AutoPlayMiniGame) AutoPlayFishingMiniGame();

            if (modConfig().AutoClosePopup) AutoCloseFishPopup();

            if (_isInTreasureChestMenu && modConfig().AutoLootTreasure) AutoLootTreasure();
        }

        private void AutoPlayFishingMiniGame()
        {
            if (_fishingRod == null || _bobberBar == null) return;

            BobberBar bar = _bobberBar.Instance;

            float fishPos = bar.bobberPosition;
            int bobberBarCenter = bar.bobberBarHeight / 2;

            if (_catchingTreasure && bar.distanceFromCatching < 0.2f)
            {
                _catchingTreasure = false;
                fishPos = bar.bobberPosition;
            }
            else if (modEntry().CatchTreasure && bar is { treasure: true, treasureCaught: false } && (bar.distanceFromCatching > 0.8f || _catchingTreasure))
            {
                _catchingTreasure = true;
                fishPos = bar.treasurePosition;
            }

            fishPos += 25;
            bar.bobberBarSpeed = (fishPos - bar.bobberBarPos - bobberBarCenter) / 2;
        }

        private bool ShouldSkipMiniGame()
        {
            if (modConfig().SkipFishingMiniGame == Enum.GetName(SkipFishingMiniGame.SkipAll)) return true;

            return modConfig().SkipFishingMiniGame == Enum.GetName(SkipFishingMiniGame.SkipOnlyCaught) && AlreadyCaughtFish();
        }

        private void AutoCloseFishPopup()
        {
            if (_fishingRod != null && _fishingRod.IsRodShowingFish() && !Game1.isFestival())
            {
                if (_autoClosePopupDelay-- > 0)
                    return;

                _autoClosePopupDelay = 30;

                SimulateUseToolPressed(() => _fishingRod.Instance.tickUpdate(Game1.currentGameTime, Game1.player));
            }

            return;

            void SimulateUseToolPressed(Action? callback = null)
            {
                foreach (InputButton button in Game1.options.useToolButton)
                {
                    if (button.key != Keys.None)
                        Game1.oldKBState = new KeyboardState(button.key);
                }

                callback?.Invoke();

                Game1.oldKBState = new KeyboardState();
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
            if (_treasureChestMenu == null) return;

            if (_autoLootDelay-- > 0) return;

            _autoLootDelay = 30;

            IList<Item> actualInventory = _treasureChestMenu.ItemsToGrabMenu.actualInventory;

            if (actualInventory.Count == _excludeItems.Count)
            {
                _excludeItems.Clear();

                if (actualInventory.Count == 0)
                {
                    _treasureChestMenu.exitThisMenu();
                    _treasureChestMenu = null;
                }
                else
                {
                    if (modConfig().ActionIfInventoryFull == ActionOnInventoryFull.Drop.ToString())
                    {
                        _treasureChestMenu.DropRemainingItems();
                        _treasureChestMenu.exitThisMenu();
                    }
                    else if (modConfig().ActionIfInventoryFull == ActionOnInventoryFull.Discard.ToString()) _treasureChestMenu.exitThisMenu();

                    _treasureChestMenu = null;
                    CommonHelper.PushErrorNotification(I18n.HudMessage_InventoryFull());
                    modEntry().ForceDisable();
                }
            }
            else
            {
                Item item = actualInventory[_excludeItems.Count];

                if (item.QualifiedItemId == "(O)102")
                {
                    Game1.player.foundArtifact(item.QualifiedItemId, 1);
                    Game1.playSound("fireball");
                    actualInventory.RemoveAt(_excludeItems.Count);
                }
                else
                {
                    if (Game1.player.addItemToInventoryBool(item))
                    {
                        Game1.playSound("coin");
                        actualInventory.RemoveAt(_excludeItems.Count);
                    }
                    else
                        _excludeItems.Add(item);
                }

                _autoLootDelay = 10;
            }
        }

        #endregion

        #region On Time Changed

        internal void DoOnTimeChangedAssistantTask()
        {
            if (!modEntry().AutomationEnable) return;

            if (modConfig().AutoPauseFishing != PauseFishingBehaviour.Off.ToString()) AutoPauseFishing(modConfig().AutoPauseFishing, modConfig().WarnCount);
        }

        private void AutoPauseFishing(string autoPauseFishing, int numToWarn)
        {
            if (Game1.timeOfDay >= modConfig().TimeToPause * 100 && NumWarnThisDay < numToWarn)
            {
                NumWarnThisDay++;
                CommonHelper.PushErrorNotification(I18n.HudMessage_AutoDisable(), Game1.getTimeOfDayString(modConfig().TimeToPause * 100));

                if (autoPauseFishing == PauseFishingBehaviour.WarnAndPause.ToString())
                {
                    modEntry().ForceDisable();
                    if (!Game1.IsMultiplayer) Game1.activeClickableMenu = new GameMenu();
                }
            }
        }

        #endregion

        #region Other

        private void CachePlayerPosition()
        {
            if (_facingDirectionCached) return;

            _facingDirection = Game1.player.getDirection() != -1 ? Game1.player.getDirection() : Game1.player.FacingDirection;
            _facingDirectionCached = true;
        }

        private void ForgetPlayerPosition()
        {
            if (!_facingDirectionCached) return;

            _facingDirection = 0;
            _facingDirectionCached = false;
        }

        public void OnAutomationStateChange(bool toggle)
        {
            if (toggle)
                CachePlayerPosition();
            else
                ForgetPlayerPosition();

            if (_fishingRod != null)
            {
                _fishingRod.UnlockCastPowerTimer = modConfig().ParsedUnlockCastPowerTime;
                _fishingRod.SmartCastPower = 0;
                _fishingRod.SmartCastPowerSaved = false;
            }
        }

        public void OnConfigSaved()
        {
            if (_fishingRod == null) return;

            _fishingRod.UnlockCastPowerTimer = modConfig().ParsedUnlockCastPowerTime;
            _fishingRod.SmartCastPower = 0;
            _fishingRod.SmartCastPowerSaved = false;
        }

        internal bool AlreadyCaughtFish(int minCaught = 1)
        {
            return _bobberBar != null && _bobberBar.AlreadyCaughtFish(minCaught);
        }

        #endregion
    }
}

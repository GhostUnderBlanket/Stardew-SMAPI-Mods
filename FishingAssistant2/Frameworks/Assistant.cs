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
        private readonly IList<Item> _excludeItems = [];

        private List<Item?> _itemsToTrash = [];

        private SBobberBar? _bobberBar;

        private SFishingRod? _fishingRod;

        private ItemGrabMenu? _treasureChestMenu;

        private InventoryPage? _inventoryPage;

        private int _autoCastDelay = 60;

        private int _autoClosePopupDelay = 30;

        private int _autoLootDelay = 30;

        private bool _catchingTreasure;

        private int _facingDirection;

        private bool _facingDirectionCached;

        private bool _isInTreasureChestMenu;

        internal bool IsInFishingMiniGame;

        internal int NumWarnThisDay;

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

            AddEnchantments();

            return;

            void AddEnchantments()
            {
                if (modConfig().AddAutoHookEnchantment && !_fishingRod.Instance.hasEnchantmentOfType<AutoHookEnchantment>())
                    _fishingRod.AddEnchantment(new AutoHookEnchantment());

                if (modConfig().AddEfficientEnchantment && !_fishingRod.Instance.hasEnchantmentOfType<EfficientToolEnchantment>())
                    _fishingRod.AddEnchantment(new EfficientToolEnchantment());

                if (modConfig().AddMasterEnchantment && !_fishingRod.Instance.hasEnchantmentOfType<MasterEnchantment>())
                    _fishingRod.AddEnchantment(new MasterEnchantment());

                if (modConfig().AddPreservingEnchantment && !_fishingRod.Instance.hasEnchantmentOfType<PreservingEnchantment>())
                    _fishingRod.AddEnchantment(new PreservingEnchantment());
            }
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
                    CommonHelper.PushError(I18n.HudMessage_LowStamina());
                    modEntry().ForceDisable();
                }
            }

            if (isInventoryFull)
            {
                CommonHelper.PushError(I18n.HudMessage_InventoryFull());
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

                CommonHelper.PushWarning(bestItem, I18n.HudMessage_AutoEatFood(), ItemRegistry.GetData(bestItem.QualifiedItemId).DisplayName);

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
            DoPullFishFromWater();

            DoTrashRecoveryMenu();

            if (!modEntry().AutomationEnable) return;

            if (IsInFishingMiniGame && modConfig().AutoPlayMiniGame) AutoPlayFishingMiniGame();

            if (modConfig().AutoClosePopup) AutoCloseFishPopup();

            if (_isInTreasureChestMenu && modConfig().AutoLootTreasure) AutoLootTreasure();
        }

        private void DoPullFishFromWater()
        {
            if (_bobberBar == null || _fishingRod == null) return;

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

            return;

            bool ShouldSkipMiniGame()
            {
                if (modConfig().SkipFishingMiniGame == Enum.GetName(SkipFishingMiniGame.SkipAll)) return true;

                return modConfig().SkipFishingMiniGame == Enum.GetName(SkipFishingMiniGame.SkipOnlyCaught) && AlreadyCaughtFish();
            }
        }

        private void DoTrashRecoveryMenu()
        {
            if (!OnTrashCanReceiveRightClicked()) return;

            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu.readyToClose()) Game1.activeClickableMenu.exitThisMenu();

            ItemGrabMenu.organizeItemsInList(_itemsToTrash);
            ItemGrabMenu trashRecoveryMenu = new(_itemsToTrash, this);
            trashRecoveryMenu.behaviorOnItemGrab += AddTrashToIgnoreList;
            trashRecoveryMenu.exitFunction += OnExit;
            Game1.activeClickableMenu = trashRecoveryMenu;

            return;

            bool OnTrashCanReceiveRightClicked()
            {
                return _inventoryPage != null && Game1.player.CursorSlotItem == null && Game1.input.GetMouseState().RightButton == ButtonState.Pressed &&
                       _inventoryPage.trashCan.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true));
            }

            void AddTrashToIgnoreList(Item item, Farmer who)
            {
                if (modConfig().JunkIgnoreList.Contains(item.QualifiedItemId)) return;

                modConfig().JunkIgnoreList.Add(item.QualifiedItemId);
                modEntry().Helper.WriteConfig(modConfig());

                CommonHelper.PushWarning(item, I18n.HudMessage_AddToIgnoreList(), item.DisplayName);
            }

            void OnExit()
            {
                _itemsToTrash = trashRecoveryMenu.ItemsToGrabMenu.actualInventory.ToList();
                _itemsToTrash.RemoveAll(trash => trash == null);
            }
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

        private void AutoCloseFishPopup()
        {
            if (_fishingRod == null || !_fishingRod.IsRodShowingFish() || Game1.isFestival()) return;

            if (_autoClosePopupDelay-- > 0)
                return;

            _autoClosePopupDelay = 30;

            SimulateUseToolPressed(() => _fishingRod.Instance.tickUpdate(Game1.currentGameTime, Game1.player));

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
                    CommonHelper.PushError(I18n.HudMessage_InventoryFull());
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

            if (modConfig().AutoPauseFishing != Enum.GetName(PauseFishingBehaviour.Off)) AutoPauseFishing(modConfig().AutoPauseFishing, modConfig().WarnCount);
        }

        private void AutoPauseFishing(string autoPauseFishing, int numToWarn)
        {
            if (Game1.timeOfDay < modConfig().TimeToPause * 100 || NumWarnThisDay >= numToWarn) return;

            NumWarnThisDay++;
            CommonHelper.PushError(I18n.HudMessage_AutoDisable(), Game1.getTimeOfDayString(modConfig().TimeToPause * 100));

            if (autoPauseFishing != Enum.GetName(PauseFishingBehaviour.WarnAndPause)) return;

            modEntry().ForceDisable();
            if (!Game1.IsMultiplayer) Game1.activeClickableMenu = new GameMenu();
        }

        #endregion

        #region On Inventory Changed

        internal void AutoTrashJunk(InventoryChangedEventArgs e)
        {
            if (!modEntry().AutomationEnable || !modConfig().AutoTrashJunk) return;

            List<Item> changedItems = [];
            changedItems.AddRange(e.Added);
            changedItems.AddRange(e.QuantityChanged.Select(newStacks => newStacks.Item));

            foreach (Item newItem in changedItems.Where(newItem => CanTrashThisItem(newItem) && TrashConditionsMet(newItem)))
            {
                _itemsToTrash = _itemsToTrash.Prepend(newItem).ToList();
                e.Player.removeItemFromInventory(newItem);
                Game1.playSound("trashcan");
                CommonHelper.PushWarning(newItem, I18n.HudMessage_MoveJunkToTrashCan(), newItem.DisplayName, newItem.Stack);
            }

            return;

            bool CanTrashThisItem(Item itemToTrash)
            {
                return itemToTrash.canBeTrashed() && !modConfig().JunkIgnoreList.Contains(itemToTrash.QualifiedItemId);
            }

            bool TrashConditionsMet(Item itemToTrash)
            {
                int sellToStorePrice = Utility.getSellToStorePriceOfItem(itemToTrash, false);

                return itemToTrash.Category == Object.junkCategory || (sellToStorePrice <= modConfig().JunkHighestPrice &&
                                                                       ((itemToTrash.Category == Object.FishCategory && modConfig().AllowTrashFish) ||
                                                                        (itemToTrash.Category != Object.FishCategory && SaveToTrashed(itemToTrash))));
            }

            bool SaveToTrashed(Item itemToTrash)
            {
                return itemToTrash is not (MeleeWeapon or FishingRod or Pan or Slingshot);
            }
        }

        internal void ActualTrashJunk()
        {
            if (_itemsToTrash.Count <= 0) return;

            CommonHelper.PushWarning(I18n.HudMessage_ActualTrashJunk(), _itemsToTrash.Count);

            foreach (Item? item in _itemsToTrash)
            {
                if (item is Object && Game1.player.specialItems.Contains(item.ItemId))
                    Game1.player.specialItems.Remove(item.ItemId);

                if (Utility.getTrashReclamationPrice(item, Game1.player) > 0)
                    Game1.player.Money += Utility.getTrashReclamationPrice(item, Game1.player);
            }

            Game1.playSound("trashcan");

            _itemsToTrash.Clear();
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

            _fishingRod?.ResetSmartCastPower();
        }

        internal void OnGameMenuOpen(GameMenu gameMenu)
        {
            _inventoryPage = gameMenu.pages[0] as InventoryPage;
        }

        internal void OnGameMenuClose()
        {
            _inventoryPage = null;
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

        public void OnConfigSaved()
        {
            _fishingRod?.ResetSmartCastPower();
        }

        internal bool AlreadyCaughtFish(int minCaught = 1)
        {
            return _bobberBar != null && _bobberBar.AlreadyCaughtFish(minCaught);
        }

        #endregion
    }
}

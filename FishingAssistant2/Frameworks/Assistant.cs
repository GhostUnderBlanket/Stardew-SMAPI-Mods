using ChibiKyu.StardewMods.Common;
using FishingAssistant2;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Menus;
using StardewValley.Tools;

namespace ChibiKyu.StardewMods.FishingAssistant2.Frameworks
{
    internal class Assistant(Func<ModEntry> modEntry, Func<ModConfig> modConfig)
    {
        internal bool IsInFishingMiniGame;
        private bool _isInTreasureChestMenu;
        
        private readonly ModConfig _config = modConfig();
        private readonly ModEntry _modEntry = modEntry();
        
        private SBobberBar? _bobberBar;
        private SFishingRod? _fishingRod;
        private ItemGrabMenu? _treasureChestMenu;
        private IList<Item> excludeItems = new List<Item>();
        
        private int _autoLootDelay = 30;
        private int _autoClosePopupDelay = 30;
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

            if (!_modEntry.ModEnable) return;
            
            if (_config.AutoCastFishingRod)
            {
                var response = _fishingRod.AutoCastFishingRod(_modEntry.FacingDirection);
                switch (response)
                {
                    case AutoActionResponse.LowStamina:
                        CommonHelper.PushErrorNotification(I18n.HudMessage_LowStamina());
                        _modEntry.ForceDisable();
                        break;
                    case AutoActionResponse.InventoryFull:
                        CommonHelper.PushErrorNotification(I18n.HudMessage_InventoryFull());
                        _modEntry.ForceDisable();
                        break;
                }
            }
            
            if (_config.AutoHookFish) _fishingRod.AutoHook();
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
            _bobberBar.OverrideFishDifficult(_config.FishDifficultyMultiplier, _config.FishDifficultyAdditive);
            
            _bobberBar.OverrideTreasureChance(_config.TreasureChance);
            
            if (_config.InstantCatchTreasure || _config.InstantCatchFish) 
                _bobberBar.InstantCatchTreasure(_modEntry.CatchTreasure);
            
            if (_config.InstantCatchFish) _bobberBar.InstantCatchFish();
        }

        #endregion

        #region Update

        internal void DoOnUpdateAssistantTask()
        {
            if (_config.AutoAttachBait) _fishingRod?.AutoAttachBait(_config.PreferBait, _config.InfiniteBait, _config.SpawnBaitIfDontHave);
            
            if (_config.AutoAttachTackles) _fishingRod?.AutoAttachTackles(_config.PreferTackle, _config.InfiniteTackle, _config.SpawnTackleIfDontHave);
            
            if (!_modEntry.ModEnable) return;
            
            if (IsInFishingMiniGame && _config.AutoPlayMiniGame) AutoPlayFishingMiniGame();
            
            if (_config.AutoClosePopup) AutoCloseFishPopup();

            if (_isInTreasureChestMenu && _config.AutoLootTreasure) AutoLootTreasure();
        }

        private void AutoPlayFishingMiniGame()
        {
            var bar = _bobberBar.Instance;
            
            float fishPos = bar.bobberPosition;
            int bobberBarCenter = (bar.bobberBarHeight / 2);

            _bobberBar.AlwaysPerfect(_config.AlwaysPerfect);
            _bobberBar.OverrideFishQuality(_config.PreferFishQuality);
            _bobberBar.OverrideFishSize(_config.AlwaysMaxFishSize);
            
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
                    }
                    else
                    {
                        if (Game1.player.addItemToInventoryBool(obj))
                        {
                            Game1.playSound("coin");
                        }
                        else
                        {
                            excludeItems.Add(obj);
                        }
                    }
                    
                    actualInventory.RemoveAt(excludeItems.Count);
                    _autoLootDelay = 10;
                }
            }
        }

        #endregion
    }
}
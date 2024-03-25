using ChibiKyu.StardewMods.Common;
using FishingAssistant2;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Menus;
using StardewValley.Tools;

namespace ChibiKyu.StardewMods.FishingAssistant2.Frameworks
{
    internal class Assistant(Func<ModEntry> modEntry, Func<ModConfig> modConfig)
    {
        public bool IsInFishingMiniGame;
        
        private readonly ModConfig _config = modConfig();
        private readonly ModEntry _modEntry = modEntry();
        
        private SBobberBar _bobberBar;
        private SFishingRod _fishingRod;
        private ItemGrabMenu? _treasureChestMenu;
        
        private int _autoLootDelay = 30;
        private int _endMiniGameDelay = 15;
        
        private bool _catchingTreasure;
        
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

            if (_config.AutoCastFishingRod && _modEntry.ModEnable)
            {
                var response = _fishingRod.DoAutoCastFishingRod(_modEntry.FacingDirection);
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
            
            if (_config.InstantFishBite) _fishingRod.InstantFishBite();
            
            if (_config.AutoHookFish && _modEntry.ModEnable) _fishingRod.AutoHook();

            if (_config.AutoClosePopup && _modEntry.ModEnable) _fishingRod.AutoCloseFishPopup();
        }

        #endregion

        #region Bobber Bar

        public void OnFishingMiniGameStart(BobberBar bobberBar)
        {
            IsInFishingMiniGame = true;
            _bobberBar = new SBobberBar(bobberBar);

            DoBobberBarAssistantTask();
        }
        
        public void OnFishingMiniGameEnd()
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

        public void DoOnUpdateAssistantTask()
        {
            if (!_modEntry.ModEnable) return;

            if (IsInFishingMiniGame && _config.AutoPlayMiniGame) AutoPlayFishingMiniGame();

            if (_config.AutoLootTreasure && _treasureChestMenu != null) AutoLootTreasure();
        }
        
        public void AutoPlayFishingMiniGame()
        {
            var bar = _bobberBar.Instance;
            
            float fishPos = bar.bobberPosition;
            int bobberBarCenter = (bar.bobberBarHeight / 2);

            _bobberBar.AlwaysPerfect(_config.AlwaysPerfect);
            
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

        public void OnTreasureMenuOpen(ItemGrabMenu itemGrabMenu)
        {
            _treasureChestMenu = itemGrabMenu;
        }

        public void AutoLootTreasure()
        {
            if (_autoLootDelay-- > 0) return;
            _autoLootDelay = 30;

            IList<Item> actualInventory = _treasureChestMenu.ItemsToGrabMenu.actualInventory;
            
            if (_treasureChestMenu.areAllItemsTaken())
            {
                _treasureChestMenu.exitThisMenu();
                _treasureChestMenu = null;
                _autoLootDelay = 30;
            }
            else
            {
                Item obj = actualInventory.First();
                if (obj == null) return;
                
                if (obj.QualifiedItemId == "(O)102")
                {
                    Game1.player.foundArtifact(obj.QualifiedItemId, 1);
                    Game1.playSound("fireball");
                }
                else
                {
                    if (!Game1.player.addItemToInventoryBool(obj))
                    {
                        _treasureChestMenu.DropRemainingItems();
                        _treasureChestMenu.exitThisMenu();
                        CommonHelper.PushErrorNotification(I18n.HudMessage_InventoryFull());
                        _modEntry.ForceDisable();
                    }
                    else
                    {
                        actualInventory.RemoveAt(0);
                        _autoLootDelay = 10;
                    }
                        
                    Game1.playSound("coin");
                }
            }
        }

        #endregion
    }
}
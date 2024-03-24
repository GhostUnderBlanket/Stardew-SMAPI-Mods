using ChibiKyu.StardewMods.Common;
using FishingAssistant2;
using FishingAssistant2.Frameworks;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
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
        private bool _catchingTreasure;

        private int _autoLootDelay = 30;
        private ItemGrabMenu? _treasureChestMenu;

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
            
            if (_config.RemoveEnchantmentWhenUnequipped) _fishingRod.Instance.ClearEnchantments();
            _fishingRod = null;
        }
        
        private void OverrideFishingRod(FishingRod fishingRod)
        {
            _fishingRod = new SFishingRod(fishingRod);

            if (_config.AddAutoHookEnchantment && !fishingRod.hasEnchantmentOfType<AutoHookEnchantment>())
                fishingRod.enchantments.Add(new AutoHookEnchantment());

            if (_config.AddEfficientEnchantment && !fishingRod.hasEnchantmentOfType<EfficientToolEnchantment>())
                fishingRod.enchantments.Add(new EfficientToolEnchantment());

            if (_config.AddMasterEnchantment && !fishingRod.hasEnchantmentOfType<MasterEnchantment>())
                fishingRod.enchantments.Add(new MasterEnchantment());

            if (_config.AddPreservingEnchantment && !fishingRod.hasEnchantmentOfType<PreservingEnchantment>())
                fishingRod.enchantments.Add(new PreservingEnchantment());
        }

        private void DoFishingRodAssistantTask()
        {
            if (_config.MaxCastPower) _fishingRod.Instance.castingPower = 1.01f;

            if (_config.AutoCastFishingRod)
            {
                var response = _fishingRod.DoAutoCastFishingRod(_modEntry._facingDirection, _modEntry._standingPosX, _modEntry._standingPosY);
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
            
            if (_config.AutoHookFish) _fishingRod.DoAutoHook();

            if (_config.AutoClosePopup) _fishingRod.DoAutoCloseFishPopup();
        }

        #endregion
        
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
                _bobberBar.InstantCatchTreasure(_modEntry._catchTreasure);
            
            if (_config.InstantCatchFish) _bobberBar.InstantCatchFish();
            
            if (_config.AlwaysPerfect) _bobberBar.AlwaysPerfect();
        }
        
        public void AutoPlayFishingMiniGame()
        {
            if (!IsInFishingMiniGame || !_config.AutoPlayMiniGame) return;
            
            var bar = _bobberBar.Instance;
            var rod = _fishingRod.Instance;
            
            float fishPos = bar.bobberPosition;
            int bobberBarCenter = (bar.bobberBarHeight / 2);

            if (bar.distanceFromCatching >= 1.0f)
            {
                EndFishingMiniGame();
            }
            else if (_catchingTreasure && bar.distanceFromCatching < 0.2f)
            {
                _catchingTreasure = false;
                fishPos = bar.bobberPosition;
            }
            else if (_modEntry._catchTreasure && bar.treasure && !bar.treasureCaught && (bar.distanceFromCatching > 0.8f || _catchingTreasure))
            {
                _catchingTreasure = true; 
                fishPos = bar.treasurePosition;
            }
            
            fishPos += 25;
            bar.bobberBarSpeed = (fishPos - bar.bobberBarPos - bobberBarCenter) / 2;
            
            return;
            
            void EndFishingMiniGame()
            {
                if (Game1.isFestival())
                    return;
            
                Game1.playSound("jingle1");
            
                string qualifiedItemId = Game1.player.CurrentTool is FishingRod currentTool ? currentTool.GetBait()?.QualifiedItemId : (string) null;
                int numCaught = bar.bossFish || qualifiedItemId != "(O)774" || Game1.random.NextDouble() >= 0.25 + Game1.player.DailyLuck / 2.0 ? 1 : 2;
                if (bar.challengeBaitFishes > 0)
                    numCaught = bar.challengeBaitFishes;

                rod.pullFishFromWater(bar.whichFish, bar.fishSize, bar.fishQuality, (int)bar.difficulty, bar.treasureCaught, bar.perfect, bar.fromFishPond, bar.setFlagOnCatch, bar.bossFish, numCaught);
                Game1.exitActiveMenu();

                Game1.setRichPresence("location", Game1.currentLocation.Name);
            }
        }

        public void OnTreasureMenuOpen(ItemGrabMenu itemGrabMenu)
        {
            _treasureChestMenu = itemGrabMenu;
        }

        public void AutoLootTreasure()
        {
            if (_config.AutoLootTreasure && _treasureChestMenu != null)
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
        }
    }
}
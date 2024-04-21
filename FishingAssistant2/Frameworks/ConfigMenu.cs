using ChibiKyu.StardewMods.Common;
using FishingAssistant2;
using StardewModdingAPI;

namespace ChibiKyu.StardewMods.FishingAssistant2.Frameworks
{
    internal class ConfigMenu(
        IModRegistry modRegistry,
        IManifest modManifest,
        Func<ModConfig> config,
        Action reset,
        Action save,
        Action onConfigSavedCallback)
    {
        internal readonly Action OnConfigSavedCallback = onConfigSavedCallback;

        private IGenericModConfigMenuApi? _configMenu;

        private ConfigUtil? _configUtil;

        public void RegisterModConfigMenu()
        {
            _configMenu = modRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (_configMenu is null) return;

            _configMenu.Register(modManifest, reset, save);

            _configUtil = new ConfigUtil(_configMenu, modManifest, config);

            AddSectionTitle(I18n.ConfigMenu_Title_GoToPage);
            AddPageLink(I18n.ConfigMenu_Page_General, "General");
            AddPageLink(I18n.ConfigMenu_Page_MiniGame, "MiniGame");
            AddPageLink(I18n.ConfigMenu_Page_FishingRod, "FishingRod");

            AddPage(I18n.ConfigMenu_Page_General, "General");
            AddSectionTitle(I18n.ConfigMenu_Title_KeyBinding);
            _configUtil.AddKeyBind(_configUtil.ToggleAutomation);
            _configUtil.AddKeyBind(_configUtil.CatchTreasure);
            _configUtil.AddKeyBind(_configUtil.OpenConfigMenu);

            AddSectionTitle(I18n.ConfigMenu_Title_Hud);
            _configUtil.AddDropDown(_configUtil.HudPosition);

            AddSectionTitle(I18n.ConfigMenu_Title_Automation);
            _configUtil.AddBool(_configUtil.AutoCastFishingRod);
            _configUtil.AddBool(_configUtil.AutoHookFish);
            _configUtil.AddBool(_configUtil.AutoPlayMiniGame);
            _configUtil.AddBool(_configUtil.AutoClosePopup);
            _configUtil.AddBool(_configUtil.AutoLootTreasure);
            _configUtil.AddDropDown(_configUtil.ActionIfInventoryFull);
            _configUtil.AddBool(_configUtil.AutoTrashJunk);
            _configUtil.AddNumber(_configUtil.JunkHighestPrice);
            _configUtil.AddBool(_configUtil.AllowTrashFish);
            _configUtil.AddDropDown(_configUtil.AutoPauseFishing);
            _configUtil.AddNumber(_configUtil.TimeToPause);
            _configUtil.AddNumber(_configUtil.WarnCount);
            _configUtil.AddBool(_configUtil.AutoEatFood);
            _configUtil.AddNumber(_configUtil.EnergyPercentToEat);
            _configUtil.AddBool(_configUtil.AllowEatingFish);
            _configUtil.AddBool(_configUtil.AutoAttachBait);
            _configUtil.AddDropDown(_configUtil.PreferBait);
            _configUtil.AddBool(_configUtil.SpawnBaitIfDontHave);
            _configUtil.AddNumber(_configUtil.BaitAmountToSpawn);
            _configUtil.AddBool(_configUtil.AutoAttachTackles);
            _configUtil.AddDropDown(_configUtil.PreferredTackle);
            _configUtil.AddDropDown(_configUtil.PreferredAdvIridiumTackle);
            _configUtil.AddBool(_configUtil.SpawnTackleIfDontHave);

            // MiniGame Page
            AddPage(I18n.ConfigMenu_Page_General, "MiniGame");
            AddSectionTitle(I18n.ConfigMenu_Title_Fishing);
            _configUtil.AddDropDown(_configUtil.SkipFishingMiniGame);
            _configUtil.AddBool(_configUtil.InstantFishBite);
            _configUtil.AddNumber(_configUtil.PreferFishAmount);
            _configUtil.AddDropDown(_configUtil.PreferFishQuality);
            _configUtil.AddBool(_configUtil.AlwaysPerfect);
            _configUtil.AddBool(_configUtil.AlwaysMaxFishSize);
            _configUtil.AddNumber(_configUtil.FishDifficultyMultiplier);
            _configUtil.AddNumber(_configUtil.FishDifficultyAdditive);
            _configUtil.AddBool(_configUtil.InstantCatchTreasure);
            _configUtil.AddDropDown(_configUtil.TreasureChance);
            _configUtil.AddDropDown(_configUtil.GoldenTreasureChance);

            AddSectionTitle(I18n.ConfigMenu_Title_FishPreview);
            _configUtil.AddBool(_configUtil.DisplayFishPreview);
            _configUtil.AddBool(_configUtil.ShowFishName);
            _configUtil.AddBool(_configUtil.ShowTreasure);
            _configUtil.AddBool(_configUtil.ShowUncaughtFish);
            _configUtil.AddBool(_configUtil.ShowLegendaryFish);

            // FishingRod Page
            AddPage(I18n.ConfigMenu_Page_General, "FishingRod");
            AddSectionTitle(I18n.ConfigMenu_Title_FishingRod);
            _configUtil.AddDropDown(_configUtil.StartWithFishingRod);
            _configUtil.AddNumber(_configUtil.DefaultCastPower);
            _configUtil.AddNumber(_configUtil.UnlockCastPowerTime);
            _configUtil.AddBool(_configUtil.InfiniteBait);
            _configUtil.AddBool(_configUtil.InfiniteTackle);

            AddSectionTitle(I18n.ConfigMenu_Title_Enchantment);
            _configUtil.AddBool(_configUtil.AddAutoHookEnchantment);
            _configUtil.AddBool(_configUtil.AddEfficientEnchantment);
            _configUtil.AddBool(_configUtil.AddMasterEnchantment);
            _configUtil.AddBool(_configUtil.AddPreservingEnchantment);
            _configUtil.AddBool(_configUtil.RemoveWhenUnequipped);
        }

        #region Warpper

        internal void OpenModMenu()
        {
            _configMenu?.OpenModMenu(modManifest);
        }

        private void AddPage(Func<string> text, string pageTitle)
        {
            _configMenu?.AddPage(modManifest, $"chibiKyu.FishingAssistant2.{pageTitle}", text);
        }

        private void AddPageLink(Func<string> text, string pageTitle)
        {
            _configMenu?.AddPageLink(modManifest, $"chibiKyu.FishingAssistant2.{pageTitle}", text);
        }

        private void AddSectionTitle(Func<string> text)
        {
            _configMenu?.AddSectionTitle(modManifest, text);
        }

        #endregion
    }
}

using ChibiKyu.StardewMods.Common;
using FishingAssistant2;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Objects;
using Object = StardewValley.Object;

namespace ChibiKyu.StardewMods.FishingAssistant2.Frameworks
{
    internal class ConfigMenu(IModRegistry modRegistry, IManifest modManifest, Func<ModConfig> config, Action reset, Action save)
    {
        private IGenericModConfigMenuApi? _configMenu;
        
        private readonly List<string> _availableBaits = new List<string>();
        private readonly List<string> _availableTackles = new List<string>();
        private readonly List<string> _availableFishingRods = new List<string>();
        
        public void RegisterModConfigMenu()
        {
            _configMenu = modRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (_configMenu is null) return;
            
            _configMenu.Register(mod: modManifest, reset: reset, save: save);
            
            _availableBaits.Add("Any");
            _availableTackles.Add("Any");
            _availableFishingRods.Add("None");
            _availableFishingRods.Add("(T)TrainingRod");
            _availableFishingRods.Add("(T)BambooPole");
            
            foreach (KeyValuePair<string, ObjectData> item in Game1.objectData)
            {
                switch (item.Value.Category)
                {
                    case Object.baitCategory:
                        _availableBaits.Add(ItemRegistry.QualifyItemId(item.Key));
                        break;
                    case Object.tackleCategory:
                        _availableTackles.Add(ItemRegistry.QualifyItemId(item.Key));
                        break;
                }
            }

            AddSectionTitle(I18n.ConfigMenu_Title_GoToPage);
            AddPageLink(I18n.ConfigMenu_Page_General,"General");
            AddPageLink(I18n.ConfigMenu_Page_MiniGame,"MiniGame");
            AddPageLink(I18n.ConfigMenu_Page_FishingRod,"FishingRod");
            
            // General Page
            AddPage(I18n.ConfigMenu_Page_General,"General");
            AddSectionTitle(I18n.ConfigMenu_Title_KeyBinding);
            AddKeyBind(I18n.ConfigMenu_Option_ToggleAutomation, () => config().EnableAutomationButton, button => config().EnableAutomationButton = button, I18n.Tooltip_EnableAutomationButton);
            AddKeyBind(I18n.ConfigMenu_Option_CatchTreasure, () => config().CatchTreasureButton, button => config().CatchTreasureButton = button, I18n.Tooltip_CatchTreasureButton);
            AddKeyBind(I18n.ConfigMenu_Option_OpenConfigMenu, () => config().OpenConfigMenuButton, button => config().OpenConfigMenuButton = button, I18n.Tooltip_OpenConfigMenu);

            AddSectionTitle(I18n.ConfigMenu_Title_Hud);
            AddDropDown(I18n.ConfigMenu_Option_HudPosition, I18n.Tooltip_ModStatusPosition, HudPositionOptions(), ParseHudPosition, () => config().ModStatusPosition, pos => config().ModStatusPosition = pos);
            
            AddSectionTitle(I18n.ConfigMenu_Title_Automation);
            AddBool(I18n.ConfigMenu_Option_AutoCastFishingRod,I18n.Tooltip_AutoCastFishingRod, () => config().AutoCastFishingRod, b => config().AutoCastFishingRod = b);
            AddBool(I18n.ConfigMenu_Option_AutoHookFish, I18n.Tooltip_AutoHookFish, () => config().AutoHookFish, b => config().AutoHookFish = b);
            AddBool(I18n.ConfigMenu_Option_AutoPlayMiniGame, I18n.Tooltip_AutoPlayMiniGame, () => config().AutoPlayMiniGame, b => config().AutoPlayMiniGame = b);
            AddBool(I18n.ConfigMenu_Option_AutoClosePopup, I18n.Tooltip_AutoClosePopup, () => config().AutoClosePopup, b => config().AutoClosePopup = b);
            AddBool(I18n.ConfigMenu_Option_AutoLootTreasure, I18n.Tooltip_AutoLootTreasure, () => config().AutoLootTreasure, b => config().AutoLootTreasure = b);
            AddDropDown(I18n.ConfigMenu_Option_ActionIfInventoryFull,I18n.Tooltip_ActionIfInventoryFull, ActionOnInventoryFullOptions(), ParseActionOnInventoryFull, () => config().ActionIfInventoryFull, action => config().ActionIfInventoryFull = action);
            AddBool(I18n.ConfigMenu_Option_AutoTrashJunk, I18n.Tooltip_AutoTrashJunk, () => config().AutoTrashJunk, b => config().AutoTrashJunk = b);
            AddNumber(I18n.ConfigMenu_Option_JunkHighestPrice, I18n.Tooltip_JunkHighestPrice, () => config().JunkHighestPrice, i => config().JunkHighestPrice = i, min: 0);
            AddBool(I18n.ConfigMenu_Option_AllowTrashFish, I18n.Tooltip_AllowTrashFish, () => config().AllowTrashFish, b => config().AllowTrashFish = b);
            AddDropDown(I18n.ConfigMenu_Option_AutoPauseFishing, I18n.Tooltip_AutoPauseFishing, PauseFishingOptions(), ParsePauseFishing, () => config().AutoPauseFishing, s => config().AutoPauseFishing = s);
            AddNumber(I18n.Tooltip_TimeToPause, I18n.Tooltip_TimeToPause, () => config().TimeToPause, i => config().TimeToPause = i, min: 6, max: 25, interval: 1,formatValue: value => Game1.getTimeOfDayString(value * 100));
            AddNumber(I18n.ConfigMenu_Option_WarnCount, I18n.Tooltip_WarnCount, () => config().WarnCount, i => config().WarnCount = i, min: 1, max: 5, interval: 1);
            AddBool(I18n.ConfigMenu_Option_AutoEatFood, I18n.Tooltip_AutoEatFood, () => config().AutoEatFood, b => config().AutoEatFood = b);
            AddNumber(I18n.ConfigMenu_Option_EnergyPercentToEat, I18n.Tooltip_EnergyPercentToEat, () => config().EnergyPercentToEat, i => config().EnergyPercentToEat = i, min: 5, max: 95, interval: 5);
            AddBool(I18n.ConfigMenu_Option_AllowEatingFish, I18n.Tooltip_AllowEatingFish, () => config().AllowEatingFish, b => config().AllowEatingFish = b);
            AddBool(I18n.ConfigMenu_Option_AutoAttachBait, I18n.Tooltip_AutoAttachBait, () => config().AutoAttachBait, b => config().AutoAttachBait = b);
            AddDropDown(I18n.ConfigMenu_Option_PreferBait, I18n.Tooltip_PreferredBait, _availableBaits.ToArray(), ParseBaitAndTackleName, () => config().PreferredBait, s => config().PreferredBait = s);
            AddBool(I18n.ConfigMenu_Option_SpawnBaitIfDontHave, I18n.Tooltip_SpawnBaitIfDontHave, () => config().SpawnBaitIfDontHave, b => config().SpawnBaitIfDontHave = b);
            AddNumber(I18n.ConfigMenu_Option_BaitAmountToSpawn, I18n.Tooltip_BaitAmountToSpawn, () => config().BaitAmountToSpawn, i => config().BaitAmountToSpawn = i, min: 1, max: 999, interval: 1);
            AddBool(I18n.ConfigMenu_Option_AutoAttachTackles, I18n.Tooltip_AutoAttachTackles, () => config().AutoAttachTackles, b => config().AutoAttachTackles = b);
            AddDropDown(I18n.ConfigMenu_Option_PreferTackle, I18n.Tooltip_PreferredTackle, _availableTackles.ToArray(), ParseBaitAndTackleName, () => config().PreferredTackle, s => config().PreferredTackle = s);
            AddDropDown(I18n.ConfigMenu_Option_PreferAdvancedIridiumTackle, I18n.Tooltip_PreferredAdvIridiumTackle, _availableTackles.ToArray(), ParseBaitAndTackleName, () => config().PreferredAdvIridiumTackle, s => config().PreferredAdvIridiumTackle = s);
            AddBool(I18n.ConfigMenu_Option_SpawnTackleIfDontHave, I18n.Tooltip_SpawnTackleIfDontHave, () => config().SpawnTackleIfDontHave, b => config().SpawnTackleIfDontHave = b);
            
            // MiniGame Page
            AddPage(I18n.ConfigMenu_Page_General,"MiniGame");
            AddSectionTitle(I18n.ConfigMenu_Title_Fishing);
            AddDropDown(I18n.ConfigMenu_Option_SkipFishingMiniGame, I18n.Tooltip_SkipFishingMiniGame, SkipMiniGameOptions(),ParseSkipMiniGame, () => config().SkipFishingMiniGame, option => config().SkipFishingMiniGame = option);
            AddBool(I18n.ConfigMenu_Option_InstantFishBite, I18n.Tooltip_InstantFishBite, () => config().InstantFishBite, b => config().InstantFishBite = b);
            AddNumber(I18n.ConfigMenu_Option_PreferFishAmount, I18n.Tooltip_PreferFishAmount, () => config().PreferFishAmount, i => config().PreferFishAmount = i, min: 1, max: 3);
            AddDropDown(I18n.ConfigMenu_Option_PreferFishQuality, I18n.Tooltip_PreferFishQuality, FishQualityOptions(), ParseFishQuality, () => config().PreferFishQuality, quality => config().PreferFishQuality = quality);
            AddBool(I18n.ConfigMenu_Option_AlwaysPerfect, I18n.Tooltip_AlwaysPerfect, () => config().AlwaysPerfect, b => config().AlwaysPerfect = b);
            AddBool(I18n.ConfigMenu_Option_AlwaysMaxFishSize, I18n.Tooltip_AlwaysMaxFishSize, () => config().AlwaysMaxFishSize, b => config().AlwaysMaxFishSize = b);
            AddNumber(I18n.ConfigMenu_Option_FishDifficultyMultiplier, I18n.Tooltip_FishDifficultyMultiplier, () => config().FishDifficultyMultiplier, i => config().FishDifficultyMultiplier = i);
            AddNumber(I18n.ConfigMenu_Option_FishDifficultyAdditive, I18n.Tooltip_FishDifficultyAdditive, () => config().FishDifficultyAdditive, i => config().FishDifficultyAdditive = i);
            AddBool(I18n.ConfigMenu_Option_InstantCatchTreasure, I18n.Tooltip_InstantCatchTreasure, () => config().InstantCatchTreasure, b => config().InstantCatchTreasure = b);
            AddDropDown(I18n.ConfigMenu_Option_TreasureChance, I18n.Tooltip_TreasureChance, TreasureChanceOptions(), ParseTreasureChance, () => config().TreasureChance, chance => config().TreasureChance = chance);
            AddDropDown(I18n.ConfigMenu_Option_GoldenTreasureChance, I18n.Tooltip_GoldenTreasureChance, TreasureChanceOptions(), ParseTreasureChance, () => config().GoldenTreasureChance, chance => config().GoldenTreasureChance = chance);
            
            AddSectionTitle(I18n.ConfigMenu_Title_FishPreview);
            AddBool(I18n.ConfigMenu_Option_DisplayFishPreview, I18n.Tooltip_DisplayFishPreview, () => config().DisplayFishPreview, b => config().DisplayFishPreview = b);
            AddBool(I18n.ConfigMenu_Option_ShowFishName, I18n.Tooltip_ShowFishName, () => config().ShowFishName, b => config().ShowFishName = b);
            AddBool(I18n.ConfigMenu_Option_ShowTreasure, I18n.Tooltip_ShowTreasure, () => config().ShowTreasure, b => config().ShowTreasure = b);
            AddBool(I18n.ConfigMenu_Option_ShowUncaughtFish, I18n.Tooltip_ShowUncaughtFish, () => config().ShowUncaughtFish, b => config().ShowUncaughtFish = b);
            AddBool(I18n.ConfigMenu_Option_ShowLegendaryFish, I18n.Tooltip_ShowLegendaryFish, () => config().ShowLegendaryFish, b => config().ShowLegendaryFish = b);
            
            // FishingRod Page
            AddPage(I18n.ConfigMenu_Page_General,"FishingRod");
            AddSectionTitle(I18n.ConfigMenu_Title_FishingRod);
            AddDropDown(I18n.ConfigMenu_Option_StartWithFishingRod, I18n.Tooltip_StartWithFishingRod, _availableFishingRods.ToArray(), ParseFishingRodName, () => config().StartWithFishingRod, s => config().StartWithFishingRod = s);
            AddNumber(I18n.ConfigMenu_Option_CastPowerPercent, I18n.Tooltip_CastPowerPercent, () => config().CastPowerPercent, i => config().CastPowerPercent = i, min: 0, max: 100, interval: 5);
            AddBool(I18n.ConfigMenu_Option_UseSmartCastPower, I18n.Tooltip_UseSmartCastPower, () => config().UseSmartCastPower, b => config().UseSmartCastPower = b);
            AddBool(I18n.ConfigMenu_Option_InfiniteBait, I18n.Tooltip_InfiniteBait, () => config().InfiniteBait, b => config().InfiniteBait = b);
            AddBool(I18n.ConfigMenu_Option_InfiniteTackle, I18n.Tooltip_InfiniteTackle, () => config().InfiniteTackle, b => config().InfiniteTackle = b);
            
            AddSectionTitle(I18n.ConfigMenu_Title_Enchantment);
            AddBool(I18n.ConfigMenu_Option_AddAutoHookEnchantment, I18n.Tooltip_AddAutoHookEnchantment, () => config().AddAutoHookEnchantment, b => config().AddAutoHookEnchantment = b);
            AddBool(I18n.ConfigMenu_Option_AddEfficientEnchantment, I18n.Tooltip_AddEfficientEnchantment, () => config().AddEfficientEnchantment, b => config().AddEfficientEnchantment = b);
            AddBool(I18n.ConfigMenu_Option_AddMasterEnchantment, I18n.Tooltip_AddMasterEnchantment, () => config().AddMasterEnchantment, b => config().AddMasterEnchantment = b);
            AddBool(I18n.ConfigMenu_Option_AddPreservingEnchantment, I18n.Tooltip_AddPreservingEnchantment, () => config().AddPreservingEnchantment, b => config().AddPreservingEnchantment = b);
            AddBool(I18n.ConfigMenu_Option_RemoveEnchantmentsWhenUnequipped, I18n.Tooltip_RemoveWhenUnequipped, () => config().RemoveWhenUnequipped, b => config().RemoveWhenUnequipped = b);
        }

        #region Dropdown Option
        
        private static string[] SkipMiniGameOptions()
        {
            return Enum.GetNames(typeof(SkipFishingMiniGame));
        }

        private string ParseSkipMiniGame(string rawText)
        {
            if (!Enum.TryParse(rawText, out SkipFishingMiniGame text))
                return rawText;

            return text switch
            {
                SkipFishingMiniGame.Off => I18n.Off(),
                SkipFishingMiniGame.SkipAll => I18n.SkipAll(),
                SkipFishingMiniGame.SkipOnlyCaught => I18n.SkipOnlyCaught(),
                _ => text.ToString()
            };
        }

        private static string[] PauseFishingOptions()
        {
            return Enum.GetNames(typeof(PauseFishingBehaviour));
        }

        private string ParsePauseFishing(string rawText)
        {
            if (!Enum.TryParse(rawText, out PauseFishingBehaviour text))
                return rawText;

            return text switch
            {
                PauseFishingBehaviour.Off => I18n.Off(),
                PauseFishingBehaviour.WarnOnly => I18n.WarnOnly(),
                PauseFishingBehaviour.WarnAndPause => I18n.WarnAndPause(),
                _ => text.ToString()
            };
        }

        private static string[] FishQualityOptions()
        {
            return Enum.GetNames(typeof(FishQuality));
        }

        private string ParseFishQuality(string rawText)
        {
            if (!Enum.TryParse(rawText, out FishQuality text))
                return rawText;

            return text switch
            {
                FishQuality.Any => I18n.Any(),
                FishQuality.None => I18n.None(),
                FishQuality.Silver => I18n.Silver(),
                FishQuality.Gold => I18n.Gold(),
                FishQuality.Iridium => I18n.Iridium(),
                _ => text.ToString()
            };
        }

        private string ParseBaitAndTackleName(string rawText)
        {
            return rawText == "Any" ? I18n.Any() : ItemRegistry.GetData(rawText).DisplayName;
        }
        
        private string ParseFishingRodName(string rawText)
        {
            return rawText == "None" ? I18n.None () : ItemRegistry.GetData(rawText).DisplayName;
        }

        private static string[] ActionOnInventoryFullOptions()
        {
            return Enum.GetNames(typeof(ActionOnInventoryFull));
        }

        private string ParseActionOnInventoryFull(string rawText)
        {
            if (!Enum.TryParse(rawText, out ActionOnInventoryFull text))
                return rawText;

            return text switch
            {
                ActionOnInventoryFull.Stop => I18n.StopLoot(),
                ActionOnInventoryFull.Drop => I18n.DropRemaining(),
                ActionOnInventoryFull.Discard => I18n.DiscardRemaining(),
                _ => text.ToString()
            };
        }

        private static string[] TreasureChanceOptions()
        {
            return Enum.GetNames(typeof(TreasureChance));
        }

        private string ParseTreasureChance(string rawText)
        {
            if (!Enum.TryParse(rawText, out TreasureChance text))
                return rawText;

            return text switch
            {
                TreasureChance.Default => I18n.Default(),
                TreasureChance.Always => I18n.Always(),
                TreasureChance.Never => I18n.Never(),
                _ => text.ToString()
            };
        }

        private static string[] HudPositionOptions()
        {
            return Enum.GetNames(typeof(HudPosition));
        }

        private string ParseHudPosition(string rawText)
        {
            if (!Enum.TryParse(rawText, out HudPosition text))
                return rawText;

            return text switch
            {
                HudPosition.Left => I18n.Left(),
                HudPosition.Right => I18n.Right(),
                _ => text.ToString()
            };
        }

        #endregion

        #region Warpper

        internal void OpenModMenu()
        {
            _configMenu?.OpenModMenu(modManifest);
        }
        
        private void AddPage(Func<string> text, string pageTitle)
        {
            _configMenu?.AddPage(
                mod: modManifest,
                pageId: $"chibiKyu.FishingAssistant2.{pageTitle}",
                pageTitle: text);
        }

        private void AddPageLink(Func<string> text, string pageTitle)
        {
            _configMenu?.AddPageLink(
                mod: modManifest,
                pageId: $"chibiKyu.FishingAssistant2.{pageTitle}",
                text: text);
        }

        private void AddSectionTitle(Func<string> text)
        {
            _configMenu?.AddSectionTitle(
                mod: modManifest,
                text: text);
        }

        private void AddKeyBind(Func<string> text, Func<SButton> getValue, Action<SButton> setValue,
            Func<string> tooltip)
        {
            _configMenu?.AddKeybind(
                mod: modManifest,
                getValue: getValue,
                setValue: setValue,
                name: text,
                tooltip: tooltip);
        }

        private void AddDropDown(Func<string> name, Func<string> tooltip, string[] allowedValues,
            Func<string, string> formatAllowedValue,
            Func<string> getValue, Action<string> setValue)
        {
            _configMenu?.AddTextOption(
                mod: modManifest,
                name: name,
                getValue: getValue,
                setValue: setValue,
                allowedValues: allowedValues,
                formatAllowedValue: formatAllowedValue,
                tooltip: tooltip
            );
        }

        private void AddBool(Func<string> name, Func<string> tooltip, Func<bool> getValue, Action<bool> setValue)
        {
            _configMenu?.AddBoolOption(
                mod: modManifest,
                name: name,
                getValue: getValue,
                setValue: setValue,
                tooltip: tooltip
            );
        }

        private void AddNumber(Func<string> name, Func<string> tooltip, Func<int> getValue, Action<int> setValue,
            int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null)
        {
            _configMenu?.AddNumberOption(
                mod: modManifest,
                getValue: getValue,
                setValue: setValue,
                name: name,
                min: min,
                max: max,
                interval: interval,
                formatValue: formatValue,
                tooltip: tooltip
            );
        }

        private void AddNumber(Func<string> name, Func<string> tooltip, Func<float> getValue, Action<float> setValue,
            float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null)
        {
            _configMenu?.AddNumberOption(
                mod: modManifest,
                getValue: getValue,
                setValue: setValue,
                name: name,
                min: min,
                max: max,
                interval: interval,
                formatValue: formatValue,
                tooltip: tooltip
            );
        }

        #endregion
    }
}
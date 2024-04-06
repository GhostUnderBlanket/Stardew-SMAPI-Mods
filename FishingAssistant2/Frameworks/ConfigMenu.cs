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
        
        public void RegisterModConfigMenu()
        {
            _configMenu = modRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (_configMenu is null) return;
            
            _configMenu.Register(mod: modManifest, reset: reset, save: save);
            
            _availableBaits.Add("Any");
            _availableTackles.Add("Any");
            
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

            AddSectionTitle(I18n.ConfigMenu_Title_KeyBinding);
            AddKeyBind(I18n.ConfigMenu_Option_ToggleAutomation, () => config().EnableAutomationButton, button => config().EnableAutomationButton = button);
            AddKeyBind(I18n.ConfigMenu_Option_CatchTreasure, () => config().CatchTreasureButton, button => config().CatchTreasureButton = button);

            AddSectionTitle(I18n.ConfigMenu_Title_Hud);
            AddDropDown(I18n.ConfigMenu_Option_HudPosition, HudPositionOptions(), ParseHudPosition, () => config().ModStatusPosition, pos => config().ModStatusPosition = pos);
            
            AddSectionTitle(I18n.ConfigMenu_Title_Fishing);
            AddBool(I18n.ConfigMenu_Option_MaxCastPower, () => config().MaxCastPower, b => config().MaxCastPower = b);
            AddBool(I18n.ConfigMenu_Option_InstantFishBite, () => config().InstantFishBite, b => config().InstantFishBite = b);
            AddBool(I18n.ConfigMenu_Option_InstantCatchFish, () => config().InstantCatchFish, b => config().InstantCatchFish = b);
            AddBool(I18n.ConfigMenu_Option_InstantCatchTreasure, () => config().InstantCatchTreasure, b => config().InstantCatchTreasure = b);
            AddDropDown(I18n.ConfigMenu_Option_TreasureChance, TreasureChanceOptions(), ParseTreasureChance, () => config().TreasureChance, chance => config().TreasureChance = chance);
            AddNumber(I18n.ConfigMenu_Option_PreferFishAmount, () => config().PreferFishAmount, i => config().PreferFishAmount = i, 1, 3);
            AddDropDown(I18n.ConfigMenu_Option_PreferFishQuality, FishQualityOptions(), ParseFishQuality, () => config().PreferFishQuality, quality => config().PreferFishQuality = quality);
            AddBool(I18n.ConfigMenu_Option_AlwaysPerfect, () => config().AlwaysPerfect, b => config().AlwaysPerfect = b);
            AddBool(I18n.ConfigMenu_Option_AlwaysMaxFishSize, () => config().AlwaysMaxFishSize, b => config().AlwaysMaxFishSize = b);
            AddNumber(I18n.ConfigMenu_Option_FishDifficultyMultiplier, () => config().FishDifficultyMultiplier, i => config().FishDifficultyMultiplier = i);
            AddNumber(I18n.ConfigMenu_Option_FishDifficultyAdditive, () => config().FishDifficultyAdditive, i => config().FishDifficultyAdditive = i);

            AddSectionTitle(I18n.ConfigMenu_Title_Automation);
            AddBool(I18n.ConfigMenu_Option_AutoCastFishingRod, () => config().AutoCastFishingRod, b => config().AutoCastFishingRod = b);
            AddBool(I18n.ConfigMenu_Option_AutoHookFish, () => config().AutoHookFish, b => config().AutoHookFish = b);
            AddBool(I18n.ConfigMenu_Option_AutoPlayMiniGame, () => config().AutoPlayMiniGame, b => config().AutoPlayMiniGame = b);
            AddBool(I18n.ConfigMenu_Option_AutoClosePopup, () => config().AutoClosePopup, b => config().AutoClosePopup = b);
            AddBool(I18n.ConfigMenu_Option_AutoLootTreasure, () => config().AutoLootTreasure, b => config().AutoLootTreasure = b);
            AddDropDown(I18n.ConfigMenu_Option_ActionIfInventoryFull, ActionOnInventoryFullOptions(), ParseActionOnInventoryFull, () => config().ActionIfInventoryFull, action => config().ActionIfInventoryFull = action);
            AddDropDown(I18n.ConfigMenu_Option_AutoPauseFishing, PauseFishingOptions(), ParsePauseFishing, () => config().AutoPauseFishing, s => config().AutoPauseFishing = s);
            AddNumber(I18n.ConfigMenu_Option_AutoPauseFishingTime, () => config().PauseFishingTime, i => config().PauseFishingTime = i, 6, 25, 1,value => Game1.getTimeOfDayString(value * 100));
            AddNumber(I18n.ConfigMenu_Option_NumToWarn, () => config().NumToWarn, i => config().NumToWarn = i, 1, 5, 1);
            AddBool(I18n.ConfigMenu_Option_AutoEatFood, () => config().AutoEatFood, b => config().AutoEatFood = b);
            AddNumber(I18n.ConfigMenu_Option_EnergyPercentToEat, () => config().EnergyPercentToEat, i => config().EnergyPercentToEat = i, 1, 99, 1);
            AddBool(I18n.ConfigMenu_Option_AllowEatingFish, () => config().AllowEatingFish, b => config().AllowEatingFish = b);
            
            AddSectionTitle(I18n.ConfigMenu_Title_FishPreview);
            AddBool(I18n.ConfigMenu_Option_DisplayFishPreview, () => config().DisplayFishPreview, b => config().DisplayFishPreview = b);
            AddBool(I18n.ConfigMenu_Option_ShowFishName, () => config().ShowFishName, b => config().ShowFishName = b);
            AddBool(I18n.ConfigMenu_Option_ShowTreasure, () => config().ShowTreasure, b => config().ShowTreasure = b);
            AddBool(I18n.ConfigMenu_Option_ShowUncaughtFishSpecies, () => config().ShowUncaughtFishSpecies, b => config().ShowUncaughtFishSpecies = b);
            AddBool(I18n.ConfigMenu_Option_AlwaysShowLegendaryFish, () => config().AlwaysShowLegendaryFish, b => config().AlwaysShowLegendaryFish = b);
            
            AddSectionTitle(I18n.ConfigMenu_Title_FishingRod);
            AddBool(I18n.ConfigMenu_Option_AutoAttachBait, () => config().AutoAttachBait, b => config().AutoAttachBait = b);
            AddDropDown(I18n.ConfigMenu_Option_PreferBait, _availableBaits.ToArray(), ParseItemName, () => config().PreferBait, s => config().PreferBait = s);
            AddBool(I18n.ConfigMenu_Option_InfiniteBait, () => config().InfiniteBait, b => config().InfiniteBait = b);
            AddBool(I18n.ConfigMenu_Option_SpawnBaitIfDontHave, () => config().SpawnBaitIfDontHave, b => config().SpawnBaitIfDontHave = b);
            
            AddBool(I18n.ConfigMenu_Option_AutoAttachTackles, () => config().AutoAttachTackles, b => config().AutoAttachTackles = b);
            AddDropDown(I18n.ConfigMenu_Option_PreferTackle, _availableTackles.ToArray(), ParseItemName, () => config().PreferTackle, s => config().PreferTackle = s);
            AddDropDown(I18n.ConfigMenu_Option_PreferAdvancedIridiumTackle, _availableTackles.ToArray(), ParseItemName, () => config().PreferAdvIridiumTackle, s => config().PreferAdvIridiumTackle = s);
            AddBool(I18n.ConfigMenu_Option_InfiniteTackle, () => config().InfiniteTackle, b => config().InfiniteTackle = b);
            AddBool(I18n.ConfigMenu_Option_SpawnTackleIfDontHave, () => config().SpawnTackleIfDontHave, b => config().SpawnTackleIfDontHave = b);
            
            AddSectionTitle(I18n.ConfigMenu_Title_Enchantment);
            AddBool(I18n.ConfigMenu_Option_AddAutoHookEnchantment, () => config().AddAutoHookEnchantment, b => config().AddAutoHookEnchantment = b);
            AddBool(I18n.ConfigMenu_Option_AddEfficientEnchantment, () => config().AddEfficientEnchantment, b => config().AddEfficientEnchantment = b);
            AddBool(I18n.ConfigMenu_Option_AddMasterEnchantment, () => config().AddMasterEnchantment, b => config().AddMasterEnchantment = b);
            AddBool(I18n.ConfigMenu_Option_AddPreservingEnchantment, () => config().AddPreservingEnchantment, b => config().AddPreservingEnchantment = b);
            AddBool(I18n.ConfigMenu_Option_RemoveEnchantmentsWhenUnequipped, () => config().RemoveEnchantmentWhenUnequipped, b => config().RemoveEnchantmentWhenUnequipped = b);
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
        
        private string ParseItemName(string rawText)
        {
            return rawText == "Any" ? I18n.Any() : ItemRegistry.GetData(rawText).DisplayName;
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

        private void AddSectionTitle(Func<string> text)
        {
            _configMenu?.AddSectionTitle(
                mod: modManifest,
                text: text);
        }

        private void AddKeyBind(Func<string> text, Func<SButton> getValue, Action<SButton> setValue)
        {
            _configMenu?.AddKeybind(
                mod: modManifest,
                getValue: getValue,
                setValue: setValue,
                name: text);
        }

        private void AddDropDown(Func<string> name, string[] allowedValues, Func<string, string> formatAllowedValue, Func<string> getValue, Action<string> setValue)
        {
            _configMenu?.AddTextOption(
                mod: modManifest,
                name: name,
                getValue: getValue,
                setValue: setValue,
                allowedValues: allowedValues,
                formatAllowedValue: formatAllowedValue
            );
        }

        private void AddBool(Func<string> name, Func<bool> getValue, Action<bool> setValue)
        {
            _configMenu?.AddBoolOption(
                mod: modManifest,
                name: name,
                getValue: getValue,
                setValue: setValue);
        }

        private void AddNumber(Func<string> name, Func<int> getValue, Action<int> setValue, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null)
        {
            _configMenu?.AddNumberOption(
                mod: modManifest,
                getValue: getValue,
                setValue: setValue,
                name: name,
                min: min,
                max: max,
                interval: interval,
                formatValue: formatValue);
        }
        
        private void AddNumber(Func<string> name, Func<float> getValue, Action<float> setValue, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null)
        {
            _configMenu?.AddNumberOption(
                mod: modManifest,
                getValue: getValue,
                setValue: setValue,
                name: name,
                min: min,
                max: max,
                interval: interval,
                formatValue: formatValue);
        }
    }
}
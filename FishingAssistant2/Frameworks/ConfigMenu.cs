using ChibiKyu.StardewMods.Common;
using FishingAssistant2;
using StardewModdingAPI;

namespace ChibiKyu.StardewMods.FishingAssistant2.Frameworks
{
    internal class ConfigMenu(IModRegistry modRegistry, IManifest modManifest, Func<ModConfig> config, Action configReset, Action configSave, List<string> availableBaits, List<string> availableTackles)
    {
        private IGenericModConfigMenuApi? _configMenu;
        
        public void RegisterModConfigMenu()
        {
            _configMenu = modRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (_configMenu is null) return;
            
            Register();

            AddSectionTitle(I18n.ConfigMenu_Title_KeyBinding);
            AddKeyBind(I18n.ConfigMenu_Option_ToggleMod, () => config().EnableModButton, button => config().EnableModButton = button);
            AddKeyBind(I18n.ConfigMenu_Option_CatchTreasure, () => config().CatchTreasureButton, button => config().CatchTreasureButton = button);

            AddSectionTitle(I18n.ConfigMenu_Title_Hud);
            AddDropDown(I18n.ConfigMenu_Option_HudPosition, HudPositionOptions(), ParseHudPosition, () => config().ModStatusPosition, pos => config().ModStatusPosition = pos);
            
            AddSectionTitle(I18n.ConfigMenu_Title_Fishing);
            AddBool(I18n.ConfigMenu_Option_MaxCastPower, () => config().MaxCastPower, b => config().MaxCastPower = b);
            AddBool(I18n.ConfigMenu_Option_InstantFishBite, () => config().InstantFishBite, b => config().InstantFishBite = b);
            AddBool(I18n.ConfigMenu_Option_AlwaysPerfect, () => config().AlwaysPerfect, b => config().AlwaysPerfect = b);
            AddDropDown(I18n.ConfigMenu_Option_TreasureChance, TreasureChanceOptions(), ParseTreasureChance, () => config().TreasureChance, chance => config().TreasureChance = chance);
            AddBool(I18n.ConfigMenu_Option_InstantCatchFish, () => config().InstantCatchFish, b => config().InstantCatchFish = b);
            AddBool(I18n.ConfigMenu_Option_InstantCatchTreasure, () => config().InstantCatchTreasure, b => config().InstantCatchTreasure = b);
            AddNumber(I18n.ConfigMenu_Option_FishDifficultyMultiplier, () => config().FishDifficultyMultiplier, i => config().FishDifficultyMultiplier = i);
            AddNumber(I18n.ConfigMenu_Option_FishDifficultyAdditive, () => config().FishDifficultyAdditive, i => config().FishDifficultyAdditive = i);

            AddSectionTitle(I18n.ConfigMenu_Title_Automation);
            AddBool(I18n.ConfigMenu_Option_AutoCastFishingRod, () => config().AutoCastFishingRod, b => config().AutoCastFishingRod = b);
            AddBool(I18n.ConfigMenu_Option_AutoHookFish, () => config().AutoHookFish, b => config().AutoHookFish = b);
            AddBool(I18n.ConfigMenu_Option_AutoPlayMiniGame, () => config().AutoPlayMiniGame, b => config().AutoPlayMiniGame = b);
            AddBool(I18n.ConfigMenu_Option_AutoClosePopup, () => config().AutoClosePopup, b => config().AutoClosePopup = b);
            AddBool(I18n.ConfigMenu_Option_AutoLootTreasure, () => config().AutoLootTreasure, b => config().AutoLootTreasure = b);
            
            AddSectionTitle(I18n.ConfigMenu_Title_Enchantment);
            AddBool(I18n.ConfigMenu_Option_AddAutoHookEnchantment, () => config().AddAutoHookEnchantment, b => config().AddAutoHookEnchantment = b);
            AddBool(I18n.ConfigMenu_Option_AddEfficientEnchantment, () => config().AddEfficientEnchantment, b => config().AddEfficientEnchantment = b);
            AddBool(I18n.ConfigMenu_Option_AddMasterEnchantment, () => config().AddMasterEnchantment, b => config().AddMasterEnchantment = b);
            AddBool(I18n.ConfigMenu_Option_AddPreservingEnchantment, () => config().AddPreservingEnchantment, b => config().AddPreservingEnchantment = b);
            AddBool(I18n.ConfigMenu_Option_RemoveEnchantmentsWhenUnequipped, () => config().RemoveEnchantmentWhenUnequipped, b => config().RemoveEnchantmentWhenUnequipped = b);
        }

        private static string[] TreasureChanceOptions()
        {
            return Enum.GetNames(typeof(TreasureChance));
        }

        private static string[] HudPositionOptions()
        {
            return ["Left", "Right"];
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
        
        private string ParseHudPosition(string text)
        {
            return text switch
            {
                "Left" => I18n.Left(),
                "Right" => I18n.Right(),
                _ => text
            };
        }

        private void Register()
        {
            _configMenu?.Register(
                mod: modManifest,
                reset: configReset,
                save: configSave
            );
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

        private void AddNumber(Func<string> name, Func<int> getValue, Action<int> setValue)
        {
            _configMenu?.AddNumberOption(
                mod: modManifest,
                getValue: getValue,
                setValue: setValue,
                name: name);
        }
    }
}
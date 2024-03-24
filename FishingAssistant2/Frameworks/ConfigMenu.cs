using ChibiKyu.StardewMods.Common;
using FishingAssistant2;
using StardewModdingAPI;

namespace ChibiKyu.StardewMods.FishingAssistant2.Frameworks
{
    internal class ConfigMenu(IModRegistry modRegistry, IManifest modManifest, Func<ModConfig> config, Action configReset, Action configSave)
    {
        private IGenericModConfigMenuApi? _configMenu;
        
        public void RegisterModConfigMenu()
        {
            _configMenu = modRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (_configMenu is null) return;
            
            Register();

            AddSectionTitle(I18n.ConfigMenu_Title_KeyBinding());
            AddKeyBind(I18n.ConfigMenu_Option_ToggleMod(), GetEnableModValue(), SetEnableModValue());
            AddKeyBind(I18n.ConfigMenu_Option_CatchTreasure(), GetCatchTreasureValue(), SetCatchTreasureValue());

            AddSectionTitle(I18n.ConfigMenu_Title_Hud());
            AddDropDown(I18n.ConfigMenu_Option_HudPosition(), GetHudPositionValue(), SetHudPositionValue(), HudPositionOptions());
            
            AddSectionTitle(I18n.ConfigMenu_Title_Fishing());
            AddBool(I18n.ConfigMenu_Option_MaxCastPower(), () => config().MaxCastPower, b => config().MaxCastPower = b);
            AddBool(I18n.ConfigMenu_Option_InstantFishBite(), () => config().InstantFishBite, b => config().InstantFishBite = b);
            AddBool(I18n.ConfigMenu_Option_AlwaysPerfect(), () => config().AlwaysPerfect, b => config().AlwaysPerfect = b);
            AddDropDown(I18n.ConfigMenu_Option_TreasureChance(), GetTreasureChanceValue(), SetTreasureChanceValue(), TreasureChanceOptions());
            AddBool(I18n.ConfigMenu_Option_InstantCatchFish(), () => config().InstantCatchFish, b => config().InstantCatchFish = b);
            AddBool(I18n.ConfigMenu_Option_InstantCatchTreasure(), () => config().InstantCatchTreasure, b => config().InstantCatchTreasure = b);
            AddNumber(I18n.ConfigMenu_Option_FishDifficultyMultiplier(), () => config().FishDifficultyMultiplier, i => config().FishDifficultyMultiplier = i);
            AddNumber(I18n.ConfigMenu_Option_FishDifficultyAdditive(), () => config().FishDifficultyAdditive, i => config().FishDifficultyAdditive = i);

            AddSectionTitle(I18n.ConfigMenu_Title_Automation());
            AddBool(I18n.ConfigMenu_Option_AutoCastFishingRod(), () => config().AutoCastFishingRod, b => config().AutoCastFishingRod = b);
            AddBool(I18n.ConfigMenu_Option_AutoHookFish(), () => config().AutoHookFish, b => config().AutoHookFish = b);
            AddBool(I18n.ConfigMenu_Option_AutoPlayMiniGame(), () => config().AutoPlayMiniGame, b => config().AutoPlayMiniGame = b);
            AddBool(I18n.ConfigMenu_Option_AutoClosePopup(), () => config().AutoClosePopup, b => config().AutoClosePopup = b);
            AddBool(I18n.ConfigMenu_Option_AutoLootTreasure(), () => config().AutoLootTreasure, b => config().AutoLootTreasure = b);
            
            AddSectionTitle(I18n.ConfigMenu_Title_Enchantment());
            AddBool(I18n.ConfigMenu_Option_AddAutoHookEnchantment(), () => config().AddAutoHookEnchantment, b => config().AddAutoHookEnchantment = b);
            AddBool(I18n.ConfigMenu_Option_AddEfficientEnchantment(), () => config().AddEfficientEnchantment, b => config().AddEfficientEnchantment = b);
            AddBool(I18n.ConfigMenu_Option_AddMasterEnchantment(), () => config().AddMasterEnchantment, b => config().AddMasterEnchantment = b);
            AddBool(I18n.ConfigMenu_Option_AddPreservingEnchantment(), () => config().AddPreservingEnchantment, b => config().AddPreservingEnchantment = b);
            AddBool(I18n.ConfigMenu_Option_RemoveEnchantmentsWhenUnequipped(), () => config().RemoveEnchantmentWhenUnequipped, b => config().RemoveEnchantmentWhenUnequipped = b);
        }

        private string[] TreasureChanceOptions()
        {
            return [I18n.Default(), I18n.Always(), I18n.Never()];
        }

        private Action<string> SetTreasureChanceValue()
        {
            return chance => config().TreasureChance = GetDefaultVale(chance);
        }

        private Func<string> GetTreasureChanceValue()
        {
            return () => config().TreasureChance;
        }

        private static string[] HudPositionOptions()
        {
            return [I18n.Left(), I18n.Right()];
        }

        private Action<string> SetHudPositionValue()
        {
            return pos => config().ModStatusPosition = GetDefaultVale(pos);
        }

        private Func<string> GetHudPositionValue()
        {
            return () => config().ModStatusPosition;
        }

        private Action<SButton> SetCatchTreasureValue()
        {
            return button => config().CatchTreasureButton = button;
        }

        private Func<SButton> GetCatchTreasureValue()
        {
            return () => config().CatchTreasureButton;
        }

        private Action<SButton> SetEnableModValue()
        {
            return button => config().EnableModButton = button;
        }

        private Func<SButton> GetEnableModValue()
        {
            return () => config().EnableModButton;
        }

        string GetDefaultVale(string text)
        {
            if (text == I18n.Left()) return "Left"; 
            if (text == I18n.Right()) return "Right";
            if (text == I18n.UpperLeft()) return "Upper Left";
            if (text == I18n.UpperRight()) return "Upper Right";
            if (text == I18n.LowerLeft()) return "Lower Left";
            if (text == I18n.LowerRight()) return "Lower Right";
            if (text == I18n.Default()) return "Default";
            if (text == I18n.Always()) return "Always";
            if (text == I18n.Never()) return "Never";
            
            return string.Empty;
        }

        private void Register()
        {
            _configMenu?.Register(
                mod: modManifest,
                reset: configReset,
                save: configSave
            );
        }

        private void AddSectionTitle(string text)
        {
            _configMenu?.AddSectionTitle(
                mod: modManifest,
                text: () => text);
        }

        private void AddKeyBind(string text, Func<SButton> getValue, Action<SButton> setValue)
        {
            _configMenu?.AddKeybind(
                mod: modManifest,
                getValue: getValue,
                setValue: setValue,
                name: () => text);
        }

        private void AddDropDown(string name, Func<string> getValue, Action<string> setValue, string[] allowedValues = null)
        {
            _configMenu?.AddTextOption(
                mod: modManifest,
                name: () => name,
                getValue: getValue,
                setValue: setValue,
                allowedValues: allowedValues
            );
        }

        private void AddBool(string name, Func<bool> getValue, Action<bool> setValue)
        {
            _configMenu?.AddBoolOption(
                mod: modManifest,
                name: () => name,
                getValue: getValue,
                setValue: setValue);
        }

        private void AddNumber(string name, Func<int> getValue, Action<int> setValue)
        {
            _configMenu?.AddNumberOption(
                mod: modManifest,
                getValue: getValue,
                setValue: setValue,
                name: ()=> name);
        }
    }
}
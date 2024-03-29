using ChibiKyu.StardewMods.Common;
using FishingAssistant2;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Objects;
using Object = StardewValley.Object;

namespace ChibiKyu.StardewMods.FishingAssistant2.Frameworks
{
    internal class ConfigMenu(IModHelper modHelper, IModRegistry modRegistry, IManifest modManifest, Func<ModConfig> config, Action configReset, Action configSave)
    {
        private IGenericModConfigMenuApi? _configMenu;
        
        private readonly List<string> _availableBaits = new List<string>();
        private readonly List<string> _availableTackles = new List<string>();
        
        public void RegisterModConfigMenu()
        {
            _configMenu = modRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (_configMenu is null) return;
            
            _availableBaits.Add("Any");
            _availableTackles.Add("Any");
            
            foreach (KeyValuePair<string, ObjectData> item in Game1.objectData)
            {
                if (item.Value.Category == Object.baitCategory)
                    _availableBaits.Add(ItemRegistry.QualifyItemId(item.Key));
                else if (item.Value.Category == Object.tackleCategory)
                    _availableTackles.Add(ItemRegistry.QualifyItemId(item.Key));
            }
            
            ModConfig modConfig = config();
            
            Register();

            AddSectionTitle(I18n.ConfigMenu_Title_KeyBinding);
            AddKeyBind(I18n.ConfigMenu_Option_ToggleMod, () => modConfig.EnableModButton, button => modConfig.EnableModButton = button);
            AddKeyBind(I18n.ConfigMenu_Option_CatchTreasure, () => modConfig.CatchTreasureButton, button => modConfig.CatchTreasureButton = button);

            AddSectionTitle(I18n.ConfigMenu_Title_Hud);
            AddDropDown(I18n.ConfigMenu_Option_HudPosition, HudPositionOptions(), ParseHudPosition, () => modConfig.ModStatusPosition, pos => modConfig.ModStatusPosition = pos);
            
            AddSectionTitle(I18n.ConfigMenu_Title_Fishing);
            AddBool(I18n.ConfigMenu_Option_MaxCastPower, () => modConfig.MaxCastPower, b => modConfig.MaxCastPower = b);
            AddBool(I18n.ConfigMenu_Option_InstantFishBite, () => modConfig.InstantFishBite, b => modConfig.InstantFishBite = b);
            AddDropDown(I18n.ConfigMenu_Option_PreferFishQuality, FishQualityOptions(), ParseFishQuality, () => modConfig.PreferFishQuality, quality => modConfig.PreferFishQuality = quality);
            AddBool(I18n.ConfigMenu_Option_AlwaysPerfect, () => modConfig.AlwaysPerfect, b => modConfig.AlwaysPerfect = b);
            AddBool(I18n.ConfigMenu_Option_AlwaysMaxFishSize, () => modConfig.AlwaysMaxFishSize, b => modConfig.AlwaysMaxFishSize = b);
            AddDropDown(I18n.ConfigMenu_Option_TreasureChance, TreasureChanceOptions(), ParseTreasureChance, () => modConfig.TreasureChance, chance => modConfig.TreasureChance = chance);
            AddBool(I18n.ConfigMenu_Option_InstantCatchFish, () => modConfig.InstantCatchFish, b => modConfig.InstantCatchFish = b);
            AddBool(I18n.ConfigMenu_Option_InstantCatchTreasure, () => modConfig.InstantCatchTreasure, b => modConfig.InstantCatchTreasure = b);
            AddNumber(I18n.ConfigMenu_Option_FishDifficultyMultiplier, () => modConfig.FishDifficultyMultiplier, i => modConfig.FishDifficultyMultiplier = i);
            AddNumber(I18n.ConfigMenu_Option_FishDifficultyAdditive, () => modConfig.FishDifficultyAdditive, i => modConfig.FishDifficultyAdditive = i);

            AddSectionTitle(I18n.ConfigMenu_Title_Automation);
            AddBool(I18n.ConfigMenu_Option_AutoCastFishingRod, () => modConfig.AutoCastFishingRod, b => modConfig.AutoCastFishingRod = b);
            AddBool(I18n.ConfigMenu_Option_AutoHookFish, () => modConfig.AutoHookFish, b => modConfig.AutoHookFish = b);
            AddBool(I18n.ConfigMenu_Option_AutoPlayMiniGame, () => modConfig.AutoPlayMiniGame, b => modConfig.AutoPlayMiniGame = b);
            AddBool(I18n.ConfigMenu_Option_AutoClosePopup, () => modConfig.AutoClosePopup, b => modConfig.AutoClosePopup = b);
            AddBool(I18n.ConfigMenu_Option_AutoLootTreasure, () => modConfig.AutoLootTreasure, b => modConfig.AutoLootTreasure = b);
            AddDropDown(I18n.ConfigMenu_Option_ActionIfInventoryFull, ActionOnInventoryFullOptions(), ParseActionOnInventoryFull, () => modConfig.ActionIfInventoryFull, action => modConfig.ActionIfInventoryFull = action);
            
            AddSectionTitle(I18n.ConfigMenu_Title_FishingRod);
            AddBool(I18n.ConfigMenu_Option_AutoAttachBait, () => modConfig.AutoAttachBait, b => modConfig.AutoAttachBait = b);
            AddDropDown(I18n.ConfigMenu_Option_PreferBait, _availableBaits.ToArray(), ParseItemName, () => modConfig.PreferBait, s => modConfig.PreferBait = s);
            AddBool(I18n.ConfigMenu_Option_InfiniteBait, () => modConfig.InfiniteBait, b => modConfig.InfiniteBait = b);
            AddBool(I18n.ConfigMenu_Option_SpawnBaitIfDontHave, () => modConfig.SpawnBaitIfDontHave, b => modConfig.SpawnBaitIfDontHave = b);
            
            AddBool(I18n.ConfigMenu_Option_AutoAttachTackles, () => modConfig.AutoAttachTackles, b => modConfig.AutoAttachTackles = b);
            AddDropDown(I18n.ConfigMenu_Option_PreferTackle, _availableTackles.ToArray(), ParseItemName, () => modConfig.PreferTackle, s => modConfig.PreferTackle = s);
            AddBool(I18n.ConfigMenu_Option_InfiniteTackle, () => modConfig.InfiniteTackle, b => modConfig.InfiniteTackle = b);
            AddBool(I18n.ConfigMenu_Option_SpawnTackleIfDontHave, () => modConfig.SpawnTackleIfDontHave, b => modConfig.SpawnTackleIfDontHave = b);
            
            AddSectionTitle(I18n.ConfigMenu_Title_Enchantment);
            AddBool(I18n.ConfigMenu_Option_AddAutoHookEnchantment, () => modConfig.AddAutoHookEnchantment, b => modConfig.AddAutoHookEnchantment = b);
            AddBool(I18n.ConfigMenu_Option_AddEfficientEnchantment, () => modConfig.AddEfficientEnchantment, b => modConfig.AddEfficientEnchantment = b);
            AddBool(I18n.ConfigMenu_Option_AddMasterEnchantment, () => modConfig.AddMasterEnchantment, b => modConfig.AddMasterEnchantment = b);
            AddBool(I18n.ConfigMenu_Option_AddPreservingEnchantment, () => modConfig.AddPreservingEnchantment, b => modConfig.AddPreservingEnchantment = b);
            AddBool(I18n.ConfigMenu_Option_RemoveEnchantmentsWhenUnequipped, () => modConfig.RemoveEnchantmentWhenUnequipped, b => modConfig.RemoveEnchantmentWhenUnequipped = b);
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
        
        private void AddNumber(Func<string> name, Func<float> getValue, Action<float> setValue)
        {
            _configMenu?.AddNumberOption(
                mod: modManifest,
                getValue: getValue,
                setValue: setValue,
                name: name);
        }
    }
}
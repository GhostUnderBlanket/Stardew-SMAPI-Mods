using ChibiKyu.StardewMods.Common;
using FishingAssistant2;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

namespace ChibiKyu.StardewMods.FishingAssistant2.Frameworks
{
    public class ConfigUtil(IGenericModConfigMenuApi? configMenu, IManifest modManifest, Func<ModConfig> config)
    {
        internal class KeyBindOption
        {
            internal Func<SButton> GetValue;

            internal Func<string> Name;

            internal Action<SButton> SetValue;

            internal Func<string> Tooltip;
        }

        internal class DropdownOption
        {
            internal string[] AllowedValue;

            internal Func<string, string> FormatAllowedValue;

            internal Func<string> GetValue;

            internal Func<string> Name;

            internal Action<string> SetValue;

            internal Func<string> Tooltip;
        }

        internal class BoolOption
        {
            internal Func<bool> GetValue;

            internal Func<string> Name;

            internal Action<bool> SetValue;

            internal Func<string> Tooltip;
        }

        internal class IntOption
        {
            internal Func<int, string> FormatValue;

            internal Func<int> GetValue;

            internal int? Interval;

            internal int? Max;

            internal int? Min;

            internal Func<string> Name;

            internal Action<int> SetValue;

            internal Func<string> Tooltip;

            internal string UniqueID = null;
        }

        internal class FloatOption
        {
            internal Func<float, string> FormatValue;

            internal Func<float> GetValue;

            internal float? Interval;

            internal float? Max;

            internal float? Min;

            internal Func<string> Name;

            internal Action<float> SetValue;

            internal Func<string> Tooltip;

            internal string UniqueID = null;
        }

        internal class TextOption
        {
            internal Func<string> GetValue;

            internal Action<string> SetValue;

            internal Func<string> Name;

            internal Func<string> Tooltip;
        }

        #region Option Keybind

        internal readonly KeyBindOption CatchTreasure = new()
        {
            Name = I18n.ConfigMenu_Option_CatchTreasure,
            GetValue = () => config().CatchTreasureButton,
            SetValue = button => config().CatchTreasureButton = button,
            Tooltip = I18n.Tooltip_CatchTreasureButton
        };

        internal readonly KeyBindOption OpenConfigMenu = new()
        {
            Name = I18n.ConfigMenu_Option_OpenConfigMenu,
            GetValue = () => config().OpenConfigMenuButton,
            SetValue = button => config().OpenConfigMenuButton = button,
            Tooltip = I18n.Tooltip_OpenConfigMenu
        };

        internal readonly KeyBindOption ToggleAutomation = new()
        {
            Name = I18n.ConfigMenu_Option_ToggleAutomation,
            GetValue = () => config().EnableAutomationButton,
            SetValue = button => config().EnableAutomationButton = button,
            Tooltip = I18n.Tooltip_EnableAutomationButton
        };

        #endregion

        #region Option Dropdown

        internal readonly DropdownOption HudPosition = new()
        {
            Name = I18n.ConfigMenu_Option_HudPosition,
            GetValue = () => config().ModStatusPosition,
            SetValue = option => config().ModStatusPosition = option,
            AllowedValue = HudPositionOptions(),
            FormatAllowedValue = ParseHudPosition,
            Tooltip = I18n.ConfigMenu_Option_HudPosition
        };

        internal readonly DropdownOption ActionIfInventoryFull = new()
        {
            Name = I18n.ConfigMenu_Option_ActionIfInventoryFull,
            GetValue = () => config().ActionIfInventoryFull,
            SetValue = option => config().ActionIfInventoryFull = option,
            AllowedValue = ActionOnInventoryFullOptions(),
            FormatAllowedValue = ParseActionOnInventoryFull,
            Tooltip = I18n.Tooltip_ActionIfInventoryFull
        };

        internal readonly DropdownOption AutoPauseFishing = new()
        {
            Name = I18n.ConfigMenu_Option_AutoPauseFishing,
            GetValue = () => config().AutoPauseFishing,
            SetValue = option => config().AutoPauseFishing = option,
            AllowedValue = PauseFishingOptions(),
            FormatAllowedValue = ParsePauseFishing,
            Tooltip = I18n.Tooltip_AutoPauseFishing
        };

        internal readonly DropdownOption PreferBait = new()
        {
            Name = I18n.ConfigMenu_Option_PreferBait,
            GetValue = () => config().PreferredBait,
            SetValue = option => config().PreferredBait = option,
            AllowedValue = PreferredBaitOptions(),
            FormatAllowedValue = ParseItemName,
            Tooltip = I18n.Tooltip_PreferredBait
        };

        internal readonly DropdownOption PreferredTackle = new()
        {
            Name = I18n.ConfigMenu_Option_PreferTackle,
            GetValue = () => config().PreferredTackle,
            SetValue = option => config().PreferredTackle = option,
            AllowedValue = PreferredTackleOptions(),
            FormatAllowedValue = ParseItemName,
            Tooltip = I18n.Tooltip_PreferredTackle
        };

        internal readonly DropdownOption PreferredAdvIridiumTackle = new()
        {
            Name = I18n.ConfigMenu_Option_PreferAdvancedIridiumTackle,
            GetValue = () => config().PreferredAdvIridiumTackle,
            SetValue = option => config().PreferredAdvIridiumTackle = option,
            AllowedValue = PreferredTackleOptions(),
            FormatAllowedValue = ParseItemName,
            Tooltip = I18n.Tooltip_PreferredAdvIridiumTackle
        };

        internal readonly DropdownOption SkipFishingMiniGame = new()
        {
            Name = I18n.ConfigMenu_Option_SkipFishingMiniGame,
            GetValue = () => config().SkipFishingMiniGame,
            SetValue = option => config().SkipFishingMiniGame = option,
            AllowedValue = SkipFishingMiniGameOptions(),
            FormatAllowedValue = ParseSkipFishingMiniGame,
            Tooltip = I18n.Tooltip_SkipFishingMiniGame
        };

        internal readonly DropdownOption PreferFishQuality = new()
        {
            Name = I18n.ConfigMenu_Option_PreferFishQuality,
            GetValue = () => config().PreferFishQuality,
            SetValue = option => config().PreferFishQuality = option,
            AllowedValue = PreferFishQualityOptions(),
            FormatAllowedValue = ParsePreferFishQuality,
            Tooltip = I18n.Tooltip_PreferFishQuality
        };

        internal readonly DropdownOption TreasureChance = new()
        {
            Name = I18n.ConfigMenu_Option_TreasureChance,
            GetValue = () => config().TreasureChance,
            SetValue = option => config().TreasureChance = option,
            AllowedValue = TreasureChanceOptions(),
            FormatAllowedValue = ParseTreasureChance,
            Tooltip = I18n.Tooltip_TreasureChance
        };

        internal readonly DropdownOption GoldenTreasureChance = new()
        {
            Name = I18n.ConfigMenu_Option_GoldenTreasureChance,
            GetValue = () => config().GoldenTreasureChance,
            SetValue = option => config().GoldenTreasureChance = option,
            AllowedValue = TreasureChanceOptions(),
            FormatAllowedValue = ParseTreasureChance,
            Tooltip = I18n.Tooltip_GoldenTreasureChance
        };

        internal readonly DropdownOption StartWithFishingRod = new()
        {
            Name = I18n.ConfigMenu_Option_StartWithFishingRod,
            GetValue = () => config().StartWithFishingRod,
            SetValue = option => config().StartWithFishingRod = option,
            AllowedValue = StartWithFishingRodOptions(),
            FormatAllowedValue = ParseItemName,
            Tooltip = I18n.Tooltip_StartWithFishingRod
        };

        #endregion

        #region Option Bool

        internal readonly BoolOption AutoCastFishingRod = new()
        {
            Name = I18n.ConfigMenu_Option_AutoCastFishingRod,
            GetValue = () => config().AutoCastFishingRod,
            SetValue = value => config().AutoCastFishingRod = value,
            Tooltip = I18n.Tooltip_AutoCastFishingRod
        };

        internal readonly BoolOption AutoHookFish = new()
        {
            Name = I18n.ConfigMenu_Option_AutoHookFish, GetValue = () => config().AutoHookFish, SetValue = value => config().AutoHookFish = value, Tooltip = I18n.Tooltip_AutoHookFish
        };

        internal readonly BoolOption AutoPlayMiniGame = new()
        {
            Name = I18n.ConfigMenu_Option_AutoPlayMiniGame,
            GetValue = () => config().AutoPlayMiniGame,
            SetValue = value => config().AutoPlayMiniGame = value,
            Tooltip = I18n.Tooltip_AutoPlayMiniGame
        };

        internal readonly BoolOption AutoClosePopup = new()
        {
            Name = I18n.ConfigMenu_Option_AutoClosePopup, GetValue = () => config().AutoClosePopup, SetValue = value => config().AutoClosePopup = value, Tooltip = I18n.Tooltip_AutoClosePopup
        };

        internal readonly BoolOption AutoLootTreasure = new()
        {
            Name = I18n.ConfigMenu_Option_AutoLootTreasure,
            GetValue = () => config().AutoLootTreasure,
            SetValue = value => config().AutoLootTreasure = value,
            Tooltip = I18n.Tooltip_AutoLootTreasure
        };

        internal readonly BoolOption AutoTrashJunk = new()
        {
            Name = I18n.ConfigMenu_Option_AutoTrashJunk, GetValue = () => config().AutoTrashJunk, SetValue = value => config().AutoTrashJunk = value, Tooltip = I18n.Tooltip_AutoTrashJunk
        };

        internal readonly BoolOption AllowTrashFish = new()
        {
            Name = I18n.ConfigMenu_Option_AllowTrashFish, GetValue = () => config().AllowTrashFish, SetValue = value => config().AllowTrashFish = value, Tooltip = I18n.Tooltip_AllowTrashFish
        };

        internal readonly BoolOption AutoEatFood = new()
        {
            Name = I18n.ConfigMenu_Option_AutoEatFood, GetValue = () => config().AutoEatFood, SetValue = value => config().AutoEatFood = value, Tooltip = I18n.Tooltip_AutoEatFood
        };

        internal readonly BoolOption AllowEatingFish = new()
        {
            Name = I18n.ConfigMenu_Option_AllowEatingFish, GetValue = () => config().AllowEatingFish, SetValue = value => config().AllowEatingFish = value, Tooltip = I18n.Tooltip_AllowEatingFish
        };

        internal readonly BoolOption AutoAttachBait = new()
        {
            Name = I18n.ConfigMenu_Option_AutoAttachBait, GetValue = () => config().AutoAttachBait, SetValue = value => config().AutoAttachBait = value, Tooltip = I18n.Tooltip_AutoAttachBait
        };

        internal readonly BoolOption SpawnBaitIfDontHave = new()
        {
            Name = I18n.ConfigMenu_Option_SpawnBaitIfDontHave,
            GetValue = () => config().SpawnBaitIfDontHave,
            SetValue = value => config().SpawnBaitIfDontHave = value,
            Tooltip = I18n.Tooltip_SpawnBaitIfDontHave
        };

        internal readonly BoolOption AutoAttachTackles = new()
        {
            Name = I18n.ConfigMenu_Option_AutoAttachTackles,
            GetValue = () => config().AutoAttachTackles,
            SetValue = value => config().AutoAttachTackles = value,
            Tooltip = I18n.Tooltip_AutoAttachTackles
        };

        internal readonly BoolOption SpawnTackleIfDontHave = new()
        {
            Name = I18n.ConfigMenu_Option_SpawnTackleIfDontHave,
            GetValue = () => config().SpawnTackleIfDontHave,
            SetValue = value => config().SpawnTackleIfDontHave = value,
            Tooltip = I18n.Tooltip_SpawnTackleIfDontHave
        };

        internal readonly BoolOption InstantFishBite = new()
        {
            Name = I18n.ConfigMenu_Option_InstantFishBite, GetValue = () => config().InstantFishBite, SetValue = value => config().InstantFishBite = value, Tooltip = I18n.Tooltip_InstantFishBite
        };

        internal readonly BoolOption AlwaysPerfect = new()
        {
            Name = I18n.ConfigMenu_Option_AlwaysPerfect, GetValue = () => config().AlwaysPerfect, SetValue = value => config().AlwaysPerfect = value, Tooltip = I18n.Tooltip_AlwaysPerfect
        };

        internal readonly BoolOption AlwaysMaxFishSize = new()
        {
            Name = I18n.ConfigMenu_Option_AlwaysMaxFishSize,
            GetValue = () => config().AlwaysMaxFishSize,
            SetValue = value => config().AlwaysMaxFishSize = value,
            Tooltip = I18n.Tooltip_AlwaysMaxFishSize
        };

        internal readonly BoolOption InstantCatchTreasure = new()
        {
            Name = I18n.ConfigMenu_Option_InstantCatchTreasure,
            GetValue = () => config().InstantCatchTreasure,
            SetValue = value => config().InstantCatchTreasure = value,
            Tooltip = I18n.Tooltip_InstantCatchTreasure
        };

        internal readonly BoolOption DisplayFishPreview = new()
        {
            Name = I18n.ConfigMenu_Option_DisplayFishPreview,
            GetValue = () => config().DisplayFishPreview,
            SetValue = value => config().DisplayFishPreview = value,
            Tooltip = I18n.Tooltip_DisplayFishPreview
        };

        internal readonly BoolOption ShowFishName = new()
        {
            Name = I18n.ConfigMenu_Option_ShowFishName, GetValue = () => config().ShowFishName, SetValue = value => config().ShowFishName = value, Tooltip = I18n.Tooltip_ShowFishName
        };

        internal readonly BoolOption ShowTreasure = new()
        {
            Name = I18n.ConfigMenu_Option_ShowTreasure, GetValue = () => config().ShowTreasure, SetValue = value => config().ShowTreasure = value, Tooltip = I18n.Tooltip_ShowTreasure
        };

        internal readonly BoolOption ShowUncaughtFish = new()
        {
            Name = I18n.ConfigMenu_Option_ShowUncaughtFish,
            GetValue = () => config().ShowUncaughtFish,
            SetValue = value => config().ShowUncaughtFish = value,
            Tooltip = I18n.Tooltip_ShowUncaughtFish
        };

        internal readonly BoolOption ShowLegendaryFish = new()
        {
            Name = I18n.ConfigMenu_Option_ShowLegendaryFish,
            GetValue = () => config().ShowLegendaryFish,
            SetValue = value => config().ShowLegendaryFish = value,
            Tooltip = I18n.Tooltip_ShowLegendaryFish
        };

        internal readonly BoolOption InfiniteBait = new()
        {
            Name = I18n.ConfigMenu_Option_InfiniteBait, GetValue = () => config().InfiniteBait, SetValue = value => config().InfiniteBait = value, Tooltip = I18n.Tooltip_InfiniteBait
        };

        internal readonly BoolOption InfiniteTackle = new()
        {
            Name = I18n.ConfigMenu_Option_InfiniteTackle, GetValue = () => config().InfiniteTackle, SetValue = value => config().InfiniteTackle = value, Tooltip = I18n.Tooltip_InfiniteTackle
        };

        internal readonly BoolOption AddAutoHookEnchantment = new()
        {
            Name = I18n.ConfigMenu_Option_AddAutoHookEnchantment,
            GetValue = () => config().AddAutoHookEnchantment,
            SetValue = value => config().AddAutoHookEnchantment = value,
            Tooltip = I18n.Tooltip_AddAutoHookEnchantment
        };

        internal readonly BoolOption AddEfficientEnchantment = new()
        {
            Name = I18n.ConfigMenu_Option_AddEfficientEnchantment,
            GetValue = () => config().AddEfficientEnchantment,
            SetValue = value => config().AddEfficientEnchantment = value,
            Tooltip = I18n.Tooltip_AddEfficientEnchantment
        };

        internal readonly BoolOption AddMasterEnchantment = new()
        {
            Name = I18n.ConfigMenu_Option_AddMasterEnchantment,
            GetValue = () => config().AddMasterEnchantment,
            SetValue = value => config().AddMasterEnchantment = value,
            Tooltip = I18n.Tooltip_AddMasterEnchantment
        };

        internal readonly BoolOption AddPreservingEnchantment = new()
        {
            Name = I18n.ConfigMenu_Option_AddPreservingEnchantment,
            GetValue = () => config().AddPreservingEnchantment,
            SetValue = value => config().AddPreservingEnchantment = value,
            Tooltip = I18n.Tooltip_AddPreservingEnchantment
        };

        internal readonly BoolOption RemoveWhenUnequipped = new()
        {
            Name = I18n.ConfigMenu_Option_RemoveEnchantmentsWhenUnequipped,
            GetValue = () => config().RemoveWhenUnequipped,
            SetValue = value => config().RemoveWhenUnequipped = value,
            Tooltip = I18n.Tooltip_RemoveWhenUnequipped
        };

        #endregion

        #region Option Number

        internal readonly IntOption JunkHighestPrice = new()
        {
            Name = I18n.ConfigMenu_Option_JunkHighestPrice,
            Tooltip = I18n.Tooltip_JunkHighestPrice,
            GetValue = () => config().JunkHighestPrice,
            SetValue = value => config().JunkHighestPrice = value,
            Min = 0
        };

        internal readonly IntOption TimeToPause = new()
        {
            Name = I18n.ConfigMenu_Option_TimeToPause,
            Tooltip = I18n.Tooltip_TimeToPause,
            GetValue = () => config().TimeToPause,
            SetValue = value => config().TimeToPause = value,
            Min = 6,
            Max = 25,
            FormatValue = value => Game1.getTimeOfDayString(value * 100)
        };

        internal readonly IntOption WarnCount = new()
        {
            Name = I18n.ConfigMenu_Option_WarnCount,
            Tooltip = I18n.Tooltip_WarnCount,
            GetValue = () => config().WarnCount,
            SetValue = value => config().WarnCount = value,
            Min = 1,
            Max = 5,
            Interval = 1
        };

        internal readonly IntOption EnergyPercentToEat = new()
        {
            Name = I18n.ConfigMenu_Option_EnergyPercentToEat,
            Tooltip = I18n.Tooltip_EnergyPercentToEat,
            GetValue = () => config().EnergyPercentToEat,
            SetValue = value => config().EnergyPercentToEat = value,
            Min = 5,
            Max = 95,
            Interval = 5
        };

        internal readonly IntOption BaitAmountToSpawn = new()
        {
            Name = I18n.ConfigMenu_Option_BaitAmountToSpawn,
            Tooltip = I18n.Tooltip_BaitAmountToSpawn,
            GetValue = () => config().BaitAmountToSpawn,
            SetValue = value => config().BaitAmountToSpawn = value,
            Min = 1,
            Max = 999,
            Interval = 1
        };

        internal readonly IntOption PreferFishAmount = new()
        {
            Name = I18n.ConfigMenu_Option_PreferFishAmount,
            Tooltip = I18n.Tooltip_PreferFishAmount,
            GetValue = () => config().PreferFishAmount,
            SetValue = value => config().PreferFishAmount = value,
            Min = 1,
            Max = 3,
            Interval = 1
        };

        internal readonly FloatOption FishDifficultyMultiplier = new()
        {
            Name = I18n.ConfigMenu_Option_FishDifficultyMultiplier,
            Tooltip = I18n.Tooltip_FishDifficultyMultiplier,
            GetValue = () => config().FishDifficultyMultiplier,
            SetValue = value => config().FishDifficultyMultiplier = value
        };

        internal readonly IntOption FishDifficultyAdditive = new()
        {
            Name = I18n.ConfigMenu_Option_FishDifficultyAdditive,
            Tooltip = I18n.Tooltip_FishDifficultyAdditive,
            GetValue = () => config().FishDifficultyAdditive,
            SetValue = value => config().FishDifficultyAdditive = value
        };

        internal readonly IntOption DefaultCastPower = new()
        {
            Name = I18n.ConfigMenu_Option_DefaultCastPower,
            Tooltip = I18n.Tooltip_DefaultCastPower,
            GetValue = () => config().DefaultCastPower,
            SetValue = value => config().DefaultCastPower = value,
            Min = 0,
            Max = 100,
            Interval = 5
        };

        internal readonly FloatOption UnlockCastPowerTime = new()
        {
            Name = I18n.ConfigMenu_Option_UnlockCastPowerTime,
            Tooltip = I18n.Tooltip_UnlockCastPowerTime,
            GetValue = () => config().UnlockCastPowerTime,
            SetValue = value => config().UnlockCastPowerTime = value,
            Min = 0,
            Max = 3,
            Interval = 1,
            FormatValue = ParseUnlockCastPowerTime,
            UniqueID = "ChibiKyu.FishingAssistant2.UnlockCastPowerTime"
        };

        #endregion

        #region Option Text

        internal readonly TextOption JunkIgnoreList = new()
        {
            Name = I18n.ConfigMenu_Option_JunkIgnoreList,
            GetValue = () => string.Join(',', config().JunkIgnoreList),
            SetValue = value => config().JunkIgnoreList = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList(),
            Tooltip = I18n.Tooltip_JunkIgnoreList
        };

        #endregion

        #region Options

        internal void AddKeyBind(KeyBindOption option)
        {
            configMenu?.AddKeybind(modManifest, name: option.Name, getValue: option.GetValue, setValue: option.SetValue, tooltip: option.Tooltip);
        }

        internal void AddDropDown(DropdownOption option)
        {
            configMenu?.AddTextOption(modManifest, name: option.Name, getValue: option.GetValue, setValue: option.SetValue, allowedValues: option.AllowedValue,
                formatAllowedValue: option.FormatAllowedValue, tooltip: option.Tooltip);
        }

        internal void AddBool(BoolOption option)
        {
            configMenu?.AddBoolOption(modManifest, name: option.Name, getValue: option.GetValue, setValue: option.SetValue, tooltip: option.Tooltip);
        }

        internal void AddNumber(IntOption option)
        {
            configMenu?.AddNumberOption(modManifest, option.GetValue, option.SetValue, option.Name, min: option.Min, max: option.Max, interval: option.Interval, formatValue: option.FormatValue,
                tooltip: option.Tooltip);
        }

        internal void AddNumber(FloatOption option)
        {
            configMenu?.AddNumberOption(modManifest, option.GetValue, option.SetValue, option.Name, min: option.Min, max: option.Max, interval: option.Interval, formatValue: option.FormatValue,
                tooltip: option.Tooltip, fieldId: option.UniqueID);
        }

        internal void AddText(TextOption option)
        {
            configMenu?.AddTextOption(modManifest, option.GetValue, option.SetValue, option.Name, option.Tooltip);
        }

        #endregion

        #region Warpper

        private static string[] StartWithFishingRodOptions()
        {
            List<string> availableBaits = ["None", "(T)TrainingRod", "(T)BambooPole"];

            return availableBaits.ToArray();
        }

        private static string[] PreferredBaitOptions()
        {
            List<string> availableBaits = ["Any"];
            availableBaits.AddRange(from item in Game1.objectData where item.Value.Category == Object.baitCategory select ItemRegistry.QualifyItemId(item.Key));

            return availableBaits.ToArray();
        }

        private static string[] PreferredTackleOptions()
        {
            List<string> availableTackles = ["Any"];
            availableTackles.AddRange(from item in Game1.objectData where item.Value.Category == Object.tackleCategory select ItemRegistry.QualifyItemId(item.Key));

            return availableTackles.ToArray();
        }

        private static string[] SkipFishingMiniGameOptions()
        {
            return Enum.GetNames(typeof(SkipFishingMiniGame));
        }

        private static string[] PauseFishingOptions()
        {
            return Enum.GetNames(typeof(PauseFishingBehaviour));
        }

        private static string[] PreferFishQualityOptions()
        {
            return Enum.GetNames(typeof(FishQuality));
        }

        private static string[] ActionOnInventoryFullOptions()
        {
            return Enum.GetNames(typeof(ActionOnInventoryFull));
        }

        private static string[] TreasureChanceOptions()
        {
            return Enum.GetNames(typeof(TreasureChance));
        }

        private static string[] HudPositionOptions()
        {
            return Enum.GetNames(typeof(HudPosition));
        }

        private static string ParseUnlockCastPowerTime(float value)
        {
            return value switch
            {
                <= 0 => I18n.InstantUnlock(),
                >= 3 => I18n.NeverUnlock(),
                _ => string.Format(I18n.Second(), value)
            };
        }

        private static string ParseSkipFishingMiniGame(string rawText)
        {
            if (!Enum.TryParse(rawText, out SkipFishingMiniGame text))
                return rawText;

            return text switch
            {
                Frameworks.SkipFishingMiniGame.Off => I18n.Off(),
                Frameworks.SkipFishingMiniGame.SkipAll => I18n.SkipAll(),
                Frameworks.SkipFishingMiniGame.SkipOnlyCaught => I18n.SkipOnlyCaught(),
                _ => text.ToString()
            };
        }

        private static string ParsePauseFishing(string rawText)
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

        private static string ParsePreferFishQuality(string rawText)
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

        private static string ParseItemName(string rawText)
        {
            return rawText switch
            {
                "Any" => I18n.Any(),
                "None" => I18n.None(),
                _ => ItemRegistry.GetData(rawText).DisplayName
            };
        }

        private static string ParseActionOnInventoryFull(string rawText)
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

        private static string ParseTreasureChance(string rawText)
        {
            if (!Enum.TryParse(rawText, out TreasureChance text))
                return rawText;

            return text switch
            {
                Frameworks.TreasureChance.Default => I18n.Default(),
                Frameworks.TreasureChance.Always => I18n.Always(),
                Frameworks.TreasureChance.Never => I18n.Never(),
                _ => text.ToString()
            };
        }

        private static string ParseHudPosition(string rawText)
        {
            if (!Enum.TryParse(rawText, out HudPosition text))
                return rawText;

            return text switch
            {
                Frameworks.HudPosition.Left => I18n.Left(),
                Frameworks.HudPosition.Right => I18n.Right(),
                _ => text.ToString()
            };
        }

        #endregion
    }
}
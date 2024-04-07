# Fishing Assistant 2

Simple ```Stardew Valley Mod``` which allows you to ```automatically catching fish``` and customize fishing mechanics by making it easier or harder even cheating it. this mod comes with additional features like fish and treasure preview, auto-stop fishing at night or low stamina, and more.

![FishingAssistant2](Image/FishingAssistant2.gif)

Fore more information please visit [Nexus](https://www.nexusmods.com/stardewvalley/mods/5815?tab=description&BH=14) mod page

## Configuration
The configuration file is located in the mod's folder under the StardewValley installation directory, and it's automatically created the first time the game is run with this mod installed. Alternatively, you can install the [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) for an in-game config menu.

### EnableAutomationButton
**Default value:** `F5`
**Possible values:** See `Modding:Player Guide/Key Bindings` on Stardew valley wiki. [Here](https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings#Button_codes)

```
Button for toggling automation of this mod.
```

### CatchTreasureButton
**Default value:** `F6`
**Possible values:** See `Modding:Player Guide/Key Bindings` on Stardew valley wiki. [Here](https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings#Button_codes)

```
Button for toggling catch or ignore treasure in fishing mini-game.
```

### ModStatusPosition
**Default value:** `Left`
**Possible values:** `Left | Right`

```
Position to display mod status.
```

### AutoCastFishingRod
**Default value:** `true`
**Possible values:** `true | false`

```
Toggle for auto cast fishing rod.
```

### AutoHookFish
**Default value:** `true`
**Possible values:** `true | false`

```
Toggle for auto hook fish.
```

### AutoPlayMiniGame
**Default value:** `true`
**Possible values:** `true | false`

```
Toggle for auto playing fishing mini-game.
```

### AutoClosePopup
**Default value:** `true`
**Possible values:** `true | false`

```
Toggle for auto closing fish popup.
```

### AutoLootTreasure
**Default value:** `true`
**Possible values:** `true | false`

```
Toggle for auto looting treasure.
```

### ActionIfInventoryFull
**Default value:** `Stop`
**Possible values:** `Stop | Drop | Discard`

```
Action to take if inventory is full.
```

### AutoTrashJunk
**Default value:** `false`
**Possible values:** `true | false`

```
Toggle for auto trash junk item when get new item.
```

### JunkHighestPrice
**Default value:** `10`
**Possible values:** `higher than 0`

```
The item that price that less than or equal to this will consider as junk.
```

### AutoPauseFishing
**Default value:** `WarnAndPause`
**Possible values:** `Off | WarnOnly | WarnAndPause`

```
Should mod auto pause fishing on night.
```

### PauseFishingTime
**Default value:** `24`
**Possible values:** `6 - 25`

```
Time to stop fishing.
```

### NumToWarn
**Default value:** `1`
**Possible values:** `1 - 5`

```
Number of warnings after time reach.
```

### AutoEatFood
**Default value:** `false`
**Possible values:** `true | false`

```
Whether to eat some food if need.
```

### EnergyPercentToEat
**Default value:** `10`
**Possible values:** `1 - 99`

```
Energy percent to consider as low energy and find something to eat.
```

### AllowEatingFish
**Default value:** `false`
**Possible values:** `true | false`

```
Toggle for allowing eating of caught fish.
```

### MaxCastPower
**Default value:** `true`
**Possible values:** `true | false`

```
Toggle for enabling max cast power.
```

### InstantFishBite
**Default value:** `false`
**Possible values:** `true | false`

```
Toggle for making fish bite instantly.
```

### InstantCatchFish
**Default value:** `false`
**Possible values:** `true | false`

```
Instantly catch fish when fish hooked.
```

### PreferFishAmount
**Default value:** `1`
**Possible values:** `1 - 3`

```
Preference for fish amount.
```

### PreferFishQuality
**Default value:** `Any`
**Possible values:** `Any | None | Silver | Gold | Iridium`

```
Preference for fish quality.
```

### AlwaysPerfect
**Default value:** `false`
**Possible values:** `true | false`

```
Whether to consider every catch as perfectly executed.
```

### AlwaysMaxFishSize
**Default value:** `false`
**Possible values:** `true | false`

```
Toggle for always maximum fish size.
```

### FishDifficultyMultiplier
**Default value:** `1`
**Possible values:** `0 - 1 to lower difficulty, or more than 1 to increase it.`

```
A multiplier applied to the fish difficulty.
```

### FishDifficultyAdditive
**Default value:** `0`
**Possible values:** `< 0 to lower difficulty, or > 0 to increase it.`

```
A value added to the fish difficulty.
```

### InstantCatchTreasure
**Default value:** `false`
**Possible values:** `true | false`

```
Instantly catch treasure when treasure appeared.
```

### TreasureChance
**Default value:** `Default`
**Possible values:** `Default | Always | Never`

```
Chance of finding treasure while fishing.
```

### GoldenTreasureChance
**Default value:** `Default`
**Possible values:** `Default | Always | Never`

```
Chance of finding golden treasure while fishing.
```

### DisplayFishPreview
**Default value:** `true`
**Possible values:** `true | false`

```
Toggle for displaying fish info while catching fish.
```

### ShowFishName
**Default value:** `true`
**Possible values:** `true | false`

```
Toggle for showing fish name with fish info.
```

### ShowTreasure
**Default value:** `true`
**Possible values:** `true | false`

```
Toggle for showing treasure with fish info.
```

### ShowUncaughtFishSpecies
**Default value:** `false`
**Possible values:** `true | false`

```
Show a preview for all fish species, even ones you have never caught.
```

### AlwaysShowLegendaryFish
**Default value:** `false`
**Possible values:** `true | false`

```
Toggle for always showing fish info if current fish is legendary.
```

### AutoAttachBait
**Default value:** `false`
**Possible values:** `true | false`

```
Toggle for auto attaching bait if possible.
```

### PreferBait
**Default value:** `Any`
**Possible values:** `Any | Bait Qualified Item ID`

```
Preference for bait type.
```

### InfiniteBait
**Default value:** `false`
**Possible values:** `true | false`

```
Make your fishing bait last long forever.
```

### SpawnBaitIfDontHave
**Default value:** `false`
**Possible values:** `true | false`

```
Toggle for spawning bait if none is available.
```

### BaitAmountToSpawn
**Default value:** `10`
**Possible values:** `1 - 999`

```
Amount of bait when spawned.
```

### AutoAttachTackles
**Default value:** `false`
**Possible values:** `true | false`

```
Toggle for auto attaching tackles if possible.
```

### PreferTackle
**Default value:** `Any`
**Possible values:** `Any | Tackle Qualified Item ID`

```
Preference for tackle type.
```

### PreferAdvIridiumTackle
**Default value:** `Any`
**Possible values:** `Any | Tackle Qualified Item ID`

```
Tackle type preference for Advance Iridium Rod.
```

### InfiniteTackle
**Default value:** `false`
**Possible values:** `true | false`

```
Make your tackle last long forever.
```

### SpawnTackleIfDontHave
**Default value:** `false`
**Possible values:** `true | false`

```
Toggle for spawning tackle if none is available.
```

### AddAutoHookEnchantment
**Default value:** `false`
**Possible values:** `true | false`

```
Toggle for add Auto-Hook enchantment to fishing rod.
```

### AddEfficientEnchantment
**Default value:** `false`
**Possible values:** `true | false`

```
Toggle for add Efficient enchantment to fishing rod.
```

### AddMasterEnchantment
**Default value:** `false`
**Possible values:** `true | false`

```
Toggle for add Master enchantment to fishing rod.
```

### AddPreservingEnchantment
**Default value:** `false`
**Possible values:** `true | false`

```
Toggle for add Preserving enchantment to fishing rod.
```

### OnlyAddEnchantmentWhenHeld
**Default value:** `false`
**Possible values:** `true | false`

```
Toggle for removing enchantment when fishing rod is unequipped.
```

## Thank you
- The Fishing Assistant 2 source code is available with an MIT license on GitHub page.
- Translations are welcome! You can go to the i18n folder on GitHub to translate my mod I already prepared a translation file for you. After that, create a pull request or you can send me the translated file directly via DM. If you prefer to upload the translated file yourself, please make sure that the file you upload contains only the translated content.
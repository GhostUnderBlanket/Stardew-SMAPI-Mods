# Fishing Assistant 2

Simple ```Stardew Valley Mod``` which allows you to ```automatically catching fish``` and customize fishing mechanics by making it easier or harder even cheating it. this mod comes with additional features like fish and treasure preview, auto-stop fishing at night or low stamina, and more.

## Feature

List of currently available mod features
- Automation mode (cast fishing rod, hook fish, play mini-games, loot treasure, attach bait/tackle).
- Tweak the fishing mechanic (max cast power, instant fish bite, treasure chance, etc)
- Modify equipped fishing rod (add/remove enchantment)
- Prevent players from fishing when stamina is low.
- Auto-stop fishing at night (time can be changed in the config file).
- ~~Preview current fish and show you if this round has a treasure.~~
- Configurable cheat in a config file.

## Requirements

- SMAPI - Stardew Modding API. [SMAPI Homepage](https://smapi.io/) | [Nexus](https://www.nexusmods.com/stardewvalley/mods/2400)
- Generic Mod Config Menu - For in-game config menu [Nexus](https://www.nexusmods.com/stardewvalley/mods/5098)

## Installation
- Download and Install all mod requirements.
- Decompress the downloaded zip file.
- Move the Fishing Assistant 2 folder to the Mods directory of your StardewValley installation. If you don't have a Mods directory, please ensure you have already downloaded and installed SMAPI and have launched the game at least once after its installation.

## Usage
When first run, the mod includes a default configuration that disables all cheats. You can enable this mod while in-game by pressing the F5 button on the keyboard and catch or ignore treasure by pressing the F6 button.

To further tweak the fishing mechanics, you need to edit the Mod configuration file located in the mod's directory. This file is created automatically once the game has been launched at least once with the mod installed. Please refer to the Configuration section for details on how to further tweak the mod.


## Remarks
- This mod doesn't affect the achievements you can get through Steam; fish counts will still increment as normal when a fish is reeled.
- The mod is designed to be compatible with the Multiplayer/Coop mode of the game. However, this is still being tested and may have problems playing the animation of characters on other players.
- Bug reports are available at the Nexus mod page [link](https://www.nexusmods.com/stardewvalley/mods/5815?tab=bugs). Bug fixes are prioritized based on availability and the severity of the problem.

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

### TreasureChance
**Default value:** `Default`
**Possible values:** `Default | Always | Never`

```
Chance of finding treasure while fishing.
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

### InstantCatchFish
**Default value:** `false`
**Possible values:** `true | false`

```
Instantly catch fish when fish hooked.
```

### InstantCatchTreasure
**Default value:** `false`
**Possible values:** `true | false`

```
Instantly catch treasure when treasure appeared.
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
**Default value:** `Stop Loot`
**Possible values:** `Stop | Drop | Discard`

```
Action to take if inventory is full.
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

### AllowEatingFish
**Default value:** `false`
**Possible values:** `true | false`

```
Toggle for allowing eating of caught fish.
```

### DisplayFishInfo
**Default value:** `true`
**Possible values:** `true | false`

```
Should mod show fish info while catching fish?
```

### ShowFishName
**Default value:** `true`
**Possible values:** `true | false`

```
Shows the text of the fish name under the icon.
```

### ShowTreasure
**Default value:** `true`
**Possible values:** `true | false`

```
Show treasure icon with fish info.
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
Show a preview for legendary fish.
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

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace FishingAssistant2
{
    /// <summary>The mod entry point.</summary>
    internal partial class ModEntry : Mod
    {
        private ModConfig _config;
        
        /*********
         ** Public methods
         *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();
            I18n.Init(helper.Translation);
            
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }
        
        /// <summary>Raised after the game is launched, right before the first update tick.</summary>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // read the Config for display position and get list priority for displayOrder
            ReloadConfig();
        }

        private void ReloadConfig(bool hideMessage = true)
        {
            _config = Helper.ReadConfig<ModConfig>();
            
            if (!hideMessage) AddHudMessage(2, I18n.Hud_Message_Config_Saved());
        }
        
        private void AddHudMessage(int whatType, string key, params object[] args)
        {
            if (!Context.IsWorldReady) return;
            Game1.addHUDMessage(new HUDMessage(string.Format(key, args), whatType));
        }

        /*********
         ** Private methods
         *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // print button presses to the console window
            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
        }
        
        
    }
}
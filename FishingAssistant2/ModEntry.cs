using System;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace FishingAssistant2
{
    internal class ModEntry : Mod
    {
        private ModConfig _config;
        private IGenericModConfigMenuApi? _configMenu;
        
        private bool _modEnable;
        private bool _maxCastPower;
        private bool _catchTreasure;
        
        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();
            I18n.Init(helper.Translation);
            
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Display.RenderingHud += this.OnRenderingHud;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            InitializeModConfigMenu();
        }

        private void OnRenderingHud(object? sender, RenderingHudEventArgs e)
        {
            DrawModStatus();
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            
            if (e.Button == _config.EnableModButton)
                _modEnable = !_modEnable;
            
            if (e.Button == _config.CastPowerButton)
            {
                _maxCastPower = !_maxCastPower;
                // if (Game1.isFestival())
                // {
                //     string status = maxCastPower ? I18n.Mod_Status_Enable() : I18n.Mod_Status_Disable();
                //     AddHUDMessage(2, I18n.Hud_Message_Cast_Power(), status);
                // }
            }
            
            if (e.Button == _config.CatchTreasureButton)
            {
                _catchTreasure = !_catchTreasure;
                // if (Game1.isFestival())
                // {
                //     string status = autoCatchTreasure ? I18n.Mod_Status_Enable() : I18n.Mod_Status_Disable();
                //     AddHUDMessage(2, I18n.Hud_Message_Catch_Treasure(), status);
                // }
            }
        }

        private void DrawModStatus()
        {
            if (Game1.eventUp || (!_modEnable && !_maxCastPower && !_catchTreasure)) return;
            
            float toolBarTransparency = 1;
            int toolBarWidth = 0;
            
            foreach (var menu in Game1.onScreenMenus)
            {
                if (menu is not Toolbar toolBar) continue;
                
                toolBarTransparency = Helper.Reflection.GetField<float>(toolBar, "transparency").GetValue();
                toolBarWidth = toolBar.width / 2;
                break;
            }
            
            Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
            Point playerGlobalPos = Game1.player.GetBoundingBox().Center;
            Vector2 playerLocalVec = Game1.GlobalToLocal(Game1.viewport, new Vector2(playerGlobalPos.X, playerGlobalPos.Y));
            bool alignTop = !Game1.options.pinToolbarToggle && playerLocalVec.Y > (float)(viewport.Height / 2 + 64);
            
            const int boxSize = 96;
            const int iconSize = 40;
            const int screenMargin = 8;
            const int spacing = 2;

            int toolbarOffset = _config.ModStatusDisplayPosition == "Left" ? -toolBarWidth - spacing : toolBarWidth + spacing;
            int boxPosX = viewport.Width / 2 + toolbarOffset - (boxSize / 2);
            int boxPosY = alignTop ? screenMargin : viewport.Height - screenMargin - boxSize;
            int boxCenterX = boxPosX + boxSize / 2;
            int boxCenterY = boxPosY + boxSize / 2;

            var rectangles = new[]
            {
                new Rectangle(0, 256, 60, 60), //Box Texture
                new Rectangle(20, 428, 10, 10), // Fishing Texture
                new Rectangle(545, 1921, 53, 19), // Max Texture
                new Rectangle(137, 412, 10, 11)  // Treasure Texture
            };
            
            IClickableMenu.drawTextureBox(Game1.spriteBatch, Game1.menuTexture, rectangles[0], boxPosX, boxPosY, boxSize, boxSize, Color.White * toolBarTransparency, drawShadow: false);
            DrawIcon(_modEnable, rectangles[1], boxCenterX - (boxSize / 4) - spacing, boxCenterY - (boxSize / 4) - spacing, 2f);
            DrawIcon(_maxCastPower, rectangles[2], boxCenterX - (boxSize / 4), boxCenterY + (boxSize / 4) - (iconSize / 2) + spacing, 1f);
            DrawIcon(_catchTreasure, rectangles[3], boxCenterX + (boxSize / 4) - (iconSize / 2) + spacing, boxCenterY - (boxSize / 4) - spacing, 2f);

            return;

            void DrawIcon(bool value, Rectangle source, int x, int y, float scale)
            {
                float iconTransparency = value ? 1 : 0.2f;
                var icon = new ClickableTextureComponent(new Rectangle(x, y, iconSize, iconSize), Game1.mouseCursors, source, scale);
                icon.draw(Game1.spriteBatch, Color.White * toolBarTransparency * iconTransparency, 0);
            }
        }


        #region Mod Config Menu

        private void InitializeModConfigMenu()
        {
            _configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (_configMenu is null)
                return;
            
            Register();

            AddSectionTitle("Key Binding");
            
            AddKeyBind("Enable Mod Button", () => _config.EnableModButton, button => _config.EnableModButton = button);
            AddKeyBind("Cast Power Button", () => _config.CastPowerButton, button => _config.CastPowerButton = button);
            AddKeyBind("Catch Treasure Button", () => _config.CatchTreasureButton, button => _config.CatchTreasureButton = button);

            AddSectionTitle("UI");
            AddDropDown("Mod Status display Position",
                () => _config.ModStatusDisplayPosition,
                pos => _config.ModStatusDisplayPosition = pos,
                new[] { "Left", "Right" });
        }

        private void Register()
        {
            _configMenu?.Register(
                mod: this.ModManifest,
                reset: () => _config = new ModConfig(),
                save: () => this.Helper.WriteConfig(_config)
            );
        }

        private void AddSectionTitle(string text)
        {
            _configMenu?.AddSectionTitle(
                mod: this.ModManifest,
                text: () => text);
        }

        private void AddKeyBind(string text, Func<SButton> getValue, Action<SButton> setValue)
        {
            _configMenu?.AddKeybind(
                mod: this.ModManifest,
                getValue: getValue,
                setValue: setValue,
                name: () => text);
        }

        private void AddDropDown(string name, Func<string> getValue, Action<string> setValue, string[] allowedValues = null)
        {
            _configMenu?.AddTextOption(
                mod: this.ModManifest,
                name: () => name,
                getValue: getValue,
                setValue: setValue,
                allowedValues: allowedValues
            );
        }

        #endregion
    }
}
using ChibiKyu.StardewMods.Common;
using ChibiKyu.StardewMods.FishingAssistant2.Frameworks;
using FishingAssistant2;
using FishingAssistant2.Frameworks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace ChibiKyu.StardewMods.FishingAssistant2
{
    internal class ModEntry : Mod
    {
        private ModConfig Config { get; set; }
        private Assistant Assistant { get; set; }

        public bool _modEnable;
        public bool _catchTreasure;
        
        public int _facingDirection;
        public int _standingPosX;
        public int _standingPosY;
        public bool _playerDataCached;
        
        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);
            
            this.Config = helper.ReadConfig<ModConfig>();
            
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Display.RenderingHud += this.OnRenderingHud;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            Assistant = new Assistant(()=> this,() => Config);
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var configMenu = new ConfigMenu(this.Helper.ModRegistry, this.ModManifest, () => Config, () => Config = new ModConfig(), () => this.Helper.WriteConfig(Config));
            configMenu.RegisterModConfigMenu();
        }
        
        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            
            if (Game1.player.CurrentTool is FishingRod fishingRod)
                Assistant.OnEquipFishingRod(fishingRod, _modEnable);
            if (Game1.player.CurrentTool is not FishingRod)
                Assistant.OnUnEquipFishingRod();

            Assistant.DoOnUpdateAssistantTask();
        }

        private void OnRenderingHud(object? sender, RenderingHudEventArgs e)
        {
            DrawModStatus();
        }
        
        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is BobberBar bobberBar)
                Assistant.OnFishingMiniGameStart(bobberBar);
            else if (e.OldMenu is BobberBar)
                Assistant.OnFishingMiniGameEnd();

            if (e.NewMenu is ItemGrabMenu { source: ItemGrabMenu.source_fishingChest } itemGrabMenu)
                Assistant.OnTreasureMenuOpen(itemGrabMenu);
        }
        
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            if (e.Button == Config.EnableModButton)
            {
                _modEnable = !_modEnable;
                Game1.playSound("coin");

                if (!_modEnable) ForgetPlayerPosition();
            }
            
            if (e.Button == Config.CatchTreasureButton)
            {
                _catchTreasure = !_catchTreasure;
                Game1.playSound("dwop");
            }
        }
        
        public void CachePlayerPosition()
        {
            _facingDirection = Game1.player.getDirection() != -1 ? Game1.player.getDirection() : Game1.player.FacingDirection;
            _standingPosX = (int)Game1.player.getStandingPosition().X;
            _standingPosY = (int)Game1.player.getStandingPosition().Y;

            _playerDataCached = true;
        }

        public void ForgetPlayerPosition()
        {
            if (_playerDataCached)
            {
                _facingDirection = 0;
                _standingPosX = 0;
                _standingPosY = 0;

                _playerDataCached = false;
            }
        }
        
        public void ForceDisable()
        {
            Game1.playSound("coin");
            _modEnable = false;
            CommonHelper.PushToggleNotification(_modEnable, I18n.HudMessage_ModEnable());
        }

        private void DrawModStatus()
        {
            if (Game1.eventUp && !Game1.isFestival() || (!_modEnable && !_catchTreasure)) 
                return;
            
            float toolBarTransparency = 0;
            int toolBarWidth = 0;
            
            foreach (var menu in Game1.onScreenMenus)
            {
                if (menu is not Toolbar toolBar) continue;
                
                toolBarTransparency = Helper.Reflection.GetField<float>(toolBar, "transparency").GetValue();
                toolBarWidth = toolBar.width / 2;
                break;
            }
            
            var viewport = Game1.graphics.GraphicsDevice.Viewport;
            var playerGlobalPos = Game1.player.GetBoundingBox().Center;
            var playerLocalVec = Game1.GlobalToLocal(Game1.viewport, new Vector2(playerGlobalPos.X, playerGlobalPos.Y));
            bool alignTop = !Game1.options.pinToolbarToggle && playerLocalVec.Y > (float)(viewport.Height / 2 + 64);
            
            const int boxSize = 96;
            const int iconSize = 20;
            const int screenMargin = 8;
            const int spacing = 2;

            int toolbarOffset = (toolBarTransparency == 0 || Assistant.IsInFishingMiniGame || Game1.isFestival()) ? 0 : 
                Config.ModStatusPosition == "Left" ? -toolBarWidth - spacing : toolBarWidth + spacing;
            int boxPosX = viewport.Width / 2 + toolbarOffset - (boxSize / 2);
            int boxPosY = alignTop ? screenMargin : viewport.Height - screenMargin - boxSize;
            int boxCenterX = boxPosX + boxSize / 2;
            int boxCenterY = boxPosY + boxSize / 2;
            int offset = boxSize / 4;

            var rectangles = new[]
            {
                new Rectangle(0, 256, 60, 60), //Box Texture
                new Rectangle(20, 428, 10, 10), // Fishing Texture
                new Rectangle(137, 412, 10, 11)  // Treasure Texture
            };
            
            IClickableMenu.drawTextureBox(Game1.spriteBatch, Game1.menuTexture, rectangles[0], boxPosX, boxPosY, boxSize, boxSize, Color.White * toolBarTransparency, drawShadow: false);
            DrawIcon(_modEnable, rectangles[1], boxCenterX - (iconSize / 2), boxPosY + offset, 2f);
            DrawIcon(_catchTreasure, rectangles[2], boxCenterX - (iconSize / 2), boxPosY + boxSize - offset - iconSize, 2f);

            return;

            void DrawIcon(bool value, Rectangle source, int x, int y, float scale)
            {
                float iconTransparency = value ? 1 : 0.2f;
                var icon = new ClickableTextureComponent(new Rectangle(x, y, iconSize, iconSize), Game1.mouseCursors, source, scale);
                icon.draw(Game1.spriteBatch, Color.White * toolBarTransparency * iconTransparency, 0);
            }
        }
    }
}
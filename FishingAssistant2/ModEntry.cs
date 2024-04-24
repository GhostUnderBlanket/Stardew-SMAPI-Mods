using ChibiKyu.StardewMods.Common;
using ChibiKyu.StardewMods.FishingAssistant2.Frameworks;
using FishingAssistant2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace ChibiKyu.StardewMods.FishingAssistant2
{
    internal class ModEntry : Mod
    {
        private const int Margin = 10;

        private const float TextScale = 0.5f;

        private const int TreasureOffsetX = 20;

        private const int TreasureOffsetY = 15;

        private int _boxHeight;

        private int _boxWidth;

        private string _fishPreviewId = "";

        private Object? _fishSprite;

        private Vector2 _textSize;

        private string? _textValue;

        private Object? _treasureSprite;

        public bool CatchTreasure;

        public bool AutomationEnable;

        private ModConfig Config { get; set; }

        private Assistant Assistant { get; set; }

        private ConfigMenu ConfigMenu { get; set; }

        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);

            Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            helper.Events.Player.Warped += OnWarped;
            helper.Events.Player.InventoryChanged += OnInventoryChanged;
            helper.Events.Display.RenderingHud += OnRenderingHud;
            helper.Events.Display.RenderedActiveMenu += OnRenderMenu;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.Input.ButtonPressed += OnButtonPressed;

            Assistant = new Assistant(() => this, () => Config);
        }

        public void ForceDisable()
        {
            Game1.playSound("coin");
            AutomationEnable = false;

            CommonHelper.PushToggleNotification(AutomationEnable, I18n.HudMessage_AutomationToggle());
        }

        #region Events

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            ConfigMenu = new ConfigMenu(Helper.ModRegistry, ModManifest, () => Config, () =>
            {
                Config = new ModConfig();
                Helper.WriteConfig(Config);
            }, () =>
            {
                Helper.WriteConfig(Config);
                Config = Helper.ReadConfig<ModConfig>();
                ConfigMenu.OnConfigSavedCallback?.Invoke();
            }, Assistant.OnConfigSaved);

            ConfigMenu.RegisterModConfigMenu();
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            Assistant.GiveStarterFishingRod(Config.StartWithFishingRod);
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            Assistant.NumWarnThisDay = 0;
        }

        private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
        {
            Assistant.DoOnTimeChangedAssistantTask();
        }

        private void OnWarped(object? sender, WarpedEventArgs e)
        {
            Assistant.ActualTrashJunk();
        }

        private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
        {
            Assistant.AutoTrashJunk(e);
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            if (Game1.player.CurrentTool is FishingRod fishingRod)
                Assistant.OnEquipFishingRod(fishingRod, AutomationEnable);

            if (Game1.player.CurrentTool is not FishingRod)
                Assistant.OnUnEquipFishingRod();

            Assistant.DoOnUpdateAssistantTask();
        }

        private void OnRenderingHud(object? sender, RenderingHudEventArgs e)
        {
            DrawModStatus();
        }

        private void OnRenderMenu(object? sender, RenderedActiveMenuEventArgs e)
        {
            DrawFishPreview();
        }

        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is BobberBar bobberBar)
                Assistant.OnFishingMiniGameStart(bobberBar);
            else if (e.OldMenu is BobberBar)
                Assistant.OnFishingMiniGameEnd();

            if (e.NewMenu is ItemGrabMenu { source: ItemGrabMenu.source_fishingChest } itemGrabMenu)
                Assistant.OnTreasureMenuOpen(itemGrabMenu);
            else if (e.OldMenu is ItemGrabMenu { source: ItemGrabMenu.source_fishingChest })
                Assistant.OnTreasureMenuClose();

            if (e.NewMenu is GameMenu gameMenu)
                Assistant.OnGameMenuOpen(gameMenu);
            else if (e.OldMenu is GameMenu)
                Assistant.OnGameMenuClose();
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            if (e.Button == Config.EnableAutomationButton)
            {
                AutomationEnable = !AutomationEnable;
                Game1.playSound("coin");

                Assistant.OnAutomationStateChange(AutomationEnable);
            }

            if (e.Button == Config.CatchTreasureButton)
            {
                CatchTreasure = !CatchTreasure;
                Game1.playSound("dwop");
            }

            if (e.Button == Config.OpenConfigMenuButton && Config.OpenConfigMenuButton != SButton.None) ConfigMenu.OpenModMenu();
        }

        #endregion

        #region HUD

        private void DrawModStatus()
        {
            if ((Game1.eventUp && !Game1.isFestival()) || (!AutomationEnable && !CatchTreasure))
                return;

            float toolBarTransparency = 0;
            int toolBarWidth = 0;

            foreach (IClickableMenu? menu in Game1.onScreenMenus)
            {
                if (menu is not Toolbar toolBar) continue;

                toolBarTransparency = Helper.Reflection.GetField<float>(toolBar, "transparency").GetValue();
                toolBarWidth = toolBar.width / 2;

                break;
            }

            Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
            Point playerGlobalPos = Game1.player.GetBoundingBox().Center;
            Vector2 playerLocalVec = Game1.GlobalToLocal(Game1.viewport, new Vector2(playerGlobalPos.X, playerGlobalPos.Y));
            bool alignTop = !Game1.options.pinToolbarToggle && playerLocalVec.Y > viewport.Height / 2 + 64;

            const int boxSize = 96;
            const int iconSize = 20;
            const int screenMargin = 8;
            const int spacing = 2;

            int toolbarOffset = toolBarTransparency == 0 || Assistant.IsInFishingMiniGame || Game1.isFestival() ? 0 :
                Config.ModStatusPosition == HudPosition.Left.ToString() ? -toolBarWidth - spacing : toolBarWidth + spacing;

            int boxPosX = viewport.Width / 2 + toolbarOffset - boxSize / 2;
            int boxPosY = alignTop ? screenMargin : viewport.Height - screenMargin - boxSize;
            int boxCenterX = boxPosX + boxSize / 2;
            int boxCenterY = boxPosY + boxSize / 2;
            const int offset = boxSize / 4;

            Rectangle[] rectangles =
            {
                new(0, 256, 60, 60), //Box Texture
                new(20, 428, 10, 10), // Fishing Texture
                new(137, 412, 10, 11) // Treasure Texture
            };

            IClickableMenu.drawTextureBox(Game1.spriteBatch, Game1.menuTexture, rectangles[0], boxPosX, boxPosY, boxSize, boxSize, Color.White * toolBarTransparency, drawShadow: false);

            DrawIcon(AutomationEnable, rectangles[1], boxCenterX - iconSize / 2, boxPosY + offset, 2f);
            DrawIcon(CatchTreasure, rectangles[2], boxCenterX - iconSize / 2, boxPosY + boxSize - offset - iconSize, 2f);

            return;

            void DrawIcon(bool value, Rectangle source, int x, int y, float scale)
            {
                float iconTransparency = value ? 1 : 0.2f;
                ClickableTextureComponent icon = new(new Rectangle(x, y, iconSize, iconSize), Game1.mouseCursors, source, scale);
                icon.draw(Game1.spriteBatch, Color.White * toolBarTransparency * iconTransparency, 0);
            }
        }

        private void DrawFishPreview()
        {
            if (Game1.player == null || !Game1.player.IsLocalPlayer || !Config.DisplayFishPreview)
                return;

            if (Game1.activeClickableMenu is BobberBar bar && Assistant.IsInFishingMiniGame)
            {
                // stop drawing when bar play fadeOut animation
                if (bar.scale < 1) return;

                // check if fish has changed somehow. If yes, re-make the fish sprite and its display text.
                GetFishData(out bool showFish, out bool showText);

                // call a function to position and draw the box
                DrawFishDisplay(showFish, showText);
            }

            return;

            void GetFishData(out bool showFish, out bool showText)
            {
                ItemMetadata? metadata = ItemRegistry.GetMetadata(bar.whichFish);

                if (_fishPreviewId != bar.whichFish || _fishSprite == null)
                {
                    _fishPreviewId = bar.whichFish;
                    // save fish object to use in drawing // check for errors?
                    _fishSprite = new Object(bar.whichFish, 1);
                }

                // determine value of showFish value
                showFish = Config.ShowUncaughtFish || Assistant.AlreadyCaughtFish() || (Config.ShowLegendaryFish && bar.bossFish);

                // determine value of showText value
                showText = Config.ShowFishName;

                // determine text to show if true
                _textValue = showText && showFish ? metadata.GetParsedData().DisplayName : "???";

                // determine size of textValue
                _textSize = Game1.dialogueFont.MeasureString(_textValue) * TextScale;

                // determine width and height of display box
                _boxWidth = 128;
                _boxHeight = 90;

                if (showText && showFish) _boxWidth = Math.Max(_boxWidth, (int)_textSize.X + Margin * 2);

                _boxHeight += (int)_textSize.Y;
            }

            void DrawFishDisplay(bool showFish, bool showText)
            {
                // determine the x and y coords
                int x = (int)((bar.xPositionOnScreen + bar.width + 80) * (Game1.options.zoomLevel / Game1.options.uiScale));
                int y = (int)(bar.yPositionOnScreen * (Game1.options.zoomLevel / Game1.options.uiScale));

                const float spriteScale = 1f;
                const int spriteSize = (int)(64 * spriteScale);
                const int spriteCenter = spriteSize / 2;
                int boxCenterX = _boxWidth / 2;
                int boxCenterY = _boxHeight / 2;
                int textCenter = (int)(_textSize.X / 2);

                // draw box of height and width at location
                IClickableMenu.drawTextureBox(Game1.spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, _boxWidth, _boxHeight, Color.White, 1f, false);

                Vector2 drawFishPos;

                if (showText)
                {
                    //Calculate draw position for fish and draw at calculated position
                    drawFishPos = new Vector2(x + boxCenterX - spriteCenter, y + boxCenterY - spriteCenter - Margin);
                    _fishSprite?.drawInMenu(Game1.spriteBatch, drawFishPos, spriteScale, 1.0f, 1.0f, StackDrawType.Hide, showFish ? Color.White : Color.Black * 0.8f, false);

                    // if showText, center the text x below the fish
                    Game1.spriteBatch.DrawString(Game1.dialogueFont, _textValue, new Vector2(x + boxCenterX - textCenter, drawFishPos.Y + spriteSize), Color.Black, 0f, Vector2.Zero, TextScale,
                        SpriteEffects.None, 0f);
                }
                else
                {
                    //Calculate draw position for fish and draw at calculated position
                    drawFishPos = new Vector2(x + boxCenterX - spriteCenter, y + boxCenterY - spriteCenter - Margin / 2);
                    _fishSprite?.drawInMenu(Game1.spriteBatch, drawFishPos, spriteScale, 1.0f, 1.0f, StackDrawType.Hide, showFish ? Color.White : Color.Black * 0.8f, false);
                }

                // if show treasure draw treasure with fish icon
                if (Config.ShowTreasure && bar is { treasure: true, treasureCaught: false })
                {
                    _treasureSprite ??= new Object("693", 1);

                    _treasureSprite.drawInMenu(Game1.spriteBatch, new Vector2(drawFishPos.X + TreasureOffsetX, drawFishPos.Y + TreasureOffsetY), 0.75f, 1.0f, 1.0f, StackDrawType.Hide, Color.White,
                        false);
                }
            }
        }

        #endregion
    }
}

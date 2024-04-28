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

            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
            helper.Events.Multiplayer.PeerConnected += OnPeerConnected;

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

        private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
        {
            foreach (IMultiplayerPeer peer in Helper.Multiplayer.GetConnectedPlayers())
            {
                if (peer is { HasSmapi: true, IsHost: true })
                    Helper.Multiplayer.SendMessage("Test Hello World", "Notify", [ModManifest.UniqueID], [peer.PlayerID]);
            }
        }

        private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == ModManifest.UniqueID && e.Type == "Notify")
            {
                string message = e.ReadAs<string>();
                Monitor.Log($"message: {message}");
            }
        }

        public void ForceDisable()
        {
            Game1.playSound("coin");
            AutomationEnable = false;

            CommonHelper.PushToggle(AutomationEnable, I18n.HudMessage_AutomationToggle());
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
            }, Assistant.OnFieldChanged);

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

            CommonHelper.DrawTextureBox(boxPosX, boxPosY, boxSize, Color.White * toolBarTransparency);
            DrawToggleIcon(AutomationEnable, TextureSource.Fishing, boxCenterX - iconSize / 2, boxPosY + offset);
            DrawToggleIcon(CatchTreasure, TextureSource.Treasure, boxCenterX - iconSize / 2, boxPosY + boxSize - offset - iconSize);

            return;

            void DrawToggleIcon(bool value, Rectangle source, int x, int y, float scale = 2.0f)
            {
                CommonHelper.DrawToggleIcon(value, source, x, y, iconSize, scale);
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
                CommonHelper.DrawTextureBox(x, y, _boxWidth, _boxHeight, Color.White, 1f, false);

                Vector2 drawFishPos;

                if (showText)
                {
                    //Calculate draw position for fish and draw at calculated position
                    drawFishPos = new Vector2(x + boxCenterX - spriteCenter, y + boxCenterY - spriteCenter - Margin);
                    CommonHelper.DrawSprite(_fishSprite, drawFishPos, spriteScale, showFish ? Color.White : Color.Black * 0.8f);

                    // if showText, center the text x below the fish
                    Vector2 textPos = new(x + boxCenterX - textCenter, drawFishPos.Y + spriteSize);
                    CommonHelper.DrawString(_textValue, textPos, TextScale);
                }
                else
                {
                    //Calculate draw position for fish and draw at calculated position
                    drawFishPos = new Vector2(x + boxCenterX - spriteCenter, y + boxCenterY - spriteCenter - Margin / 2);
                    CommonHelper.DrawSprite(_fishSprite, drawFishPos, spriteScale, showFish ? Color.White : Color.Black * 0.8f);
                }

                // if show treasure draw treasure with fish icon
                if (Config.ShowTreasure && bar is { treasure: true, treasureCaught: false })
                {
                    _treasureSprite ??= new Object("693", 1);

                    Vector2 drawTreasurePos = new(drawFishPos.X + TreasureOffsetX, drawFishPos.Y + TreasureOffsetY);
                    CommonHelper.DrawSprite(_treasureSprite, drawTreasurePos, 0.75f);
                }
            }
        }

        #endregion
    }
}

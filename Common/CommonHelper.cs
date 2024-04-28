using FishingAssistant2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace ChibiKyu.StardewMods.Common
{
    public static class CommonHelper
    {
        private static void PushNotification(int whatType, Item? item, string key, params object[] args)
        {
            if (!Context.IsWorldReady) return;

            HUDMessage hudMessage = new(string.Format(key, args), whatType) { messageSubject = item, timeLeft = 3300 };
            Game1.addHUDMessage(hudMessage);
        }

        private static void PushNotification(int whatType, string key, params object[] args)
        {
            PushNotification(whatType, null, key, args);
        }

        internal static void PushWarning(string key, params object[] args)
        {
            PushNotification(HUDMessage.newQuest_type, key, args);
        }

        internal static void PushWarning(Item item, string key, params object[] args)
        {
            PushNotification(HUDMessage.newQuest_type, item, key, args);
        }

        internal static void PushError(string key, params object[] args)
        {
            PushNotification(HUDMessage.error_type, key, args);
        }

        internal static void PushError(Item item, string key, params object[] args)
        {
            PushNotification(HUDMessage.newQuest_type, item, key, args);
        }

        internal static void PushToggle(bool toggleValue, string message)
        {
            string status = toggleValue ? I18n.On() : I18n.Off();
            PushWarning(message, status);
        }

        internal static void DrawString(string? text, Vector2 drawPos, float scale)
        {
            Game1.spriteBatch.DrawString(Game1.dialogueFont, text, drawPos, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        internal static void DrawSprite(Object? obj, Vector2 drawPos, float scale)
        {
            DrawSprite(obj, drawPos, scale, Color.White);
        }

        internal static void DrawSprite(Object? obj, Vector2 drawPos, float scale, Color color)
        {
            obj?.drawInMenu(Game1.spriteBatch, drawPos, scale, 1.0f, 1.0f, StackDrawType.Hide, color, false);
        }

        internal static void DrawToggleIcon(bool value,
            Rectangle source,
            int x,
            int y,
            int size = 20,
            float scale = 1.0f,
            float trueTrans = 1.0f,
            float falseTrans = 0.2f)
        {
            ClickableTextureComponent icon = new(new Rectangle(x, y, size, size), Game1.mouseCursors, source, scale);
            float transparency = value ? trueTrans : falseTrans;
            icon.draw(Game1.spriteBatch, Color.White * transparency, 0);
        }

        internal static void DrawTextureBox(int x, int y, int size, Color color, bool drawShadow = false)
        {
            DrawTextureBox(x, y, size, size, color, drawShadow: drawShadow);
        }

        internal static void DrawTextureBox(int x,
            int y,
            int width,
            int height,
            Color color,
            float scale = 1f,
            bool drawShadow = false)
        {
            IClickableMenu.drawTextureBox(Game1.spriteBatch, Game1.menuTexture, TextureSource.TextureBox, x, y, width, height, color, scale, drawShadow);
        }
    }
}

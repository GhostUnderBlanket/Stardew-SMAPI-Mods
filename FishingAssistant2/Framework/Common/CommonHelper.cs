using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace FishingAssistant2.Framework.Common
{
    /// This code is copied from CJBok/SDV-Mods repository
    /// available under the MIT License. See that repository for the latest version.
    /// https://github.com/CJBok/SDV-Mods

    /// available under the MIT License. See that repository for the latest version.
    /// <summary>Provides common helpers for the mods.</summary>
    internal static class CommonHelper
    {
        /*********
         ** Accessors
         *********/
        /// <summary>The width of the borders drawn by <c>DrawTab</c>.</summary>
        public const int ButtonBorderWidth = 4 * Game1.pixelZoom;

        /// <summary>Draw a button texture to the screen.</summary>
        /// <param name="x">The X position at which to draw.</param>
        /// <param name="y">The Y position at which to draw.</param>
        /// <param name="innerWidth">The width of the button's inner content.</param>
        /// <param name="innerHeight">The height of the button's inner content.</param>
        /// <param name="innerDrawPosition">The position at which the content should be drawn.</param>
        /// <param name="align">
        ///     The button's horizontal alignment relative to <paramref name="x" />. The possible values are 0
        ///     (left), 1 (center), or 2 (right).
        /// </param>
        /// <param name="alpha">The button opacity, as a value from 0 (transparent) to 1 (opaque).</param>
        /// <param name="drawShadow">Whether to draw a shadow under the tab.</param>
        public static void DrawTab(int x, int y, int innerWidth, int innerHeight, out Vector2 innerDrawPosition,
            int align = 0, float alpha = 1, bool drawShadow = true)
        {
            var spriteBatch = Game1.spriteBatch;

            // calculate outer coordinates
            var outerWidth = innerWidth + ButtonBorderWidth * 2;
            var outerHeight = innerHeight + Game1.tileSize / 3;
            var offsetX = align switch
            {
                1 => -outerWidth / 2,
                2 => -outerWidth,
                _ => 0
            };

            // draw texture
            IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x + offsetX, y,
                outerWidth, outerHeight + Game1.tileSize / 16, Color.White * alpha, drawShadow: drawShadow);
            innerDrawPosition = new Vector2(x + ButtonBorderWidth + offsetX, y + ButtonBorderWidth);
        }

        /****
         ** Math helpers
         ****/
        /// <summary>
        ///     Get a value's fractional position within a range, as a value between 0 (<paramref name="minValue" />) and 1 (
        ///     <paramref name="maxValue" />).
        /// </summary>
        /// <param name="value">The value within the range.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        public static float GetRangePosition(int value, int minValue, int maxValue)
        {
            var position = (value - minValue) / (float)(maxValue - minValue);
            return MathHelper.Clamp(position, 0, 1);
        }

        /// <summary>Get the value from a position within a range.</summary>
        /// <param name="position">The position within the range, where 0 is the minimum value and 1 is the maximum value.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        public static int GetValueAtPosition(float position, int minValue, int maxValue)
        {
            var value = position * (maxValue - minValue) + minValue;
            return (int)MathHelper.Clamp(value, minValue, maxValue);
        }
    }
}
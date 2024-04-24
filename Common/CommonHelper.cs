using FishingAssistant2;
using StardewModdingAPI;
using StardewValley;

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
    }
}

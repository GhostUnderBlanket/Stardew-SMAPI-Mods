using ChibiKyu.StardewMods.FishingAssistant2.Frameworks;
using FishingAssistant2;
using StardewModdingAPI;
using StardewValley;

namespace ChibiKyu.StardewMods.Common
{
    public static class CommonHelper
    {
        internal static void PushNotification(int whatType, string key, params object[] args)
        {
            if (!Context.IsWorldReady) return;
            Game1.addHUDMessage(new HUDMessage(string.Format(key, args), whatType));
        }
        
        internal static void PushWarnNotification(string key, params object[] args)
        {
            PushNotification(HUDMessage.newQuest_type, key, args);
        }
        
        internal static void PushErrorNotification(string key, params object[] args)
        {
            PushNotification(HUDMessage.error_type, key, args);
        }
        
        internal static void PushToggleNotification(bool toggleValue, string message)
        {
            string status = toggleValue ? I18n.On() : I18n.Off();
            PushWarnNotification(message, status);
        }
    }
}
using Android.App;
using Android.Content;
using Android.OS;
using EHealth.Services;
using Java.Lang;
using AContext = Android.Content.Context;
using AndroidApplication = Android.App.Application;

[assembly: Dependency(typeof(EHealthApp.Platforms.Android.LocalNotificationService))]
namespace EHealthApp.Platforms.Android
{
    public class LocalNotificationService : ILocalNotificationService
    {
        public void ScheduleNotification(DateTime notifyTime, string title, string message)
        {
            AContext context = AndroidApplication.Context;
            var alarmManager = (AlarmManager)context.GetSystemService(AContext.AlarmService);

            var intent = new Intent(context, Class.FromType(typeof(NotificationReceiver)));
            intent.PutExtra("title", title ?? string.Empty);
            intent.PutExtra("message", message ?? string.Empty);

            int requestCode = (int)((notifyTime.Ticks ^ (title?.GetHashCode() ?? 0)) & 0xFFFFFFF);

            var pendingIntent = PendingIntent.GetBroadcast(
                context,
                requestCode,
                intent,
                PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
            );

            long triggerAtMillis = (long)(notifyTime.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;

            System.Diagnostics.Debug.WriteLine(
                $"[LocalNotificationService] Programare notificare: {title} - {message} la {notifyTime} (UTC ms: {triggerAtMillis})"
            );

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
            }
            else
            {
                alarmManager.SetExact(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
            }
        }

        public void CancelNotification(DateTime notifyTime, string title)
        {
            AContext context = AndroidApplication.Context;
            var alarmManager = (AlarmManager)context.GetSystemService(AContext.AlarmService);

            int requestCode = (int)((notifyTime.Ticks ^ (title?.GetHashCode() ?? 0)) & 0xFFFFFFF);

            var intent = new Intent(context, Class.FromType(typeof(NotificationReceiver)));

            var pendingIntent = PendingIntent.GetBroadcast(
                context,
                requestCode,
                intent,
                PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
            );

            alarmManager.Cancel(pendingIntent);
            pendingIntent.Cancel();

            System.Diagnostics.Debug.WriteLine(
                $"[LocalNotificationService] Anulare notificare: {title} la {notifyTime}"
            );
        }
    }
}

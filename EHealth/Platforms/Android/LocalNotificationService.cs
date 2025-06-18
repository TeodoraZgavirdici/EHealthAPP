using Android.App;
using Android.Content;
using Android.OS;
using EHealth.Services;
using Java.Lang;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(EHealthApp.Platforms.Android.LocalNotificationService))]
namespace EHealthApp.Platforms.Android
{
    public class LocalNotificationService : ILocalNotificationService
    {
        public void ScheduleNotification(DateTime notifyTime, string title, string message)
        {
            Context context = global::Android.App.Application.Context;

            AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);

            Intent intent = new Intent(context, Class.FromType(typeof(NotificationReceiver)));
            intent.PutExtra("title", title);
            intent.PutExtra("message", message);

            int requestCode = (int)(notifyTime.Ticks & 0xFFFFFFF);

            PendingIntent pendingIntent = PendingIntent.GetBroadcast(
                context,
                requestCode,
                intent,
                PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent);

            long triggerAtMillis = (long)(notifyTime.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
            }
            else
            {
                alarmManager.SetExact(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
            }
        }
    }
}

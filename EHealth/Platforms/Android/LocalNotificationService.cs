using Android.App;
using Android.Content;
using Android.OS;
using EHealthApp.Services;
using EHealthApp.Platforms.Android;
using System;

[assembly: Microsoft.Maui.Controls.Dependency(typeof(LocalNotificationService))]
namespace EHealthApp.Services
{
    public class LocalNotificationService : ILocalNotificationService
    {
        const string ChannelId = "default_channel";

        public void ScheduleNotification(DateTime notifyTime, string title, string message)
        {
            var context = Android.App.Application.Context;

            CreateNotificationChannel();

            Intent intent = new Intent(context, typeof(NotificationReceiver));
            intent.PutExtra("title", title);
            intent.PutExtra("message", message);

            int requestCode = (int)(notifyTime.Ticks & 0xFFFFFFF); // id unic bazat pe data

            PendingIntent pendingIntent = PendingIntent.GetBroadcast(context, requestCode, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            var alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);

            long triggerAtMillis = (long)(notifyTime.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
            }
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                alarmManager.SetExact(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
            }
            else
            {
                alarmManager.Set(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
            }
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var context = Android.App.Application.Context;
                var channel = new NotificationChannel(ChannelId, "Default Channel", NotificationImportance.Default)
                {
                    Description = "Default notification channel"
                };
                var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
        }
    }
}

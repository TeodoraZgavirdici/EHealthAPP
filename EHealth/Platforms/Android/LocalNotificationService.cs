using Android.App;
using Android.Content;
using AndroidX.Core.App;
using EHealth.Services;
using EHealth.Platforms.Android;
using Java.Lang; // pentru Class
using System;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(LocalNotificationService))]
namespace EHealth.Platforms.Android
{
    public class LocalNotificationService : ILocalNotificationService
    {
        public void ScheduleNotification(DateTime notifyTime, string title, string message)
        {
            var context = Android.App.Application.Context;

            var intent = new Intent(context, Class.FromType(typeof(MainActivity)));
            var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);

            var notificationBuilder = new NotificationCompat.Builder(context, "default")
                .SetContentTitle(title)
                .SetContentText(message)
                .SetSmallIcon(Resource.Drawable.appicon) // schimbă cu icon-ul tău
                .SetContentIntent(pendingIntent)
                .SetAutoCancel(true)
                .SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis());

            var notificationManager = NotificationManagerCompat.From(context);

            // Pentru demo trimitem imediat notificarea
            notificationManager.Notify(new Random().Next(), notificationBuilder.Build());

            // Dacă vrei notificări programate la notifyTime, trebuie să folosești AlarmManager
        }
    }
}

using Android.App;
using Android;
using Android.Content;
using AndroidX.Core.App;

namespace EHealthApp.Platforms.Android
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    public class NotificationReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            string title = intent.GetStringExtra("title");
            string message = intent.GetStringExtra("message");

            var builder = new NotificationCompat.Builder(context, "default_channel")
                .SetContentTitle(title)
                .SetContentText(message)
                .SetSmallIcon(global::Android.Resource.Drawable.SymDefAppIcon)
                .SetAutoCancel(true);

            var notificationManager = NotificationManagerCompat.From(context);
            notificationManager.Notify(new System.Random().Next(), builder.Build());
        }
    }
}

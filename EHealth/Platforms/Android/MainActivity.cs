using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;

namespace EHealth
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                               ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        const int RequestNotificationId = 1000;
        const string ChannelId = "default_channel";
        const string ChannelName = "Default Channel";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            CreateNotificationChannel();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu) 
            {
                if (CheckSelfPermission(Android.Manifest.Permission.PostNotifications) != Android.Content.PM.Permission.Granted)
                {
                    RequestPermissions(new[] { Android.Manifest.Permission.PostNotifications }, RequestNotificationId);
                }
            }
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O) 
            {
                var channel = new NotificationChannel(ChannelId, ChannelName, NotificationImportance.Default)
                {
                    Description = "Channel for local notifications"
                };
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
        }
    }
}

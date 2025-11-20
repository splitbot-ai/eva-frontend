using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using Firebase;
using EvaMobil.Platforms.Android;
using Microsoft.AspNetCore.Components;
using Plugin.Firebase.CloudMessaging;

namespace EvaMobil
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode, ScreenOrientation = ScreenOrientation.Portrait,
        WindowSoftInputMode = SoftInput.AdjustResize)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window?.SetSoftInputMode(SoftInput.AdjustResize);
            GlobalLayoutUtil.AssistActivity(this);

            if (FirebaseApp.GetApps(this).Count == 0)
                FirebaseApp.InitializeApp(this);


            HandleIntent(Intent);
            CreateNotificationChannelIfNedded();

        }

        protected override void OnNewIntent(Intent intent)
		{
			base.OnNewIntent(intent);
            HandleIntent(intent);

		}

        private static void HandleIntent(Intent intent)
        {
            FirebaseCloudMessagingImplementation.OnNewIntent(intent);
            
        }

        private void CreateNotificationChannelIfNedded()
        {
            if(Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                CreateNotificationChannel();
            }
        }

        private void CreateNotificationChannel()
        {
            var channelId = $"{PackageName}.general";
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            var channel = new NotificationChannel(channelId, "General", NotificationImportance.Default);
            notificationManager.CreateNotificationChannel(channel);
            FirebaseCloudMessagingImplementation.ChannelId = channelId;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaComponentLibrary.Services;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls.PlatformConfiguration;

#if ANDROID
using Android.App;
using Android.OS;
using AndroidX.Core.App;
using Android.Content;
using Java.IO;
#endif
#if IOS
using Foundation;
using System;
using System.IO;
using UIKit;
using Microsoft.Maui.ApplicationModel;
using CoreGraphics;




#endif




namespace EvaMobil.Services
{
    public class DownloadPDF : IDownloadPDF
    {
        public async Task<bool> Save(byte[] pdfByte, string fileName)
        {
            try
            {
				//string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
				//await System.IO.File.WriteAllBytesAsync(filePath, pdfByte);
				//await File.WriteAllBytesAsync(filePath, pdfByte);

#if ANDROID
                string downloadsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
                string filePath = Path.Combine(downloadsPath, fileName);
                await System.IO.File.WriteAllBytesAsync(filePath, pdfByte);
                var context = Android.App.Application.Context;
                var uri = Android.Net.Uri.FromFile(new Java.IO.File(filePath));
                var scanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                scanIntent.SetData(uri);
                context.SendBroadcast(scanIntent);
                ShowSimpleNotification($"{fileName} Downloaded");
#elif IOS
				// Get file path in Documents folder
				string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

				// Write the byte array (PDF) to the file
				await File.WriteAllBytesAsync(filePath, pdfByte);

				// Create an NSURL for the file
				var fileUrl = NSUrl.FromFilename(filePath);

				// Create the activity controller to share the file
				var activityController = new UIActivityViewController(new NSObject[] { fileUrl }, null);

				// Ensure proper popover configuration for iPad
				if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
				{
					// Get the top view controller and ensure the source view is valid
					var topViewController = GetTopViewController(UIApplication.SharedApplication.KeyWindow.RootViewController);
					var sourceView = topViewController?.View; // Get the source view (could be a button or any other view)

					if (sourceView != null)
					{
						// Ensure the bounds are valid
						
							// If invalid bounds, set a default position
							activityController.PopoverPresentationController.SourceView = sourceView;
							activityController.PopoverPresentationController.SourceRect = 
							new CGRect(0, 0, 1, 1); // Default small rect
						
					}
					else
					{
						// Fallback if sourceView is not valid
						//("SourceView is invalid.");
					}
				}

				// Present the activity controller
				var rootViewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
				var topController = GetTopViewController(rootViewController);
				topController?.PresentViewController(activityController, true, null);
#endif
				return true;
            }
            catch(Exception e)
            {
                System.Console.WriteLine(e.ToString());
                return false;
            }
        }

#if IOS

		private UIViewController GetTopViewController(UIViewController rootViewController)
		{
			if (rootViewController is UINavigationController navController)
			{
				return GetTopViewController(navController.VisibleViewController);
			}

			if (rootViewController.PresentedViewController != null)
			{
				return GetTopViewController(rootViewController.PresentedViewController);
			}

			return rootViewController;
		}

#endif

#if ANDROID
        public void ShowSimpleNotification(string fileName)
        {
            var context = Android.App.Application.Context;
            var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            string channelId = "download_channel";

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                var Appcontext = Android.App.Application.Context;
                if (Appcontext.CheckSelfPermission(Android.Manifest.Permission.PostNotifications) != Android.Content.PM.Permission.Granted)
                {
                    var activity = Platform.CurrentActivity;
                    activity.RequestPermissions(new string[] { Android.Manifest.Permission.PostNotifications }, 0);
                }
            }
      
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(channelId, "Downloads", NotificationImportance.Default);
                notificationManager.CreateNotificationChannel(channel);
            }

            var notification = new NotificationCompat.Builder(context, channelId)
                .SetContentTitle(fileName)  
                .SetSmallIcon(Android.Resource.Drawable.StatSysDownloadDone) 
                .SetAutoCancel(true)
                .Build();

            notificationManager.Notify(1, notification); 
        }
#endif
	}
}

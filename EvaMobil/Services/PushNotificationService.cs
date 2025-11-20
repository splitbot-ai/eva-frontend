using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaComponentLibrary.Services;

using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.CloudMessaging.EventArgs;
using System.Diagnostics;



#if ANDROID
using Firebase.Messaging;
#elif IOS
using Firebase.CloudMessaging;
#endif


namespace EvaMobil.Services
{
	public class PushNotificationService : IPushNotificationService
	{
		public async Task DeleteDeviceTokenAsync()
		{
#if ANDROID
			FirebaseMessaging.Instance.DeleteToken();

#elif IOS
            Messaging.SharedInstance.DeleteToken((_) => { });
#endif

		}

		public async Task<string> GetDeviceTokenAsync()
		{
			try
			{
				var canConnect = await VerifyPermissionGrantedAsync();
				if (!canConnect) return string.Empty;
#if ANDROID
				if(OperatingSystem.IsAndroidVersionAtLeast(33))
				{
					PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
					if (status.Equals(PermissionStatus.Denied))
						await Permissions.RequestAsync<Permissions.PostNotifications>();
				}
#endif

				var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
				return token;

			} catch 
			{
				return string.Empty;
			}
			return string.Empty;
		}

		public async Task<bool> VerifyPermissionGrantedAsync()
		{
			if (!CrossFirebaseCloudMessaging.IsSupported) return false; 

			try
			{
				await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();				
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return false;
			}
			return true;

		}
	}
}

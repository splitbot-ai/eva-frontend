using EvaComponentLibrary.Services;

namespace EvaWASM.Services
{
	public class PushNotificationService : IPushNotificationService
	{
		public async Task DeleteDeviceTokenAsync()
		{
			await Task.CompletedTask;
		}

		public async Task<string> GetDeviceTokenAsync()
		{
			await Task.CompletedTask;
			return string.Empty;
		}

		public async Task<bool> VerifyPermissionGrantedAsync()
		{
			await Task.CompletedTask;
			return false;
		}
	}
}

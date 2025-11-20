namespace EvaComponentLibrary.Services
{
	public interface IPushNotificationService
	{
		public Task<bool> VerifyPermissionGrantedAsync();
		public Task<string> GetDeviceTokenAsync();
		public Task DeleteDeviceTokenAsync();
	}
}

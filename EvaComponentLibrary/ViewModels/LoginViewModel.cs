using EvaComponentLibrary.Models;
using EvaComponentLibrary.Services;

namespace EvaComponentLibrary.ViewModels
{
	public class LoginViewModel
	{

		public  Registration registrationData = new Registration();


		public Action<string>? Registration;
		public Action<bool>? Validation;
		public Action<bool>? Confirmation;
		public readonly WebServices _ws;

		public LoginViewModel(WebServices service)
		{
			_ws = service ?? throw new ArgumentNullException(nameof(service));
		}

		public void Init()
		{

			_ws.Init();
        }

		public async Task LoginAsync(string mail, string passwort)
		{


			LoginData loginData = new LoginData()
			{
				UserName = mail,
				Password = passwort
			};
			bool succ = await _ws.LoginAsync(loginData);
			
			Validation?.Invoke(succ);
		}

		public async Task<bool> RefreshTokenAsync(string token)
		{
			return await _ws.RefreshTokenAsync(token);
		}

		public async Task PostNewUserAsync(Registration data)
		{
			string isSucc = await _ws.PostNewUserAsync(data);
			Registration?.Invoke(isSucc);
		}


		public void SetNetwork(string authserver, string realm, string host )
		{
			try
			{
                if (string.IsNullOrEmpty(authserver) || string.IsNullOrEmpty(realm) || string.IsNullOrEmpty(host))
                {
                    return;
                }
                _ws.Auth = authserver;
				_ws.Host = host;
				_ws.Realm = realm;
				////("Auth   " + _ws.Auth);
				////("host   " + _ws.Host);
				////("realm  " + _ws.Realm);
				Confirmation?.Invoke(true);
			}
			catch (Exception ex) { 

				Confirmation?.Invoke(false);

			}

		}


	}
}

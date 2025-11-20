#if IOS
using EvaMobil.Services;
using EvaMobil.Platforms.iOS;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
#endif
using EvaComponentLibrary.Services;
using EvaMobil.Services;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;
using Microsoft.AspNetCore.Components.WebView;
using Plugin.Firebase.CloudMessaging;



namespace EvaMobil
{
    public partial class MainPage : ContentPage
    {

        private double _startHeight, _startWidth, _startBl = -1;

        private IKeyboardServices _keyboardService;
        private ISafeAreaService _safeAreaService;
        private ContentPage _contentPage;
        public ContentPage ContentPage { get { return _contentPage; } }
	
        private async void GetToken()
        {
#if ANDROID
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
                if (status.Equals(PermissionStatus.Denied))
                    await Permissions.RequestAsync<Permissions.PostNotifications>();
            }
#endif
#if IOS || ANDROID
            await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
			var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();

			await Share.RequestAsync(new ShareTextRequest
			{
				Text = token,
				Title = "Share Fírebase"
			});
           //($"FCM token: {token}");
#endif
        }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            //ContentPageMain.BackgroundColor = Color.FromArgb("#FFFFFF00");

            Application.Current.RequestedThemeChanged += (sender, args) => Console.WriteLine($"Theme chagned: {Application.Current.RequestedTheme}");

#if IOS
            _keyboardService = new iOSKeyboadServices();
            _safeAreaService = new SafeAreaService();
            InitiOSKeyboardHandling();
			

			DeviceDisplay.Current.MainDisplayInfoChanged += (sender, args) =>
            {
               //("Rotation " + DeviceDisplay.MainDisplayInfo.Height);
                _startHeight = (DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density) 
                - (_safeAreaService.GetSafeAreaInsets().Bottom + _safeAreaService.GetSafeAreaInsets().Top);

                _startWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;

                //iOSContainer.WidthRequest = _startWidth;
                //iOSContainer.HeightRequest = _startHeight;

                blazorWebView.WidthRequest = _startWidth;
                blazorWebView.HeightRequest = _startHeight;
            };
			blazorWebView.UrlLoading +=
				(sender, urlLoadingEventArgs) =>
				{
					if (urlLoadingEventArgs.Url.Host != "127.0.0.1"
					&& urlLoadingEventArgs.Url.Host != "localhost")
					{
						_ = Browser.Default.OpenAsync(urlLoadingEventArgs.Url, BrowserLaunchMode.External);
					}
				};


#endif
		}


#if IOS
        protected override async void OnAppearing()
        {
            base.OnAppearing();
           
            

           //(this.Height + " Height before ");
            _startHeight = (DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density);
            
            _startWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;

            //_contentPage = ContentPageMain;

            //await Task.Delay(100);

            //iOSContainer.VerticalOptions = LayoutOptions.Start;
            //iOSContainer.HorizontalOptions = LayoutOptions.Start;

            //iOSContainer.WidthRequest = _startWidth;
            //iOSContainer.HeightRequest = _startHeight;


            blazorWebView.HeightRequest = _startHeight;
            blazorWebView.WidthRequest = _startWidth;

            blazorWebView.VerticalOptions = LayoutOptions.Start;
            blazorWebView.HorizontalOptions = LayoutOptions.Start;
            ContentPageMain.ForceLayout();


            Microsoft.Maui.Platform.KeyboardAutoManagerScroll.Disconnect();
            //BottomBoxView();
            _keyboardService?.RegisterForKeyboardNotification();
            await Task.Delay(1);
            OnKeyboardHeightChanged(this, 0);
           
        }


        private void InitiOSKeyboardHandling()
        {
            if (_keyboardService != null)
                _keyboardService.KeyboardHeightChanged += OnKeyboardHeightChanged;
        }




        private void OnKeyboardHeightChanged(object sender, float keyboardHeight)
        {
           //($"Keyboard Height is: {keyboardHeight}");
            double newHeight = _startHeight - keyboardHeight - (_safeAreaService.GetSafeAreaInsets().Top 
                + (keyboardHeight < 1 ? _safeAreaService.GetSafeAreaInsets().Bottom : 0));

            blazorWebView.HeightRequest = newHeight;
            ContentPageMain.HeightRequest = newHeight;

            Window.MinimumHeight = newHeight;
            //iOSContainer.HeightRequest = newHeight;

            ContentPageMain.ForceLayout();

           //($"New Height: {newHeight}");

        }
#endif
        public void SafeTokenInStorage(string key, string value)
        {
            Preferences.Set(key, value);
        }

        public string RetreaveToken(string key)
        {
            return Preferences.Get(key, string.Empty);
        }

        public bool HasToken(string key)
        {
            return Preferences.ContainsKey(key);
        }

        public void ChangeBoxViewColor(bool isDarkMode)
        {
           
            //if (isDarkMode)
            //{
            //    navBar.Color = Color.FromArgb("#444444");
            //}
            //else
            //{
            //    navBar.Color = Color.FromArgb("#CCCCCC");
            //}
        }

    }
}

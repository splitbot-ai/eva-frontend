using EvaComponentLibrary.Services;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Plugin.Firebase.CloudMessaging;
using Application = Microsoft.Maui.Controls.Application;

namespace EvaMobil
{
    public partial class App : Application
    {
        private Microsoft.Maui.ApplicationModel.AppTheme currentTheme;
        private readonly IThemeService _themeService;
        public App(IThemeService themeService)
        {

            InitializeComponent();



            MainPage = new MainPage();
            //GetToken();
			_themeService = themeService;
#if !IOS
            Current?.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>()
                .UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
#endif
        }
		

        protected override void OnStart()
        {
            base.OnStart();

            
            _themeService.ThemeChanged += OnThemeChange;

            Application.Current.RequestedThemeChanged += (s, a) =>
            {
                currentTheme = Application.Current.RequestedTheme;
                _themeService.NotifyThemeChanged();
            };
        }

        private void OnThemeChange()
        {
            _themeService.ApplyThemeColor(
                (EvaComponentLibrary.Services.AppTheme)currentTheme);
        }
    }
}

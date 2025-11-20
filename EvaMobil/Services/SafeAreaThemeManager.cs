using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaComponentLibrary.Services;

using Microsoft.Maui.ApplicationModel;
#if ANDROID
using Android.Views;
#endif




#if ANDROID
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
#endif


namespace EvaMobil.Services
{
    public class SafeAreaThemeManager :  IThemeService
    {
        public event Action ThemeChanged;
        private string _defaultTheme = string.Empty;

        public void NotifyThemeChanged()
        {
            ThemeChanged?.Invoke();
        }

        public void ApplyThemeColor(string theme)
        {
			_defaultTheme = theme;

            bool isDarkMode;

            if (theme.Equals("System") || string.IsNullOrEmpty(theme))
            {
                isDarkMode = AppInfo.RequestedTheme ==  Microsoft.Maui.ApplicationModel.AppTheme.Dark;
            }
            else if (theme.Equals("dark"))
            {
                isDarkMode = true;
            }
            else
            {
                isDarkMode = false;
            }



#if ANDROID
            Android.Graphics.Color StatusBar = isDarkMode ? Android.Graphics.Color.Rgb(34, 34, 34) : Android.Graphics.Color.Rgb(238, 238, 238);
            Android.Graphics.Color NavigationBar = isDarkMode ? Android.Graphics.Color.Rgb(68, 68, 68) : Android.Graphics.Color.Rgb(204, 204, 204);

            Application.Current.MainPage.BackgroundColor = isDarkMode ? Color.FromArgb("#222222") : Color.FromArgb("#EEEEEE");

            Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.Window.SetStatusBarColor(StatusBar);
            Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.Window.SetNavigationBarColor(NavigationBar);
            Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.Window.DecorView.SystemUiFlags = isDarkMode ? 0 : SystemUiFlags.LightStatusBar;
#elif IOS

            if (isDarkMode)
            {
                Application.Current.MainPage.BackgroundColor = Color.FromArgb("#222222");

            }
            else
            {
				Application.Current.MainPage.BackgroundColor = Color.FromArgb("#EEEEEE");
            }

            var mainpage = Application.Current.MainPage as MainPage;
            mainpage?.ChangeBoxViewColor(isDarkMode);
#endif

        }

		public void ApplyThemeColor(EvaComponentLibrary.Services.AppTheme? theme)
		{
            if (_defaultTheme.Equals("System"))
                ApplyThemeColor(theme.ToString().ToLower());
		}
	}
}

using Foundation;

namespace EvaMobil
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp()
        {
            Microsoft.Maui.Platform.KeyboardAutoManagerScroll.Disconnect();
            return MauiProgram.CreateMauiApp();
        }
    }
}

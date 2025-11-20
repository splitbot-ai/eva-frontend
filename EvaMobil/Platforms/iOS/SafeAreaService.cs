
using EvaMobil.Services;
using UIKit;

namespace EvaMobil.Platforms.iOS
{
    public class SafeAreaService : ISafeAreaService
    {
        public Thickness GetSafeAreaInsets()
        {
            var insets = UIApplication.SharedApplication.KeyWindow.SafeAreaInsets;
            return new Thickness(insets.Left, insets.Top, insets.Right, insets.Bottom);
        }
    }
}

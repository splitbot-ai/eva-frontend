using Foundation;
using EvaMobil.Platforms.iOS;
using EvaMobil.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;


[assembly: Dependency(typeof(iOSKeyboadServices))]
namespace EvaMobil.Platforms.iOS
{
    public class iOSKeyboadServices : IKeyboardServices
    {
        public event EventHandler<float> KeyboardHeightChanged;


        public void RegisterForKeyboardNotification()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyboardIsShowing);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyboardIsHidding);
        }

        public void UnregisterForKeyboardNotification()
        {
        }
        private void KeyboardIsShowing(NSNotification not)
        {
            var keyboardFrame = UIKeyboard.FrameEndFromNotification(not);
            KeyboardHeightChanged?.Invoke(this, (float)keyboardFrame.Height);
           //("Keyboard Showing");
        }

        private void KeyboardIsHidding(NSNotification not)
        {
            KeyboardHeightChanged?.Invoke(this, 0f);
           //("Keyboard Hiding");
        }
    }
}

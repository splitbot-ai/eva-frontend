using Android.Content.Res;
using Android.Content;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;

using static Android.Resource;
using Rect = Android.Graphics.Rect;
using View = Android.Views.View;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Android.Views;
using Android.OS;

namespace EvaMobil.Platforms.Android
{
    public class GlobalLayoutUtil
    {

        private bool isImmersed = false;
        private View mChildOfContent;
        private FrameLayout.LayoutParams frameLayoutParams;
        private int usableHeightPrevious = 0;

        public static void AssistActivity(Activity activity)
        {
            _ = new GlobalLayoutUtil(activity);
        }

        public GlobalLayoutUtil(Activity activity)
        {
            FrameLayout content = (FrameLayout)activity.FindViewById(Id.Content);
            mChildOfContent = content.GetChildAt(0);
            mChildOfContent.ViewTreeObserver.GlobalLayout += (s, o) => PossiblyResizeChildOfContent(activity);
            frameLayoutParams = (FrameLayout.LayoutParams)mChildOfContent.LayoutParameters;
        }

        private void PossiblyResizeChildOfContent(Activity activity)
        {
            int usableHeightNow = ComputeUsableHeight();
            if (usableHeightNow != usableHeightPrevious)
            {
                int usableHeightSansKeyboard = mChildOfContent.RootView.Height;
                int heightDifference = usableHeightSansKeyboard - usableHeightNow;
                if (heightDifference < 0)
                {
                    usableHeightSansKeyboard = mChildOfContent.RootView.Width;
                    heightDifference = usableHeightSansKeyboard - usableHeightNow;
                }
                 if (heightDifference > usableHeightSansKeyboard / 4)
                {
                    frameLayoutParams.Height = usableHeightSansKeyboard - heightDifference;
                }
                else if (heightDifference >= GetNavigationBarHeight(activity))
                {
                    frameLayoutParams.Height = usableHeightNow; 
                }
                else
                {
                    frameLayoutParams.Height = usableHeightNow;
                }
            }
            mChildOfContent.RequestLayout();
            usableHeightPrevious = usableHeightNow;
        }


        /**
         * Get the height of the visible content area
         */
        private int ComputeUsableHeight()
        {
            Rect rect = new Rect();
            mChildOfContent.GetWindowVisibleDisplayFrame(rect);
            if (isImmersed)
                return (int)rect.Bottom;
            else
                return (int)(rect.Bottom - rect.Top);
        }

        /**
         * Get the actual height of the navigation bar
         *
         * @param context:
         * @return: Navigation bar height
         */
        private static int GetNavigationBarHeight(Activity activity)
        {
            int height = 0;

            var decorView = activity.Window.DecorView;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                var windowInsets = decorView.RootWindowInsets;
                if (windowInsets != null)
                {
                    var insets = windowInsets.GetInsets(WindowInsets.Type.NavigationBars());
                    height = insets.Bottom;
                }
            }
            else
            {
                Resources resources = activity.Resources;
                int resourceId = resources.GetIdentifier("navigation_bar_height", "dimen", "android");
                if (resourceId > 0)
                    height = resources.GetDimensionPixelSize(resourceId);
            }

            return height;
        }

        private static int GetStatusBarHeight(Activity activity)
        {
            int result = 0;
            int resourceId = activity.Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId > 0)
                result = activity.Resources.GetDimensionPixelSize(resourceId);
            return result;
        }
    }
}

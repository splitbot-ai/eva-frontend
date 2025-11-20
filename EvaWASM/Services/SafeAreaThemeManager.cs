using EvaComponentLibrary.Services;

namespace EvaWASM.Services
{
    public class SafeAreaThemeManager : IThemeService
    {
        public event Action ThemeChanged;

        public void ApplyThemeColor(string theme)
        {
            return;
        }

        public void ApplyThemeColor(AppTheme? theme)
        {
            return ;
        }

        public void NotifyThemeChanged()
        {
            return;
        }
    }
}

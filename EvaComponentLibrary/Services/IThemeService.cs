
namespace EvaComponentLibrary.Services
{
    public interface IThemeService
    {
        event Action ThemeChanged;
        void NotifyThemeChanged();
        void ApplyThemeColor(string? theme);
        void ApplyThemeColor(AppTheme? theme);
    }

	public enum AppTheme
	{
		/// <summary>Default, unknown or unspecified theme.</summary>
		Unspecified,

		/// <summary>Light theme.</summary>
		Light,

		/// <summary>Dark theme.</summary>
		Dark
	}
}

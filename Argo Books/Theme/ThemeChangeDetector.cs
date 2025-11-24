using Microsoft.Win32;
using Argo_Books.Classes;
using Argo_Books.Theme;

namespace Argo_Books.Theme
{
    /// <summary>
    /// Detects Windows system theme changes and automatically updates application theme settings.
    /// </summary>
    internal class ThemeChangeDetector
    {
        private static bool _isListeningForThemeChanges = false;
        private static RegistryKey _personalizeKey = null;
        private static RegistryWatcher _watcher;

        /// <summary>
        /// Starts monitoring Windows system theme changes by setting up a registry watcher on the Windows personalization settings,
        /// and automatically updates the application theme when Windows theme changes are detected.
        /// </summary>
        public static void StartListeningForThemeChanges()
        {
            if (_isListeningForThemeChanges)
            {
                return;
            }

            // Add a small delay to avoid startup timing issues
            Task.Delay(500).ContinueWith(_ =>
            {
                try
                {
                    _personalizeKey = Registry.CurrentUser.OpenSubKey(
                        @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                        false);

                    if (_personalizeKey != null)
                    {
                        _personalizeKey = Registry.CurrentUser.OpenSubKey(
                            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                            true);

                        if (_personalizeKey != null)
                        {
                            // Create a registry watcher
                            _watcher = new RegistryWatcher(
                                RegistryHive.CurrentUser,
                                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize");

                            _watcher.RegChanged += OnWindowsThemeChanged;
                            _watcher.Start();
                            _isListeningForThemeChanges = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error_StartThemeListener(ex.Message);
                }
            });
        }
        private static void OnWindowsThemeChanged(object sender, EventArgs e)
        {
            if (ThemeManager.CurrentTheme == ThemeManager.ThemeType.Windows)
            {
                CustomColors.SetColors();
                FormThemeManager.UpdateAllForms();
                ThemeManager.UpdateOtherControls();
            }
        }
        public static void StopListeningForThemeChanges()
        {
            if (_isListeningForThemeChanges)
            {
                Log.Write(1, "Stopped listening for Windows theme changes");
                _isListeningForThemeChanges = false;
                _personalizeKey?.Close();
                _watcher?.Stop();
            }
        }
    }
}

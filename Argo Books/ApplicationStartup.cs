using Argo_Books.Classes;
using Argo_Books.Encryption;
using Argo_Books.Passwords;
using Argo_Books.Theme;
using Argo_Books.UI;
using Microsoft.Win32;
using Argo_Books;
using Argo_Books.Classes;
using Argo_Books.Language;

namespace Argo_Books
{
    /// <summary>
    /// Handles application startup tasks including file associations and opening files from command line.
    /// </summary>
    public static class ApplicationStartup
    {
        // Properties
        private static bool? _autoOpenAfterUpdate = null;

        /// <summary>
        /// Gets the auto-open flag value, calculating it only once.
        /// </summary>
        private static bool AutoOpenAfterUpdate
        {
            get
            {
                _autoOpenAfterUpdate ??= DataFileManager.GetBoolValue(GlobalAppDataSettings.AutoOpenRecentAfterUpdate);
                return _autoOpenAfterUpdate.Value;
            }
        }

        // Methods
        /// <summary>
        /// This method performs essential startup tasks including:
        /// - Setting up visual styles and high DPI mode
        /// - Starting the theme change detection
        /// - Registering file associations (only on first run)
        /// </summary>
        public static void InitializeApplication()
        {
            NetSparkleUpdateManager.Initialize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            CustomColors.SetColors();
            LoadingPanel.InitBlankLoadingPanel();
            LoadingPanel.InitLoadingPanel();

            Directories.SetUniversalDirectories();
            Directories.EnsureAppDataDirectoriesExist();
            ArgoCompany.InitCacheFiles();
            EncryptionManager.Initialize();
            DotEnv.Load();
            LanguageManager.InitLanguageManager();

            TextBoxManager.ConstructRightClickTextBoxMenu();
            SearchBox.ConstructSearchBox();
            CustomControls.ConstructRightClickRename();
            ThemeChangeDetector.StartListeningForThemeChanges();

            _ = Task.Run(EnsureDefaultLanguageTranslationsAsync);
            _ = Task.Run(HandlePostUpgradeTranslationUpdate);

            LicenseManager licenseManager = new();
            _ = licenseManager.ValidateKeyAsync();

            RegisterFileAssociationOnFirstRun();

            if (AutoOpenAfterUpdate)
            {
                // Clear the auto-open flag
                DataFileManager.SetValue(GlobalAppDataSettings.AutoOpenRecentAfterUpdate, bool.FalseString);
            }
        }

        /// <summary>
        /// Ensures that translations for the default language are available.
        /// Downloads them if the translations file is missing or doesn't contain the required language.
        /// </summary>
        private static async Task EnsureDefaultLanguageTranslationsAsync()
        {
            try
            {
                string defaultLanguage = Properties.Settings.Default.Language;

                // If default language is English, no need to download translations
                if (string.IsNullOrEmpty(defaultLanguage) || defaultLanguage.Equals("English", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                // Get the language abbreviation for the default language
                List<KeyValuePair<string, string>> languages = LanguageManager.GetLanguages();
                KeyValuePair<string, string> languageInfo = languages.FirstOrDefault(l => l.Key.Equals(defaultLanguage, StringComparison.OrdinalIgnoreCase));

                if (string.IsNullOrEmpty(languageInfo.Value))
                {
                    Log.WriteWithFormat(1, "Unknown default language: {0}", defaultLanguage);
                    return;
                }

                string languageAbbreviation = languageInfo.Value;

                // Check if translations are already available for this language
                bool translationsAvailable = LanguageManager.TranslationCache != null &&
                                             LanguageManager.TranslationCache.ContainsKey(languageAbbreviation) &&
                                             LanguageManager.TranslationCache[languageAbbreviation].Count > 0;

                if (!translationsAvailable)
                {
                    Log.WriteWithFormat(1, "Translations file missing or empty for default language '{0}'. Downloading...", defaultLanguage);

                    // Download the language file for the default language
                    bool downloadSuccess = await LanguageManager.DownloadAndMergeLanguageJson(defaultLanguage, false);

                    // Apply the translations to any open forms if the startup forms are still shown
                    if (MainMenu_Form.Instance == null)
                    {
                        foreach (Form form in Application.OpenForms)
                        {
                            LanguageManager.UpdateLanguageForControl(form);
                        }
                    }

                    if (downloadSuccess)
                    {
                        Log.WriteWithFormat(1, "Successfully downloaded translations for default language '{0}'", defaultLanguage);
                    }
                    else
                    {
                        Log.WriteWithFormat(1, "Failed to download translations for default language '{0}'. App will use original text.", defaultLanguage);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error_EnsureDefaultLanguageTranslations($"{ex.Message}");
            }
        }

        /// <summary>
        /// Checks if this is the first run after an upgrade and updates all translations if needed.
        /// </summary>
        private static async Task HandlePostUpgradeTranslationUpdate()
        {
            if (AutoOpenAfterUpdate)
            {
                // Update all existing translations in background
                bool updateSuccess = await LanguageManager.UpdateExistingTranslationsAfterUpgrade();

                if (updateSuccess)
                {
                    Log.WriteWithFormat(2, "All translations updated successfully");
                }
                else
                {
                    Log.WriteWithFormat(0, "Translations update completed with failures");
                }
            }
        }

        /// <summary>
        /// Registers Argo file associations if they have not already been registered.
        /// </summary>
        private static void RegisterFileAssociationOnFirstRun()
        {
            try
            {
                string subKey = @"Software\ArgoSalesTracker";
                bool hasRegistered = false;

                using RegistryKey key = Registry.CurrentUser.OpenSubKey(subKey, false);
                if (key != null)
                {
                    hasRegistered = key.GetValue("AssociationsRegistered", false) as bool? ?? false;
                }

                if (!hasRegistered)
                {
                    ArgoFiles.RegisterFileIcon(ArgoFiles.ArgoCompanyFileExtension, Properties.Resources.ArgoColor, 0);
                    ArgoFiles.RegisterFileIcon(ArgoFiles.ArgoTemplateFileExtension, Properties.Resources.ArgoColor, 0);

                    using RegistryKey writeKey = Registry.CurrentUser.CreateSubKey(subKey);
                    writeKey?.SetValue("AssociationsRegistered", true);
                }
            }
            catch (Exception ex)
            {
                Log.Error_RegisterFileAssociations(ex.Message);
            }
        }

        /// <summary>
        /// Handles opening a company directly from command line arguments.
        /// </summary>
        /// <returns>True if file was successfully opened, otherwise false.</returns>
        public static bool TryOpenCompanyFromCommandLine(string[] args)
        {
            try
            {
                // If the command is invalid
                if (args.Length == 0 || !File.Exists(args[0]))
                {
                    return false;
                }

                string extension = Path.GetExtension(args[0]);

                if (!extension.Equals(ArgoFiles.ArgoCompanyFileExtension, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                // Open the company file directly
                string filePath = args[0];
                string directory = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileName(filePath);
                string companyName = Path.GetFileNameWithoutExtension(fileName);

                if (!ArgoCompany.OnlyAllowOneInstanceOfACompany(companyName))
                {
                    return false;
                }

                // Open the company
                Directories.SetDirectories(directory, companyName);

                if (!PasswordManager.EnterPassword())
                {
                    return false;
                }

                // Save recently opened companies
                DataFileManager.AppendValue(GlobalAppDataSettings.RecentCompanies, filePath);

                // Import company data
                List<string> dirNames = Directories.GetListOfAllDirectoryNamesInDirectory(Directories.AppData_dir);
                Directories.ImportArgoTarFile(filePath, Directories.AppData_dir, dirNames, false);
                DataFileManager.SetValue(AppDataSettings.ChangesMade, false.ToString());

                MainMenu_Form form = new();
                form.Show();

                return true;
            }
            catch (Exception ex)
            {
                Log.Error_OpenCompanyFromCommandLine($"{ex.Message}");

                // Show a regular MessageBox in case CustomMessageBox is unavailable
                MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Auto-opens the most recent company after an update if the flag is set.
        /// </summary>
        /// <returns>True if a company was automatically opened, otherwise false.</returns>
        public static bool TryAutoOpenRecentCompanyAfterUpdate()
        {
            try
            {
                if (!AutoOpenAfterUpdate)
                {
                    return false;
                }

                // Delete the language files cache to force re-download
                Directories.DeleteFile(Directories.English_file);
                Directories.DeleteFile(Directories.Translations_file);

                // Get the most recent company
                List<string> recentCompanies = ArgoCompany.GetValidRecentCompanyPaths(excludeCurrentCompany: false);
                if (recentCompanies.Count == 0)
                {
                    return false;
                }

                string mostRecentCompany = recentCompanies[0];

                return TryOpenCompanyFromCommandLine([mostRecentCompany]);
            }
            catch (Exception ex)
            {
                Log.Error_AutoOpenRecentCompanyAfterUpdate($"{ex.Message}");
                return false;
            }
        }
    }
}
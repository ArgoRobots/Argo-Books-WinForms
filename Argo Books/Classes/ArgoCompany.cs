using Argo_Books.GridView;
using Argo_Books.Passwords;
using Argo_Books.UI;
using Argo_Books;
using Argo_Books.Classes;
using Argo_Books.Rentals;
using Argo_Books.Startup.Menus;
using System.Text;

namespace Argo_Books.Classes
{
    /// <summary>
    /// Static class responsible for managing company data files, company state, and file operations.
    /// </summary>
    public static class ArgoCompany
    {
        /// <summary>
        /// Static mutex used to ensure only one instance of a company can be open at a time.
        /// </summary>
        public static Mutex? ApplicationMutex { get; private set; } = null;

        /// <summary>
        /// Initializes cache directory and global application settings file.
        /// Creates necessary directories and files if they don't exist.
        /// </summary>
        public static void InitCacheFiles()
        {
            Directories.CreateDirectory(Directories.Cache_dir);

            if (!File.Exists(Directories.GlobalAppDataSettings_file))
            {
                Directories.CreateFile(Directories.GlobalAppDataSettings_file);
                Directories.CreateFile(Directories.ExchangeRates_file);

                // TEMPORARILY DISABLE TUTORIAL VIDEOS BECAUSE THEY DO NOT EXIST YET
                DataFileManager.SetValue(GlobalAppDataSettings.ImportSpreadsheetTutorial, bool.FalseString);
                DataFileManager.SetValue(GlobalAppDataSettings.ShowWelcomeForm, bool.FalseString);
            }
        }

        /// <summary>
        /// Saves all current company data to a tar file and resets change tracking.
        /// </summary>
        public static void SaveAll()
        {
            // Save the current application version
            string currentVersion = Tools.GetVersionNumber();
            DataFileManager.SetValue(AppDataSettings.AppVersion, currentVersion);

            Directories.CreateArgoTarFileFromDirectory(Directories.TempCompany_dir, Directories.ArgoCompany_file, true);
            Log.WriteWithFormat(2, "Saved '{0}'", Directories.CompanyName);
            ResetChanges();

            DataFileManager.SetValue(AppDataSettings.ChangesMade, false.ToString());
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Prompts user to select a location and saves the company data to a new location.
        /// </summary>
        public static void SaveAs()
        {
            using SaveFileDialog dialog = new();
            dialog.FileName = Directories.CompanyName;
            dialog.DefaultExt = ArgoFiles.ArgoCompanyFileExtension;
            dialog.Filter = $"Argo Company Files|*{ArgoFiles.ArgoCompanyFileExtension}";
            dialog.Title = "Save Company As";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Create a tar file from the temp directory directly to the new location
                Directories.CreateArgoTarFileFromDirectory(Directories.TempCompany_dir, dialog.FileName, true);
                Log.WriteWithFormat(2, "Saved '{0}' to new location", Path.GetFileNameWithoutExtension(dialog.FileName));
            }
        }

        /// <summary>
        /// Asks the user to save if there are any changes.
        /// </summary>
        /// <returns>Returns true if the operation should continue (save, don't save, or no changes). Returns false if the user cancels.</returns>
        public static bool AskUserToSave()
        {
            if (!AreAnyChangesMade()) { return true; }

            CustomMessageBoxResult result = CustomMessageBox.Show(
            "Save changes", "Save changes to the following items?",
            CustomMessageBoxIcon.None, CustomMessageBoxButtons.SaveDontSaveCancel);

            switch (result)
            {
                case CustomMessageBoxResult.Save:
                    SaveAll();
                    break;
                case CustomMessageBoxResult.DontSave:
                    ResetChanges();
                    break;
                case CustomMessageBoxResult.Cancel:
                    // Cancel close
                    return false;
                default:  // If the CustomMessageBox was closed
                    // Cancel close
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if any changes have been made across all forms in the application.
        /// </summary>
        /// <returns>True if any changes have been made, false otherwise.</returns>
        public static bool AreAnyChangesMade()
        {
            if (MainMenu_Form.ThingsThatHaveChangedInFile.Count > 0 ||
                MainMenu_Form.SettingsThatHaveChangedInFile.Count > 0 ||
                Accountants_Form.ThingsThatHaveChangedInFile.Count > 0 ||
                Categories_Form.ThingsThatHaveChangedInFile.Count > 0 ||
                Companies_Form.ThingsThatHaveChangedInFile.Count > 0 ||
                Products_Form.ThingsThatHaveChangedInFile.Count > 0 ||
                AddSale_Form.ThingsThatHaveChangedInFile.Count > 0 ||
                AddPurchase_Form.ThingsThatHaveChangedInFile.Count > 0 ||
                AddRentalItem_Form.ThingsThatHaveChangedInFile.Count > 0 ||
                Customers_Form.ThingsThatHaveChangedInFile.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Resets all change tracking collections across all forms.
        /// </summary>
        public static void ResetChanges()
        {
            MainMenu_Form.ThingsThatHaveChangedInFile.Clear();
            MainMenu_Form.SettingsThatHaveChangedInFile.Clear();
            Accountants_Form.ThingsThatHaveChangedInFile.Clear();
            Categories_Form.ThingsThatHaveChangedInFile.Clear();
            Companies_Form.ThingsThatHaveChangedInFile.Clear();
            Products_Form.ThingsThatHaveChangedInFile.Clear();
            AddSale_Form.ThingsThatHaveChangedInFile.Clear();
            AddPurchase_Form.ThingsThatHaveChangedInFile.Clear();
            AddRentalItem_Form.ThingsThatHaveChangedInFile.Clear();
            Customers_Form.ThingsThatHaveChangedInFile.Clear();
            DataFileManager.SetValue(AppDataSettings.ChangesMade, false.ToString());
        }

        /// <summary>
        /// Opens a dialog for selecting and opening an existing company file.
        /// </summary>
        public static void OpenCompanyFromDialog()
        {
            // Select file
            OpenFileDialog dialog = new()
            {
                Filter = $"Argo company (*{ArgoFiles.ArgoCompanyFileExtension})|*{ArgoFiles.ArgoCompanyFileExtension}"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (!OnlyAllowOneInstanceOfACompany(Path.GetFileNameWithoutExtension(dialog.FileName)))
                {
                    return;
                }

                if (!OpenCompanyFromPath(Directory.GetParent(dialog.FileName).FullName, dialog.FileName))
                {
                    return;
                }
                GetStarted_Form.Instance.ShowMainMenu();
            }
        }

        /// <summary>
        /// Opens a company file at the specified location.
        /// </summary>
        /// <returns>True if successfully opened, otherwise false.</returns>
        private static bool OpenCompanyFromPath(string filePath, string name)
        {
            Directories.SetDirectories(filePath, Path.GetFileNameWithoutExtension(name));

            if (!VersionCompatibilityChecker.HandleFileVersionCompatibility(filePath))
            {
                return false;
            }

            if (!PasswordManager.EnterPassword())
            {
                return false;
            }

            // Save recently opened companies
            DataFileManager.AppendValue(GlobalAppDataSettings.RecentCompanies, Directories.ArgoCompany_file);

            // Import company data
            List<string> dirNames = Directories.GetListOfAllDirectoryNamesInDirectory(Directories.AppData_dir);
            Directories.ImportArgoTarFile(Directories.ArgoCompany_file, Directories.AppData_dir, dirNames, false);
            DataFileManager.SetValue(AppDataSettings.ChangesMade, false.ToString());

            return true;
        }

        /// <summary>
        /// Renames the company company and updates all associated files and directories.
        /// </summary>
        public static void Rename(string name)
        {
            string newFile = Directories.ArgoCompany_dir + name + ArgoFiles.ArgoCompanyFileExtension;
            string newDir = Directories.AppData_dir + name;

            // Rename in file
            Directories.MoveFile(Directories.ArgoCompany_file, newFile);
            Directories.RenameFolder(Directories.TempCompany_dir, newDir);

            Directories.SetDirectories(Directories.ArgoCompany_dir, name);

            // Update recently opened companies
            DataFileManager.AppendValue(GlobalAppDataSettings.RecentCompanies, Directories.ArgoCompany_file);
        }

        /// <summary>
        /// Clears all cached data and displays the amount of space cleared.
        /// </summary>
        public static void ClearCache()
        {
            string directoryPath = Directories.Cache_dir;
            long totalSizeInBytes = 0;

            if (Directory.Exists(directoryPath))
            {
                totalSizeInBytes = Directories.CalculateDirectorySize(directoryPath);
                Directories.DeleteDirectory(directoryPath, true);
            }

            string totalSizeReadable = Tools.ConvertBytesToReadableSize(totalSizeInBytes);

            if (totalSizeInBytes > 0)
            {
                CustomMessageBox.ShowWithFormat("Cleared cache", "Cleared {0} of cached data.",
                    CustomMessageBoxIcon.Success, CustomMessageBoxButtons.Ok,
                    totalSizeReadable);
            }
            else
            {
                CustomMessageBox.Show("No cache to clear", "No cache to clear.",
                    CustomMessageBoxIcon.Info, CustomMessageBoxButtons.Ok);
            }
        }

        /// <summary>
        /// Opens a new company when another company is already open. Handles saving current company's changes if necessary.
        /// </summary>
        public static void OpenCompanyWhenACompanyIsAlreadyOpen()
        {
            // Select file
            OpenFileDialog dialog = new()
            {
                Filter = $"Argo company (*{ArgoFiles.ArgoCompanyFileExtension})|*{ArgoFiles.ArgoCompanyFileExtension}"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                OpenCompanyWhenACompanyIsAlreadyOpenFromPath(dialog.FileName);
            }
        }

        /// <summary>
        /// Opens a new company from the specified file path when another company is already open.
        /// Handles saving current company's changes if necessary.
        /// </summary>
        public static void OpenCompanyWhenACompanyIsAlreadyOpenFromPath(string filePath, bool overrideCompanyAlreadyOpen = false)
        {
            // Validate the file path
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                CustomMessageBox.Show("Company does not exist", "The specified company file does not exist.",
                    CustomMessageBoxIcon.Exclamation, CustomMessageBoxButtons.Ok);
                return;
            }

            // If this company is already open
            if (Directories.ArgoCompany_file == filePath && !overrideCompanyAlreadyOpen)
            {
                CustomMessageBox.Show("Company already open", "This company is already open.",
                    CustomMessageBoxIcon.Exclamation, CustomMessageBoxButtons.Ok);
                return;
            }

            if (!VersionCompatibilityChecker.HandleFileVersionCompatibility(filePath))
            {
                return;
            }

            // Save current company
            if (!AskUserToSave())
            {
                return;
            }

            Directories.DeleteDirectory(Directories.TempCompany_dir, true);

            if (!OpenCompanyFromPath(Directory.GetParent(filePath).FullName, filePath))
            {
                return;
            }

            MainMenu_Form.IsProgramLoading = true;
            MainMenu_Form.Instance.ResetData();
            MainMenu_Form.Instance.LoadData();
            MainMenu_Form.IsProgramLoading = false;

            // Reset controls
            MainMenu_Form.Instance.SetCompanyLabel();
            MainMenu_Form.Instance.UpdateTotalLabels();
            MainMenu_Form.Instance.LoadOrRefreshMainCharts();
            MainMenu_Form.Instance.HideShowingResultsForLabel();
            MainMenu_Form.Instance.UpdateMainMenuFormText();
            MainMenu_Form.Instance.SetAllAnalyticTabsAsNotLoaded();
            MainMenu_Form.Instance.ReloadCurrentAnalyticTab();
            DataGridViewManager.UpdateRowColors(MainMenu_Form.Instance.SelectedDataGridView);
            DateRange_Form.Instance.ResetControls();
            CompanyLogo.SetCompanyLogo();

            Tools.CloseAllOpenForms();

            bool hasVisibleRows = AreRowsVisible(MainMenu_Form.Instance.SelectedDataGridView);
            LabelManager.ManageNoDataLabelOnControl(hasVisibleRows, MainMenu_Form.Instance.SelectedDataGridView);
        }

        /// <summary>
        /// Checks if any rows in the given DataGridView are visible.
        /// </summary>
        /// <returns>True if any rows are visible, otherwise false.</returns>
        private static bool AreRowsVisible(DataGridView dataGridView)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Visible)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Ensures only one instance of a company can be open at a time.
        /// </summary>
        /// <returns>True if this is the only instance, false if another instance exists.</returns>
        public static bool OnlyAllowOneInstanceOfACompany(string companyFilePath)
        {
            if (!CreateMutex(companyFilePath))
            {
                CustomMessageBox.Show("Already open",
                    "This company is already open.",
                    CustomMessageBoxIcon.Exclamation, CustomMessageBoxButtons.Ok);
                ApplicationMutex?.Dispose();  // Reset
                return false;
            }
            return true;
        }

        /// <summary>
        /// Only allow one instance of a company to be open at a time.
        /// </summary>
        /// <returns>True if a mutex is created. False if a mutex already exists.</returns>
        public static bool CreateMutex(string companyFilePath)
        {
            string uniqueMutexName = "Global\\MyApplication_" + GetUniqueCompanyIdentifier(companyFilePath);
            ApplicationMutex = new Mutex(initiallyOwned: true, name: uniqueMutexName, out bool createdNew);

            if (createdNew)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Generates a unique identifier for a company based on its file path.
        /// </summary>
        /// <returns>Base64 encoded unique identifier.</returns>
        public static string GetUniqueCompanyIdentifier(string companyFilePath)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(companyFilePath));
        }

        /// <summary>
        /// This will prompt the user to recover any unsaved work.
        /// </summary>
        /// <returns>True if unsaved work was recovered. False if no work was recovered.</returns>
        public static void RecoverUnsavedWork()
        {
            List<string> companies = Directories.GetListOfAllDirectoriesInDirectory(Directories.AppData_dir);

            foreach (string company in companies)
            {
                string translationsFolder = Path.Combine(Directories.AppData_dir, Directories.TranlationsReferenceFolderName);

                if (company + @"\" == Directories.Cache_dir ||
                    company == translationsFolder)
                {
                    continue;
                }

                // If there are no changes
                string filepath = Path.Combine(company, Directories.CompanyDataFileName);
                bool changesMade = DataFileManager.GetBoolValue(AppDataSettings.ChangesMade, filepath);
                if (!changesMade)
                {
                    // Delete the temp folder
                    Directories.DeleteDirectory(company, true);
                    return;
                }

                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat("Unsaved work found",
                    "Unsaved work was found. Would you like to recover it? {0}",
                    CustomMessageBoxIcon.Exclamation,
                    CustomMessageBoxButtons.YesNo,
                    Path.GetFileName(company));

                if (result == CustomMessageBoxResult.Yes)
                {
                    CustomMessageBox.Show("Select folder",
                        $"You will be promted to select a folder to save the unsaved work.",
                        CustomMessageBoxIcon.Info, CustomMessageBoxButtons.Ok);

                    // Select folder
                    Ookii.Dialogs.WinForms.VistaFolderBrowserDialog dialog = new();

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        Directories.SetDirectories(dialog.SelectedPath, Path.GetFileNameWithoutExtension(company));
                        SaveAll();

                        // Delete the temp folder
                        Directories.DeleteDirectory(company, true);
                    }
                }
                else if (result == CustomMessageBoxResult.No)
                {
                    // Delete the temp folder
                    Directories.DeleteDirectory(company, true);
                }
            }
        }

        /// <summary>
        /// Retrieves a list of valid recent company paths from the global application data settings.
        /// </summary>
        public static List<string> GetValidRecentCompanyPaths(bool excludeCurrentCompany)
        {
            string? value = DataFileManager.GetValue(GlobalAppDataSettings.RecentCompanies);
            if (string.IsNullOrEmpty(value))
            {
                return [];
            }

            string[] companyPaths = value.Split(',');
            Array.Reverse(companyPaths);  // Reverse the array so it loads in the correct order

            string? currentCompanyPath = null;
            if (excludeCurrentCompany)
            {
                currentCompanyPath = Directories.ArgoCompany_file;
            }

            // Remove duplicates (case-insensitive), filter valid paths, and optionally exclude the current company
            List<string> validCompanyPaths = companyPaths
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Where(File.Exists)
                .Where(path => !excludeCurrentCompany || !string.Equals(path, currentCompanyPath, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return validCompanyPaths;
        }
    }
}
using Guna.UI2.WinForms;
using Sales_Tracker.AnonymousData;
using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.Language;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker.Settings.Menus
{
    /// <summary>
    /// General settings form for application preferences.
    /// </summary>
    public partial class General_Form : Form
    {
        // Properties
        private static General_Form _instance;

        // Getter
        public static General_Form Instance => _instance;

        // Init.
        public General_Form()
        {
            InitializeComponent();
            _instance = this;

            DpiHelper.ScaleComboBox(ColorTheme_ComboBox);
            InitializeAdminModeControls();
            ThemeManager.SetThemeForForm(this);
            SetAccessibleDescription();
            LanguageManager.UpdateLanguageForControl(this);
            UpdateControls();
            AddEventHandlersToTextBoxes();
            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private static string GetThemeDisplayText(ThemeManager.ThemeType theme)
        {
            return theme switch
            {
                ThemeManager.ThemeType.Light => LanguageManager.TranslateString("Light"),
                ThemeManager.ThemeType.Dark => LanguageManager.TranslateString("Dark"),
                ThemeManager.ThemeType.Windows => "Windows",  // Keep Windows untranslated
                _ => theme.ToString()
            };
        }
        private void SetAccessibleDescription()
        {
            Language_TextBox.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
            Currency_TextBox.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;

            // Prevent automatic translation since we handle it manually
            ColorTheme_ComboBox.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
        }
        public void AlignLabels()
        {
            Language_Label.Left = Language_TextBox.Left - Language_Label.Width - 2;
            Currency_Label.Left = Currency_TextBox.Left - Currency_Label.Width - 2;
            ColorTheme_Label.Left = ColorTheme_ComboBox.Left - ColorTheme_Label.Width - 2;
            ShowTooltips_Label.Left = ShowTooltips_CheckBox.Left - ShowTooltips_Label.Width - 2;
            ShowDebugInfo_Label.Left = ShowDebugInfo_CheckBox.Left - ShowDebugInfo_Label.Width - 2;
            SendAnonymousInformation_Label.Left = SendAnonymousInformation_CheckBox.Left - SendAnonymousInformation_Label.Width - 2;
            PurchaseReceipts_Label.Left = PurchaseReceipts_CheckBox.Left - PurchaseReceipts_Label.Width - 2;
            SalesReceipts_Label.Left = SalesReceipts_CheckBox.Left - SalesReceipts_Label.Width - 2;
            AnimateButtons_Label.Left = AnimateButtons_CheckBox.Left - AnimateButtons_Label.Width - 2;
            AnimateCharts_Label.Left = AnimateCharts_CheckBox.Left - AnimateCharts_Label.Width - 2;
            EnableAISearch_Label.Left = EnableAISearch_CheckBox.Left - EnableAISearch_Label.Width - 2;
            ShowCompanyLogo_Label.Left = ShowCompanyLogo_CheckBox.Left - ShowCompanyLogo_Label.Width - 2;
        }
        private void AddEventHandlersToTextBoxes()
        {
            byte searchBoxMaxHeight = 255;

            TextBoxManager.Attach(Language_TextBox);
            List<SearchResult> searchResult = LanguageManager.GetLanguageSearchResults();
            SearchBox.Attach(Language_TextBox, this, () => searchResult, searchBoxMaxHeight, false, false, false, false);
            Language_TextBox.TextChanged += (_, _) => ValidateInputs();

            TextBoxManager.Attach(Currency_TextBox);
            SearchBox.Attach(Currency_TextBox, this, Currency.GetSearchResults, searchBoxMaxHeight, false, false, false, false);
            Currency_TextBox.TextChanged += (_, _) => ValidateInputs();
        }
        private void InitializeAdminModeControls()
        {
            if (!MainMenu_Form.IsAdminMode)
            {
                return;
            }

            float scale = DpiHelper.GetRelativeDpiScale();

            Guna2Button generateTranslationsButton = new()
            {
                Text = LanguageManager.TranslateString("Generate Translations"),
                Size = new Size((int)(200 * scale), (int)(40 * scale)),
                Location = new Point((Width - (int)(200 * scale)) / 2, EnableAISearch_CheckBox.Bottom + (int)(80 * scale)),
                Anchor = AnchorStyles.Top
            };
            generateTranslationsButton.Click += GenerateTranslationsButton_Click;
            ThemeManager.MakeGButtonBlueSecondary(generateTranslationsButton);

            Controls.Add(generateTranslationsButton);
        }

        // Form event handlers
        private void General_form_Shown(object sender, EventArgs e)
        {
            General_Label.Focus();  // Deselect controls
            AlignLabels();
            LoadingPanel.HideBlankLoadingPanel(this);
        }

        // Label event handlers
        private void ShowTooltips_Label_Click(object sender, EventArgs e)
        {
            ShowTooltips_CheckBox.Checked = !ShowTooltips_CheckBox.Checked;
        }
        private void ShowDebugInfo_Label_Click(object sender, EventArgs e)
        {
            ShowDebugInfo_CheckBox.Checked = !ShowDebugInfo_CheckBox.Checked;
        }
        private void SendAnonymousInformation_Label_Click(object sender, EventArgs e)
        {
            SendAnonymousInformation_CheckBox.Checked = !SendAnonymousInformation_CheckBox.Checked;
        }
        private void PurchaseReceipts_Label_Click(object sender, EventArgs e)
        {
            PurchaseReceipts_CheckBox.Checked = !PurchaseReceipts_CheckBox.Checked;
        }
        private void SalesReceipts_Label_Click(object sender, EventArgs e)
        {
            SalesReceipts_CheckBox.Checked = !SalesReceipts_CheckBox.Checked;
        }
        private void AnimateButtons_Label_Click(object sender, EventArgs e)
        {
            AnimateButtons_CheckBox.Checked = !AnimateButtons_CheckBox.Checked;
        }
        private void AnimateCharts_Label_Click(object sender, EventArgs e)
        {
            AnimateCharts_CheckBox.Checked = !AnimateCharts_CheckBox.Checked;
        }
        private void EnableAISearch_Label_Click(object sender, EventArgs e)
        {
            EnableAISearch_CheckBox.Checked = !EnableAISearch_CheckBox.Checked;
        }
        private void ShowCompanyLogo_Label_Click(object sender, EventArgs e)
        {
            ShowCompanyLogo_CheckBox.Checked = !ShowCompanyLogo_CheckBox.Checked;
        }

        // Event handlers
        private void MoreInformation_Button_Click(object sender, EventArgs e)
        {
            Tools.OpenLink("https://argorobots.com/documentation/index.php#anonymous-data");
        }
        private void ExportData_Button_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Directories.AnonymousUserData_file) || AnonymousDataManager.GetUserDataCacheSize() == 0)
            {
                CustomMessageBox.Show("No user data",
                    "No user data exists. Either the setting was disabled or the cache was cleared.",
                    CustomMessageBoxIcon.Info, CustomMessageBoxButtons.Ok);
                return;
            }

            using SaveFileDialog dialog = new();
            dialog.FileName = $"AnonymousUsageData_{DateTime.Now:yyyyMMdd}{ArgoFiles.JsonFileExtension}";
            dialog.DefaultExt = ArgoFiles.JsonFileExtension;
            dialog.Filter = $"JSON files|*{ArgoFiles.JsonFileExtension}";
            dialog.Title = "Export user data";
            dialog.OverwritePrompt = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _ = AnonymousDataManager.ExportOrganizedDataAsync(dialog.FileName, true);

                    Log.WriteWithFormat(2, "Exported organized anonymous usage data to '{0}'", Path.GetFileName(dialog.FileName));

                    CustomMessageBox.Show("Export Successful",
                        "Successfully exported anonymous usage data.",
                        CustomMessageBoxIcon.Success, CustomMessageBoxButtons.Ok);
                }
                catch (Exception ex)
                {
                    Log.Error_AnonymousDataCollection($"Failed to export organized user data: {ex.Message}");
                }
            }
        }
        private void DeleteData_Button_Click(object sender, EventArgs e)
        {
            CustomMessageBoxResult result = CustomMessageBox.Show(
                "Delete anonymous usage data", $"Are you sure you want to delete your anonymous usage data?",
                 CustomMessageBoxIcon.Question, CustomMessageBoxButtons.YesNo);

            if (result == CustomMessageBoxResult.Yes)
            {
                AnonymousDataManager.ClearUserData();

                CustomMessageBox.Show("Export Successful", "Successfully deleted anonymous usage data.",
                    CustomMessageBoxIcon.Success, CustomMessageBoxButtons.Ok);
            }
        }
        private void GenerateTranslationsButton_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.Language != "English")
            {
                CustomMessageBox.Show("Language Setting", "The default language must be set to English to generate translations.",
                    CustomMessageBoxIcon.Exclamation, CustomMessageBoxButtons.Ok);
                return;
            }

            bool isLicenseActivated = Properties.Settings.Default.LicenseActivated;

            // Ensure all controls are visible so translations are generated for them
            Properties.Settings.Default.LicenseActivated = false;
            Properties.Settings.Default.Save();

            try
            {
                // Open the new language selection form
                LanguageSelection_Form languageForm = new();
                DialogResult result = languageForm.ShowDialog(this);

                if (result == DialogResult.Cancel)
                {
                    Log.Write(1, "Translation generation cancelled by user");
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowWithFormat("Error", "Error opening translation generator: {0}",
                    CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok, ex.Message);
            }
            finally
            {
                Properties.Settings.Default.LicenseActivated = isLicenseActivated;
                Properties.Settings.Default.Save();
            }
        }

        // Methods
        private void ValidateInputs()
        {
            bool isValid = (Language_TextBox.Tag == null || Language_TextBox.Tag.ToString() != "0") &&
                    (Currency_TextBox.Tag == null || Currency_TextBox.Tag.ToString() != "0");

            Settings_Form.Instance.Ok_Button.Enabled = isValid;
            Settings_Form.Instance.Apply_Button.Enabled = isValid;
        }
        public void UpdateControls()
        {
            Language_TextBox.Text = Properties.Settings.Default.Language;
            Currency_TextBox.Text = DataFileManager.GetValue(AppDataSettings.DefaultCurrencyType);
            PopulateThemeComboBox();

            ShowTooltips_CheckBox.Checked = Properties.Settings.Default.ShowTooltips;
            ShowDebugInfo_CheckBox.Checked = Properties.Settings.Default.ShowDebugInfo;
            SendAnonymousInformation_CheckBox.Checked = Properties.Settings.Default.SendAnonymousInformation;
            PurchaseReceipts_CheckBox.Checked = Properties.Settings.Default.PurchaseReceipts;
            SalesReceipts_CheckBox.Checked = Properties.Settings.Default.SaleReceipts;
            AnimateButtons_CheckBox.Checked = Properties.Settings.Default.AnimateButtons;
            AnimateCharts_CheckBox.Checked = Properties.Settings.Default.AnimateCharts;
            ShowCompanyLogo_CheckBox.Checked = Properties.Settings.Default.ShowCompanyLogo;
            EnableAISearch_CheckBox.Checked = Properties.Settings.Default.AISearchEnabled;
        }
        public void PopulateThemeComboBox()
        {
            // Clear and repopulate with translated text
            ColorTheme_ComboBox.Items.Clear();

            foreach (ThemeManager.ThemeType theme in Enum.GetValues<ThemeManager.ThemeType>())
            {
                string displayText = GetThemeDisplayText(theme);
                ColorTheme_ComboBox.Items.Add(displayText);
            }

            // Restore selection
            ColorTheme_ComboBox.SelectedItem = GetThemeDisplayText(ThemeManager.CurrentTheme);
        }
    }
}
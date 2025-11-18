using Guna.UI2.WinForms;
using Newtonsoft.Json;
using Sales_Tracker.Classes;
using Sales_Tracker.Language;
using Sales_Tracker.Theme;

namespace Sales_Tracker.Settings.Menus
{
    /// <summary>
    /// Form for selecting and managing language preferences.
    /// </summary>
    public partial class LanguageSelection_Form : BaseForm
    {
        // Properties
        private readonly Dictionary<string, Guna2CustomCheckBox> _languageCheckBoxes = [];
        private readonly Dictionary<string, Label> _languageLabels = [];
        private readonly Dictionary<string, Label> _statusLabels = [];
        private string _referenceFolder = "";

        // Init
        public LanguageSelection_Form()
        {
            InitializeComponent();
            SetupLanguageCheckboxes();

            _referenceFolder = Path.Combine(Directories.AppData_dir, Directories.TranlationsReferenceFolderName);

            if (!Directory.Exists(_referenceFolder))
            {
                Directory.CreateDirectory(_referenceFolder);
            }

            ThemeManager.SetThemeForForm(this);
            UpdateUI();
        }
        private void SetupLanguageCheckboxes()
        {
            List<KeyValuePair<string, string>> languages = LanguageManager.GetLanguages();
            int yPosition = 10;
            int itemHeight = 35;
            int xPos1 = 20;   // Checkbox
            int xPos2 = 50;   // Language name 
            int xPos3 = 280;  // Status

            // Sort languages alphabetically, but with priority languages first
            string[] priorityLanguages = ["English", "French", "German", "Italian"];
            List<KeyValuePair<string, string>> sortedLanguages = priorityLanguages
                .Where(p => languages.Any(l => l.Key == p))
                .Select(p => languages.First(l => l.Key == p))
                .Concat(languages.Where(l => !priorityLanguages.Contains(l.Key)).OrderBy(l => l.Key))
                .ToList();

            foreach (KeyValuePair<string, string> language in sortedLanguages)
            {
                // Create checkbox
                Guna2CustomCheckBox checkBox = new()
                {
                    Location = new Point(xPos1, yPosition + 8),
                    Size = new Size(20, 20),
                    Checked = false,
                    Animated = true
                };

                checkBox.CheckedChanged += (s, e) => UpdateUI();

                _languageCheckBoxes[language.Value] = checkBox;
                LanguagesPanel.Controls.Add(checkBox);

                // Create language name label
                Label nameLabel = new()
                {
                    Text = $"{language.Key} ({language.Value})",
                    Location = new Point(xPos2, yPosition + 5),
                    Size = new Size(200, 25),
                    Font = new Font("Segoe UI", 10F),
                    AutoSize = false,
                    Cursor = Cursors.Hand
                };
                nameLabel.Click += (s, e) =>
                {
                    checkBox.Checked = !checkBox.Checked;
                    UpdateUI();
                };
                _languageLabels[language.Value] = nameLabel;
                LanguagesPanel.Controls.Add(nameLabel);

                // Create status label
                Label statusLabel = new()
                {
                    Text = "Checking...",
                    Location = new Point(xPos3, yPosition + 5),
                    Size = new Size(180, 25),
                    Font = new Font("Segoe UI", 9F),
                    AutoSize = false
                };
                _statusLabels[language.Value] = statusLabel;
                LanguagesPanel.Controls.Add(statusLabel);

                yPosition += itemHeight;
            }
        }
        private void UpdateUI()
        {
            bool hasFolderSelected = !string.IsNullOrEmpty(_referenceFolder) && Directory.Exists(_referenceFolder);
            bool hasLanguagesSelected = _languageCheckBoxes.Any(kv => kv.Value.Checked);

            Generate_Button.Enabled = hasFolderSelected && hasLanguagesSelected;

            if (hasFolderSelected)
            {
                FolderPath_Label.Text = _referenceFolder;
                FolderPath_Label.ForeColor = CustomColors.AccentGreen;
                UpdateLanguageStatus();
            }
            else
            {
                FolderPath_Label.Text = "No folder selected";
                FolderPath_Label.ForeColor = Color.Red;

                // Clear all status labels
                foreach (Label statusLabel in _statusLabels.Values)
                {
                    statusLabel.Text = "Select folder first";
                    statusLabel.ForeColor = CustomColors.Text;
                }
            }

            // Update selected count
            int selectedCount = _languageCheckBoxes.Count(kv => kv.Value.Checked);
            Status_Label.Text = $"{selectedCount} language(s) selected";
        }
        private void UpdateLanguageStatus()
        {
            if (string.IsNullOrEmpty(_referenceFolder) || !Directory.Exists(_referenceFolder))
            {
                return;
            }

            foreach (KeyValuePair<string, Label> kvp in _statusLabels)
            {
                string languageCode = kvp.Key;
                Label statusLabel = kvp.Value;

                string filePath = Path.Combine(_referenceFolder, $"{languageCode}.json");

                if (File.Exists(filePath))
                {
                    try
                    {
                        string content = File.ReadAllText(filePath);
                        Dictionary<string, string>? translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                        int count = translations?.Count ?? 0;

                        if (count == 0)
                        {
                            statusLabel.Text = "File empty";
                            statusLabel.ForeColor = Color.FromArgb(255, 165, 0);
                        }
                        else
                        {
                            statusLabel.Text = $"Exists ({count:N0} items)";
                            statusLabel.ForeColor = CustomColors.AccentGreen;
                        }
                    }
                    catch
                    {
                        statusLabel.Text = "File corrupted";
                        statusLabel.ForeColor = CustomColors.AccentRed;
                    }
                }
                else
                {
                    statusLabel.Text = "New language";
                    statusLabel.ForeColor = CustomColors.LinkColor;
                }
            }
        }

        // Event handlers
        private void BrowseFolder_Button_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog folderDialog = new();
            folderDialog.Description = "Select the folder containing existing translation files";
            folderDialog.ShowNewFolderButton = true;
            folderDialog.SelectedPath = _referenceFolder;

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                _referenceFolder = folderDialog.SelectedPath;
                UpdateUI();
            }
        }
        private void SelectAll_Button_Click(object sender, EventArgs e)
        {
            foreach (Guna2CustomCheckBox checkBox in _languageCheckBoxes.Values)
            {
                checkBox.Checked = true;
            }
            UpdateUI();
        }
        private void SelectNone_Button_Click(object sender, EventArgs e)
        {
            foreach (Guna2CustomCheckBox checkBox in _languageCheckBoxes.Values)
            {
                checkBox.Checked = false;
            }
            UpdateUI();
        }
        private void SelectMissing_Button_Click(object sender, EventArgs e)
        {
            // Select only languages that don't exist or are empty
            foreach (KeyValuePair<string, Guna2CustomCheckBox> kvp in _languageCheckBoxes)
            {
                string languageCode = kvp.Key;
                string filePath = Path.Combine(_referenceFolder, $"{languageCode}.json");

                bool shouldSelect = false;

                if (!File.Exists(filePath))
                {
                    shouldSelect = true;
                }
                else
                {
                    try
                    {
                        string content = File.ReadAllText(filePath);
                        Dictionary<string, string>? translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                        if (translations == null || translations.Count == 0)
                        {
                            shouldSelect = true;
                        }
                    }
                    catch
                    {
                        shouldSelect = true;  // Corrupted file
                    }
                }

                kvp.Value.Checked = shouldSelect;
            }
            UpdateUI();
        }
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        private async void Generate_Button_Click(object sender, EventArgs e)
        {
            List<string> selectedLanguages = _languageCheckBoxes
                .Where(kv => kv.Value.Checked)
                .Select(kv => kv.Key)
                .ToList();

            if (selectedLanguages.Count == 0)
            {
                CustomMessageBox.Show("No Languages Selected",
                    "Please select at least one language to generate translations for.",
                    CustomMessageBoxIcon.Exclamation, CustomMessageBoxButtons.Ok);
                return;
            }

            // Validate folder exists
            if (!Directory.Exists(_referenceFolder))
            {
                CustomMessageBox.Show("Invalid Folder",
                    "The selected folder does not exist. Please choose a valid folder.",
                    CustomMessageBoxIcon.Exclamation, CustomMessageBoxButtons.Ok);
                return;
            }

            // Check for internet connection for non-English languages
            if (selectedLanguages.Any(lang => lang != "en"))
            {
                if (!await InternetConnectionManager.CheckInternetAndShowMessageAsync("generating translations", true))
                {
                    Log.Write(1, "Translation generation cancelled - no internet connection");
                    return;
                }
            }

            TranslationProgress_Form progressForm = new();
            Tools.OpenForm(progressForm);

            try
            {
                TranslationResult result = await TranslationGenerator.GenerateSelectedLanguageTranslationFiles(
                    selectedLanguages, _referenceFolder, progressForm, progressForm.CancellationToken);

                if (result.Success && !progressForm.IsCancelled)
                {
                    CustomMessageBox.Show("Success",
                        $"Translation files generated successfully in:\n{_referenceFolder}\n" +
                        $"Summary:\n" +
                        $"• Total source texts: {result.TotalSourceTexts:N0}\n" +
                        $"• New translations created: {result.TotalNewTranslations:N0}\n" +
                        $"• Languages processed: {result.LanguagesProcessed}",
                        CustomMessageBoxIcon.Success, CustomMessageBoxButtons.Ok);

                    // Refresh status after completion
                    UpdateLanguageStatus();

                    DialogResult = DialogResult.OK;
                }
                else if (progressForm.IsCancelled)
                {
                    DialogResult = DialogResult.Cancel;
                }
                else
                {
                    CustomMessageBox.Show("Error", "Translation generation failed. Check logs for details.",
                        CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok);
                }
            }
            catch
            {
                CustomMessageBox.ShowWithFormat("Error", "Error generating translations.",
                    CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok);
            }
            finally
            {
                if (!progressForm.IsDisposed)
                {
                    progressForm.Close();
                }
            }
        }
    }
}
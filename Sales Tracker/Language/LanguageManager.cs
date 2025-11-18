using Guna.UI2.WinForms;
using Newtonsoft.Json;
using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.GridView;
using Sales_Tracker.ReportGenerator;
using Sales_Tracker.ReportGenerator.Menus;
using Sales_Tracker.Settings.Menus;
using Sales_Tracker.UI;
using System.Text.RegularExpressions;

namespace Sales_Tracker.Language
{
    /// <summary>
    /// Manages language translation and caching for user interface controls.
    /// Downloads pre-translated JSON files from the server and applies them to UI controls.
    /// </summary>
    public partial class LanguageManager
    {
        // Properties
        private static readonly HttpClient _httpClient = new();
        private static readonly Dictionary<string, Rectangle> _controlBoundsCache = [];
        private static readonly Dictionary<Control, float> _originalFontSizes = [];
        private static readonly Dictionary<string, Size> _formSizeCache = [];

        // Constants
        private static readonly string
            _placeholder_text = "Placeholder",
            _item_text = "Item",
            _column_text = "Column",
            _before_text = "before",
            _link_text = "link",
            _after_text = "after",
            _full_text = "full";

        [GeneratedRegex(@"[^\w{}]")]
        private static partial Regex NonWordCharactersRegex();

        private static readonly Regex _nonWordCharactersRegex = NonWordCharactersRegex();

        // Getters and setters
        public static Dictionary<string, Dictionary<string, string>> TranslationCache { get; set; }
        public static Dictionary<string, string> EnglishCache { get; set; }

        // Init.
        /// <summary>
        /// Initializes the LanguageManager by loading cached translation data from the translations file.
        /// </summary>
        public static void InitLanguageManager()
        {
            TranslationCache = [];
            EnglishCache = [];

            // Load non-English translation cache
            if (File.Exists(Directories.Translations_file))
            {
                string cacheContent = File.ReadAllText(Directories.Translations_file);

                if (!string.IsNullOrWhiteSpace(cacheContent))
                {
                    Dictionary<string, Dictionary<string, string>> translations = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(cacheContent);

                    if (translations != null)
                    {
                        TranslationCache = translations;
                    }
                }
            }

            // Load English translations from separate file into separate cache
            if (File.Exists(Directories.English_file))
            {
                string englishContent = File.ReadAllText(Directories.English_file);

                if (!string.IsNullOrWhiteSpace(englishContent))
                {
                    Dictionary<string, string> englishTranslations = JsonConvert.DeserializeObject<Dictionary<string, string>>(englishContent);

                    if (englishTranslations != null && englishTranslations.Count > 0)
                    {
                        EnglishCache = englishTranslations;
                    }
                }
            }
        }

        /// <summary>
        /// Downloads and merges language JSON for the specified language.
        /// </summary>
        /// <returns>True if successful, false if failed or skipped.</returns>
        public static async Task<bool> DownloadAndMergeLanguageJson(string languageName, bool overwrite, CancellationToken cancellationToken = default)
        {
            string languageAbbreviation = GetLanguages().FirstOrDefault(l => l.Key == languageName).Value;

            if (string.IsNullOrEmpty(languageAbbreviation))
            {
                Log.WriteWithFormat(1, "Skipping download for language: {0}", languageName);
                return false;
            }

            // Check if language already exists in cache (or English cache for English)
            if (!overwrite)
            {
                if (languageAbbreviation == "en")
                {
                    if (EnglishCache.Count > 0)
                    {
                        Log.WriteWithFormat(1, "Found English language in cache");
                        return true;
                    }
                }
                else if (TranslationCache.ContainsKey(languageAbbreviation))
                {
                    Log.WriteWithFormat(1, "Found language '{0}' in cache", languageName);
                    return true;  // Consider cached language as success
                }
            }

            // Check for internet connection
            if (!await InternetConnectionManager.CheckInternetAndShowMessageAsync("translating language", true))
            {
                Log.Write(1, "Translating language cancelled - no internet connection");
                return false;
            }

            try
            {
                string currentVersion = Tools.GetVersionNumber();
                string downloadUrl = $"https://argorobots.com/resources/downloads/versions/{currentVersion}/languages/{languageAbbreviation}.json";

                HttpResponseMessage response = await _httpClient.GetAsync(downloadUrl, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    Log.Error_GetTranslation("Failed to download language file");
                    return false;
                }

                string jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Dictionary<string, string> downloadedTranslations = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);

                if (downloadedTranslations == null || downloadedTranslations.Count == 0)
                {
                    Log.Write(1, "Downloaded translations are empty or invalid");
                    return false;
                }

                // Handle English separately
                if (languageAbbreviation == "en")
                {
                    // Save to dedicated English file
                    Directories.WriteTextToFile(Directories.English_file, jsonContent);

                    // Merge into English cache
                    foreach (KeyValuePair<string, string> kvp in downloadedTranslations)
                    {
                        EnglishCache[kvp.Key] = kvp.Value;
                    }
                }
                else
                {
                    // Merge downloaded translations with existing cache for other languages
                    if (!TranslationCache.TryGetValue(languageAbbreviation, out Dictionary<string, string> existingTranslations))
                    {
                        existingTranslations = [];
                        TranslationCache[languageAbbreviation] = existingTranslations;
                    }

                    // Merge translations, giving priority to downloaded translations
                    foreach (KeyValuePair<string, string> kvp in downloadedTranslations)
                    {
                        existingTranslations[kvp.Key] = kvp.Value;
                    }
                }

                SaveCacheToFile();
                Log.WriteWithFormat(1, "Successfully merged translations for {0}", languageName);
                return true;
            }
            catch
            {
                Log.Error_GetTranslation("Failed to download language file");
                return false;
            }
        }

        /// <summary>
        /// Updates the application's language translation by downloading and merging the language JSON.
        /// </summary>
        /// <returns>True if translation was successful, false if failed (e.g., no internet).</returns>
        public static async Task<bool> UpdateApplicationLanguage(string targetLanguageName, CancellationToken cancellationToken = default)
        {
            bool downloadSuccess = await DownloadAndMergeLanguageJson(targetLanguageName, false, cancellationToken);

            if (downloadSuccess)
            {
                await ApplyTranslations(targetLanguageName, cancellationToken);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Update the controls text with the cached translated language. It also updates all the child controls.
        /// </summary>
        public static void UpdateLanguageForControl(Control control)
        {
            TranslateAllTextInControlFromCache(control, GetDefaultLanguageAbbreviation());
        }

        /// <summary>
        /// Applies cached translations to all application forms and controls.
        /// </summary>
        private static async Task ApplyTranslations(string targetLanguageName, CancellationToken cancellationToken = default)
        {
            string targetLanguageAbbreviation = GetDefaultLanguageAbbreviation(targetLanguageName);
            if (targetLanguageAbbreviation == null) { return; }

            Log.WriteWithFormat(2, "Translating application language to {0}...", targetLanguageName);

            List<Control> controlsList = [MainMenu_Form.Instance];

            // Add all currently open forms
            foreach (Form openForm in Application.OpenForms.Cast<Form>())
            {
                if (openForm.IsDisposed || openForm.Disposing)
                {
                    continue;
                }
                controlsList.Add(openForm);
            }

            // Add UI panels
            List<Control> panelsList = MainMenu_Form.GetMenus().Cast<Control>().ToList();
            controlsList.AddRange(panelsList);

            // Add other controls
            controlsList.Add(CustomControls.ControlsDropDown_Button);
            controlsList.AddRange(MainMenu_Form.DateRangePanel);
            controlsList.AddRange(MainMenu_Form.Instance.GetAnalyticsControls());
            controlsList.AddRange(MainMenu_Form.Instance.GetMainControls());

            // Apply translations to all controls on UI thread
            List<Task> updateTasks = [];

            foreach (Control control in controlsList)
            {
                if (control == null) { continue; }

                cancellationToken.ThrowIfCancellationRequested();

                Tools.EnsureHandleCreated(control);

                TaskCompletionSource<bool> tcs = new();

                control.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        TranslateAllTextInControlFromCache(control, targetLanguageAbbreviation);
                        tcs.SetResult(true);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                }));

                updateTasks.Add(tcs.Task);
            }

            // Wait for all UI updates to complete
            await Task.WhenAll(updateTasks);

            // Final UI updates
            if (Tools.IsFormOpen<ReportGenerator_Form>())
            {
                ReportDataSelection_Form.Instance.BeginInvoke(new Action(ReportDataSelection_Form.Instance.UpdateLanguageForChartSelectionControl));
                ReportGenerator_Form.Instance.BeginInvoke(new Action(ReportGenerator_Form.AlignControlsAfterLanguageChange));
            }

            if (Tools.IsFormOpen<ReportLayoutDesigner_Form>())
            {
                ReportLayoutDesigner_Form.Instance.BeginInvoke(new Action(ReportLayoutDesigner_Form.Instance.RefreshPropertyPanelTranslations));
            }

            if (Tools.IsFormOpen<Log_Form>())
            {
                Log_Form.Instance.BeginInvoke(new Action(Log_Form.Instance.SetLogColoringAndTranslate));
            }

            if (Tools.IsFormOpen<General_Form>())
            {
                General_Form.Instance.BeginInvoke(new Action(General_Form.Instance.PopulateThemeComboBox));
                General_Form.Instance.BeginInvoke(new Action(General_Form.Instance.AlignLabels));
            }

            if (Tools.IsFormOpen<Security_Form>())
            {
                Security_Form.Instance.BeginInvoke(new Action(Security_Form.Instance.SetPasswordButton));
                Security_Form.Instance.BeginInvoke(new Action(Security_Form.Instance.CenterEncryptControls));
            }

            if (Tools.IsFormOpen<Products_Form>())
            {
                Products_Form.Instance.BeginInvoke(new Action(Products_Form.Instance.PopulateTypeComboBox));
                Products_Form.Instance.BeginInvoke(new Action(Products_Form.Instance.AdjustRadioButtonPositions));
            }

            if (Tools.IsFormOpen<Categories_Form>())
            {
                Categories_Form.Instance.BeginInvoke(new Action(Categories_Form.Instance.AdjustRadioButtonPositions));
            }

            if (Tools.IsFormOpen<AddPurchase_Form>())
            {
                AddPurchase_Form.Instance.BeginInvoke(new Action(AddPurchase_Form.Instance.RecalculateMultipleItemsLayout));
            }

            if (Tools.IsFormOpen<AddSale_Form>())
            {
                AddSale_Form.Instance.BeginInvoke(new Action(AddSale_Form.Instance.RecalculateMultipleItemsLayout));
            }

            if (Tools.IsFormOpen<Upgrade_Form>())
            {
                Upgrade_Form.Instance.BeginInvoke(new Action(Upgrade_Form.Instance.CenterLabels));
            }

            MainMenu_Form.Instance.BeginInvoke(new Action(MainMenu_Form.Instance.CenterAndResizeControls));
            MainMenu_Form.Instance.BeginInvoke(new Action(MainMenu_Form.Instance.RefreshDataGridViewAndCharts));
            MainMenu_Form.Instance.BeginInvoke(new Action(MainMenu_Form.Instance.RecalculateWorldMapControlsLayout));

            DateRange_Form.Instance?.BeginInvoke(new Action(DateRange_Form.Instance.AdjustLabels));

            // Update all custom tooltips with the new language
            CustomTooltip.UpdateAllToolTipTranslations(targetLanguageAbbreviation);
        }

        /// <summary>
        /// Translates all text in control using only cached translations.
        /// </summary>
        private static void TranslateAllTextInControlFromCache(Control control, string targetLanguageAbbreviation)
        {
            CacheControlBounds(control);

            foreach (Control childControl in control.Controls)
            {
                TranslateAllTextInControlFromCache(childControl, targetLanguageAbbreviation);
            }

            if (control.AccessibleDescription == AccessibleDescriptionManager.DoNotTranslate)
            {
                return;
            }

            switch (control)
            {
                case LinkLabel linkLabel:
                    TranslateLinkLabelFromCache(linkLabel, targetLanguageAbbreviation);
                    AdjustLabelPosition(linkLabel);
                    break;

                case Label label:
                    string controlKey = GetControlKey(label);
                    string translatedLabelText = GetCachedTranslationByControlKey(targetLanguageAbbreviation, controlKey, label.Text);
                    label.InvokeIfRequired(() =>
                    {
                        label.Text = translatedLabelText;
                        AdjustLabelPosition(label);
                    });
                    break;

                case Guna2Button guna2Button:
                    string buttonKey = GetControlKey(guna2Button);
                    string translatedButtonText = GetCachedTranslationByControlKey(targetLanguageAbbreviation, buttonKey, guna2Button.Text);
                    guna2Button.InvokeIfRequired(() =>
                    {
                        AdjustButtonFontSize(guna2Button, translatedButtonText);
                    });
                    break;

                case RichTextBox textBox:
                    string textBoxKey = GetControlKey(textBox);
                    textBox.Text = GetCachedTranslationByControlKey(targetLanguageAbbreviation, textBoxKey, textBox.Text);
                    break;

                case Guna2GroupBox guna2GroupBox:
                    string guna2GroupBoxKey = GetControlKey(guna2GroupBox);
                    guna2GroupBox.Text = GetCachedTranslationByControlKey(targetLanguageAbbreviation, guna2GroupBoxKey, guna2GroupBox.Text);
                    break;

                case Guna2TextBox guna2TextBox:
                    string guna2TextBoxKey = GetControlKey(guna2TextBox);
                    guna2TextBox.Text = GetCachedTranslationByControlKey(targetLanguageAbbreviation, guna2TextBoxKey, guna2TextBox.Text);
                    guna2TextBox.PlaceholderText = GetCachedTranslationByControlKey(targetLanguageAbbreviation, $"{guna2TextBoxKey}_{_placeholder_text}", guna2TextBox.PlaceholderText);
                    break;

                case Guna2ComboBox guna2ComboBox:
                    if (guna2ComboBox.DataSource == null)
                    {
                        int selectedIndex = guna2ComboBox.SelectedIndex;
                        List<object> translatedItems = [];
                        string comboBoxKey = GetControlKey(guna2ComboBox);

                        for (int i = 0; i < guna2ComboBox.Items.Count; i++)
                        {
                            string originalText = guna2ComboBox.Items[i].ToString();
                            string itemKey = $"{comboBoxKey}_{_item_text}_{i}";
                            string translatedText = GetCachedTranslationByControlKey(targetLanguageAbbreviation, itemKey, originalText);
                            translatedItems.Add(translatedText);
                        }
                        guna2ComboBox.Items.Clear();
                        guna2ComboBox.Items.AddRange(translatedItems.ToArray());
                        if (selectedIndex >= 0 && selectedIndex < guna2ComboBox.Items.Count)
                        {
                            guna2ComboBox.SelectedIndex = selectedIndex;
                        }
                    }
                    break;

                case Guna2DataGridView gunaDataGridView:
                    string dataGridKey = GetControlKey(gunaDataGridView);
                    foreach (DataGridViewColumn column in gunaDataGridView.Columns)
                    {
                        string columnKey = $"{dataGridKey}_{_column_text}_{column.Name}";
                        if (column.HeaderCell is DataGridViewImageHeaderCell imageHeaderCell)
                        {
                            string translatedHeaderText = GetCachedTranslationByControlKey(targetLanguageAbbreviation, columnKey, imageHeaderCell.HeaderText);
                            imageHeaderCell.HeaderText = translatedHeaderText;
                        }
                        else
                        {
                            column.HeaderText = GetCachedTranslationByControlKey(targetLanguageAbbreviation, columnKey, column.HeaderText);
                        }
                    }
                    gunaDataGridView.Refresh();
                    break;
            }
        }

        /// <summary>
        /// Gets cached translation using control-based key with fallback to original text.
        /// </summary>
        private static string GetCachedTranslationByControlKey(string targetLanguageAbbreviation, string controlKey, string originalText)
        {
            if (string.IsNullOrEmpty(originalText))
            {
                return originalText;
            }

            // Handle English separately
            if (targetLanguageAbbreviation == "en")
            {
                if (EnglishCache.TryGetValue(controlKey, out string cachedTranslation))
                {
                    return cachedTranslation;
                }
            }
            else
            {
                // Handle other languages
                if (TranslationCache.TryGetValue(targetLanguageAbbreviation, out Dictionary<string, string>? controlTranslations))
                {
                    if (controlTranslations.TryGetValue(controlKey, out string cachedTranslation))
                    {
                        return cachedTranslation;
                    }
                }
            }

            // If no cached translation is found, return original
            return originalText;
        }

        /// <summary>
        /// Gets cached translation using text content as the key basis.
        /// </summary>
        public static string GetCachedTranslationByText(string targetLanguageAbbreviation, string originalText)
        {
            if (string.IsNullOrEmpty(originalText))
            {
                return originalText;
            }

            // Generate key from the text content
            string textKey = GetStringKey(originalText);

            // Handle English separately
            if (targetLanguageAbbreviation == "en")
            {
                if (EnglishCache.TryGetValue(textKey, out string cachedTranslation))
                {
                    return cachedTranslation;
                }
            }
            else
            {
                // Handle other languages
                if (TranslationCache.TryGetValue(targetLanguageAbbreviation, out Dictionary<string, string>? controlTranslations))
                {
                    if (controlTranslations.TryGetValue(textKey, out string cachedTranslation))
                    {
                        return cachedTranslation;
                    }
                }
            }

            // If no cached translation is found, return original
            return originalText;
        }
        private static void TranslateLinkLabelFromCache(LinkLabel linkLabel, string targetLanguageAbbreviation)
        {
            string fullText = linkLabel.Text.Replace("\r\n", "\n");
            int linkStart = linkLabel.LinkArea.Start;
            int linkLength = linkLabel.LinkArea.Length;

            if (linkLength > 0)
            {
                string textBeforeLink = fullText.Substring(0, linkStart).Trim();
                string linkText = fullText.Substring(linkStart, linkLength).Trim();
                string textAfterLink = fullText.Substring(linkStart + linkLength).Trim();

                string controlKey = GetControlKey(linkLabel);
                string translatedTextBefore = GetCachedTranslationByControlKey(targetLanguageAbbreviation, $"{controlKey}_{_before_text}", textBeforeLink);
                string translatedLink = GetCachedTranslationByControlKey(targetLanguageAbbreviation, $"{controlKey}_{_link_text}", linkText);
                string translatedTextAfter = GetCachedTranslationByControlKey(targetLanguageAbbreviation, $"{controlKey}_{_after_text}", textAfterLink);

                bool hasNewLineBefore = fullText.Substring(0, linkStart).EndsWith('\n');
                string finalText = (hasNewLineBefore ? translatedTextBefore + "\n" : translatedTextBefore) + " " +
                    translatedLink + " " + translatedTextAfter;

                linkLabel.Text = finalText;
                linkLabel.LinkArea = new LinkArea(translatedTextBefore.Length + 1, translatedLink.Length + 1);
            }
            else
            {
                string controlKey = GetControlKey(linkLabel);
                string translatedFullText = GetCachedTranslationByControlKey(targetLanguageAbbreviation, $"{controlKey}_{_full_text}", fullText.Trim());
                linkLabel.Text = translatedFullText;
            }
        }

        /// <summary>
        /// Translates a string using cached translations with text-based keys.
        /// The string also needs to be added to TranslationGenerator.CollectStringsToTranslate().
        /// </summary>
        public static string TranslateString(string text, string targetLanguageAbbreviation = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            targetLanguageAbbreviation ??= GetDefaultLanguageAbbreviation();
            if (targetLanguageAbbreviation == "en")
            {
                return text;
            }

            return GetCachedTranslationByText(targetLanguageAbbreviation, text);
        }

        /// <summary>
        /// Downloads and updates all existing translations after an application upgrade.
        /// </summary>
        /// <returns>True if all updates were successful, false if any failed</returns>
        public static async Task<bool> UpdateExistingTranslationsAfterUpgrade(CancellationToken cancellationToken = default)
        {
            try
            {
                // Get list of all languages that are currently cached/downloaded
                HashSet<string> languagesToUpdate = [];

                // Add languages from TranslationCache (non-English translations)
                if (TranslationCache != null)
                {
                    foreach (string languageCode in TranslationCache.Keys)
                    {
                        // Find the language name by abbreviation
                        KeyValuePair<string, string> languageInfo = GetLanguages().FirstOrDefault(l => l.Value == languageCode);
                        if (!string.IsNullOrEmpty(languageInfo.Key))
                        {
                            languagesToUpdate.Add(languageInfo.Key);
                        }
                    }
                }

                // Add English if it exists in cache
                if (EnglishCache != null && EnglishCache.Count > 0)
                {
                    languagesToUpdate.Add("English");
                }

                if (languagesToUpdate.Count == 0)
                {
                    Log.Write(1, "No existing translations found to update");
                    return true;
                }

                Log.WriteWithFormat(1, "Found {0} languages to update: {1}",
                    languagesToUpdate.Count, string.Join(", ", languagesToUpdate));

                // Check internet connection before attempting downloads
                if (!await InternetConnectionManager.CheckInternetAndShowMessageAsync("updating translations", true))
                {
                    Log.Write(1, "Translation update cancelled - no internet connection");
                    return false;
                }

                int successCount = 0;
                int totalCount = languagesToUpdate.Count;

                // Download updates for each language
                foreach (string languageName in languagesToUpdate)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Log.Write(1, "Translation update cancelled by user");
                        return false;
                    }

                    bool success = await DownloadAndMergeLanguageJson(languageName, true, cancellationToken);

                    if (success)
                    {
                        successCount++;
                    }
                    else
                    {
                        Log.WriteWithFormat(0, "Failed to update translations for {0}", languageName);
                    }
                }

                return successCount == totalCount;
            }
            catch (Exception ex)
            {
                Log.WriteWithFormat(0, "Error during bulk translation update: {0}", ex.Message);
                return false;
            }
        }

        // Helper methods for UI adjustments
        /// <summary>
        /// Adjusts label position based on original bounds and current form size.
        /// Accounts for form resizing by calculating scale factors.
        /// </summary>
        private static void AdjustLabelPosition(Label label)
        {
            string controlKey = GetControlKey(label);

            if (!_controlBoundsCache.TryGetValue(controlKey, out Rectangle originalBounds))
            {
                return;
            }

            // Find the top-level form containing this label
            Form parentForm = label.FindForm();
            if (parentForm == null) { return; }

            string formKey = GetFormKey(parentForm);

            // Get the original form size when bounds were cached
            if (!_formSizeCache.TryGetValue(formKey, out Size originalFormSize))
            {
                return;
            }

            // Calculate scaling factors based on form size changes
            double scaleX = (double)parentForm.ClientSize.Width / originalFormSize.Width;
            double scaleY = (double)parentForm.ClientSize.Height / originalFormSize.Height;

            // Scale the original bounds to match current form size
            Rectangle scaledBounds = new(
                (int)(originalBounds.X * scaleX),
                (int)(originalBounds.Y * scaleY),
                (int)(originalBounds.Width * scaleX),
                (int)(originalBounds.Height * scaleY)
            );

            // Apply positioning based on alignment
            if (label.AccessibleDescription == AccessibleDescriptionManager.AlignRight)
            {
                label.Left = scaledBounds.Right - label.Width;
            }
            else if (label.AccessibleDescription != AccessibleDescriptionManager.AlignLeft
                && !label.Anchor.HasFlag(AnchorStyles.Left))
            {
                // Center alignment
                int scaledCenterX = scaledBounds.Left + scaledBounds.Width / 2;
                label.Left = scaledCenterX - label.Width / 2;
            }
            // For left alignment or anchored left, no adjustment is needed as the form handles it
        }
        public static void AdjustButtonFontSize(Guna2Button button, string text)
        {
            if (!_originalFontSizes.ContainsKey(button))
            {
                _originalFontSizes[button] = button.Font.Size;
            }

            float originalFontSize = _originalFontSizes[button];
            float minFontSize = 3.0f;
            float maxFontSize = originalFontSize;

            while (maxFontSize - minFontSize > 0.5f)
            {
                float fontSize = (minFontSize + maxFontSize) / 2;
                button.Font = new Font(button.Font.FontFamily, fontSize, button.Font.Style);
                button.Text = text;

                button.PerformLayout();
                Size preferredSize = button.PreferredSize;

                if (preferredSize.Width <= button.Width && preferredSize.Height <= button.Height)
                {
                    minFontSize = fontSize;  // Try a larger font
                }
                else
                {
                    maxFontSize = fontSize;  // Try a smaller font
                }
            }

            // Set the final font and text
            float finalFontSize = Math.Max(minFontSize, 3.0f);
            button.Font = new Font(button.Font.FontFamily, finalFontSize, button.Font.Style);
            button.Text = text;
        }
        private static void CacheControlBounds(Control control)
        {
            if (control is not Label)
            {
                return;
            }

            // Skip caching for controls marked as DoNotCache
            if (control.AccessibleDescription == AccessibleDescriptionManager.DoNotCache)
            {
                return;
            }

            string controlKey = GetControlKey(control);
            if (!_controlBoundsCache.ContainsKey(controlKey))
            {
                _controlBoundsCache[controlKey] = control.Bounds;

                // Also cache the form size
                Form parentForm = control.FindForm();
                if (parentForm != null)
                {
                    string formKey = GetFormKey(parentForm);
                    if (!_formSizeCache.ContainsKey(formKey))
                    {
                        _formSizeCache[formKey] = parentForm.ClientSize;
                    }
                }
            }
        }
        private static void SaveCacheToFile()
        {
            try
            {
                string jsonContent = JsonConvert.SerializeObject(TranslationCache, Formatting.Indented);
                Directories.WriteTextToFile(Directories.Translations_file, jsonContent);
            }
            catch (Exception ex)
            {
                Log.Error_SaveTranslationCache(ex.Message);
            }
        }

        // Misc. methods
        public static List<SearchResult> GetLanguageSearchResults()
        {
            List<KeyValuePair<string, string>> allLanguages = GetLanguages();
            List<SearchResult> searchResults = [];

            // Priority languages that appear at the top
            HashSet<string> priorityLanguages =
            [
                "English",
                "French",
                "German",
                "Italian"
            ];

            // Add priority languages first
            foreach (string lang in priorityLanguages)
            {
                KeyValuePair<string, string> language = allLanguages.First(l => l.Key == lang);
                searchResults.Add(new SearchResult(language.Key, null, 0));
            }

            // Add line
            searchResults.Add(new SearchResult(SearchBox.AddLine, null, 0));

            // Add remaining languages
            IEnumerable<KeyValuePair<string, string>> remainingLanguages = allLanguages
                .Where(l => !priorityLanguages.Contains(l.Key));

            foreach (KeyValuePair<string, string> language in remainingLanguages)
            {
                searchResults.Add(new SearchResult(language.Key, null, 0));
            }

            return searchResults;
        }

        /// <summary>
        /// Gets a list of supported languages with their corresponding ISO language codes, sorted alphabetically.
        /// </summary>
        public static List<KeyValuePair<string, string>> GetLanguages()
        {
            return
            [
                new("Albanian", "sq"),          // Albania
                new("Arabic", "ar"),            // Middle East, North Africa
                new("Basque", "eu"),            // Basque Country
                new("Belarusian", "be"),        // Belarus
                new("Bengali", "bn"),           // Bangladesh, India
                new("Bosnian", "bs"),           // Bosnia and Herzegovina
                new("Bulgarian", "bg"),         // Bulgaria
                new("Catalan", "ca"),           // Catalonia
                new("Chinese (Simplified)", "zh-Hans"),  // Mainland China
                new("Chinese (Traditional)", "zh-Hant"), // Taiwan, Hong Kong
                new("Croatian", "hr"),          // Croatia
                new("Czech", "cs"),             // Czech Republic
                new("Danish", "da"),            // Denmark
                new("Dutch", "nl"),             // Netherlands, Belgium
                new("English", "en"),           // North America, UK, Australia
                new("Estonian", "et"),          // Estonia
                new("Filipino", "fil"),         // Philippines
                new("Finnish", "fi"),           // Finland
                new("French", "fr"),            // France, Canada, Belgium
                new("Galician", "gl"),          // Galicia
                new("German", "de"),            // Germany, Austria
                new("Greek", "el"),             // Greece
                new("Hebrew", "he"),            // Israel
                new("Hindi", "hi"),             // India
                new("Hungarian", "hu"),         // Hungary
                new("Icelandic", "is"),         // Iceland
                new("Indonesian", "id"),        // Indonesia
                new("Irish", "ga"),             // Ireland
                new("Italian", "it"),           // Italy
                new("Japanese", "ja"),          // Japan
                new("Korean", "ko"),            // South Korea
                new("Latvian", "lv"),           // Latvia
                new("Lithuanian", "lt"),        // Lithuania
                new("Luxembourgish", "lb"),     // Luxembourg
                new("Macedonian", "mk"),        // North Macedonia
                new("Malay", "ms"),             // Malaysia, Brunei
                new("Maltese", "mt"),           // Malta
                new("Norwegian", "no"),         // Norway
                new("Persian", "fa"),           // Iran, Afghanistan
                new("Polish", "pl"),            // Poland
                new("Portuguese", "pt"),        // Portugal, Brazil
                new("Romanian", "ro"),          // Romania
                new("Russian", "ru"),           // Russia, Eastern Europe
                new("Serbian", "sr"),           // Serbia
                new("Slovak", "sk"),            // Slovakia
                new("Slovenian", "sl"),         // Slovenia
                new("Spanish", "es"),           // Spain, Latin America
                new("Swahili", "sw"),           // East Africa
                new("Swedish", "sv"),           // Sweden
                new("Thai", "th"),              // Thailand
                new("Turkish", "tr"),           // Turkey
                new("Ukrainian", "uk"),         // Ukraine
                new("Urdu", "ur"),              // Pakistan, India
                new("Vietnamese", "vi"),        // Vietnam
            ];
        }
        public static string? GetDefaultLanguageAbbreviation(string targetLanguageName = null)
        {
            targetLanguageName ??= Properties.Settings.Default.Language;

            string languageAbbreviation = GetLanguages().FirstOrDefault(l => l.Key == targetLanguageName).Value;

            if (string.IsNullOrEmpty(languageAbbreviation))
            {
                Log.Error_GetTranslation($"Language '{targetLanguageName}' not found in the list.");
                return null;
            }
            else
            {
                return languageAbbreviation;
            }
        }

        // Get keys
        /// <summary>
        /// Gets a unique key for a control.
        /// </summary>
        public static string GetControlKey(Control control, string section = null)
        {
            // Collect parent names from the current control up to the root
            List<string> parentNames = [];

            for (Control currentControl = control; currentControl != null; currentControl = currentControl.Parent)
            {
                if (!string.IsNullOrEmpty(currentControl.Name))
                {
                    parentNames.Add(currentControl.Name);
                }

                // Stop if we encounter a form that is not the current control (a form within a form)
                if (currentControl is Form && currentControl != control)
                {
                    break;
                }
            }

            // Reverse to get top-down order
            parentNames.Reverse();

            // Join the parent names with a period
            string key = string.Join(".", parentNames);

            // Append section (before, link, after) if provided
            if (!string.IsNullOrEmpty(section))
            {
                key += $"_{section}";
            }

            // Handle DateRangePanel special case - Main_Panel is moved from DateRange_Form to MainMenu_Form
            key = key.Replace("MainMenu_Form.Main_Panel", "DateRange_Form.Main_Panel");

            return key;
        }

        /// <summary>
        /// Gets a unique key for a form to cache its size.
        /// </summary>
        private static string GetFormKey(Form form)
        {
            return $"Form_{form.Name ?? form.GetType().Name}";
        }

        /// <summary>
        /// Gets a unique key for a string using text content with shortened prefix.
        /// </summary>
        public static string GetStringKey(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "";
            }

            // Remove non-word characters and use shortened prefix to save space
            string cleanText = _nonWordCharactersRegex.Replace(text.ToLowerInvariant(), "");

            // Limit length to avoid extremely long keys
            if (cleanText.Length > 50)
            {
                cleanText = cleanText[..50];
            }

            return $"str_{cleanText}";
        }
    }
}
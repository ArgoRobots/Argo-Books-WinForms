using Guna.UI2.WinForms;
using Sales_Tracker.Classes;
using Sales_Tracker.Language;
using Sales_Tracker.Settings.Menus;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker.Settings
{
    /// <summary>
    /// Main settings form for application configuration.
    /// </summary>
    public partial class Settings_Form : BaseForm
    {
        // Properties
        private static Settings_Form _instance;
        private readonly Form FormGeneral = new General_Form();
        private readonly Form FormSecurity = new Security_Form();
        private string _originalLanguage;
        private CancellationTokenSource _translationCts = new();
        private bool _isClosing = false;

        // Getters
        public static Settings_Form Instance => _instance;

        // Init.
        public Settings_Form()
        {
            InitializeComponent();
            _instance = this;
            _originalLanguage = Properties.Settings.Default.Language;

            SetChildForm(FormGeneral);
            SetChildForm(FormSecurity);
            UpdateTheme();
            General_Button.PerformClick();
            AnimateButtons();
            LanguageManager.UpdateLanguageForControl(this);
            LoadingPanel.ShowBlankLoadingPanel(this);
            LoadingPanel.CancelRequested += OnTranslationCancelled;

            PanelCloseFilter panelCloseFilter = new(this, ClosePanels,
                SearchBox.SearchResultBoxContainer,
                TextBoxManager.RightClickTextBox_Panel);

            Application.AddMessageFilter(panelCloseFilter);
        }
        private void SetChildForm(Form form)
        {
            form.TopLevel = false;
            form.Visible = true;
            form.Dock = DockStyle.Fill;

            FormBack_Panel.Controls.Add(form);
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);
            ThemeManager.MakeGButtonBluePrimary(Ok_Button);
            ThemeManager.MakeGButtonBlueSecondary(Cancel_Button);
            ThemeManager.MakeGButtonBlueSecondary(Apply_Button);
        }
        public void AnimateButtons()
        {
            IEnumerable<Guna2Button> buttons =
            [
               General_Button,
               Security_Button,
               ResetToDefault_Button,
               Apply_Button,
            ];
            CustomControls.AnimateButtons(buttons);
        }

        // Form event handlers
        private void Settings_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
        }
        private async void Settings_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            _isClosing = true;

            // If there's an active translation, try to cancel it gracefully
            if (_translationCts != null && !_translationCts.Token.IsCancellationRequested)
            {
                try
                {
                    // Cancel the operation
                    _translationCts.Cancel();

                    // Give a brief moment for cancellation to take effect
                    await Task.Delay(100);
                }
                catch (ObjectDisposedException)
                {
                    // Token source was already disposed, that's fine
                }
                catch (Exception ex)
                {
                    Log.Error_WriteToFile($"Error during translation cancellation: {ex.Message}");
                }
            }

            // Clean up event handlers
            LoadingPanel.CancelRequested -= OnTranslationCancelled;

            // Dispose cancellation token
            try
            {
                _translationCts?.Dispose();
            }
            catch (ObjectDisposedException)
            {
                // Already disposed, ignore
            }
        }
        private void OnTranslationCancelled(object sender, EventArgs e)
        {
            if (_isClosing) { return; }

            try
            {
                _translationCts?.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // Token source was already disposed, that's fine
            }

            LoadingPanel.HideLoadingScreen(this);

            // Only show message if form is not closing
            if (!_isClosing && !IsDisposed && IsHandleCreated)
            {
                CustomMessageBox.Show("Translation Cancelled", "The language translation was cancelled.",
                    CustomMessageBoxIcon.Info, CustomMessageBoxButtons.Ok);
            }
        }

        // Left menu buttons
        private Guna2Button _selectedButton;
        private void GeneralButton_Click(object sender, EventArgs e)
        {
            SwitchForm(FormGeneral, sender);
        }
        private void SecurityButton_Click(object sender, EventArgs e)
        {
            SwitchForm(FormSecurity, sender);
        }

        // Bottom buttons
        private async void ResetToDefault_Button_Click(object sender, EventArgs e)
        {
            if (_isClosing) { return; }

            CustomMessageBoxResult result = CustomMessageBox.Show(
                "Reset settings", "All settings will be reset to default.",
                CustomMessageBoxIcon.Question, CustomMessageBoxButtons.OkCancel);

            if (result == CustomMessageBoxResult.Ok && !_isClosing)
            {
                UserSettings.ResetAllToDefault();
                await ApplyChanges();
            }
        }
        private async void Ok_Button_Click(object sender, EventArgs e)
        {
            if (_isClosing) { return; }

            SetButtonsEnabled(false);

            bool success = await ApplyChanges();

            if (!_isClosing)
            {
                SetButtonsEnabled(true);

                if (success)
                {
                    Close();
                }
            }
        }
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            Close();
        }
        private async void Apply_Button_Click(object sender, EventArgs e)
        {
            if (_isClosing) { return; }

            SetButtonsEnabled(false);

            bool success = await ApplyChanges();

            if (!_isClosing)
            {
                SetButtonsEnabled(true);

                if (success && HasLanguageChanged())
                {
                    CustomMessageBox.Show("Translation Complete", "Language has been successfully updated.",
                        CustomMessageBoxIcon.Success, CustomMessageBoxButtons.Ok);
                }
            }
        }

        // Methods
        private void SetButtonsEnabled(bool enabled)
        {
            if (_isClosing || IsDisposed) { return; }

            try
            {
                Ok_Button.Enabled = enabled;
                Apply_Button.Enabled = enabled;
                Cancel_Button.Enabled = enabled;
                ResetToDefault_Button.Enabled = enabled;

                General_Button.Enabled = enabled;
                Security_Button.Enabled = enabled;
            }
            catch (ObjectDisposedException)
            {
                // Form was disposed, ignore
            }
        }

        /// <summary>
        /// Applies all setting changes, including language translation if the language was changed.
        /// Returns true if all changes were successfully applied, false otherwise.
        /// </summary>
        private async Task<bool> ApplyChanges()
        {
            if (_isClosing) { return false; }

            try
            {
                bool success = await UserSettings.SaveUserSettingsAsync();

                if (!success || _isClosing)
                {
                    return false;  // Settings save was cancelled or failed, or form is closing
                }

                if (HasLanguageChanged())
                {
                    // Dispose of previous cancellation token source
                    try
                    {
                        _translationCts?.Cancel();
                        _translationCts?.Dispose();
                    }
                    catch (ObjectDisposedException)
                    {
                        // Already disposed, that's fine
                    }

                    // Check again if form is closing before creating new token
                    if (_isClosing) { return false; }

                    // Create a new cancellation token source
                    _translationCts = new CancellationTokenSource();

                    bool languageSuccess = await UpdateLanguageAsync();
                    if (!languageSuccess || _isClosing)
                    {
                        return false;  // Language update was cancelled or failed, or form is closing
                    }
                }

                return !_isClosing;  // Return false if form closed during operation
            }
            catch (Exception ex)
            {
                Log.Error_WriteToFile($"Error applying settings changes: {ex.Message}");
                return false;
            }
        }
        private async Task<bool> UpdateLanguageAsync()
        {
            if (_isClosing) { return false; }

            try
            {
                if (!IsHandleCreated || IsDisposed || _isClosing)
                {
                    return false;
                }

                LoadingPanel.ShowLoadingScreen(this, "Translating application to new language...", true, _translationCts);

                try
                {
                    string currentLanguage = General_Form.Instance.Language_TextBox.Text;
                    bool success = await LanguageManager.UpdateApplicationLanguage(currentLanguage, _translationCts.Token);

                    if (success && !_translationCts.Token.IsCancellationRequested && !_isClosing)
                    {
                        _originalLanguage = General_Form.Instance.Language_TextBox.Text;
                        UpdateLanguage();

                        // Only hide loading screen if form is still valid
                        if (!IsDisposed && IsHandleCreated && !_isClosing)
                        {
                            LoadingPanel.HideLoadingScreen(this);
                        }
                        return true;
                    }
                    else
                    {
                        // Only hide loading screen if form is still valid
                        if (!IsDisposed && IsHandleCreated && !_isClosing)
                        {
                            LoadingPanel.HideLoadingScreen(this);
                        }
                        return false;  // Translation was cancelled or failed
                    }
                }
                catch (OperationCanceledException)
                {
                    Log.Write(1, "Language translation was cancelled");

                    // Only hide loading screen if form is still valid
                    if (!IsDisposed && IsHandleCreated && !_isClosing)
                    {
                        LoadingPanel.HideLoadingScreen(this);
                    }
                    return false;
                }
            }
            catch (ObjectDisposedException)
            {
                // Form was disposed during operation
                return false;
            }
            catch (Exception ex)
            {
                // Only hide loading screen if form is still valid
                if (!IsDisposed && IsHandleCreated && !_isClosing)
                {
                    LoadingPanel.HideLoadingScreen(this);
                }
                Log.Error_GetTranslation(ex.Message);
                return false;
            }
        }
        private bool HasLanguageChanged()
        {
            if (_isClosing || General_Form.Instance == null) { return false; }

            try
            {
                string currentLanguage = General_Form.Instance.Language_TextBox.Text;
                return !string.Equals(_originalLanguage, currentLanguage, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Updates the application language setting and logs the change.
        /// </summary>
        private static void UpdateLanguage()
        {
            try
            {
                Properties.Settings.Default.Language = General_Form.Instance.Language_TextBox.Text;

                // Remove previous messages that mention language changes
                string message = LanguageManager.TranslateString("Changed the language to");
                MainMenu_Form.SettingsThatHaveChangedInFile.RemoveAll(x => x.Contains(message));

                // Add the new language change message
                string fullMessage = $"{message} {Properties.Settings.Default.Language}";
                CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.SettingsThatHaveChangedInFile, 2, fullMessage);
            }
            catch (Exception ex)
            {
                Log.Error_WriteToFile($"Error updating language setting: {ex.Message}");
            }
        }

        // Misc.
        private void SwitchForm(Form form, object btnSender)
        {
            if (_isClosing) { return; }

            Guna2Button button = (Guna2Button)btnSender;

            // If button is already selected
            if (button.FillColor == CustomColors.AccentBlue)
            {
                return;
            }

            // Unselect button
            if (_selectedButton != null)
            {
                _selectedButton.FillColor = CustomColors.ControlBack;
                if (ThemeManager.IsDarkTheme())
                {
                    _selectedButton.ForeColor = Color.White;
                }
                else
                {
                    _selectedButton.ForeColor = Color.Black;
                }
            }

            // Select new button
            button.FillColor = CustomColors.AccentBlue;
            button.ForeColor = Color.White;

            // Show form
            form.BringToFront();

            // Save
            _selectedButton = button;
        }
        private void ClosePanels()
        {
            TextBoxManager.HideRightClickPanel();
            SearchBox.Close();
        }
    }
}
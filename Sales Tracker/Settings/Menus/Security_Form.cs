using Guna.UI2.WinForms;
using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.Language;
using Sales_Tracker.Passwords;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker.Settings.Menus
{
    /// <summary>
    /// Security settings form for password and encryption configuration.
    /// </summary>
    public partial class Security_Form : BaseForm
    {
        // Properties
        private static Security_Form _instance;
        private readonly bool _isFormLoading;
        private Guna2WinProgressIndicator _progressIndicator;

        // Getters
        public static Security_Form Instance => _instance;

        // Init.
        public Security_Form()
        {
            InitializeComponent();
            _instance = this;

            _isFormLoading = true;
            UpdateControls();
            _isFormLoading = false;

            SetPasswordButton();
            UpdateTheme();
            SetAccessibleDescriptions();
            AlignLabels();
            LanguageManager.UpdateLanguageForControl(this);
            CenterEncryptControls();
            LoadingPanel.ShowBlankLoadingPanel(this);
            SetWindowsHelloButton();
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);
        }
        private void SetAccessibleDescriptions()
        {
            AddPassword_Button.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
        }
        private void AlignLabels()
        {
            EncryptFiles_Label.Left = EncryptFiles_CheckBox.Left - EncryptFiles_Label.Width - 2;
        }
        public void CenterEncryptControls()
        {
            int totalWidth = EncryptFiles_Label.Width + EncryptFiles_CheckBox.Width - 2;
            int startX = (ClientSize.Width - totalWidth) / 2;

            EncryptFiles_Label.Left = startX;
            EncryptFiles_CheckBox.Left = EncryptFiles_Label.Left + EncryptFiles_Label.Width - 2;
        }
        private void SetWindowsHelloButton()
        {
            if (!PasswordManager.HasPassword)
            {
                Controls.Remove(EnableWindowsHello_Button);
            }

            if (Properties.Settings.Default.WindowsHelloEnabled)
            {
                EnableWindowsHello_Button.Tag = "Enabled";
                string newText = LanguageManager.TranslateString("Disable Windows Hello");
                EnableWindowsHello_Button.Text = newText;
            }
            else
            {
                EnableWindowsHello_Button.Tag = "Disabled";
                string newText = LanguageManager.TranslateString("Enable Windows Hello");
                EnableWindowsHello_Button.Text = newText;
            }
        }

        // Form event handlers
        private void Security_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
        }

        // Event handlers
        private void EncryptFiles_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_isFormLoading) { return; }

            if (EncryptFiles_CheckBox.Checked) { return; }

            CustomMessageBoxResult result = CustomMessageBox.Show(
                "Unencrypt files",
                "Disabling this feature will make your data vulnerable to unauthorized access. Are you sure you want to disable this feature?",
                CustomMessageBoxIcon.Exclamation, CustomMessageBoxButtons.YesNo);

            if (result != CustomMessageBoxResult.Yes)
            {
                EncryptFiles_CheckBox.Checked = true;
            }
        }
        private void EncryptFiles_Label_Click(object sender, EventArgs e)
        {
            EncryptFiles_CheckBox.Checked = !EncryptFiles_CheckBox.Checked;
        }
        private void AddPassword_Button_Click(object sender, EventArgs e)
        {
            if (!PasswordManager.HasPassword)
            {
                Tools.OpenForm(new AddPassword_Form());
            }
            else
            {
                Tools.OpenForm(new PasswordManager_Form());
            }
            SetPasswordButton();
        }
        private async void EnableWindowsHello_Button_Click(object sender, EventArgs e)
        {
            bool result = false;

            if (EnableWindowsHello_Button.Tag.ToString() == "Enabled")
            {
                if (PasswordManager.EnterPassword(false))
                {
                    Properties.Settings.Default.WindowsHelloEnabled = false;

                    CustomMessageBox.Show("Windows Hello disabled", "Windows Hello has been disabled.",
                        CustomMessageBoxIcon.Success, CustomMessageBoxButtons.Ok);

                    string newText = LanguageManager.TranslateString("Enable Windows Hello");
                    EnableWindowsHello_Button.Text = newText;
                    EnableWindowsHello_Button.Tag = "Disabled";
                }
            }
            else
            {
                AddPassword_Button.Enabled = false;
                EnableWindowsHello_Button.Enabled = false;

                if (PasswordManager.EnterPassword(false))
                {
                    PlayLoadingAnimation();

                    result = await EnterPassword_Form.RunWindowsHello();
                    if (result)
                    {
                        Properties.Settings.Default.WindowsHelloEnabled = true;

                        string message = $"Changed the 'Windows Hello' setting";
                        CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.SettingsThatHaveChangedInFile, 2, message);

                        StopPlayingLoadingAnimation();

                        CustomMessageBox.Show("Windows Hello enabled",
                            "Windows Hello has been enabled. You can now log in with your biometrics.",
                            CustomMessageBoxIcon.Success, CustomMessageBoxButtons.Ok);

                        string newText = LanguageManager.TranslateString("Disable Windows Hello");
                        EnableWindowsHello_Button.Text = newText;
                        EnableWindowsHello_Button.Tag = "Enabled";
                    }
                    else
                    {
                        CustomMessageBox.Show("Windows Hello error",
                            "Windows Hello failed to initiate. Please try again or contact support.",
                            CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok);
                    }
                }

                AddPassword_Button.Enabled = true;
                EnableWindowsHello_Button.Enabled = true;
            }

            UserSettings.UpdateSetting("Windows Hello", Properties.Settings.Default.WindowsHelloEnabled, result,
                   value => Properties.Settings.Default.WindowsHelloEnabled = value);
        }
        private void PlayLoadingAnimation()
        {
            _progressIndicator = new()
            {
                AutoStart = true,
                ProgressColor = CustomColors.AccentBlue,
            };
            _progressIndicator.Location = new Point(
                (Width - _progressIndicator.Width) / 2,
                EnableWindowsHello_Button.Bottom + 50
            );
            Controls.Add(_progressIndicator);
        }
        private void StopPlayingLoadingAnimation()
        {
            Controls.Remove(_progressIndicator);
            _progressIndicator = null;
        }

        // Methods
        public void SetPasswordButton()
        {
            if (PasswordManager.HasPassword)
            {
                AddPassword_Button.Text = LanguageManager.TranslateString("Manage password");
                Controls.Add(EnableWindowsHello_Button);
            }
            else
            {
                AddPassword_Button.Text = LanguageManager.TranslateString("Add password protection");
            }
        }
        public void UpdateControls()
        {
            EncryptFiles_CheckBox.Checked = Properties.Settings.Default.EncryptFiles;
        }
    }
}
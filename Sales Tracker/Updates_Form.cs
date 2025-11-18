using Guna.UI2.WinForms;
using Sales_Tracker.Classes;
using Sales_Tracker.Language;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker.Settings.Menus
{
    /// <summary>
    /// Form displaying available application updates and version information.
    /// </summary>
    public partial class Updates_Form : BaseForm
    {
        // Properties
        private bool _updateReadyForRestart = false;
        private Guna2WinProgressIndicator _progressIndicator;

        // Init.
        public Updates_Form()
        {
            InitializeComponent();

            UpdateTheme();
            LanguageManager.UpdateLanguageForControl(this);
            InitializeUpdateManager();
            UpdateButtonState();
            Update_Button.Text = LanguageManager.TranslateString("Update");
        }
        private void InitializeUpdateManager()
        {
            // Subscribe to events
            NetSparkleUpdateManager.UpdateDownloadStarted += OnUpdateDownloadStarted;
            NetSparkleUpdateManager.UpdateDownloadCompleted += OnUpdateDownloadCompleted;
        }
        private void UpdateButtonState()
        {
            if (_updateReadyForRestart)
            {
                Update_Button.Text = LanguageManager.TranslateString("Restart to apply update");
                Update_Button.Enabled = true;
                Update_Button.FillColor = CustomColors.AccentGreen;

                UpdateStatusLabel(LanguageManager.TranslateString("Update ready - restart required"));
            }
            else
            {
                SetUpdateButtonText(NetSparkleUpdateManager.AvailableVersion);
                Update_Button.Enabled = !NetSparkleUpdateManager.IsUpdating;

                string statusText = string.IsNullOrEmpty(NetSparkleUpdateManager.AvailableVersion)
                    ? LanguageManager.TranslateString("Update available")
                    : $"{LanguageManager.TranslateString("Version")} {NetSparkleUpdateManager.AvailableVersion} {LanguageManager.TranslateString("is available")}";

                UpdateStatusLabel(statusText);
            }
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);
            ThemeManager.MakeGButtonBluePrimary(Update_Button);
            ThemeManager.MakeGButtonBlueSecondary(NotNow_Button);
        }

        // Form event handlers
        private void Updates_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
        }
        private void Updates_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Unsubscribe from events
            NetSparkleUpdateManager.UpdateDownloadStarted -= OnUpdateDownloadStarted;
            NetSparkleUpdateManager.UpdateDownloadCompleted -= OnUpdateDownloadCompleted;
        }

        // Event handlers
        private async void Update_Button_Click(object sender, EventArgs e)
        {
            if (_updateReadyForRestart)
            {
                NetSparkleUpdateManager.ApplyUpdateAndRestart();
                return;
            }

            bool started = await NetSparkleUpdateManager.StartUpdateAsync();

            if (!started)
            {
                CustomMessageBox.Show(
                    "Update Error",
                    "Failed to start the update process. Please try again or check the logs for more details.",
                    CustomMessageBoxIcon.Error,
                    CustomMessageBoxButtons.Ok);
            }
        }
        private void NotNow_Button_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void WhatsNew_LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Tools.OpenLink("https://argorobots.com/whats-new/index.php");
        }

        // Update Manager Event Handlers
        private void OnUpdateDownloadStarted(object sender, UpdateDownloadStartedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => OnUpdateDownloadStarted(sender, e));
                return;
            }

            // Hide the buttons
            Update_Button.Visible = false;
            NotNow_Button.Visible = false;

            // Create and configure the progress indicator
            _progressIndicator = new()
            {
                AutoStart = true,
                ProgressColor = CustomColors.AccentBlue,
                Anchor = AnchorStyles.Top
            };
            _progressIndicator.Location = new Point(
                (Width - _progressIndicator.Width) / 2,
                 Update_Button.Location.Y
            );

            Controls.Add(_progressIndicator);

            // Update status label with proper translation
            string statusText = string.IsNullOrEmpty(e.Version)
                ? LanguageManager.TranslateString("Downloading update...")
                : $"{LanguageManager.TranslateString("Downloading")} V.{e.Version}...";

            UpdateStatusLabel(statusText);
        }
        private void OnUpdateDownloadCompleted(object sender, UpdateDownloadCompletedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => OnUpdateDownloadCompleted(sender, e));
                return;
            }

            // Hide the progress indicator
            _progressIndicator.Stop();
            _progressIndicator.Visible = false;

            if (!e.Success)
            {
                // Download or Installation failed
                _updateReadyForRestart = false;
                Update_Button.Text = LanguageManager.TranslateString("Update");
                Update_Button.Visible = true;
                NotNow_Button.Visible = true;
                Update_Button.FillColor = CustomColors.AccentBlue;
                UpdateStatusLabel(LanguageManager.TranslateString("Update failed"));

                CustomMessageBox.Show(
                    "Update Error",
                    $"Failed to install update. Please try again later or contact support if the problem persists.",
                    CustomMessageBoxIcon.Error,
                    CustomMessageBoxButtons.Ok);
            }
            else if (e.RequiresRestart)
            {
                // Download and installation completed successfully
                HandleUpdateComplete();
            }
        }

        // Handle update completion
        private void HandleUpdateComplete()
        {
            _updateReadyForRestart = true;

            Update_Button.Text = LanguageManager.TranslateString("Restart to apply update");
            Update_Button.Visible = true;

            UpdateStatusLabel(LanguageManager.TranslateString("Update ready - restart required"));
        }
        private void SetUpdateButtonText(string? availableVersion)
        {
            if (string.IsNullOrEmpty(availableVersion) || availableVersion == "Update Available")
            {
                Update_Button.Text = LanguageManager.TranslateString("Download update");
            }
            else
            {
                Update_Button.Text = $"{LanguageManager.TranslateString("Update to")} V.{availableVersion}";
            }
        }
        private void UpdateStatusLabel(string text)
        {
            // Text is already translated before being passed to this method
            Status_Label.Text = text;
            Status_Label.Left = (Width - Status_Label.Width) / 2;
        }
    }
}
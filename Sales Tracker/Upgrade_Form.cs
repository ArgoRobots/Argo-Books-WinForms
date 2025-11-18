using Guna.UI2.WinForms;
using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.Language;
using Sales_Tracker.Properties;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker
{
    /// <summary>
    /// Form for upgrading to the paid version of the application.
    /// </summary>
    public partial class Upgrade_Form : BaseForm
    {
        // Properties
        private static Upgrade_Form _instance;

        // Getters
        public static Upgrade_Form Instance => _instance;

        // Init.
        public Upgrade_Form()
        {
            InitializeComponent();
            _instance = this;

            ConstructEnterKeyPanel();
            UpdateTheme();
            SetAccessibleDescriptions();
            LanguageManager.UpdateLanguageForControl(this);
            CenterButton();
            CenterLabels();

            PanelCloseFilter panelCloseFilter = new(this, ClosePanels, TextBoxManager.RightClickTextBox_Panel);
            Application.AddMessageFilter(panelCloseFilter);

            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        int CenterHorizontally(Control control) => control.Left = (ClientSize.Width - Benifits_Panel.Width - control.Width) / 2;
        private void CenterButton()
        {
            CenterHorizontally(Upgrade_Button);
            CenterHorizontally(EnterKey_Button);

            CenterHorizontally(StripeLogo_ImageButton);
            PayPalLogo_ImageButton.Left = StripeLogo_ImageButton.Left - PayPalLogo_ImageButton.Width - CustomControls.SpaceBetweenControls;
            Square_ImageButton.Left = StripeLogo_ImageButton.Right + CustomControls.SpaceBetweenControls;
        }
        public void CenterLabels()
        {
            CenterHorizontally(UpgradeTitle_Label);
            CenterHorizontally(UpgradeSubTitle_Label);
            CenterHorizontally(DollarAmount_Label);

            _enterLicense_Label.Left = (_enterKey_Panel.Width - _enterLicense_Label.Width) / 2;
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);
            Benifits_Panel.BackColor = CustomColors.HeaderBackground;
            ThemeManager.MakeGButtonBluePrimary(Upgrade_Button);
            ThemeManager.MakeGButtonBlueSecondary(EnterKey_Button);

            // For EnterKey_Panel
            ThemeManager.MakeGButtonBluePrimary(_verifyLicense_Button);
            _back_Button.FillColor = CustomColors.MainBackground;

            if (ThemeManager.IsDarkTheme())
            {
                Square_ImageButton.Image = Resources.SquareLogoWhite;
                _back_Button.Image = Resources.BackArrowWhite;
            }
            else
            {
                Square_ImageButton.Image = Resources.SquareLogoBlack;
                _back_Button.Image = Resources.BackArrowBlack;
            }
        }
        private void SetAccessibleDescriptions()
        {
            UnlimitedProducts_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            WindowsHello_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            PriorityCustomerSupport_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
        }

        // Form event handlers
        private void Upgrade_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
        }

        // Event handlers
        private void Upgrade_Button_Click(object sender, EventArgs e)
        {
            Tools.OpenLink("https://argorobots.com/upgrade/index.php");
        }
        private void EnterKey_Button_Click(object sender, EventArgs e)
        {
            _enterKey_Panel.Visible = true;
            _enterKey_Panel.BringToFront();
        }
        private void LearnMore_Button_Click(object sender, EventArgs e)
        {
            Tools.OpenLink("https://argorobots.com/documentation/index.php#version-comparison");
        }

        // EnterKey_Panel
        private Panel _enterKey_Panel;
        private Guna2Button _verifyLicense_Button, _back_Button;
        private Label _error_Label, _enterLicense_Label;
        private Guna2TextBox _license_TextBox;
        private void ConstructEnterKeyPanel()
        {
            _enterKey_Panel = new()
            {
                BackColor = CustomColors.MainBackground,
                Visible = false,
                Dock = DockStyle.Fill,
                Size = ClientSize
            };
            Controls.Add(_enterKey_Panel);

            _enterLicense_Label = new()
            {
                ForeColor = CustomColors.Text,
                AutoSize = true,
                Top = 22,
                Font = new Font("Segoe UI", 16),
                Text = "Enter your license",
                Name = "EnterYourLicense_Label",
                Anchor = AnchorStyles.Top
            };
            _enterKey_Panel.Controls.Add(_enterLicense_Label);

            _license_TextBox = new()
            {
                Anchor = AnchorStyles.Top,
                Size = new Size(350, 50),
                PlaceholderText = "Enter license",
                ShortcutsEnabled = false
            };
            TextBoxManager.Attach(_license_TextBox);
            _enterKey_Panel.Controls.Add(_license_TextBox);

            _verifyLicense_Button = new()
            {
                Anchor = AnchorStyles.Top,
                Size = new Size(350, 50),
                Text = "Verify license",
                Font = new Font("Segoe UI", 10),
                BorderRadius = 2,
                BorderThickness = 1,
                Name = "VerifyLicense_Button",
            };
            _verifyLicense_Button.Click += VerifyLicense_Button_Click;
            _enterKey_Panel.Controls.Add(_verifyLicense_Button);

            _back_Button = new()
            {
                Size = new Size(50, 45),
                ImageSize = new Size(40, 40),
                Location = new Point(_enterLicense_Label.Top, _enterLicense_Label.Top)
            };
            _back_Button.Click += BackButton_Click;
            _enterKey_Panel.Controls.Add((_back_Button));

            // Set locations
            _license_TextBox.Location = new Point((
                _enterKey_Panel.Width - _license_TextBox.Width) / 2,
                (_enterKey_Panel.Height - _license_TextBox.Height - CustomControls.SpaceBetweenControls - _verifyLicense_Button.Height) / 2);

            _verifyLicense_Button.Location = new Point(_license_TextBox.Left, _license_TextBox.Bottom + CustomControls.SpaceBetweenControls);
        }
        private async void VerifyLicense_Button_Click(object sender, EventArgs e)
        {
            string key = _license_TextBox.Text.Trim().ToUpper();
            LicenseManager licenseManager = new();
            bool iskeyValid = await licenseManager.ValidateKeyAsync(key);

            if (iskeyValid)
            {
                SetLicenseValid();
                Log.Write(2, "Enabled full version!");
            }
            else { SetLicenseInvalid(); }
        }
        private void SetLicenseValid()
        {
            Panel animationPanel = new()
            {
                BackColor = CustomColors.MainBackground,
                Dock = DockStyle.Fill
            };
            _enterKey_Panel.Controls.Add(animationPanel);
            animationPanel.BringToFront();

            Guna2CircleProgressBar progressCircle = new()
            {
                Size = new Size(100, 100),
                Anchor = AnchorStyles.Top,
                FillColor = CustomColors.ControlBack,
                BackColor = CustomColors.MainBackground,
                ProgressColor = CustomColors.AccentGreen
            };
            animationPanel.Controls.Add(progressCircle);

            Label successLabel = new()
            {
                AutoSize = true,
                Anchor = AnchorStyles.Top,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                BackColor = CustomColors.MainBackground,
                ForeColor = CustomColors.Text,
                Text = LanguageManager.TranslateString("License Verified Successfully!"),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };
            animationPanel.Controls.Add(successLabel);

            Guna2Button closeButton = new()
            {
                Size = _verifyLicense_Button.Size,
                Anchor = AnchorStyles.Top,
                Text = LanguageManager.TranslateString("Close"),
                Font = new Font("Segoe UI", 10),
                BorderRadius = 2,
                BorderThickness = 1,
                Visible = false
            };
            ThemeManager.MakeGButtonBluePrimary(closeButton);
            animationPanel.Controls.Add(closeButton);

            closeButton.Click += (_, _) => Close();

            // Center progressCircle
            progressCircle.Location = new Point(
                (animationPanel.Width - progressCircle.Width) / 2,
                (animationPanel.Height - progressCircle.Height) / 2
            );

            // Animate the progress circle
            async Task AnimateSuccess()
            {
                for (int i = 0; i <= 100; i += 2)
                {
                    progressCircle.Value = i;
                    await Task.Delay(10);
                }
                await Task.Delay(500);

                MainMenu_Form.Instance.RemoveUpgradeButton();

                progressCircle.Visible = false;
                successLabel.Visible = true;
                closeButton.Visible = true;

                // Center the two controls
                int totalHeight = successLabel.Height + closeButton.Height + 20;
                int startY = (animationPanel.Height - totalHeight) / 2;

                successLabel.Location = new Point(
                    (animationPanel.Width - successLabel.Width) / 2,
                    startY
                );
                closeButton.Location = new Point(
                    (animationPanel.Width - closeButton.Width) / 2,
                    startY + successLabel.Height + 20
                );
            }

            _ = AnimateSuccess();
        }
        private void SetLicenseInvalid()
        {
            // Create error label if it doesn't exist
            if (_error_Label == null)
            {
                _error_Label = new()
                {
                    AutoSize = true,
                    Font = new Font("Segoe UI", 11),
                    ForeColor = CustomColors.AccentRed,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Visible = false
                };
                _enterKey_Panel.Controls.Add(_error_Label);
            }

            // Set label properties
            _error_Label.Text = LanguageManager.TranslateString("Invalid license key");
            _error_Label.Location = new Point(
                (_enterKey_Panel.Width - _error_Label.Width) / 2,
                _verifyLicense_Button.Bottom + 10
            );
            _error_Label.Visible = true;

            // Disable button
            _verifyLicense_Button.Enabled = false;
            _verifyLicense_Button.Text = LanguageManager.TranslateString("Invalid License");

            _ = ShowErrorAndShake();

            // Reset to allow user to try again
            _verifyLicense_Button.Enabled = true;
            _error_Label.Visible = false;
        }
        private async Task ShowErrorAndShake()
        {
            int originalX = _verifyLicense_Button.Left;
            for (int i = 0; i < 6; i++)
            {
                _verifyLicense_Button.Left = originalX + (i % 2 == 0 ? -5 : 5);
                await Task.Delay(50);
            }
            _verifyLicense_Button.Left = originalX;

            // Wait before hiding error message
            await Task.Delay(3000);

            // Hide error message and reset button
            _error_Label.Visible = false;
            _verifyLicense_Button.Enabled = true;
            _verifyLicense_Button.Text = LanguageManager.TranslateString("Verify License");
        }
        private void BackButton_Click(object sender, EventArgs e)
        {
            _enterKey_Panel.Visible = false;
        }
        private void ClosePanels()
        {
            TextBoxManager.HideRightClickPanel();
        }
    }
}
using Guna.UI2.WinForms;
using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.Encryption;
using Sales_Tracker.Language;
using Sales_Tracker.Properties;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;
using Windows.Security.Credentials;

namespace Sales_Tracker.Passwords
{
    /// <summary>
    /// Form for entering password authentication.
    /// </summary>
    public partial class EnterPassword_Form : BaseForm
    {
        // Properties
        private readonly bool _requiresAccountantSelection;
        private readonly bool _requiresPassword;
        private Guna2TextBox _accountant_TextBox;

        // Init.
        public EnterPassword_Form() : this(true, false, true) { }  // This is needed for TranslationGenerator.GenerateAllLanguageTranslationFiles()
        public EnterPassword_Form(bool allowWindowsHello = true, bool requiresAccountantSelection = false, bool requiresPassword = true)
        {
            InitializeComponent();

            _requiresAccountantSelection = requiresAccountantSelection;
            _requiresPassword = requiresPassword;

            PasswordManager.IsPasswordValid = false;
            MainMenu_Form.SelectedAccountant = null;

            if (_requiresAccountantSelection)
            {
                ConstructAccountantControls();
            }

            AddEventHandlersToTextBoxes();
            UpdateTheme();
            LanguageManager.UpdateLanguageForControl(this);
            SetWindowsHelloControls(allowWindowsHello);
            AdjustFormLayout();

            PanelCloseFilter panelCloseFilter = new(this, ClosePanels, SearchBox.SearchResultBoxContainer);
            Application.AddMessageFilter(panelCloseFilter);

            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);

            PasswordEye_Button.BackColor = CustomColors.ControlBack;

            if (ThemeManager.IsDarkTheme())
            {
                PasswordEye_Button.Image = Resources.ViewWhite;
            }
            else
            {
                PasswordEye_Button.Image = Resources.ViewBlack;
            }
        }
        private void ConstructAccountantControls()
        {
            float scale = DpiHelper.GetRelativeDpiScale();

            // Accountant textbox
            _accountant_TextBox = new Guna2TextBox
            {
                Size = new Size(Password_TextBox.Width, (int)(50 * scale)),  // Match Password_TextBox width
                Font = new Font("Segoe UI", 10),
                PlaceholderText = LanguageManager.TranslateString("Accountant"),
                Name = "Accountant_TextBox",
                ShortcutsEnabled = false
            };

            Controls.Add(_accountant_TextBox);

            // Set up search box for accountants
            List<string> accountants = FooterManager.GetAccountants(Directories.ArgoCompany_file);
            List<SearchResult> accountantResults = SearchBox.ConvertToSearchResults(accountants);
            SearchBox.Attach(_accountant_TextBox, this, () => accountantResults, (int)(200 * scale), false, true, false, true);
        }
        private void AdjustFormLayout()
        {
            float scale = DpiHelper.GetRelativeDpiScale();

            int currentY = Password_TextBox.Top;
            int spacing = (int)(20 * scale);

            // Position accountant controls if they exist
            if (_requiresAccountantSelection)
            {
                _accountant_TextBox.Location = new Point((Width - _accountant_TextBox.Width) / 2, currentY);
                currentY += _accountant_TextBox.Height + spacing;
            }

            // Position password controls if they exist
            if (_requiresPassword)
            {
                Password_TextBox.Location = new Point((Width - Password_TextBox.Width) / 2, currentY);
                PasswordEye_Button.Top = Password_TextBox.Top + (Password_TextBox.Height - PasswordEye_Button.Height) / 2;
                currentY += Password_TextBox.Height + spacing;
            }
            else
            {
                Password_TextBox.Visible = false;
            }

            if (_requiresPassword && _requiresAccountantSelection)
            {
                EnterPassword_Label.Text = LanguageManager.TranslateString("Log in");
            }
            else if (_requiresAccountantSelection)
            {
                EnterPassword_Label.Text = LanguageManager.TranslateString("Select user");
                PasswordEye_Button.Visible = false;
            }

            EnterPassword_Label.Left = (Width - EnterPassword_Label.Width) / 2;

            // Position Enter button
            Enter_Button.Location = new Point((Width - Enter_Button.Width) / 2, currentY + spacing);
            currentY += Enter_Button.Height + spacing;

            // Adjust form height with scaling
            int baseHeight = (int)(200 * scale);
            int additionalHeight = 0;

            if (_requiresAccountantSelection) { additionalHeight += (int)(80 * scale); }
            if (_requiresPassword) { additionalHeight += (int)(80 * scale); }

            Height = baseHeight + additionalHeight;

            // Update Windows Hello button position if it exists
            if (_windowsHello_Button != null)
            {
                _windowsHello_Button.Top = currentY;
                _windowsHello_Button.Left = (Width - _windowsHello_Button.Width) / 2;
                Height += (int)(70 * scale);
            }

            // Update message label position
            if (Controls.Contains(Message_LinkLabel))
            {
                int messageSpacing = (int)(30 * scale);
                Message_LinkLabel.Top = _windowsHello_Button == null ? Enter_Button.Bottom + messageSpacing : _windowsHello_Button.Bottom + messageSpacing;
                Message_LinkLabel.Left = (Width - Message_LinkLabel.Width) / 2;

                if (_requiresPassword)
                {
                    Height += (int)(70 * scale);
                }
            }
        }
        private void AddEventHandlersToTextBoxes()
        {
            if (_requiresPassword)
            {
                TextBoxManager.Attach(false, Password_TextBox);
            }

            if (_requiresAccountantSelection)
            {
                TextBoxValidation.OnlyAllowLetters(_accountant_TextBox);
                TextBoxManager.Attach(false, _accountant_TextBox);
                _accountant_TextBox.TextChanged += ValidateInputs;
                _accountant_TextBox.KeyDown += Accountant_TextBox_KeyDown;
            }
        }

        // Form event handlers
        private void EnterPassword_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);

            // Focus the first required field
            if (_requiresAccountantSelection)
            {
                _accountant_TextBox.Focus();
            }
            else if (_requiresPassword)
            {
                Password_TextBox.Focus();
            }
        }

        // Event handlers
        private void Enter_Button_Click(object sender, EventArgs e)
        {
            ProcessEntry();
        }
        private void Password_TextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateInputs(null, null);
        }
        private void Password_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Windows Forms by default prevents copying from password textboxes for security,
            // showing a "cannot copy password" popup. We override this behavior to allow
            // clipboard operations while maintaining password masking.

            // Handle copy operation for password textbox
            if (e.Control && e.KeyCode == Keys.C)
            {
                Guna2TextBox textBox = (Guna2TextBox)sender;
                if (!string.IsNullOrEmpty(textBox.SelectedText))
                {
                    Clipboard.SetText(textBox.SelectedText);
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            // Handle cut operation for password textbox
            if (e.Control && e.KeyCode == Keys.X)
            {
                Guna2TextBox textBox = (Guna2TextBox)sender;
                if (!string.IsNullOrEmpty(textBox.SelectedText))
                {
                    Clipboard.SetText(textBox.SelectedText);
                    int selectionStart = textBox.SelectionStart;
                    textBox.Text = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength);
                    textBox.SelectionStart = selectionStart;
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            // Handle Enter key
            if (e.KeyCode == Keys.Enter)
            {
                ProcessEntry();
                e.SuppressKeyPress = true;
            }
        }
        private void PasswordEye_Button_Click(object sender, EventArgs e)
        {
            PasswordManager.TogglePasswordVisibility(Password_TextBox, PasswordEye_Button);
        }
        private void Accountant_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (_requiresPassword)
                {
                    Password_TextBox.Focus();
                }
                else
                {
                    ProcessEntry();
                }
                e.SuppressKeyPress = true;
            }
        }
        private void Message_LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Tools.OpenLink("https://argorobots.com/upgrade/index.php");
        }
        private void ValidateInputs(object sender, EventArgs e)
        {
            bool accountantValid = !_requiresAccountantSelection ||
                (!string.IsNullOrWhiteSpace(_accountant_TextBox.Text) && _accountant_TextBox.Tag?.ToString() != "0");

            bool passwordValid = !_requiresPassword || !string.IsNullOrWhiteSpace(Password_TextBox.Text);

            Enter_Button.Enabled = accountantValid && passwordValid;
        }

        // Windows Hello
        private Guna2Button _windowsHello_Button;
        private void ConstructWindowsHelloButton()
        {
            if (_windowsHello_Button != null) { return; }

            float scale = DpiHelper.GetRelativeDpiScale();

            _windowsHello_Button = new()
            {
                Anchor = AnchorStyles.Top,
                BorderRadius = 2,
                BorderThickness = 1,
                Enabled = false,
                Font = new Font("Segoe UI", 10),
                Size = new Size((int)(215 * scale), (int)(50 * scale)),
                Text = "Windows Hello"
            };
            _windowsHello_Button.Click += WindowsHello_Button_Click;
            Controls.Add(_windowsHello_Button);
            ThemeManager.SetThemeForControls([_windowsHello_Button]);
        }
        private async void WindowsHello_Button_Click(object sender, EventArgs e)
        {
            SetMessageText("Authorizing...");
            DisableControlsForAuthentication();

            bool result = await RunWindowsHello();
            if (result)
            {
                // Handle accountant selection for Windows Hello
                if (_requiresAccountantSelection && MainMenu_Form.Instance.AccountantList.Count >= 2)
                {
                    if (string.IsNullOrWhiteSpace(_accountant_TextBox.Text) || _accountant_TextBox.Tag?.ToString() == "0")
                    {
                        EnableControlsForAuthentication();
                        CustomMessageBox.Show("Accountant Required", "Please select an accountant first.", CustomMessageBoxIcon.Info, CustomMessageBoxButtons.Ok);
                        _accountant_TextBox.Focus();
                        return;
                    }
                    MainMenu_Form.SelectedAccountant = _accountant_TextBox.Text;
                }

                PasswordManager.IsPasswordValid = true;
                Close();
            }
            else
            {
                EnableControlsForAuthentication();
                Controls.Remove(Message_LinkLabel);
            }
        }
        public static async Task<bool> RunWindowsHello()
        {
            KeyCredentialRetrievalResult result =
                 await KeyCredentialManager.RequestCreateAsync("login",
                 KeyCredentialCreationOption.ReplaceExisting);

            return result.Status == KeyCredentialStatus.Success;
        }
        private async void SetWindowsHelloControls(bool allowWindowsHello)
        {
            if (!Properties.Settings.Default.WindowsHelloEnabled || !allowWindowsHello)
            {
                Controls.Remove(Message_LinkLabel);
                return;
            }

            if (Properties.Settings.Default.LicenseActivated)
            {
                ConstructWindowsHelloButton();

                bool supported = await KeyCredentialManager.IsSupportedAsync();
                if (supported)
                {
                    _windowsHello_Button.Enabled = true;
                    Controls.Remove(Message_LinkLabel);
                }
                else
                {
                    SetMessageText("Windows Hello is not supported on this device");
                }

                // Position will be set in AdjustFormLayout
            }
            else
            {
                SetMessageText("Upgrade now to enable Windows Hello");
                Message_LinkLabel.LinkArea = new LinkArea(Message_LinkLabel.Text.IndexOf("Upgrade now"), "Upgrade now".Length);
            }
        }
        private void SetMessageText(string text)
        {
            Message_LinkLabel.Text = text;
            Controls.Add(Message_LinkLabel);
            Message_LinkLabel.Left = (ClientSize.Width - Message_LinkLabel.Width) / 2;
        }

        // Methods
        private void EnableControlsForAuthentication()
        {
            if (_requiresPassword) { Password_TextBox.Enabled = true; }
            if (_requiresAccountantSelection) { _accountant_TextBox.Enabled = true; }
            Enter_Button.Enabled = true;
            if (_windowsHello_Button != null) { _windowsHello_Button.Enabled = true; }
        }
        private void DisableControlsForAuthentication()
        {
            if (_requiresPassword) { Password_TextBox.Enabled = false; }
            if (_requiresAccountantSelection) { _accountant_TextBox.Enabled = false; }
            Enter_Button.Enabled = false;
            if (_windowsHello_Button != null) { _windowsHello_Button.Enabled = false; }
        }
        private void ProcessEntry()
        {
            bool valid = true;

            // Validate accountant selection if required
            if (_requiresAccountantSelection)
            {
                if (string.IsNullOrWhiteSpace(_accountant_TextBox.Text) || _accountant_TextBox.Tag?.ToString() == "0")
                {
                    CustomMessageBox.Show("Invalid Accountant", "Please select a valid accountant from the list.",
                        CustomMessageBoxIcon.Exclamation, CustomMessageBoxButtons.Ok);
                    _accountant_TextBox.Focus();
                    valid = false;
                }
                else
                {
                    MainMenu_Form.SelectedAccountant = _accountant_TextBox.Text;
                }
            }

            // Validate password if required
            if (_requiresPassword && valid)
            {
                if (PasswordManager.Password == Password_TextBox.Text)
                {
                    PasswordManager.IsPasswordValid = true;
                }
                else
                {
                    CustomMessageBox.Show("Incorrect password", "The password is incorrect.",
                        CustomMessageBoxIcon.Exclamation, CustomMessageBoxButtons.Ok);
                    Password_TextBox.Focus();
                    valid = false;
                }
            }

            if (valid)
            {
                Close();
            }
        }
        private void ClosePanels()
        {
            SearchBox.Close();
        }
    }
}
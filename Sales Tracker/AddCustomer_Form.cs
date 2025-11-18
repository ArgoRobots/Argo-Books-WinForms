using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.GridView;
using Sales_Tracker.Language;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker
{
    /// <summary>
    /// Form for adding new customers to the system.
    /// </summary>
    public partial class AddCustomer_Form : Form
    {
        // Properties
        private static AddCustomer_Form _instance;
        private CountryCode _selectedCountryCode;

        // Getters
        public static AddCustomer_Form Instance => _instance;

        // Init.
        public AddCustomer_Form()
        {
            InitializeComponent();
            _instance = this;

            ValidateInputs(null, null);
            AddEventHandlersToTextBoxes();
            InitializeCountryCodeSearchBox();
            SetAccessibleDescriptions();
            UpdateTheme();

            LanguageManager.UpdateLanguageForControl(this);

            PanelCloseFilter panelCloseFilter = new(this, ClosePanels,
                TextBoxManager.RightClickTextBox_Panel,
                SearchBox.SearchResultBoxContainer);

            Application.AddMessageFilter(panelCloseFilter);

            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void InitializeCountryCodeSearchBox()
        {
            // Attach SearchBox with country codes and flags
            float scale = DpiHelper.GetRelativeDpiScale();
            int searchBoxMaxHeight = (int)(255 * scale);
            SearchBox.Attach(CountryCode_TextBox, this, CountryCode.GetCountryCodeSearchResults, searchBoxMaxHeight, false, true, true, false);

            // Wire up event handler for text changes
            CountryCode_TextBox.TextChanged += CountryCode_TextBox_TextChanged;
            TextBoxManager.Attach(CountryCode_TextBox);
        }
        private void AddEventHandlersToTextBoxes()
        {
            TextBoxManager.Attach(CustomerID_TextBox);

            TextBoxValidation.OnlyAllowLetters(FirstName_TextBox);
            FirstName_TextBox.TextChanged += ValidateInputs;
            TextBoxManager.Attach(FirstName_TextBox);

            TextBoxValidation.OnlyAllowLetters(LastName_TextBox);
            LastName_TextBox.TextChanged += ValidateInputs;
            TextBoxManager.Attach(LastName_TextBox);

            Email_TextBox.TextChanged += ValidateInputs;
            TextBoxManager.Attach(Email_TextBox);

            TextBoxValidation.ValidatePhoneNumber(PhoneNumber_TextBox);
            PhoneNumber_TextBox.TextChanged += ValidateInputs;
            TextBoxManager.Attach(PhoneNumber_TextBox);

            TextBoxManager.Attach(Address_TextBox);
            TextBoxManager.Attach(Notes_TextBox);
        }
        private void SetAccessibleDescriptions()
        {
            CustomerID_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            FirstName_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            LastName_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            Email_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            PhoneNumber_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            Address_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            Notes_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            WarningCustomerID_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            WarningEmail_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);
            WarningEmail_Label.ForeColor = CustomColors.AccentRed;
            WarningCustomerID_Label.ForeColor = CustomColors.AccentRed;
        }

        // Form event handlers
        private void AddCustomer_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
            CustomerID_TextBox.Focus();
        }
        private void AddCustomer_Form_Resize(object sender, EventArgs e)
        {
            ClosePanels();
        }
        private void AddCustomer_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClosePanels();
        }

        // Event handlers
        private void CustomerID_TextBox_TextChanged(object sender, EventArgs e)
        {
            string id = CustomerID_TextBox.Text.Trim();

            // Only show error and red border if field has content and is invalid
            if (!string.IsNullOrWhiteSpace(id) && IsIdDuplicate(id))
            {
                WarningCustomerID_Label.Visible = true;
                CustomerID_TextBox.BorderColor = CustomColors.AccentRed;
            }
            else
            {
                WarningCustomerID_Label.Visible = false;
                CustomerID_TextBox.BorderColor = CustomColors.ControlBorder;
            }
        }
        private void Email_TextBox_TextChanged(object sender, EventArgs e)
        {
            string email = Email_TextBox.Text.Trim();

            // Only show error and red border if field has content and is invalid
            if (!string.IsNullOrWhiteSpace(email) && !TextBoxValidation.IsValidEmail(email))
            {
                WarningEmail_Label.Visible = true;
                WarningEmail_Label.Text = LanguageManager.TranslateString("Invalid email format");
                Email_TextBox.BorderColor = CustomColors.AccentRed;
            }
            else if (!string.IsNullOrWhiteSpace(email) && TextBoxValidation.IsEmailDuplicate(email))
            {
                WarningEmail_Label.Visible = true;
                WarningEmail_Label.Text = LanguageManager.TranslateString("Email already exist");
                Email_TextBox.BorderColor = CustomColors.AccentRed;
            }
            else
            {
                WarningEmail_Label.Visible = false;
                Email_TextBox.BorderColor = CustomColors.ControlBorder;
            }
        }
        private void CountryCode_TextBox_TextChanged(object sender, EventArgs e)
        {
            // Parse the country code from the search result text
            CountryCode? selectedCountry = CountryCode.GetCountryCodeFromText(CountryCode_TextBox.Text);
            if (selectedCountry != null)
            {
                _selectedCountryCode = selectedCountry;

                // Reformat the existing phone number with the new country format
                if (!string.IsNullOrWhiteSpace(PhoneNumber_TextBox.Text))
                {
                    FormatPhoneNumber();
                }
            }
        }
        private void PhoneNumber_TextBox_TextChanged(object sender, EventArgs e)
        {
            FormatPhoneNumber();
        }
        private void AddCustomer_Button_Click(object sender, EventArgs e)
        {
            string customerID = CustomerID_TextBox.Text.Trim();
            if (customerID == "")
            {
                customerID = ReadOnlyVariables.EmptyCell;
            }

            // Get first and last name
            string firstName = FirstName_TextBox.Text.Trim();
            string lastName = LastName_TextBox.Text.Trim();

            // Create new customer
            string phoneNumber = PhoneNumber_TextBox.Text.Trim();
            if (_selectedCountryCode != null && !string.IsNullOrWhiteSpace(phoneNumber))
            {
                phoneNumber = $"{_selectedCountryCode.Code} {phoneNumber}";
            }

            Customer customer = new(
                customerID,
                firstName,
                lastName,
                Email_TextBox.Text,
                phoneNumber,
                Address_TextBox.Text.Trim(),
                Notes_TextBox.Text.Trim());

            // Add to list
            MainMenu_Form.Instance.CustomerList.Add(customer);

            // Add to DataGridView
            int newRowIndex = Customers_Form.Instance.Customers_DataGridView.Rows.Add(
                customer.CustomerID,
                customer.FirstName,
                customer.LastName,
                customer.Email,
                customer.PhoneNumber,
                customer.Address,
                customer.CurrentPaymentStatus.ToString(),
                customer.OutstandingBalance,
                customer.IsBanned,
                customer.RentalRecords.Count,
                customer.LastRentalDate?.ToString("yyyy-MM-dd") ?? "-");

            Customers_Form.Instance.Customers_DataGridView.Rows[newRowIndex].Tag = customer;
            DataGridViewManager.DataGridViewRowsAdded(Customers_Form.Instance.Customers_DataGridView, new DataGridViewRowsAddedEventArgs(newRowIndex, 1));

            string message = $"Added customer '{customer.FullName}'";
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(Customers_Form.ThingsThatHaveChangedInFile, 4, message);

            // Clear inputs
            FirstName_TextBox.Clear();
            LastName_TextBox.Clear();
            CustomerID_TextBox.Clear();
            Email_TextBox.Clear();
            PhoneNumber_TextBox.Clear();
            Address_TextBox.Clear();
            Notes_TextBox.Clear();
        }

        // Methods
        private void FormatPhoneNumber()
        {
            if (_selectedCountryCode == null)
            {
                return;
            }

            int cursorPosition = PhoneNumber_TextBox.SelectionStart;
            string currentText = PhoneNumber_TextBox.Text;

            // Extract only digits from current text
            string digitsOnly = new(currentText.Where(char.IsDigit).ToArray());

            // Count maximum allowed digits based on format (count 'X' characters)
            int maxDigits = _selectedCountryCode.Format.Count(c => c == 'X');

            // Limit digits to the format's maximum
            if (digitsOnly.Length > maxDigits)
            {
                digitsOnly = digitsOnly.Substring(0, maxDigits);
            }

            // Format the phone number
            string formattedNumber = _selectedCountryCode.FormatPhoneNumber(digitsOnly);

            if (currentText != formattedNumber)
            {
                PhoneNumber_TextBox.TextChanged -= PhoneNumber_TextBox_TextChanged;
                PhoneNumber_TextBox.Text = formattedNumber;

                // Adjust cursor position
                int newCursorPosition = Math.Min(cursorPosition + (formattedNumber.Length - currentText.Length), formattedNumber.Length);
                PhoneNumber_TextBox.SelectionStart = Math.Max(0, newCursorPosition);

                PhoneNumber_TextBox.TextChanged += PhoneNumber_TextBox_TextChanged;
            }
        }
        private static bool IsIdDuplicate(string customerID)
        {
            return customerID != ReadOnlyVariables.EmptyCell &&
                DataGridViewManager.DoesValueExistInDataGridView(Customers_Form.Instance.Customers_DataGridView, Customers_Form.Column.CustomerID.ToString(), customerID);
        }
        public void ValidateInputs(object sender, EventArgs e)
        {
            // Only require first and last name
            bool requiredFieldsFilled = !string.IsNullOrWhiteSpace(FirstName_TextBox.Text) &&
                                        !string.IsNullOrWhiteSpace(LastName_TextBox.Text);

            // If email is provided, it must be valid
            bool emailValid = true;
            if (!string.IsNullOrWhiteSpace(Email_TextBox.Text))
            {
                emailValid = TextBoxValidation.IsValidEmail(Email_TextBox.Text) && TextBoxValidation.IsEmailDuplicate(Email_TextBox.Text);
            }

            AddCustomer_Button.Enabled = requiredFieldsFilled && emailValid;
        }
        private void ClosePanels()
        {
            SearchBox.Close();
            TextBoxManager.HideRightClickPanel();
        }
    }
}

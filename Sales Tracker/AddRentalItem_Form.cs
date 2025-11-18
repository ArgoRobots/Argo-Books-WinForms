using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.Language;
using Sales_Tracker.Rentals;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker
{
    /// <summary>
    /// Form for adding new rental items to inventory.
    /// </summary>
    public partial class AddRentalItem_Form : BaseForm
    {
        // Properties
        public static List<string> ThingsThatHaveChangedInFile { get; } = [];

        // Init.
        public AddRentalItem_Form()
        {
            InitializeComponent();

            AddEventHandlersToTextBoxes();
            Date_DateTimePicker.Value = DateTime.Now;
            Date_DateTimePicker.MaxDate = DateTime.Now;
            CheckIfProductsExist();
            UpdateTheme();
            SetAccessibleDescriptions();
            LanguageManager.UpdateLanguageForControl(this);

            PanelCloseFilter panelCloseFilter = new(this, ClosePanels,
                TextBoxManager.RightClickTextBox_Panel,
                SearchBox.SearchResultBoxContainer);

            Application.AddMessageFilter(panelCloseFilter);
            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void AddEventHandlersToTextBoxes()
        {
            byte searchBoxMaxHeight = 255;

            TextBoxManager.Attach(RentalItemID_TextBox);
            RentalItemID_TextBox.TextChanged += ValidateInputs;

            TextBoxManager.Attach(ProductName_TextBox);
            SearchBox.Attach(ProductName_TextBox, this, GetSearchResultsForProducts, searchBoxMaxHeight, true, false, true, true);
            ProductName_TextBox.TextChanged += ValidateInputs;

            TextBoxValidation.OnlyAllowNumbers(TotalQuantity_TextBox);
            TextBoxManager.Attach(TotalQuantity_TextBox);
            TotalQuantity_TextBox.TextChanged += ValidateInputs;

            TextBoxValidation.OnlyAllowNumbersAndOneDecimal(DailyRate_TextBox);
            TextBoxManager.Attach(DailyRate_TextBox);
            DailyRate_TextBox.TextChanged += ValidateInputs;

            TextBoxValidation.OnlyAllowNumbersAndOneDecimal(WeeklyRate_TextBox);
            TextBoxManager.Attach(WeeklyRate_TextBox);
            WeeklyRate_TextBox.TextChanged += ValidateInputs;

            TextBoxValidation.OnlyAllowNumbersAndOneDecimal(MonthlyRate_TextBox);
            TextBoxManager.Attach(MonthlyRate_TextBox);
            MonthlyRate_TextBox.TextChanged += ValidateInputs;

            TextBoxValidation.OnlyAllowNumbersAndOneDecimal(SecurityDeposit_TextBox);
            TextBoxManager.Attach(SecurityDeposit_TextBox);

            TextBoxManager.Attach(Notes_TextBox);
        }
        private List<SearchResult> GetSearchResultsForProducts()
        {
            return SearchBox.ConvertToSearchResults(MainMenu_Form.Instance.GetFormattedRentableProductNames());
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);
        }
        private void SetAccessibleDescriptions()
        {
            Label[] labelsToAlignLeftCenter =
            [
                RentalItemID_Label,
                ProductName_Label,
                Date_Label,
                TotalQuantity_Label,
                DailyRate_Label,
                WeeklyRate_Label,
                MonthlyRate_Label,
                SecurityDeposit_Label,
                Notes_Label,
                WarningProduct_LinkLabel
            ];

            foreach (Label label in labelsToAlignLeftCenter)
            {
                label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            }
        }

        // Form event handlers
        private void AddRentalItem_Form_Activated(object sender, EventArgs e)
        {
            CheckIfProductsExist();
        }
        private void AddRentalItem_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
            RentalItemID_TextBox.Focus();
        }
        private void AddRentalItem_Form_Resize(object sender, EventArgs e)
        {
            ClosePanels();
        }
        private void AddRentalItem_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClosePanels();
        }

        // Event handlers
        private void AddRentalItem_Button_Click(object sender, EventArgs e)
        {
            if (!AddRentalItem()) { return; }
            ClearInputs();
        }
        private void WarningProduct_LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Tools.OpenForm(new Products_Form(true));
            CheckIfProductsExist();
        }

        // Methods
        private void ClearInputs()
        {
            RentalItemID_TextBox.Clear();
            ProductName_TextBox.Clear();
            TotalQuantity_TextBox.Clear();
            DailyRate_TextBox.Clear();
            WeeklyRate_TextBox.Clear();
            MonthlyRate_TextBox.Clear();
            SecurityDeposit_TextBox.Clear();
            Notes_TextBox.Clear();
        }
        private bool AddRentalItem()
        {
            string rentalItemID = RentalItemID_TextBox.Text.Trim();

            if (DoesRentalItemExist(rentalItemID))
            {
                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                    "Rental Item ID already exists",
                    "The rental item ID '{0}' already exists. Would you like to add this item anyways?",
                    CustomMessageBoxIcon.Question,
                    CustomMessageBoxButtons.YesNo,
                    rentalItemID);

                if (result != CustomMessageBoxResult.Yes)
                {
                    return false;
                }
            }

            // Get values from TextBoxes
            string[] items = ProductName_TextBox.Text.Split('>');
            string companyName = items[0].Trim();
            string productName = items[2].Trim();

            int totalQuantity = int.Parse(TotalQuantity_TextBox.Text);

            decimal dailyRate = string.IsNullOrWhiteSpace(DailyRate_TextBox.Text)
                ? 0
                : decimal.Parse(DailyRate_TextBox.Text);

            decimal? weeklyRate = string.IsNullOrWhiteSpace(WeeklyRate_TextBox.Text)
                ? null
                : decimal.Parse(WeeklyRate_TextBox.Text);

            decimal? monthlyRate = string.IsNullOrWhiteSpace(MonthlyRate_TextBox.Text)
                ? null
                : decimal.Parse(MonthlyRate_TextBox.Text);

            decimal securityDeposit = string.IsNullOrWhiteSpace(SecurityDeposit_TextBox.Text)
                ? 0
                : decimal.Parse(SecurityDeposit_TextBox.Text);

            string notes = Notes_TextBox.Text.Trim();
            DateTime dateAdded = Date_DateTimePicker.Value;

            // Create new rental item
            RentalItem newItem = new()
            {
                RentalItemID = rentalItemID,
                ProductName = productName,
                CompanyName = companyName,
                TotalQuantity = totalQuantity,
                DailyRate = dailyRate,
                WeeklyRate = weeklyRate,
                MonthlyRate = monthlyRate,
                SecurityDeposit = securityDeposit,
                Notes = notes,
                DateAdded = dateAdded,
                Status = RentalItem.AvailabilityStatus.Available
            };

            // Add to inventory
            RentalInventoryManager.AddRentalItem(newItem);
            Rentals_Form.Instance?.RefreshDataGridView();

            string logMessage = $"Added rental item '{rentalItemID}' - {productName}";
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(ThingsThatHaveChangedInFile, 3, logMessage);

            CustomMessageBox.Show(
                "Success",
                "Rental item added successfully.",
                CustomMessageBoxIcon.Success,
                CustomMessageBoxButtons.Ok);

            return true;
        }
        private static bool DoesRentalItemExist(string id)
        {
            if (string.IsNullOrEmpty(id) || id == ReadOnlyVariables.EmptyCell)
            {
                return false;
            }

            if (RentalInventoryManager.RentalInventory.Any(item =>
                    item.RentalItemID.Equals(id, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            return false;
        }
        private void CheckIfProductsExist()
        {
            if (MainMenu_Form.Instance.GetProductPurchaseNames().Count == 0)
            {
                WarningProduct_PictureBox.Visible = true;
                WarningProduct_LinkLabel.Visible = true;
            }
            else
            {
                WarningProduct_PictureBox.Visible = false;
                WarningProduct_LinkLabel.Visible = false;
            }
        }
        private void ValidateInputs(object sender, EventArgs e)
        {
            bool allFieldsFilled = !string.IsNullOrWhiteSpace(ProductName_TextBox.Text) &&
                ProductName_TextBox.Tag?.ToString() != "0" &&
                !string.IsNullOrWhiteSpace(TotalQuantity_TextBox.Text);

            // At least one rate field must be filled
            bool hasAtLeastOneRate = !string.IsNullOrWhiteSpace(DailyRate_TextBox.Text) ||
                                     !string.IsNullOrWhiteSpace(WeeklyRate_TextBox.Text) ||
                                     !string.IsNullOrWhiteSpace(MonthlyRate_TextBox.Text);

            AddRentalItem_Button.Enabled = allFieldsFilled && hasAtLeastOneRate;
        }
        private void ClosePanels()
        {
            TextBoxManager.HideRightClickPanel();
            SearchBox.Close();
        }
    }
}
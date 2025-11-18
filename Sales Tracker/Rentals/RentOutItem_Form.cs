using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.GridView;
using Sales_Tracker.Language;
using Sales_Tracker.Rentals;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker
{
    /// <summary>
    /// Form for renting out an item to a customer.
    /// </summary>
    public partial class RentOutItem_Form : BaseForm
    {
        // Properties
        private readonly RentalItem _rentalItem;
        private readonly DataGridViewRow _inventoryRow;

        // Init.
        public RentOutItem_Form(RentalItem rentalItem, DataGridViewRow inventoryRow)
        {
            InitializeComponent();
            _rentalItem = rentalItem;
            _inventoryRow = inventoryRow;

            InitializeForm();
            PopulateCustomerList();
            UpdateTheme();
            SetAccessibleDescriptions();
            LanguageManager.UpdateLanguageForControl(this);
            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void InitializeForm()
        {
            Text = $"Rent out: {_rentalItem.ProductName}";
            ProductName_Label.Text = _rentalItem.ProductName;
            AvailableQuantity_Label.Text = $"Available: {_rentalItem.QuantityAvailable}";

            // Set defaults
            Quantity_NumericUpDown.Maximum = _rentalItem.QuantityAvailable;
            Quantity_NumericUpDown.Value = 1;
            RentalStartDate_DateTimePicker.Value = DateTime.Today;
            SecurityDeposit_TextBox.Text = _rentalItem.SecurityDeposit.ToString("0.00");

            // Enable/disable rate radio buttons based on what's configured
            DailyRate_RadioButton.Enabled = _rentalItem.DailyRate > 0;
            WeeklyRate_RadioButton.Enabled = _rentalItem.WeeklyRate.HasValue && _rentalItem.WeeklyRate.Value > 0;
            MonthlyRate_RadioButton.Enabled = _rentalItem.MonthlyRate.HasValue && _rentalItem.MonthlyRate.Value > 0;

            // Select first available rate
            if (DailyRate_RadioButton.Enabled)
            {
                DailyRate_RadioButton.Checked = true;
                DailyRate_Label.Text = $"Daily: {MainMenu_Form.CurrencySymbol}{_rentalItem.DailyRate:N2}";
            }
            else if (WeeklyRate_RadioButton.Enabled)
            {
                WeeklyRate_RadioButton.Checked = true;
                WeeklyRate_Label.Text = $"Weekly: {MainMenu_Form.CurrencySymbol}{_rentalItem.WeeklyRate:N2}";
            }
            else if (MonthlyRate_RadioButton.Enabled)
            {
                MonthlyRate_RadioButton.Checked = true;
                MonthlyRate_Label.Text = $"Monthly: {MainMenu_Form.CurrencySymbol}{_rentalItem.MonthlyRate:N2}";
            }

            if (_rentalItem.WeeklyRate.HasValue)
            {
                WeeklyRate_Label.Text = $"Weekly: {MainMenu_Form.CurrencySymbol}{_rentalItem.WeeklyRate.Value:N2}";
            }

            if (_rentalItem.MonthlyRate.HasValue)
            {
                MonthlyRate_Label.Text = $"Monthly: {MainMenu_Form.CurrencySymbol}{_rentalItem.MonthlyRate.Value:N2}";
            }

            // Add event handlers to RadioButton labels
            DailyRate_Label.Click += (s, e) => DailyRate_RadioButton.Checked = true;
            WeeklyRate_Label.Click += (s, e) => WeeklyRate_RadioButton.Checked = true;
            MonthlyRate_Label.Click += (s, e) => MonthlyRate_RadioButton.Checked = true;

            TextBoxValidation.OnlyAllowNumbersAndOneDecimal(SecurityDeposit_TextBox);

            UpdateTotalCost();
        }
        private void PopulateCustomerList()
        {
            Customer_ComboBox.Items.Clear();

            foreach (Customer customer in MainMenu_Form.Instance.CustomerList)
            {
                Customer_ComboBox.Items.Add($"{customer.FullName} ({customer.CustomerID})");
            }

            if (Customer_ComboBox.Items.Count > 0)
            {
                Customer_ComboBox.SelectedIndex = 0;
            }
            else
            {
                NoCustomers_Label.Visible = true;
                RentOut_Button.Enabled = false;
            }
        }
        private void UpdateTotalCost()
        {
            decimal rate = GetSelectedRate();
            int quantity = (int)Quantity_NumericUpDown.Value;
            decimal deposit = 0;

            if (decimal.TryParse(SecurityDeposit_TextBox.Text, out decimal parsedDeposit))
            {
                deposit = parsedDeposit;
            }

            decimal totalCost = (rate * quantity) + deposit;
            TotalCost_Label.Text = $"Total: {MainMenu_Form.CurrencySymbol}{totalCost:N2}";
        }
        private decimal GetSelectedRate()
        {
            if (DailyRate_RadioButton.Checked)
            {
                return _rentalItem.DailyRate;
            }
            if (WeeklyRate_RadioButton.Checked && _rentalItem.WeeklyRate.HasValue)
            {
                return _rentalItem.WeeklyRate.Value;
            }
            if (MonthlyRate_RadioButton.Checked && _rentalItem.MonthlyRate.HasValue)
            {
                return _rentalItem.MonthlyRate.Value;
            }

            return 0;
        }
        private RentalRateType GetSelectedRateType()
        {
            if (DailyRate_RadioButton.Checked)
            {
                return RentalRateType.Daily;
            }
            if (WeeklyRate_RadioButton.Checked)
            {
                return RentalRateType.Weekly;
            }
            if (MonthlyRate_RadioButton.Checked)
            {
                return RentalRateType.Monthly;
            }

            return RentalRateType.Daily;
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);
            ThemeManager.MakeGButtonBluePrimary(RentOut_Button);
        }
        private void SetAccessibleDescriptions()
        {
            ProductName_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
            AvailableQuantity_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
            TotalCost_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
        }

        // Form event handlers
        private void RentOutItem_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
        }

        // Event handlers
        private void RentOut_Button_Click(object sender, EventArgs e)
        {
            // Get selected customer
            string selectedCustomerText = Customer_ComboBox.SelectedItem.ToString();
            string customerID = selectedCustomerText.Substring(selectedCustomerText.LastIndexOf('(') + 1).TrimEnd(')');
            Customer customer = MainMenu_Form.Instance.CustomerList.FirstOrDefault(c => c.CustomerID == customerID);

            if (customer == null)
            {
                CustomMessageBox.Show("Error", "Selected customer not found.",
                    CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok);
                return;
            }

            // Get rental details
            int quantity = (int)Quantity_NumericUpDown.Value;
            decimal deposit = decimal.Parse(SecurityDeposit_TextBox.Text);
            decimal rate = GetSelectedRate();
            decimal totalCost = (rate * quantity) + deposit;

            // Create rental record
            RentalRecord record = new(
                rentalItemID: _rentalItem.RentalItemID,
                productName: _rentalItem.ProductName,
                quantity: quantity,
                rateType: GetSelectedRateType(),
                rate: rate,
                startDate: RentalStartDate_DateTimePicker.Value,
                securityDeposit: deposit,
                notes: Notes_TextBox.Text.Trim()
            );

            // Rent out the item
            if (!_rentalItem.RentOut(quantity, customer.CustomerID))
            {
                CustomMessageBox.Show("Error", "Failed to rent out item. Please check availability.",
                    CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok);
                return;
            }

            _rentalItem.RentalRecords.Add(record);

            // Add rental record to customer
            customer.AddRentalRecord(record);
            customer.UpdatePaymentStatus();

            CreateRentalTransaction(customer, record, quantity, rate, totalCost);

            // Save changes
            RentalInventoryManager.SaveInventory();

            // Update the inventory row
            _inventoryRow.Cells[Rentals_Form.Column.Available.ToString()].Value = _rentalItem.QuantityAvailable;
            _inventoryRow.Cells[Rentals_Form.Column.Rented.ToString()].Value = _rentalItem.QuantityRented;
            _inventoryRow.Cells[Rentals_Form.Column.Status.ToString()].Value = _rentalItem.Status.ToString();
            _inventoryRow.Cells[Rentals_Form.Column.LastRentalDate.ToString()].Value = _rentalItem.LastRentalDate?.ToString("yyyy-MM-dd") ?? "-";

            // Refresh the form
            Rentals_Form.Instance?.RefreshDataGridView();

            string message = $"Rented out {quantity} unit(s) of '{_rentalItem.ProductName}' to {customer.FullName}";
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(AddRentalItem_Form.ThingsThatHaveChangedInFile, 2, message);

            DialogResult = DialogResult.OK;
            Close();
        }
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        private void Customer_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateInputs();
        }
        private void Quantity_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            UpdateTotalCost();
        }
        private void RateType_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTotalCost();
        }
        private void SecurityDeposit_TextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateTotalCost();
            ValidateInputs();
        }

        // Methods
        private void CreateRentalTransaction(Customer customer, RentalRecord record, int quantity, decimal rate, decimal totalCost)
        {
            // Get the product details from category lists
            Product product = MainMenu_Form.GetProductProductNameIsFrom(
                MainMenu_Form.Instance.CategoryPurchaseList,
                _rentalItem.ProductName,
                _rentalItem.CompanyName);

            if (product == null)
            {
                Log.Write(1, $"Product not found: {_rentalItem.ProductName} from {_rentalItem.CompanyName}");
                return;
            }

            string categoryName = MainMenu_Form.GetCategoryNameProductIsFrom(
                MainMenu_Form.Instance.CategoryPurchaseList,
                _rentalItem.ProductName,
                _rentalItem.CompanyName) ?? "";

            // Generate a unique rental ID
            string rentalID = GenerateNextRentalID();

            // Prepare the row values
            object[] rowValues =
            [
                rentalID,                                 // Rental #
                MainMenu_Form.SelectedAccountant,         // Accountant
                _rentalItem.ProductName,                  // Product / Service
                categoryName,                             // Category
                product.CountryOfOrigin ?? "-",           // Country of destination (using origin for rental)
                _rentalItem.CompanyName,                  // Company of origin
                record.StartDate.ToString("yyyy-MM-dd"),  // Date
                quantity,                                 // Total items
                rate.ToString("N2"),                      // Price per unit (rental rate)
                "0.00",                                   // Shipping (not applicable for rentals)
                "0.00",                                   // Tax
                "0.00",                                   // Fee
                "0.00",                                   // Discount
                "0.00",                                   // Charged difference
                totalCost.ToString("N2"),                 // Total rental revenue
                "-",                                      // Notes
                ReadOnlyVariables.EmptyCell               // Has receipt
            ];

            // Add the row to the DataGridView
            int rowIndex = MainMenu_Form.Instance.Rental_DataGridView.Rows.Add(rowValues);

            // Add note if present
            if (!string.IsNullOrWhiteSpace(record.Notes))
            {
                DataGridViewManager.AddNoteToCell(MainMenu_Form.Instance.Rental_DataGridView, rowIndex, record.Notes);
            }

            // Create and attach TagData
            TagData tagData = new()
            {
                CustomerID = customer.CustomerID,
                CustomerName = customer.FullName,
                RentalRecordID = record.RentalRecordID
            };

            MainMenu_Form.Instance.Rental_DataGridView.Rows[rowIndex].Tag = tagData;

            // Set the Has Receipt cell
            MainMenu_Form.SetReceiptCellToX(MainMenu_Form.Instance.Rental_DataGridView.Rows[rowIndex].Cells[MainMenu_Form.Column.HasReceipt.ToString()]);

            // Trigger the RowsAdded event to save and refresh
            DataGridViewRowsAddedEventArgs args = new(rowIndex, 1);
            DataGridViewManager.DataGridViewRowsAdded(MainMenu_Form.Instance.Rental_DataGridView, args);
        }
        private static string GenerateNextRentalID()
        {
            int highestID = 0;

            foreach (DataGridViewRow row in MainMenu_Form.Instance.Rental_DataGridView.Rows)
            {
                string idValue = row.Cells[MainMenu_Form.Column.ID.ToString()].Value?.ToString();

                if (!string.IsNullOrEmpty(idValue) && idValue.StartsWith("R-"))
                {
                    string numberPart = idValue.Substring(2);
                    if (int.TryParse(numberPart, out int id))
                    {
                        highestID = Math.Max(highestID, id);
                    }
                }
            }

            return $"R-{highestID + 1:D4}";
        }
        private void ValidateInputs()
        {
            if (Customer_ComboBox.SelectedIndex == -1 ||
                !decimal.TryParse(SecurityDeposit_TextBox.Text, out decimal deposit) || deposit < 0)
            {
                RentOut_Button.Enabled = false;
            }
            else
            {
                RentOut_Button.Enabled = true;
            }
        }
    }
}
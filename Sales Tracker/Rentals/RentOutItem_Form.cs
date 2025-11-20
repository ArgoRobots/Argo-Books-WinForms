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
        private  Customer _selectedCustomer;

        // Init.
        public RentOutItem_Form(RentalItem rentalItem, DataGridViewRow inventoryRow)
        {
            InitializeComponent();
            _rentalItem = rentalItem;
            _inventoryRow = inventoryRow;

            InitializeForm();
            InitializeCustomerSearchBox();
            UpdateTheme();
            SetAccessibleDescriptions();
            LanguageManager.UpdateLanguageForControl(this);

            PanelCloseFilter panelCloseFilter = new(this, ClosePanels, SearchBox.SearchResultBoxContainer);
            Application.AddMessageFilter(panelCloseFilter);

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

            UpdateTotalCost();
            ValidateInputs();
        }
        private void InitializeCustomerSearchBox()
        {
            if (MainMenu_Form.Instance.CustomerList.Count == 0)
            {
                NoCustomers_Label.Visible = true;
                RentOut_Button.Enabled = false;
                return;
            }

            // Attach SearchBox with customer search results
            float scale = DpiHelper.GetRelativeDpiScale();
            int searchBoxMaxHeight = (int)(255 * scale);
            SearchBox.Attach(Customer_TextBox, this, GetCustomerSearchResults, searchBoxMaxHeight, false, false, false, false);
        }
        private List<SearchResult> GetCustomerSearchResults()
        {
            List<SearchResult> results = [];
            string searchText = Customer_TextBox.Text;

            foreach (Customer customer in MainMenu_Form.Instance.CustomerList)
            {
                string displayText = $"{customer.FullName} ({customer.CustomerID})";
                if (string.IsNullOrEmpty(searchText) || displayText.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(new SearchResult(displayText, null, 0));
                }
            }

            return results;
        }
        private void UpdateTotalCost()
        {
            decimal rate = _rentalItem.DailyRate;
            int quantity = (int)Quantity_NumericUpDown.Value;
            decimal totalCost = rate * quantity;
            TotalCost_Label.Text = $"Total: {MainMenu_Form.CurrencySymbol}{totalCost:N2}";
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
            if (_selectedCustomer == null)
            {
                CustomMessageBox.Show("Error", "Please select a customer.",
                    CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok);
                return;
            }

            // Get rental details
            int quantity = (int)Quantity_NumericUpDown.Value;
            decimal deposit = _rentalItem.SecurityDeposit;
            decimal rate = _rentalItem.DailyRate;
            decimal totalCost = (rate * quantity) + deposit;

            // Create rental record
            RentalRecord record = new(
                rentalItemID: _rentalItem.RentalItemID,
                productName: _rentalItem.ProductName,
                quantity: quantity,
                rateType: RentalRateType.Daily,
                rate: rate,
                startDate: RentalStartDate_DateTimePicker.Value,
                securityDeposit: deposit,
                notes: Notes_TextBox.Text.Trim()
            );

            // Rent out the item
            if (!_rentalItem.RentOut(quantity, _selectedCustomer.CustomerID))
            {
                CustomMessageBox.Show("Error", "Failed to rent out item. Please check availability.",
                    CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok);
                return;
            }

            _rentalItem.RentalRecords.Add(record);

            // Add rental record to customer
            _selectedCustomer.AddRentalRecord(record);
            _selectedCustomer.UpdatePaymentStatus();

            CreateRentalTransaction(_selectedCustomer, record, quantity, rate, totalCost);

            // Save changes
            RentalInventoryManager.SaveInventory();

            // Update the inventory row
            _inventoryRow.Cells[Rentals_Form.Column.Available.ToString()].Value = _rentalItem.QuantityAvailable;
            _inventoryRow.Cells[Rentals_Form.Column.Rented.ToString()].Value = _rentalItem.QuantityRented;
            _inventoryRow.Cells[Rentals_Form.Column.Status.ToString()].Value = _rentalItem.Status.ToString();
            _inventoryRow.Cells[Rentals_Form.Column.LastRentalDate.ToString()].Value = _rentalItem.LastRentalDate?.ToString("yyyy-MM-dd") ?? "-";

            // Refresh the form
            Rentals_Form.Instance?.RefreshDataGridView();

            string message = $"Rented out {quantity} unit(s) of '{_rentalItem.ProductName}' to {_selectedCustomer.FullName}";
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(AddRentalItem_Form.ThingsThatHaveChangedInFile, 2, message);

            DialogResult = DialogResult.OK;
            Close();
        }
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        private void Customer_TextBox_TextChanged(object sender, EventArgs e)
        {
            // Find customer matching the selected text
            string selectedText = Customer_TextBox.Text;
            _selectedCustomer = MainMenu_Form.Instance.CustomerList.FirstOrDefault(c =>
                $"{c.FullName} ({c.CustomerID})" == selectedText);

            ValidateInputs();
        }
        private void Quantity_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            UpdateTotalCost();
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
            RentOut_Button.Enabled = _selectedCustomer != null;
        }
        private void ClosePanels()
        {
            SearchBox.Close();
        }
    }
}
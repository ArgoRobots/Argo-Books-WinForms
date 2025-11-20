using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.GridView;
using Sales_Tracker.Language;
using Sales_Tracker.Rentals;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker.Rentals
{
    /// <summary>
    /// Form for returning rented items from a customer.
    /// </summary>
    public partial class ReturnRental_Form : BaseForm
    {
        // Properties
        private readonly Customer _customer;
        private readonly RentalRecord _rentalRecord;

        // Init
        public ReturnRental_Form(Customer customer, RentalRecord rentalRecord)
        {
            InitializeComponent();
            _customer = customer;
            _rentalRecord = rentalRecord;

            if (_customer == null || _rentalRecord == null)
            {
                CustomMessageBox.Show("Error",
                    "Could not find the rental information.",
                    CustomMessageBoxIcon.Error,
                    CustomMessageBoxButtons.Ok);
                Close();
                return;
            }

            LoadRentalDetails();
            AddEventHandlersToTextBoxes();
            UpdateTheme();
            LanguageManager.UpdateLanguageForControl(this);
            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void AddEventHandlersToTextBoxes()
        {
            TextBoxValidation.OnlyAllowNumbersAndOneDecimal(Tax_TextBox);
            TextBoxManager.Attach(Tax_TextBox);

            TextBoxValidation.OnlyAllowNumbersAndOneDecimal(Fee_TextBox);
            TextBoxManager.Attach(Fee_TextBox);

            TextBoxValidation.OnlyAllowNumbersAndOneDecimal(Shipping_TextBox);
            TextBoxManager.Attach(Shipping_TextBox);

            TextBoxValidation.OnlyAllowNumbersAndOneDecimal(Discount_TextBox);
            TextBoxManager.Attach(Discount_TextBox);

            TextBoxManager.Attach(Notes_TextBox);
        }
        private void LoadRentalDetails()
        {
            // Display rental information
            RentalItem rentalItem = RentalInventoryManager.GetRentalItem(_rentalRecord.RentalItemID);
            if (rentalItem == null)
            {
                RentalDetails_Label.Text = "Rental item not found.";
                return;
            }

            string overdueText = _rentalRecord.IsOverdue ? " [OVERDUE]" : "";
            string dueText = _rentalRecord.DueDate.HasValue ? $"\nDue Date: {_rentalRecord.DueDate.Value:MMM dd, yyyy}" : "";
            decimal outstanding = _rentalRecord.TotalCost - _rentalRecord.AmountPaid;

            RentalDetails_Label.Text = $"Customer: {_customer.FullName} ({_customer.CustomerID})\n" +
                $"Rental ID: {_rentalRecord.RentalRecordID}\n" +
                $"Product: {_rentalRecord.ProductName}\n" +
                $"Quantity: {_rentalRecord.Quantity}\n" +
                $"Rate: {_rentalRecord.RateType}\n" +
                $"Start Date: {_rentalRecord.StartDate:MMM dd, yyyy}{dueText}{overdueText}\n" +
                $"Total Cost: {MainMenu_Form.CurrencySymbol}{_rentalRecord.TotalCost:N2}\n" +
                $"Amount Paid: {MainMenu_Form.CurrencySymbol}{_rentalRecord.AmountPaid:N2}\n" +
                $"Outstanding: {MainMenu_Form.CurrencySymbol}{outstanding:N2}";

            RentalDetails_Label.BackColor = CustomColors.ControlBack;
            RentalDetails_Label.ForeColor=CustomColors.Text;
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);
            ThemeManager.MakeGButtonBluePrimary(Return_Button);
        }

        // Form event handlers
        private void ReturnRental_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
        }

        // Event handlers
        private void Return_Button_Click(object sender, EventArgs e)
        {
            DateTime returnDate = ReturnDate_Picker.Value;
            string notes = Notes_TextBox.Text.Trim();

            // Parse optional fee fields
            decimal tax = string.IsNullOrWhiteSpace(Tax_TextBox.Text) ? 0 : decimal.Parse(Tax_TextBox.Text);
            decimal fee = string.IsNullOrWhiteSpace(Fee_TextBox.Text) ? 0 : decimal.Parse(Fee_TextBox.Text);
            decimal shipping = string.IsNullOrWhiteSpace(Shipping_TextBox.Text) ? 0 : decimal.Parse(Shipping_TextBox.Text);
            decimal discount = string.IsNullOrWhiteSpace(Discount_TextBox.Text) ? 0 : decimal.Parse(Discount_TextBox.Text);

            // Process return
            ProcessReturn(returnDate, notes, tax, fee, shipping, discount);
        }
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // Business logic
        private void ProcessReturn(DateTime returnDate, string notes, decimal tax, decimal fee, decimal shipping, decimal discount)
        {
            try
            {
                // Update rental record
                _rentalRecord.ReturnDate = returnDate;
                _rentalRecord.IsActive = false;
                _rentalRecord.IsOverdue = false;
                _rentalRecord.Tax = tax;
                _rentalRecord.Fee = fee;
                _rentalRecord.Shipping = shipping;
                _rentalRecord.Discount = discount;

                if (!string.IsNullOrWhiteSpace(notes))
                {
                    _rentalRecord.Notes = string.IsNullOrWhiteSpace(_rentalRecord.Notes)
                        ? $"Return Notes: {notes}"
                        : $"{_rentalRecord.Notes}\nReturn Notes: {notes}";
                }

                // Update inventory quantities using the ReturnItem method
                RentalItem rentalItem = RentalInventoryManager.GetRentalItem(_rentalRecord.RentalItemID);
                rentalItem?.ReturnItem(_rentalRecord.Quantity);

                // Update customer rental status
                _customer?.ReturnRental(_rentalRecord.RentalRecordID);

                // Add DataGridView row for the return
                AddRentalRowToDataGridView(returnDate);

                // Refresh the grid to ensure visual changes are displayed
                MainMenu_Form.Instance.Rental_DataGridView.Refresh();

                // Save all changes
                RentalInventoryManager.SaveInventory();
                MainMenu_Form.Instance.SaveCustomersToFile();

                // Refresh charts and UI
                MainMenu_Form.Instance.LoadOrRefreshMainCharts();

                // Refresh rental inventory form if open
                Rentals_Form.Instance?.RefreshDataGridView();

                // Log the action
                string message = $"Returned rental {_rentalRecord.RentalRecordID} for customer {_customer?.FullName}";
                CustomMessage_Form.AddThingThatHasChangedAndLogMessage(AddRentalItem_Form.ThingsThatHaveChangedInFile, 2, message);

                CustomMessageBox.Show("Return Successful",
                    $"Successfully returned rental {_rentalRecord.RentalRecordID} for {_customer?.FullName}.",
                    CustomMessageBoxIcon.Success,
                    CustomMessageBoxButtons.Ok);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error",
                    $"An error occurred while processing the return: {ex.Message}",
                    CustomMessageBoxIcon.Error,
                    CustomMessageBoxButtons.Ok);
            }
        }
        private void AddRentalRowToDataGridView(DateTime returnDate)
        {
            // Create a new row for the returned rental
            RentalItem rentalItem = RentalInventoryManager.GetRentalItem(_rentalRecord.RentalItemID);
            if (rentalItem == null) { return; }

            // Get the product and category information
            Product product = MainMenu_Form.GetProductProductNameIsFrom(
                MainMenu_Form.Instance.CategoryRentalList,
                rentalItem.ProductName,
                rentalItem.CompanyName);

            if (product == null)
            {
                Log.Write(1, $"Product not found: {rentalItem.ProductName} from {rentalItem.CompanyName}");
                return;
            }

            string categoryName = MainMenu_Form.GetCategoryNameProductIsFrom(
                MainMenu_Form.Instance.CategoryRentalList,
                rentalItem.ProductName,
                rentalItem.CompanyName) ?? "";

            // Determine the rental rate based on rate type
            string rateTypeLower = _rentalRecord.RateType.ToString();
            decimal? rate = rateTypeLower switch
            {
                "daily" => rentalItem.DailyRate,
                "weekly" => rentalItem.WeeklyRate,
                "monthly" => rentalItem.MonthlyRate,
                _ => 0
            };

            // Format the notes with return date
            string notes = _rentalRecord.Notes ?? "";
            string returnNote = $"[RETURNED: {returnDate:MMM dd, yyyy}]";
            if (!string.IsNullOrWhiteSpace(notes) && !notes.Contains("RETURNED"))
            {
                notes = $"{notes}\n{returnNote}";
            }
            else if (string.IsNullOrWhiteSpace(notes))
            {
                notes = returnNote;
            }

            // Prepare the row values (matching structure from RentOutItem_Form)
            object[] rowValues =
            [
                _rentalRecord.RentalRecordID,                    // Rental #
                MainMenu_Form.SelectedAccountant,                // Accountant
                rentalItem.ProductName,                          // Product / Service
                categoryName,                                    // Category
                product.CountryOfOrigin ?? "-",                  // Country of destination
                rentalItem.CompanyName,                          // Company of origin
                _rentalRecord.StartDate.ToString("yyyy-MM-dd"),  // Date
                _rentalRecord.Quantity,                          // Total items
                rate.ToString(),                                 // Price per unit (rental rate)
                "0.00",                                          // Shipping
                "0.00",                                          // Tax
                "0.00",                                          // Fee
                "0.00",                                          // Discount
                "0.00",                                          // Charged difference
                _rentalRecord.TotalCost.ToString("N2"),          // Total rental revenue
                "-",                                             // Notes placeholder
                ReadOnlyVariables.EmptyCell                      // Has receipt
            ];

            // Add the row to the DataGridView
            int rowIndex = MainMenu_Form.Instance.Rental_DataGridView.Rows.Add(rowValues);

            // Add note if present
            if (!string.IsNullOrWhiteSpace(notes))
            {
                DataGridViewManager.AddNoteToCell(MainMenu_Form.Instance.Rental_DataGridView, rowIndex, notes);
            }

            // Create and attach TagData
            TagData tagData = new()
            {
                IsReturned = true,
                ReturnDate = returnDate,
                CustomerID = _customer.CustomerID,
                CustomerName = _customer.FullName,
                RentalRecordID = _rentalRecord.RentalRecordID
            };

            DataGridViewRow newRow = MainMenu_Form.Instance.Rental_DataGridView.Rows[rowIndex];
            newRow.Tag = tagData;

            // Set the Has Receipt cell
            MainMenu_Form.SetReceiptCellToX(newRow.Cells[MainMenu_Form.Column.HasReceipt.ToString()]);

            // Trigger the RowsAdded event to save and refresh
            DataGridViewRowsAddedEventArgs args = new(rowIndex, 1);
            DataGridViewManager.DataGridViewRowsAdded(MainMenu_Form.Instance.Rental_DataGridView, args);
        }
    }
}

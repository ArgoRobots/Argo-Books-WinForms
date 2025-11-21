using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.GridView;
using Sales_Tracker.Language;
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
            ReturnDate_Picker.Value = DateTime.Now;

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

            TextBoxValidation.OnlyAllowNumbersAndOneDecimal(AmountCharged_TextBox);
            TextBoxManager.Attach(AmountCharged_TextBox);
            AmountCharged_TextBox.TextChanged += AmountCharged_TextBox_TextChanged;

            TextBoxManager.Attach(Notes_TextBox);

            // Initial validation
            ValidateForm();
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
            RentalDetails_Label.ForeColor = CustomColors.Text;
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
            decimal amountCharged = decimal.Parse(AmountCharged_TextBox.Text);

            ProcessReturn(returnDate, notes, tax, fee, shipping, discount, amountCharged);
        }
        private void AmountCharged_TextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateForm();
        }
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // Validation
        private void ValidateForm()
        {
            // Return_Button is enabled only if AmountCharged_TextBox is not empty
            Return_Button.Enabled = !string.IsNullOrWhiteSpace(AmountCharged_TextBox.Text);
        }

        // Business logic
        private void ProcessReturn(DateTime returnDate, string notes, decimal tax, decimal fee, decimal shipping, decimal discount, decimal amountCharged)
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
                _rentalRecord.AmountCharged = amountCharged;

                // Convert values to USD for currency conversion
                string defaultCurrency = DataFileManager.GetValue(AppDataSettings.DefaultCurrencyType);
                if (string.IsNullOrEmpty(_rentalRecord.OriginalCurrency))
                {
                    _rentalRecord.OriginalCurrency = defaultCurrency;
                }
                string date = Tools.FormatDate(returnDate);
                decimal exchangeRateToUSD = Currency.GetExchangeRate(defaultCurrency, "USD", date, showErrorMessage: false);
                if (exchangeRateToUSD != -1)
                {
                    _rentalRecord.TaxUSD = Math.Round(tax * exchangeRateToUSD, 2);
                    _rentalRecord.FeeUSD = Math.Round(fee * exchangeRateToUSD, 2);
                    _rentalRecord.ShippingUSD = Math.Round(shipping * exchangeRateToUSD, 2);
                    _rentalRecord.DiscountUSD = Math.Round(discount * exchangeRateToUSD, 2);
                    _rentalRecord.AmountChargedUSD = Math.Round(amountCharged * exchangeRateToUSD, 2);
                }

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

                // Save all changes
                RentalInventoryManager.SaveInventory();
                MainMenu_Form.Instance.SaveCustomersToFile();

                // Add the returned rental row to DataGridView
                AddRentalRowToDataGridView(returnDate);

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
            string rateTypeLower = _rentalRecord.RateType.ToString().ToLower();
            decimal? rate = rateTypeLower switch
            {
                "daily" => rentalItem.DailyRate,
                "weekly" => rentalItem.WeeklyRate,
                "monthly" => rentalItem.MonthlyRate,
                _ => 0
            };

            // Format rental rate display
            string ratePeriod = rateTypeLower switch
            {
                "daily" => "day",
                "weekly" => "week",
                "monthly" => "month",
                _ => "day"
            };
            string formattedRate = $"{MainMenu_Form.CurrencySymbol}{rate:N2}/{ratePeriod}";

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

            // Calculate charged difference
            // Expected amount = TotalCost + Tax + Fee + Shipping - Discount
            decimal expectedAmount = _rentalRecord.TotalCost + _rentalRecord.Tax + _rentalRecord.Fee + _rentalRecord.Shipping - _rentalRecord.Discount;
            decimal chargedDifference = _rentalRecord.AmountCharged - expectedAmount;

            // Prepare the row values (matching RentalColumnHeaders structure)
            object[] rowValues =
            [
                _rentalRecord.RentalRecordID,                    // Rental #
                _rentalRecord.Accountant,                        // Accountant
                rentalItem.ProductName,                          // Product / Service
                categoryName,                                    // Category
                product.CountryOfOrigin ?? "-",                  // Country of destination
                rentalItem.CompanyName,                          // Company of origin
                _rentalRecord.StartDate.ToString("yyyy-MM-dd"),  // Start date
                returnDate.ToString("yyyy-MM-dd"),               // End date
                _rentalRecord.Quantity,                          // Total items
                formattedRate,                                   // Rental rate
                _rentalRecord.Shipping.ToString("0.00"),         // Shipping
                _rentalRecord.Tax.ToString("0.00"),              // Tax
                _rentalRecord.Fee.ToString("0.00"),              // Fee
                _rentalRecord.Discount.ToString("0.00"),         // Discount
                chargedDifference.ToString("0.00"),              // Charged difference
                _rentalRecord.AmountCharged.ToString("0.00"),    // Amount charged (Total rental revenue column)
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

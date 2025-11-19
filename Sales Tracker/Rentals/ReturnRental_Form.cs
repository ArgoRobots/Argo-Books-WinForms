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
    /// Form for returning rented items from a customer.
    /// </summary>
    public partial class ReturnRental_Form : BaseForm
    {
        // Properties
        private readonly MainMenu_Form _mainMenuForm;
        private readonly Customer _customer;
        private readonly RentalRecord _rentalRecord;
        private readonly DataGridViewRow _dataGridViewRow;

        // Init.
        public ReturnRental_Form(MainMenu_Form mainMenu, DataGridViewRow rentalRow)
        {
            InitializeComponent();
            _mainMenuForm = mainMenu;
            _dataGridViewRow = rentalRow;

            // Get rental record ID from the row's tag
            if (rentalRow.Tag is TagData tagData)
            {
                // Find the customer and rental record
                _customer = MainMenu_Form.Instance.CustomerList.FirstOrDefault(c => c.CustomerID == tagData.CustomerID);
                _rentalRecord = _customer?.GetActiveRentals().FirstOrDefault(r => r.RentalRecordID == tagData.RentalRecordID);

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
            }
            else
            {
                CustomMessageBox.Show("Error",
                    "Invalid rental data.",
                    CustomMessageBoxIcon.Error,
                    CustomMessageBoxButtons.Ok);
                Close();
                return;
            }

            UpdateTheme();
            LanguageManager.UpdateLanguageForControl(this);
            LoadingPanel.ShowBlankLoadingPanel(this);
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

            // Confirm return
            CustomMessageBoxResult result = CustomMessageBox.Show("Confirm Return",
                $"Are you sure you want to return this rental for {_customer.FullName}?\n\n" +
                $"Rental ID: {_rentalRecord.RentalRecordID}\n" +
                $"Product: {_rentalRecord.ProductName}\n" +
                $"Return Date: {returnDate:MMM dd, yyyy}",
                CustomMessageBoxIcon.Question,
                CustomMessageBoxButtons.YesNo);

            if (result != CustomMessageBoxResult.Yes) { return; }

            // Process return
            ProcessReturn(returnDate, notes);
        }
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // Business logic
        private void ProcessReturn(DateTime returnDate, string notes)
        {
            try
            {
                // Update rental record
                _rentalRecord.ReturnDate = returnDate;
                _rentalRecord.IsActive = false;
                _rentalRecord.IsOverdue = false;

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

                // Update DataGridView row
                UpdateDataGridViewRow(returnDate);

                // Refresh the grid to ensure visual changes are displayed
                _mainMenuForm.Rental_DataGridView.Refresh();

                // Save all changes
                RentalInventoryManager.SaveInventory();
                MainMenu_Form.Instance.SaveCustomersToFile();

                // Save rental data
                DataGridViewManager.DataGridViewRowChanged(_mainMenuForm.Rental_DataGridView);

                // Refresh charts and UI
                _mainMenuForm.LoadOrRefreshMainCharts();

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
        private void UpdateDataGridViewRow(DateTime returnDate)
        {
            // Update the tag data
            if (_dataGridViewRow.Tag is TagData tagData)
            {
                tagData.IsReturned = true;
                tagData.ReturnDate = returnDate;
                _dataGridViewRow.Tag = tagData;
            }

            // Apply visual indicator (strikethrough and color)
            foreach (DataGridViewCell cell in _dataGridViewRow.Cells)
            {
                cell.Style.Font = new Font(cell.Style.Font ?? _dataGridViewRow.DataGridView.DefaultCellStyle.Font, FontStyle.Strikeout);
                cell.Style.ForeColor = Color.Gray;
            }

            // Add return date to notes column if exists
            DataGridViewCell noteCell = _dataGridViewRow.Cells[ReadOnlyVariables.Note_column];
            if (noteCell != null)
            {
                string currentNote = noteCell.Value?.ToString() ?? "";
                string returnNote = $"[RETURNED: {returnDate:MMM dd, yyyy}]";

                if (string.IsNullOrWhiteSpace(currentNote))
                {
                    noteCell.Value = returnNote;
                }
                else if (!currentNote.Contains("RETURNED"))
                {
                    noteCell.Value = $"{currentNote}\n{returnNote}";
                }
            }
        }
    }
}

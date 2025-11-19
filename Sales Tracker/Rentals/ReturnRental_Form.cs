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
        private Customer _selectedCustomer;
        private List<RentalRecord> _activeRentals;
        private readonly List<CheckBox> _rentalCheckBoxes;

        // Init.
        public ReturnRental_Form(MainMenu_Form mainMenu)
        {
            InitializeComponent();
            _mainMenuForm = mainMenu;
            _activeRentals = [];
            _rentalCheckBoxes = [];

            LoadCustomersWithActiveRentals();
            UpdateTheme();
            LanguageManager.UpdateLanguageForControl(this);
            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void LoadCustomersWithActiveRentals()
        {
            CustomerComboBox.Items.Clear();

            List<Customer> customersWithRentals = MainMenu_Form.Instance.CustomerList
                .Where(c => c.GetActiveRentals().Count > 0)
                .OrderBy(c => c.LastName)
                .ToList();

            foreach (Customer customer in customersWithRentals)
            {
                int activeCount = customer.GetActiveRentals().Count;
                CustomerComboBox.Items.Add($"{customer.FullName} ({customer.CustomerID}) - {activeCount} active rental(s)");
            }

            if (CustomerComboBox.Items.Count > 0)
            {
                CustomerComboBox.SelectedIndex = 0;
            }
            else
            {
                NoActiveRentals_Label.Visible = true;
                Return_Button.Enabled = false;
            }
        }
        private void LoadActiveRentals()
        {
            if (_selectedCustomer == null) return;

            // Clear previous rental checkboxes
            RentalsPanel.Controls.Clear();
            _rentalCheckBoxes.Clear();
            _activeRentals = _selectedCustomer.GetActiveRentals();

            int yPosition = 10;

            foreach (RentalRecord rental in _activeRentals)
            {
                RentalItem rentalItem = RentalInventoryManager.GetRentalItem(rental.RentalItemID);
                if (rentalItem == null) continue;

                // Create checkbox for rental
                CheckBox checkBox = new()
                {
                    Location = new Point(10, yPosition),
                    Size = new Size(650, 80),
                    Font = new Font("Segoe UI", 10F),
                    AutoSize = false
                };

                // Build rental info text
                string overdueText = rental.IsOverdue ? " [OVERDUE]" : "";
                string dueText = rental.DueDate.HasValue ? $" (Due: {rental.DueDate.Value:MMM dd, yyyy})" : "";
                decimal outstanding = rental.TotalCost - rental.AmountPaid;

                checkBox.Text = $"Rental #{rental.RentalRecordID}\n" +
                    $"Product: {rental.ProductName} | Qty: {rental.Quantity} | Rate: {rental.RateType}\n" +
                    $"Start: {rental.StartDate:MMM dd, yyyy}{dueText}{overdueText}\n" +
                    $"Total: {MainMenu_Form.CurrencySymbol}{rental.TotalCost:N2} | Paid: {MainMenu_Form.CurrencySymbol}{rental.AmountPaid:N2} | Outstanding: {MainMenu_Form.CurrencySymbol}{outstanding:N2}";

                if (rental.IsOverdue)
                {
                    checkBox.ForeColor = Color.Red;
                }

                _rentalCheckBoxes.Add(checkBox);
                RentalsPanel.Controls.Add(checkBox);

                yPosition += 90;
            }

            if (_activeRentals.Count == 0)
            {
                Label label = new()
                {
                    Text = "No active rentals for this customer.",
                    Location = new Point(10, 10),
                    Size = new Size(650, 30),
                    Font = new Font("Segoe UI", 10F)
                };
                RentalsPanel.Controls.Add(label);
            }
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
        private void CustomerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CustomerComboBox.SelectedIndex < 0) return;

            List<Customer> customersWithRentals = MainMenu_Form.Instance.CustomerList
                .Where(c => c.GetActiveRentals().Count > 0)
                .OrderBy(c => c.LastName)
                .ToList();

            _selectedCustomer = customersWithRentals[CustomerComboBox.SelectedIndex];
            LoadActiveRentals();
        }
        private void SelectAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            foreach (CheckBox checkBox in _rentalCheckBoxes)
            {
                checkBox.Checked = SelectAllCheckBox.Checked;
            }
        }
        private void Return_Button_Click(object sender, EventArgs e)
        {
            if (_selectedCustomer == null)
            {
                CustomMessageBox.Show("No Customer Selected",
                    "Please select a customer.",
                    CustomMessageBoxIcon.Warning,
                    CustomMessageBoxButtons.Ok);
                return;
            }

            // Get selected rentals
            List<RentalRecord> selectedRentals = [];
            for (int i = 0; i < _rentalCheckBoxes.Count; i++)
            {
                if (_rentalCheckBoxes[i].Checked)
                {
                    selectedRentals.Add(_activeRentals[i]);
                }
            }

            if (selectedRentals.Count == 0)
            {
                CustomMessageBox.Show("No Rentals Selected",
                    "Please select at least one rental to return.",
                    CustomMessageBoxIcon.Warning,
                    CustomMessageBoxButtons.Ok);
                return;
            }

            DateTime returnDate = ReturnDatePicker.Value;
            string notes = NotesTextBox.Text.Trim();

            // Confirm return
            CustomMessageBoxResult result = CustomMessageBox.Show("Confirm Return",
                $"Are you sure you want to return {selectedRentals.Count} rental(s) for {_selectedCustomer.FullName}?\n\n" +
                $"Return Date: {returnDate:MMM dd, yyyy}",
                CustomMessageBoxIcon.Question,
                CustomMessageBoxButtons.YesNo);

            if (result != CustomMessageBoxResult.Yes) return;

            // Process returns
            ProcessReturns(selectedRentals, returnDate, notes);
        }
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // Methods
        private void ProcessReturns(List<RentalRecord> rentalsToReturn, DateTime returnDate, string notes)
        {
            try
            {
                foreach (RentalRecord rental in rentalsToReturn)
                {
                    // Mark rental record as returned
                    rental.ReturnDate = returnDate;
                    rental.IsActive = false;
                    rental.IsOverdue = false;

                    if (!string.IsNullOrWhiteSpace(notes))
                    {
                        rental.Notes = string.IsNullOrWhiteSpace(rental.Notes)
                            ? $"Return notes: {notes}"
                            : $"{rental.Notes}\nReturn notes: {notes}";
                    }

                    // Update rental item inventory
                    RentalItem rentalItem = RentalInventoryManager.GetRentalItem(rental.RentalItemID);
                    rentalItem?.ReturnItem(rental.Quantity);

                    // Update customer rental status
                    _selectedCustomer?.ReturnRental(rental.RentalRecordID);

                    // Update DataGridView row
                    UpdateDataGridViewRow(rental, returnDate);
                }

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
                string message = $"Returned {rentalsToReturn.Count} rental(s) for customer {_selectedCustomer?.FullName}";
                CustomMessage_Form.AddThingThatHasChangedAndLogMessage(AddRentalItem_Form.ThingsThatHaveChangedInFile, 2, message);

                CustomMessageBox.Show("Return Successful",
                    $"Successfully returned {rentalsToReturn.Count} rental(s) for {_selectedCustomer?.FullName}.",
                    CustomMessageBoxIcon.Success,
                    CustomMessageBoxButtons.Ok);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error",
                    $"An error occurred while processing returns: {ex.Message}",
                    CustomMessageBoxIcon.Error,
                    CustomMessageBoxButtons.Ok);
            }
        }
        private void UpdateDataGridViewRow(RentalRecord rental, DateTime returnDate)
        {
            // Find the row in the DataGridView that matches this rental
            foreach (DataGridViewRow row in _mainMenuForm.Rental_DataGridView.Rows)
            {
                if (row.Tag is TagData tagData && tagData.RentalRecordID == rental.RentalRecordID)
                {
                    // Update the tag data
                    tagData.IsReturned = true;
                    tagData.ReturnDate = returnDate;
                    row.Tag = tagData;

                    // Apply visual indicator (strikethrough and color)
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        cell.Style.Font = new Font(cell.Style.Font ?? row.DataGridView.DefaultCellStyle.Font, FontStyle.Strikeout);
                        cell.Style.ForeColor = Color.Gray;
                    }

                    // Add return date to notes column if exists
                    DataGridViewCell noteCell = row.Cells[ReadOnlyVariables.Note_column];
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

                    break;
                }
            }
        }
    }
}

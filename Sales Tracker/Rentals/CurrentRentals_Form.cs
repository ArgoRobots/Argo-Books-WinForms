using Guna.UI2.WinForms;
using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.GridView;
using Sales_Tracker.Language;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker.Rentals
{
    /// <summary>
    /// Form for viewing and managing current (active) rental transactions.
    /// </summary>
    public partial class CurrentRentals_Form : BaseForm
    {
        // Properties
        private static CurrentRentals_Form _instance;
        private readonly int _topForDataGridView;
        private bool _showOverdueOnly = false;

        // Getters
        public static CurrentRentals_Form Instance => _instance;

        // Init.
        public CurrentRentals_Form()
        {
            InitializeComponent();
            _instance = this;

            _topForDataGridView = ShowingResultsFor_Label.Bottom + 20;
            ConstructDataGridView();
            LoadCurrentRentals();
            UpdateTheme();
            Guna2TextBoxIconHoverEffect.Initialize(Search_TextBox);
            SetAccessibleDescriptions();
            LabelManager.ShowTotalLabel(Total_Label, CurrentRentals_DataGridView);
            ShowingResultsFor_Label.Visible = false;
            LanguageManager.UpdateLanguageForControl(this);
            DataGridViewManager.SortFirstColumnAndSelectFirstRow(CurrentRentals_DataGridView);
            AddEventHandlersToTextBoxes();

            PanelCloseFilter panelCloseFilter = new(this, ClosePanels,
                TextBoxManager.RightClickTextBox_Panel,
                RightClickDataGridViewRowMenu.Panel);

            Application.AddMessageFilter(panelCloseFilter);
            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void AddEventHandlersToTextBoxes()
        {
            TextBoxManager.Attach(Search_TextBox);

            CurrentRentals_DataGridView.RowsAdded += (_, _) => LabelManager.ShowTotalLabel(Total_Label, CurrentRentals_DataGridView);
            CurrentRentals_DataGridView.RowsRemoved += (_, _) => LabelManager.ShowTotalLabel(Total_Label, CurrentRentals_DataGridView);
        }
        private void SetAccessibleDescriptions()
        {
            ShowingResultsFor_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            Total_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);
        }

        // DataGridView properties
        public enum Column
        {
            RentalRecordID,
            CustomerName,
            CustomerID,
            ProductName,
            RentalItemID,
            Quantity,
            RateType,
            Rate,
            StartDate,
            DueDate,
            DaysOverdue,
            TotalCost,
            AmountPaid,
            OutstandingBalance,
            Status
        }
        public static readonly Dictionary<Column, string> ColumnHeaders = new()
        {
            { Column.RentalRecordID, "Rental ID" },
            { Column.CustomerName, "Customer" },
            { Column.CustomerID, "Customer ID" },
            { Column.ProductName, "Product" },
            { Column.RentalItemID, "Item ID" },
            { Column.Quantity, "Qty" },
            { Column.RateType, "Rate Type" },
            { Column.Rate, "Rate" },
            { Column.StartDate, "Start Date" },
            { Column.DueDate, "Due Date" },
            { Column.DaysOverdue, "Days Overdue" },
            { Column.TotalCost, "Total Cost" },
            { Column.AmountPaid, "Paid" },
            { Column.OutstandingBalance, "Outstanding" },
            { Column.Status, "Status" }
        };
        private Guna2DataGridView CurrentRentals_DataGridView;

        // DataGridView methods
        private void ConstructDataGridView()
        {
            CurrentRentals_DataGridView = new();
            DataGridViewManager.InitializeDataGridView(CurrentRentals_DataGridView, "currentRentals_DataGridView", ColumnHeaders, null, this);
            CurrentRentals_DataGridView.Size = new Size(ClientSize.Width - 80, ClientSize.Height - _topForDataGridView - 70);
            CurrentRentals_DataGridView.Location = new Point((ClientSize.Width - CurrentRentals_DataGridView.Width) / 2, _topForDataGridView);
            CurrentRentals_DataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            CurrentRentals_DataGridView.CellFormatting += DataGridView_CellFormatting;
            CurrentRentals_DataGridView.CellMouseDown += DataGridView_CellMouseDown;

            // Align controls
            Search_TextBox.Left = CurrentRentals_DataGridView.Right - Search_TextBox.Width;
            FilterOverdue_Label.Left = Search_TextBox.Left - FilterOverdue_Label.Width - CustomControls.SpaceBetweenControls;
            FilterOverdue_CheckBox.Left = FilterOverdue_Label.Left - FilterOverdue_CheckBox.Width - 5;
        }
        private void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;

            // Color code status
            if (grid.Columns[e.ColumnIndex].Name == Column.Status.ToString())
            {
                if (e.Value?.ToString() == "Overdue")
                {
                    e.CellStyle.ForeColor = CustomColors.AccentRed;
                    e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
                }
                else if (e.Value?.ToString() == "Active")
                {
                    e.CellStyle.ForeColor = CustomColors.AccentGreen;
                }
            }

            // Highlight overdue rentals
            if (grid.Columns[e.ColumnIndex].Name == Column.DaysOverdue.ToString())
            {
                if (e.Value != null && int.TryParse(e.Value.ToString(), out int days) && days > 0)
                {
                    e.CellStyle.ForeColor = CustomColors.AccentRed;
                    e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
                }
            }

            // Format currency columns
            string[] currencyColumns = [
                Column.Rate.ToString(),
                Column.TotalCost.ToString(),
                Column.AmountPaid.ToString(),
                Column.OutstandingBalance.ToString()
            ];

            if (currencyColumns.Contains(grid.Columns[e.ColumnIndex].Name))
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = $"{MainMenu_Form.CurrencySymbol}{value:N2}";
                    e.FormattingApplied = true;
                }
            }
        }
        private void DataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                CurrentRentals_DataGridView.ClearSelection();
                CurrentRentals_DataGridView.Rows[e.RowIndex].Selected = true;
                ShowContextMenu(e.RowIndex);
            }
        }
        private void ShowContextMenu(int rowIndex)
        {
            DataGridViewRow row = CurrentRentals_DataGridView.Rows[rowIndex];

            if (row.Tag is not RentalRecord rentalRecord) { return; }

            Customer customer = MainMenu_Form.Instance.CustomerList.FirstOrDefault(c => c.CustomerID == rentalRecord.CustomerID);

            ContextMenuStrip contextMenu = new();
            contextMenu.Items.Add("Return Rental", null, (s, e) => ReturnRental(rentalRecord, customer));
            contextMenu.Items.Add("View Customer Details", null, (s, e) => ViewCustomerDetails(customer));
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Refresh List", null, (s, e) => RefreshDataGridView());

            contextMenu.Show(CurrentRentals_DataGridView, CurrentRentals_DataGridView.PointToClient(Cursor.Position));
        }
        private void LoadCurrentRentals()
        {
            foreach (RentalItem item in RentalInventoryManager.RentalInventory)
            {
                foreach (RentalRecord record in item.RentalRecords)
                {
                    if (!record.IsActive) continue;

                    // Apply filter
                    if (_showOverdueOnly && !record.IsOverdue) continue;

                    record.CheckOverdueStatus();

                    Customer customer = MainMenu_Form.Instance.CustomerList.FirstOrDefault(c => c.CustomerID == record.CustomerID);
                    string customerName = customer?.FullName ?? "Unknown";

                    int daysOverdue = 0;
                    if (record.IsOverdue && record.DueDate.HasValue)
                    {
                        daysOverdue = (int)(DateTime.Now - record.DueDate.Value).TotalDays;
                    }

                    string status = record.IsOverdue ? "Overdue" : "Active";

                    int rowIndex = CurrentRentals_DataGridView.Rows.Add(
                        record.RentalRecordID,
                        customerName,
                        record.CustomerID,
                        record.ProductName,
                        record.RentalItemID,
                        record.Quantity,
                        record.RateType.ToString(),
                        record.Rate,
                        record.StartDate.ToString("yyyy-MM-dd"),
                        record.DueDate?.ToString("yyyy-MM-dd") ?? "-",
                        daysOverdue,
                        record.TotalCost,
                        record.AmountPaid,
                        record.RemainingBalance,
                        status);

                    CurrentRentals_DataGridView.Rows[rowIndex].Tag = record;

                    // Highlight overdue rows
                    if (record.IsOverdue)
                    {
                        CurrentRentals_DataGridView.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 240, 240);
                    }
                }
            }

            DataGridViewManager.ScrollToTopOfDataGridView(CurrentRentals_DataGridView);
        }

        // Form event handlers
        private void CurrentRentals_Form_Shown(object sender, EventArgs e)
        {
            CurrentRentals_DataGridView.ClearSelection();
            LoadingPanel.HideBlankLoadingPanel(this);
        }
        private void CurrentRentals_Form_Resize(object sender, EventArgs e)
        {
            ClosePanels();
        }
        private void CurrentRentals_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClosePanels();
            _instance = null;
        }

        // Event handlers
        private void FilterOverdue_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _showOverdueOnly = FilterOverdue_CheckBox.Checked;
            RefreshDataGridView();
        }
        private void FilterOverdue_Label_Click(object sender, EventArgs e)
        {
            FilterOverdue_CheckBox.Checked = !FilterOverdue_CheckBox.Checked;
        }
        private void Search_TextBox_TextChanged(object sender, EventArgs e)
        {
            if (DataGridViewManager.SearchSelectedDataGridViewAndUpdateRowColors(CurrentRentals_DataGridView, Search_TextBox))
            {
                LabelManager.ShowLabelWithBaseText(ShowingResultsFor_Label, Search_TextBox.Text.Trim());
            }
            else
            {
                ShowingResultsFor_Label.Visible = false;
            }
            LabelManager.ShowTotalLabel(Total_Label, CurrentRentals_DataGridView);
        }
        private void Search_TextBox_IconRightClick(object sender, EventArgs e)
        {
            Search_TextBox.Clear();
        }

        // Methods
        private void ReturnRental(RentalRecord record, Customer customer)
        {
            if (record == null || customer == null) { return; }

            ReturnRental_Form returnForm = new(customer, record);
            if (returnForm.ShowDialog() == DialogResult.OK)
            {
                RefreshDataGridView();
                Rentals_Form.Instance?.RefreshDataGridView();
            }
        }
        private static void ViewCustomerDetails(Customer customer)
        {
            if (customer == null) { return; }

            // Open Customers_Form if not already open
            if (Customers_Form.Instance == null)
            {
                Tools.OpenForm(new Customers_Form());
            }

            // Find the customer row in the Customers_Form DataGridView
            if (Customers_Form.Instance != null)
            {
                foreach (DataGridViewRow row in Customers_Form.Instance.Customers_DataGridView.Rows)
                {
                    if (row.Cells[Customers_Form.Column.CustomerID.ToString()].Value?.ToString() == customer.CustomerID)
                    {
                        // Open ModifyRow_Form for this customer
                        Tools.OpenForm(new ModifyRow_Form(row));
                        break;
                    }
                }
            }
        }
        public void RefreshDataGridView()
        {
            CurrentRentals_DataGridView.Rows.Clear();
            LoadCurrentRentals();
            DataGridViewManager.UpdateRowColors(CurrentRentals_DataGridView);
            LabelManager.ShowTotalLabel(Total_Label, CurrentRentals_DataGridView);
        }
        private void ClosePanels()
        {
            TextBoxManager.HideRightClickPanel();
            RightClickDataGridViewRowMenu.Hide();
        }
    }
}

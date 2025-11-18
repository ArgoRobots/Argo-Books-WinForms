using Guna.UI2.WinForms;
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
    /// Form for viewing and managing rental transactions.
    /// </summary>
    public partial class Rentals_Form : BaseForm
    {
        // Properties
        private static Rentals_Form _instance;
        private readonly int _topForDataGridView;

        // Getters
        public static Rentals_Form Instance => _instance;

        // Init.
        public Rentals_Form()
        {
            InitializeComponent();
            _instance = this;

            _topForDataGridView = ShowingResultsFor_Label.Bottom + 20;
            ConstructDataGridView();
            LoadRentalInventory();
            UpdateTheme();
            Guna2TextBoxIconHoverEffect.Initialize(Search_TextBox);
            SetAccessibleDescriptions();
            LabelManager.ShowTotalLabel(Total_Label, RentalInventory_DataGridView);
            ShowingResultsFor_Label.Visible = false;
            LanguageManager.UpdateLanguageForControl(this);
            DataGridViewManager.SortFirstColumnAndSelectFirstRow(RentalInventory_DataGridView);
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

            RentalInventory_DataGridView.RowsAdded += (_, _) => LabelManager.ShowTotalLabel(Total_Label, RentalInventory_DataGridView);
            RentalInventory_DataGridView.RowsRemoved += (_, _) => LabelManager.ShowTotalLabel(Total_Label, RentalInventory_DataGridView);
        }
        private void SetAccessibleDescriptions()
        {
            ShowingResultsFor_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            Total_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);
            ThemeManager.MakeGButtonBluePrimary(AddRentalItem_Button);
        }

        // DataGridView properties
        public enum Column
        {
            RentalItemID,
            ProductName,
            CompanyName,
            Status,
            TotalQuantity,
            Available,
            Rented,
            Maintenance,
            DailyRate,
            WeeklyRate,
            MonthlyRate,
            SecurityDeposit,
            DateAdded,
            LastRentalDate
        }
        public static readonly Dictionary<Column, string> ColumnHeaders = new()
        {
            { Column.RentalItemID, "Item ID" },
            { Column.ProductName, "Product" },
            { Column.CompanyName, "Company" },
            { Column.Status, "Status" },
            { Column.TotalQuantity, "Total Qty" },
            { Column.Available, "Available" },
            { Column.Rented, "Rented" },
            { Column.Maintenance, "Maintenance" },
            { Column.DailyRate, "Daily Rate" },
            { Column.WeeklyRate, "Weekly Rate" },
            { Column.MonthlyRate, "Monthly Rate" },
            { Column.SecurityDeposit, "Deposit" },
            { Column.DateAdded, "Date Added" },
            { Column.LastRentalDate, "Last Rental" }
        };
        private Guna2DataGridView RentalInventory_DataGridView;

        // DataGridView methods
        private void ConstructDataGridView()
        {
            RentalInventory_DataGridView = new();
            DataGridViewManager.InitializeDataGridView(RentalInventory_DataGridView, "rentalInventory_DataGridView", ColumnHeaders, null, this);
            RentalInventory_DataGridView.Size = new Size(ClientSize.Width - 80, ClientSize.Height - _topForDataGridView - 70);
            RentalInventory_DataGridView.Location = new Point((ClientSize.Width - RentalInventory_DataGridView.Width) / 2, _topForDataGridView);
            RentalInventory_DataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            RentalInventory_DataGridView.Tag = MainMenu_Form.DataGridViewTag.RentalInventory;
            RentalInventory_DataGridView.CellFormatting += DataGridView_CellFormatting;

            // Align controls
            Search_TextBox.Left = RentalInventory_DataGridView.Right - Search_TextBox.Width;
            AddRentalItem_Button.Left = Search_TextBox.Left - AddRentalItem_Button.Width - CustomControls.SpaceBetweenControls;
        }
        private void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;

            // Color code status
            if (grid.Columns[e.ColumnIndex].Name == Column.Status.ToString())
            {
                if (e.Value?.ToString() == "Available")
                {
                    e.CellStyle.ForeColor = CustomColors.AccentGreen;
                }
                else if (e.Value?.ToString() == "Rented")
                {
                    e.CellStyle.ForeColor = Color.Orange;
                }
                else if (e.Value?.ToString() == "Maintenance")
                {
                    e.CellStyle.ForeColor = CustomColors.AccentRed;
                }
            }

            // Format currency columns
            string[] currencyColumns = [
                Column.DailyRate.ToString(),
                Column.WeeklyRate.ToString(),
                Column.MonthlyRate.ToString(),
                Column.SecurityDeposit.ToString()
            ];

            if (currencyColumns.Contains(grid.Columns[e.ColumnIndex].Name))
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    if (value == 0 && (grid.Columns[e.ColumnIndex].Name == Column.DailyRate.ToString() ||
                                      grid.Columns[e.ColumnIndex].Name == Column.WeeklyRate.ToString() ||
                                      grid.Columns[e.ColumnIndex].Name == Column.MonthlyRate.ToString()))
                    {
                        e.Value = ReadOnlyVariables.EmptyCell;
                    }
                    else
                    {
                        e.Value = $"{MainMenu_Form.CurrencySymbol}{value:N2}";
                    }
                    e.FormattingApplied = true;
                }
            }
        }
        private void LoadRentalInventory()
        {
            foreach (RentalItem item in RentalInventoryManager.RentalInventory)
            {
                int rowIndex = RentalInventory_DataGridView.Rows.Add(
                    item.RentalItemID,
                    item.ProductName,
                    item.CompanyName,
                    item.Status.ToString(),
                    item.TotalQuantity,
                    item.QuantityAvailable,
                    item.QuantityRented,
                    item.QuantityInMaintenance,
                    item.DailyRate,
                    item.WeeklyRate ?? 0m,
                    item.MonthlyRate ?? 0m,
                    item.SecurityDeposit,
                    item.DateAdded.ToString("yyyy-MM-dd"),
                    item.LastRentalDate?.ToString("yyyy-MM-dd") ?? "-");

                RentalInventory_DataGridView.Rows[rowIndex].Tag = item;
            }

            DataGridViewManager.ScrollToTopOfDataGridView(RentalInventory_DataGridView);
        }

        // Form event handlers
        private void ManageRentals_Form_Shown(object sender, EventArgs e)
        {
            RentalInventory_DataGridView.ClearSelection();
            LoadingPanel.HideBlankLoadingPanel(this);
        }
        private void ManageRentals_Form_Resize(object sender, EventArgs e)
        {
            ClosePanels();
        }
        private void ManageRentals_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClosePanels();
        }

        // Event handlers
        private void AddRentalItem_Button_Click(object sender, EventArgs e)
        {
            Tools.OpenForm(new AddRentalItem_Form());
        }
        private void Search_TextBox_TextChanged(object sender, EventArgs e)
        {
            if (DataGridViewManager.SearchSelectedDataGridViewAndUpdateRowColors(RentalInventory_DataGridView, Search_TextBox))
            {
                LabelManager.ShowLabelWithBaseText(ShowingResultsFor_Label, Search_TextBox.Text.Trim());
            }
            else
            {
                ShowingResultsFor_Label.Visible = false;
            }
            LabelManager.ShowTotalLabel(Total_Label, RentalInventory_DataGridView);
        }
        private void Search_TextBox_IconRightClick(object sender, EventArgs e)
        {
            Search_TextBox.Clear();
        }

        // Methods
        public void RefreshDataGridView()
        {
            RentalInventory_DataGridView.Rows.Clear();
            LoadRentalInventory();
            DataGridViewManager.UpdateRowColors(RentalInventory_DataGridView);
            LabelManager.ShowTotalLabel(Total_Label, RentalInventory_DataGridView);
        }
        private void ClosePanels()
        {
            TextBoxManager.HideRightClickPanel();
            RightClickDataGridViewRowMenu.Hide();
        }
    }
}
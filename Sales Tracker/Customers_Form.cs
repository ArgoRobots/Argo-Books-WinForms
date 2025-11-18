using Guna.UI2.WinForms;
using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.GridView;
using Sales_Tracker.Language;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker
{
    /// <summary>
    /// Form for managing customer information and records.
    /// </summary>
    public partial class Customers_Form : BaseForm
    {
        // Properties
        private static Customers_Form _instance;

        // Getters
        public static List<string> ThingsThatHaveChangedInFile { get; } = [];
        public static Customers_Form Instance => _instance;

        public Customers_Form()
        {
            InitializeComponent();
            _instance = this;

            ConstructDataGridView();
            LoadCustomers();
            UpdateTheme();
            Guna2TextBoxIconHoverEffect.Initialize(Search_TextBox);
            SetAccessibleDescriptions();
            ShowingResultsFor_Label.Visible = false;
            LanguageManager.UpdateLanguageForControl(this);
            LabelManager.ShowTotalLabel(Total_Label, _customers_DataGridView);
            DataGridViewManager.SortFirstColumnAndSelectFirstRow(_customers_DataGridView);

            TextBoxManager.Attach(Search_TextBox);
            _customers_DataGridView.RowsAdded += (_, _) => LabelManager.ShowTotalLabel(Total_Label, _customers_DataGridView);
            _customers_DataGridView.RowsRemoved += (_, _) => LabelManager.ShowTotalLabel(Total_Label, _customers_DataGridView);

            PanelCloseFilter panelCloseFilter = new(this, ClosePanels,
                TextBoxManager.RightClickTextBox_Panel,
                RightClickDataGridViewRowMenu.Panel);

            Application.AddMessageFilter(panelCloseFilter);

            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void SetAccessibleDescriptions()
        {
            ShowingResultsFor_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            Total_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);
            ThemeManager.MakeGButtonBluePrimary(AddCustomer_Button);
        }

        // Form event handlers
        private void Customers_Form_Shown(object sender, EventArgs e)
        {
            _customers_DataGridView.ClearSelection();
            LoadingPanel.HideBlankLoadingPanel(this);
        }
        private void Customers_Form_Resize(object sender, EventArgs e)
        {
            ClosePanels();
        }
        private void Customers_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClosePanels();
        }

        // Event handlers
        private void AddCustomer_Button_Click(object sender, EventArgs e)
        {
            Tools.OpenForm(new AddCustomer_Form());
        }
        private void Search_TextBox_TextChanged(object sender, EventArgs e)
        {
            if (DataGridViewManager.SearchSelectedDataGridViewAndUpdateRowColors(_customers_DataGridView, Search_TextBox))
            {
                LabelManager.ShowLabelWithBaseText(ShowingResultsFor_Label, Search_TextBox.Text.Trim());
            }
            else
            {
                ShowingResultsFor_Label.Visible = false;
            }
            LabelManager.ShowTotalLabel(Total_Label, _customers_DataGridView);
        }
        private void Search_TextBox_IconRightClick(object sender, EventArgs e)
        {
            Search_TextBox.Clear();
        }

        // DataGridView properties
        public enum Column
        {
            CustomerID,
            FirstName,
            LastName,
            Email,
            PhoneNumber,
            Address,
            PaymentStatus,
            OutstandingBalance,
            IsBanned,
            TotalRentals,
            LastRentalDate
        }
        public static readonly Dictionary<Column, string> ColumnHeaders = new()
        {
            { Column.CustomerID, "Customer ID" },
            { Column.FirstName, "First name" },
            { Column.LastName, "Last name" },
            { Column.Email, "Email" },
            { Column.PhoneNumber, "Phone number" },
            { Column.Address, "Address" },
            { Column.PaymentStatus, "Payment status" },
            { Column.OutstandingBalance, "Outstanding balance" },
            { Column.IsBanned, "Banned" },
            { Column.TotalRentals, "Total rentals" },
            { Column.LastRentalDate, "Last rental date" }
        };

        private Guna2DataGridView _customers_DataGridView;
        public Guna2DataGridView Customers_DataGridView => _customers_DataGridView;

        // DataGridView methods
        private void ConstructDataGridView()
        {
            int topForDataGridView = ShowingResultsFor_Label.Bottom + 20;

            _customers_DataGridView = new();
            DataGridViewManager.InitializeDataGridView(_customers_DataGridView, "customers_DataGridView", ColumnHeaders, null, this);
            _customers_DataGridView.Size = new Size(ClientSize.Width - 80, ClientSize.Height - topForDataGridView - 70);
            _customers_DataGridView.Location = new Point((ClientSize.Width - _customers_DataGridView.Width) / 2, topForDataGridView);
            _customers_DataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            _customers_DataGridView.Tag = MainMenu_Form.DataGridViewTag.Customer;
            _customers_DataGridView.CellFormatting += DataGridView_CellFormatting;

            // Align controls
            Search_TextBox.Left = _customers_DataGridView.Right - Search_TextBox.Width;
            AddCustomer_Button.Left = Search_TextBox.Left - AddCustomer_Button.Width - CustomControls.SpaceBetweenControls;
        }
        private void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;

            // Color code payment status
            if (grid.Columns[e.ColumnIndex].Name == Column.PaymentStatus.ToString())
            {
                if (e.Value?.ToString() == "Current")
                {
                    e.CellStyle.ForeColor = CustomColors.AccentGreen;
                }
                else if (e.Value?.ToString() == "Overdue")
                {
                    e.CellStyle.ForeColor = Color.Orange;
                }
                else if (e.Value?.ToString() == "Delinquent")
                {
                    e.CellStyle.ForeColor = CustomColors.AccentRed;
                }
            }

            if (grid.Columns[e.ColumnIndex].Name == Column.IsBanned.ToString())
            {
                if (e.Value is bool isBanned)
                {
                    if (isBanned)
                    {
                        e.Value = "✓";
                        e.CellStyle.ForeColor = CustomColors.AccentGreen;
                    }
                    else
                    {
                        e.Value = "✗";
                        e.CellStyle.ForeColor = CustomColors.AccentRed;
                    }

                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    e.FormattingApplied = true;
                }
            }
        }
        private void LoadCustomers()
        {
            foreach (Customer customer in MainMenu_Form.Instance.CustomerList)
            {
                int rowIndex = _customers_DataGridView.Rows.Add(
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

                _customers_DataGridView.Rows[rowIndex].Tag = customer;
            }
            DataGridViewManager.ScrollToTopOfDataGridView(_customers_DataGridView);
        }

        // Methods
        private void ClosePanels()
        {
            TextBoxManager.HideRightClickPanel();
            RightClickDataGridViewRowMenu.Hide();
        }
    }
}
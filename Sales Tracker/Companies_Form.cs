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
    /// Form for managing companies in the system.
    /// </summary>
    public partial class Companies_Form : BaseForm
    {
        // Properties
        private static Companies_Form _instance;
        private readonly int _topForDataGridView;

        // Getters
        public static Companies_Form Instance => _instance;
        public static List<string> ThingsThatHaveChangedInFile { get; } = [];

        // Init.
        public Companies_Form()
        {
            InitializeComponent();
            _instance = this;

            _topForDataGridView = ShowingResultsFor_Label.Bottom + 20;
            ConstructDataGridViews();
            LoadCompanies();
            ThemeManager.SetThemeForForm(this);
            Guna2TextBoxIconHoverEffect.Initialize(Search_TextBox);
            SetAccessibleDescriptions();
            LanguageManager.UpdateLanguageForControl(this);
            LabelManager.ShowTotalLabel(Total_Label, _company_DataGridView);
            ShowingResultsFor_Label.Visible = false;
            DataGridViewManager.SortFirstColumnAndSelectFirstRow(_company_DataGridView);
            AddEventHandlersToTextBoxes();

            PanelCloseFilter panelCloseFilter = new(this, ClosePanels, TextBoxManager.RightClickTextBox_Panel, RightClickDataGridViewRowMenu.Panel);
            Application.AddMessageFilter(panelCloseFilter);

            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void LoadCompanies()
        {
            MainMenu_Form.IsProgramLoading = true;

            foreach (string accountant in MainMenu_Form.Instance.CompanyList)
            {
                _company_DataGridView.Rows.Add(accountant);
            }
            DataGridViewManager.ScrollToTopOfDataGridView(_company_DataGridView);
            MainMenu_Form.IsProgramLoading = false;
        }
        private void AddEventHandlersToTextBoxes()
        {
            TextBoxManager.Attach(Company_TextBox);
            TextBoxManager.Attach(Search_TextBox);

            _company_DataGridView.RowsAdded += (_, _) => LabelManager.ShowTotalLabel(Total_Label, _company_DataGridView);
            _company_DataGridView.RowsRemoved += (_, _) => LabelManager.ShowTotalLabel(Total_Label, _company_DataGridView);
        }
        private void SetAccessibleDescriptions()
        {
            CompanyName_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            ShowingResultsFor_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            Total_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
        }

        // Form event handlers
        private void Companies_Form_Shown(object sender, EventArgs e)
        {
            _company_DataGridView.ClearSelection();
            LoadingPanel.HideBlankLoadingPanel(this);
        }
        private void Companies_Form_Resize(object sender, EventArgs e)
        {
            ClosePanels();
        }
        private void Companies_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClosePanels();
        }

        // Event handlers
        private void AddCompany_Button_Click(object sender, EventArgs e)
        {
            string name = Company_TextBox.Text.Trim();
            MainMenu_Form.Instance.CompanyList.Add(name);
            int newRowIndex = _company_DataGridView.Rows.Add(name);
            DataGridViewManager.DataGridViewRowsAdded(_company_DataGridView, new DataGridViewRowsAddedEventArgs(newRowIndex, 1));

            string message = $"Added company '{name}'";
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(ThingsThatHaveChangedInFile, 2, message);

            Company_TextBox.Clear();
            CustomControls.SetGTextBoxToValid(Company_TextBox);
            HideCompanyWarning();
            Company_TextBox.Focus();
        }
        private void Company_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (AddCompany_Button.Enabled)
                {
                    AddCompany_Button.PerformClick();
                }
            }
        }
        private void Company_TextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateInputs();
            ValidateCompanyTextBox();
        }
        private void Search_TextBox_TextChanged(object sender, EventArgs e)
        {
            if (DataGridViewManager.SearchSelectedDataGridViewAndUpdateRowColors(_company_DataGridView, Search_TextBox))
            {
                LabelManager.ShowLabelWithBaseText(ShowingResultsFor_Label, Search_TextBox.Text.Trim());
            }
            else
            {
                ShowingResultsFor_Label.Visible = false;
            }
            LabelManager.ShowTotalLabel(Total_Label, _company_DataGridView);
        }
        private void Search_TextBox_IconRightClick(object sender, EventArgs e)
        {
            Search_TextBox.Clear();
        }

        // DataGridView
        public enum Column
        {
            Company
        }
        public static readonly Dictionary<Enum, string> ColumnHeaders = new()
        {
            { Column.Company, "Company" },
        };
        private Guna2DataGridView _company_DataGridView;
        private void ConstructDataGridViews()
        {
            _company_DataGridView = new();
            DataGridViewManager.InitializeDataGridView(_company_DataGridView, "company_DataGridView", ColumnHeaders, null, this);
            _company_DataGridView.Size = new Size(ClientSize.Width - 80, ClientSize.Height - _topForDataGridView - 70);
            _company_DataGridView.Location = new Point((ClientSize.Width - _company_DataGridView.Width) / 2, _topForDataGridView);
            _company_DataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            _company_DataGridView.Tag = MainMenu_Form.DataGridViewTag.Company;

            Controls.Add(_company_DataGridView);
        }

        // Validate company name
        public void ValidateCompanyTextBox()
        {
            bool exists = MainMenu_Form.Instance.CompanyList.Any(a => string.Equals(a, Company_TextBox.Text.Trim(), StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                AddCompany_Button.Enabled = false;
                CustomControls.SetGTextBoxToInvalid(Company_TextBox);
                ShowCompanyWarning();
            }
            else
            {
                if (Company_TextBox.Text != "")
                {
                    AddCompany_Button.Enabled = true;
                }
                CustomControls.SetGTextBoxToValid(Company_TextBox);
                HideCompanyWarning();
            }
        }
        private void ShowCompanyWarning()
        {
            WarningCompanyName_PictureBox.Visible = true;
            WarningCompanyName_Label.Visible = true;
            AddCompany_Button.Enabled = false;
            AddCompany_Button.Tag = false;
        }
        private void HideCompanyWarning()
        {
            WarningCompanyName_PictureBox.Visible = false;
            WarningCompanyName_Label.Visible = false;

            if (Company_TextBox.Text != "")
            {
                AddCompany_Button.Enabled = true;
                AddCompany_Button.Tag = true;
            }
        }

        // Methods
        private void ValidateInputs()
        {
            AddCompany_Button.Enabled = !string.IsNullOrWhiteSpace(Company_TextBox.Text) && Company_TextBox.Tag.ToString() != "0";
        }
        private void ClosePanels()
        {
            TextBoxManager.HideRightClickPanel();
            RightClickDataGridViewRowMenu.Hide();
        }
    }
}
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
    /// Form for managing accountants in the system.
    /// </summary>
    public partial class Accountants_Form : BaseForm
    {
        // Properties
        private static Accountants_Form _instance;
        private readonly int _topForDataGridView;

        // Getters
        public static Accountants_Form Instance => _instance;
        public static List<string> ThingsThatHaveChangedInFile { get; } = [];

        // Init.
        public Accountants_Form()
        {
            InitializeComponent();
            _instance = this;

            _topForDataGridView = ShowingResultsFor_Label.Bottom + 20;
            ConstructDataGridViews();
            LoadAccountants();
            ThemeManager.SetThemeForForm(this);
            Guna2TextBoxIconHoverEffect.Initialize(Search_TextBox);
            SetAccessibleDescriptions();
            LanguageManager.UpdateLanguageForControl(this);
            LabelManager.ShowTotalLabel(Total_Label, _accountant_DataGridView);
            ShowingResultsFor_Label.Visible = false;
            DataGridViewManager.SortFirstColumnAndSelectFirstRow(_accountant_DataGridView);
            AddEventHandlersToTextBoxes();

            PanelCloseFilter panelCloseFilter = new(this, ClosePanels, TextBoxManager.RightClickTextBox_Panel, RightClickDataGridViewRowMenu.Panel);
            Application.AddMessageFilter(panelCloseFilter);

            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void LoadAccountants()
        {
            foreach (string accountant in MainMenu_Form.Instance.AccountantList)
            {
                _accountant_DataGridView.Rows.Add(accountant);
            }
            DataGridViewManager.ScrollToTopOfDataGridView(_accountant_DataGridView);
        }
        private void AddEventHandlersToTextBoxes()
        {
            TextBoxValidation.OnlyAllowLetters(Accountant_TextBox);
            TextBoxManager.Attach(Accountant_TextBox);
            TextBoxManager.Attach(Search_TextBox);

            _accountant_DataGridView.RowsAdded += (_, _) => LabelManager.ShowTotalLabel(Total_Label, _accountant_DataGridView);
            _accountant_DataGridView.RowsRemoved += (_, _) => LabelManager.ShowTotalLabel(Total_Label, _accountant_DataGridView);
        }
        private void SetAccessibleDescriptions()
        {
            AccountantName_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            ShowingResultsFor_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            Total_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
        }

        // Form event handlers
        private void Accountants_Form_Shown(object sender, EventArgs e)
        {
            _accountant_DataGridView.ClearSelection();
            LoadingPanel.HideBlankLoadingPanel(this);
        }
        private void Accountants_Form_Resize(object sender, EventArgs e)
        {
            ClosePanels();
        }
        private void Accountants_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClosePanels();
        }

        // Event handlers
        private void AddAccountant_Button_Click(object sender, EventArgs e)
        {
            string name = Accountant_TextBox.Text.Trim();
            MainMenu_Form.Instance.AccountantList.Add(name);
            int newRowIndex = _accountant_DataGridView.Rows.Add(name);
            DataGridViewManager.DataGridViewRowsAdded(_accountant_DataGridView, new DataGridViewRowsAddedEventArgs(newRowIndex, 1));

            string message = $"Added accountant '{name}'";
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(ThingsThatHaveChangedInFile, 2, message);

            Accountant_TextBox.Clear();
            CustomControls.SetGTextBoxToValid(Accountant_TextBox);
            HideAccountantWarning();
            Accountant_TextBox.Focus();
        }
        private void Accountant_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (AddAccountant_Button.Enabled)
                {
                    AddAccountant_Button.PerformClick();
                }
            }
        }
        private void Accountant_TextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateInputs();
            VaidateAccountantTextBox();
        }
        private void Search_TextBox_TextChanged(object sender, EventArgs e)
        {
            if (DataGridViewManager.SearchSelectedDataGridViewAndUpdateRowColors(_accountant_DataGridView, Search_TextBox))
            {
                LabelManager.ShowLabelWithBaseText(ShowingResultsFor_Label, Search_TextBox.Text.Trim());
            }
            else
            {
                ShowingResultsFor_Label.Visible = false;
            }
            LabelManager.ShowTotalLabel(Total_Label, _accountant_DataGridView);
        }
        private void Search_TextBox_IconRightClick(object sender, EventArgs e)
        {
            Search_TextBox.Clear();
        }

        // DataGridView
        public enum Column
        {
            AccountantName
        }
        public static readonly Dictionary<Enum, string> ColumnHeaders = new()
        {
            { Column.AccountantName, "Accountant" },
        };
        private Guna2DataGridView _accountant_DataGridView;
        private void ConstructDataGridViews()
        {
            _accountant_DataGridView = new();
            DataGridViewManager.InitializeDataGridView(_accountant_DataGridView, "accountants_DataGridView", ColumnHeaders, null, this);
            _accountant_DataGridView.Size = new Size(ClientSize.Width - 80, ClientSize.Height - _topForDataGridView - 70);
            _accountant_DataGridView.Location = new Point((ClientSize.Width - _accountant_DataGridView.Width) / 2, _topForDataGridView);
            _accountant_DataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            _accountant_DataGridView.Tag = MainMenu_Form.DataGridViewTag.Accountant;

            Controls.Add(_accountant_DataGridView);
        }

        // Validate accountant name
        public void VaidateAccountantTextBox()
        {
            bool exists = MainMenu_Form.Instance.AccountantList.Any(a =>
                string.Equals(a, Accountant_TextBox.Text.Trim(), StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                AddAccountant_Button.Enabled = false;
                CustomControls.SetGTextBoxToInvalid(Accountant_TextBox);
                ShowAccountantWarning();
            }
            else
            {
                if (Accountant_TextBox.Text != "")
                {
                    AddAccountant_Button.Enabled = true;
                }
                CustomControls.SetGTextBoxToValid(Accountant_TextBox);
                HideAccountantWarning();
            }
        }
        private void ShowAccountantWarning()
        {
            WarningAccountantName_PictureBox.Visible = true;
            WarningAccountantName_Label.Visible = true;
            AddAccountant_Button.Enabled = false;
            AddAccountant_Button.Tag = false;
        }
        private void HideAccountantWarning()
        {
            WarningAccountantName_PictureBox.Visible = false;
            WarningAccountantName_Label.Visible = false;

            if (Accountant_TextBox.Text != "")
            {
                AddAccountant_Button.Enabled = true;
                AddAccountant_Button.Tag = true;
            }
        }

        // Methods
        private void ValidateInputs()
        {
            if (AddAccountant_Button.Tag is bool and true)
            {
                AddAccountant_Button.Enabled = !string.IsNullOrWhiteSpace(Accountant_TextBox.Text)
                    && Accountant_TextBox.Tag.ToString() != "0";
            }
        }
        private void ClosePanels()
        {
            TextBoxManager.HideRightClickPanel();
            RightClickDataGridViewRowMenu.Hide();
        }
    }
}
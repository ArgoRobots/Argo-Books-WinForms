using Guna.UI2.WinForms;
using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.GridView;
using Sales_Tracker.Language;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;
using System.ComponentModel;

namespace Sales_Tracker
{
    /// <summary>
    /// Form for managing product categories.
    /// </summary>
    public partial class Categories_Form : BaseForm
    {
        // Properties
        private static Categories_Form _instance;
        private readonly int _topForDataGridView;

        // Getters
        public static Categories_Form Instance => _instance;
        public static List<string> ThingsThatHaveChangedInFile { get; } = [];

        // Init.
        public Categories_Form() : this(false) { }  // This is needed for TranslationGenerator.GenerateAllLanguageTranslationFiles()
        public Categories_Form(bool checkPurchaseRadioButton)
        {
            InitializeComponent();
            _instance = this;

            _topForDataGridView = ShowingResultsFor_Label.Bottom + 20;
            ConstructDataGridViews();
            LoadCategories();
            CheckRadioButton(checkPurchaseRadioButton);
            UpdateTheme();
            Guna2TextBoxIconHoverEffect.Initialize(Search_TextBox);
            SetAccessibleDescriptions();
            LabelManager.ShowTotalLabel(Total_Label, selectedDataGridView);
            ShowingResultsFor_Label.Visible = false;
            LanguageManager.UpdateLanguageForControl(this);
            AdjustRadioButtonPositions();
            DataGridViewManager.SortFirstColumnAndSelectFirstRow(Purchase_DataGridView, Sale_DataGridView, Rent_DataGridView);
            AddEventHandlers();

            PanelCloseFilter panelCloseFilter = new(this, ClosePanels, TextBoxManager.RightClickTextBox_Panel, RightClickDataGridViewRowMenu.Panel);
            Application.AddMessageFilter(panelCloseFilter);

            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void AddEventHandlers()
        {
            TextBoxManager.Attach(Category_TextBox);
            TextBoxManager.Attach(Search_TextBox);

            Purchase_DataGridView.RowsAdded += (_, _) => LabelManager.ShowTotalLabel(Total_Label, Purchase_DataGridView);
            Purchase_DataGridView.RowsRemoved += (_, _) => LabelManager.ShowTotalLabel(Total_Label, Purchase_DataGridView);

            Sale_DataGridView.RowsAdded += (_, _) => LabelManager.ShowTotalLabel(Total_Label, Sale_DataGridView);
            Sale_DataGridView.RowsRemoved += (_, _) => LabelManager.ShowTotalLabel(Total_Label, Sale_DataGridView);

            Rent_DataGridView.RowsAdded += (_, _) => LabelManager.ShowTotalLabel(Total_Label, Rent_DataGridView);
            Rent_DataGridView.RowsRemoved += (_, _) => LabelManager.ShowTotalLabel(Total_Label, Rent_DataGridView);

            ForPurchase_Label.Click += (_, _) => Purchase_RadioButton.Checked = true;
            ForSale_Label.Click += (_, _) => Sale_RadioButton.Checked = true;
            ForRent_Label.Click += (_, _) => Rent_RadioButton.Checked = true;
        }
        private void SetAccessibleDescriptions()
        {
            CategoryName_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            WarningCategoryName_Label.AccessibleDescription = AccessibleDescriptionManager.AlignRight;
            ForPurchase_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            ForSale_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            ShowingResultsFor_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            Total_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
        }
        public void AdjustRadioButtonPositions()
        {
            Sale_RadioButton.Left = ForPurchase_Label.Right + CustomControls.SpaceBetweenControls;
            ForSale_Label.Left = Sale_RadioButton.Right - 2;

            Rent_RadioButton.Left = ForSale_Label.Right + CustomControls.SpaceBetweenControls;
            ForRent_Label.Left = Rent_RadioButton.Right - 2;
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);
        }

        // Methods
        private void LoadCategories()
        {
            foreach (Category category in MainMenu_Form.Instance.CategoryPurchaseList)
            {
                Purchase_DataGridView.Rows.Add(category.Name);
            }
            DataGridViewManager.ScrollToTopOfDataGridView(Purchase_DataGridView);

            foreach (Category category in MainMenu_Form.Instance.CategorySaleList)
            {
                Sale_DataGridView.Rows.Add(category.Name);
            }
            DataGridViewManager.ScrollToTopOfDataGridView(Sale_DataGridView);

            foreach (Category category in MainMenu_Form.Instance.CategoryRentalList)
            {
                Rent_DataGridView.Rows.Add(category.Name);
            }
            DataGridViewManager.ScrollToTopOfDataGridView(Rent_DataGridView);
        }
        private void CheckRadioButton(bool selectPurchaseRadioButton)
        {
            if (selectPurchaseRadioButton)
            {
                Purchase_RadioButton.Checked = true;
            }
            else
            {
                Sale_RadioButton.Checked = true;
            }
        }

        // Form event handlers
        private void Categories_Form_Shown(object sender, EventArgs e)
        {
            Purchase_DataGridView.ClearSelection();
            LoadingPanel.HideBlankLoadingPanel(this);
        }
        private void Categories_Form_Resize(object sender, EventArgs e)
        {
            ClosePanels();
        }
        private void Categories_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClosePanels();
        }

        // Event handlers
        private void AddCategory_Button_Click(object sender, EventArgs e)
        {
            string name = Category_TextBox.Text.Trim();
            int newRowIndex;

            if (Purchase_RadioButton.Checked)
            {
                MainMenu_Form.Instance.CategoryPurchaseList.Add(new Category(name));
                newRowIndex = Purchase_DataGridView.Rows.Add(name);
            }
            else if (Sale_RadioButton.Checked)
            {
                MainMenu_Form.Instance.CategorySaleList.Add(new Category(name));
                newRowIndex = Sale_DataGridView.Rows.Add(name);
            }
            else
            {
                MainMenu_Form.Instance.CategoryRentalList.Add(new Category(name));
                newRowIndex = Rent_DataGridView.Rows.Add(name);
            }

            DataGridViewManager.DataGridViewRowsAdded(selectedDataGridView, new DataGridViewRowsAddedEventArgs(newRowIndex, 1));

            string message = $"Added category '{name}'";
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(ThingsThatHaveChangedInFile, 3, message);

            Category_TextBox.Text = "";
            Category_TextBox.Focus();
        }
        private void Purchase_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (Purchase_RadioButton.Checked)
            {
                Purchase_DataGridView.Visible = true;
                Sale_DataGridView.Visible = false;
                Purchase_DataGridView.ClearSelection();
                selectedDataGridView = Purchase_DataGridView;
                VaidateCategoryTextBox();
                LabelManager.ShowTotalLabel(Total_Label, Purchase_DataGridView);
            }
        }
        private void Sale_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (Sale_RadioButton.Checked)
            {
                Sale_DataGridView.Visible = true;
                Purchase_DataGridView.Visible = false;
                Sale_DataGridView.ClearSelection();
                selectedDataGridView = Sale_DataGridView;
                VaidateCategoryTextBox();
                LabelManager.ShowTotalLabel(Total_Label, Sale_DataGridView);
            }
        }
        private void Rent_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (Rent_RadioButton.Checked)
            {
                Rent_DataGridView.Visible = true;
                Purchase_DataGridView.Visible = false;
                Sale_DataGridView.Visible = false;
                Rent_DataGridView.ClearSelection();
                selectedDataGridView = Rent_DataGridView;
                VaidateCategoryTextBox();
                LabelManager.ShowTotalLabel(Total_Label, Rent_DataGridView);
            }
        }
        private void Category_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (AddCategory_Button.Enabled)
                {
                    AddCategory_Button.PerformClick();
                }
            }
        }
        private void Category_TextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateInputs();
            VaidateCategoryTextBox();
        }
        private void Search_TextBox_TextChanged(object sender, EventArgs e)
        {
            if (DataGridViewManager.SearchSelectedDataGridViewAndUpdateRowColors(selectedDataGridView, Search_TextBox))
            {
                LabelManager.ShowLabelWithBaseText(ShowingResultsFor_Label, Search_TextBox.Text.Trim());
            }
            else
            {
                ShowingResultsFor_Label.Visible = false;
            }
            LabelManager.ShowTotalLabel(Total_Label, selectedDataGridView);
        }
        private void Search_TextBox_IconRightClick(object sender, EventArgs e)
        {
            Search_TextBox.Text = "";
        }

        // DataGridView properties
        public enum Column
        {
            CategoryName,
        }
        public readonly Dictionary<Column, string> ColumnHeaders = new()
        {
            { Column.CategoryName, "Category" },
        };
        private Guna2DataGridView selectedDataGridView;

        // DataGridView getters
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Guna2DataGridView Purchase_DataGridView { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Guna2DataGridView Sale_DataGridView { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Guna2DataGridView Rent_DataGridView { get; set; }

        // DataGridView methods
        private void ConstructDataGridViews()
        {
            Purchase_DataGridView = new();
            DataGridViewManager.InitializeDataGridView(Purchase_DataGridView, "purchases_DataGridView", ColumnHeaders, null, this);
            Purchase_DataGridView.Size = new Size(ClientSize.Width - 80, ClientSize.Height - _topForDataGridView - 70);
            Purchase_DataGridView.Location = new Point((ClientSize.Width - Purchase_DataGridView.Width) / 2, _topForDataGridView);
            Purchase_DataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            Purchase_DataGridView.Tag = MainMenu_Form.DataGridViewTag.Category;

            Sale_DataGridView = new();
            DataGridViewManager.InitializeDataGridView(Sale_DataGridView, "sales_DataGridView", ColumnHeaders, null, this);
            Sale_DataGridView.Size = new Size(ClientSize.Width - 80, ClientSize.Height - _topForDataGridView - 70);
            Sale_DataGridView.Location = new Point((ClientSize.Width - Sale_DataGridView.Width) / 2, _topForDataGridView);
            Sale_DataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            Sale_DataGridView.Tag = MainMenu_Form.DataGridViewTag.Category;
            ThemeManager.CustomizeScrollBar(Sale_DataGridView);

            Rent_DataGridView = new();
            DataGridViewManager.InitializeDataGridView(Rent_DataGridView, "rent_DataGridView", ColumnHeaders, null, this);
            Rent_DataGridView.Size = new Size(ClientSize.Width - 80, ClientSize.Height - _topForDataGridView - 70);
            Rent_DataGridView.Location = new Point((ClientSize.Width - Rent_DataGridView.Width) / 2, _topForDataGridView);
            Rent_DataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            Rent_DataGridView.Tag = MainMenu_Form.DataGridViewTag.Category;
            ThemeManager.CustomizeScrollBar(Rent_DataGridView);
        }

        // Validate category name
        public void VaidateCategoryTextBox()
        {
            // Get list
            List<Category> categories;
            if (Sale_RadioButton.Checked)
            {
                categories = MainMenu_Form.Instance.CategorySaleList;
            }
            else if (Rent_RadioButton.Checked)
            {
                categories = MainMenu_Form.Instance.CategoryRentalList;
            }
            else
            {
                categories = MainMenu_Form.Instance.CategoryPurchaseList;
            }

            bool exists = categories.Any(category => string.Equals(category.Name, Category_TextBox.Text.Trim(), StringComparison.OrdinalIgnoreCase));
            if (exists)
            {
                AddCategory_Button.Enabled = false;
                CustomControls.SetGTextBoxToInvalid(Category_TextBox);
                ShowCategoryWarning();
            }
            else
            {
                if (Category_TextBox.Text != "")
                {
                    AddCategory_Button.Enabled = true;
                }
                CustomControls.SetGTextBoxToValid(Category_TextBox);
                HideCategoryWarning();
            }
        }
        private void ShowCategoryWarning()
        {
            WarningCategoryName_PictureBox.Visible = true;
            WarningCategoryName_Label.Visible = true;
            AddCategory_Button.Enabled = false;
            AddCategory_Button.Tag = false;
        }
        private void HideCategoryWarning()
        {
            WarningCategoryName_PictureBox.Visible = false;
            WarningCategoryName_Label.Visible = false;

            if (Category_TextBox.Text != "")
            {
                AddCategory_Button.Enabled = true;
                AddCategory_Button.Tag = true;
            }
        }

        // Methods
        private void ValidateInputs()
        {
            if (AddCategory_Button.Tag is bool and true)
            {
                AddCategory_Button.Enabled = !string.IsNullOrWhiteSpace(Category_TextBox.Text);
            }
        }
        private void ClosePanels()
        {
            TextBoxManager.HideRightClickPanel();
            RightClickDataGridViewRowMenu.Hide();
        }
    }
}
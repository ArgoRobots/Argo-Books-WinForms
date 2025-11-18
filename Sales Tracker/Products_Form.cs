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
    /// Form for managing product inventory and information.
    /// </summary>
    public partial class Products_Form : BaseForm
    {
        private static Products_Form _instance;
        private static bool _isProgramLoading;
        private readonly int _topForDataGridView;

        // Getters
        public static List<string> ThingsThatHaveChangedInFile { get; } = [];
        public static Products_Form Instance => _instance;

        // Init.
        public Products_Form() : this(false) { }  // This is needed for TranslationGenerator.GenerateAllLanguageTranslationFiles()
        public Products_Form(bool checkPurchaseRadioButton)
        {
            InitializeComponent();
            _instance = this;

            _topForDataGridView = ShowingResultsFor_Label.Bottom + 20;
            AddSearchBoxEvents();

            _isProgramLoading = true;
            ConstructDataGridViews();
            LoadProducts();
            _isProgramLoading = false;

            ValidateCompanyTextBox();
            ThemeManager.SetThemeForForm(this);
            CheckRadioButton(checkPurchaseRadioButton);
            Guna2TextBoxIconHoverEffect.Initialize(Search_TextBox);
            SetAccessibleDescriptions();
            ShowingResultsFor_Label.Visible = false;
            LanguageManager.UpdateLanguageForControl(this);
            AdjustRadioButtonPositions();
            PopulateTypeComboBox();
            DataGridViewManager.SortFirstColumnAndSelectFirstRow(Purchase_DataGridView, Sale_DataGridView, Rentals_DataGridView);
            AddEventHandlers();

            PanelCloseFilter panelCloseFilter = new(this, ClosePanels,
                TextBoxManager.RightClickTextBox_Panel,
                SearchBox.SearchResultBoxContainer,
                RightClickDataGridViewRowMenu.Panel);

            Application.AddMessageFilter(panelCloseFilter);

            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void AddEventHandlers()
        {
            TextBoxManager.Attach(ProductID_TextBox);

            TextBoxManager.Attach(ProductName_TextBox);

            TextBoxManager.Attach(ProductCategory_TextBox);

            TextBoxValidation.OnlyAllowLetters(CountryOfOrigin_TextBox);
            TextBoxManager.Attach(CountryOfOrigin_TextBox);

            TextBoxManager.Attach(CompanyOfOrigin_TextBox);

            Purchase_DataGridView.RowsAdded += (_, _) => LabelManager.ShowTotalLabel(Total_Label, Purchase_DataGridView);
            Purchase_DataGridView.RowsRemoved += (_, _) => LabelManager.ShowTotalLabel(Total_Label, Purchase_DataGridView);

            Sale_DataGridView.RowsAdded += (_, _) => LabelManager.ShowTotalLabel(Total_Label, Sale_DataGridView);
            Sale_DataGridView.RowsRemoved += (_, _) => LabelManager.ShowTotalLabel(Total_Label, Sale_DataGridView);

            Rentals_DataGridView.RowsAdded += (_, _) => LabelManager.ShowTotalLabel(Total_Label, Rentals_DataGridView);
            Rentals_DataGridView.RowsRemoved += (_, _) => LabelManager.ShowTotalLabel(Total_Label, Rentals_DataGridView);

            TextBoxManager.Attach(Search_TextBox);

            ForPurchase_Label.Click += (_, _) => Purchase_RadioButton.Checked = true;
            ForSale_Label.Click += (_, _) => Sale_RadioButton.Checked = true;
            ForRent_Label.Click += (_, _) => Rent_RadioButton.Checked = true;
        }
        private void AddSearchBoxEvents()
        {
            int searchBoxMaxHeight = 300;

            SearchBox.Attach(ProductCategory_TextBox, this, GetSearchResultsForCategory, searchBoxMaxHeight, false, false, true, true);
            SearchBox.Attach(CountryOfOrigin_TextBox, this, () => Country.CountrySearchResults, searchBoxMaxHeight, false, true, true, false);
            SearchBox.Attach(CompanyOfOrigin_TextBox, this, GetSearchResultsForCompany, searchBoxMaxHeight, false, false, true, true);
        }
        private List<SearchResult> GetSearchResultsForCategory()
        {
            List<string> categoryNames;

            if (Purchase_RadioButton.Checked)
            {
                categoryNames = MainMenu_Form.Instance.CategoryPurchaseList.Select(p => p.Name).ToList();
            }
            else if (Sale_RadioButton.Checked)
            {
                categoryNames = MainMenu_Form.Instance.CategorySaleList.Select(s => s.Name).ToList();
            }
            else
            {
                categoryNames = MainMenu_Form.Instance.CategoryRentalList.Select(s => s.Name).ToList();
            }

            return SearchBox.ConvertToSearchResults(categoryNames);
        }
        private List<SearchResult> GetSearchResultsForCompany()
        {
            return SearchBox.ConvertToSearchResults(MainMenu_Form.Instance.CompanyList);
        }
        private void LoadProducts()
        {
            foreach (Category category in MainMenu_Form.Instance.CategoryPurchaseList)
            {
                foreach (Product product in category.ProductList)
                {
                    Product.TypeOption type = product.ItemType ?? Product.TypeOption.Product;
                    Purchase_DataGridView.Rows.Add(product.ProductID, product.Name, category.Name, product.CountryOfOrigin, product.CompanyOfOrigin, type);
                }
            }
            DataGridViewManager.ScrollToTopOfDataGridView(Purchase_DataGridView);

            foreach (Category category in MainMenu_Form.Instance.CategorySaleList)
            {
                foreach (Product product in category.ProductList)
                {
                    Product.TypeOption type = product.ItemType ?? Product.TypeOption.Product;
                    Sale_DataGridView.Rows.Add(product.ProductID, product.Name, category.Name, product.CountryOfOrigin, product.CompanyOfOrigin, type);
                }
            }
            DataGridViewManager.ScrollToTopOfDataGridView(Sale_DataGridView);

            foreach (Category category in MainMenu_Form.Instance.CategoryRentalList)
            {
                foreach (Product product in category.ProductList)
                {
                    Sale_DataGridView.Rows.Add(product.ProductID, product.Name, category.Name, product.CountryOfOrigin, product.CompanyOfOrigin);
                }
            }
            DataGridViewManager.ScrollToTopOfDataGridView(Sale_DataGridView);
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
        private void SetAccessibleDescriptions()
        {
            ProductsRemaining_LinkLabel.AccessibleDescription = AccessibleDescriptionManager.AlignRight;
            ForPurchase_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            ForSale_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            ProductID_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            ProductName_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            ProductCategory_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            CountryOfOrigin_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            CompanyOfOrigin_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            WarningProductName_Label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            WarningCategory_LinkLabel.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            WarningCompany_LinkLabel.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            Total_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;

            // Prevent automatic translation since we handle it manually
            Type_ComboBox.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
        }
        public void AdjustRadioButtonPositions()
        {
            Sale_RadioButton.Left = ForPurchase_Label.Right + CustomControls.SpaceBetweenControls;
            ForSale_Label.Left = Sale_RadioButton.Right - 2;

            Rent_RadioButton.Left = ForSale_Label.Right + CustomControls.SpaceBetweenControls;
            ForRent_Label.Left = Rent_RadioButton.Right - 2;
        }

        // Form event handlers
        private void Products_Form_Activated(object sender, EventArgs e)
        {
            ValidateCategoryTextBox();
            ValidateCompanyTextBox();
        }
        private void Products_Form_Shown(object sender, EventArgs e)
        {
            Purchase_DataGridView.ClearSelection();
            LoadingPanel.HideBlankLoadingPanel(this);
        }
        private void Products_Form_Resize(object sender, EventArgs e)
        {
            ClosePanels();
        }
        private void Products_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClosePanels();
        }

        // Event handlers
        private void AddProduct_Button_Click(object sender, EventArgs e)
        {
            string productID = ProductID_TextBox.Text.Trim();
            if (productID == "")
            {
                productID = ReadOnlyVariables.EmptyCell;
            }

            // Check if product ID already exists
            if (productID != ReadOnlyVariables.EmptyCell &&
                DataGridViewManager.DoesValueExistInDataGridView(_selectedDataGridView, Column.ProductID.ToString(), productID))
            {
                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat("Product already exists",
                    "The product #{0} already exists. Would you like to add this product anyways?",
                    CustomMessageBoxIcon.Question,
                    CustomMessageBoxButtons.YesNo,
                    productID);

                if (result != CustomMessageBoxResult.Yes)
                {
                    return;
                }
            }

            string name = ProductName_TextBox.Text.Trim();

            Product.TypeOption? type = null;

            if (!Rent_RadioButton.Checked)
            {
                if (Type_ComboBox.SelectedIndex == 0)
                {
                    type = Product.TypeOption.Product;
                }
                else
                {
                    type = Product.TypeOption.Service;
                }
            }

            Product product = new(productID, name, CountryOfOrigin_TextBox.Text, CompanyOfOrigin_TextBox.Text, type);

            string category = ProductCategory_TextBox.Text;
            int newRowIndex;

            if (Purchase_RadioButton.Checked)
            {
                MainMenu_Form.AddProductToCategoryByName(MainMenu_Form.Instance.CategoryPurchaseList, category, product);
                newRowIndex = Purchase_DataGridView.Rows.Add(product.ProductID, product.Name, category, product.CountryOfOrigin, product.CompanyOfOrigin, product.ItemType);
            }
            else if (Sale_RadioButton.Checked)
            {
                MainMenu_Form.AddProductToCategoryByName(MainMenu_Form.Instance.CategorySaleList, category, product);
                newRowIndex = Sale_DataGridView.Rows.Add(product.ProductID, product.Name, category, product.CountryOfOrigin, product.CompanyOfOrigin, product.ItemType);
            }
            else
            {
                MainMenu_Form.AddProductToCategoryByName(MainMenu_Form.Instance.CategoryRentalList, category, product);
                newRowIndex = Rentals_DataGridView.Rows.Add(product.ProductID, product.Name, category, product.CountryOfOrigin, product.CompanyOfOrigin);
            }

            DataGridViewManager.DataGridViewRowsAdded(_selectedDataGridView, new DataGridViewRowsAddedEventArgs(newRowIndex, 1));

            string message = $"Added product '{name}'";
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(ThingsThatHaveChangedInFile, 4, message);

            ProductName_TextBox.Clear();
            ProductID_TextBox.Clear();
        }
        public void Purchase_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (Purchase_RadioButton.Checked)
            {
                Purchase_DataGridView.Visible = true;
                Sale_DataGridView.Visible = false;
                Rentals_DataGridView.Visible = false;
                ShowTypeControls();
                _selectedDataGridView = Purchase_DataGridView;
                Purchase_DataGridView.ClearSelection();
                ProductCategory_TextBox.Clear();
                ValidateCategoryTextBox();
                SetProductsRemainingLabel();
                LabelManager.ShowTotalLabel(Total_Label, Purchase_DataGridView);
            }
        }
        private void Sale_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (Sale_RadioButton.Checked)
            {
                Sale_DataGridView.Visible = true;
                Purchase_DataGridView.Visible = false;
                Rentals_DataGridView.Visible = false;
                ShowTypeControls();
                _selectedDataGridView = Sale_DataGridView;
                Sale_DataGridView.ClearSelection();
                ProductCategory_TextBox.Clear();
                ValidateCategoryTextBox();
                SetProductsRemainingLabel();
                LabelManager.ShowTotalLabel(Total_Label, Sale_DataGridView);
            }
        }
        private void Rent_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (Rent_RadioButton.Checked)
            {
                Rentals_DataGridView.Visible = true;
                Purchase_DataGridView.Visible = false;
                Sale_DataGridView.Visible = false;
                HideTypeControls();
                _selectedDataGridView = Rentals_DataGridView;
                Rentals_DataGridView.ClearSelection();
                ProductCategory_TextBox.Clear();
                ValidateCategoryTextBox();
                SetProductsRemainingLabel();
                LabelManager.ShowTotalLabel(Total_Label, Rentals_DataGridView);
            }
        }
        private void ProductName_TextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateProductNameTextBox();
            ValidateInputs(null, null);
        }
        private void ProductCategory_TextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateProductNameTextBox();
            ValidateInputs(null, null);
        }
        private void CountryOfOrigin_TextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateInputs(null, null);
        }
        private void CompanyOfOrigin_TextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateProductNameTextBox();
            ValidateInputs(null, null);
        }
        private void CategoryWarning_LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Tools.OpenForm(new Categories_Form(Purchase_RadioButton.Checked));
            ValidateCategoryTextBox();
        }
        private void WarningCompany_LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Tools.OpenForm(new Companies_Form());
            ValidateCompanyTextBox();
        }
        private void Search_TextBox_TextChanged(object sender, EventArgs e)
        {
            string searchText = Search_TextBox.Text.Trim();
            bool hasVisibleRows = true;

            foreach (DataGridViewRow row in _selectedDataGridView.Rows)
            {
                row.Visible = SearchDataGridView.FilterRowByAdvancedSearch(row, searchText);
                if (row.Visible)
                {
                    hasVisibleRows = true;
                }
            }

            // Update UI based on search results
            if (hasVisibleRows && !string.IsNullOrEmpty(searchText))
            {
                LabelManager.ShowLabelWithBaseText(ShowingResultsFor_Label, searchText);
            }
            else
            {
                ShowingResultsFor_Label.Visible = false;
            }

            DataGridViewManager.UpdateRowColors(_selectedDataGridView);
            LabelManager.ShowTotalLabel(Total_Label, _selectedDataGridView);
        }
        private void Search_TextBox_IconRightClick(object sender, EventArgs e)
        {
            Search_TextBox.Clear();
        }
        private void ProductsRemaining_LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Tools.OpenLink("https://argorobots.com/upgrade/index.php");
        }

        // Hide or show type controls
        private void ShowTypeControls()
        {
            if (Type_Label.Visible) { return; }

            Type_Label.Visible = true;
            Type_ComboBox.Visible = true;

            AlignControlsAfterTypeChange(true);

            Type_ComboBox.Left = CompanyOfOrigin_TextBox.Right + CustomControls.SpaceBetweenControls;
            Type_Label.Left = Type_ComboBox.Left;
        }
        private void HideTypeControls()
        {
            if (!Type_Label.Visible) { return; }

            Type_Label.Visible = false;
            Type_ComboBox.Visible = false;

            AlignControlsAfterTypeChange(false);
        }
        private void AlignControlsAfterTypeChange(bool includeTypeControls)
        {
            int totalSpace = ProductID_TextBox.Width + CustomControls.SpaceBetweenControls +
                ProductName_TextBox.Width + CustomControls.SpaceBetweenControls +
                ProductCategory_TextBox.Width + CustomControls.SpaceBetweenControls +
                CountryOfOrigin_TextBox.Width + CustomControls.SpaceBetweenControls +
                CompanyOfOrigin_TextBox.Width;

            if (includeTypeControls)
            {
                totalSpace += CustomControls.SpaceBetweenControls + Type_ComboBox.Width;
            }

            ProductID_TextBox.Left = (ClientSize.Width - totalSpace) / 2;
            ProductID_Label.Left = ProductID_TextBox.Left;

            ProductName_TextBox.Left = ProductID_TextBox.Right + CustomControls.SpaceBetweenControls;
            ProductName_Label.Left = ProductName_TextBox.Left;

            WarningProductName_PictureBox.Left = ProductName_TextBox.Left + CustomControls.SpaceBetweenControls;
            WarningProductName_Label.Left = WarningProductName_PictureBox.Right + CustomControls.SpaceBetweenControls;

            ProductCategory_TextBox.Left = ProductName_TextBox.Right + CustomControls.SpaceBetweenControls;
            ProductCategory_Label.Left = ProductCategory_TextBox.Left;

            WarningCategory_PictureBox.Left = ProductCategory_TextBox.Left + CustomControls.SpaceBetweenControls;
            WarningCategory_LinkLabel.Left = WarningCategory_PictureBox.Right + CustomControls.SpaceBetweenControls;

            CountryOfOrigin_TextBox.Left = ProductCategory_TextBox.Right + CustomControls.SpaceBetweenControls;
            CountryOfOrigin_Label.Left = CountryOfOrigin_TextBox.Left;

            CompanyOfOrigin_TextBox.Left = CountryOfOrigin_TextBox.Right + CustomControls.SpaceBetweenControls;
            CompanyOfOrigin_Label.Left = CompanyOfOrigin_TextBox.Left;

            WarningCompany_PictureBox.Left = CompanyOfOrigin_TextBox.Left + CustomControls.SpaceBetweenControls;
            WarningCompany_LinkLabel.Left = WarningCompany_PictureBox.Right + CustomControls.SpaceBetweenControls;
        }

        // Products remaining
        private int GetProductsRemaining()
        {
            return 10 - _selectedDataGridView.Rows.Count;
        }
        private void SetProductsRemainingLabel()
        {
            if (Properties.Settings.Default.LicenseActivated)
            {
                ProductsRemaining_LinkLabel.Visible = false;
                return;
            }

            int productsRemaining = GetProductsRemaining();
            if (productsRemaining <= 0)
            {
                AddProduct_Button.Enabled = false;
                ProductsRemaining_LinkLabel.ForeColor = CustomColors.AccentRed;
            }
            else
            {
                if (IsProductValid())
                {
                    AddProduct_Button.Enabled = true;
                }
                ProductsRemaining_LinkLabel.ForeColor = CustomColors.Text;
            }

            ProductsRemaining_LinkLabel.Text = $"{productsRemaining} products remaining. Upgrade now";
            ProductsRemaining_LinkLabel.LinkArea = new LinkArea(ProductsRemaining_LinkLabel.Text.IndexOf("Upgrade now"), "Upgrade now".Length);
            ProductsRemaining_LinkLabel.Left = Width - 70 - ProductsRemaining_LinkLabel.Width;
            AddProduct_Label.Focus();
        }

        // Validate product name
        public void ValidateProductNameTextBox()
        {
            if (IsProductValid())
            {
                AddProduct_Button.Enabled = false;
                CustomControls.SetGTextBoxToInvalid(ProductName_TextBox);
                ShowProductNameWarning();
            }
            else
            {
                AddProduct_Button.Enabled = true;
                CustomControls.SetGTextBoxToValid(ProductName_TextBox);
                HideProductNameWarning();
            }
        }
        private bool IsProductValid()
        {
            string category = ProductCategory_TextBox.Text.Trim();

            return MainMenu_Form.DoesProductExistInCategory(ProductName_TextBox.Text, CompanyOfOrigin_TextBox.Text, GetCategoryList(), category);
        }
        private void ShowProductNameWarning()
        {
            WarningProductName_PictureBox.Visible = true;
            WarningProductName_Label.Visible = true;
        }
        private void HideProductNameWarning()
        {
            WarningProductName_PictureBox.Visible = false;
            WarningProductName_Label.Visible = false;
        }

        // Validate category name
        private void ValidateCategoryTextBox()
        {
            List<Category> categoryList = GetCategoryList();

            if (categoryList.Count == 0)
            {
                ShowCategoryWarning();
            }
            else
            {
                HideCategoryWarning();
            }
        }
        private void ShowCategoryWarning()
        {
            WarningCategory_PictureBox.Visible = true;
            WarningCategory_LinkLabel.Visible = true;
        }
        private void HideCategoryWarning()
        {
            WarningCategory_PictureBox.Visible = false;
            WarningCategory_LinkLabel.Visible = false;
        }

        // Validate company name
        private void ValidateCompanyTextBox()
        {
            if (MainMenu_Form.Instance.CompanyList.Count == 0)
            {
                ShowCompanyWarning();
            }
            else
            {
                HideCompanyWarning();
            }
        }
        private void ShowCompanyWarning()
        {
            WarningCompany_PictureBox.Visible = true;
            WarningCompany_LinkLabel.Visible = true;
        }
        private void HideCompanyWarning()
        {
            WarningCompany_PictureBox.Visible = false;
            WarningCompany_LinkLabel.Visible = false;
        }

        // DataGridView properties
        public enum Column
        {
            ProductID,
            ProductName,
            ProductCategory,
            CountryOfOrigin,
            CompanyOfOrigin,
            Type
        }
        public static readonly Dictionary<Column, string> ColumnHeaders = new()
        {
            { Column.ProductID, "Product ID" },
            { Column.ProductName, "Product name" },
            { Column.ProductCategory, "Product category" },
            { Column.CountryOfOrigin, "Country of origin" },
            { Column.CompanyOfOrigin, "Company of origin" },
            { Column.Type, "Type" }
        };
        private Guna2DataGridView _selectedDataGridView;
        public Guna2DataGridView Purchase_DataGridView { get; private set; }
        public Guna2DataGridView Sale_DataGridView { get; private set; }
        public Guna2DataGridView Rentals_DataGridView { get; private set; }

        // DataGridView methods
        private void ConstructDataGridViews()
        {
            Purchase_DataGridView = new();
            DataGridViewManager.InitializeDataGridView(Purchase_DataGridView, "Purchase_DataGridView", ColumnHeaders, null, this);
            Purchase_DataGridView.RowsAdded += DataGridView_RowsChanged;
            Purchase_DataGridView.RowsRemoved += DataGridView_RowsChanged;
            Purchase_DataGridView.Size = new Size(ClientSize.Width - 80, ClientSize.Height - _topForDataGridView - 70);
            Purchase_DataGridView.Location = new Point((ClientSize.Width - Purchase_DataGridView.Width) / 2, _topForDataGridView);
            Purchase_DataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            Purchase_DataGridView.Tag = MainMenu_Form.DataGridViewTag.Product;
            ThemeManager.CustomizeScrollBar(Purchase_DataGridView);

            Sale_DataGridView = new();
            DataGridViewManager.InitializeDataGridView(Sale_DataGridView, "Sale_DataGridView", ColumnHeaders, null, this);
            Sale_DataGridView.RowsAdded += DataGridView_RowsChanged;
            Sale_DataGridView.RowsRemoved += DataGridView_RowsChanged;
            Sale_DataGridView.Size = new Size(ClientSize.Width - 80, ClientSize.Height - _topForDataGridView - 70);
            Sale_DataGridView.Location = new Point((ClientSize.Width - Sale_DataGridView.Width) / 2, _topForDataGridView);
            Sale_DataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            Sale_DataGridView.Tag = MainMenu_Form.DataGridViewTag.Product;
            ThemeManager.CustomizeScrollBar(Sale_DataGridView);

            List<Column> rentalColumns = [
                Column.ProductID,
                Column.ProductName,
                Column.ProductCategory,
                Column.CountryOfOrigin,
                Column.CompanyOfOrigin];

            Rentals_DataGridView = new();
            DataGridViewManager.InitializeDataGridView(Rentals_DataGridView, "rentals_DataGridView", ColumnHeaders, rentalColumns, this);
            Rentals_DataGridView.RowsAdded += DataGridView_RowsChanged;
            Rentals_DataGridView.RowsRemoved += DataGridView_RowsChanged;
            Rentals_DataGridView.Size = new Size(ClientSize.Width - 80, ClientSize.Height - _topForDataGridView - 70);
            Rentals_DataGridView.Location = new Point((ClientSize.Width - Rentals_DataGridView.Width) / 2, _topForDataGridView);
            Rentals_DataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            Rentals_DataGridView.Tag = MainMenu_Form.DataGridViewTag.Product;
            ThemeManager.CustomizeScrollBar(Rentals_DataGridView);
        }
        private void DataGridView_RowsChanged(object sender, EventArgs e)
        {
            if (_isProgramLoading) { return; }
            SetProductsRemainingLabel();
        }

        // Methods
        private List<Category> GetCategoryList()
        {
            if (Purchase_RadioButton.Checked)
            {
                return MainMenu_Form.Instance.CategoryPurchaseList;
            }
            else if (Sale_RadioButton.Checked)
            {
                return MainMenu_Form.Instance.CategorySaleList;
            }
            else
            {
                return MainMenu_Form.Instance.CategoryRentalList;
            }
        }
        private void ValidateInputs(object sender, EventArgs e)
        {
            bool allFieldsFilled = !string.IsNullOrWhiteSpace(ProductName_TextBox.Text) &&
                                   !string.IsNullOrWhiteSpace(ProductCategory_TextBox.Text) && ProductCategory_TextBox.Tag.ToString() != "0" &&
                                   !string.IsNullOrWhiteSpace(CountryOfOrigin_TextBox.Text) && CountryOfOrigin_TextBox.Tag.ToString() != "0" &&
                                   !string.IsNullOrWhiteSpace(CompanyOfOrigin_TextBox.Text) && CompanyOfOrigin_TextBox.Tag.ToString() != "0";

            if (!Properties.Settings.Default.LicenseActivated)
            {
                allFieldsFilled &= GetProductsRemaining() > 0;
            }
            AddProduct_Button.Enabled = allFieldsFilled;
        }
        public void PopulateTypeComboBox()
        {
            int index = Type_ComboBox.SelectedIndex;

            // Clear and repopulate with translated text
            Type_ComboBox.Items.Clear();

            Type_ComboBox.Items.Add(LanguageManager.TranslateString("Product"));
            Type_ComboBox.Items.Add(LanguageManager.TranslateString("Service"));

            // Restore selection
            Type_ComboBox.SelectedIndex = index != -1 ? index : 0;
        }
        private void ClosePanels()
        {
            SearchBox.Close();
            TextBoxManager.HideRightClickPanel();
            RightClickDataGridViewRowMenu.Hide();
        }
    }
}
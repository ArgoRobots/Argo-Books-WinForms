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
    /// Form for modifying transaction row data.
    /// </summary>
    public partial class ModifyRow_Form : BaseForm
    {
        // Properties
        private readonly string _selectedTag = "";
        private readonly DataGridViewRow _selectedRow;
        private string _receiptFilePath;
        private readonly Dictionary<Control, Label> _validationLabels = [];

        // Form Width/Height properties
        private static int ScaledLargeWidth => (int)(350 * DpiHelper.GetRelativeDpiScale());
        private static int ScaledStandardWidth => (int)(250 * DpiHelper.GetRelativeDpiScale());
        private static int ScaledDatePickerWidth => (int)(300 * DpiHelper.GetRelativeDpiScale());
        private static int ScaledControlHeight => (int)(50 * DpiHelper.GetRelativeDpiScale());

        // Init.
        public ModifyRow_Form() : this(null) { }  // This is needed for TranslationGenerator.GenerateAllLanguageTranslationFiles()
        public ModifyRow_Form(DataGridViewRow row)
        {
            InitializeComponent();
            if (row == null) { return; }

            _selectedRow = row;
            _selectedTag = row.DataGridView.Tag.ToString();
            _receiptFilePath = RightClickDataGridViewRowMenu.GetFilePathFromRowTag(row.Tag);

            if (_receiptFilePath != "" && !File.Exists(_receiptFilePath))
            {
                CustomMessageBox.Show("Receipt does not exist",
                    "The receipt no longer exists.",
                    CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok);

                Log.Error_FileDoesNotExist(_receiptFilePath);
            }

            ConstructControls();
            ValidateInputs(null, null);  // This is needed in case the cell contains "-", which is possible if it was imported from a spreadsheet
            AttachChangeHandlers();
            SetAccessibleDescriptions();
            UpdateTheme();

            if (_removeReceipt_ImageButton != null)
            {
                DpiHelper.ScaleImageButton(_removeReceipt_ImageButton);
            }

            PanelCloseFilter panelCloseFilter = new(this, ClosePanels, SearchBox.SearchResultBoxContainer);
            Application.AddMessageFilter(panelCloseFilter);

            LanguageManager.UpdateLanguageForControl(this);
            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void SetAccessibleDescriptions()
        {
            ModifyRow_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);
            ThemeManager.MakeGButtonBluePrimary(Save_Button);
            ThemeManager.MakeGButtonBlueSecondary(Cancel_Button);

            foreach ((_, Label label) in _validationLabels)
            {
                label.ForeColor = CustomColors.AccentRed;
            }
        }

        // Form event handlers
        private Control _controlToFocus;
        private void ModifyRow_Form_Shown(object sender, EventArgs e)
        {
            _controlToFocus?.Focus();
            CenterControls();
            ModifyRow_Label.Focus();
            LoadingPanel.HideBlankLoadingPanel(this);
        }
        private void ModifyRow_Form_Resize(object sender, EventArgs e)
        {
            ClosePanels();
        }
        private void ModifyRow_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClosePanels();
        }

        // Methods for checking if there are changes
        private static bool _hasChanges = false;
        private void AttachChangeHandlers()
        {
            foreach (Control control in Panel.Controls)
            {
                AttachChangeHandler(control);
            }

            if (_secondPanel != null)
            {
                foreach (Control control in _secondPanel.Controls)
                {
                    AttachChangeHandler(control);
                }
            }
        }
        private void AttachChangeHandler(Control control)
        {
            if (control is Guna2TextBox textBox)
            {
                textBox.TextChanged += (s, e) => CheckForChanges((Guna2TextBox)s);
            }
            else if (control is Guna2ComboBox comboBox)
            {
                comboBox.SelectedIndexChanged += (s, e) => CheckForChanges((Guna2ComboBox)s);
            }
            else if (control is Guna2DateTimePicker datePicker)
            {
                datePicker.ValueChanged += (s, e) => CheckForChanges((Guna2DateTimePicker)s);
            }
        }
        private void CheckForChanges(Control control)
        {
            int index = GetControlIndex(control.Name);
            if (index < 0 || index >= _listOfOldValues.Count) { return; }
        }
        private int GetControlIndex(string controlName)
        {
            int index = 0;
            foreach (DataGridViewColumn column in _selectedRow.DataGridView.Columns)
            {
                if (column.Name == controlName)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        // Event handlers
        private void Save_Button_Click(object sender, EventArgs e)
        {
            SaveInListsAndUpdateMainMenuForm();
            SaveInSelectedRow();
            UpdateChargedDifferenceInMainMenuRow();
            HandleReceiptChanges();

            DataGridViewManager.DataGridViewRowChanged((Guna2DataGridView)_selectedRow.DataGridView);
        }
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            Close();
        }

        // Construct controls
        private void ConstructControls()
        {
            int left = 0, secondLeft = 0;

            if (_selectedTag == MainMenu_Form.DataGridViewTag.Accountant.ToString())
            {
                ConstructControlsForAccountant();
                left = ScaledStandardWidth;
            }
            else if (_selectedTag == MainMenu_Form.DataGridViewTag.Category.ToString())
            {
                ConstructControlsForCategory();
                left = ScaledStandardWidth;
            }
            else if (_selectedTag == MainMenu_Form.DataGridViewTag.Company.ToString())
            {
                ConstructControlsForCompany();
                left = ScaledStandardWidth;
            }
            else if (_selectedTag == MainMenu_Form.DataGridViewTag.Product.ToString())
            {
                left = ConstructControlsForProduct();
            }
            else if (_selectedTag == MainMenu_Form.DataGridViewTag.SaleOrPurchase.ToString())
            {
                (left, secondLeft) = ConstructControlsForPurchaseOrSale();
            }
            else if (_selectedTag == MainMenu_Form.DataGridViewTag.ItemsInPurchase.ToString())
            {
                left = ConstructControlsForItemsInTransaction();
            }
            else if (_selectedTag == MainMenu_Form.DataGridViewTag.Customer.ToString())
            {
                (left, secondLeft) = ConstructControlsForCustomer();
            }

            SizeControls(left, secondLeft);
        }
        private void SizeControls(int left, int secondLeft)
        {
            float scale = DpiHelper.GetRelativeDpiScale();
            int scaledPadding = (int)(140 * scale);

            Width = left + scaledPadding;
            Panel.Width = left;

            if (_secondRow)
            {
                if (secondLeft > left)
                {
                    Width = secondLeft + scaledPadding;
                }

                Height += (int)(100 * scale);
                _secondPanel.Width = secondLeft;
                _secondPanel.Left = (Width - _secondPanel.Width) / 2;
            }

            if (_notes)
            {
                Height += (int)(180 * scale);
            }

            MinimumSize = new Size(Math.Max((int)(600 * scale), Width), Height);
        }
        private void CenterControls()
        {
            Panel.Left = (ClientSize.Width - Panel.Width) / 2;
            ModifyRow_Label.Left = (ClientSize.Width - ModifyRow_Label.Width) / 2;

            if (_secondRow)
            {
                _secondPanel.Left = (ClientSize.Width - _secondPanel.Width) / 2;
            }

            if (_receiptFilePath == "")
            {
                RemoveReceiptLabel();
            }
            else if (_selectedRow.Tag != null && _selectedReceipt_Label != null)
            {
                _containsReceipt = true;
                ShowReceiptLabel(Path.GetFileName(_receiptFilePath));
            }
        }

        private bool _secondRow = false, _notes = false;
        private readonly List<string> _listOfOldValues = [];
        private string _oldProductName;
        private void ConstructControlsForAccountant()
        {
            string columnName = _selectedRow.DataGridView.Columns[0].Name;
            string cellValue = _selectedRow.Cells[0].Value?.ToString() ?? ReadOnlyVariables.EmptyCell;
            _listOfOldValues.Add(cellValue);

            ConstructLabel(Accountants_Form.ColumnHeaders[Accountants_Form.Column.AccountantName], 0, Panel);

            _controlToFocus = ConstructTextBox(0, columnName, cellValue, 50, CustomControls.KeyPressValidation.OnlyLetters, true, Panel);
            _controlToFocus.TextChanged += Accountant_TextBox_TextChanged;

            ConstructWarningLabel();
        }
        private void Accountant_TextBox_TextChanged(object sender, EventArgs e)
        {
            Guna2TextBox textBox = (Guna2TextBox)sender;
            if (textBox.Text == "")
            {
                SetControlForTextBoxEmpty(textBox);
                return;
            }
            bool containsAccountant = MainMenu_Form.Instance.AccountantList.Contains(textBox.Text.Trim(), StringComparer.OrdinalIgnoreCase);
            bool isOldValueDifferent = !string.Equals(_listOfOldValues[0], textBox.Text.Trim(), StringComparison.OrdinalIgnoreCase);

            if (containsAccountant && isOldValueDifferent)
            {
                DisableSaveButton();
                CustomControls.SetGTextBoxToInvalid(textBox);
                ShowWarning(textBox, "Accountant already exists");
            }
            else
            {
                EnableSaveButton();
                CustomControls.SetGTextBoxToValid(textBox);
                HideWarning();
            }
        }
        private void ConstructControlsForCategory()
        {
            string columnName = _selectedRow.DataGridView.Columns[0].Name;
            string cellValue = _selectedRow.Cells[0].Value?.ToString() ?? ReadOnlyVariables.EmptyCell;
            _listOfOldValues.Add(cellValue);

            ConstructLabel(Categories_Form.Instance.ColumnHeaders[Categories_Form.Column.CategoryName], 0, Panel);

            _controlToFocus = ConstructTextBox(0, columnName, cellValue, 50, CustomControls.KeyPressValidation.None, true, Panel);
            _controlToFocus.TextChanged += Category_TextBox_TextChanged;

            ConstructWarningLabel();
        }
        private void Category_TextBox_TextChanged(object sender, EventArgs e)
        {
            Guna2TextBox textBox = (Guna2TextBox)sender;
            if (textBox.Text == "")
            {
                SetControlForTextBoxEmpty(textBox);
                return;
            }

            // Get list
            List<Category> categoriesList;
            if (Categories_Form.Instance.Purchase_RadioButton.Checked)
            {
                categoriesList = MainMenu_Form.Instance.CategoryPurchaseList;
            }
            else
            {
                categoriesList = MainMenu_Form.Instance.CategorySaleList;
            }

            bool containsCategory = categoriesList.Any(category => category.Name.Equals(textBox.Text.Trim(), StringComparison.OrdinalIgnoreCase));
            bool isOldValueDifferent = !string.Equals(_listOfOldValues[0], textBox.Text.Trim(), StringComparison.OrdinalIgnoreCase);

            if (containsCategory && isOldValueDifferent)
            {
                DisableSaveButton();
                CustomControls.SetGTextBoxToInvalid(textBox);
                ShowWarning(textBox, "Category already exists");
            }
            else
            {
                EnableSaveButton();
                CustomControls.SetGTextBoxToValid(textBox);
                HideWarning();
            }
        }
        private void ConstructControlsForCompany()
        {
            string columnName = _selectedRow.DataGridView.Columns[0].Name;
            string cellValue = _selectedRow.Cells[0].Value?.ToString() ?? ReadOnlyVariables.EmptyCell;
            _listOfOldValues.Add(cellValue);

            ConstructLabel(Companies_Form.ColumnHeaders[Companies_Form.Column.Company], 0, Panel);

            _controlToFocus = ConstructTextBox(0, columnName, cellValue, 50, CustomControls.KeyPressValidation.None, true, Panel);
            _controlToFocus.TextChanged += Company_TextBox_TextChanged;

            ConstructWarningLabel();
        }
        private void Company_TextBox_TextChanged(object sender, EventArgs e)
        {
            Guna2TextBox textBox = (Guna2TextBox)sender;
            if (textBox.Text == "")
            {
                SetControlForTextBoxEmpty(textBox);
                return;
            }

            bool containsCompany = MainMenu_Form.Instance.CompanyList.Contains(textBox.Text.Trim(), StringComparer.OrdinalIgnoreCase);
            bool isOldValueDifferent = !string.Equals(_listOfOldValues[0], textBox.Text.Trim(), StringComparison.OrdinalIgnoreCase);

            if (containsCompany && isOldValueDifferent)
            {
                DisableSaveButton();
                CustomControls.SetGTextBoxToInvalid(textBox);
                ShowWarning(textBox, "Company already exists");
            }
            else
            {
                EnableSaveButton();
                CustomControls.SetGTextBoxToValid(textBox);
                HideWarning();
            }
        }
        private int ConstructControlsForProduct()
        {
            int left = 0;
            int searchBoxMaxHeight = 100;
            int productIDLeft = 0; // Store ProductID position for Rentable checkbox

            foreach (DataGridViewColumn column in _selectedRow.DataGridView.Columns)
            {
                string columnName = column.Name;
                string cellValue = _selectedRow.Cells[column.Index].Value?.ToString() ?? ReadOnlyVariables.EmptyCell;
                _listOfOldValues.Add(cellValue);

                switch (columnName)
                {
                    case nameof(Products_Form.Column.ProductID):
                        ConstructLabel(Products_Form.ColumnHeaders[Products_Form.Column.ProductID], left, Panel);
                        _controlToFocus = ConstructTextBox(left, columnName, cellValue, 50, CustomControls.KeyPressValidation.None, false, Panel);
                        productIDLeft = left;  // Store position for Rentable checkbox
                        left += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(Products_Form.Column.ProductName):
                        ConstructLabel(Products_Form.ColumnHeaders[Products_Form.Column.ProductName], left, Panel);
                        Guna2TextBox productNameTextBox = ConstructTextBox(left, columnName, cellValue, 50, CustomControls.KeyPressValidation.None, false, Panel);
                        _oldProductName = cellValue;
                        productNameTextBox.TextChanged += ValidateInputs;
                        left += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(Products_Form.Column.ProductCategory):
                        ConstructLabel(Products_Form.ColumnHeaders[Products_Form.Column.ProductCategory], left, Panel);
                        Guna2TextBox textBox = ConstructTextBox(left, columnName, cellValue, 50, CustomControls.KeyPressValidation.None, false, Panel);
                        // Attach SearchBox for categories
                        List<Category> categoryList = Products_Form.Instance.Purchase_RadioButton.Checked
                            ? MainMenu_Form.Instance.CategoryPurchaseList
                            : MainMenu_Form.Instance.CategorySaleList;
                        List<SearchResult> categorySearchResults = SearchBox.ConvertToSearchResults(categoryList.Select(c => c.Name).ToList());
                        SearchBox.Attach(textBox, this, () => categorySearchResults, searchBoxMaxHeight, false, false, false, true);
                        textBox.TextChanged += ValidateInputs;
                        left += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(Products_Form.Column.CountryOfOrigin):
                        ConstructLabel(Products_Form.ColumnHeaders[Products_Form.Column.CountryOfOrigin], left, Panel);
                        textBox = ConstructTextBox(left, columnName, cellValue, 50, CustomControls.KeyPressValidation.None, false, Panel);
                        SearchBox.Attach(textBox, this, () => Country.CountrySearchResults, searchBoxMaxHeight, false, true, false, false);
                        textBox.TextChanged += ValidateInputs;
                        left += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(Products_Form.Column.CompanyOfOrigin):
                        ConstructLabel(Products_Form.ColumnHeaders[Products_Form.Column.CompanyOfOrigin], left, Panel);
                        textBox = ConstructTextBox(left, columnName, cellValue, 50, CustomControls.KeyPressValidation.None, false, Panel);
                        List<SearchResult> searchResult = SearchBox.ConvertToSearchResults(MainMenu_Form.Instance.CompanyList);
                        SearchBox.Attach(textBox, this, () => searchResult, searchBoxMaxHeight, false, false, false, true);
                        textBox.TextChanged += ValidateInputs;
                        left += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(Products_Form.Column.Type):
                        ConstructLabel(Products_Form.ColumnHeaders[Products_Form.Column.Type], left, Panel);
                        Guna2ComboBox comboBox = ConstructComboBox(left, columnName, cellValue, Panel);
                        comboBox.SelectedIndexChanged += ValidateInputs;
                        left += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;
                }
            }

            return left - CustomControls.SpaceBetweenControls;
        }

        // Construct controls for purchase or sale
        private Label _selectedReceipt_Label;
        private Guna2ImageButton _removeReceipt_ImageButton;
        private Guna2Button _receipt_Button;
        private bool _containsReceipt, _removedReceipt, _addedReceipt;
        private (int, int) ConstructControlsForPurchaseOrSale()
        {
            ConstructSecondPanel();
            int left = 0, secondLeft = 0;
            byte searchBoxMaxHeight = 200;

            foreach (DataGridViewColumn column in _selectedRow.DataGridView.Columns)
            {
                string columnName = column.Name;
                string cellValue = _selectedRow.Cells[column.Index].Value?.ToString() ?? ReadOnlyVariables.EmptyCell;
                _listOfOldValues.Add(cellValue);
                string text;
                List<SearchResult> searchResult;

                switch (columnName)
                {
                    case nameof(MainMenu_Form.Column.ID):
                        if (MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.Sales)
                        {
                            text = MainMenu_Form.Instance.SalesColumnHeaders[MainMenu_Form.Column.ID];
                        }
                        else { text = MainMenu_Form.Instance.PurchaseColumnHeaders[MainMenu_Form.Column.ID]; }

                        ConstructLabel(text, left, Panel);
                        _controlToFocus = ConstructTextBox(left, columnName, cellValue, 50, CustomControls.KeyPressValidation.None, false, Panel);
                        left += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(MainMenu_Form.Column.Accountant):
                        if (MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.Sales)
                        {
                            text = MainMenu_Form.Instance.SalesColumnHeaders[MainMenu_Form.Column.Accountant];
                        }
                        else { text = MainMenu_Form.Instance.PurchaseColumnHeaders[MainMenu_Form.Column.Accountant]; }

                        ConstructLabel(text, left, Panel);
                        Guna2TextBox Accountant_TextBox = ConstructTextBox(left, columnName, cellValue, 50, CustomControls.KeyPressValidation.None, false, Panel);
                        searchResult = SearchBox.ConvertToSearchResults(MainMenu_Form.Instance.AccountantList);
                        SearchBox.Attach(Accountant_TextBox, this, () => searchResult, searchBoxMaxHeight, false, false, false, true);
                        Accountant_TextBox.TextChanged += ValidateInputs;
                        left += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(MainMenu_Form.Column.Product):
                        if (cellValue != ReadOnlyVariables.MultipleItems_text)
                        {
                            ConstructLabel(MainMenu_Form.Instance.PurchaseColumnHeaders[MainMenu_Form.Column.Product], left, Panel);
                            string fullProductPath = GetFullProductPath(_selectedRow, cellValue);
                            Guna2TextBox ProductName_TextBox = ConstructTextBox(left, columnName, fullProductPath, 50, CustomControls.KeyPressValidation.None, false, Panel);
                            SearchBox.Attach(ProductName_TextBox, this, GetProductListForSearchBox, searchBoxMaxHeight, true, false, false, true);
                            ProductName_TextBox.TextChanged += ValidateInputs;
                            ProductName_TextBox.Width = ScaledLargeWidth;
                            left += ScaledLargeWidth + CustomControls.SpaceBetweenControls;
                            _oldProductName = cellValue;
                        }

                        int buttonWidth = (int)(210 * DpiHelper.GetRelativeDpiScale());
                        _receipt_Button = new()
                        {
                            Location = new Point(left, 45 + CustomControls.SpaceBetweenControls),
                            Text = "Add receipt",
                            BackColor = CustomColors.ControlBack,
                            FillColor = CustomColors.ControlBack,
                            Size = new Size(buttonWidth, ScaledControlHeight),
                            BorderRadius = 2,
                            BorderThickness = 1,
                            Font = new Font("Segoe UI", 10),
                        };
                        _receipt_Button.Click += (_, _) =>
                        {
                            OpenFileDialog dialog = new();
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                if (Controls.Contains(_selectedReceipt_Label))
                                {
                                    _removedReceipt = true;
                                }
                                ShowReceiptLabel(dialog.SafeFileName);
                                _receiptFilePath = dialog.FileName;
                                _addedReceipt = true;
                            }
                        };
                        Panel.Controls.Add(_receipt_Button);
                        left += buttonWidth + CustomControls.SpaceBetweenControls;

                        // ImageButton
                        _removeReceipt_ImageButton = new()
                        {
                            Size = new Size(38, 38),
                            ImageSize = new Size(30, 30),
                            Image = Properties.Resources.CloseGray,
                            Anchor = AnchorStyles.Top,
                            HoverState = { ImageSize = new Size(30, 30) },
                            PressedState = { ImageSize = new Size(30, 30) }
                        };
                        _removeReceipt_ImageButton.Click += (_, _) =>
                        {
                            RemoveReceiptLabel();
                            _removedReceipt = true;
                        };
                        _removeReceipt_ImageButton.MouseEnter += (_, _) =>
                        {
                            _removeReceipt_ImageButton.BackColor = CustomColors.MouseHover;
                        };
                        _removeReceipt_ImageButton.MouseLeave += (_, _) =>
                        {
                            _removeReceipt_ImageButton.BackColor = CustomColors.MainBackground;
                        };

                        // Label
                        _selectedReceipt_Label = new()
                        {
                            ForeColor = CustomColors.Text,
                            Font = new Font("Segoe UI", 10),
                            AutoSize = true,
                            Anchor = AnchorStyles.Top
                        };
                        break;

                    case nameof(MainMenu_Form.Column.Date):
                        ConstructLabel(MainMenu_Form.Instance.PurchaseColumnHeaders[MainMenu_Form.Column.Date], secondLeft, _secondPanel);
                        DateTime date = Tools.ParseDateOrToday(cellValue);
                        ConstructDatePicker(secondLeft, columnName, date, _secondPanel);
                        secondLeft += ScaledDatePickerWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(MainMenu_Form.Column.TotalItems):
                        if (cellValue == ReadOnlyVariables.EmptyCell) { continue; }

                        string productName = _selectedRow.Cells[ReadOnlyVariables.Product_column].Value.ToString();
                        if (productName == ReadOnlyVariables.MultipleItems_text) { continue; }

                        ConstructLabel(MainMenu_Form.Instance.PurchaseColumnHeaders[MainMenu_Form.Column.TotalItems], secondLeft, _secondPanel);
                        ConstructTextBox(secondLeft, columnName, cellValue, 10, CustomControls.KeyPressValidation.OnlyNumbers, false, _secondPanel);
                        secondLeft += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(MainMenu_Form.Column.PricePerUnit):
                        if (cellValue == ReadOnlyVariables.EmptyCell) { continue; }

                        ConstructLabel(MainMenu_Form.Instance.PurchaseColumnHeaders[MainMenu_Form.Column.PricePerUnit], secondLeft, _secondPanel);
                        ConstructTextBox(secondLeft, columnName, cellValue, 10, CustomControls.KeyPressValidation.OnlyNumbersAndDecimal, false, _secondPanel);
                        secondLeft += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(MainMenu_Form.Column.Shipping):
                        ConstructLabel(MainMenu_Form.Instance.PurchaseColumnHeaders[MainMenu_Form.Column.Shipping], secondLeft, _secondPanel);
                        ConstructTextBox(secondLeft, columnName, cellValue, 10, CustomControls.KeyPressValidation.OnlyNumbersAndDecimal, true, _secondPanel);
                        secondLeft += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(MainMenu_Form.Column.Tax):
                        ConstructLabel(MainMenu_Form.Instance.PurchaseColumnHeaders[MainMenu_Form.Column.Tax], secondLeft, _secondPanel);
                        ConstructTextBox(secondLeft, columnName, cellValue, 10, CustomControls.KeyPressValidation.OnlyNumbersAndDecimal, false, _secondPanel);
                        secondLeft += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(MainMenu_Form.Column.Fee):
                        ConstructLabel(MainMenu_Form.Instance.PurchaseColumnHeaders[MainMenu_Form.Column.Fee], secondLeft, _secondPanel);
                        ConstructTextBox(secondLeft, columnName, cellValue, 10, CustomControls.KeyPressValidation.OnlyNumbersAndDecimal, false, _secondPanel);
                        secondLeft += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(MainMenu_Form.Column.Discount):
                        ConstructLabel(MainMenu_Form.Instance.PurchaseColumnHeaders[MainMenu_Form.Column.Discount], secondLeft, _secondPanel);
                        ConstructTextBox(secondLeft, columnName, cellValue, 10, CustomControls.KeyPressValidation.OnlyNumbersAndDecimal, false, _secondPanel);
                        secondLeft += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(MainMenu_Form.Column.Total):
                        if (MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.Sales)
                        {
                            text = MainMenu_Form.Instance.SalesColumnHeaders[MainMenu_Form.Column.Total];
                        }
                        else { text = MainMenu_Form.Instance.PurchaseColumnHeaders[MainMenu_Form.Column.Total]; }

                        ConstructLabel(text, secondLeft, _secondPanel);
                        ConstructTextBox(secondLeft, columnName, cellValue, 10, CustomControls.KeyPressValidation.OnlyNumbersAndDecimal, false, _secondPanel);
                        secondLeft += ScaledStandardWidth;
                        break;

                    case nameof(MainMenu_Form.Column.Note):
                        Label label = ConstructLabel("Notes", secondLeft, this);

                        string note = _selectedRow.Cells[column.Index].Tag?.ToString() ?? "";

                        Guna2TextBox textBox = ConstructTextBox(0, columnName, note, 10, CustomControls.KeyPressValidation.None, false, this);

                        label.Location = new Point((ClientSize.Width - label.Width) / 2, _secondPanel.Bottom);
                        label.Anchor = AnchorStyles.Top;

                        int notesWidth = (int)(525 * DpiHelper.GetRelativeDpiScale());
                        int notesHeight = (int)(105 * DpiHelper.GetRelativeDpiScale());

                        textBox.Size = new Size(notesWidth, notesHeight);
                        textBox.MinimumSize = new Size(notesWidth, notesHeight);
                        textBox.Location = new Point((ClientSize.Width - textBox.Width) / 2, label.Bottom + CustomControls.SpaceBetweenControls);
                        textBox.Anchor = AnchorStyles.Top;
                        textBox.MaxLength = 1000;
                        textBox.Multiline = true;

                        _notes = true;
                        break;
                }
            }
            return (left, secondLeft);
        }
        private int ConstructControlsForItemsInTransaction()
        {
            int left = 0;
            byte searchBoxMaxHeight = 200;

            foreach (DataGridViewColumn column in _selectedRow.DataGridView.Columns)
            {
                string columnName = column.Name;
                string cellValue = _selectedRow.Cells[column.Index].Value?.ToString() ?? ReadOnlyVariables.EmptyCell;
                _listOfOldValues.Add(cellValue);

                switch (columnName)
                {
                    case nameof(MainMenu_Form.Column.Product):
                        ConstructLabel(MainMenu_Form.Instance.PurchaseColumnHeaders[MainMenu_Form.Column.Product], 0, Panel);

                        string fullProductPath = GetFullProductPath(_selectedRow, cellValue);
                        Guna2TextBox ProductName_TextBox = ConstructTextBox(left, columnName, fullProductPath, 50, CustomControls.KeyPressValidation.None, false, Panel);
                        SearchBox.Attach(ProductName_TextBox, this, GetProductListForSearchBox, searchBoxMaxHeight, true, false, false, true);
                        ProductName_TextBox.TextChanged += ValidateInputs;

                        left += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(MainMenu_Form.Column.TotalItems):
                        ConstructLabel(MainMenu_Form.Instance.PurchaseColumnHeaders[MainMenu_Form.Column.TotalItems], left, Panel);
                        ConstructTextBox(left, columnName, cellValue, 50, CustomControls.KeyPressValidation.OnlyNumbers, true, Panel);
                        left += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(MainMenu_Form.Column.PricePerUnit):
                        ConstructLabel(MainMenu_Form.Instance.PurchaseColumnHeaders[MainMenu_Form.Column.PricePerUnit], left, Panel);
                        ConstructTextBox(left, columnName, cellValue, 50, CustomControls.KeyPressValidation.OnlyNumbersAndDecimal, true, Panel);
                        left += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(MainMenu_Form.Column.Total):
                        ConstructLabel(MainMenu_Form.Instance.PurchaseColumnHeaders[MainMenu_Form.Column.Total], left, Panel);
                        ConstructTextBox(left, columnName, cellValue, 50, CustomControls.KeyPressValidation.OnlyNumbersAndDecimal, true, Panel);
                        left += ScaledStandardWidth;
                        break;
                }
            }
            return left;
        }

        // Construct controls for customer
        private CountryCode _selectedCountryCode;
        private Guna2TextBox _countryCodeTextBox;
        private Guna2TextBox _phoneNumberTextBox;
        private (int, int) ConstructControlsForCustomer()
        {
            int left = 0;
            int secondLeft = 0;

            foreach (DataGridViewColumn column in _selectedRow.DataGridView.Columns)
            {
                string columnName = column.Name;
                string cellValue = _selectedRow.Cells[column.Index].Value?.ToString() ?? ReadOnlyVariables.EmptyCell;
                _listOfOldValues.Add(cellValue);

                switch (columnName)
                {
                    case nameof(Customers_Form.Column.CustomerID):
                        ConstructLabel(Customers_Form.ColumnHeaders[Customers_Form.Column.CustomerID], left, Panel);
                        ConstructTextBox(left, columnName, cellValue, 50, CustomControls.KeyPressValidation.None, false, Panel);
                        left += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(Customers_Form.Column.FirstName):
                        ConstructLabel(Customers_Form.ColumnHeaders[Customers_Form.Column.FirstName], left, Panel);
                        _controlToFocus = ConstructTextBox(left, columnName, cellValue, 100, CustomControls.KeyPressValidation.OnlyLetters, false, Panel);
                        left += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(Customers_Form.Column.LastName):
                        ConstructLabel(Customers_Form.ColumnHeaders[Customers_Form.Column.LastName], left, Panel);
                        ConstructTextBox(left, columnName, cellValue, 100, CustomControls.KeyPressValidation.OnlyLetters, false, Panel);
                        left += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(Customers_Form.Column.Email):
                        ConstructLabel(Customers_Form.ColumnHeaders[Customers_Form.Column.Email], left, Panel);
                        Guna2TextBox emailTextBox = ConstructTextBox(left, columnName, cellValue, 100, CustomControls.KeyPressValidation.None, false, Panel);
                        emailTextBox.TextChanged += ValidateInputs;
                        left += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(Customers_Form.Column.Address):
                        ConstructLabel(Customers_Form.ColumnHeaders[Customers_Form.Column.Address], left, Panel);
                        ConstructTextBox(left, columnName, cellValue, 200, CustomControls.KeyPressValidation.None, false, Panel);
                        left += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;

                    case nameof(Customers_Form.Column.PhoneNumber):
                        // Parse phone number to extract country code and number
                        (string countryCode, string phoneNumber) = ParsePhoneNumber(cellValue);
                        _selectedCountryCode = CountryCode.GetCountryCodeFromText(countryCode);

                        ConstructSecondPanel();

                        // Country code search box with label "phone number ext."
                        ConstructLabel("phone number ext.", secondLeft, _secondPanel);
                        _countryCodeTextBox = ConstructTextBox(secondLeft, "CountryCode_TextBox", countryCode, 10, CustomControls.KeyPressValidation.None, false, _secondPanel);
                        float scale = DpiHelper.GetRelativeDpiScale();
                        int searchBoxMaxHeight = (int)(255 * scale);
                        SearchBox.Attach(_countryCodeTextBox, this, CountryCode.GetCountryCodeSearchResults, searchBoxMaxHeight, false, true, true, false);
                        _countryCodeTextBox.TextChanged += CountryCode_TextBox_TextChanged;
                        secondLeft += ScaledStandardWidth + CustomControls.SpaceBetweenControls;

                        // Phone number text box with label "phone number"
                        ConstructLabel("phone number", secondLeft, _secondPanel);
                        _phoneNumberTextBox = ConstructTextBox(secondLeft, columnName, phoneNumber, 30, CustomControls.KeyPressValidation.None, false, _secondPanel);
                        _phoneNumberTextBox.TextChanged += PhoneNumber_TextBox_TextChanged;
                        _phoneNumberTextBox.TextChanged += ValidateInputs;

                        secondLeft += ScaledStandardWidth + CustomControls.SpaceBetweenControls;
                        break;
                }
            }

            // Add notes textbox at the bottom
            _notes = true;
            Customer customer = _selectedRow.Tag as Customer;
            string notes = customer?.Notes ?? "";

            Label notesLabel = ConstructLabel("Notes", 0, this);
            Guna2TextBox notesTextBox = ConstructTextBox(0, "Notes_TextBox", notes, 10, CustomControls.KeyPressValidation.None, false, this);

            notesLabel.Location = new Point((ClientSize.Width - notesLabel.Width) / 2, _secondPanel.Bottom);
            notesLabel.Anchor = AnchorStyles.Top;

            int notesWidth = (int)(525 * DpiHelper.GetRelativeDpiScale());
            notesTextBox.Location = new Point((ClientSize.Width - notesWidth) / 2, notesLabel.Bottom + 5);
            notesTextBox.Multiline = true;
            notesTextBox.Height = 125;
            notesTextBox.Width = notesWidth;
            notesTextBox.Anchor = AnchorStyles.Top;
            TextBoxManager.Attach(notesTextBox);

            return (left, secondLeft);
        }
        private static (string countryCode, string phoneNumber) ParsePhoneNumber(string fullPhoneNumber)
        {
            if (string.IsNullOrWhiteSpace(fullPhoneNumber) || fullPhoneNumber == ReadOnlyVariables.EmptyCell)
            {
                return ("+1", "");
            }

            // Try to extract country code (starts with +)
            if (fullPhoneNumber.StartsWith('+'))
            {
                int spaceIndex = fullPhoneNumber.IndexOf(' ');
                if (spaceIndex > 0)
                {
                    string code = fullPhoneNumber.Substring(0, spaceIndex);
                    string number = fullPhoneNumber.Substring(spaceIndex + 1);
                    return (code, number);
                }
            }

            return ("+1", fullPhoneNumber);
        }
        private void PhoneNumber_TextBox_TextChanged(object sender, EventArgs e)
        {
            FormatPhoneNumberInModifyForm();
        }
        private void FormatPhoneNumberInModifyForm()
        {
            if (_selectedCountryCode == null || _phoneNumberTextBox == null)
            {
                return;
            }

            int cursorPosition = _phoneNumberTextBox.SelectionStart;
            string currentText = _phoneNumberTextBox.Text;

            // Extract only digits from current text
            string digitsOnly = new(currentText.Where(char.IsDigit).ToArray());

            // Count maximum allowed digits based on format (count 'X' characters)
            int maxDigits = _selectedCountryCode.Format.Count(c => c == 'X');

            // Limit digits to the format's maximum
            if (digitsOnly.Length > maxDigits)
            {
                digitsOnly = digitsOnly.Substring(0, maxDigits);
            }

            // Format the phone number
            string formattedNumber = _selectedCountryCode.FormatPhoneNumber(digitsOnly);

            if (currentText != formattedNumber)
            {
                _phoneNumberTextBox.TextChanged -= PhoneNumber_TextBox_TextChanged;
                _phoneNumberTextBox.Text = formattedNumber;

                // Adjust cursor position
                int newCursorPosition = Math.Min(cursorPosition + (formattedNumber.Length - currentText.Length), formattedNumber.Length);
                _phoneNumberTextBox.SelectionStart = Math.Max(0, newCursorPosition);

                _phoneNumberTextBox.TextChanged += PhoneNumber_TextBox_TextChanged;
            }
        }
        private void CountryCode_TextBox_TextChanged(object sender, EventArgs e)
        {
            // Parse the country code from the search result text
            CountryCode? selectedCountry = CountryCode.GetCountryCodeFromText(_countryCodeTextBox.Text);
            if (selectedCountry != null)
            {
                _selectedCountryCode = selectedCountry;

                // Reformat the existing phone number with the new country format
                if (!string.IsNullOrWhiteSpace(_phoneNumberTextBox.Text))
                {
                    FormatPhoneNumberInModifyForm();
                }
            }
        }
        private static List<SearchResult> GetProductListForSearchBox()
        {
            if (MainMenu_Form.Instance.Selected is MainMenu_Form.SelectedOption.ItemsInSale or MainMenu_Form.SelectedOption.Sales)
            {
                return SearchBox.ConvertToSearchResults(MainMenu_Form.Instance.GetProductSaleNames());
            }
            else
            {
                return SearchBox.ConvertToSearchResults(MainMenu_Form.Instance.GetProductPurchaseNames());
            }
        }
        private static string GetFullProductPath(DataGridViewRow row, string productName)
        {
            string company = row.Cells[ReadOnlyVariables.Company_column].Value.ToString();
            string category = row.Cells[ReadOnlyVariables.Category_column].Value.ToString();
            return $"{company} > {category} > {productName}";
        }

        // Methods for receipts
        private void ShowReceiptLabel(string text)
        {
            _selectedReceipt_Label.Text = text;
            _receipt_Button.Text = LanguageManager.TranslateString("Change receipt");

            Controls.Add(_selectedReceipt_Label);
            Controls.Add(_removeReceipt_ImageButton);
            SetReceiptLabelLocation();
            _selectedReceipt_Label.BringToFront();
            _removeReceipt_ImageButton.BringToFront();

            ValidateInputs(null, null);
        }
        private void SetReceiptLabelLocation()
        {
            _removeReceipt_ImageButton.Location = new Point(
                _receipt_Button.Parent.Left + _receipt_Button.Right - _removeReceipt_ImageButton.Width,
                _receipt_Button.Parent.Top + _receipt_Button.Bottom + CustomControls.SpaceBetweenControls);

            _selectedReceipt_Label.Location = new Point(
                _removeReceipt_ImageButton.Left - _selectedReceipt_Label.Width,
                _removeReceipt_ImageButton.Top + (_removeReceipt_ImageButton.Height - _selectedReceipt_Label.Height) / 2 - 1);
        }
        private void RemoveReceiptLabel()
        {
            if (_receipt_Button != null)
            {
                Controls.Remove(_selectedReceipt_Label);
                Controls.Remove(_removeReceipt_ImageButton);

                _receipt_Button.Text = LanguageManager.TranslateString("Add receipt");

                ValidateInputs(null, null);
            }
        }

        // Warning label
        private PictureBox _warning_PictureBox;
        private Label _warning_Label;
        private void ConstructWarningLabel()
        {
            _warning_PictureBox = new()
            {
                Size = new Size(28, 28),
                Image = Properties.Resources.ExclamationMark,
                SizeMode = PictureBoxSizeMode.StretchImage,
            };

            _warning_Label = new()
            {
                ForeColor = CustomColors.Text,
                Font = new Font("Segoe UI", 10),
                AutoSize = true
            };
        }
        private void ShowWarning(Guna2TextBox textBox, string text)
        {
            _warning_PictureBox.Top = textBox.Top + textBox.Height + CustomControls.SpaceBetweenControls;
            _warning_PictureBox.Left = textBox.Left;

            _warning_Label.Top = _warning_PictureBox.Top;
            _warning_Label.Left = _warning_PictureBox.Right + CustomControls.SpaceBetweenControls;
            _warning_Label.Text = LanguageManager.TranslateString(text);

            Panel.Controls.Add(_warning_PictureBox);
            Panel.Controls.Add(_warning_Label);
        }
        private void HideWarning()
        {
            Panel.Controls.Remove(_warning_PictureBox);
            Panel.Controls.Remove(_warning_Label);
        }

        // Save in row
        private void SaveInSelectedRow()
        {
            IEnumerable<Control> allControls = Panel.Controls.Cast<Control>();

            if (_secondPanel != null)
            {
                allControls = allControls.Concat(_secondPanel.Controls.Cast<Control>());
            }
            if (_notes)
            {
                allControls = allControls.Concat(Controls.OfType<Guna2TextBox>());
            }

            // Track which columns have been processed to avoid overwriting
            HashSet<string> processedColumns = [];

            foreach (Control control in allControls)
            {
                if (control is Guna2TextBox textBox)
                {
                    string column = textBox.Name;

                    if (column == ReadOnlyVariables.PricePerUnit_column ||
                        column == ReadOnlyVariables.Shipping_column ||
                        column == ReadOnlyVariables.Tax_column ||
                        column == ReadOnlyVariables.Fee_column ||
                        column == ReadOnlyVariables.Discount_column ||
                        column == ReadOnlyVariables.Total_column)
                    {
                        ProcessNumericColumn(textBox, column);
                    }
                    else if (column == ReadOnlyVariables.Product_column)
                    {
                        ProcessProductColumn(textBox);
                    }
                    else if (column == nameof(Products_Form.Column.ProductName))
                    {
                        ProcessProductNameColumn(textBox, processedColumns);
                    }
                    else if (column == Products_Form.Column.ProductCategory.ToString())
                    {
                        if (!processedColumns.Contains(column))
                        {
                            ProcessProductCategoryColumn(textBox);
                        }
                    }
                    else if (column == ReadOnlyVariables.Note_column)
                    {
                        ProcessNoteColumn(textBox);
                    }
                    else if (column == nameof(Customers_Form.Column.PhoneNumber) && _selectedCountryCode != null)
                    {
                        // Save phone number with country code
                        string phoneNumber = textBox.Text.Trim();
                        if (!string.IsNullOrWhiteSpace(phoneNumber))
                        {
                            _selectedRow.Cells[column].Value = $"{_selectedCountryCode.Code} {phoneNumber}";
                        }
                        else
                        {
                            _selectedRow.Cells[column].Value = ReadOnlyVariables.EmptyCell;
                        }
                    }
                    // All other columns
                    else if (column != "Notes_TextBox" && column != "CountryCode_TextBox" && !processedColumns.Contains(column))
                    {
                        // ProductID is optional - set to EmptyCell if empty
                        if (column == nameof(Products_Form.Column.ProductID) && string.IsNullOrWhiteSpace(textBox.Text))
                        {
                            _selectedRow.Cells[column].Value = ReadOnlyVariables.EmptyCell;
                        }
                        else
                        {
                            _selectedRow.Cells[column].Value = textBox.Text.Trim();
                        }
                    }
                }
                else if (control is Guna2ComboBox comboBox)
                {
                    string columnName = comboBox.Name;
                    _selectedRow.Cells[columnName].Value = comboBox.SelectedItem.ToString().Trim();
                }
                else if (control is Guna2DateTimePicker datePicker)
                {
                    string columnName = datePicker.Name;
                    _selectedRow.Cells[columnName].Value = Tools.FormatDate(datePicker.Value);
                }
                else if (control is Guna2CustomCheckBox checkBox)
                {
                    string columnName = checkBox.Name;
                    _selectedRow.Cells[columnName].Value = checkBox.Checked;
                }
            }

            Close();
        }
        private void ProcessNumericColumn(Guna2TextBox textBox, string column)
        {
            if (decimal.TryParse(textBox.Text.Trim(), out decimal number))
            {
                _selectedRow.Cells[column].Value = string.Format("{0:N2}", number);
            }
        }
        private void ProcessProductColumn(Guna2TextBox textBox)
        {
            (string productName, string companyName) = ParseProductInfo(textBox.Text);
            MainMenu_Form.SelectedOption selected = MainMenu_Form.Instance.Selected;
            List<Category> categoryList = selected is MainMenu_Form.SelectedOption.Purchases or MainMenu_Form.SelectedOption.ItemsInPurchase
                ? MainMenu_Form.Instance.CategoryPurchaseList
                : MainMenu_Form.Instance.CategorySaleList;

            if (selected is MainMenu_Form.SelectedOption.Purchases or MainMenu_Form.SelectedOption.Sales)
            {
                UpdateItemsInTransaction(productName, companyName, categoryList, false);
            }
            else if (selected is MainMenu_Form.SelectedOption.ItemsInPurchase or MainMenu_Form.SelectedOption.ItemsInSale)
            {
                _selectedRow.Cells[ReadOnlyVariables.Product_column].Value = productName;

                Product product = MainMenu_Form.GetProductProductNameIsFrom(categoryList, productName, companyName);
                Category category = MainMenu_Form.GetCategoryProductNameIsFrom(categoryList, productName, companyName);

                DataGridViewCellCollection cells = _selectedRow.Cells;
                cells[ReadOnlyVariables.Category_column].Value = category.Name;
                cells[ReadOnlyVariables.Country_column].Value = product.CountryOfOrigin;
                cells[ReadOnlyVariables.Company_column].Value = product.CompanyOfOrigin;
            }
        }
        private void ProcessProductNameColumn(Guna2TextBox textBox, HashSet<string> processedColumns)
        {
            // ProductName is now a simple textbox - just update the cell directly
            _selectedRow.Cells[nameof(Products_Form.Column.ProductName)].Value = textBox.Text.Trim();
            processedColumns.Add(nameof(Products_Form.Column.ProductName));
        }
        private static (string ProductName, string CompanyName) ParseProductInfo(string text)
        {
            string[] parts = text.Split('>');
            if (parts.Length > 2)
            {
                return (
                    ProductName: parts[2].Trim(),
                    CompanyName: parts[0].Trim()
                );
            }
            return (
                ProductName: text.Trim(),
                CompanyName: text.Trim()
            );
        }
        private void ProcessNoteColumn(Guna2TextBox textBox)
        {
            DataGridViewCell cell = _selectedRow.Cells[ReadOnlyVariables.Note_column];
            string note = textBox.Text.Trim();

            if (note == "")
            {
                cell.Value = ReadOnlyVariables.EmptyCell;
                DataGridViewManager.RemoveUnderlineFromCell(cell);
            }
            else
            {
                cell.Value = ReadOnlyVariables.Show_text;
                DataGridViewManager.AddUnderlineToCell(cell);
            }

            cell.Tag = note;
        }

        // Methods
        private void ProcessProductCategoryColumn(Guna2TextBox textBox)
        {
            // Simply update the ProductCategory cell - UpdateProduct() will handle moving the product between categories
            _selectedRow.Cells[nameof(Products_Form.Column.ProductCategory)].Value = textBox.Text.Trim();
        }
        private void ValidateInputs(object sender, EventArgs e)
        {
            IEnumerable<Control> allControls = Panel.Controls.Cast<Control>();

            if (_secondPanel != null)
            {
                allControls = allControls.Concat(_secondPanel.Controls.Cast<Control>());
            }

            bool isValid = true;

            foreach (Control control in allControls)
            {
                if (control is Guna2TextBox gunaTextBox)
                {
                    // ProductID is optional for products
                    bool isProductID = _selectedTag == nameof(MainMenu_Form.DataGridViewTag.Product) &&
                                       gunaTextBox.Name == nameof(Products_Form.Column.ProductID);

                    // Email, Address, and PhoneNumber are optional for customers
                    bool isOptionalCustomerField = _selectedTag == nameof(MainMenu_Form.DataGridViewTag.Customer) &&
                                                   (gunaTextBox.Name == nameof(Customers_Form.Column.CustomerID) ||
                                                    gunaTextBox.Name == nameof(Customers_Form.Column.Email) ||
                                                    gunaTextBox.Name == nameof(Customers_Form.Column.Address) ||
                                                    gunaTextBox.Name == nameof(Customers_Form.Column.PhoneNumber));

                    bool isEmailField = gunaTextBox.Name == nameof(Customers_Form.Column.Email);
                    bool isValidEmail = !isEmailField || TextBoxValidation.IsValidEmail(gunaTextBox.Text);
                    bool isUniqueEmail = !isEmailField || !TextBoxValidation.IsEmailDuplicate(gunaTextBox.Text);

                    bool isEmpty = string.IsNullOrEmpty(gunaTextBox.Text) ||
                                   gunaTextBox.Text == ReadOnlyVariables.EmptyCell ||
                                   gunaTextBox.Tag?.ToString() == "0";

                    if (!isValidEmail)
                    {
                        ShowValidationMessage(gunaTextBox, LanguageManager.TranslateString("Invalid email format"));
                        gunaTextBox.BorderColor = CustomColors.AccentRed;
                        isValid = false;
                    }
                    else if (!isUniqueEmail)
                    {
                        ShowValidationMessage(gunaTextBox, LanguageManager.TranslateString("Email already exist"));
                        gunaTextBox.BorderColor = CustomColors.AccentRed;
                        isValid = false;
                    }
                    else if (isEmpty && !isProductID && !isOptionalCustomerField)
                    {
                        ShowValidationMessage(gunaTextBox, LanguageManager.TranslateString("This field is required"));
                        gunaTextBox.BorderColor = CustomColors.AccentRed;
                        isValid = false;
                    }
                    else
                    {
                        HideValidationMessage(gunaTextBox);
                        gunaTextBox.BorderColor = CustomColors.ControlBorder;
                    }
                }
                else if (control is Guna2ComboBox gunaComboBox)
                {
                    if (string.IsNullOrEmpty(gunaComboBox.Text))
                    {
                        ShowValidationMessage(gunaComboBox, LanguageManager.TranslateString("This field is required"));
                        gunaComboBox.BorderColor = CustomColors.AccentRed;
                        isValid = false;
                    }
                    else
                    {
                        HideValidationMessage(gunaComboBox);
                        gunaComboBox.BorderColor = CustomColors.ControlBorder;
                    }
                }
            }

            // Check for duplicate product names in the same category and company
            if (_selectedTag == nameof(MainMenu_Form.DataGridViewTag.Product))
            {
                Guna2TextBox productNameTextBox = allControls
                    .OfType<Guna2TextBox>()
                    .FirstOrDefault(tb => tb.Name == nameof(Products_Form.Column.ProductName));

                if (productNameTextBox != null && IsDuplicateProduct(allControls))
                {
                    string message = LanguageManager.TranslateString("A product with this name already exists in this category and company");
                    ShowValidationMessage(productNameTextBox, message);
                    productNameTextBox.BorderColor = CustomColors.AccentRed;
                    isValid = false;
                }
                else if (productNameTextBox != null && !string.IsNullOrWhiteSpace(productNameTextBox.Text))
                {
                    HideValidationMessage(productNameTextBox);
                    productNameTextBox.BorderColor = CustomColors.ControlBorder;
                }
            }

            if (!Controls.Contains(_selectedReceipt_Label) && _containsReceipt && Properties.Settings.Default.PurchaseReceipts)
            {
                isValid = false;
            }

            Save_Button.Enabled = isValid;
        }
        private void ShowValidationMessage(Control control, string message)
        {
            // Check if validation label already exists for this control
            if (_validationLabels.TryGetValue(control, out Label existingLabel))
            {
                existingLabel.Text = message;
                existingLabel.Visible = true;
                return;
            }

            // Create new validation label
            Label validationLabel = new()
            {
                Text = message,
                ForeColor = CustomColors.AccentRed,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(control.Left, control.Bottom + 2),
                Name = $"{control.Name}_ValidationLabel"
            };

            // Add to the appropriate panel
            Control parent = control.Parent;
            parent.Controls.Add(validationLabel);
            validationLabel.BringToFront();

            // Store reference
            _validationLabels[control] = validationLabel;
        }
        private void HideValidationMessage(Control control)
        {
            if (_validationLabels.TryGetValue(control, out Label validationLabel))
            {
                validationLabel.Visible = false;
            }
        }
        private bool IsDuplicateProduct(IEnumerable<Control> allControls)
        {
            // Get new values from textboxes
            string newProductName = allControls
                .OfType<Guna2TextBox>()
                .FirstOrDefault(tb => tb.Name == nameof(Products_Form.Column.ProductName))?.Text.Trim();

            string newCategory = allControls
                .OfType<Guna2TextBox>()
                .FirstOrDefault(tb => tb.Name == nameof(Products_Form.Column.ProductCategory))?.Text.Trim();

            string newCompany = allControls
                .OfType<Guna2TextBox>()
                .FirstOrDefault(tb => tb.Name == nameof(Products_Form.Column.CompanyOfOrigin))?.Text.Trim();

            if (string.IsNullOrWhiteSpace(newProductName) || string.IsNullOrWhiteSpace(newCategory) || string.IsNullOrWhiteSpace(newCompany))
            {
                return false; // Can't check for duplicates if values are missing
            }

            // Get old values
            string oldProductName = _listOfOldValues[1];
            string oldCategory = _listOfOldValues[2];
            string oldCompany = _listOfOldValues[4];

            // If nothing changed, it's not a duplicate
            if (newProductName == oldProductName && newCategory == oldCategory && newCompany == oldCompany)
            {
                return false;
            }

            // Get the appropriate category list
            List<Category> categoryList = Products_Form.Instance.Purchase_RadioButton.Checked
                ? MainMenu_Form.Instance.CategoryPurchaseList
                : MainMenu_Form.Instance.CategorySaleList;

            // Find the category
            Category category = categoryList.FirstOrDefault(c => c.Name == newCategory);
            if (category == null)
            {
                return false; // Category doesn't exist, can't be a duplicate
            }

            // Check if a product with this name and company already exists in this category
            bool duplicateExists = category.ProductList.Any(p =>
                p.Name.Equals(newProductName, StringComparison.OrdinalIgnoreCase) &&
                p.CompanyOfOrigin.Equals(newCompany, StringComparison.OrdinalIgnoreCase) &&
                !(p.Name.Equals(oldProductName, StringComparison.OrdinalIgnoreCase) &&
                  p.CompanyOfOrigin.Equals(oldCompany, StringComparison.OrdinalIgnoreCase)));

            return duplicateExists;
        }
        private void UpdateItemsInTransaction(string productName, string companyName, List<Category> categoryList, bool isProductForm)
        {
            string productColumn = isProductForm ? Products_Form.Column.ProductName.ToString() : ReadOnlyVariables.Product_column;
            _selectedRow.Cells[productColumn].Value = productName;

            string categoryColumn = isProductForm ? Products_Form.Column.ProductCategory.ToString() : ReadOnlyVariables.Category_column;
            string category = MainMenu_Form.GetCategoryNameProductIsFrom(categoryList, productName, companyName);
            _selectedRow.Cells[categoryColumn].Value = category;

            string countryColumn = isProductForm ? Products_Form.Column.CountryOfOrigin.ToString() : ReadOnlyVariables.Country_column;
            string country = MainMenu_Form.GetCountryProductIsFrom(categoryList, productName);
            _selectedRow.Cells[countryColumn].Value = country;

            string companyColumn = isProductForm ? Products_Form.Column.CompanyOfOrigin.ToString() : ReadOnlyVariables.Company_column;
            _selectedRow.Cells[companyColumn].Value = companyName;

            if (_selectedRow.Tag is not (List<string> itemList, TagData tagData))
            {
                return;
            }

            for (int i = 0; i < itemList.Count; i++)
            {
                string item = itemList[i];
                if (item.Contains(ReadOnlyVariables.Receipt_text))
                {
                    continue;
                }

                string[] items = item.Split(',');

                if (items[0] == _oldProductName)
                {
                    items[0] = productName;
                    items[1] = category;
                    items[2] = country;
                    items[3] = companyName;

                    itemList[i] = string.Join(",", items);
                }
            }

            _selectedRow.Tag = (itemList, tagData);
        }
        private void UpdateChargedDifferenceInMainMenuRow()
        {
            if (_selectedTag == MainMenu_Form.DataGridViewTag.SaleOrPurchase.ToString())
            {
                MainMenu_Form.IsProgramLoading = true;

                string productName = _selectedRow.Cells[ReadOnlyVariables.Product_column].Value.ToString();

                if (productName == ReadOnlyVariables.MultipleItems_text)
                {
                    DataGridViewManager.UpdateChargedDifferenceInRowWithMultipleItems(_selectedRow);
                }
                else
                {
                    DataGridViewManager.UpdateChargedDifferenceInRowWithNoItems(_selectedRow);
                }

                MainMenu_Form.IsProgramLoading = false;
            }
        }
        private void HandleReceiptChanges()
        {
            if (MainMenu_Form.Instance.Selected != MainMenu_Form.SelectedOption.Sales
                && MainMenu_Form.Instance.Selected != MainMenu_Form.SelectedOption.Purchases)
            {
                return;
            }

            if (string.IsNullOrEmpty(_receiptFilePath)) { return; }

            // Case 1: Adding a receipt to a row that didn't have one
            if (_addedReceipt && !_removedReceipt)
            {
                (string newPath, bool saved) = ReceiptManager.SaveReceiptInFile(_receiptFilePath);
                if (saved)
                {
                    ReceiptManager.AddReceiptToTag(_selectedRow, newPath);
                }
            }

            // Case 2: Replacing an existing receipt with a new one
            else if (_removedReceipt && _addedReceipt)
            {
                ReceiptManager.RemoveReceiptFromTagAndFile(_selectedRow);

                // Then save and add the new receipt
                (string newPath, bool saved) = ReceiptManager.SaveReceiptInFile(_receiptFilePath);
                if (saved)
                {
                    ReceiptManager.AddReceiptToTag(_selectedRow, newPath);
                }
            }

            // Case 3: Removing a receipt
            else if (_removedReceipt && !_addedReceipt)
            {
                ReceiptManager.RemoveReceiptFromTagAndFile(_selectedRow);
            }
        }
        private static void UpdateAllDataGridViewRows(string columnName, string oldValue, string newValue, bool updateItemsInTransaction = false)
        {
            UpdateRowsInDataGridView(MainMenu_Form.Instance.Purchase_DataGridView, columnName, oldValue, newValue, updateItemsInTransaction);
            UpdateRowsInDataGridView(MainMenu_Form.Instance.Sale_DataGridView, columnName, oldValue, newValue, updateItemsInTransaction);
        }
        private static void UpdateRowsInDataGridView(DataGridView dataGridView, string columnName, string oldValue, string newValue, bool updateItemsInTransaction)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells[columnName].Value.ToString() == oldValue)
                {
                    row.Cells[columnName].Value = newValue;
                    _hasChanges = true;

                    // Update items in transaction if requested
                    if (updateItemsInTransaction && row.Tag is (List<string> itemList, TagData tagData))
                    {
                        for (int i = 0; i < itemList.Count; i++)
                        {
                            string item = itemList[i];
                            if (item.Contains(ReadOnlyVariables.Receipt_text))
                            {
                                continue;
                            }

                            string[] items = item.Split(',');

                            // Update the relevant field based on the column name
                            switch (columnName)
                            {
                                case nameof(MainMenu_Form.Column.Product):
                                    items[0] = items[0] == oldValue ? newValue : items[0];
                                    break;
                                case nameof(MainMenu_Form.Column.Category):
                                    items[1] = items[1] == oldValue ? newValue : items[1];
                                    break;
                                case nameof(MainMenu_Form.Column.Country):
                                    items[2] = items[2] == oldValue ? newValue : items[2];
                                    break;
                                case nameof(MainMenu_Form.Column.Company):
                                    items[3] = items[3] == oldValue ? newValue : items[3];
                                    break;
                            }
                            itemList[i] = string.Join(",", items);
                        }
                        row.Tag = (itemList, tagData);
                    }
                }
            }
        }

        // Save in lists and update MainMenu_Form
        /// <summary>
        /// When changes are made to Categories_Form, Accountants_Form, etc., the changes are reflected in MainMenu_Form's lists and DataGridViews.
        /// </summary>
        private void SaveInListsAndUpdateMainMenuForm()
        {
            if (_selectedTag == MainMenu_Form.DataGridViewTag.SaleOrPurchase.ToString())
            {
                string type = MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.Purchases ? "purchase" : "sale";
                string id = _selectedRow.Cells[ReadOnlyVariables.ID_column].Value.ToString();
                CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 2, $"Modified {type} '{id}'");
                return;
            }

            switch (_selectedTag)
            {
                case nameof(MainMenu_Form.DataGridViewTag.Category):
                    UpdateCategory();
                    break;
                case nameof(MainMenu_Form.DataGridViewTag.Product):
                    UpdateProduct();
                    break;
                case nameof(MainMenu_Form.DataGridViewTag.Company):
                    UpdateCompany();
                    break;
                case nameof(MainMenu_Form.DataGridViewTag.Accountant):
                    UpdateAccountant();
                    break;
                case nameof(MainMenu_Form.DataGridViewTag.Customer):
                    UpdateCustomer();
                    break;
            }

            if (_hasChanges && _selectedTag != MainMenu_Form.SelectedOption.ItemsInPurchase.ToString())
            {
                MainMenu_Form.Instance.UpdateTotalLabels();
                MainMenu_Form.Instance.LoadOrRefreshMainCharts();
                MainMenu_Form.SaveDataGridViewToFileAsJson(MainMenu_Form.Instance.Purchase_DataGridView, MainMenu_Form.SelectedOption.Purchases);
                MainMenu_Form.SaveDataGridViewToFileAsJson(MainMenu_Form.Instance.Sale_DataGridView, MainMenu_Form.SelectedOption.Sales);
            }

            UpdateValidationInOpenForms();
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 2, $"Modified {_selectedTag} list");
        }
        private void UpdateCategory()
        {
            Category category;

            if (Categories_Form.Instance.Purchase_RadioButton.Checked)
            {
                category = MainMenu_Form.Instance.CategoryPurchaseList.FirstOrDefault(c => c.Name == _listOfOldValues[0]);
            }
            else
            {
                category = MainMenu_Form.Instance.CategorySaleList.FirstOrDefault(c => c.Name == _listOfOldValues[0]);
            }

            string oldCategory = category.Name;
            Guna2TextBox textBox = Panel.Controls.OfType<Guna2TextBox>().FirstOrDefault();
            string newCategory = textBox.Text;
            category.Name = newCategory;

            UpdateAllDataGridViewRows(ReadOnlyVariables.Category_column, oldCategory, newCategory, true);

            // Update Products_Form if it's open
            if (Products_Form.Instance != null)
            {
                UpdateProductsFormCategory(oldCategory, newCategory);
            }

            MainMenu_Form.Instance.SaveCategoriesToFile(MainMenu_Form.Instance.Selected);
        }
        private static void UpdateProductsFormCategory(string oldCategory, string newCategory)
        {
            // Update ProductCategory column in both Purchase and Sale DataGridViews
            UpdateRowsInDataGridView(Products_Form.Instance.Purchase_DataGridView,
                nameof(Products_Form.Column.ProductCategory), oldCategory, newCategory, false);
            UpdateRowsInDataGridView(Products_Form.Instance.Sale_DataGridView,
                nameof(Products_Form.Column.ProductCategory), oldCategory, newCategory, false);
        }
        private void UpdateProduct()
        {
            Category oldCategory;
            bool isProductPurchase = Products_Form.Instance.Purchase_RadioButton.Checked;
            List<Category> categoryList;

            if (isProductPurchase)
            {
                categoryList = MainMenu_Form.Instance.CategoryPurchaseList;
                oldCategory = categoryList.FirstOrDefault(c => c.Name == _listOfOldValues[2]);
            }
            else
            {
                categoryList = MainMenu_Form.Instance.CategorySaleList;
                oldCategory = categoryList.FirstOrDefault(c => c.Name == _listOfOldValues[2]);
            }

            Product product = oldCategory.ProductList.FirstOrDefault(p => p.Name == _listOfOldValues[1]);

            if (HasProductChanged(product))
            {
                // Get the new category name from the ProductName textbox (format: Company > Category > Product)
                string newCategoryName = GetNewCategoryFromProductName();

                // Check if category has changed
                if (newCategoryName != _listOfOldValues[2])
                {
                    // Move product to new category
                    Category newCategory = categoryList.FirstOrDefault(c => c.Name == newCategoryName);
                    if (newCategory != null)
                    {
                        oldCategory.ProductList.Remove(product);
                        newCategory.ProductList.Add(product);
                    }
                }

                UpdateProductDetails(product);
                UpdateProductInDataGridViews(product);

                // Save the correct category list based on the product type
                MainMenu_Form.SelectedOption optionToSave = isProductPurchase
                    ? MainMenu_Form.SelectedOption.CategoryPurchases
                    : MainMenu_Form.SelectedOption.CategorySales;
                MainMenu_Form.Instance.SaveCategoriesToFile(optionToSave);

                // Log the change
                string message = $"Modified product '{product.Name}'";
                CustomMessage_Form.AddThingThatHasChangedAndLogMessage(Products_Form.ThingsThatHaveChangedInFile, 4, message);
            }
        }
        private string GetNewCategoryFromProductName()
        {
            // Find the ProductCategory textbox
            Guna2TextBox categoryTextBox = Panel.Controls
                .OfType<Guna2TextBox>()
                .FirstOrDefault(tb => tb.Name == nameof(Products_Form.Column.ProductCategory));

            if (categoryTextBox != null && !string.IsNullOrWhiteSpace(categoryTextBox.Text))
            {
                return categoryTextBox.Text.Trim();
            }

            return _listOfOldValues[2];  // Return old category if not found
        }
        private bool HasProductChanged(Product product)
        {
            Product newProduct = GetNewProductInfo();
            string newCategoryName = GetNewCategoryFromProductName();

            return product.ProductID != newProduct.ProductID ||
                   product.Name != newProduct.Name ||
                   product.CountryOfOrigin != newProduct.CountryOfOrigin ||
                   product.CompanyOfOrigin != newProduct.CompanyOfOrigin ||
                   product.ItemType != newProduct.ItemType ||
                   newCategoryName != _listOfOldValues[2];
        }
        private void UpdateProductDetails(Product product)
        {
            Product newProduct = GetNewProductInfo();

            product.ProductID = newProduct.ProductID;
            product.Name = newProduct.Name;
            product.CountryOfOrigin = newProduct.CountryOfOrigin;
            product.CompanyOfOrigin = newProduct.CompanyOfOrigin;
            product.ItemType = newProduct.ItemType;
        }
        private Product GetNewProductInfo()
        {
            Product product = new();
            IEnumerable<Control> allControls = Panel.Controls.Cast<Control>();

            foreach (Control control in allControls)
            {
                if (control is Guna2TextBox textBox)
                {
                    switch (textBox.Name)
                    {
                        case nameof(Products_Form.Column.ProductID):
                            product.ProductID = string.IsNullOrWhiteSpace(textBox.Text) ? ReadOnlyVariables.EmptyCell : textBox.Text;
                            break;
                        case nameof(Products_Form.Column.ProductName):
                            product.Name = textBox.Text.Trim();
                            break;
                        case nameof(Products_Form.Column.CountryOfOrigin):
                            product.CountryOfOrigin = textBox.Text;
                            break;
                        case nameof(Products_Form.Column.CompanyOfOrigin):
                            product.CompanyOfOrigin = textBox.Text;
                            break;
                    }
                }
                else if (control is Guna2ComboBox comboBox && comboBox.Name == nameof(Products_Form.Column.Type))
                {
                    product.ItemType = comboBox.SelectedIndex == 0 ? Product.TypeOption.Product : Product.TypeOption.Service;
                }
            }
            return product;
        }
        private void UpdateProductInDataGridViews(Product product)
        {
            string productColumn = ReadOnlyVariables.Product_column;
            string oldName = _listOfOldValues[1];
            string oldID = _listOfOldValues[0];
            string oldCountry = _listOfOldValues[3];
            string oldCompany = _listOfOldValues[4];

            foreach (DataGridViewRow row in MainMenu_Form.Instance.GetAllRows())
            {
                if (row.Cells[productColumn].Value.ToString() == oldName)
                {
                    UpdateProductRowValues(row, product, oldID, oldCountry, oldCompany);
                }
            }
        }
        private static void UpdateProductRowValues(DataGridViewRow row, Product product, string oldID, string oldCountry, string oldCompany)
        {
            row.Cells[ReadOnlyVariables.Product_column].Value = product.Name;

            string idColumn = ReadOnlyVariables.ID_column;
            if (row.Cells[idColumn].Value.ToString() == oldID)
            {
                row.Cells[idColumn].Value = product.ProductID;
            }

            string countryColumn = ReadOnlyVariables.Country_column;
            if (row.Cells[countryColumn].Value.ToString() == oldCountry)
            {
                row.Cells[countryColumn].Value = product.CountryOfOrigin;
            }

            string companyColumn = ReadOnlyVariables.Company_column;
            if (row.Cells[companyColumn].Value.ToString() == oldCompany)
            {
                row.Cells[companyColumn].Value = product.CompanyOfOrigin;
            }
        }
        private void UpdateCompany()
        {
            string oldCompany = MainMenu_Form.Instance.CompanyList.FirstOrDefault(a => a == _listOfOldValues[0]);
            Guna2TextBox textBox = Panel.Controls.OfType<Guna2TextBox>().FirstOrDefault();
            string newCompany = textBox.Text;

            // Update company in the company list
            if (oldCompany != null)
            {
                int index = MainMenu_Form.Instance.CompanyList.IndexOf(oldCompany);
                MainMenu_Form.Instance.CompanyList[index] = newCompany;
            }

            UpdateCompanyInProducts(oldCompany, newCompany, MainMenu_Form.Instance.CategoryPurchaseList);
            UpdateCompanyInProducts(oldCompany, newCompany, MainMenu_Form.Instance.CategorySaleList);
            MainMenu_Form.Instance.SaveCategoriesToFile(MainMenu_Form.SelectedOption.CategoryPurchases);
            MainMenu_Form.Instance.SaveCategoriesToFile(MainMenu_Form.SelectedOption.CategorySales);

            UpdateAllDataGridViewRows(ReadOnlyVariables.Company_column, oldCompany, newCompany, true);
        }
        private static void UpdateCompanyInProducts(string oldCompany, string newCompany, List<Category> categoryList)
        {
            foreach (Category category in categoryList)
            {
                foreach (Product product in category.ProductList)
                {
                    if (product.CompanyOfOrigin == oldCompany)
                    {
                        product.CompanyOfOrigin = newCompany;
                        _hasChanges = true;
                    }
                }
            }
        }
        private void UpdateAccountant()
        {
            string oldAccountant = MainMenu_Form.Instance.AccountantList.FirstOrDefault(a => a == _listOfOldValues[0]);
            Guna2TextBox textBox = Panel.Controls.OfType<Guna2TextBox>().FirstOrDefault();
            string newAccountant = textBox.Text;

            if (oldAccountant != null)
            {
                int index = MainMenu_Form.Instance.AccountantList.IndexOf(oldAccountant);
                MainMenu_Form.Instance.AccountantList[index] = newAccountant;
            }

            UpdateAllDataGridViewRows(ReadOnlyVariables.Accountant_column, oldAccountant, newAccountant, false);
        }
        private void UpdateCustomer()
        {
            // Get the customer object from the row tag
            if (_selectedRow.Tag is not Customer customer)
            {
                return;
            }

            // Update customer object with new values from controls
            foreach (Control control in Panel.Controls)
            {
                if (control is Guna2TextBox textBox)
                {
                    switch (textBox.Name)
                    {
                        case nameof(Customers_Form.Column.CustomerID):
                            customer.CustomerID = textBox.Text.Trim();
                            break;
                        case nameof(Customers_Form.Column.FirstName):
                            customer.FirstName = textBox.Text.Trim();
                            break;
                        case nameof(Customers_Form.Column.LastName):
                            customer.LastName = textBox.Text.Trim();
                            break;
                        case nameof(Customers_Form.Column.Email):
                            customer.Email = textBox.Text.Trim();
                            break;
                        case nameof(Customers_Form.Column.PhoneNumber):
                            string phoneNumber = textBox.Text.Trim();
                            if (_selectedCountryCode != null && !string.IsNullOrWhiteSpace(phoneNumber))
                            {
                                customer.PhoneNumber = $"{_selectedCountryCode.Code} {phoneNumber}";
                            }
                            else
                            {
                                customer.PhoneNumber = ReadOnlyVariables.EmptyCell;
                            }
                            break;
                    }
                }
            }

            // Update address from second panel if it exists
            if (_secondPanel != null)
            {
                foreach (Control control in _secondPanel.Controls)
                {
                    if (control is Guna2TextBox textBox && textBox.Name == nameof(Customers_Form.Column.Address))
                    {
                        customer.Address = textBox.Text.Trim();
                        break;
                    }
                }
            }

            // Update notes from main form controls
            if (_notes)
            {
                foreach (Control control in Controls)
                {
                    if (control is Guna2TextBox textBox && textBox.Name == "Notes_TextBox")
                    {
                        customer.Notes = textBox.Text.Trim();
                        break;
                    }
                }
            }
        }

        // Validate TextBoxes in other forms
        private void UpdateValidationInOpenForms()
        {
            string newText = GetNewTextValue();

            if (Application.OpenForms[nameof(AddPurchase_Form)] is AddPurchase_Form purchaseForm)
            {
                switch (_selectedTag)
                {
                    case nameof(MainMenu_Form.DataGridViewTag.Product):
                        UpdateProductTextBox(purchaseForm.ProductName_TextBox, _listOfOldValues[1], newText);
                        break;
                    case nameof(MainMenu_Form.DataGridViewTag.Category):
                        UpdateCategoryInProductPath(purchaseForm.ProductName_TextBox, _listOfOldValues[0], newText);
                        break;
                    case nameof(MainMenu_Form.DataGridViewTag.Company):
                        UpdateCompanyInProductPath(purchaseForm.ProductName_TextBox, _listOfOldValues[0], newText);
                        break;
                }
            }

            if (Application.OpenForms[nameof(AddSale_Form)] is AddSale_Form saleForm)
            {
                switch (_selectedTag)
                {
                    case nameof(MainMenu_Form.DataGridViewTag.Product):
                        UpdateProductTextBox(saleForm.ProductName_TextBox, _listOfOldValues[1], newText);
                        break;
                    case nameof(MainMenu_Form.DataGridViewTag.Category):
                        UpdateCategoryInProductPath(saleForm.ProductName_TextBox, _listOfOldValues[0], newText);
                        break;
                    case nameof(MainMenu_Form.DataGridViewTag.Company):
                        UpdateCompanyInProductPath(saleForm.ProductName_TextBox, _listOfOldValues[0], newText);
                        break;
                }
            }

            if (Application.OpenForms[nameof(Products_Form)] is Products_Form productsForm)
            {
                switch (_selectedTag)
                {
                    case nameof(MainMenu_Form.DataGridViewTag.Product):
                        UpdateTextBox(productsForm.ProductName_TextBox, _listOfOldValues[1], newText);
                        break;
                    case nameof(MainMenu_Form.DataGridViewTag.Category):
                        UpdateTextBox(productsForm.ProductCategory_TextBox, _listOfOldValues[0], newText);
                        break;
                    case nameof(MainMenu_Form.DataGridViewTag.Company):
                        UpdateTextBox(productsForm.CompanyOfOrigin_TextBox, _listOfOldValues[0], newText);
                        break;
                }
            }
        }
        private string GetNewTextValue()
        {
            if (_selectedTag == nameof(MainMenu_Form.DataGridViewTag.Product))
            {
                return Panel.Controls.OfType<Guna2TextBox>()
                    .First(t => t.Name == Products_Form.Column.ProductName.ToString()).Text;
            }
            return Panel.Controls.OfType<Guna2TextBox>().First().Text;
        }
        private static void UpdateTextBox(Guna2TextBox textBox, string oldValue, string newValue)
        {
            if (textBox.Text == oldValue)
            {
                textBox.Text = newValue;
            }
        }
        private static void UpdateProductTextBox(Guna2TextBox textBox, string oldProduct, string newProduct)
        {
            if (textBox.Text.EndsWith($"> {oldProduct}"))
            {
                textBox.Text = textBox.Text.Replace($"> {oldProduct}", $"> {newProduct}");
            }
        }
        private static void UpdateCategoryInProductPath(Guna2TextBox textBox, string oldCategory, string newCategory)
        {
            if (textBox.Text.Contains($"> {oldCategory} >"))
            {
                textBox.Text = textBox.Text.Replace($"> {oldCategory} >", $"> {newCategory} >");
            }
        }
        private static void UpdateCompanyInProductPath(Guna2TextBox textBox, string oldCompany, string newCompany)
        {
            if (textBox.Text.StartsWith($"{oldCompany} >"))
            {
                textBox.Text = textBox.Text.Replace($"{oldCompany} >", $"{newCompany} >");
            }
        }

        // Construct controls
        private Panel _secondPanel;
        private void ConstructSecondPanel()
        {
            _secondRow = true;

            _secondPanel = new()
            {
                Size = Panel.Size,
                Location = new Point(Panel.Left, Panel.Bottom),
                Anchor = AnchorStyles.Top
            };
            Controls.Add(_secondPanel);
        }
        private static Label ConstructLabel(string text, int left, Control control)
        {
            Label label = new()
            {
                ForeColor = CustomColors.Text,
                Cursor = Cursors.Arrow,
                Location = new Point(left, 20),
                Text = text,
                Name = text + "_Label",  // This is needed for the language translation
                Font = new Font("Segoe UI", 11),
                AutoSize = true,
                AccessibleDescription = AccessibleDescriptionManager.DoNotCache
            };
            control.Controls.Add(label);

            return label;
        }
        private Guna2TextBox ConstructTextBox(
            int left,
            string name,
            string text,
            int maxLength,
            CustomControls.KeyPressValidation keyPressValidation,
            bool pressSaveButton,
            Control control)
        {
            Guna2TextBox textBox = new()
            {
                Location = new Point(left, 45 + CustomControls.SpaceBetweenControls),
                Height = ScaledControlHeight,
                Name = name,
                Text = text,
                ForeColor = CustomColors.Text,
                BackColor = CustomColors.ControlBack,
                Font = new Font("Segoe UI", 9),
                MaxLength = maxLength,
                FillColor = CustomColors.ControlBack,
                BorderColor = CustomColors.ControlBorder,
                ShortcutsEnabled = false,
                AccessibleDescription = AccessibleDescriptionManager.DoNotCache,
                FocusedState = {
                    FillColor = CustomColors.ControlBack,
                    BorderColor = CustomColors.AccentBlue
                },
                HoverState = { BorderColor = CustomColors.AccentBlue },
                Width = ScaledStandardWidth
            };

            // Assign the appropriate KeyPress event handler based on the keyPressValidation parameter
            switch (keyPressValidation)
            {
                case CustomControls.KeyPressValidation.OnlyNumbersAndDecimalAndMinus:
                    TextBoxValidation.OnlyAllowNumbersAndOneDecimalAndOneMinus(textBox);
                    break;
                case CustomControls.KeyPressValidation.OnlyNumbersAndDecimal:
                    TextBoxValidation.OnlyAllowNumbersAndOneDecimal(textBox);
                    break;
                case CustomControls.KeyPressValidation.OnlyNumbers:
                    TextBoxValidation.OnlyAllowNumbers(textBox);
                    break;
                case CustomControls.KeyPressValidation.OnlyLetters:
                    TextBoxValidation.OnlyAllowLetters(textBox);
                    break;
                case CustomControls.KeyPressValidation.None:
                    break;
            }

            textBox.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    // Remove Windows "ding" noise when user presses enter
                    e.SuppressKeyPress = true;

                    if (pressSaveButton)
                    {
                        if (Save_Button.Tag is bool tag && tag == true)
                        {
                            Save_Button.PerformClick();
                        }
                    }
                    else
                    {
                        // Press tab
                        SendKeys.Send("{TAB}");
                    }
                }
            };

            textBox.TextChanged += ValidateInputs;
            TextBoxManager.Attach(textBox);
            control.Controls.Add(textBox);

            return textBox;
        }
        private static Guna2DateTimePicker ConstructDatePicker(int left, string name, DateTime value, Control control)
        {
            Guna2DateTimePicker gDatePicker = new()
            {
                Location = new Point(left, 45 + CustomControls.SpaceBetweenControls),
                Size = new Size(ScaledDatePickerWidth, ScaledControlHeight),
                FillColor = CustomColors.ControlBack,
                ForeColor = CustomColors.Text,
                BorderColor = CustomColors.ControlBorder,
                BorderRadius = 3,
                Name = name,
                Value = value,
                Font = new Font("Segoe UI", 9),
                AccessibleDescription = AccessibleDescriptionManager.DoNotCache,
                HoverState = { BorderColor = CustomColors.AccentBlue }
            };
            control.Controls.Add(gDatePicker);

            return gDatePicker;
        }
        private static Guna2ComboBox ConstructComboBox(int left, string name, string value, Control control)
        {
            Guna2ComboBox comboBox = new()
            {
                Location = new Point(left, 45 + CustomControls.SpaceBetweenControls),
                Size = new Size(ScaledStandardWidth, ScaledControlHeight),
                ItemHeight = (int)(44 * DpiHelper.GetRelativeDpiScale()),
                FillColor = CustomColors.ControlBack,
                ForeColor = CustomColors.Text,
                BorderColor = CustomColors.ControlBorder,
                BorderRadius = 3,
                Name = name,
                Font = new Font("Segoe UI", 9),
                AccessibleDescription = AccessibleDescriptionManager.DoNotCache,
                HoverState = { BorderColor = CustomColors.AccentBlue },
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            comboBox.Items.Add("Product");
            comboBox.Items.Add("Service");
            comboBox.SelectedIndex = value == "Service" ? 1 : 0;

            control.Controls.Add(comboBox);
            return comboBox;
        }

        // Misc.
        private void DisableSaveButton()
        {
            Save_Button.Enabled = false;
            Save_Button.Tag = false;
        }
        private void EnableSaveButton()
        {
            Save_Button.Enabled = true;
            Save_Button.Tag = true;
        }
        private void SetControlForTextBoxEmpty(Guna2TextBox textBox)
        {
            CustomControls.SetGTextBoxToValid(textBox);
            HideWarning();
            DisableSaveButton();
        }
        public static void ClosePanels()
        {
            SearchBox.Close();
        }
    }
}
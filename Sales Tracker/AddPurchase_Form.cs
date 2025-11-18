using Guna.UI2.WinForms;
using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.GridView;
using Sales_Tracker.Language;
using Sales_Tracker.Properties;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker
{
    /// <summary>
    /// Form for adding new purchase transactions.
    /// </summary>
    public partial class AddPurchase_Form : BaseForm
    {
        // Properties
        private static AddPurchase_Form _instance;

        // Getter
        public static AddPurchase_Form Instance => _instance;
        public static List<string> ThingsThatHaveChangedInFile { get; } = [];

        // Init.
        public AddPurchase_Form()
        {
            InitializeComponent();
            _instance = this;

            AddEventHandlersToTextBoxes();
            DpiHelper.ScaleImageButton(RemoveReceipt_ImageButton);
            Date_DateTimePicker.Value = DateTime.Now;
            Date_DateTimePicker.MaxDate = DateTime.Now;
            CheckIfProductsExist();
            UpdateTheme();
            SetAccessibleDescriptions();
            LanguageManager.UpdateLanguageForControl(this);
            RecalculateMultipleItemsLayout();
            RemoveReceiptLabel();
            Charged_Label.Text = $"{MainMenu_Form.CurrencySymbol} charged ({DataFileManager.GetValue(AppDataSettings.DefaultCurrencyType)})";
            Currency_TextBox.Text = "CAD";

            PanelCloseFilter panelCloseFilter = new(this, ClosePanels, TextBoxManager.RightClickTextBox_Panel, SearchBox.SearchResultBoxContainer);
            Application.AddMessageFilter(panelCloseFilter);

            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void AddEventHandlersToTextBoxes()
        {
            byte searchBoxMaxHeight = 255;

            TextBoxManager.Attach(Currency_TextBox);
            SearchBox.Attach(Currency_TextBox, this, Currency.GetSearchResults, searchBoxMaxHeight, false, false, true, false);

            TextBoxManager.Attach(OrderNumber_TextBox);

            TextBoxManager.Attach(ProductName_TextBox);
            SearchBox.Attach(ProductName_TextBox, this, GetSearchResultsForProducts, searchBoxMaxHeight, true, false, true, true);
            ProductName_TextBox.TextChanged += ValidateInputs;

            TextBoxValidation.OnlyAllowNumbers(Quantity_TextBox);
            TextBoxManager.Attach(Quantity_TextBox);

            TextBoxValidation.OnlyAllowNumbersAndOneDecimal(PricePerUnit_TextBox);
            TextBoxManager.Attach(PricePerUnit_TextBox);

            TextBoxValidation.OnlyAllowNumbersAndOneDecimal(Shipping_TextBox);
            TextBoxManager.Attach(Shipping_TextBox);

            TextBoxValidation.OnlyAllowNumbersAndOneDecimal(Tax_TextBox);
            TextBoxManager.Attach(Tax_TextBox);

            TextBoxValidation.OnlyAllowNumbersAndOneDecimal(Fee_TextBox);
            TextBoxManager.Attach(Fee_TextBox);

            TextBoxValidation.OnlyAllowNumbersAndOneDecimal(Discount_TextBox);
            TextBoxManager.Attach(Discount_TextBox);

            TextBoxValidation.OnlyAllowNumbersAndOneDecimal(Charged_TextBox);
            TextBoxManager.Attach(Charged_TextBox);

            TextBoxManager.Attach(Notes_TextBox);
        }
        private List<SearchResult> GetSearchResultsForProducts()
        {
            return SearchBox.ConvertToSearchResults(MainMenu_Form.Instance.GetProductPurchaseNames());
        }
        private void SetAccessibleDescriptions()
        {
            Label[] labelsToAlignLeftCenter =
            [
                MultipleItems_Label,
                Currency_Label,
                OrderNumber_Label,
                ProductName_Label,
                Date_Label,
                Quantity_Label,
                PricePerUnit_Label,
                Shipping_Label,
                Tax_Label,
                Discount_Label,
                Charged_Label,
                WarningProduct_LinkLabel
            ];

            foreach (Label label in labelsToAlignLeftCenter)
            {
                label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            }

            SelectedReceipt_Label.AccessibleDescription = AccessibleDescriptionManager.AlignRight;
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);
            CustomControls.InitHoverEffectForImageButton(RemoveReceipt_ImageButton);
        }

        // Form event handlers
        private void AddPurchase_Form_Activated(object sender, EventArgs e)
        {
            CheckIfProductsExist();
        }
        private void AddPurchase_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
            OrderNumber_TextBox.Focus();
        }
        private void AddPurchase_Form_Resize(object sender, EventArgs e)
        {
            ClosePanels();
        }
        private void AddPurchase_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClosePanels();
        }

        // Event handlers
        private void AddPurchase_Button_Click(object sender, EventArgs e)
        {
            if (MainMenu_Form.Instance.Selected != MainMenu_Form.SelectedOption.Purchases)
            {
                MainMenu_Form.Instance.Purchases_Button.PerformClick();
            }

            if (_panelsForMultipleProducts_List.Count == 0 || !MultipleItems_CheckBox.Checked)
            {
                if (!AddPurchase()) { return; }
            }
            // When the user selects "multiple items in this order" but only adds one, treat it as one
            else if (_panelsForMultipleProducts_List.Count == 1)
            {
                // Extract details from the single panel and populate the single purchase fields
                Guna2Panel singlePanel = _panelsForMultipleProducts_List[0];
                ProductName_TextBox.Text = ((Guna2TextBox)singlePanel.Controls.Find(TextBoxnames.name.ToString(), false).FirstOrDefault()).Text;
                Quantity_TextBox.Text = ((Guna2TextBox)singlePanel.Controls.Find(TextBoxnames.quantity.ToString(), false).FirstOrDefault()).Text;
                PricePerUnit_TextBox.Text = ((Guna2TextBox)singlePanel.Controls.Find(TextBoxnames.pricePerUnit.ToString(), false).FirstOrDefault()).Text;

                if (!AddPurchase()) { return; }
            }
            else if (!AddPurchaseWithMultipleItems())
            {
                return;
            }

            ClearInputs();
        }
        private void MultipleItems_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (MultipleItems_CheckBox.Checked)
            {
                if (_addButton == null)
                {
                    CalculatePanelDimensions();
                    ConstructFlowPanel();
                    ConstructAddButton();
                    ConstructControlsForMultipleProducts();
                }
                SetControlsForMultipleProducts();
            }
            else
            {
                SetControlsForSingleProduct();
            }
        }
        private void WarningProduct_LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Tools.OpenForm(new Products_Form(true));
            CheckIfProductsExist();
        }
        private void Receipt_Button_Click(object sender, EventArgs e)
        {
            // Select file
            OpenFileDialog dialog = new();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _receiptFilePath = ReadOnlyVariables.Receipt_text + dialog.FileName;
                ShowReceiptLabel(dialog.SafeFileName);
            }
        }
        private void RemoveReceipt_ImageButton_Click(object sender, EventArgs e)
        {
            RemoveReceiptLabel();
        }
        private void MultipleItems_Label_Click(object sender, EventArgs e)
        {
            MultipleItems_CheckBox.Checked = !MultipleItems_CheckBox.Checked;
        }

        // Methods to add purchases
        private void ClearInputs()
        {
            RemoveReceiptLabel();
            OrderNumber_TextBox.Clear();
            ProductName_TextBox.Clear();
            Quantity_TextBox.Clear();
            PricePerUnit_TextBox.Clear();
            Shipping_TextBox.Clear();
            Tax_TextBox.Clear();
            Fee_TextBox.Clear();
            Discount_TextBox.Clear();
            Charged_TextBox.Clear();
            Notes_TextBox.Clear();
        }
        private bool AddPurchase()
        {
            string purchaseNumber = OrderNumber_TextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(purchaseNumber))
            {
                purchaseNumber = ReadOnlyVariables.EmptyCell;
            }

            // Check if purchase ID already exists
            if (purchaseNumber != ReadOnlyVariables.EmptyCell
                && DataGridViewManager.DoesValueExistInDataGridView(MainMenu_Form.Instance.Purchase_DataGridView, ReadOnlyVariables.ID_column, purchaseNumber))
            {
                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                    "Purchase # already exists",
                    "The purchase #{0} already exists. Would you like to add this purchase anyways?",
                    CustomMessageBoxIcon.Question,
                    CustomMessageBoxButtons.YesNo,
                    purchaseNumber);

                if (result != CustomMessageBoxResult.Yes)
                {
                    return false;
                }
            }

            // Get values from TextBoxes
            string[] items = ProductName_TextBox.Text.Split('>');
            string companyName = items[0].Trim();
            string categoryName = items[1].Trim();
            string productName = items[2].Trim();

            string country = MainMenu_Form.GetCountryProductIsFrom(MainMenu_Form.Instance.CategoryPurchaseList, productName);
            string date = Tools.FormatDate(Date_DateTimePicker.Value);
            int quantity = int.Parse(Quantity_TextBox.Text);
            decimal pricePerUnit = decimal.Parse(PricePerUnit_TextBox.Text);
            decimal shipping = string.IsNullOrWhiteSpace(Shipping_TextBox.Text) ? 0 : decimal.Parse(Shipping_TextBox.Text);
            decimal tax = string.IsNullOrWhiteSpace(Tax_TextBox.Text) ? 0 : decimal.Parse(Tax_TextBox.Text);
            decimal fee = string.IsNullOrWhiteSpace(Fee_TextBox.Text) ? 0 : decimal.Parse(Fee_TextBox.Text);
            decimal discount = string.IsNullOrWhiteSpace(Discount_TextBox.Text) ? 0 : decimal.Parse(Discount_TextBox.Text);
            string noteLabel = ReadOnlyVariables.EmptyCell;
            string note = Notes_TextBox.Text.Trim();
            if (note != "")
            {
                noteLabel = ReadOnlyVariables.Show_text;
            }

            // Convert selected currency to default
            decimal exchangeRateToDefault = Currency.GetExchangeRate(Currency_TextBox.Text, DataFileManager.GetValue(AppDataSettings.DefaultCurrencyType), date);
            if (exchangeRateToDefault == -1) { return false; }

            decimal pricePerUnitDefault = pricePerUnit * exchangeRateToDefault;
            decimal shippingDefault = shipping * exchangeRateToDefault;
            decimal taxDefault = tax * exchangeRateToDefault;
            decimal feeDefault = fee * exchangeRateToDefault;
            decimal discountDefault = discount * exchangeRateToDefault;
            decimal totalPriceDefault = Math.Round(pricePerUnitDefault * quantity + shippingDefault + taxDefault + feeDefault - discountDefault, 2);
            decimal charged = Math.Round(decimal.Parse(Charged_TextBox.Text), 2);
            decimal chargedDifference = charged - totalPriceDefault;

            if (chargedDifference != 0)
            {
                string currency = DataFileManager.GetValue(AppDataSettings.DefaultCurrencyType);
                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                    "Amount charged is different",
                    "Amount charged ({0}{1} {2}) is not equal to the total price of the purchase ({3}{4} {5}). The difference will be accounted for.",
                    CustomMessageBoxIcon.Exclamation,
                    CustomMessageBoxButtons.OkCancel,
                    MainMenu_Form.CurrencySymbol, charged, currency, MainMenu_Form.CurrencySymbol, totalPriceDefault, currency);

                if (result != CustomMessageBoxResult.Ok)
                {
                    return false;
                }
            }

            // Convert selected currency to USD
            decimal exchangeRateToUSD = Currency.GetExchangeRate(Currency_TextBox.Text, "USD", date);
            if (exchangeRateToUSD == -1) { return false; }

            decimal pricePerUnitUSD = Math.Round(pricePerUnit * exchangeRateToUSD, 2);
            decimal shippingUSD = Math.Round(shipping * exchangeRateToUSD, 2);
            decimal taxUSD = Math.Round(tax * exchangeRateToUSD, 2);
            decimal feeUSD = Math.Round(fee * exchangeRateToUSD, 2);
            decimal discountUSD = Math.Round(discount * exchangeRateToUSD, 2);

            string defaultCurrency = DataFileManager.GetValue(AppDataSettings.DefaultCurrencyType);

            // Convert default currency to USD
            decimal defaultToUSD = Currency.GetExchangeRate(defaultCurrency, "USD", date);
            decimal chargedDifferenceUSD = Math.Round(chargedDifference * defaultToUSD, 2);
            decimal chargedUSD = Math.Round(charged * defaultToUSD, 2);

            // Store the USD and default values in the tag
            TagData purchaseData = new()
            {
                PricePerUnitUSD = pricePerUnitUSD,
                ShippingUSD = shippingUSD,
                TaxUSD = taxUSD,
                FeeUSD = feeUSD,
                DiscountUSD = discountUSD,
                ChargedDifferenceUSD = chargedDifferenceUSD,
                ChargedOrCreditedUSD = chargedUSD,
                OriginalCurrency = defaultCurrency,
                OriginalPricePerUnit = pricePerUnitDefault,
                OriginalShipping = shippingDefault,
                OriginalTax = taxDefault,
                OriginalFee = feeDefault,
                OriginalDiscount = discountDefault,
                OriginalChargedDifference = chargedDifference,
                OriginalChargedOrCredited = charged
            };

            // Save the receipt
            string newFilePath = "";
            if (!ReceiptManager.CheckIfReceiptExists(_receiptFilePath))
            {
                return false;
            }
            if (Controls.Contains(SelectedReceipt_Label))
            {
                (newFilePath, bool saved) = ReceiptManager.SaveReceiptInFile(_receiptFilePath);
                if (!saved)
                {
                    return false;
                }
            }

            // Add the row with the default values
            int newRowIndex = MainMenu_Form.Instance.SelectedDataGridView.Rows.Add(
                purchaseNumber,
                MainMenu_Form.SelectedAccountant,
                productName,
                categoryName,
                country,
                companyName,
                date,
                quantity.ToString(),
                pricePerUnitDefault.ToString("N2"),
                shippingDefault.ToString("N2"),
                taxDefault.ToString("N2"),
                feeDefault.ToString("N2"),
                discountDefault.ToString("N2"),
                chargedDifference.ToString("N2"),
                charged.ToString("N2"),
                noteLabel
            );

            if (noteLabel == ReadOnlyVariables.Show_text)
            {
                DataGridViewManager.AddNoteToCell(MainMenu_Form.Instance.SelectedDataGridView, newRowIndex, note);
            }

            // Set the tag
            if (newFilePath != "")
            {
                MainMenu_Form.Instance.SelectedDataGridView.Rows[newRowIndex].Tag = (newFilePath, purchaseData);
            }
            else
            {
                MainMenu_Form.Instance.SelectedDataGridView.Rows[newRowIndex].Tag = purchaseData;
            }

            DataGridViewManager.DataGridViewRowsAdded(MainMenu_Form.Instance.SelectedDataGridView, new DataGridViewRowsAddedEventArgs(newRowIndex, 1));

            string logMessage = $"Added purchase '{purchaseNumber}'";
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(ThingsThatHaveChangedInFile, 3, logMessage);

            return true;
        }
        private bool AddPurchaseWithMultipleItems()
        {
            bool isCategoryNameConsistent = true, isCountryConsistent = true, isCompanyConsistent = true;

            string purchaseNumber = OrderNumber_TextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(purchaseNumber))
            {
                purchaseNumber = ReadOnlyVariables.EmptyCell;
            }

            // Check if purchase ID already exists
            if (purchaseNumber != ReadOnlyVariables.EmptyCell
                && DataGridViewManager.DoesValueExistInDataGridView(MainMenu_Form.Instance.Purchase_DataGridView, ReadOnlyVariables.ID_column, purchaseNumber))
            {
                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                    "Purchase # already exists",
                    "The purchase #{0} already exists. Would you like to add this purchase anyways?",
                    CustomMessageBoxIcon.Question,
                    CustomMessageBoxButtons.YesNo,
                    purchaseNumber);

                if (result != CustomMessageBoxResult.Yes)
                {
                    return false;
                }
            }

            string date = Tools.FormatDate(Date_DateTimePicker.Value);
            decimal shipping = string.IsNullOrWhiteSpace(Shipping_TextBox.Text) ? 0 : decimal.Parse(Shipping_TextBox.Text);
            decimal tax = string.IsNullOrWhiteSpace(Tax_TextBox.Text) ? 0 : decimal.Parse(Tax_TextBox.Text);
            decimal fee = string.IsNullOrWhiteSpace(Fee_TextBox.Text) ? 0 : decimal.Parse(Fee_TextBox.Text);
            decimal discount = string.IsNullOrWhiteSpace(Discount_TextBox.Text) ? 0 : decimal.Parse(Discount_TextBox.Text);
            string noteLabel = ReadOnlyVariables.EmptyCell;
            string note = Notes_TextBox.Text.Trim();
            if (note != "")
            {
                noteLabel = ReadOnlyVariables.Show_text;
            }

            // Convert selected currency to default
            decimal exchangeRateToDefault = Currency.GetExchangeRate(Currency_TextBox.Text, DataFileManager.GetValue(AppDataSettings.DefaultCurrencyType), date);
            if (exchangeRateToDefault == -1) { return false; }

            List<string> items = [];

            string firstCategoryName = null, firstCountry = null, firstCompany = null;
            decimal totalPrice = 0;
            int totalQuantity = 0;

            // Convert selected currency to USD
            decimal exchangeRateToUSD = Currency.GetExchangeRate(Currency_TextBox.Text, "USD", date);
            if (exchangeRateToUSD == -1) { return false; }

            foreach (Guna2Panel panel in _panelsForMultipleProducts_List)
            {
                Guna2TextBox nameTextBox = (Guna2TextBox)panel.Controls.Find(TextBoxnames.name.ToString(), false).FirstOrDefault();
                string[] itemsInName = nameTextBox.Text.Split('>');
                string companyName = itemsInName[0].Trim();
                string categoryName = itemsInName[1].Trim();
                string productName = itemsInName[2].Trim();

                string currentCountry = MainMenu_Form.GetCountryProductIsFrom(MainMenu_Form.Instance.CategoryPurchaseList, productName);

                if (firstCategoryName == null)
                {
                    firstCategoryName = categoryName;
                }
                else if (isCategoryNameConsistent && firstCategoryName != categoryName)
                {
                    isCategoryNameConsistent = false;
                }

                if (firstCountry == null)
                {
                    firstCountry = currentCountry;
                }
                else if (isCountryConsistent && firstCountry != currentCountry)
                {
                    isCountryConsistent = false;
                }

                if (firstCompany == null)
                {
                    firstCompany = companyName;
                }
                else if (isCompanyConsistent && firstCompany != companyName)
                {
                    isCompanyConsistent = false;
                }

                Guna2TextBox quantityTextBox = (Guna2TextBox)panel.Controls.Find(TextBoxnames.quantity.ToString(), false).FirstOrDefault();
                int quantity = int.Parse(quantityTextBox.Text);
                Guna2TextBox pricePerUnitTextBox = (Guna2TextBox)panel.Controls.Find(TextBoxnames.pricePerUnit.ToString(), false).FirstOrDefault();
                decimal pricePerUnit = decimal.Parse(pricePerUnitTextBox.Text);
                decimal pricePerUnitUSD = pricePerUnit * exchangeRateToUSD;
                totalPrice += quantity * pricePerUnit;
                totalQuantity += quantity;

                string item = string.Join(",",
                    productName,
                    categoryName,
                    currentCountry,
                    companyName,
                    quantity.ToString(),
                    (pricePerUnit * exchangeRateToDefault).ToString("F2"),
                    pricePerUnitUSD.ToString("F2")
                );

                items.Add(item);
            }

            // Convert currency
            decimal shippingDefault = shipping * exchangeRateToDefault;
            decimal taxDefault = tax * exchangeRateToDefault;
            decimal feeDefault = fee * exchangeRateToDefault;
            decimal discountDefault = discount * exchangeRateToDefault;
            decimal totalPriceDefault = Math.Round(totalPrice * exchangeRateToDefault + shippingDefault + taxDefault + feeDefault - discountDefault, 2);
            decimal charged = Math.Round(decimal.Parse(Charged_TextBox.Text), 2);
            decimal chargedDifference = charged - totalPriceDefault;

            if (totalPriceDefault != charged)
            {
                string currency = DataFileManager.GetValue(AppDataSettings.DefaultCurrencyType);
                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                    "Amount charged is different",
                    "Amount charged ({0}{1} {2}) is not equal to the total price of the purchase ({3}{4} {5}). The difference will be accounted for.",
                    CustomMessageBoxIcon.Exclamation,
                    CustomMessageBoxButtons.OkCancel,
                    MainMenu_Form.CurrencySymbol, charged, currency, MainMenu_Form.CurrencySymbol, totalPriceDefault, currency);

                if (result != CustomMessageBoxResult.Ok)
                {
                    return false;
                }
            }

            string newFilePath = "";
            if (!ReceiptManager.CheckIfReceiptExists(_receiptFilePath))
            {
                return false;
            }
            if (Controls.Contains(SelectedReceipt_Label))
            {
                (newFilePath, bool saved) = ReceiptManager.SaveReceiptInFile(_receiptFilePath);
                if (!saved)
                {
                    return false;
                }
            }

            string finalCategoryName = isCategoryNameConsistent ? firstCategoryName : ReadOnlyVariables.EmptyCell;
            string finalCountry = isCountryConsistent ? firstCountry : ReadOnlyVariables.EmptyCell;
            string finalCompany = isCompanyConsistent ? firstCompany : ReadOnlyVariables.EmptyCell;

            int newRowIndex = MainMenu_Form.Instance.SelectedDataGridView.Rows.Add(
                purchaseNumber,
                MainMenu_Form.SelectedAccountant,
                ReadOnlyVariables.MultipleItems_text,
                finalCategoryName,
                finalCountry,
                finalCompany,
                date,
                totalQuantity.ToString(),
                ReadOnlyVariables.EmptyCell,
                shippingDefault.ToString("N2"),
                taxDefault.ToString("N2"),
                feeDefault.ToString("N2"),
                discountDefault.ToString("N2"),
                chargedDifference.ToString("N2"),
                charged.ToString("N2"),
                noteLabel
            );

            if (noteLabel == ReadOnlyVariables.Show_text)
            {
                DataGridViewManager.AddNoteToCell(MainMenu_Form.Instance.SelectedDataGridView, newRowIndex, note);
            }
            if (newFilePath != "")
            {
                items.Add(newFilePath);
            }

            // Calculate USD values
            decimal shippingUSD = Math.Round(shipping * exchangeRateToUSD, 2);
            decimal taxUSD = Math.Round(tax * exchangeRateToUSD, 2);
            decimal feeUSD = Math.Round(fee * exchangeRateToUSD, 2);
            decimal discountUSD = Math.Round(discount * exchangeRateToUSD, 2);

            string defaultCurrency = DataFileManager.GetValue(AppDataSettings.DefaultCurrencyType);

            // Convert default currency to USD
            decimal DefaultToUSD = Currency.GetExchangeRate(defaultCurrency, "USD", date);
            decimal chargedDifferenceUSD = Math.Round(chargedDifference * DefaultToUSD, 2);
            decimal chargedUSD = Math.Round(charged * DefaultToUSD, 2);

            // Store the USD and default values in the tag
            TagData tagData = new()
            {
                ShippingUSD = shippingUSD,
                TaxUSD = taxUSD,
                FeeUSD = feeUSD,
                DiscountUSD = discountUSD,
                ChargedDifferenceUSD = chargedDifferenceUSD,
                ChargedOrCreditedUSD = chargedUSD,
                OriginalCurrency = defaultCurrency,
                OriginalShipping = shippingDefault,
                OriginalTax = taxDefault,
                OriginalFee = feeDefault,
                OriginalDiscount = discountDefault,
                OriginalChargedDifference = chargedDifference,
                OriginalChargedOrCredited = charged
            };

            // Set the tag
            MainMenu_Form.Instance.SelectedDataGridView.Rows[newRowIndex].Tag = (items, tagData);

            DataGridViewManager.DataGridViewRowsAdded(MainMenu_Form.Instance.SelectedDataGridView, new DataGridViewRowsAddedEventArgs(newRowIndex, 1));

            string logMessage = $"Added purchase '{purchaseNumber}' with '{totalQuantity}' items";
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(ThingsThatHaveChangedInFile, 3, logMessage);

            return true;
        }

        // Receipts
        private string _receiptFilePath;
        private void ShowReceiptLabel(string text)
        {
            SelectedReceipt_Label.Text = text;

            Controls.Add(SelectedReceipt_Label);
            Controls.Add(RemoveReceipt_ImageButton);
            SetReceiptLabelLocation();

            ValidateInputs(null, null);
        }
        private void SetReceiptLabelLocation()
        {
            if (!Controls.Contains(SelectedReceipt_Label))
            {
                return;
            }

            RemoveReceipt_ImageButton.Location = new Point(Receipt_Button.Right - RemoveReceipt_ImageButton.Width, Receipt_Button.Bottom + CustomControls.SpaceBetweenControls);
            SelectedReceipt_Label.Location = new Point(
                RemoveReceipt_ImageButton.Left - SelectedReceipt_Label.Width,
                RemoveReceipt_ImageButton.Top + (RemoveReceipt_ImageButton.Height - SelectedReceipt_Label.Height) / 2 - 1);
        }
        private void RemoveReceiptLabel()
        {
            Controls.Remove(SelectedReceipt_Label);
            Controls.Remove(RemoveReceipt_ImageButton);
            ValidateInputs(null, null);
        }

        // Properties for multiple items
        private static int PanelAndTextBoxHeight => (int)(45 * DpiHelper.GetRelativeDpiScale());
        private static int LabelPanelHeight => (int)(30 * DpiHelper.GetRelativeDpiScale());
        private static int CircleButtonHeight => (int)(38 * DpiHelper.GetRelativeDpiScale());
        private static int SpaceOnSidesOfPanel => (int)(100 * DpiHelper.GetRelativeDpiScale());
        private static int FlowPanelMaxHeight => (int)(300 * DpiHelper.GetRelativeDpiScale());

        private int _topForPanels, _panelWidth;
        private readonly List<Guna2Panel> _panelsForMultipleProducts_List = [];
        private Guna2Panel _labelPanel;
        private CustomCircleButton _addButton;
        private FlowLayoutPanel _flowPanel;
        private enum TextBoxnames
        {
            name,
            quantity,
            pricePerUnit
        }

        // Methods for multiple items
        private List<Control> GetControlsForSingleProduct()
        {
            return [ProductName_TextBox, ProductName_Label, Quantity_TextBox, Quantity_Label, PricePerUnit_TextBox, PricePerUnit_Label];
        }
        private void SetControlsForSingleProduct()
        {
            byte space = CustomControls.SpaceBetweenControls;

            // Center controls
            Currency_TextBox.Left = (ClientSize.Width -
                Currency_TextBox.Width - space -
                OrderNumber_TextBox.Width - space -
                ProductName_TextBox.Width - space -
                Receipt_Button.Width) / 2;

            Currency_Label.Left = Currency_TextBox.Left;
            OrderNumber_TextBox.Left = Currency_TextBox.Right + space;
            OrderNumber_Label.Left = OrderNumber_TextBox.Left;
            ProductName_TextBox.Left = OrderNumber_TextBox.Right + space;
            ProductName_Label.Left = ProductName_TextBox.Left;
            Receipt_Button.Left = ProductName_TextBox.Right + space;

            Date_DateTimePicker.Left = (ClientSize.Width -
                Date_DateTimePicker.Width - space -
                Quantity_TextBox.Width - space -
                PricePerUnit_TextBox.Width - space -
                Shipping_TextBox.Width - space -
                Tax_TextBox.Width - space -
                Fee_TextBox.Width - space -
                Charged_TextBox.Width - space -
                Discount_TextBox.Width) / 2;

            Date_Label.Left = Date_DateTimePicker.Left;
            Quantity_TextBox.Left = Date_DateTimePicker.Right + space;
            Quantity_Label.Left = Quantity_TextBox.Left;
            PricePerUnit_TextBox.Left = Quantity_TextBox.Right + space;
            PricePerUnit_Label.Left = PricePerUnit_TextBox.Left;
            Shipping_TextBox.Left = PricePerUnit_TextBox.Right + space;
            Shipping_Label.Left = Shipping_TextBox.Left;
            Tax_TextBox.Left = Shipping_TextBox.Right + space;
            Tax_Label.Left = Tax_TextBox.Left;
            Fee_TextBox.Left = Tax_TextBox.Right + space;
            Fee_Label.Left = Fee_TextBox.Left;
            Discount_TextBox.Left = Fee_TextBox.Right + space;
            Discount_Label.Left = Discount_TextBox.Left;
            Charged_TextBox.Left = Discount_TextBox.Right + space;
            Charged_Label.Left = Charged_TextBox.Left;

            // Add controls
            foreach (Control control in GetControlsForSingleProduct())
            {
                Controls.Add(control);
            }

            _labelPanel.Visible = false;
            _flowPanel.Visible = false;
            _addButton.Visible = false;

            float scale = DpiHelper.GetRelativeDpiScale();
            MinimumSize = new Size(Width, Notes_TextBox.Bottom + (int)(140 * scale));
            Size = MinimumSize;

            SetReceiptLabelLocation();

            if (WarningProduct_PictureBox.Visible)
            {
                WarningProduct_PictureBox.Location = new Point(ProductName_TextBox.Left, ProductName_TextBox.Bottom + CustomControls.SpaceBetweenControls);
                WarningProduct_LinkLabel.Location = new Point(WarningProduct_PictureBox.Left + WarningProduct_PictureBox.Width + CustomControls.SpaceBetweenControls, WarningProduct_PictureBox.Top);
            }
        }
        private void SetControlsForMultipleProducts()
        {
            byte space = CustomControls.SpaceBetweenControls;

            // Center controls
            Currency_TextBox.Left = (ClientSize.Width -
                Currency_TextBox.Width - space -
                OrderNumber_TextBox.Width - space -
                Receipt_Button.Width) / 2;

            Currency_Label.Left = Currency_TextBox.Left;
            OrderNumber_TextBox.Left = Currency_TextBox.Right + space;
            OrderNumber_Label.Left = OrderNumber_TextBox.Left;
            Receipt_Button.Left = OrderNumber_TextBox.Right + space;

            Date_DateTimePicker.Left = (ClientSize.Width -
                Date_DateTimePicker.Width - space -
                Shipping_TextBox.Width - space -
                Tax_TextBox.Width - space -
                Fee_TextBox.Width - space -
                Charged_TextBox.Width - space -
                Discount_TextBox.Width) / 2;

            Date_Label.Left = Date_DateTimePicker.Left;
            Shipping_TextBox.Left = Date_DateTimePicker.Right + space;
            Shipping_Label.Left = Shipping_TextBox.Left;
            Tax_TextBox.Left = Shipping_TextBox.Right + space;
            Tax_Label.Left = Tax_TextBox.Left;
            Fee_TextBox.Left = Tax_TextBox.Right + space;
            Fee_Label.Left = Fee_TextBox.Left;
            Discount_TextBox.Left = Fee_TextBox.Right + space;
            Discount_Label.Left = Discount_TextBox.Left;
            Charged_TextBox.Left = Discount_TextBox.Right + space;
            Charged_Label.Left = Charged_TextBox.Left;

            // Remove controls
            foreach (Control control in GetControlsForSingleProduct())
            {
                Controls.Remove(control);
            }

            _labelPanel.Visible = true;
            _flowPanel.Visible = true;
            SetHeightAndAddButton();
            SetReceiptLabelLocation();

            if (WarningProduct_PictureBox.Visible)
            {
                WarningProduct_PictureBox.Location = new Point(_flowPanel.Left + SpaceOnSidesOfPanel / 2, _flowPanel.Bottom + CustomControls.SpaceBetweenControls);
                WarningProduct_LinkLabel.Location = new Point(WarningProduct_PictureBox.Left + WarningProduct_PictureBox.Width + CustomControls.SpaceBetweenControls, WarningProduct_PictureBox.Top);
                _addButton.Visible = false;
            }
            else
            {
                _addButton.Visible = true;
            }
        }
        private void ConstructControlsForMultipleProducts()
        {
            Guna2Panel panel = new()
            {
                Size = new Size(_panelWidth, PanelAndTextBoxHeight),
                FillColor = CustomColors.MainBackground
            };
            _panelsForMultipleProducts_List.Add(panel);

            if (_panelsForMultipleProducts_List.Count == 1)
            {
                _labelPanel = new()
                {
                    Location = new Point((ClientSize.Width - _panelWidth) / 2, _topForPanels),
                    Anchor = AnchorStyles.Top,
                    Size = new Size(_panelWidth, LabelPanelHeight),
                    FillColor = CustomColors.MainBackground,
                    Visible = false
                };
                Controls.Add(_labelPanel);
            }

            // Product name
            if (_labelPanel != null)
            {
                ConstructLabel(ProductName_Label.Text, 0, _labelPanel);
            }
            float scale = DpiHelper.GetRelativeDpiScale();
            Guna2TextBox textBox = ConstructTextBox(0, ProductName_TextBox.Width, TextBoxnames.name.ToString(), CustomControls.KeyPressValidation.None, panel);
            SearchBox.Attach(textBox, this, GetSearchResultsForProducts, (int)(150 * scale), true, false, true, true);

            // Price per unit
            int left = textBox.Right + CustomControls.SpaceBetweenControls;
            if (_labelPanel != null)
            {
                ConstructLabel(PricePerUnit_Label.Text, left, _labelPanel);
            }
            textBox = ConstructTextBox(left, PricePerUnit_TextBox.Width, TextBoxnames.pricePerUnit.ToString(), CustomControls.KeyPressValidation.OnlyNumbersAndDecimal, panel);

            // Quantity
            left = textBox.Right + CustomControls.SpaceBetweenControls;
            if (_labelPanel != null)
            {
                ConstructLabel(Quantity_Label.Text, left, _labelPanel);
            }
            textBox = ConstructTextBox(left, Quantity_TextBox.Width, TextBoxnames.quantity.ToString(), CustomControls.KeyPressValidation.OnlyNumbers, panel);

            // Add minus button unless this is the first panel
            left = textBox.Right + CustomControls.SpaceBetweenControls;
            if (_panelsForMultipleProducts_List.Count > 1)
            {
                ConstructMinusButton(new Point(left + CustomControls.SpaceBetweenControls, (PanelAndTextBoxHeight - CircleButtonHeight) / 2 + textBox.Top), panel);
            }

            SuspendLayout();
            _flowPanel.Controls.Add(panel);
            SetHeightAndAddButton();
            _flowPanel.ScrollControlIntoView(panel);
            ResumeLayout();
        }
        private static void ConstructLabel(string text, int left, Control parent)
        {
            Label label = new()
            {
                Text = text,
                Font = new Font("Segoe UI", 11),
                ForeColor = CustomColors.Text,
                Left = left,
                AutoSize = true
            };
            parent.Controls.Add(label);
        }
        private Guna2TextBox ConstructTextBox(int left, int width, string name, CustomControls.KeyPressValidation keyPressValidation, Control parent)
        {
            Guna2TextBox textBox = new()
            {
                Size = new Size(width, PanelAndTextBoxHeight),
                Name = name,
                Left = left,
                FillColor = CustomColors.ControlBack,
                BorderColor = CustomColors.ControlBorder,
                ForeColor = CustomColors.Text,
                ShortcutsEnabled = false,
                HoverState = { BorderColor = CustomColors.AccentBlue },
                FocusedState = {
                    BorderColor = CustomColors.AccentBlue,
                    FillColor = CustomColors.ControlBack
                }
            };

            // Assign the appropriate KeyPress event handler
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

            textBox.TextChanged += ValidateInputs;
            TextBoxManager.Attach(textBox);

            parent.Controls.Add(textBox);
            return textBox;
        }
        private void ConstructMinusButton(Point location, Control parent)
        {
            CustomCircleButton minusButton = new()
            {
                FillColor = CustomColors.MainBackground,
                Location = location,
                Size = new Size(CircleButtonHeight, CircleButtonHeight),
                PressedColor = CustomColors.ControlBack
            };

            if (ThemeManager.IsDarkTheme())
            {
                minusButton.ButtonImage = Resources.MinusWhite;
            }
            else
            {
                minusButton.ButtonImage = Resources.MinusBlack;
            }

            minusButton.Click += MinusButton_Click;
            parent.Controls.Add(minusButton);
        }
        private void MinusButton_Click(object? sender, EventArgs e)
        {
            CustomCircleButton button = (CustomCircleButton)sender;
            Guna2Panel panel = (Guna2Panel)button.Parent;

            _flowPanel.Controls.Remove(panel);
            _panelsForMultipleProducts_List.Remove(panel);

            SetHeightAndAddButton();
            ValidateInputs(null, null);
        }
        private void ConstructFlowPanel()
        {
            int width = _panelWidth + SpaceOnSidesOfPanel;
            _flowPanel = new()
            {
                Anchor = AnchorStyles.Top,
                AutoScroll = true,
                Location = new Point((ClientSize.Width - width) / 2, _topForPanels + LabelPanelHeight + CustomControls.SpaceBetweenControls),  // Use scaled property
                Size = new Size(width, PanelAndTextBoxHeight + CustomControls.SpaceBetweenControls),
                Padding = new Padding(SpaceOnSidesOfPanel / 2, 0, SpaceOnSidesOfPanel / 2, 0),
                Visible = false
            };
            ThemeManager.CustomizeScrollBar(_flowPanel);
            Controls.Add(_flowPanel);
        }
        private void ConstructAddButton()
        {
            _addButton = new CustomCircleButton()
            {
                FillColor = CustomColors.MainBackground,
                Location = new Point(0, 60),
                Size = new Size(CircleButtonHeight, CircleButtonHeight),
                Left = _flowPanel.Left + SpaceOnSidesOfPanel / 2,
                PressedColor = CustomColors.ControlBack,
                Visible = false,
                Anchor = AnchorStyles.Top
            };

            if (ThemeManager.IsDarkTheme())
            {
                _addButton.ButtonImage = Resources.AddWhite;
            }
            else
            {
                _addButton.ButtonImage = Resources.AddBlack;
            }

            _addButton.Click += (_, _) =>
            {
                ConstructControlsForMultipleProducts();
                ValidateInputs(null, null);
            };
            Controls.Add(_addButton);
        }
        private void SetHeightAndAddButton()
        {
            int totalHeight = _panelsForMultipleProducts_List.Sum(panel => panel.Height + panel.Margin.Top + panel.Margin.Bottom);

            _flowPanel.MinimumSize = new Size(_flowPanel.Width, Math.Min(totalHeight, FlowPanelMaxHeight));
            _flowPanel.Height = _flowPanel.MinimumSize.Height;
            _flowPanel.Top = _topForPanels + LabelPanelHeight + CustomControls.SpaceBetweenControls;
            _addButton.Top = _flowPanel.Bottom + CustomControls.SpaceBetweenControls;

            float scale = DpiHelper.GetRelativeDpiScale();
            MinimumSize = new Size(Width, _flowPanel.Bottom + (int)(180 * scale));
        }
        private void CalculatePanelDimensions()
        {
            float scale = DpiHelper.GetRelativeDpiScale();
            _topForPanels = Notes_TextBox.Bottom + (int)(20 * scale);

            // Calculate width based on the content that needs to fit in the panels
            byte space = CustomControls.SpaceBetweenControls;
            int contentWidth = ProductName_TextBox.Width + space +
                               PricePerUnit_TextBox.Width + space +
                               Quantity_TextBox.Width + space +
                               CircleButtonHeight + space;  // For the minus button

            // Set panel width based on content
            _panelWidth = contentWidth;
        }

        // Warning labels
        private void CheckIfProductsExist()
        {
            if (MainMenu_Form.Instance.GetProductPurchaseNames().Count == 0)
            {
                WarningProduct_PictureBox.Visible = true;
                WarningProduct_LinkLabel.Visible = true;
            }
            else
            {
                WarningProduct_PictureBox.Visible = false;
                WarningProduct_LinkLabel.Visible = false;
            }
        }

        // Misc.
        public void RecalculateMultipleItemsLayout()
        {
            LayoutUtils.RecalculateCheckboxLabelLayout(MultipleItems_CheckBox, MultipleItems_Label, this);
        }
        private void ValidateInputs(object sender, EventArgs e)
        {
            bool allFieldsFilled = !string.IsNullOrWhiteSpace(Charged_TextBox.Text);

            if (Properties.Settings.Default.PurchaseReceipts)
            {
                allFieldsFilled &= Controls.Contains(SelectedReceipt_Label);
            }

            bool allMultipleFieldsFilled = true;

            if (MultipleItems_CheckBox.Checked)
            {
                allMultipleFieldsFilled = _panelsForMultipleProducts_List
                    .SelectMany(panel => panel.Controls.OfType<Guna2TextBox>())
                    .All(textBox => !string.IsNullOrWhiteSpace(textBox.Text));
            }
            else
            {
                allFieldsFilled &= !string.IsNullOrWhiteSpace(ProductName_TextBox.Text) && ProductName_TextBox.Tag.ToString() != "0" &&
                    !string.IsNullOrWhiteSpace(Quantity_TextBox.Text) &&
                    !string.IsNullOrWhiteSpace(PricePerUnit_TextBox.Text);
            }
            AddPurchase_Button.Enabled = allFieldsFilled && allMultipleFieldsFilled;
        }
        private void ClosePanels()
        {
            TextBoxManager.HideRightClickPanel();
            SearchBox.Close();
        }
    }
}
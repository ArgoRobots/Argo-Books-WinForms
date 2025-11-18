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
    /// Form for adding new sale transactions.
    /// </summary>
    public partial class AddSale_Form : BaseForm
    {
        // Properties
        private static AddSale_Form _instance;

        // Getter
        public static AddSale_Form Instance => _instance;
        public static List<string> ThingsThatHaveChangedInFile { get; } = [];

        // Init.
        public AddSale_Form()
        {
            InitializeComponent();
            _instance = this;

            AddEventHandlersToTextBoxes();
            DpiHelper.ScaleImageButton(RemoveReceipt_ImageButton);
            Date_DateTimePicker.Value = DateTime.Now;
            Date_DateTimePicker.MaxDate = DateTime.Now;
            CheckIfProductsExist();
            ThemeManager.SetThemeForForm(this);
            SetAccessibleDescriptions();
            LanguageManager.UpdateLanguageForControl(this);
            RecalculateMultipleItemsLayout();
            RemoveReceiptLabel();
            string currency = DataFileManager.GetValue(AppDataSettings.DefaultCurrencyType);
            Credited_Label.Text = $"{MainMenu_Form.CurrencySymbol} credited ({currency})";

            PanelCloseFilter panelCloseFilter = new(this, ClosePanels, TextBoxManager.RightClickTextBox_Panel, SearchBox.SearchResultBoxContainer);
            Application.AddMessageFilter(panelCloseFilter);

            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void AddEventHandlersToTextBoxes()
        {
            byte searchBoxMaxHeight = 255;

            TextBoxManager.Attach(SaleNumber_TextBox);

            TextBoxManager.Attach(Customer_TextBox);
            SearchBox.Attach(Customer_TextBox, this, GetSearchResultsForCustomers, searchBoxMaxHeight, false, false, true, false);

            TextBoxManager.Attach(ProductName_TextBox);
            SearchBox.Attach(ProductName_TextBox, this, GetSearchResultsForProducts, searchBoxMaxHeight, true, false, true, true);
            ProductName_TextBox.TextChanged += ValidateInputs;

            TextBoxManager.Attach(CountryOfDestinaion_TextBox);
            SearchBox.Attach(CountryOfDestinaion_TextBox, this, () => Country.CountrySearchResults, searchBoxMaxHeight, false, true, true, false);
            CountryOfDestinaion_TextBox.TextChanged += ValidateInputs;

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

            TextBoxValidation.OnlyAllowNumbersAndOneDecimal(Credited_TextBox);
            TextBoxManager.Attach(Credited_TextBox);

            TextBoxManager.Attach(Notes_TextBox);
        }
        private List<SearchResult> GetSearchResultsForProducts()
        {
            return SearchBox.ConvertToSearchResults(MainMenu_Form.Instance.GetProductSaleNames());
        }
        private List<SearchResult> GetSearchResultsForCustomers()
        {
            return SearchBox.ConvertToSearchResults(
                MainMenu_Form.Instance.CustomerList.Select(c => c.FullName).ToList());
        }
        private void SetAccessibleDescriptions()
        {
            Label[] labelsToAlignLeftCenter =
            [
               MultipleItems_Label,
               SaleNumber_Label,
               Customer_Label,
               ProductName_Label,
               Date_Label,
               Quantity_Label,
               PricePerUnit_Label,
               Shipping_Label,
               Tax_Label,
               Discount_Label,
               Credited_Label,
               WarningProduct_LinkLabel
            ];

            foreach (Label label in labelsToAlignLeftCenter)
            {
                label.AccessibleDescription = AccessibleDescriptionManager.AlignLeft;
            }

            SelectedReceipt_Label.AccessibleDescription = AccessibleDescriptionManager.AlignRight;
        }

        // Form event handlers
        private void AddSale_Form_Activated(object sender, EventArgs e)
        {
            CheckIfProductsExist();
        }
        private void AddSale_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
        }
        private void AddSale_Form_Resize(object sender, EventArgs e)
        {
            ClosePanels();
        }
        private void AddSale_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClosePanels();
        }

        // Event handlers
        private void AddSale_Button_Click(object sender, EventArgs e)
        {
            if (MainMenu_Form.Instance.Selected != MainMenu_Form.SelectedOption.Sales)
            {
                MainMenu_Form.Instance.Sales_Button.PerformClick();
            }

            if (_panelsForMultipleProducts_List.Count == 0 || !MultipleItems_CheckBox.Checked)
            {
                if (!AddSale()) { return; }
            }
            // When the user selects "multiple items in this order" but only adds one, treat it as one
            else if (_panelsForMultipleProducts_List.Count == 1)
            {
                // Extract details from the single panel and populate the single sale fields
                Guna2Panel singlePanel = _panelsForMultipleProducts_List[0];
                ProductName_TextBox.Text = ((Guna2TextBox)singlePanel.Controls.Find(TextBoxnames.name.ToString(), false).FirstOrDefault()).Text;
                Quantity_TextBox.Text = ((Guna2TextBox)singlePanel.Controls.Find(TextBoxnames.quantity.ToString(), false).FirstOrDefault()).Text;
                PricePerUnit_TextBox.Text = ((Guna2TextBox)singlePanel.Controls.Find(TextBoxnames.pricePerUnit.ToString(), false).FirstOrDefault()).Text;

                if (!AddSale()) { return; }
            }
            else if (!AddSaleWithMultipleItems())
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
            Tools.OpenForm(new Products_Form(false));
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
        private void RemoveReceipt_ImageButton_MouseEnter(object sender, EventArgs e)
        {
            RemoveReceipt_ImageButton.BackColor = CustomColors.MouseHover;
        }
        private void RemoveReceipt_ImageButton_MouseLeave(object sender, EventArgs e)
        {
            RemoveReceipt_ImageButton.BackColor = CustomColors.MainBackground;
        }
        private void MultipleItems_Label_Click(object sender, EventArgs e)
        {
            MultipleItems_CheckBox.Checked = !MultipleItems_CheckBox.Checked;
        }

        // Methods to add sales
        private void ClearInputs()
        {
            RemoveReceiptLabel();
            SaleNumber_TextBox.Clear();
            Customer_TextBox.Clear();
            Quantity_TextBox.Clear();
            PricePerUnit_TextBox.Clear();
            Shipping_TextBox.Clear();
            Tax_TextBox.Clear();
            Fee_TextBox.Clear();
            Discount_TextBox.Clear();
            Credited_TextBox.Clear();
            Notes_TextBox.Clear();
        }
        private bool CheckAndPromptForNewCustomer()
        {
            string customerName = Customer_TextBox.Text.Trim();

            // If customer field is empty, allow the sale to proceed
            if (string.IsNullOrWhiteSpace(customerName))
            {
                return true;
            }

            // Check if customer exists in the customer list
            bool customerExists = MainMenu_Form.Instance.CustomerList.Any(c =>
                c.FullName.Equals(customerName, StringComparison.OrdinalIgnoreCase));

            if (!customerExists)
            {
                // Ask user if they want to add the new customer
                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                    "Customer not found",
                    "The customer '{0}' does not exist. Would you like to add this customer?",
                    CustomMessageBoxIcon.Question,
                    CustomMessageBoxButtons.YesNo,
                    customerName);

                if (result != CustomMessageBoxResult.Yes)
                {
                    return false;
                }

                // Parse customer name into first and last name
                string[] nameParts = customerName.Split([' '], 2, StringSplitOptions.RemoveEmptyEntries);
                string firstName = nameParts.Length > 0 ? nameParts[0] : customerName;
                string lastName = nameParts.Length > 1 ? nameParts[1] : "";

                // Create new customer with minimal information
                Customer newCustomer = new(
                    ReadOnlyVariables.EmptyCell,  // customerID
                    firstName,
                    lastName,
                    "",  // email
                    "",  // phoneNumber
                    "",  // address
                    ""); // notes

                // Add to customer list
                MainMenu_Form.Instance.CustomerList.Add(newCustomer);

                // If Customers_Form is open, add to DataGridView
                if (Tools.IsFormOpen<Customers_Form>())
                {
                    int newRowIndex = Customers_Form.Instance.Customers_DataGridView.Rows.Add(
                        newCustomer.CustomerID,
                        newCustomer.FirstName,
                        newCustomer.LastName,
                        newCustomer.Email,
                        newCustomer.PhoneNumber,
                        newCustomer.Address,
                        newCustomer.CurrentPaymentStatus.ToString(),
                        newCustomer.OutstandingBalance,
                        newCustomer.IsBanned,
                        newCustomer.RentalRecords.Count,
                        newCustomer.LastRentalDate?.ToString("yyyy-MM-dd") ?? "-");

                    Customers_Form.Instance.Customers_DataGridView.Rows[newRowIndex].Tag = newCustomer;
                    DataGridViewManager.DataGridViewRowsAdded(Customers_Form.Instance.Customers_DataGridView,
                        new DataGridViewRowsAddedEventArgs(newRowIndex, 1));
                }

                string logMessage = $"Added customer '{newCustomer.FullName}'";
                CustomMessage_Form.AddThingThatHasChangedAndLogMessage(Customers_Form.ThingsThatHaveChangedInFile, 4, logMessage);
            }

            return true;
        }
        private bool AddSale()
        {
            string saleNumber = SaleNumber_TextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(saleNumber))
            {
                saleNumber = ReadOnlyVariables.EmptyCell;
            }

            // Check if sale ID already exists
            if (saleNumber != ReadOnlyVariables.EmptyCell && DataGridViewManager.DoesValueExistInDataGridView(MainMenu_Form.Instance.Sale_DataGridView, ReadOnlyVariables.ID_column, saleNumber))
            {
                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                    "Sale # already exists",
                    "The sale #{0} already exists. Would you like to add this sale anyways?",
                    CustomMessageBoxIcon.Question,
                    CustomMessageBoxButtons.YesNo,
                    saleNumber);

                if (result != CustomMessageBoxResult.Yes)
                {
                    return false;
                }
            }

            // Check if customer exists, and prompt to add if not
            if (!CheckAndPromptForNewCustomer())
            {
                return false;
            }

            // Get values from TextBoxes
            string[] items = ProductName_TextBox.Text.Split('>');

            string companyName = items[0].Trim();
            string categoryName = items[1].Trim();
            string productName = items[2].Trim();

            string country = CountryOfDestinaion_TextBox.Text;
            string date = Tools.FormatDate(Date_DateTimePicker.Value);
            int quantity = int.Parse(Quantity_TextBox.Text);
            decimal pricePerUnit = decimal.Parse(PricePerUnit_TextBox.Text);
            decimal shipping = string.IsNullOrWhiteSpace(Shipping_TextBox.Text) ? 0 : decimal.Parse(Shipping_TextBox.Text);
            decimal tax = string.IsNullOrWhiteSpace(Tax_TextBox.Text) ? 0 : decimal.Parse(Tax_TextBox.Text);
            decimal fee = string.IsNullOrWhiteSpace(Fee_TextBox.Text) ? 0 : decimal.Parse(Fee_TextBox.Text);
            decimal discount = string.IsNullOrWhiteSpace(Discount_TextBox.Text) ? 0 : decimal.Parse(Discount_TextBox.Text);
            decimal totalPrice = Math.Round(quantity * pricePerUnit - discount, 2);
            string noteLabel = ReadOnlyVariables.EmptyCell;
            string note = Notes_TextBox.Text.Trim();
            if (note != "")
            {
                noteLabel = ReadOnlyVariables.Show_text;
            }

            decimal credited = Math.Round(decimal.Parse(Credited_TextBox.Text), 2);
            decimal creditedDifference = credited - totalPrice;

            if (creditedDifference != 0)
            {
                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                    "Amount credited is different",
                    "Amount credited ({0}{1}) is not equal to the total price of the sale (${2}). The difference will be accounted for.",
                    CustomMessageBoxIcon.Exclamation,
                    CustomMessageBoxButtons.OkCancel,
                    MainMenu_Form.CurrencySymbol, credited, totalPrice);

                if (result != CustomMessageBoxResult.Ok)
                {
                    return false;
                }
            }

            string defaultCurrency = DataFileManager.GetValue(AppDataSettings.DefaultCurrencyType);

            // Convert default currency to USD
            decimal defaultToUSD = Currency.GetExchangeRate(defaultCurrency, "USD", date);
            if (defaultToUSD == -1) { return false; }

            decimal pricePerUnitUSD = Math.Round(pricePerUnit * defaultToUSD, 2);
            decimal shippingUSD = Math.Round(shipping * defaultToUSD, 2);
            decimal taxUSD = Math.Round(tax * defaultToUSD, 2);
            decimal feeUSD = Math.Round(fee * defaultToUSD, 2);
            decimal discountUSD = Math.Round(discount * defaultToUSD, 2);
            decimal chargedDifferenceUSD = Math.Round(creditedDifference * defaultToUSD, 2);
            decimal creditedUSD = Math.Round(credited * defaultToUSD, 2);

            // Store the USD and default values in the tag
            TagData saleData = new()
            {
                PricePerUnitUSD = pricePerUnitUSD,
                ShippingUSD = shippingUSD,
                TaxUSD = taxUSD,
                FeeUSD = feeUSD,
                DiscountUSD = discountUSD,
                ChargedDifferenceUSD = chargedDifferenceUSD,
                ChargedOrCreditedUSD = creditedUSD,
                OriginalCurrency = defaultCurrency,
                OriginalPricePerUnit = pricePerUnit,
                OriginalShipping = shipping,
                OriginalTax = tax,
                OriginalFee = fee,
                OriginalDiscount = discount,
                OriginalChargedOrCredited = credited
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
                saleNumber,
                MainMenu_Form.SelectedAccountant,
                Customer_TextBox.Text.Trim(),
                productName,
                categoryName,
                country,
                companyName,
                date,
                quantity.ToString(),
                pricePerUnit.ToString("N"),
                shipping.ToString("N"),
                tax.ToString("N"),
                fee.ToString("N"),
                discount.ToString("N"),
                creditedDifference.ToString("N2"),
                credited.ToString("N"),
                noteLabel
            );

            if (noteLabel == ReadOnlyVariables.Show_text)
            {
                DataGridViewManager.AddNoteToCell(MainMenu_Form.Instance.SelectedDataGridView, newRowIndex, note);
            }

            // Set the tag
            if (newFilePath != "")
            {
                MainMenu_Form.Instance.SelectedDataGridView.Rows[newRowIndex].Tag = (newFilePath, saleData);
            }
            else
            {
                MainMenu_Form.Instance.SelectedDataGridView.Rows[newRowIndex].Tag = saleData;
            }

            DataGridViewManager.DataGridViewRowsAdded(MainMenu_Form.Instance.SelectedDataGridView, new DataGridViewRowsAddedEventArgs(newRowIndex, 1));

            string logMessage = $"Added Sale '{saleNumber}'";
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(ThingsThatHaveChangedInFile, 3, logMessage);

            return true;
        }
        private bool AddSaleWithMultipleItems()
        {
            bool isCategoryNameConsistent = true, isCountryConsistent = true, isCompanyConsistent = true;

            string saleNumber = SaleNumber_TextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(saleNumber))
            {
                saleNumber = ReadOnlyVariables.EmptyCell;
            }

            // Check if sale ID already exists
            if (saleNumber != ReadOnlyVariables.EmptyCell && DataGridViewManager.DoesValueExistInDataGridView(MainMenu_Form.Instance.Sale_DataGridView, ReadOnlyVariables.ID_column, saleNumber))
            {
                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                    "Sale # already exists",
                    "The sale #{0} already exists. Would you like to add this sale anyways?",
                    CustomMessageBoxIcon.Question,
                    CustomMessageBoxButtons.YesNo,
                    saleNumber);

                if (result != CustomMessageBoxResult.Yes)
                {
                    return false;
                }
            }

            // Check if customer exists, and prompt to add if not
            if (!CheckAndPromptForNewCustomer())
            {
                return false;
            }

            // Get values from TextBoxes
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

            List<string> items = [];

            string firstCategoryName = null, firstCountry = null, firstCompany = null;
            decimal totalPrice = 0;
            int totalQuantity = 0;

            string defaultCurrency = DataFileManager.GetValue(AppDataSettings.DefaultCurrencyType);

            // Convert default currency to USD
            decimal defaultToUSD = Currency.GetExchangeRate(defaultCurrency, "USD", date);
            if (defaultToUSD == -1) { return false; }

            foreach (Guna2Panel panel in _panelsForMultipleProducts_List)
            {
                Guna2TextBox nameTextBox = (Guna2TextBox)panel.Controls.Find(TextBoxnames.name.ToString(), false).FirstOrDefault();
                string[] itemsInName = nameTextBox.Text.Split('>');
                string companyName = itemsInName[0].Trim();
                string categoryName = itemsInName[1].Trim();
                string productName = itemsInName[2].Trim();

                string currentCountry = MainMenu_Form.GetCountryProductIsFrom(MainMenu_Form.Instance.CategorySaleList, productName);

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
                decimal pricePerUnitUSD = pricePerUnit * defaultToUSD;
                totalPrice += quantity * pricePerUnit;
                totalQuantity += quantity;

                string item = string.Join(",",
                    productName,
                    categoryName,
                    currentCountry,
                    companyName,
                    quantity.ToString(),
                    pricePerUnit.ToString("F2"),
                    pricePerUnitUSD.ToString("F2")
                );

                items.Add(item);
            }

            totalPrice = Math.Round(totalPrice - discount, 2);
            decimal credited = Math.Round(decimal.Parse(Credited_TextBox.Text), 2);
            decimal creditedDifference = credited - totalPrice;

            if (creditedDifference != 0)
            {
                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                    "Amount credited is different",
                    "Amount credited ({0}{1}) is not equal to the total price of the sale (${2}). The difference will be accounted for.",
                    CustomMessageBoxIcon.Exclamation,
                    CustomMessageBoxButtons.OkCancel,
                    MainMenu_Form.CurrencySymbol, credited, totalPrice);

                if (result != CustomMessageBoxResult.Ok)
                {
                    return false;
                }
            }
            totalPrice += creditedDifference;

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
                saleNumber,
                MainMenu_Form.SelectedAccountant,
                Customer_TextBox.Text.Trim(),
                ReadOnlyVariables.MultipleItems_text,
                finalCategoryName,
                finalCountry,
                finalCompany,
                date,
                totalQuantity.ToString(),
                ReadOnlyVariables.EmptyCell,
                shipping.ToString("N2"),
                tax.ToString("N2"),
                fee.ToString("N2"),
                discount.ToString("N2"),
                creditedDifference.ToString("N2"),
                totalPrice.ToString("N2"),
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
            decimal shippingUSD = Math.Round(shipping * defaultToUSD, 2);
            decimal taxUSD = Math.Round(tax * defaultToUSD, 2);
            decimal feeUSD = Math.Round(fee * defaultToUSD, 2);
            decimal discountUSD = Math.Round(discount * defaultToUSD, 2);
            decimal chargedDifferenceUSD = Math.Round(creditedDifference * defaultToUSD, 2);
            decimal creditedUSD = Math.Round(credited * defaultToUSD, 2);

            // Store the USD and default values in the tag
            TagData tagData = new()
            {
                ShippingUSD = shippingUSD,
                TaxUSD = taxUSD,
                FeeUSD = feeUSD,
                DiscountUSD = discountUSD,
                ChargedDifferenceUSD = chargedDifferenceUSD,
                ChargedOrCreditedUSD = creditedUSD,
                OriginalCurrency = defaultCurrency,
                OriginalShipping = shipping,
                OriginalTax = tax,
                OriginalFee = fee,
                OriginalDiscount = discount,
                OriginalChargedOrCredited = credited
            };

            // Set the tag
            MainMenu_Form.Instance.SelectedDataGridView.Rows[newRowIndex].Tag = (items, tagData);

            DataGridViewManager.DataGridViewRowsAdded(MainMenu_Form.Instance.SelectedDataGridView, new DataGridViewRowsAddedEventArgs(newRowIndex, 1));

            string logMessage = $"Added sale '{saleNumber}' with '{totalQuantity}' items";
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
            SaleNumber_TextBox.Left = (ClientSize.Width -
                SaleNumber_TextBox.Width - space -
                Customer_TextBox.Width - space -
                ProductName_TextBox.Width - space -
                CountryOfDestinaion_TextBox.Width - space -
                Receipt_Button.Width) / 2;

            SaleNumber_Label.Left = SaleNumber_TextBox.Left;
            Customer_TextBox.Left = SaleNumber_TextBox.Right + space;
            Customer_Label.Left = Customer_TextBox.Left;
            ProductName_TextBox.Left = Customer_TextBox.Right + space;
            ProductName_Label.Left = ProductName_TextBox.Left;
            CountryOfDestinaion_TextBox.Left = ProductName_TextBox.Right + space;
            CountryOfDestination_Label.Left = CountryOfDestinaion_TextBox.Left;
            Receipt_Button.Left = CountryOfDestinaion_TextBox.Right + space;

            Date_DateTimePicker.Left = (ClientSize.Width -
                Date_DateTimePicker.Width - space -
                Quantity_TextBox.Width - space -
                PricePerUnit_TextBox.Width - space -
                Shipping_TextBox.Width - space -
                Tax_TextBox.Width - space -
                Fee_TextBox.Width - space -
                Discount_TextBox.Width - space -
                Credited_TextBox.Width) / 2;

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
            Credited_TextBox.Left = Discount_TextBox.Right + space;
            Credited_Label.Left = Credited_TextBox.Left;

            // Add controls
            List<Control> controls = GetControlsForSingleProduct();
            foreach (Control control in controls)
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
            SaleNumber_TextBox.Left = (ClientSize.Width -
                SaleNumber_TextBox.Width - space -
                Customer_TextBox.Width - space -
                CountryOfDestinaion_TextBox.Width - space -
                Receipt_Button.Width) / 2;

            SaleNumber_Label.Left = SaleNumber_TextBox.Left;
            Customer_TextBox.Left = SaleNumber_TextBox.Right + space;
            Customer_Label.Left = Customer_TextBox.Left;
            CountryOfDestinaion_TextBox.Left = Customer_TextBox.Right + space;
            CountryOfDestination_Label.Left = CountryOfDestinaion_TextBox.Left;
            Receipt_Button.Left = CountryOfDestinaion_TextBox.Right + space;

            Date_DateTimePicker.Left = (ClientSize.Width -
                Date_DateTimePicker.Width - space -
                Shipping_TextBox.Width - space -
                Tax_TextBox.Width - space -
                Fee_TextBox.Width - space -
                Discount_TextBox.Width - space -
                Credited_TextBox.Width) / 2;

            Date_Label.Left = Date_DateTimePicker.Left;
            Shipping_TextBox.Left = Date_DateTimePicker.Right + space;
            Shipping_Label.Left = Shipping_TextBox.Left;
            Tax_TextBox.Left = Shipping_TextBox.Right + space;
            Tax_Label.Left = Tax_TextBox.Left;
            Fee_TextBox.Left = Tax_TextBox.Right + space;
            Fee_Label.Left = Fee_TextBox.Left;
            Discount_TextBox.Left = Fee_TextBox.Right + space;
            Discount_Label.Left = Discount_TextBox.Left;
            Credited_TextBox.Left = Discount_TextBox.Right + space;
            Credited_Label.Left = Credited_TextBox.Left;

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
                Location = new Point((ClientSize.Width - width) / 2, _topForPanels + LabelPanelHeight + CustomControls.SpaceBetweenControls),
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
                             CircleButtonHeight + space;  // For the minus button (now scaled)

            // Set panel width based on content
            _panelWidth = contentWidth;
        }

        // Warning labels
        private void CheckIfProductsExist()
        {
            if (MainMenu_Form.Instance.GetProductSaleNames().Count == 0)
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
            bool allFieldsFilled = !string.IsNullOrWhiteSpace(CountryOfDestinaion_TextBox.Text) && CountryOfDestinaion_TextBox.Tag.ToString() != "0" &&
                !string.IsNullOrWhiteSpace(Credited_TextBox.Text);

            if (Properties.Settings.Default.SaleReceipts)
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
            AddSale_Button.Enabled = allFieldsFilled && allMultipleFieldsFilled;
        }
        private void ClosePanels()
        {
            TextBoxManager.HideRightClickPanel();
            SearchBox.Close();
        }
    }
}
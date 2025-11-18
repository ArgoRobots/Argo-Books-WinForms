using Guna.UI2.WinForms;
using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.Language;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker.ReturnProduct
{
    /// <summary>
    /// Form for processing product return transactions.
    /// </summary>
    public partial class ReturnProduct_Form : BaseForm
    {
        // Properties
        private readonly DataGridViewRow _transactionRow;
        private readonly bool _isPurchase;
        private readonly bool _hasMultipleItems;
        private readonly List<string> _items;
        private List<Guna2CustomCheckBox> _itemCheckboxes;
        private Guna2Panel _itemsPanel;
        private Label _selectItemsLabel;

        // Init.
        public ReturnProduct_Form() : this(null, false) { }  // This is needed for TranslationGenerator.GenerateAllLanguageTranslationFiles()
        public ReturnProduct_Form(DataGridViewRow transactionRow, bool isPurchase)
        {
            InitializeComponent();
            if (transactionRow == null) { return; }

            _transactionRow = transactionRow;
            _isPurchase = isPurchase;

            // Check if this transaction has multiple items
            _hasMultipleItems = _transactionRow.Tag is (List<string>, TagData);

            if (_hasMultipleItems && _transactionRow.Tag is ValueTuple<List<string>, TagData> tuple)
            {
                _items = tuple.Item1;
            }
            else
            {
                _items = [];
            }

            DpiHelper.ScaleComboBox(ReturnReason_ComboBox);
            ReturnReason_ComboBox.Items.AddRange(ReturnReasons.GetReturnReasons().Cast<object>().ToArray());
            LoadTransactionData();
            if (_hasMultipleItems)
            {
                CreateItemSelectionControls();
            }
            UpdateTheme();
            SetAccessibleDescriptions();
            UpdateCharacterCount();
            LanguageManager.UpdateLanguageForControl(this);
            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void LoadTransactionData()
        {
            string transactionId = _transactionRow.Cells[ReadOnlyVariables.ID_column].Value.ToString();
            string productName = _transactionRow.Cells[ReadOnlyVariables.Product_column].Value.ToString();
            string categoryName = _transactionRow.Cells[ReadOnlyVariables.Category_column].Value.ToString();
            string date = _transactionRow.Cells[ReadOnlyVariables.Date_column].Value.ToString();
            string transactionType = _isPurchase ? LanguageManager.TranslateString("Purchase") : LanguageManager.TranslateString("Sale");

            // Check if there are already returned items
            bool hasReturnedItems = ReturnManager.IsTransactionPartiallyReturned(_transactionRow);

            if (hasReturnedItems)
            {
                TransactionDetails_Label.Text = $"{LanguageManager.TranslateString("Return Additional Items from")} {transactionType}:";
            }
            else
            {
                TransactionDetails_Label.Text = $"{transactionType} {LanguageManager.TranslateString("Transaction Details:")}";
            }

            if (_hasMultipleItems)
            {
                // For multiple items, show transaction overview
                int totalItems = _items.Count;

                // Exclude receipt from count if present
                if (_items.Count > 0 && _items[^1].StartsWith(ReadOnlyVariables.Receipt_text))
                {
                    totalItems--;
                }

                // Count already returned items
                int returnedCount = 0;
                for (int i = 0; i < totalItems; i++)
                {
                    if (IsItemReturned(i))
                    {
                        returnedCount++;
                    }
                }

                string returnInfo = hasReturnedItems ?
                    $"\n{LanguageManager.TranslateString("Already Returned:")}: {returnedCount} {LanguageManager.TranslateString("items")}" : "";

                TransactionInfo_Label.Text = $"{LanguageManager.TranslateString("Transaction ID")}: {transactionId}\n" +
                                             $"{LanguageManager.TranslateString("Multiple Items Transaction")}\n" +
                                             $"{LanguageManager.TranslateString("Total Items")}: {totalItems}{returnInfo}\n" +
                                             $"{LanguageManager.TranslateString("Date")}: {date}";
            }
            else
            {
                // For single item, show standard details
                TransactionInfo_Label.Text = $"{LanguageManager.TranslateString("Transaction ID")}: {transactionId}\n" +
                                             $"{LanguageManager.TranslateString("Product")}: {productName}\n" +
                                             $"{LanguageManager.TranslateString("Category")}: {categoryName}\n" +
                                             $"{LanguageManager.TranslateString("Date")}: {date}";
            }
        }
        private void CreateItemSelectionControls()
        {
            // Check if there are already returned items
            bool hasReturnedItems = ReturnManager.IsTransactionPartiallyReturned(_transactionRow);

            // Create label for item selection
            _selectItemsLabel = new Label
            {
                Text = hasReturnedItems ? LanguageManager.TranslateString("Select additional items to return:") : LanguageManager.TranslateString("Select items to return:"),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = CustomColors.Text,
                AutoSize = true,
                Location = new Point(TransactionDetails_Label.Left, TransactionInfo_Label.Bottom + 20)
            };
            Controls.Add(_selectItemsLabel);

            // Don't include receipt in the list
            int itemsToShow = _items.Count;
            if (_items.Count > 0 && _items[^1].StartsWith(ReadOnlyVariables.Receipt_text))
            {
                itemsToShow--;
            }

            const int checkBoxHeight = 20;
            const int minItemHeight = 35;
            const int itemPadding = 5;
            const int panelPadding = 20;
            const int maxPanelHeight = 300;

            // Create panel to hold item checkboxes
            _itemsPanel = new Guna2Panel
            {
                Location = new Point(TransactionInfo_Label.Left, _selectItemsLabel.Bottom + 10),
                FillColor = CustomColors.ControlBack,
                Width = Width - 60,
                AutoScroll = true
            };
            Controls.Add(_itemsPanel);

            // Create checkboxes for each item
            _itemCheckboxes = [];
            int yPosition = 10;
            int totalCalculatedHeight = panelPadding;

            for (int i = 0; i < itemsToShow; i++)
            {
                string[] itemDetails = _items[i].Split(',');
                if (itemDetails.Length < 6) { continue; }

                string productName = itemDetails[0];
                string categoryName = itemDetails[1];
                string companyName = itemDetails[3];
                int quantity = int.Parse(itemDetails[4]);
                decimal pricePerUnit = decimal.Parse(itemDetails[5]);

                // Check if this item is already returned
                bool isItemReturned = IsItemReturned(i);

                // Create custom checkbox
                Guna2CustomCheckBox itemCheckBox = new()
                {
                    Size = new Size(20, 20),
                    Location = new Point(10, yPosition),
                    Enabled = !isItemReturned,
                    Tag = i,  // Store the item index
                    Animated = true
                };

                // Create label for the checkbox
                Label itemLabel = new()
                {
                    Text = $"{productName} ({companyName}) - {LanguageManager.TranslateString("Quantity")}: {quantity} @ {MainMenu_Form.CurrencySymbol}{pricePerUnit:N2}",
                    MaximumSize = new Size(_itemsPanel.Width - 50, 0),
                    Font = new Font("Segoe UI", 10),
                    ForeColor = isItemReturned ? CustomColors.AccentRed : CustomColors.Text,
                    AutoSize = true,
                    AutoEllipsis = true,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                if (isItemReturned)
                {
                    itemLabel.Text += " (" + LanguageManager.TranslateString("Already returned") + ")";
                }

                // Calculate the actual height needed for this item
                int labelHeight = itemLabel.PreferredHeight;
                int actualItemHeight = Math.Max(minItemHeight, Math.Max(checkBoxHeight, labelHeight) + itemPadding);

                // Position label vertically centered with checkbox
                int labelYOffset = (actualItemHeight - labelHeight) / 2;
                itemLabel.Location = new Point(35, yPosition + labelYOffset);

                // Position checkbox vertically centered
                int checkBoxYOffset = (actualItemHeight - checkBoxHeight) / 2;
                itemCheckBox.Location = new Point(10, yPosition + checkBoxYOffset);

                // Handle label click to toggle checkbox
                itemLabel.Click += (s, e) =>
                {
                    if (itemCheckBox.Enabled)
                    {
                        itemCheckBox.Checked = !itemCheckBox.Checked;
                    }
                };

                itemCheckBox.CheckedChanged += ItemCheckBox_CheckedChanged;
                _itemCheckboxes.Add(itemCheckBox);
                _itemsPanel.Controls.Add(itemCheckBox);
                _itemsPanel.Controls.Add(itemLabel);

                yPosition += actualItemHeight;
                totalCalculatedHeight += actualItemHeight;
            }

            // Set the final panel height
            _itemsPanel.Height = Math.Min(totalCalculatedHeight, maxPanelHeight);

            // Adjust form height to accommodate new controls
            int additionalHeight = _itemsPanel.Bottom - TransactionInfo_Label.Bottom;
            Height += additionalHeight;
            MinimumSize = new Size(Width, Height);

            // Move existing controls down
            ReturnReason_Label.Top += additionalHeight;
            ReturnReason_ComboBox.Top += additionalHeight;
            AdditionalNotes_Label.Top += additionalHeight;
            AdditionalNotes_TextBox.Top += additionalHeight;
            CharacterCount_Label.Top += additionalHeight;
            ProcessReturn_Button.Top += additionalHeight;
            Cancel_Button.Top += additionalHeight;

            // Update validation since we now need at least one item selected
            ValidateInputs();
        }
        private bool IsItemReturned(int itemIndex)
        {
            return ReturnManager.IsItemReturned(_transactionRow, itemIndex);
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);

            ThemeManager.MakeGButtonBlueSecondary(Cancel_Button);
            ThemeManager.MakeGButtonBluePrimary(ProcessReturn_Button);
        }
        private void SetAccessibleDescriptions()
        {
            TransactionInfo_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
            ReturnReason_ComboBox.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
        }

        // Form event handlers
        private void ReturnProduct_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
        }

        // Event handlers
        private void ProcessReturn_Button_Click(object sender, EventArgs e)
        {
            string currentUser = MainMenu_Form.SelectedAccountant;

            if (_hasMultipleItems)
            {
                // Get selected items
                List<int> selectedItemIndices = [];
                foreach (Guna2CustomCheckBox checkBox in _itemCheckboxes)
                {
                    if (checkBox.Checked && checkBox.Tag is int index)
                    {
                        selectedItemIndices.Add(index);
                    }
                }

                if (selectedItemIndices.Count == 0)
                {
                    CustomMessageBox.Show("No items selected", "Please select at least one item to return.",
                        CustomMessageBoxIcon.Info, CustomMessageBoxButtons.Ok);
                    return;
                }

                // Process partial return for selected items
                ReturnManager.ProcessPartialReturn(_transactionRow,
                    selectedItemIndices,
                    ReturnReason_ComboBox.SelectedItem.ToString(),
                    AdditionalNotes_TextBox.Text.Trim(),
                    currentUser);
            }
            else
            {
                // Process full return for single item
                ReturnManager.ProcessReturn(_transactionRow,
                    ReturnReason_ComboBox.SelectedItem.ToString(),
                    AdditionalNotes_TextBox.Text.Trim(),
                    currentUser);
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        private void AdditionalNotes_TextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateInputs();
            UpdateCharacterCount();
        }
        private void ReturnReason_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateInputs();
        }
        private void ItemCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ValidateInputs();
        }

        // Methods
        private void ValidateInputs()
        {
            bool reasonSelected = ReturnReason_ComboBox.SelectedIndex != -1;
            bool itemsSelected = true;

            if (_hasMultipleItems)
            {
                // For multiple items, at least one item must be selected
                itemsSelected = _itemCheckboxes?.Any(cb => cb.Checked) ?? false;
            }

            ProcessReturn_Button.Enabled = reasonSelected && itemsSelected;
        }
        private void UpdateCharacterCount()
        {
            int currentLength = AdditionalNotes_TextBox.Text.Length;
            int maxLength = AdditionalNotes_TextBox.MaxLength;

            CharacterCount_Label.Text = $"{currentLength}/{maxLength}";

            // Change color to red when at max limit
            CharacterCount_Label.ForeColor = currentLength >= maxLength ? CustomColors.AccentRed : CustomColors.Text;
        }
    }
}
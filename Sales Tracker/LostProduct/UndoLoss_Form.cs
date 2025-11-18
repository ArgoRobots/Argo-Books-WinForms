using Guna.UI2.WinForms;
using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.Language;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker.LostProduct
{
    /// <summary>
    /// Form for undoing a product loss transaction.
    /// </summary>
    public partial class UndoLoss_Form : BaseForm
    {
        // Properties
        private readonly DataGridViewRow _transactionRow;
        private readonly bool _hasMultipleItems;
        private readonly List<string> _items;
        private List<Guna2CustomCheckBox> _itemCheckboxes;
        private Guna2Panel _itemsPanel;
        private Label _selectItemsLabel;

        // Init.
        public UndoLoss_Form() : this(null) { }  // This is needed for TranslationGenerator.GenerateAllLanguageTranslationFiles()
        public UndoLoss_Form(DataGridViewRow transactionRow)
        {
            InitializeComponent();
            if (transactionRow == null) { return; }

            _transactionRow = transactionRow;
            _hasMultipleItems = _transactionRow.Tag is (List<string>, TagData);

            if (_hasMultipleItems && _transactionRow.Tag is ValueTuple<List<string>, TagData> tuple)
            {
                _items = tuple.Item1;
            }
            else
            {
                _items = [];
            }

            LoadTransactionData();
            LoadLossInformation();

            UpdateTheme();
            SetAccessibleDescriptions();
            UpdateCharacterCount();
            ValidateInputs();
            LanguageManager.UpdateLanguageForControl(this);
            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void LoadTransactionData()
        {
            string transactionId = _transactionRow.Cells[ReadOnlyVariables.ID_column].Value.ToString();
            string productName = _transactionRow.Cells[ReadOnlyVariables.Product_column].Value.ToString();
            string categoryName = _transactionRow.Cells[ReadOnlyVariables.Category_column].Value.ToString();
            string date = _transactionRow.Cells[ReadOnlyVariables.Date_column].Value.ToString();

            if (_hasMultipleItems)
            {
                // For multiple items, show transaction overview
                int totalItems = _items.Count;

                // Exclude receipt from count if present
                if (_items.Count > 0 && _items[^1].StartsWith(ReadOnlyVariables.Receipt_text))
                {
                    totalItems--;
                }

                TransactionInfo_Label.Text = $"{LanguageManager.TranslateString("Transaction ID")}: {transactionId}\n" +
                                             $"{LanguageManager.TranslateString("Multiple Items Transaction")}\n" +
                                             $"{LanguageManager.TranslateString("Total Items")}: {totalItems}\n" +
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

            // Only create item selection for partial losses
            if (_hasMultipleItems)
            {
                CreateItemSelectionControls();
            }
        }
        private void LoadLossInformation()
        {
            LossInfo lossInfo = LostManager.GetLossInfo(_transactionRow);

            LossInfo_Label.Text = $"{LanguageManager.TranslateString("Loss Date")}: {lossInfo.FormattedDate}\n" +
                                  $"{LanguageManager.TranslateString("Reason")}: {lossInfo.DisplayReason}\n" +
                                  $"{LanguageManager.TranslateString("Marked by")}: {lossInfo.DisplayActionBy}";
        }
        private void CreateItemSelectionControls()
        {
            // Create label for item selection
            _selectItemsLabel = new Label
            {
                Text = LanguageManager.TranslateString("Select items to undo loss for:"),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = CustomColors.Text,
                AutoSize = true,
                Location = new Point(LossInformation_Label.Left, LossInfo_Label.Bottom + 20)
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
            const int maxPanelHeight = 200;

            // Create panel to hold item checkboxes
            _itemsPanel = new Guna2Panel
            {
                Location = new Point(LossInfo_Label.Left, _selectItemsLabel.Bottom + 10),
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
                string companyName = itemDetails[3];
                int quantity = int.Parse(itemDetails[4]);
                decimal pricePerUnit = decimal.Parse(itemDetails[5]);

                bool isItemLost = LostManager.IsItemLost(_transactionRow, i);

                // Create custom checkbox
                Guna2CustomCheckBox itemCheckBox = new()
                {
                    Size = new Size(20, 20),
                    Location = new Point(10, yPosition),
                    Enabled = isItemLost,  // Enable only lost items
                    Tag = i,  // Store the item index
                    Animated = true
                };

                // Create label for the checkbox
                Label itemLabel = new()
                {
                    Text = $"{productName} ({companyName}) - {LanguageManager.TranslateString("Quantity")}: {quantity} @ {MainMenu_Form.CurrencySymbol}{pricePerUnit:N2}",
                    MaximumSize = new Size(_itemsPanel.Width - 50, 0),
                    Font = new Font("Segoe UI", 10),
                    ForeColor = isItemLost ? CustomColors.Text : CustomColors.AccentGray,
                    AutoSize = true,
                    AutoEllipsis = true,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                if (!isItemLost)
                {
                    itemLabel.Text += " (" + LanguageManager.TranslateString("Not lost") + ")";
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

                // Handle label click to toggle checkbox (only if enabled)
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
            int additionalHeight = _itemsPanel.Bottom - LossInfo_Label.Bottom;
            Height += additionalHeight;
            MinimumSize = new Size(Width, Height);

            // Update validation since we now need at least one item selected for partial undos
            ValidateInputs();
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);
            ThemeManager.MakeGButtonBlueSecondary(Cancel_Button);
            ThemeManager.MakeGButtonBluePrimary(UndoLoss_Button);
        }
        private void SetAccessibleDescriptions()
        {
            TransactionInfo_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
            LossInfo_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
        }

        // Form event handlers
        private void UndoLoss_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
        }

        // Event handlers
        private void UndoLoss_Button_Click(object sender, EventArgs e)
        {
            string undoReason = UndoReason_TextBox.Text.Trim();

            if (_hasMultipleItems)
            {
                // Get selected items for partial undo
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
                    CustomMessageBox.Show("No items selected", "Please select at least one item to undo the loss for.",
                        CustomMessageBoxIcon.Info, CustomMessageBoxButtons.Ok);
                    return;
                }

                // Process partial undo loss for selected items
                LostManager.UndoPartialLoss(_transactionRow, selectedItemIndices, undoReason);
            }
            else
            {
                // Process full undo loss
                LostManager.UndoLoss(_transactionRow, undoReason);
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        private void UndoReason_TextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateInputs();
            UpdateCharacterCount();
        }
        private void ItemCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ValidateInputs();
        }

        // Methods
        private void ValidateInputs()
        {
            bool itemsSelected = true;

            if (_hasMultipleItems)
            {
                // For partial losses, at least one item must be selected
                itemsSelected = _itemCheckboxes?.Any(cb => cb.Checked) ?? false;
            }

            UndoLoss_Button.Enabled = itemsSelected;
        }
        private void UpdateCharacterCount()
        {
            int currentLength = UndoReason_TextBox.Text.Length;
            int maxLength = UndoReason_TextBox.MaxLength;

            CharacterCount_Label.Text = $"{currentLength}/{maxLength}";

            // Change color to red when at max limit
            CharacterCount_Label.ForeColor = currentLength >= maxLength ? CustomColors.AccentRed : CustomColors.Text;
        }
    }
}
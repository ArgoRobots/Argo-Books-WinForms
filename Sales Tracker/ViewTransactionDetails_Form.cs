using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.Language;
using Sales_Tracker.LostProduct;
using Sales_Tracker.ReturnProduct;
using Sales_Tracker.Theme;

namespace Sales_Tracker.UI
{
    /// <summary>
    /// Form for viewing detailed information about a transaction.
    /// </summary>
    public partial class ViewTransactionDetails_Form : BaseForm
    {
        // Enums
        public enum ViewType
        {
            Return,
            Loss
        }

        // Properties
        private readonly DataGridViewRow _transactionRow;
        private readonly ViewType _viewType;
        private readonly bool _hasMultipleItems;
        private readonly List<string> _items;

        // Constants for sizing
        private const int BaseFormWidth = 600;
        private const int MinimumFormHeight = 350;
        private const int MaximumFormHeight = 800;
        private const int FormPadding = 40;

        // Init.
        public ViewTransactionDetails_Form() : this(null, ViewType.Return) { }  // This is needed for TranslationGenerator.GenerateAllLanguageTranslationFiles()
        public ViewTransactionDetails_Form(DataGridViewRow transactionRow, ViewType viewType)
        {
            InitializeComponent();
            if (transactionRow == null) { return; }

            _transactionRow = transactionRow;
            _viewType = viewType;

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

            LoadTransactionData();
            LoadDetailsInformation();
            LoadAffectedItemsInformation();

            PositionLabels();
            SetFormSize();

            UpdateTheme();
            SetAccessibleDescriptions();
            LanguageManager.UpdateLanguageForControl(this);
            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void LoadTransactionData()
        {
            string transactionId = _transactionRow.Cells[ReadOnlyVariables.ID_column].Value.ToString();
            string productName = _transactionRow.Cells[ReadOnlyVariables.Product_column].Value.ToString();
            string categoryName = _transactionRow.Cells[ReadOnlyVariables.Category_column].Value.ToString();
            string date = _transactionRow.Cells[ReadOnlyVariables.Date_column].Value.ToString();

            // Update form title and header based on view type
            string detailsType = _viewType == ViewType.Return ?
                LanguageManager.TranslateString("Return") :
                LanguageManager.TranslateString("Loss");

            Text = $"{detailsType} {LanguageManager.TranslateString("Details")}";
            DetailsHeader_Label.Text = $"{detailsType} {LanguageManager.TranslateString("Details")}:";

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
        }
        private void LoadDetailsInformation()
        {
            if (_viewType == ViewType.Return)
            {
                LoadReturnInformation();
            }
            else
            {
                LoadLossInformation();
            }
        }
        private void LoadReturnInformation()
        {
            ReturnInfo returnInfo = ReturnManager.GetReturnInfo(_transactionRow);

            if (_hasMultipleItems && returnInfo.HasAffectedItems)
            {
                DetailsInfo_Label.Text = $"{LanguageManager.TranslateString("Returned on")}: {returnInfo.FormattedDate}\n" +
                                         $"{LanguageManager.TranslateString("Reason")}: {returnInfo.DisplayReason}\n" +
                                         $"{LanguageManager.TranslateString("Returned by")}: {returnInfo.DisplayActionBy}";
            }
            else
            {
                // Single item or full return
                DetailsInfo_Label.Text = $"{LanguageManager.TranslateString("Returned on")}: {returnInfo.FormattedDate}\n" +
                                         $"{LanguageManager.TranslateString("Reason")}: {returnInfo.DisplayReason}\n" +
                                         $"{LanguageManager.TranslateString("Returned by")}: {returnInfo.DisplayActionBy}";
            }
        }
        private void LoadLossInformation()
        {
            LossInfo lossInfo = LostManager.GetLossInfo(_transactionRow);

            if (_hasMultipleItems && lossInfo.HasAffectedItems)
            {
                bool isPartiallyLost = LostManager.IsTransactionPartiallyLost(_transactionRow);
                string lossType = isPartiallyLost ? LanguageManager.TranslateString("Partial Loss") : LanguageManager.TranslateString("Full Loss");

                DetailsInfo_Label.Text = $"{LanguageManager.TranslateString("Loss Type")}: {lossType}\n" +
                                         $"{LanguageManager.TranslateString("Loss Date")}: {lossInfo.FormattedDate}\n" +
                                         $"{LanguageManager.TranslateString("Reason")}: {lossInfo.DisplayReason}\n" +
                                         $"{LanguageManager.TranslateString("Marked by")}: {lossInfo.DisplayActionBy}";
            }
            else
            {
                DetailsInfo_Label.Text = $"{LanguageManager.TranslateString("Loss Date")}: {lossInfo.FormattedDate}\n" +
                                         $"{LanguageManager.TranslateString("Reason")}: {lossInfo.DisplayReason}\n" +
                                         $"{LanguageManager.TranslateString("Marked by")}: {lossInfo.DisplayActionBy}";
            }
        }
        private void LoadAffectedItemsInformation()
        {
            if (!_hasMultipleItems)
            {
                // Single item - no need to show affected items section
                ItemsHeader_Label.Visible = false;
                ItemsInfo_Label.Visible = false;
                return;
            }

            List<int> affectedIndices;

            if (_viewType == ViewType.Return)
            {
                ReturnInfo returnInfo = ReturnManager.GetReturnInfo(_transactionRow);
                affectedIndices = returnInfo.ReturnedItems;
                ItemsHeader_Label.Text = LanguageManager.TranslateString("Returned Items") + ":";
            }
            else
            {
                LossInfo lossInfo = LostManager.GetLossInfo(_transactionRow);
                affectedIndices = lossInfo.LostItems;
                ItemsHeader_Label.Text = LanguageManager.TranslateString("Lost Items") + ":";
            }

            if (affectedIndices.Count > 0)
            {
                ItemsHeader_Label.Visible = true;
                ItemsInfo_Label.Visible = true;

                // Create a detailed list with quantities and prices
                List<string> detailedItems = [];

                foreach (int index in affectedIndices)
                {
                    if (index < _items.Count && !_items[index].StartsWith(ReadOnlyVariables.Receipt_text))
                    {
                        string[] itemDetails = _items[index].Split(',');
                        if (itemDetails.Length >= 6)
                        {
                            string productName = itemDetails[0];
                            string companyName = itemDetails[3];
                            int quantity = int.Parse(itemDetails[4]);
                            decimal pricePerUnit = decimal.Parse(itemDetails[5]);

                            detailedItems.Add($"• {productName} ({companyName}) - {LanguageManager.TranslateString("Quantity")}: {quantity} @ {MainMenu_Form.CurrencySymbol}{pricePerUnit:N2}");
                        }
                    }
                }

                if (detailedItems.Count > 0)
                {
                    ItemsInfo_Label.Text = string.Join("\n", detailedItems);
                }
            }
            else
            {
                ItemsHeader_Label.Visible = false;
                ItemsInfo_Label.Visible = false;
            }
        }
        private void SetFormSize()
        {
            int requiredHeight = CalculateRequiredHeight();

            // Ensure height is within acceptable bounds
            int formHeight = Math.Max(MinimumFormHeight, Math.Min(requiredHeight, MaximumFormHeight)) + 50;  // Space for button

            // Set form size
            Size = new Size(BaseFormWidth, formHeight);
            MinimumSize = new Size(BaseFormWidth, Math.Max(MinimumFormHeight, formHeight));

            // Position the close button at the bottom
            Close_Button.Location = new Point(
                ClientSize.Width - Close_Button.Width - FormPadding + 17,
                ClientSize.Height - Close_Button.Height - 20
            );
        }
        private void PositionLabels()
        {
            int currentY = 20;  // Starting position

            // Transaction Details section
            TransactionDetails_Label.Top = currentY;
            currentY += TransactionDetails_Label.Height + 5;

            TransactionInfo_Label.Top = currentY;
            currentY += TransactionInfo_Label.Height + 15;

            // Return/Loss Details section
            DetailsHeader_Label.Top = currentY;
            currentY += DetailsHeader_Label.Height + 5;

            DetailsInfo_Label.Top = currentY;
            currentY += DetailsInfo_Label.Height + 15;

            // Affected Items section (if visible)
            if (_hasMultipleItems)
            {
                ItemsHeader_Label.Top = currentY;
                currentY += ItemsHeader_Label.Height + 5;

                ItemsInfo_Label.Top = currentY;
            }
        }
        private int CalculateRequiredHeight()
        {
            int totalHeight = FormPadding;

            // Transaction details section
            totalHeight += TransactionDetails_Label.Height + 10;
            totalHeight += GetTextHeight(TransactionInfo_Label.Text, TransactionInfo_Label.Font, TransactionInfo_Label.MaximumSize.Width) + 20;

            // Return/Loss details section  
            totalHeight += DetailsHeader_Label.Height + 10;
            totalHeight += GetTextHeight(DetailsInfo_Label.Text, DetailsInfo_Label.Font, DetailsInfo_Label.MaximumSize.Width) + 20;

            // Affected items section (if visible)
            if (_hasMultipleItems)
            {
                totalHeight += ItemsHeader_Label.Height + 10;
                totalHeight += GetTextHeight(ItemsInfo_Label.Text, ItemsInfo_Label.Font, ItemsInfo_Label.MaximumSize.Width) + 20;
            }

            // Close button and bottom padding
            totalHeight += Close_Button.Height + 40;

            return totalHeight;
        }
        private int GetTextHeight(string text, Font font, int maxWidth)
        {
            if (string.IsNullOrEmpty(text)) { return 20; }  // Default height for empty text

            using Graphics g = CreateGraphics();
            SizeF textSize = g.MeasureString(text, font, maxWidth);
            return (int)Math.Ceiling(textSize.Height);
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);
            ThemeManager.MakeGButtonBluePrimary(Close_Button);
        }
        private void SetAccessibleDescriptions()
        {
            TransactionInfo_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
            DetailsInfo_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
            ItemsInfo_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
        }

        // Form event handlers
        private void ViewTransactionDetails_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
        }

        // Event handlers
        private void Close_Button_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
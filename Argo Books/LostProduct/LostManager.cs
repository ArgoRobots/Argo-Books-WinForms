using Argo_Books.DataClasses;
using Argo_Books.Theme;
using Argo_Books;

namespace Argo_Books.LostProduct
{
    /// <summary>
    /// Manages product loss operations for transactions including processing, undoing, and tracking loss status.
    /// </summary>
    public static class LostManager
    {
        /// <summary>
        /// Processes a product loss for the specified transaction row.
        /// </summary>
        public static void ProcessLoss(DataGridViewRow row, string reason, string additionalNotes, string lostBy)
        {
            if (row.Tag is TagData tagData)
            {
                // Single item transaction
                tagData.IsLost = true;
                tagData.LostDate = DateTime.Now;
                tagData.LostReason = string.IsNullOrEmpty(additionalNotes) ? reason : $"{reason} - {additionalNotes}";
                tagData.LostBy = lostBy;
                tagData.LostItems = null;  // Full loss, no need for item tracking
            }
            else if (row.Tag is (List<string>, TagData purchaseData))
            {
                // Multiple items transaction - mark all items as lost
                purchaseData.IsLost = true;
                purchaseData.LostDate = DateTime.Now;
                purchaseData.LostReason = string.IsNullOrEmpty(additionalNotes) ? reason : $"{reason} - {additionalNotes}";
                purchaseData.LostBy = lostBy;
                purchaseData.LostItems = null;  // Full loss, no need for item tracking
            }
            else if (row.Tag is (string, TagData receiptData))
            {
                // Single item with receipt
                receiptData.IsLost = true;
                receiptData.LostDate = DateTime.Now;
                receiptData.LostReason = string.IsNullOrEmpty(additionalNotes) ? reason : $"{reason} - {additionalNotes}";
                receiptData.LostBy = lostBy;
                receiptData.LostItems = null;  // Full loss, no need for item tracking
            }

            UpdateRowAppearanceForLoss(row, true);

            // Log the loss
            string transactionId = row.Cells[ReadOnlyVariables.ID_column].Value?.ToString() ?? "Unknown";
            string productName = row.Cells[ReadOnlyVariables.Product_column].Value?.ToString() ?? "Unknown";
            string logMessage = $"Marked {(MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.Purchases
                ? "purchase" : "sale")} '{transactionId}' - {productName} as lost. Reason: {reason}";

            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 2, logMessage);

            SaveLossChanges();
            MainMenu_Form.Instance.RefreshDataGridViewAndCharts();
            MainMenu_Form.Instance.UpdateTotalLabels();
        }

        /// <summary>
        /// Processes a partial loss for specific items in a multi-item transaction.
        /// </summary>
        public static void ProcessPartialLoss(DataGridViewRow row, List<int> itemIndices, string reason, string additionalNotes, string lostBy)
        {
            if (row.Tag is not (List<string> items, TagData tagData))
            {
                // Not a multi-item transaction, fall back to regular loss
                ProcessLoss(row, reason, additionalNotes, lostBy);
                return;
            }

            // Initialize lost items list if it doesn't exist
            tagData.LostItems ??= [];

            // Add new lost items (avoid duplicates)
            foreach (int itemIndex in itemIndices)
            {
                if (!tagData.LostItems.Contains(itemIndex))
                {
                    tagData.LostItems.Add(itemIndex);
                }
            }

            // Determine total loseable items (excluding receipt if present)
            int totalLoseableItems = items.Count;
            if (items.Count > 0 && items[^1].StartsWith(ReadOnlyVariables.Receipt_text))
            {
                totalLoseableItems--;
            }

            // Check if all items are now lost
            bool allItemsLost = tagData.LostItems.Count >= totalLoseableItems;

            // Update loss status
            tagData.IsLost = allItemsLost;
            tagData.IsPartiallyLost = !allItemsLost && tagData.LostItems.Count > 0;

            // Update loss details
            if (tagData.LostDate == null)
            {
                tagData.LostDate = DateTime.Now;
                tagData.LostBy = lostBy;
            }

            // Append to existing loss reason if there was a previous partial loss
            string newReason = string.IsNullOrEmpty(additionalNotes) ? reason : $"{reason} - {additionalNotes}";
            if (string.IsNullOrEmpty(tagData.LostReason))
            {
                tagData.LostReason = newReason;
            }
            else
            {
                tagData.LostReason += $"; {newReason}";
            }

            UpdateRowAppearanceForLoss(row, tagData.IsLost, tagData.IsPartiallyLost);

            // Log the partial loss
            string transactionId = row.Cells[ReadOnlyVariables.ID_column].Value?.ToString() ?? "Unknown";
            List<string> lostItemNames = [];

            foreach (int index in itemIndices)
            {
                if (index < items.Count && !items[index].StartsWith(ReadOnlyVariables.Receipt_text))
                {
                    string[] itemDetails = items[index].Split(',');
                    if (itemDetails.Length > 0)
                    {
                        lostItemNames.Add(itemDetails[0]);  // Product name
                    }
                }
            }

            string itemsText = string.Join(", ", lostItemNames);
            string lossType = allItemsLost ? "Completed loss" : "Partial loss";
            string logMessage = $"{lossType} for {(MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.Purchases
                ? "purchase" : "sale")} '{transactionId}' - Items: {itemsText}. Reason: {reason}";

            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 2, logMessage);

            SaveLossChanges();
            MainMenu_Form.Instance.RefreshDataGridViewAndCharts();
            MainMenu_Form.Instance.UpdateTotalLabels();
        }

        /// <summary>
        /// Undoes a product loss for the specified transaction row.
        /// </summary>
        public static void UndoLoss(DataGridViewRow row, string undoReason)
        {
            if (row.Tag is TagData tagData)
            {
                tagData.IsLost = false;
                tagData.IsPartiallyLost = false;
                tagData.LostDate = null;
                tagData.LostReason = null;
                tagData.LostBy = null;
                tagData.LostItems = null;
            }
            else if (row.Tag is (List<string>, TagData purchaseData))
            {
                purchaseData.IsLost = false;
                purchaseData.IsPartiallyLost = false;
                purchaseData.LostDate = null;
                purchaseData.LostReason = null;
                purchaseData.LostBy = null;
                purchaseData.LostItems = null;
            }
            else if (row.Tag is (string, TagData receiptData))
            {
                receiptData.IsLost = false;
                receiptData.IsPartiallyLost = false;
                receiptData.LostDate = null;
                receiptData.LostReason = null;
                receiptData.LostBy = null;
                receiptData.LostItems = null;
            }

            UpdateRowAppearanceForLoss(row, false);

            // Log the undo
            string transactionId = row.Cells[ReadOnlyVariables.ID_column].Value?.ToString() ?? "Unknown";
            string productName = row.Cells[ReadOnlyVariables.Product_column].Value?.ToString() ?? "Unknown";
            string logMessage = $"Undid loss for {(MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.Purchases
                ? "purchase" : "sale")} '{transactionId}' - {productName}. Reason: {undoReason}";

            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 2, logMessage);

            SaveLossChanges();
            MainMenu_Form.Instance.RefreshDataGridViewAndCharts();
            MainMenu_Form.Instance.UpdateTotalLabels();
        }

        /// <summary>
        /// Undoes a partial loss for specific items in a multi-item transaction.
        /// </summary>
        public static void UndoPartialLoss(DataGridViewRow row, List<int> itemIndices, string undoReason)
        {
            if (row.Tag is not (List<string> items, TagData tagData) || tagData.LostItems == null)
            {
                return;
            }

            // Remove specified items from lost items list
            foreach (int itemIndex in itemIndices)
            {
                tagData.LostItems.Remove(itemIndex);
            }

            // Update loss status
            bool hasLostItems = tagData.LostItems.Count > 0;
            tagData.IsPartiallyLost = hasLostItems;
            tagData.IsLost = false;  // If we're undoing items, it's no longer fully lost

            // If no items are lost anymore, clear all loss data
            if (!hasLostItems)
            {
                tagData.LostDate = null;
                tagData.LostReason = null;
                tagData.LostBy = null;
                tagData.LostItems = null;
            }

            UpdateRowAppearanceForLoss(row, tagData.IsLost, tagData.IsPartiallyLost);

            // Log the undo
            string transactionId = row.Cells[ReadOnlyVariables.ID_column].Value?.ToString() ?? "Unknown";
            List<string> undoneItemNames = [];

            foreach (int index in itemIndices)
            {
                if (index < items.Count && !items[index].StartsWith(ReadOnlyVariables.Receipt_text))
                {
                    string[] itemDetails = items[index].Split(',');
                    if (itemDetails.Length > 0)
                    {
                        undoneItemNames.Add(itemDetails[0]);  // Product name
                    }
                }
            }

            string itemsText = string.Join(", ", undoneItemNames);
            string logMessage = $"Undid partial loss for {(MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.Purchases
                ? "purchase" : "sale")} '{transactionId}' - Items: {itemsText}. Reason: {undoReason}";

            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 2, logMessage);

            SaveLossChanges();
            MainMenu_Form.Instance.RefreshDataGridViewAndCharts();
            MainMenu_Form.Instance.UpdateTotalLabels();
        }

        /// <summary>
        /// Checks if a transaction is lost (fully or partially).
        /// </summary>
        public static bool IsTransactionLost(DataGridViewRow row)
        {
            if (row.Tag is TagData tagData)
            {
                return tagData.IsLost || tagData.IsPartiallyLost;
            }
            else if (row.Tag is (List<string>, TagData purchaseData))
            {
                return purchaseData.IsLost || purchaseData.IsPartiallyLost;
            }
            else if (row.Tag is (string, TagData receiptData))
            {
                return receiptData.IsLost || receiptData.IsPartiallyLost;
            }
            return false;
        }

        /// <summary>
        /// Checks if a transaction is fully lost.
        /// </summary>
        public static bool IsTransactionFullyLost(DataGridViewRow row)
        {
            if (row.Tag is TagData tagData)
            {
                return tagData.IsLost;
            }
            else if (row.Tag is (List<string>, TagData purchaseData))
            {
                return purchaseData.IsLost;
            }
            else if (row.Tag is (string, TagData receiptData))
            {
                return receiptData.IsLost;
            }
            return false;
        }

        /// <summary>
        /// Checks if a transaction is partially lost.
        /// </summary>
        public static bool IsTransactionPartiallyLost(DataGridViewRow row)
        {
            if (row.Tag is TagData tagData)
            {
                return tagData.IsPartiallyLost;
            }
            else if (row.Tag is (List<string>, TagData purchaseData))
            {
                return purchaseData.IsPartiallyLost;
            }
            else if (row.Tag is (string, TagData receiptData))
            {
                return receiptData.IsPartiallyLost;
            }
            return false;
        }

        /// <summary>
        /// Checks if a specific item within a transaction is lost.
        /// </summary>
        public static bool IsItemLost(DataGridViewRow row, int itemIndex)
        {
            if (row.Tag is TagData tagData)
            {
                // Single item transaction
                return tagData.IsLost;
            }
            else if (row.Tag is (List<string>, TagData purchaseData))
            {
                // Multi-item transaction
                if (purchaseData.IsLost)
                {
                    return true;  // All items lost
                }
                return purchaseData.LostItems?.Contains(itemIndex) ?? false;
            }
            else if (row.Tag is (string, TagData receiptData))
            {
                // Single item with receipt
                return receiptData.IsLost;
            }
            return false;
        }

        /// <summary>
        /// Gets loss information for a transaction.
        /// </summary>
        public static LossInfo GetLossInfo(DataGridViewRow row)
        {
            TagData tagData = null;

            if (row.Tag is TagData data)
            {
                tagData = data;
            }
            else if (row.Tag is (List<string>, TagData purchaseData))
            {
                tagData = purchaseData;
            }
            else if (row.Tag is (string, TagData receiptData))
            {
                tagData = receiptData;
            }

            if (tagData != null)
            {
                return new LossInfo
                {
                    LostDate = tagData.LostDate,
                    LostReason = tagData.LostReason ?? "",
                    LostBy = tagData.LostBy ?? "",
                    LostItems = tagData.LostItems ?? []
                };
            }

            // Return empty LossInfo for invalid data
            return new LossInfo();
        }

        /// <summary>
        /// Updates the visual appearance of a row based on its loss status.
        /// </summary>
        public static void UpdateRowAppearanceForLoss(DataGridViewRow row, bool isLost, bool isPartiallyLost = false)
        {
            if (isLost)
            {
                // Fully lost - dark red/maroon background
                row.DefaultCellStyle.BackColor = CustomColors.LostItemBackground;
                row.DefaultCellStyle.SelectionBackColor = CustomColors.LostItemSelection;
                row.DefaultCellStyle.ForeColor = CustomColors.LostItemText;
            }
            else if (isPartiallyLost)
            {
                // Partially lost - dark orange/brown background
                row.DefaultCellStyle.BackColor = CustomColors.PartiallyLostItemBackground;
                row.DefaultCellStyle.SelectionBackColor = CustomColors.PartiallyLostItemSelection;
                row.DefaultCellStyle.ForeColor = CustomColors.PartiallyLostItemText;
            }
            else
            {
                // Not lost - default colors
                row.DefaultCellStyle.BackColor = Color.Empty;
                row.DefaultCellStyle.SelectionBackColor = Color.Empty;
                row.DefaultCellStyle.ForeColor = Color.Empty;
            }
        }

        private static void SaveLossChanges()
        {
            // Save both purchase and sale data
            MainMenu_Form.SaveDataGridViewToFileAsJson(MainMenu_Form.Instance.Purchase_DataGridView, MainMenu_Form.SelectedOption.Purchases);
            MainMenu_Form.SaveDataGridViewToFileAsJson(MainMenu_Form.Instance.Sale_DataGridView, MainMenu_Form.SelectedOption.Sales);
        }
    }
}
using Argo_Books.DataClasses;
using Argo_Books.Theme;
using Argo_Books;

namespace Argo_Books.ReturnProduct
{
    /// <summary>
    /// Manages product return operations for transactions including processing, undoing, and tracking return status.
    /// </summary>
    public static class ReturnManager
    {
        /// <summary>
        /// Processes a product return for the specified transaction row.
        /// </summary>
        public static void ProcessReturn(DataGridViewRow row, string reason, string additionalNotes, string returnedBy)
        {
            if (row.Tag is TagData tagData)
            {
                // Single item transaction
                tagData.IsReturned = true;
                tagData.ReturnDate = DateTime.Now;
                tagData.ReturnReason = string.IsNullOrEmpty(additionalNotes) ? reason : $"{reason} - {additionalNotes}";
                tagData.ReturnedBy = returnedBy;
                tagData.ReturnedItems = null;  // Full return, no need for item tracking
            }
            else if (row.Tag is (List<string>, TagData purchaseData))
            {
                // Multiple items transaction - return all items
                purchaseData.IsReturned = true;
                purchaseData.ReturnDate = DateTime.Now;
                purchaseData.ReturnReason = string.IsNullOrEmpty(additionalNotes) ? reason : $"{reason} - {additionalNotes}";
                purchaseData.ReturnedBy = returnedBy;
                purchaseData.ReturnedItems = null;  // Full return, no need for item tracking
            }
            else if (row.Tag is (string, TagData receiptData))
            {
                // Single item with receipt
                receiptData.IsReturned = true;
                receiptData.ReturnDate = DateTime.Now;
                receiptData.ReturnReason = string.IsNullOrEmpty(additionalNotes) ? reason : $"{reason} - {additionalNotes}";
                receiptData.ReturnedBy = returnedBy;
                receiptData.ReturnedItems = null;  // Full return, no need for item tracking
            }

            UpdateRowAppearanceForReturn(row, true);

            // Log the return
            string transactionId = row.Cells[ReadOnlyVariables.ID_column].Value?.ToString() ?? "Unknown";
            string productName = row.Cells[ReadOnlyVariables.Product_column].Value?.ToString() ?? "Unknown";
            string logMessage = $"Returned {(MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.Purchases
                ? "purchase" : "sale")} '{transactionId}' - {productName}. Reason: {reason}";

            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 2, logMessage);

            SaveReturnChanges();
            MainMenu_Form.Instance.RefreshDataGridViewAndCharts();
            MainMenu_Form.Instance.UpdateTotalLabels();
        }

        /// <summary>
        /// Processes a partial return for specific items in a multi-item transaction.
        /// </summary>
        public static void ProcessPartialReturn(DataGridViewRow row, List<int> itemIndices, string reason, string additionalNotes, string returnedBy)
        {
            if (row.Tag is not (List<string> items, TagData tagData))
            {
                // Not a multi-item transaction, fall back to regular return
                ProcessReturn(row, reason, additionalNotes, returnedBy);
                return;
            }

            // Initialize returned items list if it doesn't exist
            tagData.ReturnedItems ??= [];

            // Add new returned items (avoid duplicates)
            foreach (int itemIndex in itemIndices)
            {
                if (!tagData.ReturnedItems.Contains(itemIndex))
                {
                    tagData.ReturnedItems.Add(itemIndex);
                }
            }

            // Determine total returnable items (excluding receipt if present)
            int totalReturnableItems = items.Count;
            if (items.Count > 0 && items[^1].StartsWith(ReadOnlyVariables.Receipt_text))
            {
                totalReturnableItems--;
            }

            // Check if all items are now returned
            bool allItemsReturned = tagData.ReturnedItems.Count >= totalReturnableItems;

            // Update return status
            tagData.IsReturned = allItemsReturned;
            tagData.IsPartiallyReturned = !allItemsReturned && tagData.ReturnedItems.Count > 0;

            // Update return details
            if (tagData.ReturnDate == null)
            {
                tagData.ReturnDate = DateTime.Now;
                tagData.ReturnedBy = returnedBy;
            }

            // Append to existing return reason if there was a previous partial return
            string newReason = string.IsNullOrEmpty(additionalNotes) ? reason : $"{reason} - {additionalNotes}";
            if (string.IsNullOrEmpty(tagData.ReturnReason))
            {
                tagData.ReturnReason = newReason;
            }
            else
            {
                tagData.ReturnReason += $"; {newReason}";
            }

            UpdateRowAppearanceForReturn(row, tagData.IsReturned, tagData.IsPartiallyReturned);

            // Log the partial return
            string transactionId = row.Cells[ReadOnlyVariables.ID_column].Value?.ToString() ?? "Unknown";
            List<string> returnedItemNames = [];

            foreach (int index in itemIndices)
            {
                if (index < items.Count && !items[index].StartsWith(ReadOnlyVariables.Receipt_text))
                {
                    string[] itemDetails = items[index].Split(',');
                    if (itemDetails.Length > 0)
                    {
                        returnedItemNames.Add(itemDetails[0]);  // Product name
                    }
                }
            }

            string itemsText = string.Join(", ", returnedItemNames);
            string returnType = allItemsReturned ? "Completed return" : "Partial return";
            string logMessage = $"{returnType} for {(MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.Purchases
                ? "purchase" : "sale")} '{transactionId}' - Items: {itemsText}. Reason: {reason}";

            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 2, logMessage);

            SaveReturnChanges();
            MainMenu_Form.Instance.RefreshDataGridViewAndCharts();
            MainMenu_Form.Instance.UpdateTotalLabels();
        }

        /// <summary>
        /// Undoes a product return for the specified transaction row.
        /// </summary>
        public static void UndoReturn(DataGridViewRow row, string undoReason)
        {
            if (row.Tag is TagData tagData)
            {
                tagData.IsReturned = false;
                tagData.IsPartiallyReturned = false;
                tagData.ReturnDate = null;
                tagData.ReturnReason = null;
                tagData.ReturnedBy = null;
                tagData.ReturnedItems = null;
            }
            else if (row.Tag is (List<string>, TagData purchaseData))
            {
                purchaseData.IsReturned = false;
                purchaseData.IsPartiallyReturned = false;
                purchaseData.ReturnDate = null;
                purchaseData.ReturnReason = null;
                purchaseData.ReturnedBy = null;
                purchaseData.ReturnedItems = null;
            }
            else if (row.Tag is (string, TagData receiptData))
            {
                receiptData.IsReturned = false;
                receiptData.IsPartiallyReturned = false;
                receiptData.ReturnDate = null;
                receiptData.ReturnReason = null;
                receiptData.ReturnedBy = null;
                receiptData.ReturnedItems = null;
            }

            UpdateRowAppearanceForReturn(row, false);

            // Log the undo
            string transactionId = row.Cells[ReadOnlyVariables.ID_column].Value?.ToString() ?? "Unknown";
            string productName = row.Cells[ReadOnlyVariables.Product_column].Value?.ToString() ?? "Unknown";
            string logMessage = $"Undid return for {(MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.Purchases
                ? "purchase" : "sale")} '{transactionId}' - {productName}. Reason: {undoReason}";

            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 2, logMessage);

            SaveReturnChanges();
            MainMenu_Form.Instance.RefreshDataGridViewAndCharts();
            MainMenu_Form.Instance.UpdateTotalLabels();
        }

        /// <summary>
        /// Undoes a partial return for specific items in a multi-item transaction.
        /// </summary>
        public static void UndoPartialReturn(DataGridViewRow row, List<int> itemIndices, string undoReason)
        {
            if (row.Tag is not (List<string> items, TagData tagData) || tagData.ReturnedItems == null)
            {
                return;
            }

            // Remove specified items from returned items list
            foreach (int itemIndex in itemIndices)
            {
                tagData.ReturnedItems.Remove(itemIndex);
            }

            // Update return status
            bool hasReturnedItems = tagData.ReturnedItems.Count > 0;
            tagData.IsPartiallyReturned = hasReturnedItems;
            tagData.IsReturned = false;  // If we're undoing items, it's no longer fully returned

            // If no items are returned anymore, clear all return data
            if (!hasReturnedItems)
            {
                tagData.ReturnDate = null;
                tagData.ReturnReason = null;
                tagData.ReturnedBy = null;
                tagData.ReturnedItems = null;
            }

            UpdateRowAppearanceForReturn(row, tagData.IsReturned, tagData.IsPartiallyReturned);

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
            string logMessage = $"Undid partial return for {(MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.Purchases
                ? "purchase" : "sale")} '{transactionId}' - Items: {itemsText}. Reason: {undoReason}";

            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 2, logMessage);

            SaveReturnChanges();
            MainMenu_Form.Instance.RefreshDataGridViewAndCharts();
            MainMenu_Form.Instance.UpdateTotalLabels();
        }

        /// <summary>
        /// Checks if a transaction is returned (fully or partially).
        /// </summary>
        public static bool IsTransactionReturned(DataGridViewRow row)
        {
            if (row.Tag is TagData tagData)
            {
                return tagData.IsReturned || tagData.IsPartiallyReturned;
            }
            else if (row.Tag is (List<string>, TagData purchaseData))
            {
                return purchaseData.IsReturned || purchaseData.IsPartiallyReturned;
            }
            else if (row.Tag is (string, TagData receiptData))
            {
                return receiptData.IsReturned || receiptData.IsPartiallyReturned;
            }
            return false;
        }

        /// <summary>
        /// Checks if a transaction is fully returned.
        /// </summary>
        public static bool IsTransactionFullyReturned(DataGridViewRow row)
        {
            if (row.Tag is TagData tagData)
            {
                return tagData.IsReturned;
            }
            else if (row.Tag is (List<string>, TagData purchaseData))
            {
                return purchaseData.IsReturned;
            }
            else if (row.Tag is (string, TagData receiptData))
            {
                return receiptData.IsReturned;
            }
            return false;
        }

        /// <summary>
        /// Checks if a transaction is partially returned.
        /// </summary>
        public static bool IsTransactionPartiallyReturned(DataGridViewRow row)
        {
            if (row.Tag is TagData tagData)
            {
                return tagData.IsPartiallyReturned;
            }
            else if (row.Tag is (List<string>, TagData purchaseData))
            {
                return purchaseData.IsPartiallyReturned;
            }
            else if (row.Tag is (string, TagData receiptData))
            {
                return receiptData.IsPartiallyReturned;
            }
            return false;
        }

        /// <summary>
        /// Checks if a specific item within a transaction is returned.
        /// </summary>
        public static bool IsItemReturned(DataGridViewRow row, int itemIndex)
        {
            if (row.Tag is TagData tagData)
            {
                // Single item transaction
                return tagData.IsReturned;
            }
            else if (row.Tag is (List<string>, TagData purchaseData))
            {
                // Multi-item transaction
                if (purchaseData.IsReturned)
                {
                    return true;  // All items returned
                }
                return purchaseData.ReturnedItems?.Contains(itemIndex) ?? false;
            }
            else if (row.Tag is (string, TagData receiptData))
            {
                // Single item with receipt
                return receiptData.IsReturned;
            }
            return false;
        }

        /// <summary>
        /// Gets return information for a transaction.
        /// </summary>
        public static ReturnInfo GetReturnInfo(DataGridViewRow row)
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
                return new ReturnInfo
                {
                    ReturnDate = tagData.ReturnDate,
                    ReturnReason = tagData.ReturnReason ?? "",
                    ReturnedBy = tagData.ReturnedBy ?? "",
                    ReturnedItems = tagData.ReturnedItems ?? []
                };
            }

            // Return empty ReturnInfo for invalid data
            return new ReturnInfo();
        }

        /// <summary>
        /// Updates the visual appearance of a row based on its return status.
        /// </summary>
        public static void UpdateRowAppearanceForReturn(DataGridViewRow row, bool isReturned, bool isPartiallyReturned = false)
        {
            if (isReturned)
            {
                // Fully returned - red background
                row.DefaultCellStyle.BackColor = CustomColors.ReturnedItemBackground;
                row.DefaultCellStyle.SelectionBackColor = CustomColors.ReturnedItemSelection;
                row.DefaultCellStyle.ForeColor = CustomColors.ReturnedItemText;
            }
            else if (isPartiallyReturned)
            {
                // Partially returned - orange background
                row.DefaultCellStyle.BackColor = CustomColors.PartiallyReturnedItemBackground;
                row.DefaultCellStyle.SelectionBackColor = CustomColors.PartiallyReturnedItemSelection;
                row.DefaultCellStyle.ForeColor = CustomColors.PartiallyReturnedItemText;
            }
            else
            {
                // Not returned - default colors
                row.DefaultCellStyle.BackColor = Color.Empty;
                row.DefaultCellStyle.SelectionBackColor = Color.Empty;
                row.DefaultCellStyle.ForeColor = Color.Empty;
            }
        }

        private static void SaveReturnChanges()
        {
            // Save both purchase and sale data
            MainMenu_Form.SaveDataGridViewToFileAsJson(MainMenu_Form.Instance.Purchase_DataGridView, MainMenu_Form.SelectedOption.Purchases);
            MainMenu_Form.SaveDataGridViewToFileAsJson(MainMenu_Form.Instance.Sale_DataGridView, MainMenu_Form.SelectedOption.Sales);
        }
    }
}
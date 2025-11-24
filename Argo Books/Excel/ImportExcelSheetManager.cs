using Argo_Books.Classes;
using Argo_Books.DataClasses;
using Argo_Books.GridView;
using ClosedXML.Excel;
using Guna.UI2.WinForms;
using Argo_Books;
using Argo_Books.Classes;

namespace Argo_Books.Excel
{
    public class ImportExcelSheetManager
    {
        public enum InvalidValueAction
        {
            Skip,
            Cancel,
            Continue
        }
        public enum ImportTransactionResult
        {
            Success,
            Skip,
            Cancel,
            Failed
        }

        // Import data with rollback and cancellation support
        public static int ImportAccountantsData(IXLWorksheet worksheet, ImportSession session)
        {
            if (session.IsCancelled == true) { return 0; }

            return ImportSimpleListData(
                worksheet,
                MainMenu_Form.Instance.AccountantList,
                MainMenu_Form.SelectedOption.Accountants,
                "Accountant",
                Accountants_Form.Column.AccountantName,
                session.AddedAccountants,
                session);
        }
        public static int ImportCompaniesData(IXLWorksheet worksheet, ImportSession session)
        {
            if (session.IsCancelled == true) { return 0; }

            return ImportSimpleListData(
                worksheet,
                MainMenu_Form.Instance.CompanyList,
                MainMenu_Form.SelectedOption.Companies,
                "Company",
                Companies_Form.Column.Company,
                session.AddedCompanies,
                session);
        }

        /// <summary>
        /// Helper method for importing simple list data like accountants or companies.
        /// </summary>
        private static int ImportSimpleListData(
            IXLWorksheet worksheet,
            List<string> existingList,
            MainMenu_Form.SelectedOption optionType,
            string itemTypeName,
            Enum column,
            List<string> addedItems,
            ImportSession session)
        {
            if (session.IsCancelled == true) { return 0; }

            ExcelColumnHelper.ValidationResult validation = ExcelColumnHelper.ValidateSimpleListColumns(worksheet, column, GetColumnHeadersForType(optionType));
            if (!validation.IsValid)
            {
                ShowColumnValidationError(itemTypeName, validation.MissingColumns);
                return 0;
            }

            // Always skip header row (row 1) since we use it for column lookup
            IEnumerable<IXLRow> rowsToProcess = worksheet.RowsUsed().Skip(1);

            HashSet<string> existingItems = new(existingList.Count);
            foreach (string item in existingList)
            {
                existingItems.Add(item.ToLowerInvariant());
            }

            HashSet<string> addedDuringImport = [];

            foreach (IXLRow row in rowsToProcess)
            {
                // Check for cancellation before processing each row
                if (session.IsCancelled == true) { break; }

                string itemName = ExcelColumnHelper.GetCellValue(row, column);
                if (string.IsNullOrWhiteSpace(itemName) || itemName == ReadOnlyVariables.EmptyCell)
                {
                    continue;
                }

                string itemNameLower = itemName.ToLowerInvariant();

                // If the item already exists
                if (existingItems.Contains(itemNameLower))
                {
                    // Check if user has already made a "to all" choice for this entity type
                    bool? existingChoice = itemTypeName switch
                    {
                        "Accountant" => session.UserChoices.DuplicateAccountantChoice,
                        "Company" => session.UserChoices.DuplicateCompanyChoice,
                        _ => null
                    };

                    bool shouldSkip = false;

                    if (existingChoice.HasValue)
                    {
                        shouldSkip = !existingChoice.Value;  // If choice is false, skip (don't import)
                    }
                    else
                    {
                        // Show dialog with "Yes to All" and "No to All" options
                        CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                            "{0} already exists",
                            "The {1} '{2}' already exists. Would you like to import it anyway?",
                            CustomMessageBoxIcon.Question,
                            CustomMessageBoxButtons.YesNoAll,
                            itemTypeName, itemTypeName.ToLowerInvariant(), itemName);

                        switch (result)
                        {
                            case CustomMessageBoxResult.Yes:
                                break;
                            case CustomMessageBoxResult.No:
                                shouldSkip = true;
                                break;
                            case CustomMessageBoxResult.YesAll:
                                // Import all duplicates from now on
                                if (itemTypeName == "Accountant")
                                {
                                    session.UserChoices.DuplicateAccountantChoice = true;
                                }
                                else if (itemTypeName == "Company")
                                {
                                    session.UserChoices.DuplicateCompanyChoice = true;
                                }
                                break;
                            case CustomMessageBoxResult.NoAll:
                                // Skip all duplicates from now on
                                if (itemTypeName == "Accountant")
                                {
                                    session.UserChoices.DuplicateAccountantChoice = false;
                                }
                                else if (itemTypeName == "Company")
                                {
                                    session.UserChoices.DuplicateCompanyChoice = false;
                                }
                                shouldSkip = true;
                                break;
                            default:
                                shouldSkip = true;
                                break;
                        }
                    }

                    if (shouldSkip)
                    {
                        continue;
                    }
                }

                existingList.Add(itemName);
                addedDuringImport.Add(itemNameLower);
                addedItems.Add(itemName);
            }

            return addedItems.Count;
        }
        private static Dictionary<Enum, string> GetColumnHeadersForType(MainMenu_Form.SelectedOption optionType)
        {
            return optionType switch
            {
                MainMenu_Form.SelectedOption.Accountants =>
                    Accountants_Form.ColumnHeaders.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                MainMenu_Form.SelectedOption.Companies =>
                    Companies_Form.ColumnHeaders.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                _ => []
            };
        }
        public static int ImportProductsData(IXLWorksheet worksheet, bool isPurchase, ImportSession session)
        {
            if (session.IsCancelled == true) { return 0; }

            // Create column helper and validate required columns
            ExcelColumnHelper.ValidationResult validation = ExcelColumnHelper.ValidateProductColumns(worksheet);

            if (!validation.IsValid)
            {
                ShowColumnValidationError("Products", validation.MissingColumns);
                return 0;
            }

            // Always skip header row (row 1) since we use it for column lookup
            IEnumerable<IXLRow> rowsToProcess = worksheet.RowsUsed().Skip(1);

            List<Category> list = isPurchase
                ? MainMenu_Form.Instance.CategoryPurchaseList
                : MainMenu_Form.Instance.CategorySaleList;

            Dictionary<string, HashSet<string>> existingProducts = [];
            foreach (Category category in list)
            {
                existingProducts[category.Name] = [.. category.ProductList.Select(p => p.Name.ToLowerInvariant())];
            }

            Dictionary<string, HashSet<string>> addedDuringImport = [];

            // Read product data from the worksheet and add it to the category purchase list
            foreach (IXLRow row in rowsToProcess)
            {
                // Check for cancellation before processing each row
                if (session.IsCancelled == true) { break; }

                string productId = ExcelColumnHelper.GetCellValue(row, Products_Form.Column.ProductID);
                string productName = ExcelColumnHelper.GetCellValue(row, Products_Form.Column.ProductName);
                string categoryName = ExcelColumnHelper.GetCellValue(row, Products_Form.Column.ProductCategory);
                string countryOfOrigin = ExcelColumnHelper.GetCellValue(row, Products_Form.Column.CountryOfOrigin);
                string companyOfOrigin = ExcelColumnHelper.GetCellValue(row, Products_Form.Column.CompanyOfOrigin);

                // Skip if any essential field is empty
                if (string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(categoryName))
                {
                    continue;
                }

                countryOfOrigin = Country.NormalizeCountryName(countryOfOrigin);

                if (!ValidateCountry(countryOfOrigin, session))
                {
                    continue;
                }

                EnsureCompanyExists(companyOfOrigin, session);

                // Find or create the category
                Category category = FindOrCreateCategory(
                    list,
                    categoryName,
                    existingProducts,
                    addedDuringImport,
                    session);

                string productNameLower = productName.ToLowerInvariant();

                // Check if product already exists
                if (ProductExists(
                    existingProducts,
                    categoryName,
                    productNameLower,
                    productName,
                    isPurchase,
                    session))
                {
                    continue;
                }

                Product newProduct = CreateProduct(
                    productId,
                    productName,
                    productNameLower,
                    countryOfOrigin,
                    companyOfOrigin,
                    addedDuringImport,
                    categoryName);

                category.ProductList.Add(newProduct);

                // Track for rollback
                if (session != null)
                {
                    if (!session.AddedProducts.TryGetValue(categoryName, out List<Product>? value))
                    {
                        value = [];
                        session.AddedProducts[categoryName] = value;
                    }

                    value.Add(newProduct);
                }
            }

            return session.AddedProducts.Values.Sum(list => list.Count);
        }
        private static bool ValidateCountry(string countryName, ImportSession session)
        {
            bool countryExists = Country.CountrySearchResults.Any(
                c => c.Name.Equals(countryName, StringComparison.OrdinalIgnoreCase));

            if (!countryExists)
            {
                // Check if user has already made a "to all" choice
                if (session.UserChoices.CountryNotFoundChoice.HasValue)
                {
                    return session.UserChoices.CountryNotFoundChoice.Value;
                }

                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                    "Country does not exist",
                    "Country '{0}' does not exist in the system. Please check the documentation for more information. Do you want to skip this product and continue?",
                    CustomMessageBoxIcon.Exclamation,
                    CustomMessageBoxButtons.YesNoAll,
                    countryName);

                switch (result)
                {
                    case CustomMessageBoxResult.Yes:
                        return false;  // Skip this product
                    case CustomMessageBoxResult.No:
                        return true;  // Don't skip, continue with invalid country
                    case CustomMessageBoxResult.YesAll:
                        session.UserChoices.CountryNotFoundChoice = false;  // Skip all invalid countries
                        return false;
                    case CustomMessageBoxResult.NoAll:
                        session.UserChoices.CountryNotFoundChoice = true;  // Continue with all invalid countries
                        return true;
                    default:
                        return false;
                }
            }

            return true;
        }
        private static Category FindOrCreateCategory(
            List<Category> list,
            string categoryName,
            Dictionary<string, HashSet<string>> existingProducts,
            Dictionary<string, HashSet<string>> addedDuringImport,
            ImportSession session)
        {
            Category category = list.FirstOrDefault(c => c.Name == categoryName);

            if (category == null)
            {
                category = new Category { Name = categoryName };
                list.Add(category);
                existingProducts[categoryName] = [];
                addedDuringImport[categoryName] = [];

                // Track for rollback
                session.AddedCategories.Add(category);
            }
            else if (!addedDuringImport.ContainsKey(categoryName))
            {
                addedDuringImport[categoryName] = [];
            }

            return category;
        }
        private static bool ProductExists(
            Dictionary<string, HashSet<string>> existingProducts,
            string categoryName,
            string productNameLower,
            string productName,
            bool purchase,
            ImportSession session)
        {
            if (existingProducts.TryGetValue(categoryName, out HashSet<string> existingCategoryProducts) &&
                existingCategoryProducts.Contains(productNameLower))
            {
                // Check if user has already made a "to all" choice for duplicate products
                if (session.UserChoices.DuplicateProductChoice.HasValue)
                {
                    return !session.UserChoices.DuplicateProductChoice.Value;  // If choice is false, return true (exists, don't import)
                }

                string type = purchase ? "purchase" : "sale";

                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                    "Product already exists",
                    "The product for {0} '{1}' already exists. Would you like to import it anyway?",
                    CustomMessageBoxIcon.Question,
                    CustomMessageBoxButtons.YesNoAll,
                    type, productName);

                switch (result)
                {
                    case CustomMessageBoxResult.Yes:
                        return false;  // Don't skip, import this one
                    case CustomMessageBoxResult.No:
                        return true;  // Skip this one
                    case CustomMessageBoxResult.YesAll:
                        session.UserChoices.DuplicateProductChoice = true;  // Import all duplicates
                        return false;
                    case CustomMessageBoxResult.NoAll:
                        session.UserChoices.DuplicateProductChoice = false;  // Skip all duplicates
                        return true;
                    default:
                        return true;  // Default to skip
                }
            }

            return false;  // Product doesn't exist, proceed with import
        }
        private static Product CreateProduct(
            string productId,
            string productName,
            string productNameLower,
            string countryOfOrigin,
            string companyOfOrigin,
            Dictionary<string, HashSet<string>> addedDuringImport,
            string categoryName)
        {
            Product product = new()
            {
                ProductID = productId,
                Name = productName,
                CountryOfOrigin = countryOfOrigin,
                CompanyOfOrigin = companyOfOrigin
            };

            // Track that we've added this product
            if (!addedDuringImport.TryGetValue(categoryName, out HashSet<string> productsSet))
            {
                productsSet = [];
                addedDuringImport[categoryName] = productsSet;
            }
            productsSet.Add(productNameLower);

            return product;
        }

        public static ImportSummary ImportPurchaseData(IXLWorksheet worksheet, string sourceCurrency, ImportSession session)
        {
            if (session.IsCancelled == true)
            {
                return new ImportSummary { WasCancelled = true };
            }
            return ImportTransactionData(worksheet, true, sourceCurrency, session);
        }
        public static ImportSummary ImportSalesData(IXLWorksheet worksheet, string sourceCurrency, ImportSession session)
        {
            if (session.IsCancelled == true)
            {
                return new ImportSummary { WasCancelled = true };
            }
            return ImportTransactionData(worksheet, false, sourceCurrency, session);
        }
        public static int ImportReceiptsData(IXLWorksheet worksheet, string receiptsFolderPath, bool isPurchase, ImportSession session = null)
        {
            // Always skip header row (row 1) since we use it for column lookup
            IEnumerable<IXLRow> rowsToProcess = worksheet.RowsUsed().Skip(1);

            int importedCount = 0;

            foreach (IXLRow row in rowsToProcess)
            {
                string transactionId = ExcelColumnHelper.GetCellValue(row, MainMenu_Form.Column.ID);
                string receiptFileName = ExcelColumnHelper.GetCellValue(row, MainMenu_Form.Column.Receipt);

                DataGridViewRow? targetRow = FindTransactionRow(transactionId, isPurchase);

                // Skip item rows (rows without transaction IDs)
                if (string.IsNullOrEmpty(transactionId) || transactionId == ReadOnlyVariables.EmptyCell)
                {
                    continue;
                }

                // Skip receipt if the transaction was skipped during import
                if (session != null && session.SkippedTransactionIds.Contains(transactionId))
                {
                    continue;
                }

                // Skip if any essential field is empty
                if (string.IsNullOrWhiteSpace(transactionId) ||
                    string.IsNullOrWhiteSpace(receiptFileName) ||
                    string.IsNullOrWhiteSpace(receiptsFolderPath) ||
                    receiptFileName == ReadOnlyVariables.EmptyCell)
                {
                    DataGridViewCell noteCell = targetRow.Cells[ReadOnlyVariables.HasReceipt_column];
                    MainMenu_Form.SetReceiptCellToX(noteCell);
                    continue;
                }

                string receiptFilePath = Path.Combine(receiptsFolderPath, receiptFileName);

                // Check if the receipt file exists
                if (!File.Exists(receiptFilePath))
                {
                    CustomMessageBox.ShowWithFormat(
                        "Receipt does not exist",
                        "The receipt '{0}' does not exist in the folder you selected. This receipt will not be added.",
                        CustomMessageBoxIcon.Exclamation,
                        CustomMessageBoxButtons.Ok,
                        receiptFileName);
                    continue;
                }

                // Find the transaction in the correct DataGridView
                if (targetRow == null)
                {
                    string transactionType = isPurchase ? "purchase" : "sale";
                    Log.WriteWithFormat(1, "{0} {1} not found for receipt {2}", transactionType, transactionId, receiptFileName);
                    continue;
                }

                // Check if transaction already has a receipt
                if (TransactionHasReceipt(targetRow))
                {
                    string transactionType = isPurchase ? "purchase" : "sale";
                    CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                        "Transaction already has receipt",
                        "{0} {1} already has a receipt. Do you want to replace it?",
                        CustomMessageBoxIcon.Question,
                        CustomMessageBoxButtons.YesNo,
                        transactionType, transactionId);

                    if (result != CustomMessageBoxResult.Yes)
                    {
                        continue;
                    }
                }

                // Copy the receipt file to the receipts directory
                (string newReceiptPath, bool saved) = ReceiptManager.SaveReceiptInFile(receiptFilePath);
                if (!saved)
                {
                    continue;
                }

                ReceiptManager.AddReceiptToTag(targetRow, newReceiptPath);

                importedCount++;
            }

            // Save the updated transaction data
            if (importedCount > 0)
            {
                MainMenu_Form.SaveDataGridViewToFileAsJson(MainMenu_Form.Instance.Purchase_DataGridView, MainMenu_Form.SelectedOption.Purchases);
                MainMenu_Form.SaveDataGridViewToFileAsJson(MainMenu_Form.Instance.Sale_DataGridView, MainMenu_Form.SelectedOption.Sales);
            }

            return importedCount;
        }
        private static DataGridViewRow? FindTransactionRow(string transactionId, bool isPurchase)
        {
            Guna2DataGridView targetDataGridView = isPurchase
                ? MainMenu_Form.Instance.Purchase_DataGridView
                : MainMenu_Form.Instance.Sale_DataGridView;

            foreach (DataGridViewRow row in targetDataGridView.Rows)
            {
                if (row.Cells[ReadOnlyVariables.ID_column].Value?.ToString() == transactionId)
                {
                    return row;
                }
            }

            return null;
        }
        private static bool TransactionHasReceipt(DataGridViewRow row)
        {
            if (row.Tag is (List<string> tagList, TagData))
            {
                return tagList[^1].StartsWith(ReadOnlyVariables.Receipt_text);
            }
            else if (row.Tag is (string tagString, TagData))
            {
                return !string.IsNullOrEmpty(tagString);
            }
            return false;
        }

        /// <summary>
        /// Helper method for importing purchase and sales data with source currency support and immediate cancellation support.
        /// </summary>
        private static ImportSummary ImportTransactionData(IXLWorksheet worksheet, bool isPurchase, string sourceCurrency, ImportSession session)
        {
            ImportSummary summary = new();

            if (session.IsCancelled == true)
            {
                summary.WasCancelled = true;
                return summary;
            }

            // Get the target DataGridView to use its column structure
            Guna2DataGridView targetGridView = isPurchase
                ? MainMenu_Form.Instance.Purchase_DataGridView
                : MainMenu_Form.Instance.Sale_DataGridView;

            string itemType = isPurchase ? "Purchase" : "Sale";
            string worksheetName = isPurchase ? "Purchases" : "Sales";

            ExcelColumnHelper.ValidationResult validation = ExcelColumnHelper.ValidateTransactionColumns(worksheet, isPurchase);

            if (!validation.IsValid)
            {
                ShowColumnValidationError("Transactions", validation.MissingColumns);
                summary.Errors.Add(new ImportError
                {
                    WorksheetName = worksheetName,
                    FieldName = "Column Validation",
                    InvalidValue = $"Missing columns: {string.Join(", ", validation.MissingColumns)}"
                });
                return summary;
            }

            // Always skip header row (row 1) since we use it for column lookup
            IEnumerable<IXLRow> rowsToProcess = worksheet.RowsUsed().Skip(1);

            int newRowIndex = -1;
            int currentRowNumber = 2;  // Start at 2 since we're skipping header row (row 1)
            int successfulTransactions = 0;

            // Get existing transaction numbers
            HashSet<string> existingTransactionNumbers = new(targetGridView.Rows.Count);
            string idColumnHeader = targetGridView.Columns[0].HeaderText;

            foreach (DataGridViewRow row in targetGridView.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    existingTransactionNumbers.Add(row.Cells[0].Value.ToString());
                }
            }

            HashSet<string> addedDuringImport = [];

            foreach (IXLRow row in rowsToProcess)
            {
                // Check for cancellation before processing each row
                if (session.IsCancelled == true)
                {
                    summary.WasCancelled = true;
                    break;
                }

                currentRowNumber++;
                string transactionNumber = ExcelColumnHelper.GetCellValue(row, MainMenu_Form.Column.ID);

                // Check if this is an item row (part of a multi-item transaction)
                if (string.IsNullOrEmpty(transactionNumber) || transactionNumber == ReadOnlyVariables.EmptyCell)
                {
                    // This is an item row, not a main transaction row
                    // Item rows are processed by ImportItemsInTransaction, so we just skip them here
                    summary.ItemRowsProcessed++;
                    continue;
                }

                // Check if this row's transaction number already exists
                bool shouldContinue = CheckIfItemExists(
                    transactionNumber,
                    existingTransactionNumbers,
                    addedDuringImport,
                    itemType,
                    session);

                if (!shouldContinue)
                {
                    summary.SkippedRows++;
                    session.SkippedTransactionIds.Add(transactionNumber);
                    continue;
                }

                // Create a new row
                DataGridViewRow newRow = (DataGridViewRow)targetGridView.RowTemplate.Clone();
                newRow.CreateCells(targetGridView);

                // Add the row to the DataGridView right away to give it a OwningColumn.Name so it can be accessed by the column name
                targetGridView.InvokeIfRequired(() =>
                {
                    newRowIndex = targetGridView.Rows.Add(newRow);
                });

                ImportTransactionResult importResult = ImportTransaction(targetGridView, newRowIndex, row, newRow, currentRowNumber, worksheetName, sourceCurrency, session);

                switch (importResult)
                {
                    case ImportTransactionResult.Cancel:
                        RemoveRowFromDataGridView(targetGridView, newRowIndex);
                        session.IsCancelled = true;
                        summary.WasCancelled = true;
                        return summary;

                    case ImportTransactionResult.Skip:
                        RemoveRowFromDataGridView(targetGridView, newRowIndex);
                        summary.SkippedRows++;
                        session.SkippedTransactionIds.Add(transactionNumber);
                        continue;

                    case ImportTransactionResult.Failed:
                        RemoveRowFromDataGridView(targetGridView, newRowIndex);
                        summary.Errors.Add(new ImportError
                        {
                            TransactionId = transactionNumber,
                            RowNumber = currentRowNumber,
                            WorksheetName = worksheetName,
                            FieldName = "Transaction Import",
                            InvalidValue = "Technical failure during import"
                        });
                        return summary;

                    case ImportTransactionResult.Success:
                        break;
                }

                ImportTransactionResult itemsImportResult = ImportItemsInTransaction(row, newRow, currentRowNumber, worksheetName, sourceCurrency, session);

                switch (itemsImportResult)
                {
                    case ImportTransactionResult.Cancel:
                        session.IsCancelled = true;
                        summary.WasCancelled = true;
                        return summary;
                    case ImportTransactionResult.Skip:
                        summary.SkippedRows++;
                        session.SkippedTransactionIds.Add(transactionNumber);
                        continue;
                    case ImportTransactionResult.Failed:
                        summary.Errors.Add(new ImportError
                        {
                            TransactionId = transactionNumber,
                            RowNumber = currentRowNumber,
                            WorksheetName = worksheetName,
                            FieldName = "Items Import",
                            InvalidValue = "Technical failure during items import"
                        });
                        return summary;
                    case ImportTransactionResult.Success:
                        break;
                }

                FormatNoteCell(newRow);

                // Track for rollback
                if (isPurchase)
                {
                    session.AddedPurchaseRows.Add(newRow);
                }
                else
                {
                    session.AddedSaleRows.Add(newRow);
                }

                // Track that we've added this transaction number
                if (!string.IsNullOrEmpty(transactionNumber) && transactionNumber != ReadOnlyVariables.EmptyCell)
                {
                    addedDuringImport.Add(transactionNumber);
                }

                successfulTransactions++;
                DataGridViewManager.DataGridViewRowsAdded(targetGridView, new DataGridViewRowsAddedEventArgs(newRowIndex, 1));
            }

            if (isPurchase)
            {
                summary.PurchaseTransactionsImported = successfulTransactions;
            }
            else
            {
                summary.SaleTransactionsImported = successfulTransactions;
            }

            return summary;
        }
        private static void RemoveRowFromDataGridView(DataGridView dataGridView, int rowIndex)
        {
            dataGridView.InvokeIfRequired(() =>
            {
                if (rowIndex >= 0 && rowIndex < dataGridView.Rows.Count)
                {
                    dataGridView.Rows.RemoveAt(rowIndex);
                }
            });
        }

        /// <summary>
        /// Validates if a product exists in the appropriate category list.
        /// </summary>
        /// <returns>InvalidValueAction indicating whether to continue, skip, or cancel.</returns>
        private static InvalidValueAction ValidateProductExists(
            string productName,
            string categoryName,
            bool isPurchase,
            string transactionId,
            int rowNumber,
            string worksheetName,
            ImportSession session)
        {
            if (string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(categoryName))
            {
                return InvalidValueAction.Continue;  // Empty values are handled elsewhere
            }

            // Skip validation for "Multiple items" placeholder
            if (productName.Equals("Multiple items", StringComparison.OrdinalIgnoreCase))
            {
                return InvalidValueAction.Continue;
            }

            List<Category> categoryList = isPurchase
                ? MainMenu_Form.Instance.CategoryPurchaseList
                : MainMenu_Form.Instance.CategorySaleList;

            // Find the category
            Category? category = categoryList.FirstOrDefault(c =>
                c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

            if (category == null)
            {
                // Category doesn't exist
                return ShowProductNotFoundError(
                    productName,
                    categoryName,
                    transactionId,
                    rowNumber,
                    worksheetName,
                    $"Category '{categoryName}' does not exist",
                    session);
            }

            // Check if product exists in the category
            bool productExists = category.ProductList.Any(p =>
                p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));

            if (!productExists)
            {
                // Product doesn't exist in the category
                return ShowProductNotFoundError(
                  productName,
                  categoryName,
                  transactionId,
                  rowNumber,
                  worksheetName,
                  $"Product '{productName}' does not exist in category '{categoryName}'",
                  session);
            }

            return InvalidValueAction.Continue;
        }

        /// <summary>
        /// Shows error dialog when a product is not found during transaction import.
        /// </summary>
        private static InvalidValueAction ShowProductNotFoundError(
            string productName,
            string categoryName,
            string transactionId,
            int rowNumber,
            string worksheetName,
            string errorDescription,
            ImportSession session)
        {
            // Check if user has already made a "to all" choice
            if (session.UserChoices.ProductNotFoundChoice.HasValue)
            {
                return session.UserChoices.ProductNotFoundChoice.Value
                    ? InvalidValueAction.Skip
                    : InvalidValueAction.Cancel;
            }

            CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                "Product Not Found - Transaction Import",
                "Product not found during transaction import:\n\n" +
                "Worksheet: {0}\n" +
                "Row: {1}\n" +
                "Transaction ID: {2}\n" +
                "Product: '{3}'\n" +
                "Category: '{4}'\n" +
                "Error: {5}\n\n" +
                "The transaction cannot be imported because the product does not exist in the system. " +
                "Please ensure the product is created before importing transactions that reference it.\n\n" +
                "How would you like to proceed?",
                CustomMessageBoxIcon.Error,
                CustomMessageBoxButtons.SkipCancel,
                worksheetName, rowNumber, transactionId, productName, categoryName, errorDescription);

            return result switch
            {
                CustomMessageBoxResult.Skip => InvalidValueAction.Skip,
                CustomMessageBoxResult.Cancel => InvalidValueAction.Cancel,
                _ => InvalidValueAction.Cancel
            };
        }

        private static void ShowColumnValidationError(string importType, List<string> missingColumns)
        {
            string missingColumnsText = string.Join(", ", missingColumns.Select(c => $"'{c}'"));

            CustomMessageBox.ShowWithFormat(
                "Missing Required Columns",
                "Cannot import {0} data because the following required columns are missing:\n\n" +
                "Missing: {1}\n\n" +
                "Please ensure your spreadsheet has the correct column headers.",
                CustomMessageBoxIcon.Error,
                CustomMessageBoxButtons.Ok,
                importType, missingColumnsText);
        }

        public enum ImportReturnResult
        {
            Success,
            Skip,
            Cancel
        }
        private static ConversionResult ConvertStringToDecimalWithOptions(string value, ImportError errorContext)
        {
            if (value == ReadOnlyVariables.EmptyCell)
            {
                return new ConversionResult { Value = 0, IsValid = true, Action = InvalidValueAction.Continue };
            }

            string currentValue = value;

            while (true)
            {
                try
                {
                    decimal result = Convert.ToDecimal(currentValue);
                    return new ConversionResult
                    {
                        Value = Math.Round(result, 2, MidpointRounding.AwayFromZero),
                        IsValid = true,
                        Action = InvalidValueAction.Continue
                    };
                }
                catch
                {
                    errorContext.InvalidValue = currentValue;
                    CustomMessageBoxResult result = ShowDetailedImportError(errorContext);

                    switch (result)
                    {
                        case CustomMessageBoxResult.Skip:  // Skip transaction
                            return new ConversionResult { Value = 0, IsValid = false, Action = InvalidValueAction.Skip };
                        case CustomMessageBoxResult.Cancel:  // Cancel import
                            return new ConversionResult { Value = 0, IsValid = false, Action = InvalidValueAction.Cancel };
                        case CustomMessageBoxResult.Retry:  // Retry with user input
                            break;  // Continue the while loop - the user may update the spreadsheet with a new value
                        default:  // Treat as cancel
                            return new ConversionResult { Value = 0, IsValid = false, Action = InvalidValueAction.Cancel };
                    }
                }
            }
        }
        private static CustomMessageBoxResult ShowDetailedImportError(ImportError error)
        {
            return CustomMessageBox.ShowWithFormat(
                "Import Error - Invalid Monetary Value",
                "Invalid value found during import:\n\n" +
                "Worksheet: {0}\n" +
                "Row: {1}\n" +
                "Transaction ID: {2}\n" +
                "Field: {3}\n" +
                "Invalid Value: '{4}'\n\n" +
                "This value cannot be converted to a valid monetary amount. How would you like to proceed?",
                CustomMessageBoxIcon.Error,
                CustomMessageBoxButtons.SkipRetryCancel,
                error.WorksheetName, error.RowNumber, error.TransactionId, error.FieldName, error.InvalidValue);
        }

        /// <summary>
        /// Checks if an item already exists and asks the user if they want to add it anyway.
        /// </summary>
        /// <returns>True if the item should be added, false if it should be skipped.</returns>
        private static bool CheckIfItemExists(
            string itemNumber,
            HashSet<string> existingItems,
            HashSet<string> addedDuringImport,
            string itemTypeName,
            ImportSession session)
        {
            bool alreadyExistsInSystem = existingItems.Contains(itemNumber);
            bool alreadyAddedDuringImport = addedDuringImport.Contains(itemNumber);

            if (alreadyExistsInSystem || alreadyAddedDuringImport)
            {
                // Check if user has already made a "to all" choice
                if (session.UserChoices.DuplicateItemChoice.HasValue)
                {
                    return session.UserChoices.DuplicateItemChoice.Value;
                }

                CustomMessageBoxResult result;

                if (alreadyExistsInSystem)
                {
                    result = CustomMessageBox.ShowWithFormat(
                        "{0} # already exists",
                        "The {1} #{2} already exists. Would you like to add this {2} anyways?",
                        CustomMessageBoxIcon.Question,
                        CustomMessageBoxButtons.YesNoAll,
                        itemTypeName, itemTypeName.ToLowerInvariant(), itemNumber, itemTypeName.ToLowerInvariant());
                }
                else
                {
                    result = CustomMessageBox.ShowWithFormat(
                        "{0} # appears multiple times in this spreadsheet",
                        "The {1} #{2} appears multiple times in this spreadsheet. Would you like to add this duplicate anyways?",
                        CustomMessageBoxIcon.Question,
                        CustomMessageBoxButtons.YesNoAll,
                        itemTypeName, itemTypeName.ToLowerInvariant(), itemNumber);
                }

                switch (result)
                {
                    case CustomMessageBoxResult.Yes:
                        return true;
                    case CustomMessageBoxResult.No:
                        return false;
                    case CustomMessageBoxResult.YesAll:
                        session.UserChoices.DuplicateItemChoice = true;
                        return true;
                    case CustomMessageBoxResult.NoAll:
                        session.UserChoices.DuplicateItemChoice = false;
                        return false;
                    default:
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// This needs to be done after the row has been added to a DataGridView.
        /// </summary>
        private static void FormatNoteCell(DataGridViewRow row)
        {
            DataGridViewCell lastCell = row.Cells[ReadOnlyVariables.Note_column];

            // Only add underline if the cell has a note
            if (lastCell.Value?.ToString() == ReadOnlyVariables.Show_text && lastCell.Tag != null)
            {
                DataGridViewManager.AddUnderlineToCell(lastCell);
            }
        }

        /// <summary>
        /// Imports data into a DataGridViewRow with source currency support and immediate cancellation support.
        /// </summary>
        private static ImportTransactionResult ImportTransaction(
            DataGridView targetGridView,
            int rowIndex,
            IXLRow row,
            DataGridViewRow transaction,
            int rowNumber,
            string worksheetName,
            string sourceCurrency,
            ImportSession session)
        {
            if (session.IsCancelled == true) { return ImportTransactionResult.Cancel; }

            TagData tagData = new();

            // Get exchange rates
            string dateCellValue = ExcelColumnHelper.GetCellValue(row, MainMenu_Form.Column.Date);
            string date = Tools.FormatDate(Tools.ParseDateOrToday(dateCellValue));

            string defaultCurrency = DataFileManager.GetValue(AppDataSettings.DefaultCurrencyType);

            decimal exchangeRateToDefault = Currency.GetExchangeRate(sourceCurrency, defaultCurrency, date, false);
            if (exchangeRateToDefault == -1) { return ImportTransactionResult.Failed; }

            decimal exchangeRateToUSD = Currency.GetExchangeRate(sourceCurrency, "USD", date, false);
            if (exchangeRateToUSD == -1) { return ImportTransactionResult.Failed; }

            string transactionId = ExcelColumnHelper.GetCellValue(row, MainMenu_Form.Column.ID);
            string productName = ExcelColumnHelper.GetCellValue(row, MainMenu_Form.Column.Product);
            string categoryName = ExcelColumnHelper.GetCellValue(row, MainMenu_Form.Column.Category);
            string accountantName = ExcelColumnHelper.GetCellValue(row, MainMenu_Form.Column.Accountant);
            bool isPurchase = worksheetName.Equals("Purchases", StringComparison.OrdinalIgnoreCase);

            EnsureAccountantExists(accountantName, session);

            // Validate product exists
            InvalidValueAction productValidationResult = ValidateProductExists(
                productName,
                categoryName,
                isPurchase,
                transactionId,
                rowNumber,
                worksheetName,
                session);

            switch (productValidationResult)
            {
                case InvalidValueAction.Cancel:
                    return ImportTransactionResult.Cancel;
                case InvalidValueAction.Skip:
                    return ImportTransactionResult.Skip;
                case InvalidValueAction.Continue:
                    break;
            }

            // Get country and company from product data (not from Excel sheet)
            List<Category> categoryList = isPurchase
                ? MainMenu_Form.Instance.CategoryPurchaseList
                : MainMenu_Form.Instance.CategorySaleList;

            string companyFromProduct = MainMenu_Form.GetCompanyProductIsFrom(categoryList, productName);
            string countryFromProduct = MainMenu_Form.GetCountryProductIsFrom(categoryList, productName);

            int noteCellIndex = targetGridView.Rows[rowIndex].Cells[ReadOnlyVariables.Note_column].ColumnIndex;

            // Add values to each cell in the transaction row
            for (int i = 0; i < noteCellIndex; i++)
            {
                if (session.IsCancelled == true) { return ImportTransactionResult.Cancel; }

                // Skip the "Has Receipt" column
                if (targetGridView.Columns[i].Name == ReadOnlyVariables.HasReceipt_column)
                {
                    continue;
                }

                string headerText = ExportExcelSheetManager.GetColumnHeaderText(targetGridView.Columns[i]);
                MainMenu_Form.Column? columnType = GetColumnTypeFromHeader(headerText, isPurchase);

                if (columnType == null)
                {
                    // Log the error and cancel import
                    Log.WriteWithFormat(1, "Column mapping error in transaction import");

                    CustomMessageBox.ShowWithFormat(
                        "Column Mapping Error",
                        "Failed to map column '{0}' in {1}. The spreadsheet may have changed after it was selected. The import operation will be cancelled.",
                        CustomMessageBoxIcon.Error,
                        CustomMessageBoxButtons.Ok,
                        headerText, worksheetName);

                    return ImportTransactionResult.Cancel;
                }

                string value;

                // Handle Country and Company columns - get from product data instead of Excel
                if (columnType == MainMenu_Form.Column.Country)
                {
                    value = countryFromProduct;
                }
                else if (columnType == MainMenu_Form.Column.Company)
                {
                    value = companyFromProduct;
                }
                else
                {
                    // Get value from Excel for all other columns
                    value = ExcelColumnHelper.GetCellValue(row, columnType);
                }

                // Handle monetary fields using column type detection
                if (IsMonetaryColumn(headerText, isPurchase))
                {
                    ImportError errorContext = new()
                    {
                        TransactionId = transactionId,
                        FieldName = headerText,
                        RowNumber = rowNumber,
                        WorksheetName = worksheetName
                    };

                    ConversionResult conversionResult = ConvertStringToDecimalWithOptions(value, errorContext);

                    switch (conversionResult.Action)
                    {
                        case InvalidValueAction.Cancel:
                            return ImportTransactionResult.Cancel;
                        case InvalidValueAction.Skip:
                            return ImportTransactionResult.Skip;
                        case InvalidValueAction.Continue:
                            break;
                    }

                    decimal sourceValue = conversionResult.Value;
                    bool useEmpty = false;

                    // Store USD values in TagData based on column type
                    switch (columnType)
                    {
                        case MainMenu_Form.Column.PricePerUnit:
                            if (value == ReadOnlyVariables.EmptyCell)
                            {
                                useEmpty = true;
                            }
                            else
                            {
                                tagData.PricePerUnitUSD = Math.Round(sourceValue * exchangeRateToUSD, 2, MidpointRounding.AwayFromZero);
                            }
                            break;
                        case MainMenu_Form.Column.Shipping:
                            tagData.ShippingUSD = Math.Round(sourceValue * exchangeRateToUSD, 2, MidpointRounding.AwayFromZero);
                            break;
                        case MainMenu_Form.Column.Tax:
                            tagData.TaxUSD = Math.Round(sourceValue * exchangeRateToUSD, 2, MidpointRounding.AwayFromZero);
                            break;
                        case MainMenu_Form.Column.Fee:
                            tagData.FeeUSD = Math.Round(sourceValue * exchangeRateToUSD, 2, MidpointRounding.AwayFromZero);
                            break;
                        case MainMenu_Form.Column.Discount:
                            tagData.DiscountUSD = Math.Round(sourceValue * exchangeRateToUSD, 2, MidpointRounding.AwayFromZero);
                            break;
                        case MainMenu_Form.Column.ChargedDifference:
                            tagData.ChargedDifferenceUSD = Math.Round(sourceValue * exchangeRateToUSD, 2, MidpointRounding.AwayFromZero);
                            break;
                        case MainMenu_Form.Column.Total:
                            tagData.ChargedOrCreditedUSD = Math.Round(sourceValue * exchangeRateToUSD, 2, MidpointRounding.AwayFromZero);
                            break;
                    }

                    // Store the display value in default currency
                    transaction.Cells[i].Value = useEmpty
                        ? ReadOnlyVariables.EmptyCell
                        : Math.Round(sourceValue * exchangeRateToDefault, 2, MidpointRounding.AwayFromZero).ToString("N2");
                }
                else
                {
                    // Handle non-monetary fields
                    transaction.Cells[i].Value = string.IsNullOrEmpty(value) ? ReadOnlyVariables.EmptyCell : value;
                }
            }

            // Store the original currency information in TagData
            tagData.OriginalCurrency = sourceCurrency;
            if (exchangeRateToUSD != 0)
            {
                tagData.OriginalPricePerUnit = tagData.PricePerUnitUSD > 0 ? Math.Round(tagData.PricePerUnitUSD / exchangeRateToUSD, 2) : 0;
                tagData.OriginalShipping = Math.Round(tagData.ShippingUSD / exchangeRateToUSD, 2);
                tagData.OriginalTax = Math.Round(tagData.TaxUSD / exchangeRateToUSD, 2);
                tagData.OriginalFee = Math.Round(tagData.FeeUSD / exchangeRateToUSD, 2);
                tagData.OriginalDiscount = Math.Round(tagData.DiscountUSD / exchangeRateToUSD, 2);
                tagData.OriginalChargedDifference = Math.Round(tagData.ChargedDifferenceUSD / exchangeRateToUSD, 2);
                tagData.OriginalChargedOrCredited = Math.Round(tagData.ChargedOrCreditedUSD / exchangeRateToUSD, 2);
            }

            // Process return data if return columns exist
            ImportReturnResult returnResult = ProcessReturnDataFromExcel(row, tagData, transactionId, rowNumber, worksheetName, session);
            switch (returnResult)
            {
                case ImportReturnResult.Cancel:
                    return ImportTransactionResult.Cancel;
                case ImportReturnResult.Skip:
                    return ImportTransactionResult.Skip;
                case ImportReturnResult.Success:
                    break;
            }

            // Process loss data if loss columns exist
            ImportReturnResult lossResult = ProcessLossDataFromExcel(row, tagData, transactionId, rowNumber, worksheetName, session);
            switch (lossResult)
            {
                case ImportReturnResult.Cancel:
                    return ImportTransactionResult.Cancel;
                case ImportReturnResult.Skip:
                    return ImportTransactionResult.Skip;
                case ImportReturnResult.Success:
                    break;
            }

            // Handle notes
            DataGridViewCell noteCell = transaction.Cells[noteCellIndex];
            string excelNoteCellValue = ExcelColumnHelper.GetCellValue(row, MainMenu_Form.Column.Note);

            if (string.IsNullOrWhiteSpace(excelNoteCellValue) || excelNoteCellValue == ReadOnlyVariables.EmptyCell)
            {
                noteCell.Value = ReadOnlyVariables.EmptyCell;
            }
            else
            {
                noteCell.Value = ReadOnlyVariables.Show_text;
                noteCell.Tag = excelNoteCellValue;
            }

            transaction.Tag = tagData;
            return ImportTransactionResult.Success;
        }

        // Process return data
        private static ImportReturnResult ProcessReturnDataFromExcel(IXLRow row, TagData tagData, string transactionId, int rowNumber, string worksheetName, ImportSession session)
        {
            try
            {
                // Check if return columns exist in the worksheet
                bool hasReturnColumns = HasReturnColumns(row.Worksheet);
                if (!hasReturnColumns)
                {
                    // No return columns found, skip return processing
                    return ImportReturnResult.Success;
                }

                // Get return status
                string isReturnedValue = ExcelColumnHelper.GetCellValue(row, MainMenu_Form.Column.IsReturned);
                if (string.IsNullOrWhiteSpace(isReturnedValue) || isReturnedValue == ReadOnlyVariables.EmptyCell)
                {
                    // No return status specified
                    return ImportReturnResult.Success;
                }

                // Parse return status
                bool isReturned = false;
                bool isPartiallyReturned = false;

                switch (isReturnedValue.ToLowerInvariant().Trim())
                {
                    case "yes":
                    case "true":
                    case "1":
                    case "returned":
                        isReturned = true;
                        break;
                    case "partial":
                    case "partially":
                    case "partial return":
                        isPartiallyReturned = true;
                        break;
                    case "no":
                    case "false":
                    case "0":
                    case "":
                        // Not returned, leave defaults
                        break;
                    default:
                        // Invalid return status, show error
                        CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                            "Invalid Return Status",
                            "Invalid return status '{0}' found in row {1} of {2}. Expected values are: 'Yes', 'No', or 'Partial'.\n\nHow would you like to proceed?",
                            CustomMessageBoxIcon.Exclamation,
                            CustomMessageBoxButtons.SkipCancel,
                            isReturnedValue, rowNumber, worksheetName);

                        return result == CustomMessageBoxResult.Skip ? ImportReturnResult.Skip : ImportReturnResult.Cancel;
                }

                // If returned or partially returned, process additional return data
                if (isReturned || isPartiallyReturned)
                {
                    tagData.IsReturned = isReturned;
                    tagData.IsPartiallyReturned = isPartiallyReturned;

                    // Parse return date
                    string returnDateValue = ExcelColumnHelper.GetCellValue(row, MainMenu_Form.Column.ReturnDate);
                    if (!string.IsNullOrWhiteSpace(returnDateValue) && returnDateValue != ReadOnlyVariables.EmptyCell)
                    {
                        if (DateTime.TryParse(returnDateValue, out DateTime returnDate))
                        {
                            tagData.ReturnDate = returnDate;
                        }
                        else
                        {
                            // Invalid date format, use current date and log warning
                            tagData.ReturnDate = DateTime.Now;
                            Log.WriteWithFormat(1, "Invalid return date '{0}' in transaction {1}, using current date", returnDateValue, transactionId);
                        }
                    }
                    else
                    {
                        // No return date specified, use current date
                        tagData.ReturnDate = DateTime.Now;
                    }

                    // Get return reason
                    string returnReason = ExcelColumnHelper.GetCellValue(row, MainMenu_Form.Column.ReturnReason);
                    if (!string.IsNullOrWhiteSpace(returnReason) && returnReason != ReadOnlyVariables.EmptyCell)
                    {
                        tagData.ReturnReason = returnReason;
                    }
                    else
                    {
                        tagData.ReturnReason = "Imported return (no reason specified)";
                    }

                    // Get returned by
                    string returnedBy = ExcelColumnHelper.GetCellValue(row, MainMenu_Form.Column.ReturnedBy);
                    if (!string.IsNullOrWhiteSpace(returnedBy) && returnedBy != ReadOnlyVariables.EmptyCell)
                    {
                        tagData.ReturnedBy = returnedBy;
                        EnsureAccountantExists(returnedBy, session);
                    }
                    else
                    {
                        tagData.ReturnedBy = "Import process";
                    }

                    // For partial returns, parse returned items (this will be handled later in multi-item import)
                    if (isPartiallyReturned)
                    {
                        string returnedItemsValue = ExcelColumnHelper.GetCellValue(row, MainMenu_Form.Column.ReturnedItems);
                        if (!string.IsNullOrWhiteSpace(returnedItemsValue) && returnedItemsValue != ReadOnlyVariables.EmptyCell)
                        {
                            // Store the returned items text to be processed later when we have the full item list
                            tagData.ReturnedItemsText = returnedItemsValue;
                        }
                    }
                }

                return ImportReturnResult.Success;
            }
            catch (Exception ex)
            {
                Log.WriteWithFormat(1, "Error processing return data for transaction {0}: {1}", transactionId, ex.Message);

                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                    "Return Data Import Error",
                    "An error occurred while processing return data for transaction {0} in row {1}:\n\n{2}\n\nHow would you like to proceed?",
                    CustomMessageBoxIcon.Error,
                    CustomMessageBoxButtons.SkipCancel,
                    transactionId, rowNumber, ex.Message);

                return result == CustomMessageBoxResult.Skip ? ImportReturnResult.Skip : ImportReturnResult.Cancel;
            }
        }
        private static bool HasReturnColumns(IXLWorksheet worksheet)
        {
            Dictionary<Enum, string> columnHeaders = new()
            {
                { MainMenu_Form.Column.IsReturned, "Is Returned" },
                { MainMenu_Form.Column.ReturnDate, "Return Date" },
                { MainMenu_Form.Column.ReturnReason, "Return Reason" },
                { MainMenu_Form.Column.ReturnedBy, "Returned By" },
                { MainMenu_Form.Column.ReturnedItems, "Returned Items" }
            };

            // Check if at least one return column exists
            return columnHeaders.Any(kvp => ExcelColumnHelper.HasColumn(worksheet, kvp.Key, columnHeaders));
        }
        private static void ProcessReturnedItemsForMultiItemTransaction(List<string> items, TagData tagData)
        {
            if (string.IsNullOrWhiteSpace(tagData.ReturnedItemsText))
            {
                return;
            }

            tagData.ReturnedItems = [];

            try
            {
                // Parse returned items text
                string[] returnedItemsArray = tagData.ReturnedItemsText.Split([';', ','], StringSplitOptions.RemoveEmptyEntries);

                foreach (string returnedItemText in returnedItemsArray)
                {
                    string trimmedItem = returnedItemText.Trim();

                    // Try to parse as item index first
                    if (int.TryParse(trimmedItem, out int itemIndex) && itemIndex >= 0 && itemIndex < items.Count)
                    {
                        tagData.ReturnedItems.Add(itemIndex);
                        continue;
                    }

                    // Try to find by item name
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i].StartsWith(ReadOnlyVariables.Receipt_text))
                        {
                            continue;  // Skip receipt entries
                        }

                        string[] itemDetails = items[i].Split(',');
                        if (itemDetails.Length > 0 &&
                            itemDetails[0].Equals(trimmedItem, StringComparison.OrdinalIgnoreCase))
                        {
                            tagData.ReturnedItems.Add(i);
                            break;
                        }
                    }
                }

                // Validate partial return consistency
                if (tagData.IsPartiallyReturned && tagData.ReturnedItems.Count == 0)
                {
                    // No valid returned items found for partial return, treat as not returned
                    tagData.IsPartiallyReturned = false;
                    tagData.ReturnDate = null;
                    tagData.ReturnReason = null;
                    tagData.ReturnedBy = null;

                    Log.WriteWithFormat(1, "No valid returned items found for partial return, treating transaction as not returned");
                }
                else if (tagData.IsPartiallyReturned && tagData.ReturnedItems.Count >= items.Count - (items[^1].StartsWith(ReadOnlyVariables.Receipt_text) ? 1 : 0))
                {
                    // All items are returned, change to full return
                    tagData.IsReturned = true;
                    tagData.IsPartiallyReturned = false;
                    tagData.ReturnedItems = null;  // Clear since it's a full return
                }
            }
            catch (Exception ex)
            {
                Log.WriteWithFormat(1, "Error parsing returned items '{0}': {1}", tagData.ReturnedItemsText, ex.Message);

                // Clear partial return data due to parsing error
                tagData.IsPartiallyReturned = false;
                tagData.ReturnDate = null;
                tagData.ReturnReason = null;
                tagData.ReturnedBy = null;
                tagData.ReturnedItems = null;
            }
            finally
            {
                // Clean up the temporary text field
                tagData.ReturnedItemsText = null;
            }
        }

        // Process loss data
        private static ImportReturnResult ProcessLossDataFromExcel(IXLRow row, TagData tagData, string transactionId, int rowNumber, string worksheetName, ImportSession session)
        {
            try
            {
                // Check if loss columns exist in the worksheet
                bool hasLossColumns = HasLossColumns(row.Worksheet);
                if (!hasLossColumns)
                {
                    return ImportReturnResult.Success;
                }

                // Get loss status
                string isLostValue = ExcelColumnHelper.GetCellValue(row, MainMenu_Form.Column.IsLost);
                if (string.IsNullOrWhiteSpace(isLostValue) || isLostValue == ReadOnlyVariables.EmptyCell)
                {
                    return ImportReturnResult.Success;
                }

                // Parse loss status
                bool isLost = false;
                bool isPartiallyLost = false;

                switch (isLostValue.ToLowerInvariant().Trim())
                {
                    case "yes":
                    case "true":
                    case "1":
                    case "lost":
                        isLost = true;
                        break;
                    case "partial":
                    case "partially":
                    case "partial loss":
                        isPartiallyLost = true;
                        break;
                    case "no":
                    case "false":
                    case "0":
                    case "":
                        break;
                    default:
                        // Handle invalid loss status similar to return status
                        CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                            "Invalid Loss Status",
                            "Invalid loss status '{0}' found in row {1} of {2}. Expected values are: 'Yes', 'No', or 'Partial'.\n\nHow would you like to proceed?",
                            CustomMessageBoxIcon.Exclamation,
                            CustomMessageBoxButtons.SkipCancel,
                            isLostValue, rowNumber, worksheetName);
                        return result == CustomMessageBoxResult.Skip ? ImportReturnResult.Skip : ImportReturnResult.Cancel;
                }

                // Process loss data similar to return data
                if (isLost || isPartiallyLost)
                {
                    tagData.IsLost = isLost;
                    tagData.IsPartiallyLost = isPartiallyLost;

                    // Parse loss date
                    string lostDateValue = ExcelColumnHelper.GetCellValue(row, MainMenu_Form.Column.LostDate);
                    if (!string.IsNullOrWhiteSpace(lostDateValue) && lostDateValue != ReadOnlyVariables.EmptyCell)
                    {
                        if (DateTime.TryParse(lostDateValue, out DateTime lostDate))
                        {
                            tagData.LostDate = lostDate;
                        }
                        else
                        {
                            tagData.LostDate = DateTime.Now;
                            Log.WriteWithFormat(1, "Invalid lost date '{0}' in transaction {1}, using current date", lostDateValue, transactionId);
                        }
                    }
                    else
                    {
                        tagData.LostDate = DateTime.Now;
                    }

                    // Get loss reason and lost by
                    string lostReason = ExcelColumnHelper.GetCellValue(row, MainMenu_Form.Column.LostReason);
                    tagData.LostReason = !string.IsNullOrWhiteSpace(lostReason) && lostReason != ReadOnlyVariables.EmptyCell
                        ? lostReason
                        : "Imported loss (no reason specified)";

                    string lostBy = ExcelColumnHelper.GetCellValue(row, MainMenu_Form.Column.LostBy);
                    if (!string.IsNullOrWhiteSpace(lostBy) && lostBy != ReadOnlyVariables.EmptyCell)
                    {
                        tagData.LostBy = lostBy;
                        // Ensure the accountant exists in the system
                        EnsureAccountantExists(lostBy, session);
                    }
                    else
                    {
                        tagData.LostBy = "Import process";
                    }
                }

                return ImportReturnResult.Success;
            }
            catch (Exception ex)
            {
                Log.WriteWithFormat(1, "Error processing loss data for transaction {0}: {1}", transactionId, ex.Message);
                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                    "Loss Data Import Error",
                    "An error occurred while processing loss data for transaction {0} in row {1}:\n\n{2}\n\nHow would you like to proceed?",
                    CustomMessageBoxIcon.Error,
                    CustomMessageBoxButtons.SkipCancel,
                    transactionId, rowNumber, ex.Message);
                return result == CustomMessageBoxResult.Skip ? ImportReturnResult.Skip : ImportReturnResult.Cancel;
            }
        }
        private static bool HasLossColumns(IXLWorksheet worksheet)
        {
            Dictionary<Enum, string> columnHeaders = new()
            {
                { MainMenu_Form.Column.IsLost, "Is Lost" },
                { MainMenu_Form.Column.LostDate, "Lost Date" },
                { MainMenu_Form.Column.LostReason, "Lost Reason" },
                { MainMenu_Form.Column.LostBy, "Lost By" },
                { MainMenu_Form.Column.LostItems, "Lost Items" }
            };

            return columnHeaders.Any(kvp => ExcelColumnHelper.HasColumn(worksheet, kvp.Key, columnHeaders));
        }

        // Import spreadsheet helper methods
        /// <summary>
        /// Gets the column type from a header text.
        /// </summary>
        private static MainMenu_Form.Column? GetColumnTypeFromHeader(string headerText, bool isPurchase)
        {
            Dictionary<MainMenu_Form.Column, string> columnHeaders = isPurchase
                ? MainMenu_Form.Instance.PurchaseColumnHeaders
                : MainMenu_Form.Instance.SalesColumnHeaders;

            // First try exact match (case-insensitive) for performance
            foreach (KeyValuePair<MainMenu_Form.Column, string> kvp in columnHeaders)
            {
                if (string.Equals(kvp.Value, headerText, StringComparison.OrdinalIgnoreCase))
                {
                    return kvp.Key;
                }
            }

            // Try flexible matching for each column type
            foreach (KeyValuePair<MainMenu_Form.Column, string> kvp in columnHeaders)
            {
                if (ExcelColumnHelper.IsFlexibleMatch(headerText, kvp.Key))
                {
                    return kvp.Key;
                }
            }

            // No match found
            return null;
        }

        /// <summary>
        /// Determines if a column contains monetary values based on header text and context.
        /// </summary>
        private static bool IsMonetaryColumn(string columnHeaderText, bool isPurchase)
        {
            // Get the appropriate column headers dictionary
            Dictionary<MainMenu_Form.Column, string> columnHeaders = isPurchase
                ? MainMenu_Form.Instance.PurchaseColumnHeaders
                : MainMenu_Form.Instance.SalesColumnHeaders;

            // Define which columns are monetary
            MainMenu_Form.Column[] monetaryColumnTypes =
            [
                MainMenu_Form.Column.PricePerUnit,
                MainMenu_Form.Column.Shipping,
                MainMenu_Form.Column.Tax,
                MainMenu_Form.Column.Fee,
                MainMenu_Form.Column.Discount,
                MainMenu_Form.Column.ChargedDifference,
                MainMenu_Form.Column.Total
            ];

            // Check if the given header text matches any of the monetary columns
            foreach (MainMenu_Form.Column columnType in monetaryColumnTypes)
            {
                if (columnHeaders.TryGetValue(columnType, out string standardHeader) &&
                    string.Equals(standardHeader, columnHeaderText, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        private static ImportTransactionResult ImportItemsInTransaction(IXLRow row, DataGridViewRow transaction, int baseRowNumber, string worksheetName, string sourceCurrency, ImportSession session)
        {
            if (session.IsCancelled == true) { return ImportTransactionResult.Cancel; }

            TagData tagData = (TagData)transaction.Tag;
            List<string> items = [];
            int currentRowOffset = 0;

            // Get exchange rates
            string dateCellValue = transaction.Cells[6].Value?.ToString();
            string date = Tools.FormatDate(Tools.ParseDateOrToday(dateCellValue));

            decimal exchangeRateToUSD = Currency.GetExchangeRate(sourceCurrency, "USD", date, false);
            decimal exchangeRateToDefault = Currency.GetExchangeRate(sourceCurrency, DataFileManager.GetValue(AppDataSettings.DefaultCurrencyType), date, false);

            if (exchangeRateToUSD == -1 || exchangeRateToDefault == -1)
            {
                return ImportTransactionResult.Failed;
            }

            bool isPurchase = worksheetName.Equals("Purchases", StringComparison.OrdinalIgnoreCase);
            List<Category> categoryList = isPurchase
                ? MainMenu_Form.Instance.CategoryPurchaseList
                : MainMenu_Form.Instance.CategorySaleList;

            while (true)
            {
                // Check for cancellation before processing each item
                if (session.IsCancelled == true) { return ImportTransactionResult.Cancel; }

                IXLRow nextRow = row.RowBelow();
                currentRowOffset++;

                // Check if the row has any data
                if (nextRow.IsEmpty())
                {
                    break;
                }

                // Check if the next row has a transaction ID - if it doesn't, it's an item row
                string number = ExcelColumnHelper.GetCellValue(nextRow, MainMenu_Form.Column.ID);

                // Check if the next row has no number, indicating multiple items
                if (string.IsNullOrEmpty(number))
                {
                    string productName = ExcelColumnHelper.GetCellValue(nextRow, MainMenu_Form.Column.Product);
                    string categoryName = ExcelColumnHelper.GetCellValue(nextRow, MainMenu_Form.Column.Category);

                    // Get country and company from product data instead of Excel sheet
                    string currentCountry = "";
                    string currentCompany = "";

                    // Try to find the product in the category list to get country and company
                    Category? category = MainMenu_Form.GetCategoryCategoryNameIsFrom(categoryList, categoryName);
                    if (category != null)
                    {
                        Product? product = category.ProductList.FirstOrDefault(p =>
                            p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));

                        if (product != null)
                        {
                            currentCountry = product.CountryOfOrigin ?? "";
                            currentCompany = product.CompanyOfOrigin ?? "";
                        }
                    }

                    // Validate product exists
                    string transactionId = transaction.Cells[0].Value?.ToString() ?? "Unknown";
                    InvalidValueAction productValidationResult = ValidateProductExists(
                        productName,
                        categoryName,
                        isPurchase,
                        transactionId,
                        baseRowNumber + currentRowOffset,
                        worksheetName,
                        session);

                    switch (productValidationResult)
                    {
                        case InvalidValueAction.Cancel:
                            return ImportTransactionResult.Cancel;
                        case InvalidValueAction.Skip:
                            return ImportTransactionResult.Skip;
                        case InvalidValueAction.Continue:
                            break;
                    }

                    // Get quantity and price using column types
                    string quantityValue = ExcelColumnHelper.GetCellValue(nextRow, MainMenu_Form.Column.TotalItems);
                    string priceValue = ExcelColumnHelper.GetCellValue(nextRow, MainMenu_Form.Column.PricePerUnit);

                    // Use error handling for quantity and price
                    ImportError quantityErrorContext = new()
                    {
                        TransactionId = transactionId,
                        FieldName = "Item Quantity",
                        RowNumber = baseRowNumber + currentRowOffset,
                        WorksheetName = worksheetName
                    };

                    ConversionResult quantityResult = ConvertStringToDecimalWithOptions(quantityValue, quantityErrorContext);

                    if (quantityResult.Action == InvalidValueAction.Cancel)
                    {
                        return ImportTransactionResult.Cancel;
                    }
                    if (quantityResult.Action == InvalidValueAction.Skip)
                    {
                        return ImportTransactionResult.Skip;
                    }

                    ImportError priceErrorContext = new()
                    {
                        TransactionId = transactionId,
                        FieldName = "Item Price Per Unit",
                        RowNumber = baseRowNumber + currentRowOffset,
                        WorksheetName = worksheetName
                    };

                    ConversionResult priceResult = ConvertStringToDecimalWithOptions(priceValue, priceErrorContext);

                    if (priceResult.Action == InvalidValueAction.Cancel)
                    {
                        return ImportTransactionResult.Cancel;
                    }
                    if (priceResult.Action == InvalidValueAction.Skip)
                    {
                        return ImportTransactionResult.Skip;
                    }

                    decimal quantity = quantityResult.Value;
                    decimal sourcePricePerUnit = priceResult.Value;

                    // Convert prices to default currency for display and USD for storage
                    decimal defaultPricePerUnit = Math.Round(sourcePricePerUnit * exchangeRateToDefault, 2, MidpointRounding.AwayFromZero);
                    decimal usdPricePerUnit = Math.Round(sourcePricePerUnit * exchangeRateToUSD, 2, MidpointRounding.AwayFromZero);

                    string item = string.Join(",",
                        productName,
                        categoryName,
                        currentCountry,
                        currentCompany,
                        quantity.ToString(),
                        defaultPricePerUnit.ToString("F2"),
                        usdPricePerUnit.ToString("F2")
                    );

                    items.Add(item);
                    row = nextRow;  // Move to the next row
                }
                else
                {
                    break;
                }
            }

            // After all items are processed, handle returned items for partial returns
            if (tagData.IsPartiallyReturned && !string.IsNullOrWhiteSpace(tagData.ReturnedItemsText))
            {
                ProcessReturnedItemsForMultiItemTransaction(items, tagData);
            }

            // Save updated tag data
            if (items.Count > 0)
            {
                transaction.Tag = (items, tagData);
            }

            return ImportTransactionResult.Success;
        }

        /// <summary>
        /// Rollback all changes made during the import session.
        /// </summary>
        public static void RollbackImportSession(ImportSession session)
        {
            if (!session.HasChanges())
            {
                return;
            }

            // Remove added accountants
            MainMenu_Form.Instance.AccountantList.RemoveRange(session.AddedAccountants);

            // Remove added companies
            MainMenu_Form.Instance.CompanyList.RemoveRange(session.AddedCompanies);

            // Remove added products
            foreach (KeyValuePair<string, List<Product>> categoryProducts in session.AddedProducts)
            {
                string categoryName = categoryProducts.Key;
                List<Product> productsToRemove = categoryProducts.Value;

                // Find the category in both purchase and sale lists
                Category? purchaseCategory = MainMenu_Form.Instance.CategoryPurchaseList
                    .FirstOrDefault(c => c.Name == categoryName);
                Category? saleCategory = MainMenu_Form.Instance.CategorySaleList
                    .FirstOrDefault(c => c.Name == categoryName);

                purchaseCategory?.ProductList.RemoveRange(productsToRemove);
                saleCategory?.ProductList.RemoveRange(productsToRemove);
            }

            // Remove added categories
            MainMenu_Form.Instance.CategoryPurchaseList.RemoveRange(session.AddedCategories);
            MainMenu_Form.Instance.CategorySaleList.RemoveRange(session.AddedCategories);

            // Remove added rows from DataGridViews
            MainMenu_Form.Instance.Purchase_DataGridView.Rows.RemoveRowsIfExists(session.AddedPurchaseRows);
            MainMenu_Form.Instance.Sale_DataGridView.Rows.RemoveRowsIfExists(session.AddedSaleRows);

            // Save the reverted state
            MainMenu_Form.SaveListToFile(MainMenu_Form.Instance.AccountantList, MainMenu_Form.SelectedOption.Accountants);
            MainMenu_Form.SaveListToFile(MainMenu_Form.Instance.CompanyList, MainMenu_Form.SelectedOption.Companies);
            MainMenu_Form.Instance.SaveCategoriesToFile(MainMenu_Form.SelectedOption.CategoryPurchases);
            MainMenu_Form.Instance.SaveCategoriesToFile(MainMenu_Form.SelectedOption.CategorySales);
            MainMenu_Form.SaveDataGridViewToFileAsJson(MainMenu_Form.Instance.Purchase_DataGridView, MainMenu_Form.SelectedOption.Purchases);
            MainMenu_Form.SaveDataGridViewToFileAsJson(MainMenu_Form.Instance.Sale_DataGridView, MainMenu_Form.SelectedOption.Sales);
        }

        /// <summary>
        /// Commit all changes made during the import session.
        /// </summary>
        public static void CommitImportSession(ImportSession session)
        {
            if (!session.HasChanges())
            {
                return;
            }

            // Save all changes to files
            if (session.AddedAccountants.Count > 0)
            {
                MainMenu_Form.SaveListToFile(MainMenu_Form.Instance.AccountantList, MainMenu_Form.SelectedOption.Accountants);
            }

            if (session.AddedCompanies.Count > 0)
            {
                MainMenu_Form.SaveListToFile(MainMenu_Form.Instance.CompanyList, MainMenu_Form.SelectedOption.Companies);
            }

            if (session.AddedProducts.Count > 0 || session.AddedCategories.Count > 0)
            {
                MainMenu_Form.Instance.SaveCategoriesToFile(MainMenu_Form.SelectedOption.CategoryPurchases);
                MainMenu_Form.Instance.SaveCategoriesToFile(MainMenu_Form.SelectedOption.CategorySales);
            }

            if (session.AddedPurchaseRows.Count > 0)
            {
                MainMenu_Form.SaveDataGridViewToFileAsJson(MainMenu_Form.Instance.Purchase_DataGridView, MainMenu_Form.SelectedOption.Purchases);
            }

            if (session.AddedSaleRows.Count > 0)
            {
                MainMenu_Form.SaveDataGridViewToFileAsJson(MainMenu_Form.Instance.Sale_DataGridView, MainMenu_Form.SelectedOption.Sales);
            }
        }

        /// <summary>
        /// Ensures a company exists in the system, creating it silently if missing.
        /// </summary>
        private static void EnsureCompanyExists(string companyName, ImportSession session)
        {
            if (string.IsNullOrWhiteSpace(companyName))
            {
                return;
            }

            // Check if company already exists (case-insensitive)
            bool companyExists = MainMenu_Form.Instance.CompanyList.Any(c =>
                c.Equals(companyName, StringComparison.OrdinalIgnoreCase));

            if (!companyExists)
            {
                MainMenu_Form.Instance.CompanyList.Add(companyName);
                session.AddedCompanies.Add(companyName);  // Track for rollback
            }
        }

        /// <summary>
        /// Ensures an accountant exists in the system, creating it silently if missing.
        /// </summary>
        private static void EnsureAccountantExists(string accountantName, ImportSession session)
        {
            if (string.IsNullOrWhiteSpace(accountantName))
            {
                return;
            }

            // Check if accountant already exists (case-insensitive)
            bool accountantExists = MainMenu_Form.Instance.AccountantList.Any(a =>
                a.Equals(accountantName, StringComparison.OrdinalIgnoreCase));

            if (!accountantExists)
            {
                MainMenu_Form.Instance.AccountantList.Add(accountantName);
                session.AddedAccountants.Add(accountantName);  // Track for rollback
            }
        }
    }
}
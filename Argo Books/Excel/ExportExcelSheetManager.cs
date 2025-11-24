using Argo_Books.Classes;
using Argo_Books.DataClasses;
using Argo_Books.GridView;
using ClosedXML.Excel;
using Argo_Books;
using Argo_Books.Classes;

namespace Argo_Books.Excel
{
    /// <summary>
    /// Handles the import and export of data to and from .xlsx spreadsheets, including transactions
    /// and receipts. It also has immediate cancellation support and multi-currency import capabilities.
    /// </summary>
    public class ExportExcelSheetManager
    {
        // Constants
        private const string _numberFormatPattern = "0";

        // Helper method
        /// <summary>
        /// Gets the header text from a DataGridView column, handling both regular and custom image header cells.
        /// </summary>
        public static string GetColumnHeaderText(DataGridViewColumn column)
        {
            if (column.HeaderCell is DataGridViewImageHeaderCell customHeaderCell)
            {
                return customHeaderCell.HeaderText;
            }
            else
            {
                return column.HeaderText;
            }
        }

        // Export spreadsheet methods
        public static void ExportSpreadsheet(string filePath, string currency)
        {
            filePath = Directories.GetNewFileNameIfItAlreadyExists(filePath);

            // Parse currency code from the ComboBox format (e.g., "USD ($)" -> "USD")
            string currencyCode = ParseCurrencyCode(currency);
            string currencySymbol = Currency.GetSymbol(currencyCode);

            using XLWorkbook workbook = new();

            IXLWorksheet purchaseWorksheet = workbook.Worksheets.Add("Purchases");
            AddTransactionToWorksheet(purchaseWorksheet, MainMenu_Form.Instance.Purchase_DataGridView, currencyCode, currencySymbol);

            IXLWorksheet salesWorksheet = workbook.Worksheets.Add("Sales");
            AddTransactionToWorksheet(salesWorksheet, MainMenu_Form.Instance.Sale_DataGridView, currencyCode, currencySymbol);

            IXLWorksheet purchaseProductsWorksheet = workbook.Worksheets.Add("Purchase products");
            AddProductsToWorksheet(purchaseProductsWorksheet, MainMenu_Form.Instance.CategoryPurchaseList);

            IXLWorksheet saleProductsWorksheet = workbook.Worksheets.Add("Sale products");
            AddProductsToWorksheet(saleProductsWorksheet, MainMenu_Form.Instance.CategorySaleList);

            IXLWorksheet companiesWorksheet = workbook.Worksheets.Add("Companies");
            AddCompaniesToWorksheet(companiesWorksheet);

            IXLWorksheet accountantsWorksheet = workbook.Worksheets.Add("Accountants");
            AddAccountantsToWorksheet(accountantsWorksheet);

            // Save the file
            workbook.SaveAs(filePath);
        }
        private static string ParseCurrencyCode(string currencySelection)
        {
            // Extract currency code (first 3 characters) (e.g., "USD ($)" -> "USD")
            string[] parts = currencySelection.Split(' ');
            if (parts.Length > 0 && parts[0].Length >= 3)
            {
                return parts[0].Substring(0, 3).ToUpperInvariant();
            }

            return "USD";  // Default fallback
        }
        private static void AddTransactionToWorksheet(IXLWorksheet worksheet, DataGridView dataGridView, string targetCurrency, string currencySymbol)
        {
            // Add headers and format them
            int excelColumnIndex = 1;
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                // Skip the "Has Receipt" column
                if (dataGridView.Columns[i].Name == ReadOnlyVariables.HasReceipt_column)
                {
                    continue;
                }

                // Skip the country and company columns because they already exist in the product sheets
                if (dataGridView.Columns[i].Name == ReadOnlyVariables.Country_column
                    || dataGridView.Columns[i].Name == ReadOnlyVariables.Company_column)
                {
                    continue;
                }

                IXLCell cell = worksheet.Cell(1, excelColumnIndex);

                string headerText = GetColumnHeaderText(dataGridView.Columns[i]);

                cell.Value = headerText;
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightBlue;
                excelColumnIndex++;
            }

            // Add return-related headers
            int returnColumnsStartIndex = excelColumnIndex;
            string[] returnHeaders = ["Is Returned", "Return Date", "Return Reason", "Returned By", "Returned Items"];

            foreach (string header in returnHeaders)
            {
                IXLCell cell = worksheet.Cell(1, excelColumnIndex);
                cell.Value = header;
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightBlue;
                excelColumnIndex++;
            }

            // Add loss-related headers
            int lossColumnsStartIndex = excelColumnIndex;
            string[] lossHeaders = ["Is Lost", "Lost Date", "Lost Reason", "Lost By", "Lost Items"];

            foreach (string header in lossHeaders)
            {
                IXLCell cell = worksheet.Cell(1, excelColumnIndex);
                cell.Value = header;
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightBlue;
                excelColumnIndex++;
            }

            int receiptCellIndex = excelColumnIndex;

            // Add header for the receipt column
            worksheet.Cell(1, receiptCellIndex).Value = "Receipt";
            worksheet.Cell(1, receiptCellIndex).Style.Font.Bold = true;
            worksheet.Cell(1, receiptCellIndex).Style.Fill.BackgroundColor = XLColor.LightBlue;

            // Add currency message
            int messageCellIndex = receiptCellIndex + 2;
            string currencyMessage = targetCurrency == "USD" ? "All prices in USD" : $"All prices in {targetCurrency}";
            worksheet.Cell(1, messageCellIndex).Value = currencyMessage;
            worksheet.Cell(1, messageCellIndex).Style.Font.Bold = true;

            string currencyFormatPattern = $"\"{currencySymbol}\"#,##0.00";
            int currentRow = 2;

            // Add transactions
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                string receiptFileName = ReadOnlyVariables.EmptyCell;
                int rowForReceipt = currentRow;

                switch (row.Tag)
                {
                    case (List<string> itemList, TagData tagData) when itemList.Count > 0:
                        // Check if there's a receipt in the item list
                        byte receiptOffset = 0;
                        string receipt = itemList[^1];
                        if (receipt.StartsWith(ReadOnlyVariables.Receipt_text))
                        {
                            receiptOffset = 1;
                            receiptFileName = Path.GetFileName(receipt);
                        }

                        AddRowToWorksheet(worksheet, row, currentRow, tagData, targetCurrency, currencyFormatPattern, returnColumnsStartIndex, lossColumnsStartIndex);

                        // Add items in transaction if they exist in itemList
                        for (int i = 0; i < itemList.Count - receiptOffset; i++)
                        {
                            currentRow++;
                            string[] values = itemList[i].Split(',');

                            AddItemRowToWorksheet(worksheet, values, currentRow, targetCurrency, currencyFormatPattern);
                        }
                        break;

                    case (string tagString, TagData tagData):
                        AddRowToWorksheet(worksheet, row, currentRow, tagData, targetCurrency, currencyFormatPattern, returnColumnsStartIndex, lossColumnsStartIndex);

                        if (!string.IsNullOrEmpty(tagString))
                        {
                            receiptFileName = Path.GetFileName(tagString);
                        }
                        break;

                    case TagData tagData:
                        // Transaction with no items and no receipt
                        AddRowToWorksheet(worksheet, row, currentRow, tagData, targetCurrency, currencyFormatPattern, returnColumnsStartIndex, lossColumnsStartIndex);
                        break;
                }

                // Add receipt to the main transaction row (not item rows)
                worksheet.Cell(rowForReceipt, receiptCellIndex).Value = receiptFileName;

                currentRow++;
            }

            worksheet.Columns().AdjustToContents();
        }
        private static void AddRowToWorksheet(
            IXLWorksheet worksheet,
            DataGridViewRow row,
            int currentRow,
            TagData tagData,
            string targetCurrency,
            string currencyFormatPattern,
            int returnColumnsStartIndex,
            int lossColumnsStartIndex)
        {
            int excelColumnIndex = 1;

            // Get exchange rate from USD to target currency
            string transactionDate = row.Cells[6].Value?.ToString() ?? Tools.FormatDate(DateTime.Today);
            decimal exchangeRate = GetExchangeRateForExport(transactionDate, targetCurrency);

            // Skip the Notes column - it will be handled separately
            int notesColumnIndex = row.Cells[ReadOnlyVariables.Note_column].ColumnIndex;

            // Skip country and company columns becausethey are already in the product data
            int countryColumnIndex = row.Cells[ReadOnlyVariables.Country_column].ColumnIndex;
            int companyColumnIndex = row.Cells[ReadOnlyVariables.Company_column].ColumnIndex;

            for (int i = 0; i < row.Cells.Count; i++)
            {
                if (i == notesColumnIndex)
                {
                    break;
                }
                if (i == countryColumnIndex || i == companyColumnIndex)
                {
                    continue;
                }

                IXLCell excelCell = worksheet.Cell(currentRow, excelColumnIndex);

                if (tagData != null && i >= 8 && i <= 14)
                {
                    (decimal usdValue, bool useEmpty) = i switch
                    {
                        8 => (tagData.PricePerUnitUSD, tagData.PricePerUnitUSD == 0),
                        9 => (tagData.ShippingUSD, false),
                        10 => (tagData.TaxUSD, false),
                        11 => (tagData.FeeUSD, false),
                        12 => (tagData.DiscountUSD, false),
                        13 => (tagData.ChargedDifferenceUSD, false),
                        14 => (tagData.ChargedOrCreditedUSD, false),
                        _ => (0, false)
                    };

                    if (useEmpty)
                    {
                        excelCell.Value = ReadOnlyVariables.EmptyCell;

                        // Check if this is the price per unit column (index 8) for multiple items
                        if (i == 8 && row.Cells[i].Value?.ToString() == ReadOnlyVariables.EmptyCell)
                        {
                            excelCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        }
                    }
                    else
                    {
                        // Convert from USD to target currency
                        decimal convertedValue = Math.Round(usdValue * exchangeRate, 2, MidpointRounding.AwayFromZero);
                        excelCell.Value = convertedValue;
                        excelCell.Style.NumberFormat.Format = currencyFormatPattern;
                    }
                }
                else
                {
                    // Handle other cell types
                    object? cellValue = row.Cells[i].Value;

                    // Special handling for quantity column (index 7) to ensure it's treated as numeric
                    if (i == 7)
                    {
                        if (cellValue != null && decimal.TryParse(cellValue.ToString(), out decimal quantityValue))
                        {
                            excelCell.Value = quantityValue;
                        }
                        else
                        {
                            excelCell.Value = cellValue?.ToString();
                        }
                        excelCell.Style.NumberFormat.Format = _numberFormatPattern;
                    }
                    else
                    {
                        excelCell.Value = cellValue?.ToString();
                    }
                }

                excelColumnIndex++;
            }

            // Handle the Notes column
            DataGridViewCell notesCell = row.Cells[ReadOnlyVariables.Note_column];
            string? notesCellValue = notesCell.Value?.ToString();
            IXLCell notesExcelCell = worksheet.Cell(currentRow, excelColumnIndex);

            notesExcelCell.Value = notesCellValue == ReadOnlyVariables.EmptyCell
                ? ReadOnlyVariables.EmptyCell
                : (notesCellValue == ReadOnlyVariables.Show_text && notesCell.Tag != null)
                    ? notesCell.Tag.ToString()
                    : notesCellValue;

            AddReturnDataToWorksheet(worksheet, currentRow, returnColumnsStartIndex, tagData, row);
            AddLossDataToWorksheet(worksheet, currentRow, lossColumnsStartIndex, tagData, row);
        }
        private static void AddReturnDataToWorksheet(IXLWorksheet worksheet, int currentRow, int startColumnIndex, TagData tagData, DataGridViewRow row)
        {
            int columnIndex = startColumnIndex;

            if (tagData != null)
            {
                // Is Returned column
                IXLCell isReturnedCell = worksheet.Cell(currentRow, columnIndex++);
                if (tagData.IsReturned)
                {
                    isReturnedCell.Value = "Yes";
                    isReturnedCell.Style.Font.FontColor = XLColor.Red;
                    isReturnedCell.Style.Font.Bold = true;
                }
                else if (tagData.IsPartiallyReturned)
                {
                    isReturnedCell.Value = "Partial";
                    isReturnedCell.Style.Font.FontColor = XLColor.Orange;
                    isReturnedCell.Style.Font.Bold = true;
                }
                else
                {
                    isReturnedCell.Value = "No";
                }

                // Return Date column
                IXLCell returnDateCell = worksheet.Cell(currentRow, columnIndex++);
                if (tagData.ReturnDate.HasValue)
                {
                    returnDateCell.Value = tagData.ReturnDate.Value.ToString("yyyy-MM-dd");
                }
                else
                {
                    returnDateCell.Value = ReadOnlyVariables.EmptyCell;
                }

                // Return Reason column
                IXLCell returnReasonCell = worksheet.Cell(currentRow, columnIndex++);
                returnReasonCell.Value = string.IsNullOrEmpty(tagData.ReturnReason) ? ReadOnlyVariables.EmptyCell : tagData.ReturnReason;

                // Returned By column
                IXLCell returnedByCell = worksheet.Cell(currentRow, columnIndex++);
                returnedByCell.Value = string.IsNullOrEmpty(tagData.ReturnedBy) ? ReadOnlyVariables.EmptyCell : tagData.ReturnedBy;

                // Returned Items column (for partial returns)
                IXLCell returnedItemsCell = worksheet.Cell(currentRow, columnIndex++);
                if (tagData.IsPartiallyReturned && tagData.ReturnedItems != null && tagData.ReturnedItems.Count > 0)
                {
                    // Get the actual item names that were returned
                    List<string> returnedItemNames = GetReturnedItemNamesFromTransaction(row, tagData.ReturnedItems);
                    returnedItemsCell.Value = string.Join("; ", returnedItemNames);
                }
                else if (tagData.IsReturned)
                {
                    returnedItemsCell.Value = "All items";
                }
                else
                {
                    returnedItemsCell.Value = ReadOnlyVariables.EmptyCell;
                }
            }
            else
            {
                // No TagData, fill with empty values
                for (int i = 0; i < 5; i++)
                {
                    worksheet.Cell(currentRow, columnIndex++).Value = ReadOnlyVariables.EmptyCell;
                }
            }
        }
        private static void AddLossDataToWorksheet(IXLWorksheet worksheet, int currentRow, int startColumnIndex, TagData tagData, DataGridViewRow row)
        {
            int columnIndex = startColumnIndex;

            if (tagData != null)
            {
                // Is Lost column
                IXLCell isLostCell = worksheet.Cell(currentRow, columnIndex++);
                if (tagData.IsLost)
                {
                    isLostCell.Value = "Yes";
                    isLostCell.Style.Font.FontColor = XLColor.DarkRed;
                    isLostCell.Style.Font.Bold = true;
                }
                else if (tagData.IsPartiallyLost)
                {
                    isLostCell.Value = "Partial";
                    isLostCell.Style.Font.FontColor = XLColor.DarkOrange;
                    isLostCell.Style.Font.Bold = true;
                }
                else
                {
                    isLostCell.Value = "No";
                }

                // Lost Date column
                IXLCell lostDateCell = worksheet.Cell(currentRow, columnIndex++);
                if (tagData.LostDate.HasValue)
                {
                    lostDateCell.Value = tagData.LostDate.Value.ToString("yyyy-MM-dd");
                }
                else
                {
                    lostDateCell.Value = ReadOnlyVariables.EmptyCell;
                }

                // Lost Reason, Lost By, and Lost Items columns
                worksheet.Cell(currentRow, columnIndex++).Value = string.IsNullOrEmpty(tagData.LostReason) ? ReadOnlyVariables.EmptyCell : tagData.LostReason;
                worksheet.Cell(currentRow, columnIndex++).Value = string.IsNullOrEmpty(tagData.LostBy) ? ReadOnlyVariables.EmptyCell : tagData.LostBy;

                // Lost Items column (similar to returned items)
                IXLCell lostItemsCell = worksheet.Cell(currentRow, columnIndex++);
                if (tagData.IsPartiallyLost && tagData.LostItems != null && tagData.LostItems.Count > 0)
                {
                    List<string> lostItemNames = GetLostItemNamesFromTransaction(row, tagData.LostItems);
                    lostItemsCell.Value = string.Join("; ", lostItemNames);
                }
                else if (tagData.IsLost)
                {
                    lostItemsCell.Value = "All items";
                }
                else
                {
                    lostItemsCell.Value = ReadOnlyVariables.EmptyCell;
                }
            }
            else
            {
                // No TagData, fill with empty values
                for (int i = 0; i < 5; i++)
                {
                    worksheet.Cell(currentRow, columnIndex++).Value = ReadOnlyVariables.EmptyCell;
                }
            }
        }
        private static List<string> GetLostItemNamesFromTransaction(DataGridViewRow row, List<int> lostItemIndices)
        {
            List<string> itemNames = [];

            if (row.Tag is (List<string> items, TagData _))
            {
                foreach (int itemIndex in lostItemIndices)
                {
                    if (itemIndex < items.Count && !items[itemIndex].StartsWith(ReadOnlyVariables.Receipt_text))
                    {
                        string[] itemDetails = items[itemIndex].Split(',');
                        if (itemDetails.Length > 0)
                        {
                            itemNames.Add(itemDetails[0]);  // Product name
                        }
                    }
                }
            }
            else
            {
                // Single item transaction
                string productName = row.Cells[ReadOnlyVariables.Product_column].Value?.ToString();
                if (!string.IsNullOrEmpty(productName))
                {
                    itemNames.Add(productName);
                }
            }

            return itemNames;
        }
        private static List<string> GetReturnedItemNamesFromTransaction(DataGridViewRow row, List<int> returnedItemIndices)
        {
            List<string> itemNames = [];

            if (row.Tag is (List<string> items, TagData _))
            {
                foreach (int itemIndex in returnedItemIndices)
                {
                    if (itemIndex < items.Count && !items[itemIndex].StartsWith(ReadOnlyVariables.Receipt_text))
                    {
                        string[] itemDetails = items[itemIndex].Split(',');
                        if (itemDetails.Length > 0)
                        {
                            itemNames.Add(itemDetails[0]);  // Product name
                        }
                    }
                }
            }
            else
            {
                // Single item transaction
                string productName = row.Cells[ReadOnlyVariables.Product_column].Value?.ToString();
                if (!string.IsNullOrEmpty(productName))
                {
                    itemNames.Add(productName);
                }
            }

            return itemNames;
        }
        private static void AddItemRowToWorksheet(IXLWorksheet worksheet, string[] row, int currentRow, string targetCurrency, string currencyFormatPattern)
        {
            // Get transaction date from the main row for exchange rate calculation
            string transactionDate = worksheet.Cell(currentRow - GetItemOffsetForTransaction(worksheet, currentRow), 7).Value.ToString()
                ?? Tools.FormatDate(DateTime.Today);
            decimal exchangeRate = GetExchangeRateForExport(transactionDate, targetCurrency);

            for (int i = 0; i < row.Length - 1; i++)  // Skip the total value with - 1
            {
                // Skip country and company columns because they are already in the product data
                if (i == 2 || i == 3)
                {
                    continue;
                }

                // Shift the data one column to the left after the date column
                int columnIndex = i < 4 ? i : i - 1;
                IXLCell excelCell = worksheet.Cell(currentRow, columnIndex + 3);

                string cellValue = row[i];

                // Check if this should be a numeric value (quantity, price, etc.)
                if (i == 4 || i == 5)  // quantity and price columns
                {
                    if (decimal.TryParse(cellValue, out decimal numericValue))
                    {
                        if (i == 5)  // price column - convert from USD to target currency
                        {
                            decimal convertedPrice = Math.Round(numericValue * exchangeRate, 2, MidpointRounding.AwayFromZero);
                            excelCell.Value = convertedPrice;
                            excelCell.Style.NumberFormat.Format = currencyFormatPattern;
                        }
                        else  // quantity column
                        {
                            excelCell.Value = numericValue;
                            excelCell.Style.NumberFormat.Format = _numberFormatPattern;
                        }
                    }
                    else
                    {
                        excelCell.Value = cellValue;
                    }
                }
                else
                {
                    excelCell.Value = cellValue;
                }
            }
        }
        private static int GetItemOffsetForTransaction(IXLWorksheet worksheet, int currentRow)
        {
            // Look backwards to find how many item rows we've added for this transaction
            int offset = 0;
            for (int row = currentRow - 1; row >= 2; row--)
            {
                // If we find a row with a transaction ID (first column not empty), we've found the main transaction row
                if (!string.IsNullOrEmpty(worksheet.Cell(row, 1).Value.ToString()))
                {
                    break;
                }
                offset++;
            }
            return offset;
        }
        private static decimal GetExchangeRateForExport(string transactionDate, string targetCurrency)
        {
            if (targetCurrency == "USD")
            {
                return 1.0m;  // No conversion needed
            }

            try
            {
                // Parse date to ensure correct format
                if (DateTime.TryParse(transactionDate, out DateTime parsedDate))
                {
                    string formattedDate = Tools.FormatDate(parsedDate);
                    decimal rate = Currency.GetExchangeRate("USD", targetCurrency, formattedDate, false);
                    return rate > 0 ? rate : 1.0m;  // Fallback to 1:1 if rate fetch fails
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't stop the export
                Log.WriteWithFormat(1, "Failed to get exchange rate for {0}: {1}", targetCurrency, ex.Message);
            }

            return 1.0m;  // Fallback to 1:1 ratio
        }
        private static void AddCompaniesToWorksheet(IXLWorksheet worksheet)
        {
            worksheet.Cell(1, 1).Value = "Company name";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;

            int currentRow = 2;
            foreach (string company in MainMenu_Form.Instance.CompanyList)
            {
                worksheet.Cell(currentRow, 1).Value = company;
                currentRow++;
            }

            worksheet.Columns().AdjustToContents();
        }
        private static void AddAccountantsToWorksheet(IXLWorksheet worksheet)
        {
            worksheet.Cell(1, 1).Value = "Accountant name";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.LightBlue;

            int currentRow = 2;
            foreach (string accountant in MainMenu_Form.Instance.AccountantList)
            {
                worksheet.Cell(currentRow, 1).Value = accountant;
                currentRow++;
            }

            worksheet.Columns().AdjustToContents();
        }
        private static void AddProductsToWorksheet(IXLWorksheet worksheet, List<Category> list)
        {
            worksheet.Cell(1, 1).Value = "Product ID";
            worksheet.Cell(1, 2).Value = "Product name";
            worksheet.Cell(1, 3).Value = "Category";
            worksheet.Cell(1, 4).Value = "Country of origin";
            worksheet.Cell(1, 5).Value = "Company of origin";

            // Format title cells
            for (int i = 1; i <= 5; i++)
            {
                worksheet.Cell(1, i).Style.Font.Bold = true;
                worksheet.Cell(1, i).Style.Fill.BackgroundColor = XLColor.LightBlue;
            }

            int currentRow = 2;
            foreach (Category category in list)
            {
                foreach (Product product in category.ProductList)
                {
                    worksheet.Cell(currentRow, 1).Value = product.ProductID;
                    worksheet.Cell(currentRow, 2).Value = product.Name;
                    worksheet.Cell(currentRow, 3).Value = category.Name;
                    worksheet.Cell(currentRow, 4).Value = product.CountryOfOrigin;
                    worksheet.Cell(currentRow, 5).Value = product.CompanyOfOrigin;
                    currentRow++;
                }
            }

            worksheet.Columns().AdjustToContents();
        }
    }
}
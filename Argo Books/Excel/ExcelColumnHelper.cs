using ClosedXML.Excel;
using Argo_Books;

namespace Argo_Books.Excel
{
    /// <summary>
    /// Static helper for matching Excel column headers to DataGridView columns using flexible matching patterns and validation.
    /// </summary>
    public static partial class ExcelColumnHelper
    {
        // Match Excel headers to DataGridView columns
        /// <summary>
        /// Performs flexible matching for column headers - extended to handle all form types.
        /// </summary>
        public static bool IsFlexibleMatch(string excelHeader, Enum columnType)
        {
            excelHeader = excelHeader.ToLower();

            return columnType switch
            {
                MainMenu_Form.Column.ID => IsIdMatch(excelHeader),
                MainMenu_Form.Column.Accountant => IsAccountantMatch(excelHeader),
                MainMenu_Form.Column.Product => IsProductMatch(excelHeader),
                MainMenu_Form.Column.Category => IsCategoryMatch(excelHeader),
                MainMenu_Form.Column.Country => IsCountryMatch(excelHeader),
                MainMenu_Form.Column.Company => IsCompanyMatch(excelHeader),
                MainMenu_Form.Column.Date => IsDateMatch(excelHeader),
                MainMenu_Form.Column.TotalItems => IsTotalItemsMatch(excelHeader),
                MainMenu_Form.Column.PricePerUnit => IsPricePerUnitMatch(excelHeader),
                MainMenu_Form.Column.Shipping => IsShippingMatch(excelHeader),
                MainMenu_Form.Column.Tax => IsTaxMatch(excelHeader),
                MainMenu_Form.Column.Fee => IsFeeMatch(excelHeader),
                MainMenu_Form.Column.Discount => IsDiscountMatch(excelHeader),
                MainMenu_Form.Column.ChargedDifference => IsChargedDifferenceMatch(excelHeader),
                MainMenu_Form.Column.Total => IsTotalMatch(excelHeader),
                MainMenu_Form.Column.Note => IsNoteMatch(excelHeader),

                MainMenu_Form.Column.IsReturned => IsReturnedMatch(excelHeader),
                MainMenu_Form.Column.ReturnDate => IsReturnDateMatch(excelHeader),
                MainMenu_Form.Column.ReturnReason => IsReturnReasonMatch(excelHeader),
                MainMenu_Form.Column.ReturnedBy => IsReturnedByMatch(excelHeader),
                MainMenu_Form.Column.ReturnedItems => IsReturnedItemsMatch(excelHeader),
                MainMenu_Form.Column.Receipt => IsReceiptMatch(excelHeader),

                MainMenu_Form.Column.IsLost => IsLostMatch(excelHeader),
                MainMenu_Form.Column.LostDate => IsLostDateMatch(excelHeader),
                MainMenu_Form.Column.LostReason => IsLostReasonMatch(excelHeader),
                MainMenu_Form.Column.LostBy => IsLostByMatch(excelHeader),
                MainMenu_Form.Column.LostItems => IsLostItemsMatch(excelHeader),

                Accountants_Form.Column.AccountantName => IsAccountantNameMatch(excelHeader),

                Companies_Form.Column.Company => IsCompanyNameMatch(excelHeader),

                Products_Form.Column.ProductID => IsProductIdMatch(excelHeader),
                Products_Form.Column.ProductName => IsProductNameMatch(excelHeader),
                Products_Form.Column.ProductCategory => IsCategoryMatch(excelHeader),
                Products_Form.Column.CountryOfOrigin => IsCountryMatch(excelHeader),
                Products_Form.Column.CompanyOfOrigin => IsCompanyMatch(excelHeader),

                _ => HandleUnknownColumnType(columnType)
            };
        }
        private static bool IsIdMatch(string excel)
        {
            return excel.Contains("id") || excel.Contains("number") || excel.Contains('#') ||
                excel.Contains("order") || excel.Contains("sale") || excel.Contains("purchase");
        }
        private static bool IsProductMatch(string excel)
        {
            return excel.Contains("product") || excel.Contains("service");
        }
        private static bool IsCategoryMatch(string excel)
        {
            return excel.Contains("category") || excel.Contains("group");
        }
        private static bool IsCountryMatch(string excel)
        {
            return excel.Contains("country") || excel.Contains("countries");
        }
        private static bool IsCompanyMatch(string excel)
        {
            return excel.Contains("company") || excel.Contains("companies") ||
                excel.Contains("manufacturer") || excel.Contains("supplier") ||
                excel.Contains("vendor");
        }
        private static bool IsDateMatch(string excel)
        {
            return excel.Contains("date");
        }
        private static bool IsTotalItemsMatch(string excel)
        {
            return excel.Contains("item") || excel.Contains("quantity") ||
                excel.Contains("qty") || excel.Contains("amount");
        }
        private static bool IsPricePerUnitMatch(string excel)
        {
            return (excel.Contains("price") && excel.Contains("unit")) ||
                (excel.Contains("cost") && excel.Contains("unit")) ||
                (excel.Contains("price") && excel.Contains("each")) ||
                (excel.Contains("cost") && excel.Contains("each"));
        }
        private static bool IsNoteMatch(string excel)
        {
            return excel.Contains("note") || excel == "comment" ||
                excel.Contains("comments") || excel.Contains("remarks");
        }
        private static bool IsAccountantMatch(string excel)
        {
            return excel.Contains("accountant") || excel.Contains("cpa") ||
                excel.Contains("bookkeeper");
        }
        private static bool IsShippingMatch(string excel)
        {
            return excel.Contains("shipping") || excel.Contains("delivery") ||
                excel.Contains("freight") || excel.Contains("postage");
        }
        private static bool IsTaxMatch(string excel)
        {
            return excel.Contains("tax") || excel.Contains("vat") ||
                excel.Contains("gst") || excel.Contains("sales tax");
        }
        private static bool IsFeeMatch(string excel)
        {
            return excel.Contains("fee") || excel.Contains("charge") ||
                excel.Contains("service fee");
        }
        private static bool IsDiscountMatch(string excel)
        {
            return excel.Contains("discount") || excel.Contains("rebate") ||
                excel.Contains("reduction") || excel.Contains("deduction");
        }
        private static bool IsChargedDifferenceMatch(string excel)
        {
            return (excel.Contains("charged") && excel.Contains("difference")) ||
                excel.Contains("variance") || excel.Contains("adjustment") ||
                excel.Contains("difference");
        }
        private static bool IsTotalMatch(string excel)
        {
            return (excel.Contains("total") || excel.Contains("sum") ||
                excel.Contains("amount") || excel.Contains("expense") ||
                excel.Contains("revenue"))
                && !IsTotalItemsMatch(excel);
        }
        private static bool IsAccountantNameMatch(string excel)
        {
            return excel.Contains("accountant") || excel.Contains("name") ||
                excel.Contains("cpa") || excel.Contains("bookkeeper");
        }
        private static bool IsCompanyNameMatch(string excel)
        {
            return excel.Contains("company") || excel.Contains("companies") ||
                excel.Contains("name") || excel.Contains("business") ||
                excel.Contains("organization") || excel.Contains("corp") ||
                excel.Contains("corporation");
        }
        private static bool IsProductIdMatch(string excel)
        {
            return excel.Contains("id") || excel.Contains("sku") ||
                excel.Contains("code") || excel.Contains('#');
        }
        private static bool IsProductNameMatch(string excel)
        {
            return (excel.Contains("product") || excel.Contains("service"))
                && !IsProductIdMatch(excel);
        }
        private static bool IsReturnedMatch(string excel)
        {
            return excel.Contains("return") && (excel.Contains("is") || excel.Contains("status"));
        }
        private static bool IsReturnDateMatch(string excel)
        {
            return excel.Contains("return") && excel.Contains("date");
        }
        private static bool IsReturnReasonMatch(string excel)
        {
            return excel.Contains("return") && excel.Contains("reason");
        }
        private static bool IsReturnedByMatch(string excel)
        {
            return excel.Contains("return") && (excel.Contains("by") || excel.Contains("who"));
        }
        private static bool IsReturnedItemsMatch(string excel)
        {
            return excel.Contains("return") && excel.Contains("item");
        }
        private static bool IsReceiptMatch(string excel)
        {
            return excel.Contains("receipt") || excel.Contains("file") ||
                excel.Contains("attachment") || excel.Contains("document");
        }
        private static bool HandleUnknownColumnType(Enum columnType)
        {
            CustomMessageBox.ShowWithFormat(
                "Column Validation Error",
                "An unexpected column type was encountered during import. The spreadsheet structure may have changed after validation.\n\nColumn type: {0}\n\nThe import operation will be cancelled.",
                CustomMessageBoxIcon.Error,
                CustomMessageBoxButtons.Ok,
                columnType.ToString());

            return false;
        }
        private static bool IsLostMatch(string excel)
        {
            return excel.Contains("lost") && (excel.Contains("is") || excel.Contains("status"));
        }
        private static bool IsLostDateMatch(string excel)
        {
            return excel.Contains("lost") && excel.Contains("date");
        }
        private static bool IsLostReasonMatch(string excel)
        {
            return excel.Contains("lost") && excel.Contains("reason");
        }
        private static bool IsLostByMatch(string excel)
        {
            return excel.Contains("lost") && (excel.Contains("by") || excel.Contains("who"));
        }
        private static bool IsLostItemsMatch(string excel)
        {
            return excel.Contains("lost") && excel.Contains("item");
        }

        // Main methods
        /// <summary>
        /// Gets a cell value by column type using standard column headers.
        /// </summary>
        public static string GetCellValue(IXLRow row, Enum column)
        {
            IXLWorksheet worksheet = row.Worksheet;
            IXLRow headerRow = worksheet.RowsUsed().First();

            // Find the column index that matches our enum
            int columnIndex = FindColumnIndex(headerRow, column);

            if (columnIndex == -1)
            {
                return "";  // Column not found
            }

            // Get the cell value at that column index
            IXLCell cell = row.Cell(columnIndex);
            return cell.GetValue<string>()?.Trim() ?? "";
        }

        /// <summary>
        /// Finds the column index for a given enum column in the header row using display text.
        /// </summary>
        private static int FindColumnIndex(IXLRow headerRow, Enum column, Dictionary<Enum, string> columnHeaders = null)
        {
            string standardHeader = columnHeaders?.TryGetValue(column, out string displayText) == true
                ? displayText
                : column.ToString();

            IXLCells headerCells = headerRow.CellsUsed();

            int columnIndex = 1;  // Excel columns are 1-based

            foreach (IXLCell headerCell in headerCells)
            {
                string excelHeader = headerCell.GetValue<string>()?.Trim();

                if (string.IsNullOrEmpty(excelHeader))
                {
                    columnIndex++;
                    continue;
                }

                // First try exact match (case-insensitive)
                if (string.Equals(excelHeader, standardHeader, StringComparison.OrdinalIgnoreCase))
                {
                    return columnIndex;
                }

                if (IsFlexibleMatch(excelHeader, column))
                {
                    return columnIndex;
                }

                columnIndex++;
            }

            return -1;  // Column not found
        }

        /// <summary>
        /// Checks if a column exists by column type using flexible matching.
        /// </summary>
        public static bool HasColumn(IXLWorksheet worksheet, Enum column, Dictionary<Enum, string> columnHeaders)
        {
            if (!worksheet.RowsUsed().Any())
            {
                return false;
            }

            // Get the standard header text from the dictionary
            if (!columnHeaders.TryGetValue(column, out string standardHeader))
            {
                return false;
            }

            List<string> columnsInWorksheet = GetColumnNamesInWorksheet(worksheet);

            // Check each column in the worksheet
            foreach (string excelHeader in columnsInWorksheet)
            {
                if (string.IsNullOrWhiteSpace(excelHeader))
                {
                    continue;
                }

                // First try exact match (case-insensitive)
                if (string.Equals(excelHeader, standardHeader, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (IsFlexibleMatch(excelHeader, column))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets column header names from worksheet's first row.
        /// </summary>
        private static List<string> GetColumnNamesInWorksheet(IXLWorksheet worksheet)
        {
            IXLCells columnCells = worksheet.RowsUsed().First().CellsUsed();
            List<string> columnNames = [];

            foreach (IXLCell? cell in columnCells)
            {
                string cellText = cell.GetValue<string>()?.Trim();
                if (!string.IsNullOrEmpty(cellText))
                {
                    columnNames.Add(cellText);
                }
            }

            return columnNames;
        }

        /// <summary>
        /// Checks if worksheet has any column headers.
        /// </summary>
        public static bool ContainsAnyColumns(IXLWorksheet worksheet)
        {
            return GetColumnNamesInWorksheet(worksheet).Count > 0;
        }

        /// <summary>
        /// Validates that required columns exist for simple list import (accountants/companies).
        /// </summary>
        public static ValidationResult ValidateSimpleListColumns(IXLWorksheet worksheet, Enum column, Dictionary<Enum, string> columnHeaders)
        {
            if (!HasColumn(worksheet, column, columnHeaders))
            {
                string displayText = columnHeaders.TryGetValue(column, out string text) ? text : column.ToString();
                return new ValidationResult([displayText]);
            }

            return new ValidationResult([]);
        }

        /// <summary>
        /// Validates that required columns exist for product import.
        /// </summary>
        public static ValidationResult ValidateProductColumns(IXLWorksheet worksheet)
        {
            List<Products_Form.Column> requiredColumns =
            [
                Products_Form.Column.ProductName,
                Products_Form.Column.ProductCategory
            ];

            List<string> missingColumns = [];
            Dictionary<Enum, string> columnHeaders = Products_Form.ColumnHeaders
               .ToDictionary(kvp => (Enum)kvp.Key, kvp => kvp.Value);

            foreach (Products_Form.Column column in requiredColumns)
            {
                if (!HasColumn(worksheet, column, columnHeaders))
                {
                    string displayText = columnHeaders.TryGetValue(column, out string text) ? text : column.ToString();
                    missingColumns.Add(displayText);
                }
            }

            return new ValidationResult(missingColumns);
        }

        /// <summary>
        /// Validates that required columns exist for transaction import.
        /// </summary>
        public static ValidationResult ValidateTransactionColumns(IXLWorksheet worksheet, bool isPurchase)
        {
            List<MainMenu_Form.Column> requiredColumns =
            [
                MainMenu_Form.Column.ID,
                MainMenu_Form.Column.Product,
                MainMenu_Form.Column.Category,
                MainMenu_Form.Column.Date,
                MainMenu_Form.Column.TotalItems,
                MainMenu_Form.Column.PricePerUnit,
                MainMenu_Form.Column.Total
            ];

            List<string> missingColumns = [];

            Dictionary<Enum, string> columnHeaders = isPurchase
                ? MainMenu_Form.Instance.PurchaseColumnHeaders.ToDictionary(kvp => (Enum)kvp.Key, kvp => kvp.Value)
                : MainMenu_Form.Instance.SalesColumnHeaders.ToDictionary(kvp => (Enum)kvp.Key, kvp => kvp.Value);

            foreach (MainMenu_Form.Column column in requiredColumns)
            {
                if (!HasColumn(worksheet, column, columnHeaders))
                {
                    string displayText = columnHeaders.TryGetValue(column, out string text) ? text : column.ToString();
                    missingColumns.Add(displayText);
                }
            }

            return new ValidationResult(missingColumns);
        }

        public class ValidationResult(List<string> missingColumns)
        {
            public List<string> MissingColumns { get; } = missingColumns;
            public bool IsValid { get => MissingColumns.Count == 0; }
        }
    }
}
using Argo_Books.DataClasses;
using Argo_Books.LostProduct;
using Argo_Books.ReportGenerator;
using Argo_Books.ReturnProduct;
using Argo_Books.Theme;
using Argo_Books.UI;
using Guna.UI2.WinForms;
using Argo_Books;
using Argo_Books.Language;
using Argo_Books.ReportGenerator.Menus;
using Argo_Books.Theme;
using System.Data;

namespace Argo_Books.ReportGenerator.Elements
{
    /// <summary>
    /// Data selection options for the transaction table.
    /// </summary>
    public enum TableDataSelection
    {
        All,             // Show all transactions within report filters
        TopByAmount,     // Top N highest value transactions
        BottomByAmount,  // Bottom N lowest value transactions  
        ReturnsOnly,     // Only returned items
        LossesOnly       // Only lost items
    }

    /// <summary>
    /// Sort order for table data.
    /// </summary>
    public enum TableSortOrder
    {
        DateDescending,
        DateAscending,
        AmountDescending,
        AmountAscending
    }

    /// <summary>
    /// Transaction table element for displaying tabular data.
    /// </summary>
    public class TableElement : BaseElement
    {
        // Data selection properties (General tab)
        public TransactionType TransactionType { get; set; } = TransactionType.Revenue;
        public bool IncludeReturns { get; set; } = true;
        public bool IncludeLosses { get; set; } = true;
        public TableDataSelection DataSelection { get; set; } = TableDataSelection.All;
        public TableSortOrder SortOrder { get; set; } = TableSortOrder.DateDescending;
        public int MaxRows { get; set; } = 10;

        // Display properties (Style tab)
        public bool ShowHeaders { get; set; } = true;
        public bool AlternateRowColors { get; set; } = true;
        public bool ShowGridLines { get; set; } = true;
        public bool ShowTotalsRow { get; set; } = false;
        public bool AutoSizeColumns { get; set; } = true;
        public float FontSize { get; set; } = 8f;
        public string FontFamily { get; set; } = "Segoe UI";
        public int DataRowHeight { get; set; } = 20;
        public int HeaderRowHeight { get; set; } = 25;
        public int CellPadding { get; set; } = 3;

        // Colors (Style tab)
        public Color HeaderBackgroundColor { get; set; } = Color.FromArgb(94, 148, 255);
        public Color HeaderTextColor { get; set; } = Color.White;
        public Color DataRowTextColor { get; set; } = Color.Black;
        public Color GridLineColor { get; set; } = Color.LightGray;
        public Color BaseRowColor { get; set; } = Color.White;
        public Color AlternateRowColor { get; set; } = Color.FromArgb(248, 248, 248);

        // Column visibility (Columns tab)
        public bool ShowDateColumn { get; set; } = true;
        public bool ShowTransactionIdColumn { get; set; } = true;
        public bool ShowCompanyColumn { get; set; } = true;
        public bool ShowProductColumn { get; set; } = true;
        public bool ShowQuantityColumn { get; set; } = true;
        public bool ShowUnitPriceColumn { get; set; } = false;
        public bool ShowTotalColumn { get; set; } = true;
        public bool ShowStatusColumn { get; set; } = false;
        public bool ShowAccountantColumn { get; set; } = false;
        public bool ShowShippingColumn { get; set; } = false;

        // Internal data structure for transactions
        private class TransactionData
        {
            public DateTime Date { get; set; }
            public string TransactionId { get; set; }
            public string CustomerSupplier { get; set; }
            public string Product { get; set; }
            public decimal Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Total { get; set; }
            public string Status { get; set; }
            public string Accountant { get; set; }
            public decimal Shipping { get; set; }
            public bool IsReturn { get; set; }
            public bool IsLoss { get; set; }
            public TransactionType Type { get; set; }
        }

        // Overrides
        public override byte MinimumSize => 80;
        public override string DisplayName => LanguageManager.TranslateString("transaction table");
        public override ReportElementType GetElementType() => ReportElementType.TransactionTable;
        public override BaseElement Clone()
        {
            return new TableElement
            {
                TransactionType = TransactionType,
                IncludeReturns = IncludeReturns,
                IncludeLosses = IncludeLosses,
                Id = Id,
                Bounds = Bounds,
                ZOrder = ZOrder,
                IsVisible = IsVisible,
                DataSelection = DataSelection,
                SortOrder = SortOrder,
                MaxRows = MaxRows,
                ShowHeaders = ShowHeaders,
                AlternateRowColors = AlternateRowColors,
                ShowGridLines = ShowGridLines,
                ShowTotalsRow = ShowTotalsRow,
                AutoSizeColumns = AutoSizeColumns,
                FontSize = FontSize,
                FontFamily = FontFamily,
                DataRowHeight = DataRowHeight,
                HeaderRowHeight = HeaderRowHeight,
                CellPadding = CellPadding,
                HeaderBackgroundColor = HeaderBackgroundColor,
                HeaderTextColor = HeaderTextColor,
                DataRowTextColor = DataRowTextColor,
                GridLineColor = GridLineColor,
                BaseRowColor = BaseRowColor,
                AlternateRowColor = AlternateRowColor,
                ShowDateColumn = ShowDateColumn,
                ShowTransactionIdColumn = ShowTransactionIdColumn,
                ShowCompanyColumn = ShowCompanyColumn,
                ShowProductColumn = ShowProductColumn,
                ShowQuantityColumn = ShowQuantityColumn,
                ShowUnitPriceColumn = ShowUnitPriceColumn,
                ShowTotalColumn = ShowTotalColumn,
                ShowStatusColumn = ShowStatusColumn,
                ShowAccountantColumn = ShowAccountantColumn,
                ShowShippingColumn = ShowShippingColumn
            };
        }
        public override void RenderElement(Graphics graphics, ReportConfiguration config, float renderScale)
        {
            try
            {
                List<TransactionData> transactions = GetFilteredTransactions(config);

                if (transactions == null || transactions.Count == 0)
                {
                    RenderNoDataMessage(graphics);
                    return;
                }

                RenderTable(graphics, transactions);
            }
            catch
            {
                RenderError(graphics);
            }
        }
        private List<TransactionData> GetFilteredTransactions(ReportConfiguration config)
        {
            List<TransactionData> allTransactions = [];

            // Get date range from filters
            DateTime startDate = config?.Filters?.StartDate ?? DateTime.MinValue;
            DateTime endDate = config?.Filters?.EndDate ?? DateTime.MaxValue;

            // Load sales transactions if needed
            if (TransactionType == TransactionType.Revenue)
            {
                DataGridView salesGrid = MainMenu_Form.Instance.Sale_DataGridView;
                allTransactions.AddRange(ExtractTransactionsFromGrid(salesGrid, startDate, endDate, TransactionType.Revenue, IncludeReturns, IncludeLosses));
            }

            // Load purchase transactions if needed  
            if (TransactionType == TransactionType.Expenses)
            {
                DataGridView purchaseGrid = MainMenu_Form.Instance.Purchase_DataGridView;
                allTransactions.AddRange(ExtractTransactionsFromGrid(purchaseGrid, startDate, endDate, TransactionType.Expenses, IncludeReturns, IncludeLosses));
            }

            // Sort transactions
            allTransactions = ApplyDataSelection(allTransactions);
            allTransactions = SortTransactions(allTransactions);

            // Limit to MaxRows
            if (allTransactions.Count > MaxRows)
            {
                allTransactions = allTransactions.Take(MaxRows).ToList();
            }

            return allTransactions;
        }
        private static List<TransactionData> ExtractTransactionsFromGrid(DataGridView grid, DateTime startDate, DateTime endDate, TransactionType type, bool includeReturns, bool includeLosses)
        {
            List<TransactionData> transactions = [];

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.Cells[ReadOnlyVariables.Date_column].Value == null)
                {
                    continue;
                }

                if (!DateTime.TryParse(row.Cells[ReadOnlyVariables.Date_column].Value.ToString(), out DateTime date))
                {
                    continue;
                }

                if (date < startDate || date > endDate)
                {
                    continue;
                }

                decimal total = 0;
                if (row.Cells[ReadOnlyVariables.Total_column].Value != null)
                {
                    if (!decimal.TryParse(row.Cells[ReadOnlyVariables.Total_column].Value.ToString(), out total))
                    {
                        total = 0;
                    }
                }

                // Use the existing managers to check return and loss status
                bool isReturn = ReturnManager.IsTransactionReturned(row);
                bool isLoss = LostManager.IsTransactionLost(row);

                // Check filters for returns and losses
                if (isReturn && !includeReturns)
                {
                    continue;
                }
                if (isLoss && !includeLosses)
                {
                    continue;
                }

                // Determine status text
                string status = "Normal";
                if (isReturn && isLoss)
                {
                    status = "Return/Loss";
                }
                else if (isReturn)
                {
                    bool isPartialReturn = ReturnManager.IsTransactionPartiallyReturned(row);
                    status = isPartialReturn ? "Partial Return" : "Return";
                }
                else if (isLoss)
                {
                    bool isPartialLoss = LostManager.IsTransactionPartiallyLost(row);
                    status = isPartialLoss ? "Partial Loss" : "Loss";
                }

                TransactionData transaction = new()
                {
                    Date = date,
                    TransactionId = row.Cells[ReadOnlyVariables.ID_column].Value?.ToString() ?? "",
                    CustomerSupplier = row.Cells[ReadOnlyVariables.Company_column].Value?.ToString() ?? "",
                    Product = row.Cells[ReadOnlyVariables.Product_column].Value?.ToString() ?? "",
                    Quantity = ParseDecimal(row.Cells[ReadOnlyVariables.TotalItems_column].Value),
                    UnitPrice = ParseDecimal(row.Cells[ReadOnlyVariables.PricePerUnit_column].Value),
                    Total = Math.Abs(total),
                    Status = status,
                    Accountant = row.Cells[ReadOnlyVariables.Accountant_column].Value?.ToString() ?? "",
                    Shipping = ParseDecimal(row.Cells[ReadOnlyVariables.Shipping_column].Value),
                    IsReturn = isReturn,
                    IsLoss = isLoss,
                    Type = type
                };

                transactions.Add(transaction);
            }

            return transactions;
        }
        private static decimal ParseDecimal(object value)
        {
            if (value == null) { return 0; }

            if (decimal.TryParse(value.ToString(), out decimal result))
            {
                return result;
            }
            return 0;
        }
        private List<TransactionData> ApplyDataSelection(List<TransactionData> transactions)
        {
            return DataSelection switch
            {
                TableDataSelection.TopByAmount => transactions.OrderByDescending(t => t.Total).ToList(),
                TableDataSelection.BottomByAmount => transactions.OrderBy(t => t.Total).ToList(),
                TableDataSelection.ReturnsOnly => transactions.Where(t => t.IsReturn).ToList(),
                TableDataSelection.LossesOnly => transactions.Where(t => t.IsLoss).ToList(),
                _ => transactions
            };
        }
        private List<TransactionData> SortTransactions(List<TransactionData> transactions)
        {
            return SortOrder switch
            {
                TableSortOrder.DateAscending => transactions.OrderBy(t => t.Date).ToList(),
                TableSortOrder.DateDescending => transactions.OrderByDescending(t => t.Date).ToList(),
                TableSortOrder.AmountAscending => transactions.OrderBy(t => t.Total).ToList(),
                TableSortOrder.AmountDescending => transactions.OrderByDescending(t => t.Total).ToList(),
                _ => transactions
            };
        }
        private void RenderTable(Graphics graphics, List<TransactionData> transactions)
        {
            using Font headerFont = new(FontFamily, FontSize, FontStyle.Bold);
            using Font dataFont = new(FontFamily, FontSize);
            using SolidBrush headerBgBrush = new(HeaderBackgroundColor);
            using SolidBrush headerTextBrush = new(HeaderTextColor);
            using SolidBrush dataBrush = new(DataRowTextColor);
            using SolidBrush alternateBrush = new(AlternateRowColor);
            using Pen gridPen = new(GridLineColor, 0.5f);

            // Draw base background for entire table bounds
            using (SolidBrush baseBgBrush = new(BaseRowColor))
            {
                graphics.FillRectangle(baseBgBrush, Bounds);
            }

            // Calculate column widths
            List<(string Header, float Width, bool Show, StringAlignment Align)> columns = GetColumnDefinitions(graphics, headerFont, dataFont, transactions);

            float totalWidth = columns.Where(c => c.Show).Sum(c => c.Width);
            float scaleFactor = Bounds.Width / totalWidth;

            // Scale columns to fit
            for (int i = 0; i < columns.Count; i++)
            {
                columns[i] = (columns[i].Header, columns[i].Width * scaleFactor, columns[i].Show, columns[i].Align);
            }

            float currentY = Bounds.Y;
            float currentX = Bounds.X;

            // Draw header if enabled
            if (ShowHeaders)
            {
                graphics.FillRectangle(headerBgBrush, Bounds.X, currentY, Bounds.Width, HeaderRowHeight);

                currentX = Bounds.X;
                foreach ((string Header, float Width, bool Show, StringAlignment Align) in columns.Where(c => c.Show))
                {
                    RectangleF headerRect = new(currentX, currentY, Width, HeaderRowHeight);
                    DrawCellText(graphics, Header, headerRect, headerFont, headerTextBrush, StringAlignment.Center);

                    if (ShowGridLines)
                    {
                        graphics.DrawRectangle(gridPen, headerRect.X, headerRect.Y, headerRect.Width, headerRect.Height);
                    }

                    currentX += Width;
                }

                currentY += HeaderRowHeight;
            }

            // Draw data rows
            int rowIndex = 0;
            foreach (TransactionData transaction in transactions)
            {
                // Alternate row colors
                if (AlternateRowColors && rowIndex % 2 == 1)
                {
                    graphics.FillRectangle(alternateBrush, Bounds.X, currentY, Bounds.Width, DataRowHeight);
                }
                else
                {
                    using SolidBrush bgBrush = new(BaseRowColor);
                    graphics.FillRectangle(bgBrush, Bounds.X, currentY, Bounds.Width, DataRowHeight);
                }

                currentX = Bounds.X;
                int colIndex = 0;

                // Draw each column
                if (ShowDateColumn && columns[colIndex].Show)
                {
                    RectangleF cellRect = new(currentX, currentY, columns[colIndex].Width, DataRowHeight);
                    DrawCellText(graphics, transaction.Date.ToShortDateString(), cellRect, dataFont, dataBrush, columns[colIndex].Align);
                    if (ShowGridLines) { graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height); }
                    currentX += columns[colIndex].Width;
                }
                colIndex++;

                if (ShowTransactionIdColumn && columns[colIndex].Show)
                {
                    RectangleF cellRect = new(currentX, currentY, columns[colIndex].Width, DataRowHeight);
                    DrawCellText(graphics, transaction.TransactionId, cellRect, dataFont, dataBrush, columns[colIndex].Align);
                    if (ShowGridLines) { graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height); }
                    currentX += columns[colIndex].Width;
                }
                colIndex++;

                if (ShowCompanyColumn && columns[colIndex].Show)
                {
                    RectangleF cellRect = new(currentX, currentY, columns[colIndex].Width, DataRowHeight);
                    DrawCellText(graphics, transaction.CustomerSupplier, cellRect, dataFont, dataBrush, columns[colIndex].Align);
                    if (ShowGridLines) { graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height); }
                    currentX += columns[colIndex].Width;
                }
                colIndex++;

                if (ShowProductColumn && columns[colIndex].Show)
                {
                    RectangleF cellRect = new(currentX, currentY, columns[colIndex].Width, DataRowHeight);
                    DrawCellText(graphics, transaction.Product, cellRect, dataFont, dataBrush, columns[colIndex].Align);
                    if (ShowGridLines) { graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height); }
                    currentX += columns[colIndex].Width;
                }
                colIndex++;

                if (ShowQuantityColumn && columns[colIndex].Show)
                {
                    RectangleF cellRect = new(currentX, currentY, columns[colIndex].Width, DataRowHeight);
                    DrawCellText(graphics, transaction.Quantity.ToString("N0"), cellRect, dataFont, dataBrush, columns[colIndex].Align);
                    if (ShowGridLines) { graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height); }
                    currentX += columns[colIndex].Width;
                }
                colIndex++;

                if (ShowUnitPriceColumn && columns[colIndex].Show)
                {
                    RectangleF cellRect = new(currentX, currentY, columns[colIndex].Width, DataRowHeight);
                    DrawCellText(graphics, $"${transaction.UnitPrice:N2}", cellRect, dataFont, dataBrush, columns[colIndex].Align);
                    if (ShowGridLines) { graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height); }
                    currentX += columns[colIndex].Width;
                }
                colIndex++;

                if (ShowTotalColumn && columns[colIndex].Show)
                {
                    RectangleF cellRect = new(currentX, currentY, columns[colIndex].Width, DataRowHeight);
                    DrawCellText(graphics, $"${transaction.Total:N2}", cellRect, dataFont, dataBrush, columns[colIndex].Align);
                    if (ShowGridLines) { graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height); }
                    currentX += columns[colIndex].Width;
                }
                colIndex++;

                if (ShowStatusColumn && columns[colIndex].Show)
                {
                    RectangleF cellRect = new(currentX, currentY, columns[colIndex].Width, DataRowHeight);
                    DrawCellText(graphics, transaction.Status, cellRect, dataFont, dataBrush, columns[colIndex].Align);
                    if (ShowGridLines) { graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height); }
                    currentX += columns[colIndex].Width;
                }
                colIndex++;

                if (ShowAccountantColumn && columns[colIndex].Show)
                {
                    RectangleF cellRect = new(currentX, currentY, columns[colIndex].Width, DataRowHeight);
                    DrawCellText(graphics, transaction.Accountant, cellRect, dataFont, dataBrush, columns[colIndex].Align);
                    if (ShowGridLines) { graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height); }
                    currentX += columns[colIndex].Width;
                }
                colIndex++;

                if (ShowShippingColumn && columns[colIndex].Show)
                {
                    RectangleF cellRect = new(currentX, currentY, columns[colIndex].Width, DataRowHeight);
                    DrawCellText(graphics, $"${transaction.Shipping:N2}", cellRect, dataFont, dataBrush, columns[colIndex].Align);
                    if (ShowGridLines) { graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height); }
                    currentX += columns[colIndex].Width;
                }

                currentY += DataRowHeight;
                rowIndex++;

                // Stop if we've exceeded the bounds
                if (currentY > Bounds.Bottom - DataRowHeight)
                {
                    break;
                }
            }

            // Draw totals row if enabled
            if (ShowTotalsRow && transactions.Count > 0)
            {
                using SolidBrush totalsBrush = new(Color.FromArgb(230, 230, 230));
                graphics.FillRectangle(totalsBrush, Bounds.X, currentY, Bounds.Width, DataRowHeight);

                currentX = Bounds.X;

                // Calculate totals for all numeric columns
                decimal totalQuantity = transactions.Sum(t => t.Quantity);
                decimal totalUnitPrice = transactions.Sum(t => t.UnitPrice);
                decimal totalAmount = transactions.Sum(t => t.Total);
                decimal totalShipping = transactions.Sum(t => t.Shipping);
                int totalCount = transactions.Count;

                // Draw totals for each visible column
                bool labelDrawn = false;
                foreach ((string Header, float Width, bool Show, StringAlignment Align) in columns.Where(c => c.Show))
                {
                    RectangleF cellRect = new(currentX, currentY, Width, DataRowHeight);

                    if (!labelDrawn)
                    {
                        DrawCellText(graphics, $"Totals ({totalCount} rows)", cellRect, headerFont, dataBrush, StringAlignment.Near);
                        labelDrawn = true;
                    }
                    else if (Header == "Qty")
                    {
                        DrawCellText(graphics, $"{totalQuantity:N0}", cellRect, headerFont, dataBrush, StringAlignment.Far);
                    }
                    else if (Header == "Unit Price")
                    {
                        DrawCellText(graphics, $"${totalUnitPrice:N2}", cellRect, headerFont, dataBrush, StringAlignment.Far);
                    }
                    else if (Header == "Total")
                    {
                        DrawCellText(graphics, $"${totalAmount:N2}", cellRect, headerFont, dataBrush, StringAlignment.Far);
                    }
                    else if (Header == "Shipping")
                    {
                        DrawCellText(graphics, $"${totalShipping:N2}", cellRect, headerFont, dataBrush, StringAlignment.Far);
                    }

                    if (ShowGridLines)
                    {
                        graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    }

                    currentX += Width;
                }
            }

            // Draw outer border
            using Pen borderPen = new(Color.Black, 1);
            graphics.DrawRectangle(borderPen, Bounds);
        }
        private List<(string Header, float Width, bool Show, StringAlignment Align)> GetColumnDefinitions(
            Graphics graphics, Font headerFont, Font dataFont, List<TransactionData> transactions)
        {
            List<(string Header, float Width, bool Show, StringAlignment Align)> columns = [];

            // Define columns with minimum widths
            columns.Add(("Date", ShowDateColumn ? CalculateColumnWidth(graphics, "Date", "2024-12-31", headerFont, dataFont) : 0, ShowDateColumn, StringAlignment.Near));
            columns.Add(("ID", ShowTransactionIdColumn ? CalculateColumnWidth(graphics, "ID", "INV-12345", headerFont, dataFont) : 0, ShowTransactionIdColumn, StringAlignment.Near));
            columns.Add(("Customer/Supplier", ShowCompanyColumn ? CalculateColumnWidth(graphics, "Customer/Supplier", GetLongestText(transactions, t => t.CustomerSupplier), headerFont, dataFont) : 0, ShowCompanyColumn, StringAlignment.Near));
            columns.Add(("Product", ShowProductColumn ? CalculateColumnWidth(graphics, "Product", GetLongestText(transactions, t => t.Product), headerFont, dataFont) : 0, ShowProductColumn, StringAlignment.Near));
            columns.Add(("Qty", ShowQuantityColumn ? CalculateColumnWidth(graphics, "Qty", "9,999", headerFont, dataFont) : 0, ShowQuantityColumn, StringAlignment.Far));
            columns.Add(("Unit Price", ShowUnitPriceColumn ? CalculateColumnWidth(graphics, "Unit Price", "$9,999.99", headerFont, dataFont) : 0, ShowUnitPriceColumn, StringAlignment.Far));
            columns.Add(("Total", ShowTotalColumn ? CalculateColumnWidth(graphics, "Total", "$999,999.99", headerFont, dataFont) : 0, ShowTotalColumn, StringAlignment.Far));
            columns.Add(("Status", ShowStatusColumn ? CalculateColumnWidth(graphics, "Status", "Return", headerFont, dataFont) : 0, ShowStatusColumn, StringAlignment.Center));
            columns.Add(("Accountant", ShowAccountantColumn ? CalculateColumnWidth(graphics, "Accountant", GetLongestText(transactions, t => t.Accountant), headerFont, dataFont) : 0, ShowAccountantColumn, StringAlignment.Near));
            columns.Add(("Shipping", ShowShippingColumn ? CalculateColumnWidth(graphics, "Shipping", "$999.99", headerFont, dataFont) : 0, ShowShippingColumn, StringAlignment.Far));

            return columns;
        }
        private static string GetLongestText(List<TransactionData> transactions, Func<TransactionData, string> selector)
        {
            return transactions == null || transactions.Count == 0
                ? "Sample Text"
                : transactions.Select(selector).OrderByDescending(s => s?.Length ?? 0).FirstOrDefault() ?? "Sample Text";
        }
        private float CalculateColumnWidth(Graphics graphics, string header, string sampleData, Font headerFont, Font dataFont)
        {
            float headerWidth = graphics.MeasureString(header, headerFont).Width;
            float dataWidth = graphics.MeasureString(sampleData, dataFont).Width;
            return Math.Max(headerWidth, dataWidth) + (CellPadding * 2);  // Add padding
        }
        private void DrawCellText(Graphics graphics, string text, RectangleF cellRect, Font font, Brush brush, StringAlignment alignment)
        {
            using StringFormat format = new()
            {
                Alignment = alignment,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoWrap,
                Trimming = StringTrimming.EllipsisCharacter
            };

            // Add padding
            RectangleF textRect = new(
                cellRect.X + CellPadding,
                cellRect.Y,
                cellRect.Width - (CellPadding * 2),
                cellRect.Height
            );

            graphics.DrawString(text, font, brush, textRect, format);
        }
        private void RenderNoDataMessage(Graphics graphics)
        {
            // Draw background
            using SolidBrush baseBgBrush = new(Color.FromArgb(240, 240, 240));
            graphics.FillRectangle(baseBgBrush, Bounds);

            using Font font = new("Segoe UI", 10);
            using SolidBrush textBrush = new(Color.Gray);

            StringFormat format = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            string text = LanguageManager.TranslateString("No transactions to display");
            graphics.DrawString(text, font, textBrush, Bounds, format);

            // Draw border
            using Pen borderPen = new(Color.Black, 1);
            graphics.DrawRectangle(borderPen, Bounds);
        }

        // Override the property to indicate this element handles its own common controls
        public override bool HandlesOwnCommonControls => true;

        public Panel General_Panel { get; private set; }
        public Panel Style_Panel { get; private set; }
        public Panel Columns_Panel { get; private set; }
        protected override int CreateElementSpecificControls(Panel container, int yPosition, Action onPropertyChanged)
        {
            // Create panels for each tab
            General_Panel = CreateTabPanel();
            Style_Panel = CreateTabPanel();
            Columns_Panel = CreateTabPanel();

            // Create controls for each tab panel
            CreateGeneralTabControls(General_Panel, onPropertyChanged);
            CreateStyleTabControls(Style_Panel, onPropertyChanged);
            CreateColumnsTabControls(Columns_Panel, onPropertyChanged);

            // Capture instance for the handler
            Panel generalPanel = General_Panel;
            Panel stylePanel = Style_Panel;
            Panel columnsPanel = Columns_Panel;

            // Tab changed handler
            void TabChangedHandler(int tabIndex)
            {
                // This prevents flickering
                LoadingPanel.ShowBlankLoadingPanel(CachedPropertyPanel, CustomColors.ControlBack);

                generalPanel.Visible = false;
                stylePanel.Visible = false;
                columnsPanel.Visible = false;

                ThemeManager.RemoveCustomScrollBar(generalPanel);
                ThemeManager.RemoveCustomScrollBar(stylePanel);
                ThemeManager.RemoveCustomScrollBar(columnsPanel);

                if (tabIndex == 0)
                {
                    generalPanel.Visible = true;
                    ThemeManager.CustomizeScrollBar(generalPanel);
                }
                else if (tabIndex == 1)
                {
                    stylePanel.Visible = true;
                    ThemeManager.CustomizeScrollBar(stylePanel);
                }
                else if (tabIndex == 2)
                {
                    columnsPanel.Visible = true;
                    ThemeManager.CustomizeScrollBar(columnsPanel);
                }

                UpdateAllControlValues();
                LoadingPanel.HideBlankLoadingPanel(CachedPropertyPanel);
            }

            // Create tab buttons
            string[] tabNames = [
                LanguageManager.TranslateString("General"),
                LanguageManager.TranslateString("Style"),
                LanguageManager.TranslateString("Columns")
            ];
            Panel tabPanel = CreateTabButtons(tabNames, TabChangedHandler);

            return yPosition + 45;
        }
        private Panel CreateTabPanel()
        {
            Panel panel = new()
            {
                Location = new Point(0, 45),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AutoScroll = true,
                Size = new Size(CachedPropertyPanel.Width, CachedPropertyPanel.Height - 45)
            };

            CachedPropertyPanel.Controls.Add(panel);
            return panel;
        }
        private void CreateGeneralTabControls(Panel panel, Action onPropertyChanged)
        {
            int yPosition = 0;

            // Get undo manager
            UndoRedoManager? undoRedoManager = ReportLayoutDesigner_Form.Instance?.GetUndoRedoManager();
            string text;

            // Add common controls first (X, Y, Width, Height)
            CreateCommonPropertyControls(panel, this, yPosition, onPropertyChanged,
                out Dictionary<string, Action> commonUpdateActions);

            // Cache the common control update actions
            foreach (KeyValuePair<string, Action> kvp in commonUpdateActions)
            {
                CacheControl(kvp.Key, null, kvp.Value);
            }

            yPosition += ControlRowHeight * 4;

            // Data selection combo box
            text = LanguageManager.TranslateString("Data") + ":";
            AddPropertyLabel(panel, text, yPosition);
            Guna2ComboBox dataCombo = AddPropertyComboBox(panel, DataSelection.ToString(), yPosition,
                Enum.GetNames<TableDataSelection>(),
                value =>
                {
                    TableDataSelection newSelection = Enum.Parse<TableDataSelection>(value);
                    if (DataSelection != newSelection)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(DataSelection),
                            DataSelection,
                            newSelection,
                            onPropertyChanged));
                        DataSelection = newSelection;
                        onPropertyChanged();
                    }
                });
            CacheControl("DataSelection", dataCombo, () => dataCombo.SelectedItem = DataSelection.ToString());
            yPosition += ControlRowHeight;

            // Transaction type
            text = LanguageManager.TranslateString("Type") + ":";
            AddPropertyLabel(panel, text, yPosition);
            Guna2ComboBox typeCombo = AddPropertyComboBox(panel, TransactionType.ToString(), yPosition,
                Enum.GetNames<TransactionType>(),
                value =>
                {
                    TransactionType newType = Enum.Parse<TransactionType>(value);
                    if (TransactionType != newType)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(TransactionType),
                            TransactionType,
                            newType,
                            onPropertyChanged));
                        TransactionType = newType;
                        onPropertyChanged();
                    }
                });
            CacheControl("TransactionType", typeCombo, () => typeCombo.SelectedItem = TransactionType.ToString());
            yPosition += ControlRowHeight;

            // Include returns checkbox
            text = LanguageManager.TranslateString("Include Returns");
            Guna2CustomCheckBox returnsCheck = AddPropertyCheckBoxWithLabel(panel, text, IncludeReturns, yPosition,
                value =>
                {
                    if (IncludeReturns != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(IncludeReturns),
                            IncludeReturns,
                            value,
                            onPropertyChanged));
                        IncludeReturns = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("IncludeReturns", returnsCheck, () => returnsCheck.Checked = IncludeReturns);
            yPosition += CheckBoxRowHeight;

            // Include losses checkbox
            text = LanguageManager.TranslateString("Include Losses");
            Guna2CustomCheckBox lossesCheck = AddPropertyCheckBoxWithLabel(panel, text, IncludeLosses, yPosition,
                value =>
                {
                    if (IncludeLosses != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(IncludeLosses),
                            IncludeLosses,
                            value,
                            onPropertyChanged));
                        IncludeLosses = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("IncludeLosses", lossesCheck, () => lossesCheck.Checked = IncludeLosses);
            yPosition += CheckBoxRowHeight;

            // Sort order combo box
            text = LanguageManager.TranslateString("Sort") + ":";
            AddPropertyLabel(panel, text, yPosition);
            Guna2ComboBox sortCombo = AddPropertyComboBox(panel, FormatSortOrder(SortOrder), yPosition,
                ["Date ↓", "Date ↑", "Amount ↓", "Amount ↑"],
                value =>
                {
                    TableSortOrder newOrder = ParseSortOrder(value);
                    if (SortOrder != newOrder)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(SortOrder),
                            SortOrder,
                            newOrder,
                            onPropertyChanged));
                        SortOrder = newOrder;
                        onPropertyChanged();
                    }
                });
            CacheControl("SortOrder", sortCombo, () => sortCombo.SelectedItem = FormatSortOrder(SortOrder));
            yPosition += ControlRowHeight;

            // Max rows
            text = LanguageManager.TranslateString("Max Rows") + ":";
            AddPropertyLabel(panel, text, yPosition, false, NumericUpDownWidth);
            Guna2NumericUpDown numericUpDown = AddPropertyNumericUpDown(panel, MaxRows, yPosition,
                value =>
                {
                    int newMaxRows = (int)value;
                    if (MaxRows != newMaxRows)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(MaxRows),
                            MaxRows,
                            newMaxRows,
                            onPropertyChanged));
                        MaxRows = newMaxRows;
                        onPropertyChanged();
                    }
                }, 1, 100);
            CacheControl("MaxRows", numericUpDown, () => numericUpDown.Value = MaxRows);
            yPosition += ControlRowHeight;

            // Totals row
            text = LanguageManager.TranslateString("Show Totals Row");
            Guna2CustomCheckBox totalsCheck = AddPropertyCheckBoxWithLabel(panel, text, ShowTotalsRow, yPosition,
                value =>
                {
                    if (ShowTotalsRow != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(ShowTotalsRow),
                            ShowTotalsRow,
                            value,
                            onPropertyChanged));
                        ShowTotalsRow = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("ShowTotalsRow", totalsCheck, () => totalsCheck.Checked = ShowTotalsRow);
        }
        private void CreateStyleTabControls(Panel panel, Action onPropertyChanged)
        {
            int yPosition = 0;

            // Get undo manager
            UndoRedoManager? undoRedoManager = ReportLayoutDesigner_Form.Instance?.GetUndoRedoManager();
            string text;

            // Font Family
            text = LanguageManager.TranslateString("Font") + ":";
            Label fontLabel = AddPropertyLabel(panel, text, yPosition);

            Guna2TextBox fontTextBox = AddPropertySearchBox(
                panel,
                FontFamily,
                yPosition,
                GetFontSearchResults,
                value =>
                {
                    if (FontFamily != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(FontFamily),
                            FontFamily,
                            value,
                            onPropertyChanged));
                        FontFamily = value;
                        onPropertyChanged();
                    }
                },
                fontLabel);

            CacheControl("FontFamily", fontTextBox, () => fontTextBox.Text = FontFamily);
            yPosition += ControlRowHeight;

            // Font size
            text = LanguageManager.TranslateString("Font Size") + ":";
            AddPropertyLabel(panel, text, yPosition, false, NumericUpDownWidth);
            Guna2NumericUpDown fontSizeNumeric = AddPropertyNumericUpDown(panel, (decimal)FontSize, yPosition,
                value =>
                {
                    float newFontSize = (float)value;
                    if (Math.Abs(FontSize - newFontSize) > 0.01f)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(FontSize),
                            FontSize,
                            newFontSize,
                            onPropertyChanged));
                        FontSize = newFontSize;
                        onPropertyChanged();
                    }
                }, 6, 14);
            CacheControl("FontSize", fontSizeNumeric, () => fontSizeNumeric.Value = (decimal)FontSize);
            yPosition += ControlRowHeight;

            // Row Height
            text = LanguageManager.TranslateString("Row Height") + ":";
            AddPropertyLabel(panel, text, yPosition, false, NumericUpDownWidth);
            Guna2NumericUpDown rowHeightNumeric = AddPropertyNumericUpDown(panel, DataRowHeight, yPosition,
                value =>
                {
                    int newRowHeight = (int)value;
                    if (DataRowHeight != newRowHeight)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(DataRowHeight),
                            DataRowHeight,
                            newRowHeight,
                            onPropertyChanged));
                        DataRowHeight = newRowHeight;
                        onPropertyChanged();
                    }
                }, 15, 50);
            CacheControl("DataRowHeight", rowHeightNumeric, () => rowHeightNumeric.Value = DataRowHeight);
            yPosition += ControlRowHeight;

            // Header Row Height
            text = LanguageManager.TranslateString("Header Height") + ":";
            AddPropertyLabel(panel, text, yPosition, false, NumericUpDownWidth);
            Guna2NumericUpDown headerHeightNumeric = AddPropertyNumericUpDown(panel, HeaderRowHeight, yPosition,
                value =>
                {
                    int newHeaderHeight = (int)value;
                    if (HeaderRowHeight != newHeaderHeight)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(HeaderRowHeight),
                            HeaderRowHeight,
                            newHeaderHeight,
                            onPropertyChanged));
                        HeaderRowHeight = newHeaderHeight;
                        onPropertyChanged();
                    }
                }, 20, 60);
            CacheControl("HeaderRowHeight", headerHeightNumeric, () => headerHeightNumeric.Value = HeaderRowHeight);
            yPosition += ControlRowHeight;

            // Cell Padding
            text = LanguageManager.TranslateString("Cell Padding") + ":";
            AddPropertyLabel(panel, text, yPosition, false, NumericUpDownWidth);
            Guna2NumericUpDown cellPaddingNumeric = AddPropertyNumericUpDown(panel, CellPadding, yPosition,
                value =>
                {
                    int newPadding = (int)value;
                    if (CellPadding != newPadding)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(CellPadding),
                            CellPadding,
                            newPadding,
                            onPropertyChanged));
                        CellPadding = newPadding;
                        onPropertyChanged();
                    }
                }, 0, 20);
            CacheControl("CellPadding", cellPaddingNumeric, () => cellPaddingNumeric.Value = CellPadding);
            yPosition += ControlRowHeight;

            // Header Background Color
            text = LanguageManager.TranslateString("Header Background") + ":";
            AddPropertyLabel(panel, text, yPosition, false, ColorPickerWidth);
            Panel headerBgPicker = AddColorPicker(panel, yPosition, HeaderBackgroundColor,
                newColor =>
                {
                    if (HeaderBackgroundColor != newColor)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(HeaderBackgroundColor),
                            HeaderBackgroundColor,
                            newColor,
                            onPropertyChanged));
                        HeaderBackgroundColor = newColor;
                        onPropertyChanged();
                    }
                });
            CacheControl("HeaderBackgroundColor", headerBgPicker, () => headerBgPicker.BackColor = HeaderBackgroundColor);
            yPosition += ControlRowHeight;

            // Header Text Color
            text = LanguageManager.TranslateString("Header Text") + ":";
            AddPropertyLabel(panel, text, yPosition, false, ColorPickerWidth);
            Panel headerTextPicker = AddColorPicker(panel, yPosition, HeaderTextColor,
                color =>
                {
                    if (HeaderTextColor != color)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(HeaderTextColor),
                            HeaderTextColor,
                            color,
                            onPropertyChanged));
                        HeaderTextColor = color;
                        onPropertyChanged();
                    }
                });
            CacheControl("HeaderTextColor", headerTextPicker, () => headerTextPicker.BackColor = HeaderTextColor);
            yPosition += ControlRowHeight;

            // Data Row Text Color
            text = LanguageManager.TranslateString("Row Text") + ":";
            AddPropertyLabel(panel, text, yPosition, false, ColorPickerWidth);
            Panel rowTextPicker = AddColorPicker(panel, yPosition, DataRowTextColor,
                color =>
                {
                    if (DataRowTextColor != color)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(DataRowTextColor),
                            DataRowTextColor,
                            color,
                            onPropertyChanged));
                        DataRowTextColor = color;
                        onPropertyChanged();
                    }
                });
            CacheControl("DataRowTextColor", rowTextPicker, () => rowTextPicker.BackColor = DataRowTextColor);
            yPosition += ControlRowHeight;

            // Grid Line Color
            text = LanguageManager.TranslateString("Grid Lines") + ":";
            AddPropertyLabel(panel, text, yPosition, false, ColorPickerWidth);
            Panel gridLinePicker = AddColorPicker(panel, yPosition, GridLineColor,
                color =>
                {
                    if (GridLineColor != color)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(GridLineColor),
                            GridLineColor,
                            color,
                            onPropertyChanged));
                        GridLineColor = color;
                        onPropertyChanged();
                    }
                });
            CacheControl("GridLineColor", gridLinePicker, () => gridLinePicker.BackColor = GridLineColor);
            yPosition += ControlRowHeight;

            // Display option checkboxes
            text = LanguageManager.TranslateString("Show Headers");
            Guna2CustomCheckBox headersCheck = AddPropertyCheckBoxWithLabel(panel, text, ShowHeaders, yPosition,
                value =>
                {
                    if (ShowHeaders != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(ShowHeaders),
                            ShowHeaders,
                            value,
                            onPropertyChanged));
                        ShowHeaders = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("ShowHeaders", headersCheck, () => headersCheck.Checked = ShowHeaders);
            yPosition += CheckBoxRowHeight;

            text = LanguageManager.TranslateString("Alternate Row Colors");
            Guna2CustomCheckBox alternateCheck = AddPropertyCheckBoxWithLabel(panel, text, AlternateRowColors, yPosition,
                value =>
                {
                    if (AlternateRowColors != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(AlternateRowColors),
                            AlternateRowColors,
                            value,
                            onPropertyChanged));
                        AlternateRowColors = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("AlternateRowColors", alternateCheck, () => alternateCheck.Checked = AlternateRowColors);
            yPosition += CheckBoxRowHeight;

            text = LanguageManager.TranslateString("Show Grid Lines");
            Guna2CustomCheckBox gridCheck = AddPropertyCheckBoxWithLabel(panel, text, ShowGridLines, yPosition,
                value =>
                {
                    if (ShowGridLines != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(ShowGridLines),
                            ShowGridLines,
                            value,
                            onPropertyChanged));
                        ShowGridLines = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("ShowGridLines", gridCheck, () => gridCheck.Checked = ShowGridLines);
        }
        private void CreateColumnsTabControls(Panel panel, Action onPropertyChanged)
        {
            int yPosition = 0;

            // Get undo manager
            UndoRedoManager? undoRedoManager = ReportLayoutDesigner_Form.Instance?.GetUndoRedoManager();
            string text;

            // Add column visibility checkboxes
            text = LanguageManager.TranslateString("Date");
            Guna2CustomCheckBox dateCheck = AddPropertyCheckBoxWithLabel(panel, text, ShowDateColumn, yPosition,
                value =>
                {
                    if (ShowDateColumn != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(ShowDateColumn),
                            ShowDateColumn,
                            value,
                            onPropertyChanged));
                        ShowDateColumn = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("ShowDateColumn", dateCheck, () => dateCheck.Checked = ShowDateColumn);
            yPosition += CheckBoxRowHeight;

            text = LanguageManager.TranslateString("Transaction ID");
            Guna2CustomCheckBox idCheck = AddPropertyCheckBoxWithLabel(panel, text, ShowTransactionIdColumn, yPosition,
                value =>
                {
                    if (ShowTransactionIdColumn != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(ShowTransactionIdColumn),
                            ShowTransactionIdColumn,
                            value,
                            onPropertyChanged));
                        ShowTransactionIdColumn = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("ShowTransactionIdColumn", idCheck, () => idCheck.Checked = ShowTransactionIdColumn);
            yPosition += CheckBoxRowHeight;

            text = LanguageManager.TranslateString("Company");
            Guna2CustomCheckBox customerCheck = AddPropertyCheckBoxWithLabel(panel, text, ShowCompanyColumn, yPosition,
                value =>
                {
                    if (ShowCompanyColumn != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(ShowCompanyColumn),
                            ShowCompanyColumn,
                            value,
                            onPropertyChanged));
                        ShowCompanyColumn = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("ShowCompanyColumn", customerCheck, () => customerCheck.Checked = ShowCompanyColumn);
            yPosition += CheckBoxRowHeight;

            text = LanguageManager.TranslateString("Product");
            Guna2CustomCheckBox productCheck = AddPropertyCheckBoxWithLabel(panel, text, ShowProductColumn, yPosition,
                value =>
                {
                    if (ShowProductColumn != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(ShowProductColumn),
                            ShowProductColumn,
                            value,
                            onPropertyChanged));
                        ShowProductColumn = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("ShowProductColumn", productCheck, () => productCheck.Checked = ShowProductColumn);
            yPosition += CheckBoxRowHeight;

            text = LanguageManager.TranslateString("Quantity");
            Guna2CustomCheckBox quantityCheck = AddPropertyCheckBoxWithLabel(panel, text, ShowQuantityColumn, yPosition,
                value =>
                {
                    if (ShowQuantityColumn != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(ShowQuantityColumn),
                            ShowQuantityColumn,
                            value,
                            onPropertyChanged));
                        ShowQuantityColumn = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("ShowQuantityColumn", quantityCheck, () => quantityCheck.Checked = ShowQuantityColumn);
            yPosition += CheckBoxRowHeight;

            text = LanguageManager.TranslateString("Unit Price");
            Guna2CustomCheckBox unitPriceCheck = AddPropertyCheckBoxWithLabel(panel, text, ShowUnitPriceColumn, yPosition,
                value =>
                {
                    if (ShowUnitPriceColumn != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(ShowUnitPriceColumn),
                            ShowUnitPriceColumn,
                            value,
                            onPropertyChanged));
                        ShowUnitPriceColumn = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("ShowUnitPriceColumn", unitPriceCheck, () => unitPriceCheck.Checked = ShowUnitPriceColumn);
            yPosition += CheckBoxRowHeight;

            text = LanguageManager.TranslateString("Total");
            Guna2CustomCheckBox totalCheck = AddPropertyCheckBoxWithLabel(panel, text, ShowTotalColumn, yPosition,
                value =>
                {
                    if (ShowTotalColumn != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(ShowTotalColumn),
                            ShowTotalColumn,
                            value,
                            onPropertyChanged));
                        ShowTotalColumn = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("ShowTotalColumn", totalCheck, () => totalCheck.Checked = ShowTotalColumn);
            yPosition += CheckBoxRowHeight;

            text = LanguageManager.TranslateString("Status");
            Guna2CustomCheckBox statusCheck = AddPropertyCheckBoxWithLabel(panel, text, ShowStatusColumn, yPosition,
                value =>
                {
                    if (ShowStatusColumn != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(ShowStatusColumn),
                            ShowStatusColumn,
                            value,
                            onPropertyChanged));
                        ShowStatusColumn = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("ShowStatusColumn", statusCheck, () => statusCheck.Checked = ShowStatusColumn);
            yPosition += CheckBoxRowHeight;

            text = LanguageManager.TranslateString("Accountant");
            Guna2CustomCheckBox accountantCheck = AddPropertyCheckBoxWithLabel(panel, text, ShowAccountantColumn, yPosition,
                value =>
                {
                    if (ShowAccountantColumn != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(ShowAccountantColumn),
                            ShowAccountantColumn,
                            value,
                            onPropertyChanged));
                        ShowAccountantColumn = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("ShowAccountantColumn", accountantCheck, () => accountantCheck.Checked = ShowAccountantColumn);
            yPosition += CheckBoxRowHeight;

            text = LanguageManager.TranslateString("Shipping");
            Guna2CustomCheckBox shippingCheck = AddPropertyCheckBoxWithLabel(panel, text, ShowShippingColumn, yPosition,
                value =>
                {
                    if (ShowShippingColumn != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(ShowShippingColumn),
                            ShowShippingColumn,
                            value,
                            onPropertyChanged));
                        ShowShippingColumn = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("ShowShippingColumn", shippingCheck, () => shippingCheck.Checked = ShowShippingColumn);
        }

        // Helper methods
        private static string FormatSortOrder(TableSortOrder order)
        {
            return order switch
            {
                TableSortOrder.DateDescending => "Date ↓",
                TableSortOrder.DateAscending => "Date ↑",
                TableSortOrder.AmountDescending => "Amount ↓",
                TableSortOrder.AmountAscending => "Amount ↑",
                _ => "Date ↓"
            };
        }
        private static TableSortOrder ParseSortOrder(string formatted)
        {
            return formatted switch
            {
                "Date ↓" => TableSortOrder.DateDescending,
                "Date ↑" => TableSortOrder.DateAscending,
                "Amount ↓" => TableSortOrder.AmountDescending,
                "Amount ↑" => TableSortOrder.AmountAscending,
                _ => TableSortOrder.DateDescending
            };
        }
    }
}
using Argo_Books.Classes;
using Argo_Books.DataClasses;
using Argo_Books.UI;
using Guna.UI2.WinForms;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.SkiaSharpView.WinForms;
using Argo_Books;
using Argo_Books.Classes;
using Argo_Books.Language;
using System.Drawing.Imaging;

namespace Argo_Books.Charts
{
    /// <summary>
    /// Manages the right-click menu for chart controls with options for saving, exporting, and zoom reset.
    /// </summary>
    public static class RightClickChartMenu
    {
        // Properties
        private static Guna2Button _resetZoom_Button, _exportExcel_Button, _exportSheet_Button;

        // Getter
        public static Guna2Panel Panel { get; private set; }

        // Right click menu methods
        public static void Construct()
        {
            Panel = CustomControls.ConstructPanelForMenu(
                new Size(CustomControls.PanelWidth - 50, 4 * CustomControls.PanelButtonHeight + CustomControls.SpaceForPanel),
                "rightClickChart_Panel"
            );

            FlowLayoutPanel flowPanel = (FlowLayoutPanel)Panel.Controls[0];
            int newBtnWidth = CustomControls.PanelBtnWidth - 50;

            Guna2Button button = CustomControls.ConstructBtnForMenu("Save image", newBtnWidth, flowPanel);
            button.Click += SaveImage;

            _exportExcel_Button = CustomControls.ConstructBtnForMenu("Export to Microsoft Excel", newBtnWidth, flowPanel);
            _exportExcel_Button.Click += ExportToMicrosoftExcel;

            _exportSheet_Button = CustomControls.ConstructBtnForMenu("Export to Google Sheets", newBtnWidth, flowPanel);
            _exportSheet_Button.Click += ExportToGoogleSheets;

            _resetZoom_Button = CustomControls.ConstructBtnForMenu("Reset zoom", newBtnWidth, flowPanel);
            _resetZoom_Button.Click += ResetZoom;
        }
        private static void SaveImage(object sender, EventArgs e)
        {
            Control chart = (Control)Panel.Tag;

            using SaveFileDialog dialog = new();
            string date = Tools.FormatDate(DateTime.Now);
            dialog.FileName = $"{Directories.CompanyName} {chart.Name} {date}";
            dialog.DefaultExt = ArgoFiles.PngFileExtension;
            dialog.Filter = $"PNG Image|*{ArgoFiles.PngFileExtension}";
            dialog.Title = "Save Chart Image";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SaveChartAsImage(chart, dialog.FileName);
            }
        }
        private static void SaveChartAsImage(Control chart, string fileName)
        {
            try
            {
                // Capture the chart as a bitmap
                using Bitmap bitmap = new(chart.Width, chart.Height);
                chart.DrawToBitmap(bitmap, new Rectangle(0, 0, chart.Width, chart.Height));
                bitmap.Save(fileName, ImageFormat.Png);
            }
            catch (Exception ex)
            {
                Log.Error_ExportingChart($"Failed to save chart image: {ex.Message}");
            }
        }
        private static void ExportToMicrosoftExcel(object sender, EventArgs e)
        {
            Control chart = (Control)Panel.Tag;
            string directory = "";

            using SaveFileDialog dialog = new();
            string date = Tools.FormatDate(DateTime.Now);
            string name = GetChartTitle(chart).Split(':')[0];
            dialog.FileName = $"{Directories.CompanyName} {name} {date}";
            dialog.DefaultExt = ArgoFiles.XlsxFileExtension;
            dialog.Filter = $"XLSX spreadsheet|*{ArgoFiles.XlsxFileExtension}";
            dialog.Title = "Export Chart to XLSX";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                directory = dialog.FileName;
            }
            else { return; }

            bool isLine = MainMenu_Form.Instance.LineChart_ToggleSwitch.Checked;

            switch (chart.Tag)
            {
                case MainMenu_Form.ChartDataType.TotalRevenue:
                    LoadChart.LoadTotalsIntoChart(MainMenu_Form.Instance.Sale_DataGridView, MainMenu_Form.Instance.TotalSales_Chart, isLine, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.TotalExpenses:
                    LoadChart.LoadTotalsIntoChart(MainMenu_Form.Instance.Purchase_DataGridView, MainMenu_Form.Instance.TotalPurchases_Chart, isLine, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.RevenueDistribution:
                    LoadChart.LoadDistributionIntoChart(MainMenu_Form.Instance.Sale_DataGridView, MainMenu_Form.Instance.DistributionOfSales_Chart, PieChartGrouping.Unlimited, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.ExpensesDistribution:
                    LoadChart.LoadDistributionIntoChart(MainMenu_Form.Instance.Purchase_DataGridView, MainMenu_Form.Instance.DistributionOfPurchases_Chart, PieChartGrouping.Unlimited, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.TotalProfits:
                    LoadChart.LoadProfitsIntoChart(MainMenu_Form.Instance.Profits_Chart, isLine, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.CountriesOfOrigin:
                    LoadChart.LoadCountriesOfOriginChart(MainMenu_Form.Instance.CountriesOfOrigin_Chart, PieChartGrouping.Unlimited, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.CompaniesOfOrigin:
                    LoadChart.LoadCompaniesOfOriginChart(MainMenu_Form.Instance.CompaniesOfOrigin_Chart, PieChartGrouping.Unlimited, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.CountriesOfDestination:
                    LoadChart.LoadCountriesOfDestinationChart(MainMenu_Form.Instance.CountriesOfDestination_Chart, PieChartGrouping.Unlimited, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.AccountantsTransactions:
                    LoadChart.LoadAccountantsIntoChart(MainMenu_Form.Instance.Accountants_Chart, PieChartGrouping.Unlimited, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.SalesVsExpenses:
                    LoadChart.LoadSalesVsExpensesChart(MainMenu_Form.Instance.TotalExpensesVsSales_Chart, isLine, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.AverageTransactionValue:
                    LoadChart.LoadAverageTransactionValueChart(MainMenu_Form.Instance.AverageTransactionValue_Chart, isLine, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.TotalTransactions:
                    LoadChart.LoadTotalTransactionsChart(MainMenu_Form.Instance.TotalTransactions_Chart, isLine, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.AverageShippingCosts:
                    LoadChart.LoadAverageShippingCostsChart(MainMenu_Form.Instance.AverageShippingCosts_Chart, isLine, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.GrowthRates:
                    LoadChart.LoadGrowthRateChart(MainMenu_Form.Instance.GrowthRates_Chart, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.ReturnsOverTime:
                    LoadChart.LoadReturnsOverTimeChart(MainMenu_Form.Instance.ReturnsOverTime_Chart, isLine, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.ReturnReasons:
                    LoadChart.LoadReturnReasonsChart(MainMenu_Form.Instance.ReturnReasons_Chart, PieChartGrouping.Unlimited, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.ReturnFinancialImpact:
                    LoadChart.LoadReturnFinancialImpactChart(MainMenu_Form.Instance.ReturnFinancialImpact_Chart, isLine, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.ReturnsByCategory:
                    LoadChart.LoadReturnsByCategoryChart(MainMenu_Form.Instance.ReturnsByCategory_Chart, PieChartGrouping.Unlimited, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.ReturnsByProduct:
                    LoadChart.LoadReturnsByProductChart(MainMenu_Form.Instance.ReturnsByProduct_Chart, PieChartGrouping.Unlimited, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.PurchaseVsSaleReturns:
                    LoadChart.LoadPurchaseVsSaleReturnsChart(MainMenu_Form.Instance.PurchaseVsSaleReturns_Chart, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.LossesOverTime:
                    LoadChart.LoadLossesOverTimeChart(MainMenu_Form.Instance.LossesOverTime_Chart, isLine, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.LossReasons:
                    LoadChart.LoadLossReasonsChart(MainMenu_Form.Instance.LossReasons_Chart, PieChartGrouping.Unlimited, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.LossFinancialImpact:
                    LoadChart.LoadLossFinancialImpactChart(MainMenu_Form.Instance.LossFinancialImpact_Chart, isLine, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.LossesByCategory:
                    LoadChart.LoadLossesByCategoryChart(MainMenu_Form.Instance.LossesByCategory_Chart, PieChartGrouping.Unlimited, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.LossesByProduct:
                    LoadChart.LoadLossesByProductChart(MainMenu_Form.Instance.LossesByProduct_Chart, PieChartGrouping.Unlimited, true, directory);
                    break;

                case MainMenu_Form.ChartDataType.PurchaseVsSaleLosses:
                    LoadChart.LoadPurchaseVsSaleLossesChart(MainMenu_Form.Instance.PurchaseVsSaleLosses_Chart, true, directory);
                    break;
            }
        }
        private static async void ExportToGoogleSheets(object sender, EventArgs e)
        {
            Control chart = (Control)Panel.Tag;
            bool isLine = MainMenu_Form.Instance.LineChart_ToggleSwitch.Checked;

            try
            {
                switch (chart.Tag)
                {
                    case MainMenu_Form.ChartDataType.TotalRevenue:
                        {
                            ChartData chartData = LoadChart.LoadTotalsIntoChart(MainMenu_Form.Instance.Sale_DataGridView, MainMenu_Form.Instance.TotalSales_Chart, isLine, canUpdateChart: false);
                            string chartTitle = TranslatedChartTitles.TotalRevenue;
                            GoogleSheetManager.ChartType chartType = isLine
                                ? GoogleSheetManager.ChartType.Line
                                : GoogleSheetManager.ChartType.Column;
                            string first = LanguageManager.TranslateString("Date");
                            string second = LanguageManager.TranslateString("Revenue");

                            await GoogleSheetManager.ExportChartToGoogleSheetsAsync(chartData.Data, chartTitle, chartType, first, second);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.TotalExpenses:
                        {
                            ChartData chartData = LoadChart.LoadTotalsIntoChart(MainMenu_Form.Instance.Purchase_DataGridView, MainMenu_Form.Instance.TotalPurchases_Chart, isLine, canUpdateChart: false);
                            string chartTitle = TranslatedChartTitles.TotalExpenses;
                            GoogleSheetManager.ChartType chartType = isLine
                                ? GoogleSheetManager.ChartType.Line
                                : GoogleSheetManager.ChartType.Column;
                            string first = LanguageManager.TranslateString("Date");
                            string second = LanguageManager.TranslateString("Expenses");

                            await GoogleSheetManager.ExportChartToGoogleSheetsAsync(chartData.Data, chartTitle, chartType, first, second);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.RevenueDistribution:
                        {
                            ChartData chartData = LoadChart.LoadDistributionIntoChart(MainMenu_Form.Instance.Sale_DataGridView, MainMenu_Form.Instance.DistributionOfSales_Chart, PieChartGrouping.Unlimited, canUpdateChart: false);
                            string chartTitle = TranslatedChartTitles.RevenueDistribution;
                            string first = LanguageManager.TranslateString("Category");
                            string second = LanguageManager.TranslateString("Revenue");

                            await GoogleSheetManager.ExportChartToGoogleSheetsAsync(chartData.Data, chartTitle, GoogleSheetManager.ChartType.Pie, first, second);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.ExpensesDistribution:
                        {
                            ChartData chartData = LoadChart.LoadDistributionIntoChart(MainMenu_Form.Instance.Purchase_DataGridView, MainMenu_Form.Instance.DistributionOfPurchases_Chart, PieChartGrouping.Unlimited, canUpdateChart: false);
                            string chartTitle = TranslatedChartTitles.ExpensesDistribution;
                            string first = LanguageManager.TranslateString("Category");
                            string second = LanguageManager.TranslateString("Expenses");

                            await GoogleSheetManager.ExportChartToGoogleSheetsAsync(chartData.Data, chartTitle, GoogleSheetManager.ChartType.Pie, first, second);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.TotalProfits:
                        {
                            ChartData chartData = LoadChart.LoadProfitsIntoChart(MainMenu_Form.Instance.Profits_Chart, isLine, canUpdateChart: false);
                            string chartTitle = TranslatedChartTitles.TotalProfits;
                            GoogleSheetManager.ChartType chartType = isLine
                                ? GoogleSheetManager.ChartType.Line
                                : GoogleSheetManager.ChartType.Column;
                            string first = LanguageManager.TranslateString("Date");
                            string second = LanguageManager.TranslateString("profits");

                            await GoogleSheetManager.ExportChartToGoogleSheetsAsync(chartData.Data, chartTitle, chartType, first, second);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.CountriesOfOrigin:
                        {
                            ChartData chartData = LoadChart.LoadCountriesOfOriginChart(MainMenu_Form.Instance.CountriesOfOrigin_Chart, PieChartGrouping.Unlimited, canUpdateChart: false);
                            string chartTitle = TranslatedChartTitles.CountriesOfOrigin;
                            string first = LanguageManager.TranslateString("Countries");
                            string second = LanguageManager.TranslateString("# of items");

                            await GoogleSheetManager.ExportChartToGoogleSheetsAsync(chartData.Data, chartTitle, GoogleSheetManager.ChartType.Pie, first, second);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.CompaniesOfOrigin:
                        {
                            ChartData chartData = LoadChart.LoadCompaniesOfOriginChart(MainMenu_Form.Instance.CompaniesOfOrigin_Chart, PieChartGrouping.Unlimited, canUpdateChart: false);
                            string chartTitle = TranslatedChartTitles.CompaniesOfOrigin;
                            string first = LanguageManager.TranslateString("Companies");
                            string second = LanguageManager.TranslateString("# of items");

                            await GoogleSheetManager.ExportChartToGoogleSheetsAsync(chartData.Data, chartTitle, GoogleSheetManager.ChartType.Pie, first, second);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.CountriesOfDestination:
                        {
                            ChartData chartData = LoadChart.LoadCountriesOfDestinationChart(MainMenu_Form.Instance.CountriesOfDestination_Chart, PieChartGrouping.Unlimited, canUpdateChart: false);
                            string chartTitle = TranslatedChartTitles.CountriesOfDestination;
                            string first = LanguageManager.TranslateString("Countries");
                            string second = LanguageManager.TranslateString("# of items");

                            await GoogleSheetManager.ExportChartToGoogleSheetsAsync(chartData.Data, chartTitle, GoogleSheetManager.ChartType.Pie, first, second);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.AccountantsTransactions:
                        {
                            ChartData chartData = LoadChart.LoadAccountantsIntoChart(MainMenu_Form.Instance.Accountants_Chart, PieChartGrouping.Unlimited, canUpdateChart: false);
                            string chartTitle = TranslatedChartTitles.AccountantsTransactions;
                            string first = LanguageManager.TranslateString("Accountants");
                            string second = LanguageManager.TranslateString("# of transactions");

                            await GoogleSheetManager.ExportChartToGoogleSheetsAsync(chartData.Data, chartTitle, GoogleSheetManager.ChartType.Pie, first, second);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.SalesVsExpenses:
                        {
                            SalesExpensesChartData salesExpensesData = LoadChart.LoadSalesVsExpensesChart(MainMenu_Form.Instance.TotalExpensesVsSales_Chart, isLine, canUpdateChart: false);
                            Dictionary<string, Dictionary<string, double>> combinedData = [];

                            foreach (string date in salesExpensesData.GetDateOrder())
                            {
                                combinedData[date] = new Dictionary<string, double>
                                {
                                    [LanguageManager.TranslateString("Total Expenses")] = salesExpensesData.GetExpensesForDate(date),
                                    [LanguageManager.TranslateString("Total Sales")] = salesExpensesData.GetSalesForDate(date)
                                };
                            }

                            string name = TranslatedChartTitles.SalesVsExpenses;
                            GoogleSheetManager.ChartType chartType = isLine
                                ? GoogleSheetManager.ChartType.Line
                                : GoogleSheetManager.ChartType.Column;

                            await GoogleSheetManager.ExportMultiDataSetChartToGoogleSheetsAsync(combinedData, name, chartType);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.AverageTransactionValue:
                        {
                            SalesExpensesChartData chartData = LoadChart.LoadAverageTransactionValueChart(MainMenu_Form.Instance.AverageTransactionValue_Chart, isLine, canUpdateChart: false);
                            Dictionary<string, Dictionary<string, double>> combinedData = [];

                            foreach (string date in chartData.GetDateOrder())
                            {
                                combinedData[date] = new Dictionary<string, double>
                                {
                                    [LanguageManager.TranslateString("Average purchase value")] = chartData.GetExpensesForDate(date),
                                    [LanguageManager.TranslateString("Average sale value")] = chartData.GetSalesForDate(date)
                                };
                            }

                            string name = TranslatedChartTitles.AverageTransactionValue;
                            GoogleSheetManager.ChartType chartType = isLine
                                ? GoogleSheetManager.ChartType.Line
                                : GoogleSheetManager.ChartType.Column;

                            await GoogleSheetManager.ExportMultiDataSetChartToGoogleSheetsAsync(combinedData, name, chartType);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.TotalTransactions:
                        {
                            SalesExpensesChartData chartData = LoadChart.LoadTotalTransactionsChart(MainMenu_Form.Instance.TotalTransactions_Chart, isLine, canUpdateChart: false);
                            Dictionary<string, Dictionary<string, double>> combinedData = [];

                            foreach (string date in chartData.GetDateOrder())
                            {
                                combinedData[date] = new Dictionary<string, double>
                                {
                                    [LanguageManager.TranslateString("Purchases")] = chartData.GetExpensesForDate(date),
                                    [LanguageManager.TranslateString("Sales")] = chartData.GetSalesForDate(date)
                                };
                            }

                            string name = TranslatedChartTitles.TotalTransactions;
                            GoogleSheetManager.ChartType chartType = isLine
                                ? GoogleSheetManager.ChartType.Line
                                : GoogleSheetManager.ChartType.Column;

                            await GoogleSheetManager.ExportMultiDataSetChartToGoogleSheetsAsync(combinedData, name, chartType);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.AverageShippingCosts:
                        {
                            SalesExpensesChartData chartData = LoadChart.LoadAverageShippingCostsChart(MainMenu_Form.Instance.AverageShippingCosts_Chart, isLine, canUpdateChart: false);
                            Dictionary<string, Dictionary<string, double>> combinedData = [];

                            foreach (string date in chartData.GetDateOrder())
                            {
                                combinedData[date] = new Dictionary<string, double>
                                {
                                    [LanguageManager.TranslateString("Purchases")] = chartData.GetExpensesForDate(date),
                                    [LanguageManager.TranslateString("Sales")] = chartData.GetSalesForDate(date)
                                };
                            }

                            string name = TranslatedChartTitles.AverageShippingCosts;
                            GoogleSheetManager.ChartType chartType = isLine
                                ? GoogleSheetManager.ChartType.Line
                                : GoogleSheetManager.ChartType.Column;

                            await GoogleSheetManager.ExportMultiDataSetChartToGoogleSheetsAsync(combinedData, name, chartType);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.GrowthRates:
                        {
                            SalesExpensesChartData chartData = LoadChart.LoadGrowthRateChart(MainMenu_Form.Instance.GrowthRates_Chart, exportToExcel: false, filePath: null, canUpdateChart: false);
                            Dictionary<string, Dictionary<string, double>> combinedData = [];

                            foreach (string date in chartData.GetDateOrder())
                            {
                                combinedData[date] = new Dictionary<string, double>
                                {
                                    [LanguageManager.TranslateString("Expenses growth %")] = chartData.GetExpensesForDate(date),
                                    [LanguageManager.TranslateString("Revenue growth %")] = chartData.GetSalesForDate(date)
                                };
                            }

                            string name = TranslatedChartTitles.GrowthRates;
                            GoogleSheetManager.ChartType chartType = GoogleSheetManager.ChartType.Spline;

                            await GoogleSheetManager.ExportMultiDataSetChartToGoogleSheetsAsync(combinedData, name, chartType);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.ReturnsOverTime:
                        {
                            SalesExpensesChartData chartData = LoadChart.LoadReturnsOverTimeChart(MainMenu_Form.Instance.ReturnsOverTime_Chart, isLine, canUpdateChart: false);
                            Dictionary<string, Dictionary<string, double>> combinedData = [];

                            foreach (string date in chartData.GetDateOrder())
                            {
                                combinedData[date] = new Dictionary<string, double>
                                {
                                    [LanguageManager.TranslateString("Purchase returns")] = chartData.GetExpensesForDate(date),
                                    [LanguageManager.TranslateString("Sale returns")] = chartData.GetSalesForDate(date)
                                };
                            }

                            string chartTitle = TranslatedChartTitles.ReturnsOverTime;
                            GoogleSheetManager.ChartType chartType = isLine
                                ? GoogleSheetManager.ChartType.Line
                                : GoogleSheetManager.ChartType.Column;

                            await GoogleSheetManager.ExportMultiDataSetCountChartToGoogleSheetsAsync(combinedData, chartTitle, chartType);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.ReturnReasons:
                        {
                            ChartCountData chartData = LoadChart.LoadReturnReasonsChart(MainMenu_Form.Instance.ReturnReasons_Chart, PieChartGrouping.Unlimited, canUpdateChart: false);
                            string chartTitle = TranslatedChartTitles.ReturnReasons;
                            string first = LanguageManager.TranslateString("Reasons");
                            string second = LanguageManager.TranslateString("# of returns");

                            await GoogleSheetManager.ExportCountChartToGoogleSheetsAsync(chartData.Data, chartTitle, GoogleSheetManager.ChartType.Pie, first, second);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.ReturnFinancialImpact:
                        {
                            SalesExpensesChartData chartData = LoadChart.LoadReturnFinancialImpactChart(MainMenu_Form.Instance.ReturnFinancialImpact_Chart, isLine, canUpdateChart: false);
                            Dictionary<string, Dictionary<string, double>> combinedData = [];

                            foreach (string date in chartData.GetDateOrder())
                            {
                                combinedData[date] = new Dictionary<string, double>
                                {
                                    [LanguageManager.TranslateString("Purchase return value")] = chartData.GetExpensesForDate(date),
                                    [LanguageManager.TranslateString("Sale return value")] = chartData.GetSalesForDate(date)
                                };
                            }

                            string chartTitle = TranslatedChartTitles.ReturnFinancialImpact;
                            GoogleSheetManager.ChartType chartType = isLine
                                ? GoogleSheetManager.ChartType.Line
                                : GoogleSheetManager.ChartType.Column;

                            await GoogleSheetManager.ExportMultiDataSetChartToGoogleSheetsAsync(combinedData, chartTitle, chartType);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.ReturnsByCategory:
                        {
                            ChartCountData chartData = LoadChart.LoadReturnsByCategoryChart(MainMenu_Form.Instance.ReturnsByCategory_Chart, PieChartGrouping.Unlimited, canUpdateChart: false);
                            string chartTitle = TranslatedChartTitles.ReturnsByCategory;
                            string first = LanguageManager.TranslateString("Categories");
                            string second = LanguageManager.TranslateString("# of returns");

                            await GoogleSheetManager.ExportCountChartToGoogleSheetsAsync(chartData.Data, chartTitle, GoogleSheetManager.ChartType.Pie, first, second);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.ReturnsByProduct:
                        {
                            ChartCountData chartData = LoadChart.LoadReturnsByProductChart(MainMenu_Form.Instance.ReturnsByProduct_Chart, PieChartGrouping.Unlimited, canUpdateChart: false);
                            string chartTitle = TranslatedChartTitles.ReturnsByProduct;
                            string first = LanguageManager.TranslateString("Products");
                            string second = LanguageManager.TranslateString("# of returns");

                            await GoogleSheetManager.ExportCountChartToGoogleSheetsAsync(chartData.Data, chartTitle, GoogleSheetManager.ChartType.Pie, first, second);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.PurchaseVsSaleReturns:
                        {
                            ChartCountData chartData = LoadChart.LoadPurchaseVsSaleReturnsChart(MainMenu_Form.Instance.PurchaseVsSaleReturns_Chart, canUpdateChart: false);
                            string chartTitle = TranslatedChartTitles.PurchaseVsSaleReturns;
                            string first = LanguageManager.TranslateString("Transaction Type");
                            string second = LanguageManager.TranslateString("# of returns");

                            await GoogleSheetManager.ExportCountChartToGoogleSheetsAsync(chartData.Data, chartTitle, GoogleSheetManager.ChartType.Pie, first, second);
                        }
                        break;

                    // Add Lost Products Google Sheets export cases
                    case MainMenu_Form.ChartDataType.LossesOverTime:
                        {
                            SalesExpensesChartData chartData = LoadChart.LoadLossesOverTimeChart(MainMenu_Form.Instance.LossesOverTime_Chart, isLine, canUpdateChart: false);
                            Dictionary<string, Dictionary<string, double>> combinedData = [];

                            foreach (string date in chartData.GetDateOrder())
                            {
                                combinedData[date] = new Dictionary<string, double>
                                {
                                    [LanguageManager.TranslateString("Purchase losses")] = chartData.GetExpensesForDate(date),
                                    [LanguageManager.TranslateString("Sale losses")] = chartData.GetSalesForDate(date)
                                };
                            }

                            string chartTitle = TranslatedChartTitles.LossesOverTime;
                            GoogleSheetManager.ChartType chartType = isLine
                                ? GoogleSheetManager.ChartType.Line
                                : GoogleSheetManager.ChartType.Column;

                            await GoogleSheetManager.ExportMultiDataSetCountChartToGoogleSheetsAsync(combinedData, chartTitle, chartType);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.LossReasons:
                        {
                            ChartCountData chartData = LoadChart.LoadLossReasonsChart(MainMenu_Form.Instance.LossReasons_Chart, PieChartGrouping.Unlimited, canUpdateChart: false);
                            string chartTitle = TranslatedChartTitles.LossReasons;
                            string first = LanguageManager.TranslateString("Reasons");
                            string second = LanguageManager.TranslateString("# of losses");

                            await GoogleSheetManager.ExportCountChartToGoogleSheetsAsync(chartData.Data, chartTitle, GoogleSheetManager.ChartType.Pie, first, second);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.LossFinancialImpact:
                        {
                            SalesExpensesChartData chartData = LoadChart.LoadLossFinancialImpactChart(MainMenu_Form.Instance.LossFinancialImpact_Chart, isLine, canUpdateChart: false);
                            Dictionary<string, Dictionary<string, double>> combinedData = [];

                            foreach (string date in chartData.GetDateOrder())
                            {
                                combinedData[date] = new Dictionary<string, double>
                                {
                                    [LanguageManager.TranslateString("Purchase loss value")] = chartData.GetExpensesForDate(date),
                                    [LanguageManager.TranslateString("Sale loss value")] = chartData.GetSalesForDate(date)
                                };
                            }

                            string chartTitle = TranslatedChartTitles.LossFinancialImpact;
                            GoogleSheetManager.ChartType chartType = isLine
                                ? GoogleSheetManager.ChartType.Line
                                : GoogleSheetManager.ChartType.Column;

                            await GoogleSheetManager.ExportMultiDataSetChartToGoogleSheetsAsync(combinedData, chartTitle, chartType);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.LossesByCategory:
                        {
                            ChartCountData chartData = LoadChart.LoadLossesByCategoryChart(MainMenu_Form.Instance.LossesByCategory_Chart, PieChartGrouping.Unlimited, canUpdateChart: false);
                            string chartTitle = TranslatedChartTitles.LossesByCategory;
                            string first = LanguageManager.TranslateString("Categories");
                            string second = LanguageManager.TranslateString("# of losses");

                            await GoogleSheetManager.ExportCountChartToGoogleSheetsAsync(chartData.Data, chartTitle, GoogleSheetManager.ChartType.Pie, first, second);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.LossesByProduct:
                        {
                            ChartCountData chartData = LoadChart.LoadLossesByProductChart(MainMenu_Form.Instance.LossesByProduct_Chart, PieChartGrouping.Unlimited, canUpdateChart: false);
                            string chartTitle = TranslatedChartTitles.LossesByProduct;
                            string first = LanguageManager.TranslateString("Products");
                            string second = LanguageManager.TranslateString("# of losses");

                            await GoogleSheetManager.ExportCountChartToGoogleSheetsAsync(chartData.Data, chartTitle, GoogleSheetManager.ChartType.Pie, first, second);
                        }
                        break;

                    case MainMenu_Form.ChartDataType.PurchaseVsSaleLosses:
                        {
                            ChartCountData chartData = LoadChart.LoadPurchaseVsSaleLossesChart(MainMenu_Form.Instance.PurchaseVsSaleLosses_Chart, canUpdateChart: false);
                            string chartTitle = TranslatedChartTitles.PurchaseVsSaleLosses;
                            string first = LanguageManager.TranslateString("Transaction Type");
                            string second = LanguageManager.TranslateString("# of losses");

                            await GoogleSheetManager.ExportCountChartToGoogleSheetsAsync(chartData.Data, chartTitle, GoogleSheetManager.ChartType.Pie, first, second);
                        }
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                // Don't show error message for user-initiated cancellation
                // This is expected behavior, not an error
            }
            catch (Exception ex)
            {
                Log.Error_ExportingChart(ex.Message);
            }
        }
        private static void ResetZoom(object sender, EventArgs e)
        {
            Control chart = (Control)Panel.Tag;

            if (chart is CartesianChart cartesianChart)
            {
                ResetCartesianChartZoom(cartesianChart);
            }

            Hide();
        }
        private static void ResetCartesianChartZoom(CartesianChart chart)
        {
            foreach (ICartesianAxis axis in chart.XAxes)
            {
                axis.MinLimit = null;
                axis.MaxLimit = null;
            }

            foreach (ICartesianAxis axis in chart.YAxes)
            {
                axis.MinLimit = null;
                axis.MaxLimit = null;
            }
        }

        // Other methods
        public static void Show(Control chart, Point mousePosition)
        {
            MainMenu_Form.Instance.ClosePanels();

            if (!ChartHasData(chart)) { return; }

            Form form = chart.FindForm();
            Panel.Tag = chart;
            Point localMousePosition = form.PointToClient(mousePosition);
            int formWidth = form.ClientSize.Width;
            int formHeight = form.ClientSize.Height;
            byte offset = ReadOnlyVariables.OffsetRightClickPanel;
            byte padding = ReadOnlyVariables.PaddingRightClickPanel;

            FlowLayoutPanel flowPanel = (FlowLayoutPanel)Panel.Controls[0];

            if (IsPieChart(chart))
            {
                flowPanel.Controls.Add(_exportExcel_Button);
                flowPanel.Controls.Add(_exportSheet_Button);
                flowPanel.Controls.Remove(_resetZoom_Button);
            }
            else if (IsGeoMap(chart))
            {
                flowPanel.Controls.Remove(_exportExcel_Button);
                flowPanel.Controls.Remove(_exportSheet_Button);
                flowPanel.Controls.Remove(_resetZoom_Button);
            }
            else
            {
                flowPanel.Controls.Add(_exportExcel_Button);
                flowPanel.Controls.Add(_exportSheet_Button);
                flowPanel.Controls.Add(_resetZoom_Button);
            }

            CustomControls.SetRightClickMenuHeight(Panel);

            // Calculate the horizontal position
            bool tooFarRight = false;
            if (Panel.Width + localMousePosition.X - offset + padding > formWidth)
            {
                Panel.Left = formWidth - Panel.Width - padding;
                tooFarRight = true;
            }
            else
            {
                Panel.Left = localMousePosition.X - offset;
            }

            // Calculate the vertical position
            if (localMousePosition.Y + Panel.Height + padding > formHeight)
            {
                Panel.Top = formHeight - Panel.Height - padding;
                if (!tooFarRight)
                {
                    Panel.Left += offset;
                }
            }
            else
            {
                Panel.Top = localMousePosition.Y;
            }

            form.Controls.Add(Panel);
            Panel.BringToFront();
        }
        public static void Hide()
        {
            Panel?.Parent?.Controls.Remove(Panel);
        }

        // Helper methods
        private static bool ChartHasData(Control chart)
        {
            return chart switch
            {
                CartesianChart cartesianChart => cartesianChart.Series?.Any() == true,
                PieChart pieChart => pieChart.Series?.Any() == true,
                GeoMap geoMap => geoMap.Series?.Any() == true,
                _ => false
            };
        }
        private static bool IsPieChart(Control chart)
        {
            return chart is PieChart;
        }
        private static bool IsGeoMap(Control chart)
        {
            return chart is GeoMap;
        }
        private static string GetChartTitle(Control chart)
        {
            return chart switch
            {
                CartesianChart cartesianChart => GetTitleText(cartesianChart.Title) ?? chart.Name ?? "Chart",
                PieChart pieChart => GetTitleText(pieChart.Title) ?? chart.Name ?? "Chart",
                _ => chart.Name ?? "Chart"
            };
        }
        private static string? GetTitleText(object title)
        {
            if (title is LabelVisual labelVisual)
            {
                return labelVisual.Text;
            }
            return null;
        }
    }
}
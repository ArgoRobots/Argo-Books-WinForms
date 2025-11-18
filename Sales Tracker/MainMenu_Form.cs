using Guna.UI2.WinForms;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.SkiaSharpView.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sales_Tracker.AISearch;
using Sales_Tracker.AnonymousData;
using Sales_Tracker.Charts;
using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.Encryption;
using Sales_Tracker.GridView;
using Sales_Tracker.Language;
using Sales_Tracker.LostProduct;
using Sales_Tracker.Properties;
using Sales_Tracker.Rentals;
using Sales_Tracker.ReportGenerator;
using Sales_Tracker.ReturnProduct;
using Sales_Tracker.Startup.Menus;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;
using System.ComponentModel;
using Timer = System.Windows.Forms.Timer;

namespace Sales_Tracker
{
    /// <summary>
    /// Main application form displaying transactions, charts, and analytics.
    /// </summary>
    public partial class MainMenu_Form : BaseForm
    {
        // Admin mode can only be enabled by directly setting it to true here
        public static bool IsAdminMode { get; } = true && Tools.IsRunningInVisualStudio();

        // Properties
        private static MainMenu_Form _instance;
        public static readonly string _noteTextKey = "note", _rowTagKey = "RowTag", _itemsKey = "Items", _purchaseDataKey = "PurchaseData", _tagKey = "Tag";
        private static int _chartTop, _analyticChartTop;
        private static Guna2Button _upgrade_Button;

        // Getters and setters
        public static MainMenu_Form Instance => _instance;
        public static List<string> ThingsThatHaveChangedInFile { get; } = [];
        public static List<string> SettingsThatHaveChangedInFile { get; } = [];
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string CurrencySymbol { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static bool IsProgramLoading { get; set; }
        public static string SelectedAccountant { get; set; }

        // Init.
        public MainMenu_Form()
        {
            InitializeComponent();
            _instance = this;

            IsProgramLoading = true;
            CurrencySymbol = Currency.GetSymbol();

            CustomControls.ConstructControls();
            ConstructMainCharts();
            ConstructControlsForAnalytics();
            InitiateSearchTimer();
            SetCompanyLabel();
            InitDataGridViews();
            LoadData();
            LoadCustomColumnHeaders();
            CreateUpgradeButtonIfNeeded();
            CompanyLogo.SetCompanyLogo();
            UpdateTheme();
            Guna2TextBoxIconHoverEffect.Initialize(Search_TextBox);
            SetAccessibleDescriptions();
            LanguageManager.UpdateLanguageForControl(this);
            SetToolTips();
            HideShowingResultsForLabel();
            ConstructTimeRangePanel();
            InitChartTags();
            AddEventHandlersToTextBoxes();
            AnimateButtons();
            AnimateCharts();
            InitializeAISearch();
            UpdateMainMenuFormText();
            ScaleButtonImages();
            _ = AnonymousDataManager.TryUploadDataOnStartupAsync();
            AnonymousDataManager.TrackSessionStart();
            NetSparkleUpdateManager.CheckForUpdates();

            PanelCloseFilter panelCloseFilter = new(this, ClosePanels,
                TextBoxManager.RightClickTextBox_Panel,
                RightClickDataGridViewRowMenu.Panel,
                RightClickChartMenu.Panel,
                CustomControls.FileMenu,
                CustomControls.RecentlyOpenedMenu,
                CustomControls.HelpMenu,
                CustomControls.ControlDropDown_Panel,
                CompanyLogo.CompanyLogoRightClick_Panel,
                ColumnVisibilityPanel.Panel,
                DateRangePanel,
                File_Button,
                Help_Button,
                TimeRange_Button,
                CustomControls.ControlsDropDown_Button);

            Application.AddMessageFilter(panelCloseFilter);

            _chartTop = Purchases_Button.Bottom + 20;
            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void ConstructMainCharts()
        {
            TotalPurchases_Chart = ConstructMainChart("purchaseTotals_Chart", true) as CartesianChart;
            DistributionOfPurchases_Chart = ConstructMainChart("purchaseDistribution_Chart", false) as PieChart;
            TotalSales_Chart = ConstructMainChart("saleTotals_Chart", true) as CartesianChart;
            DistributionOfSales_Chart = ConstructMainChart("saleDistribution_Chart", false) as PieChart;
            TotalRentals_Chart = ConstructMainChart("rentalTotals_Chart", true) as CartesianChart;
            DistributionOfRentals_Chart = ConstructMainChart("rentalDistribution_Chart", false) as PieChart;
            Profits_Chart = ConstructMainChart("profits_Chart", true) as CartesianChart;

            LoadChart.ConfigurePieChart(DistributionOfPurchases_Chart);
            LoadChart.ConfigurePieChart(DistributionOfSales_Chart);
            LoadChart.ConfigurePieChart(DistributionOfRentals_Chart);

            MouseClickChartManager.InitCharts([
                TotalPurchases_Chart, DistributionOfPurchases_Chart,
                TotalSales_Chart, DistributionOfSales_Chart,
                TotalRentals_Chart, DistributionOfRentals_Chart,
                Profits_Chart
            ]);
        }
        private Control ConstructMainChart(string name, bool isCartesian)
        {
            Control chart;

            if (isCartesian)
            {
                CartesianChart cartesianChart = new()
                {
                    Name = name
                };

                chart = cartesianChart;
            }
            else
            {
                PieChart pieChart = new()
                {
                    Name = name
                };

                chart = pieChart;
            }

            Controls.Add(chart);
            return chart;
        }
        private void SetToolTips()
        {
            CustomTooltip.SetToolTip(File_Button, "", "File");
            CustomTooltip.SetToolTip(Save_Button, "", "Save");
            CustomTooltip.SetToolTip(Help_Button, "", "Help");
        }
        public void ResetData()
        {
            CategoryPurchaseList.Clear();
            CategorySaleList.Clear();
            CategoryRentalList.Clear();
            AccountantList.Clear();
            CompanyList.Clear();
            Purchase_DataGridView.Rows.Clear();
            Sale_DataGridView.Rows.Clear();
            CustomerList.Clear();

            Search_TextBox.Clear();
        }
        private void InitDataGridViews()
        {
            DataGridViewManager.InitializeDataGridView(Purchase_DataGridView, "purchases_DataGridView", PurchaseColumnHeaders, null, this);
            Purchase_DataGridView.Tag = DataGridViewTag.SaleOrPurchase;
            Purchase_DataGridView.CellFormatting += DataGridView_CellFormatting;

            DataGridViewManager.InitializeDataGridView(Sale_DataGridView, "sales_DataGridView", SalesColumnHeaders, null, this);
            Sale_DataGridView.Tag = DataGridViewTag.SaleOrPurchase;
            Sale_DataGridView.CellFormatting += DataGridView_CellFormatting;

            DataGridViewManager.InitializeDataGridView(Rental_DataGridView, "rentals_DataGridView", RentalColumnHeaders, null, this);
            Rental_DataGridView.Tag = DataGridViewTag.SaleOrPurchase;
            Rental_DataGridView.CellFormatting += DataGridView_CellFormatting;
        }
        private void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;

            if (grid.Columns[e.ColumnIndex].Name == Column.HasReceipt.ToString())
            {
                string newReceipt = ReceiptManager.GetReceiptPathFromRow(grid.Rows[e.RowIndex]);

                if (!string.IsNullOrEmpty(newReceipt) && File.Exists(newReceipt))
                {
                    e.Value = "✓";
                    e.CellStyle.ForeColor = CustomColors.AccentGreen;
                }
                else
                {
                    e.Value = "✗";
                    e.CellStyle.ForeColor = CustomColors.AccentRed;
                }

                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }
        public void LoadData()
        {
            LoadCategoriesFromFile(Directories.CategoryPurchases_file, CategoryPurchaseList);
            LoadCategoriesFromFile(Directories.CategorySales_file, CategorySaleList);
            LoadCategoriesFromFile(Directories.CategoryRentals_file, CategoryRentalList);

            AccountantList = Directories.ReadAllLinesInFile(Directories.Accountants_file).ToList();
            CompanyList = Directories.ReadAllLinesInFile(Directories.Companies_file).ToList();

            LoadCustomersFromFile();
            RentalInventoryManager.LoadInventory();

            AddRowsFromFile(Purchase_DataGridView, SelectedOption.Purchases);
            AddRowsFromFile(Sale_DataGridView, SelectedOption.Sales);
            AddRowsFromFile(Rental_DataGridView, SelectedOption.Rentals);
        }
        private void LoadCustomColumnHeaders()
        {
            DataGridViewColumn chargedDifferenceColumn = Purchase_DataGridView.Columns[Column.ChargedDifference.ToString()];
            string existingHeaderText = chargedDifferenceColumn.HeaderText;
            string messageBoxText = "Having a charged difference is common and is usually due to taxes, duties, bank fees, exchange rate differences, or policy variations across countries.";
            chargedDifferenceColumn.HeaderCell = new DataGridViewImageHeaderCell(Resources.HelpGray, existingHeaderText, messageBoxText);

            DataGridViewColumn totalColumn = Sale_DataGridView.Columns[Column.Total.ToString()];
            existingHeaderText = totalColumn.HeaderText;
            messageBoxText = "The revenue excludes shipping, taxes, and fees.";
            totalColumn.HeaderCell = new DataGridViewImageHeaderCell(Resources.HelpGray, existingHeaderText, messageBoxText);
        }
        private static void LoadCategoriesFromFile(string filePath, List<Category> categoryList)
        {
            string json = Directories.ReadAllTextInFile(filePath);
            if (string.IsNullOrWhiteSpace(json)) { return; }

            List<Category>? loadedCategories = JsonConvert.DeserializeObject<List<Category>>(json);

            if (loadedCategories != null)
            {
                categoryList.AddRange(loadedCategories);
            }
        }
        private void LoadCustomersFromFile()
        {
            string json = Directories.ReadAllTextInFile(Directories.Customers_file);
            if (string.IsNullOrWhiteSpace(json)) { return; }

            List<Customer>? loadedCustomers = JsonConvert.DeserializeObject<List<Customer>>(json);

            if (loadedCustomers != null)
            {
                CustomerList.AddRange(loadedCustomers);
            }
        }
        public void LoadOrRefreshMainCharts(bool onlyLoadForLineCharts = false)
        {
            bool isLine = LineChart_ToggleSwitch.Checked;

            // Load purchase charts
            if (Selected == SelectedOption.Analytics || Purchase_DataGridView.Visible)
            {
                ChartData totalsData = LoadChart.LoadTotalsIntoChart(Purchase_DataGridView, TotalPurchases_Chart, isLine);
                string translatedExpenses = LanguageManager.TranslateString("Total expenses");
                SetChartTitle(TotalPurchases_Chart, $"{translatedExpenses}: {CurrencySymbol}{totalsData.Total:N2}");

                if (!onlyLoadForLineCharts)
                {
                    LoadChart.LoadDistributionIntoChart(Purchase_DataGridView, DistributionOfPurchases_Chart, PieChartGrouping.Top12);
                    SetChartTitle(DistributionOfPurchases_Chart, TranslatedChartTitles.ExpensesDistribution);
                }
            }

            // Load sale charts
            if (Selected == SelectedOption.Analytics || Sale_DataGridView.Visible)
            {
                ChartData totalsData = LoadChart.LoadTotalsIntoChart(Sale_DataGridView, TotalSales_Chart, isLine);
                string translatedRevenue = LanguageManager.TranslateString("Total revenue");
                SetChartTitle(TotalSales_Chart, $"{translatedRevenue}: {CurrencySymbol}{totalsData.Total:N2}");

                if (!onlyLoadForLineCharts)
                {
                    LoadChart.LoadDistributionIntoChart(Sale_DataGridView, DistributionOfSales_Chart, PieChartGrouping.Top12);
                    SetChartTitle(DistributionOfSales_Chart, TranslatedChartTitles.RevenueDistribution);
                }
            }

            // Load rental charts
            if (Selected == SelectedOption.Rentals || Rental_DataGridView.Visible)
            {
                ChartData totalsData = LoadChart.LoadTotalsIntoChart(Rental_DataGridView, TotalRentals_Chart, isLine);
                string translatedRentalRevenue = LanguageManager.TranslateString("Total rental revenue");
                SetChartTitle(TotalRentals_Chart, $"{translatedRentalRevenue}: {CurrencySymbol}{totalsData.Total:N2}");

                if (!onlyLoadForLineCharts)
                {
                    LoadChart.LoadDistributionIntoChart(Rental_DataGridView, DistributionOfRentals_Chart, PieChartGrouping.Top12);
                    string translatedDistribution = LanguageManager.TranslateString("Distribution of rental revenue");
                    SetChartTitle(DistributionOfRentals_Chart, translatedDistribution);
                }
            }

            // Always load profits chart
            ChartData profitsData = LoadChart.LoadProfitsIntoChart(Profits_Chart, isLine);
            SetProfitsChartTitle(profitsData.Total);
        }
        public static void SetChartTitle(Control chart, string title)
        {
            if (chart is CartesianChart cartesianChart)
            {
                cartesianChart.Title = CreateChartTitle(title);
            }
            else if (chart is PieChart pieChart)
            {
                pieChart.Title = CreateChartTitle(title);
            }
        }
        private static LabelVisual CreateChartTitle(string text) => new()
        {
            Text = text,
            TextSize = 25,
            Padding = new LiveChartsCore.Drawing.Padding(15),
            Paint = ChartColors.CreateSolidColorPaint(ChartColors.ToSKColor(CustomColors.Text))
        };
        private void SetProfitsChartTitle(double total)
        {
            string translatedPrefix = LanguageManager.TranslateString("Total profits");
            SetChartTitle(Profits_Chart, $"{translatedPrefix}: {CurrencySymbol}{total:N2}");
        }
        public void UpdateChartCurrencyFormats()
        {
            List<Control> mainCharts = [TotalPurchases_Chart, TotalSales_Chart, Profits_Chart];
            List<Control> analyticsCharts = [TotalExpensesVsSales_Chart, AverageTransactionValue_Chart, AverageShippingCosts_Chart];

            // Combine charts based on current view
            List<Control> chartsToUpdate = [.. mainCharts];
            if (Selected == SelectedOption.Analytics)
            {
                chartsToUpdate.AddRange(analyticsCharts);
            }

            // Update currency formats for all relevant charts
            foreach (Control chart in chartsToUpdate)
            {
                if (chart is CartesianChart cartesianChart)
                {
                    LoadChart.ApplyCurrencyFormatToChart(cartesianChart);
                    cartesianChart.Invalidate();
                }
            }
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);

            if (ThemeManager.IsDarkTheme())
            {
                Edit_Button.Image = Resources.EditWhite;
            }
            else
            {
                Edit_Button.Image = Resources.EditBlack;
            }

            MainTop_Panel.FillColor = CustomColors.ContentPanelBackground;
            Edit_Button.FillColor = CustomColors.ContentPanelBackground;
            Top_Panel.BackColor = CustomColors.ToolbarBackground;
            File_Button.FillColor = CustomColors.ToolbarBackground;
            Save_Button.FillColor = CustomColors.ToolbarBackground;
            if (_upgrade_Button != null)
            {
                _upgrade_Button.FillColor = CustomColors.ToolbarBackground;
            }
            Help_Button.FillColor = CustomColors.ToolbarBackground;

            ReselectButton();
        }
        private void ReselectButton()
        {
            if (Purchases_Button.BorderThickness == 2)
            {
                Purchases_Button.BorderColor = CustomColors.AccentBlue;
            }
            else if (Sales_Button.BorderThickness == 2)
            {
                Sales_Button.BorderColor = CustomColors.AccentBlue;
            }
            else if (Rentals_Button.BorderThickness == 2)
            {
                Rentals_Button.BorderColor = CustomColors.AccentBlue;
            }
            else if (Analytics_Button.BorderThickness == 2)
            {
                Analytics_Button.BorderColor = CustomColors.AccentBlue;
            }
        }
        private void SetAccessibleDescriptions()
        {
            CompanyName_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
            Quantity_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
            Shipping_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
            Tax_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
            Fee_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
            Discount_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
            ChargedDifference_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
            Price_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
            LineChart_Label.AccessibleDescription = AccessibleDescriptionManager.AlignRight;

            // Chart titles are saved in cache using a string, not the control
            TotalPurchases_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            DistributionOfPurchases_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            TotalSales_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            DistributionOfSales_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            TotalRentals_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            DistributionOfRentals_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            Profits_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;

            // Geographic Analysis Charts
            CountriesOfOrigin_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            CountriesOfDestination_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            CompaniesOfOrigin_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            WorldMap_GeoMap.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;

            // Operational Charts
            Accountants_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;

            // Performance and Growth Charts
            GrowthRates_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            TotalExpensesVsSales_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            AverageTransactionValue_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            TotalTransactions_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            AverageShippingCosts_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;

            // Returns Analysis Charts
            ReturnsOverTime_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            ReturnReasons_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            ReturnFinancialImpact_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            ReturnsByCategory_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            ReturnsByProduct_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            PurchaseVsSaleReturns_Chart.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
        }
        private void InitChartTags()
        {
            // Main charts
            TotalPurchases_Chart.Tag = ChartDataType.TotalExpenses;
            DistributionOfPurchases_Chart.Tag = ChartDataType.ExpensesDistribution;
            TotalSales_Chart.Tag = ChartDataType.TotalRevenue;
            DistributionOfSales_Chart.Tag = ChartDataType.RevenueDistribution;
            Profits_Chart.Tag = ChartDataType.TotalProfits;

            // Geographic Analysis Charts
            CountriesOfOrigin_Chart.Tag = ChartDataType.CountriesOfOrigin;
            CompaniesOfOrigin_Chart.Tag = ChartDataType.CompaniesOfOrigin;
            CountriesOfDestination_Chart.Tag = ChartDataType.CountriesOfDestination;
            WorldMap_GeoMap.Tag = ChartDataType.WorldMap;

            // Operational Charts
            Accountants_Chart.Tag = ChartDataType.AccountantsTransactions;

            // Performance and Growth Charts
            GrowthRates_Chart.Tag = ChartDataType.GrowthRates;
            TotalExpensesVsSales_Chart.Tag = ChartDataType.SalesVsExpenses;
            TotalTransactions_Chart.Tag = ChartDataType.TotalTransactions;
            AverageTransactionValue_Chart.Tag = ChartDataType.AverageTransactionValue;
            AverageShippingCosts_Chart.Tag = ChartDataType.AverageShippingCosts;

            // Returns Analysis Charts
            ReturnsOverTime_Chart.Tag = ChartDataType.ReturnsOverTime;
            ReturnReasons_Chart.Tag = ChartDataType.ReturnReasons;
            ReturnFinancialImpact_Chart.Tag = ChartDataType.ReturnFinancialImpact;
            ReturnsByCategory_Chart.Tag = ChartDataType.ReturnsByCategory;
            ReturnsByProduct_Chart.Tag = ChartDataType.ReturnsByProduct;
            PurchaseVsSaleReturns_Chart.Tag = ChartDataType.PurchaseVsSaleReturns;

            // Losses Analysis Charts
            LossesOverTime_Chart.Tag = ChartDataType.LossesOverTime;
            LossReasons_Chart.Tag = ChartDataType.LossReasons;
            LossFinancialImpact_Chart.Tag = ChartDataType.LossFinancialImpact;
            LossesByCategory_Chart.Tag = ChartDataType.LossesByCategory;
            LossesByProduct_Chart.Tag = ChartDataType.LossesByProduct;
            PurchaseVsSaleLosses_Chart.Tag = ChartDataType.PurchaseVsSaleLosses;
        }
        private void AddEventHandlersToTextBoxes()
        {
            TextBoxManager.Attach(Search_TextBox);
        }
        public void CreateUpgradeButtonIfNeeded()
        {
            if (!Properties.Settings.Default.LicenseActivated)
            {
                float scale = DpiHelper.GetRelativeDpiScale();
                int height = Top_Panel.Height;
                int scaledImageSize = (int)(32 * scale);

                _upgrade_Button = new Guna2Button
                {
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Image = Resources.Upgrade,
                    ImageSize = new Size(scaledImageSize, scaledImageSize),
                    Size = new Size(height, height),
                    Left = Help_Button.Left - height
                };
                _upgrade_Button.Click += Upgrade_Button_Click;
                Top_Panel.Controls.Add(_upgrade_Button);
                CustomTooltip.SetToolTip(_upgrade_Button, "", "Upgrade to full version");
            }
        }
        public void RemoveUpgradeButton()
        {
            Top_Panel.Controls.Remove(_upgrade_Button);
        }
        public void AnimateButtons()
        {
            IEnumerable<Guna2Button> buttons =
            [
               Purchases_Button,
               Sales_Button,
               Rentals_Button,
               Analytics_Button,
               AddPurchase_Button,
               AddSale_Button,
               Products_Button,
               Categories_Button,
               Companies_Button,
               Customers_Button,
               Accountants_Button,
               TimeRange_Button,
            ];
            CustomControls.AnimateButtons(buttons);
        }
        public void AnimateCharts()
        {
            bool enableAnimations = Properties.Settings.Default.AnimateCharts;

            // Apply to all charts
            foreach (Control chart in GetAllCharts())
            {
                switch (chart)
                {
                    case CartesianChart cartesianChart:
                        cartesianChart.EasingFunction = enableAnimations ? LiveChartsCore.EasingFunctions.QuadraticOut : null;
                        break;
                    case PieChart pieChart:
                        pieChart.EasingFunction = enableAnimations ? LiveChartsCore.EasingFunctions.QuadraticOut : null;
                        break;
                }
            }
        }
        private static void InitializeAISearch()
        {
            if (Properties.Settings.Default.AISearchEnabled)
            {
                string _chatGptApiKey = DotEnv.Get("CHATGPT_API_KEY");
                if (!string.IsNullOrEmpty(_chatGptApiKey))
                {
                    AISearchExtensions.InitializeAISearch(_chatGptApiKey);
                }
                else
                {
                    Log.Write(1, "AI Search disabled: No API key found");
                }
            }
        }
        private void ScaleButtonImages()
        {
            DpiHelper.ScaleImageSize(File_Button);
            DpiHelper.ScaleImageSize(Save_Button);
            DpiHelper.ScaleImageSize(Help_Button);
        }

        // Add rows from file
        public static void AddRowsFromFile(Guna2DataGridView dataGridView, SelectedOption selected)
        {
            string filePath = DataGridViewManager.GetFilePathForDataGridView(selected);
            if (!ValidateFile(filePath))
            {
                return;
            }

            List<Dictionary<string, object>>? rowsData = DeserializeJsonFile(filePath);
            if (rowsData == null)
            {
                return;
            }

            foreach (Dictionary<string, object> rowData in rowsData)
            {
                ProcessRow(dataGridView, rowData);
            }

            bool hasVisibleRows = DataGridViewManager.HasVisibleRowsExcludingReturnedOrLost(dataGridView);
            LabelManager.ManageNoDataLabelOnControl(hasVisibleRows, dataGridView);
        }
        private static bool ValidateFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                try
                {
                    Directories.CreateFile(filePath);
                    Log.Write(2, $"Created new data file: {filePath}");
                }
                catch
                {
                    Log.Write(1, $"Failed to create data file: {filePath}");
                    return false;
                }
            }
            return true;
        }
        private static List<Dictionary<string, object>>? DeserializeJsonFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);
        }
        private static void ProcessRow(Guna2DataGridView dataGridView, Dictionary<string, object> rowData)
        {
            string?[] cellValues = ExtractCellValues(rowData, dataGridView);
            int rowIndex = dataGridView.Rows.Add(cellValues.Cast<object>().ToArray());

            ProcessRowTag(dataGridView, rowData, rowIndex);
            ProcessNoteText(dataGridView, rowData, rowIndex);
        }
        private static string?[] ExtractCellValues(Dictionary<string, object> rowData, Guna2DataGridView dataGridView)
        {
            // Get column names from the DataGridView
            List<string> columnNames = dataGridView.Columns.Cast<DataGridViewColumn>()
                .Select(col => col.Name)
                .ToList();

            // Build cell values array in the correct column order
            List<string?> cellValues = [];
            foreach (string columnName in columnNames)
            {
                // Skip special columns
                if (columnName == Column.HasReceipt.ToString())
                {
                    continue;
                }

                // Get value from rowData, or use empty cell if missing (for migration)
                if (rowData.TryGetValue(columnName, out object? value))
                {
                    cellValues.Add(value?.ToString());
                }
                else
                {
                    // Column doesn't exist in saved data (e.g., Customer column for old data)
                    cellValues.Add(ReadOnlyVariables.EmptyCell);
                }
            }

            return [.. cellValues];
        }
        private static void ProcessRowTag(Guna2DataGridView dataGridView, Dictionary<string, object> rowData, int rowIndex)
        {
            if (!rowData.TryGetValue(_rowTagKey, out object value) || value is not JObject jsonObject)
            {
                return;
            }

            Dictionary<string, object>? tagObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonObject.ToString());
            if (tagObject == null)
            {
                return;
            }

            if (TryProcessItemsAndPurchaseData(tagObject, out (List<string>, TagData)? rowTag))
            {
                dataGridView.Rows[rowIndex].Tag = rowTag;
            }
            else if (TryProcessTagStringAndPurchaseData(tagObject, out (string, TagData)? stringPurchaseTag))
            {
                dataGridView.Rows[rowIndex].Tag = stringPurchaseTag;
            }
            else
            {
                TryProcessTagData(jsonObject, dataGridView.Rows[rowIndex]);
            }
        }
        private static bool TryProcessItemsAndPurchaseData(Dictionary<string, object> tagObject, out (List<string>, TagData)? rowTag)
        {
            rowTag = null;

            if (!tagObject.TryGetValue(_itemsKey, out object? itemsElement) || itemsElement is not JArray itemsArray)
            {
                return false;
            }

            if (!tagObject.TryGetValue(_purchaseDataKey, out object? purchaseDataElement) || purchaseDataElement is not JObject purchaseDataObject)
            {
                return false;
            }

            List<string> itemList = itemsArray.Select(e => e.ToString()).ToList();
            TagData? purchaseData = JsonConvert.DeserializeObject<TagData>(purchaseDataObject.ToString());

            if (purchaseData != null)
            {
                rowTag = (itemList, purchaseData);
                return true;
            }

            return false;
        }
        private static bool TryProcessTagStringAndPurchaseData(Dictionary<string, object> tagObject, out (string, TagData)? tag)
        {
            tag = null;

            if (!tagObject.TryGetValue(_tagKey, out object? tagStringElement) ||
                !tagObject.TryGetValue(_purchaseDataKey, out object? purchaseDataElement))
            {
                return false;
            }

            string? tagString = tagStringElement?.ToString();
            TagData? purchaseData = JsonConvert.DeserializeObject<TagData>(purchaseDataElement?.ToString());

            if (tagString != null && purchaseData != null)
            {
                tag = (tagString, purchaseData);
                return true;
            }

            return false;
        }
        private static void TryProcessTagData(JObject jsonObject, DataGridViewRow row)
        {
            TagData? tagData = JsonConvert.DeserializeObject<TagData>(jsonObject.ToString());
            if (tagData != null)
            {
                row.Tag = tagData;
            }
        }
        private static void ProcessNoteText(Guna2DataGridView dataGridView, Dictionary<string, object> rowData, int rowIndex)
        {
            if (!rowData.TryGetValue(_noteTextKey, out object noteValue))
            {
                return;
            }

            DataGridViewCell noteCell = dataGridView.Rows[rowIndex].Cells[Column.Note.ToString()];
            noteCell.Value = ReadOnlyVariables.Show_text;
            noteCell.Tag = noteValue;
            DataGridViewManager.AddUnderlineToCell(noteCell);
        }
        public static void SetReceiptCellToX(DataGridViewCell cell)
        {
            cell.Value = "✗";
            cell.Style.ForeColor = CustomColors.AccentRed;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        // Form event handlers
        private void MainMenu_Form_Load(object sender, EventArgs e)
        {
            Sales_Button.PerformClick();
            Sale_DataGridView.ClearSelection();

            IsProgramLoading = false;

            SortDataGridViewsByDate();
            CenterAndResizeControls();

            Total_Panel.Location = new Point(SelectedDataGridView.Left, SelectedDataGridView.Top + SelectedDataGridView.Height);
            Total_Panel.Width = SelectedDataGridView.Width;
        }
        private void MainMenu_form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
            Log.Write(2, "Argo Sales Tracker has finished starting");

            // Check if the user has accepted the EULA
            bool eulaAccepted = DataFileManager.GetBoolValue(GlobalAppDataSettings.EULAAccepted);
            if (!eulaAccepted)
            {
                // The user hasn't accepted the EULA yet, show the EULA form
                EULA_Form eulaForm = new();
                if (eulaForm.ShowDialog() != DialogResult.OK)
                {
                    // User declined the EULA, the application will exit in EULA_Form.Decline_Button_Click()
                    return;
                }
            }

            // Check if the welcome form should be shown
            bool showWelcomeForm = DataFileManager.GetBoolValue(GlobalAppDataSettings.ShowWelcomeForm);
            if (showWelcomeForm)
            {
                new Welcome_Form().ShowDialog();
            }
        }
        private void MainMenu_Form_ResizeBegin(object sender, EventArgs e)
        {
            ClosePanels();
        }
        private void MainMenu_form_Resize(object sender, EventArgs e)
        {
            if (_instance == null) { return; }
            CenterAndResizeControls();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Handle menu navigation keys
            if (keyData is Keys.Up or Keys.Down or Keys.Right or Keys.Left or Keys.Enter)
            {
                foreach (Guna2Panel panel in GetMenus())
                {
                    if (Controls.Contains(panel))
                    {
                        MenuKeyShortcutManager.HandlePanelKeyDown(panel, keyData);
                        return true;
                    }
                }
            }

            // Ctrl+S (Save)
            if (keyData == (Keys.Control | Keys.S))
            {
                CustomControls.SaveAll();
                return true;
            }

            // Ctrl+Shift+S (Save As)
            if (keyData == (Keys.Control | Keys.Shift | Keys.S))
            {
                ArgoCompany.SaveAs();
                return true;
            }

            // Ctrl+E (Export)
            if (keyData == (Keys.Control | Keys.E))
            {
                Tools.OpenForm(new Export_Form());
                return true;
            }

            // Ctrl+L (Open logs)
            if (keyData == (Keys.Control | Keys.L))
            {
                Tools.OpenForm(new Log_Form());
                return true;
            }

            // Alt+F4 (Close program)
            if (keyData == (Keys.Alt | Keys.F4))
            {
                if (!ArgoCompany.AskUserToSave())
                {
                    return true;  // Prevent closing
                }
                // Let it close if AskUserToSave returns true
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void MainMenu_form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log.Write(2, "Closing Argo Sales Tracker");

            if (!ArgoCompany.AskUserToSave())
            {
                e.Cancel = true;
                Log.Write(2, "Close canceled");
                return;
            }

            AnonymousDataManager.TrackSessionEnd();
            ThemeChangeDetector.StopListeningForThemeChanges();
            Log.SaveLogs();
            ArgoCompany.ApplicationMutex?.Dispose();
            Environment.Exit(0);  // Make sure all processes are fully closed
        }

        // Center and resize controls
        public void CenterAndResizeControls()
        {
            if (IsProgramLoading) { return; }

            byte spaceBetweenCharts = 20, chartWidthOffset = 35;

            // Handle dropdown menu for narrow windows - check if AddPurchase_Button overlaps with Edit_Button
            if (Edit_Button.Right + 25 > AddPurchase_Button.Left)
            {
                AddControlsDropDown();
            }
            else if (MainTop_Panel.Controls.Contains(CustomControls.ControlsDropDown_Button))
            {
                RemoveControlsDropDown();
            }

            if (Search_TextBox.Text != "")
            {
                CenterShowingResultsLabel();
            }

            if (Selected == SelectedOption.Analytics)
            {
                PositionTabButtons();
                _analyticChartTop = _analyticsTabButtons_Panel.Bottom + 20;
                LayoutChartsForTab(_selectedTabKey, spaceBetweenCharts);
            }
            else
            {
                int chartHeight = ClientSize.Height switch
                {
                    > 1400 => 500,
                    > 1000 => 400,
                    _ => 300
                };
                SetMainChartsHeight(chartHeight);

                // Regular view layout for main charts
                int chartWidth = ClientSize.Width / 3 - chartWidthOffset;

                // Calculate X positions for charts
                int leftX = (ClientSize.Width - 3 * chartWidth - spaceBetweenCharts * 2) / 2;
                int middleX = leftX + chartWidth + spaceBetweenCharts;
                int rightX = middleX + chartWidth + spaceBetweenCharts;

                // Position the currently visible charts
                Control totalsChart = Rental_DataGridView.Visible ? TotalRentals_Chart :
                                      Sale_DataGridView.Visible ? TotalSales_Chart : TotalPurchases_Chart;
                Control distributionChart = Rental_DataGridView.Visible ? DistributionOfRentals_Chart :
                                            Sale_DataGridView.Visible ? DistributionOfSales_Chart : DistributionOfPurchases_Chart;

                SetChartPosition(totalsChart, new Size(chartWidth, chartHeight), leftX, _chartTop);
                SetChartPosition(distributionChart, new Size(chartWidth, chartHeight), middleX, _chartTop);
                SetChartPosition(Profits_Chart, new Size(chartWidth, chartHeight), rightX, _chartTop);

                // Position DataGridView
                SelectedDataGridView.Size = new Size(ClientSize.Width - 65,
                    ClientSize.Height - MainTop_Panel.Height - Top_Panel.Height - chartHeight - totalsChart.Top + 35);
                SelectedDataGridView.Location = new Point((ClientSize.Width - SelectedDataGridView.Width) / 2,
                    ClientSize.Height - MainTop_Panel.Height - Top_Panel.Height - SelectedDataGridView.Height + 50);
            }
        }
        private void PositionTabButtons()
        {
            if (_tabButtons == null || _analyticsTabButtons_Panel == null)
            {
                return;
            }

            const int startX = 10;
            int buttonWidth = Width >= 1600 ? 200 : 180;
            int fontSize = Width >= 1600 ? 10 : 9;
            int buttonSpacing = Width >= 1600 ? 12 : 10;
            int imageSize = Width >= 1600 ? 25 : 20;

            // Position and size buttons
            for (int i = 0; i < _tabButtons.Count; i++)
            {
                Guna2Button button = _tabButtons[i];

                button.Size = new Size(buttonWidth, 45);
                button.Location = new Point(i * (buttonWidth + buttonSpacing) + startX, 10);
                button.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
                button.ImageSize = new Size(imageSize, imageSize);
            }

            // Update panel size
            int totalWidth = _tabButtons.Count * (buttonWidth + buttonSpacing) - buttonSpacing + (startX * 2);
            _analyticsTabButtons_Panel.Size = new Size(totalWidth, 65);
            _analyticsTabButtons_Panel.Location = new Point((ClientSize.Width - _analyticsTabButtons_Panel.Width) / 2, Purchases_Button.Bottom + 20);
        }
        private void LayoutChartsForTab(AnalyticsTab tabKey, int spacing)
        {
            int startY = _analyticChartTop;
            int availableHeight = ClientSize.Height - startY - 10;
            int availableWidth = ClientSize.Width - 80;
            const int maxChartWidth = 800;
            const int maxChartHeight = 550;

            List<Control> charts = _tabControls[tabKey];

            switch (tabKey)
            {
                case AnalyticsTab.Overview:
                    // 2x2 grid for overview
                    if (charts.Count >= 4)
                    {
                        int chartWidth = Math.Min(maxChartWidth, (availableWidth - spacing) / 2);
                        int chartHeight = Math.Min(maxChartHeight, (availableHeight - spacing) / 2);

                        int startX = (ClientSize.Width - (chartWidth * 2 + spacing)) / 2;

                        SetChartPosition(charts[0], new Size(chartWidth, chartHeight), startX, startY);
                        SetChartPosition(charts[1], new Size(chartWidth, chartHeight), startX + chartWidth + spacing, startY);
                        SetChartPosition(charts[2], new Size(chartWidth, chartHeight), startX, startY + chartHeight + spacing);
                        SetChartPosition(charts[3], new Size(chartWidth, chartHeight), startX + chartWidth + spacing, startY + chartHeight + spacing);
                    }
                    break;

                case AnalyticsTab.Geographic:
                    // Position controls at the top
                    _worldMapControls_Panel.Location = new Point((
                        ClientSize.Width - _worldMapControls_Panel.Width) / 2,
                        _analyticsTabButtons_Panel.Bottom + 10);

                    if (charts.Count >= 4)
                    {
                        Control geoMap = charts[0];

                        // GeoMap takes top 60% of space
                        int geoMapY = _worldMapControls_Panel.Bottom + 10;
                        int geoMapHeight = (int)((availableHeight - _worldMapControls_Panel.Height - 20) * 0.6);
                        int geoMapWidth = availableWidth;

                        SetChartPosition(geoMap, new Size(geoMapWidth, geoMapHeight),
                            (ClientSize.Width - geoMapWidth) / 2, geoMapY);

                        // Pie charts take remaining space with max constraints
                        List<Control> pieCharts = charts.Skip(1).Take(3).ToList();
                        if (pieCharts.Count == 3)
                        {
                            int smallChartWidth = Math.Min(maxChartWidth, (availableWidth - (2 * spacing)) / 3);
                            int smallChartHeight = Math.Min(maxChartHeight, availableHeight - geoMapHeight - _worldMapControls_Panel.Height - 30);
                            int smallChartsY = geoMapY + geoMapHeight + spacing;
                            int smallChartsStartX = (ClientSize.Width - (smallChartWidth * 3 + spacing * 2)) / 2;

                            for (int i = 0; i < pieCharts.Count; i++)
                            {
                                int x = smallChartsStartX + (smallChartWidth + spacing) * i;
                                SetChartPosition(pieCharts[i], new Size(smallChartWidth, smallChartHeight), x, smallChartsY);
                            }
                        }
                    }
                    break;

                case AnalyticsTab.Financial:
                    // 2x2 grid for financial
                    if (charts.Count >= 4)
                    {
                        int chartWidth = Math.Min(maxChartWidth, (availableWidth - spacing) / 2);
                        int chartHeight = Math.Min(maxChartHeight, (availableHeight - spacing) / 2);

                        int startX = (ClientSize.Width - (chartWidth * 2 + spacing)) / 2;

                        for (int i = 0; i < 4; i++)
                        {
                            int x = startX + (chartWidth + spacing) * (i % 2);
                            int y = startY + (chartHeight + spacing) * (i / 2);
                            SetChartPosition(charts[i], new Size(chartWidth, chartHeight), x, y);
                        }
                    }
                    break;

                case AnalyticsTab.Performance:
                    // 3 charts in a row
                    if (charts.Count >= 3)
                    {
                        int chartWidth = Math.Min(maxChartWidth, (availableWidth - (2 * spacing)) / 3);
                        int chartHeight = Math.Min(maxChartHeight, availableHeight);

                        int startX = (ClientSize.Width - (chartWidth * 3 + spacing * 2)) / 2;

                        for (int i = 0; i < 3; i++)
                        {
                            int x = startX + (chartWidth + spacing) * i;
                            SetChartPosition(charts[i], new Size(chartWidth, chartHeight), x, startY);
                        }
                    }
                    break;

                case AnalyticsTab.Operational:
                    // 2 charts side by side
                    if (charts.Count >= 2)
                    {
                        int chartWidth = Math.Min(maxChartWidth, (availableWidth - spacing) / 2);
                        int chartHeight = Math.Min(maxChartHeight, availableHeight - 50);  // Leave space for controls

                        int startX = (ClientSize.Width - (chartWidth * 2 + spacing)) / 2;

                        SetChartPosition(charts[0], new Size(chartWidth, chartHeight), startX, startY);
                        SetChartPosition(charts[1], new Size(chartWidth, chartHeight), startX + chartWidth + spacing, startY);

                        // Position controls below the shipping chart
                        int shippingChartX = startX + chartWidth + spacing;
                        int controlsY = startY + chartHeight + 20;

                        IncludeFreeShipping_CheckBox.Location = new Point(shippingChartX, controlsY);

                        int labelX = IncludeFreeShipping_CheckBox.Right - 2;
                        int labelY = IncludeFreeShipping_CheckBox.Top + (IncludeFreeShipping_CheckBox.Height / 2) - (_includeFreeShipping_Label.Height / 2);
                        _includeFreeShipping_Label.Location = new Point(labelX, labelY);
                    }
                    break;

                case AnalyticsTab.Returns:
                    // 2x3 grid layout
                    if (charts.Count >= 6)
                    {
                        int chartWidth = Math.Min(maxChartWidth, (availableWidth - (2 * spacing)) / 3);
                        int chartHeight = Math.Min(maxChartHeight, (availableHeight - spacing) / 2);

                        int startX = (ClientSize.Width - (chartWidth * 3 + spacing * 2)) / 2;

                        // Top row - 3 charts
                        SetChartPosition(charts[0], new Size(chartWidth, chartHeight), startX, startY);
                        SetChartPosition(charts[1], new Size(chartWidth, chartHeight), startX + chartWidth + spacing, startY);
                        SetChartPosition(charts[2], new Size(chartWidth, chartHeight), startX + (chartWidth + spacing) * 2, startY);

                        // Bottom row - 3 charts
                        SetChartPosition(charts[3], new Size(chartWidth, chartHeight), startX, startY + chartHeight + spacing);
                        SetChartPosition(charts[4], new Size(chartWidth, chartHeight), startX + chartWidth + spacing, startY + chartHeight + spacing);
                        SetChartPosition(charts[5], new Size(chartWidth, chartHeight), startX + (chartWidth + spacing) * 2, startY + chartHeight + spacing);
                    }
                    break;

                case AnalyticsTab.LostProducts:
                    // 2x3 grid layout
                    if (charts.Count >= 6)
                    {
                        int chartWidth = Math.Min(maxChartWidth, (availableWidth - (2 * spacing)) / 3);
                        int chartHeight = Math.Min(maxChartHeight, (availableHeight - spacing) / 2);

                        int startX = (ClientSize.Width - (chartWidth * 3 + spacing * 2)) / 2;

                        // Top row - 3 charts
                        SetChartPosition(charts[0], new Size(chartWidth, chartHeight), startX, startY);
                        SetChartPosition(charts[1], new Size(chartWidth, chartHeight), startX + chartWidth + spacing, startY);
                        SetChartPosition(charts[2], new Size(chartWidth, chartHeight), startX + (chartWidth + spacing) * 2, startY);

                        // Bottom row - 3 charts
                        SetChartPosition(charts[3], new Size(chartWidth, chartHeight), startX, startY + chartHeight + spacing);
                        SetChartPosition(charts[4], new Size(chartWidth, chartHeight), startX + chartWidth + spacing, startY + chartHeight + spacing);
                        SetChartPosition(charts[5], new Size(chartWidth, chartHeight), startX + (chartWidth + spacing) * 2, startY + chartHeight + spacing);
                    }
                    break;

                case AnalyticsTab.Customers:
                    // 2x3 grid layout
                    if (charts.Count >= 6)
                    {
                        int chartWidth = Math.Min(maxChartWidth, (availableWidth - (2 * spacing)) / 3);
                        int chartHeight = Math.Min(maxChartHeight, (availableHeight - spacing) / 2);

                        int startX = (ClientSize.Width - (chartWidth * 3 + spacing * 2)) / 2;

                        // Top row - 3 charts
                        SetChartPosition(charts[0], new Size(chartWidth, chartHeight), startX, startY);
                        SetChartPosition(charts[1], new Size(chartWidth, chartHeight), startX + chartWidth + spacing, startY);
                        SetChartPosition(charts[2], new Size(chartWidth, chartHeight), startX + (chartWidth + spacing) * 2, startY);

                        // Bottom row - 3 charts
                        SetChartPosition(charts[3], new Size(chartWidth, chartHeight), startX, startY + chartHeight + spacing);
                        SetChartPosition(charts[4], new Size(chartWidth, chartHeight), startX + chartWidth + spacing, startY + chartHeight + spacing);
                        SetChartPosition(charts[5], new Size(chartWidth, chartHeight), startX + (chartWidth + spacing) * 2, startY + chartHeight + spacing);
                    }
                    break;
            }
        }
        private void SetMainChartsHeight(int height)
        {
            TotalPurchases_Chart.Height = height;
            DistributionOfPurchases_Chart.Height = height;
            TotalSales_Chart.Height = height;
            DistributionOfSales_Chart.Height = height;
            TotalRentals_Chart.Height = height;
            DistributionOfRentals_Chart.Height = height;
            Profits_Chart.Height = height;
        }
        private static void SetChartPosition(Control chart, Size size, int left, int top)
        {
            chart.Size = size;
            chart.Location = new Point(left, top);

            if (chart is PieChart pieChart)
            {
                pieChart.LegendPosition = LoadChart.GetLegendPosition(pieChart);
            }
        }
        private void AddControlsDropDown()
        {
            if (!MainTop_Panel.Controls.Contains(CustomControls.ControlsDropDown_Button))
            {
                CustomControls.ControlsDropDown_Button.Location = new Point(
                    MainTop_Panel.Width - CustomControls.ControlsDropDown_Button.Width - 11,
                    (MainTop_Panel.Height - CustomControls.ControlsDropDown_Button.Height) / 2);

                MainTop_Panel.Controls.Add(CustomControls.ControlsDropDown_Button);

                foreach (Control button in GetMainTopButtons())
                {
                    button.Visible = false;
                }
            }
        }
        private void RemoveControlsDropDown()
        {
            if (MainTop_Panel.Controls.Contains(CustomControls.ControlsDropDown_Button))
            {
                MainTop_Panel.Controls.Remove(CustomControls.ControlsDropDown_Button);

                // Also remove the dropdown panel if it's open
                Controls.Remove(CustomControls.ControlDropDown_Panel);

                foreach (Control button in GetMainTopButtons())
                {
                    button.Visible = true;
                }
            }
        }
        private Control[] GetMainTopButtons()
        {
            return [
                Accountants_Button, Categories_Button, Companies_Button,
                Products_Button, Customers_Button, ManageRentals_Button,
                AddSale_Button, AddPurchase_Button];
        }

        // Event handlers - top bar
        private void File_Button_Click(object sender, EventArgs e)
        {
            if (ToggleMenu(CustomControls.FileMenu, File_Button, Resources.FileGray, Resources.FileWhite, false))
            {
                Controls.Remove(CustomControls.RecentlyOpenedMenu);
            }
        }
        private void Save_Button_Click(object sender, EventArgs e)
        {
            CustomControls.SaveAll();
        }
        private void Save_Button_MouseDown(object sender, MouseEventArgs e)
        {
            Save_Button.Image = Resources.SaveWhite;
        }
        private void Save_Button_MouseUp(object sender, MouseEventArgs e)
        {
            Save_Button.Image = Resources.SaveGray;
        }
        private void Upgrade_Button_Click(object sender, EventArgs e)
        {
            Tools.OpenForm(new Upgrade_Form());
        }
        private void Help_Button_Click(object sender, EventArgs e)
        {
            ToggleMenu(CustomControls.HelpMenu, Help_Button, Resources.HelpGray, Resources.HelpWhite, true);
        }

        /// <summary>
        /// Toggles the visibility of a menu and updates the button's image accordingly.
        /// </summary>
        /// <returns>True if the menu was closed; False if the menu was opened.</returns>
        private bool ToggleMenu(Control menu, Guna2Button button, Image grayImage, Image whiteImage, bool alignRight)
        {
            if (Controls.Contains(menu))
            {
                Controls.Remove(menu);
                button.Image = grayImage;
                return true;
            }
            else
            {
                ClosePanels();
                button.Image = whiteImage;

                // Calculate X position based on left or right alignment
                int xPosition = alignRight
                    ? button.Left - menu.Width + button.Width
                    : button.Left;

                menu.Location = new Point(xPosition, Top_Panel.Height);
                Controls.Add(menu);
                menu.BringToFront();
                return false;
            }
        }

        // Event handlers
        private void Purchases_Button_Click(object sender, EventArgs e)
        {
            if (Selected == SelectedOption.Purchases) { return; }

            Purchase_DataGridView.ColumnWidthChanged -= DataGridViewManager.DataGridView_ColumnWidthChanged;

            ShowMainControls();
            SelectedDataGridView = Purchase_DataGridView;
            Purchase_DataGridView.Visible = true;
            Sale_DataGridView.Visible = false;
            Rental_DataGridView.Visible = false;

            // Show purchase charts, hide sale and rental charts
            TotalPurchases_Chart.Visible = true;
            DistributionOfPurchases_Chart.Visible = true;
            TotalSales_Chart.Visible = false;
            DistributionOfSales_Chart.Visible = false;
            TotalRentals_Chart.Visible = false;
            DistributionOfRentals_Chart.Visible = false;

            SelectButton(Purchases_Button);
            CenterAndResizeControls();
            RefreshDataGridViewAndCharts();

            Purchase_DataGridView.ColumnWidthChanged += DataGridViewManager.DataGridView_ColumnWidthChanged;
            AlignTotalLabels();
            UpdateTotalLabels();
            Search_TextBox.PlaceholderText = LanguageManager.TranslateString("Search for purchases");
        }
        private void Sales_Button_Click(object sender, EventArgs e)
        {
            if (Selected == SelectedOption.Sales) { return; }

            Sale_DataGridView.ColumnWidthChanged -= DataGridViewManager.DataGridView_ColumnWidthChanged;

            ShowMainControls();
            SelectedDataGridView = Sale_DataGridView;
            Sale_DataGridView.Visible = true;
            Purchase_DataGridView.Visible = false;
            Rental_DataGridView.Visible = false;

            // Show sale charts, hide purchase and rental charts
            TotalSales_Chart.Visible = true;
            DistributionOfSales_Chart.Visible = true;
            TotalPurchases_Chart.Visible = false;
            DistributionOfPurchases_Chart.Visible = false;
            TotalRentals_Chart.Visible = false;
            DistributionOfRentals_Chart.Visible = false;

            SelectButton(Sales_Button);
            CenterAndResizeControls();
            RefreshDataGridViewAndCharts();

            Sale_DataGridView.ColumnWidthChanged += DataGridViewManager.DataGridView_ColumnWidthChanged;
            AlignTotalLabels();
            UpdateTotalLabels();
            Search_TextBox.PlaceholderText = LanguageManager.TranslateString("Search for sales");
        }
        private void Rentals_Button_Click(object sender, EventArgs e)
        {
            if (Selected == SelectedOption.Rentals) { return; }

            Rental_DataGridView.ColumnWidthChanged -= DataGridViewManager.DataGridView_ColumnWidthChanged;

            ShowMainControls();
            SelectedDataGridView = Rental_DataGridView;
            Rental_DataGridView.Visible = true;
            Purchase_DataGridView.Visible = false;
            Sale_DataGridView.Visible = false;

            // Show rental charts, hide other charts
            TotalRentals_Chart.Visible = true;
            DistributionOfRentals_Chart.Visible = true;
            TotalSales_Chart.Visible = false;
            DistributionOfSales_Chart.Visible = false;
            TotalPurchases_Chart.Visible = false;
            DistributionOfPurchases_Chart.Visible = false;

            SelectButton(Rentals_Button);
            CenterAndResizeControls();
            RefreshDataGridViewAndCharts();

            Rental_DataGridView.ColumnWidthChanged += DataGridViewManager.DataGridView_ColumnWidthChanged;
            AlignTotalLabels();
            UpdateTotalLabels();
            Search_TextBox.PlaceholderText = LanguageManager.TranslateString("Search for rentals");
        }
        private void Analytics_Button_Click(object sender, EventArgs e)
        {
            if (Selected == SelectedOption.Analytics) { return; }

            SelectButton(Analytics_Button);
            ShowAnalyticsControls();
            CenterAndResizeControls();
            LoadOrRefreshAnalyticsCharts();
        }
        private void AddPurchase_Button_Click(object sender, EventArgs e)
        {
            Tools.OpenForm(new AddPurchase_Form());
        }
        private void AddSale_Button_Click(object sender, EventArgs e)
        {
            Tools.OpenForm(new AddSale_Form());
        }
        private void ManageRentals_Button_Click(object sneder, EventArgs e)
        {
            Tools.OpenForm(new Rentals_Form());
        }
        private void ManageAccountants_Button_Click(object sender, EventArgs e)
        {
            Tools.OpenForm(new Accountants_Form());
        }
        private void ManageProducts_Button_Click(object sender, EventArgs e)
        {
            Tools.OpenForm(new Products_Form(true));
        }
        private void ManageCompanies_Button_Click(object sender, EventArgs e)
        {
            Tools.OpenForm(new Companies_Form());
        }
        private void ManageCategories_Button_Click(object sender, EventArgs e)
        {
            Tools.OpenForm(new Categories_Form(true));
        }
        private void Customers_Button_Click(object sender, EventArgs e)
        {
            Tools.OpenForm(new Customers_Form());
        }
        private void LineChart_ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            LoadOrRefreshMainCharts(true);
            LoadOrRefreshAnalyticsCharts(true);
            SetAllAnalyticTabsAsNotLoaded();
        }
        private void Edit_Button_Click(object sender, EventArgs e)
        {
            CustomControls.Rename_TextBox.Text = CompanyName_Label.Text;
            CustomControls.Rename_TextBox.Font = CompanyName_Label.Font;
            Controls.Add(CustomControls.Rename_TextBox);
            CustomControls.Rename_TextBox.Location = new Point(CompanyName_Label.Left, CompanyName_Label.Top + CompanyName_Label.Parent.Top - 1);
            CustomControls.Rename_TextBox.Size = new Size(300, CompanyName_Label.Height + 2);
            CustomControls.Rename_TextBox.Focus();
            CustomControls.Rename_TextBox.SelectAll();
            CustomControls.Rename_TextBox.BringToFront();
            MainTop_Panel.Controls.Remove(Edit_Button);
            MainTop_Panel.Controls.Remove(CompanyName_Label);
        }
        private async void Search_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsProgramLoading) { return; }

            bool isAIQuery = Properties.Settings.Default.AISearchEnabled
                && Properties.Settings.Default.LicenseActivated
                && Search_TextBox.Text.StartsWith('!');

            // Process AI search only on Enter key
            if (e.KeyCode == Keys.Enter && isAIQuery)
            {
                // Check for internet connection
                if (!await InternetConnectionManager.CheckInternetAndShowMessageAsync("AI search", true))
                {
                    Log.Write(1, "AI search cancelled - no internet connection");
                    return;
                }

                ShowingResultsFor_Label.Text = LanguageManager.TranslateString("AI search in progress...");
                CenterShowingResultsLabel();
                await Search_TextBox.EnhanceSearchAsync();
            }
            // Show AI search prompt for AI queries
            else if (isAIQuery)
            {
                ShowingResultsFor_Label.Text = LanguageManager.TranslateString("Press enter to begin AI search");
                CenterShowingResultsLabel();
            }
            // Start timer for regular searches
            else if (!_timerRunning)
            {
                _timerRunning = true;
                _search_Timer.Start();
            }
        }
        private void Search_TextBox_TextChanged(object sender, EventArgs e)
        {
            if (Search_TextBox.Text == "")
            {
                AISearchExtensions.ResetQuery();

                // Start timer for regular searches
                if (!_timerRunning)
                {
                    _timerRunning = true;
                    _search_Timer.Start();
                }
            }
        }
        private void Search_TextBox_IconRightClick(object sender, EventArgs e)
        {
            Search_TextBox.Clear();
        }

        // DateRange
        public static Guna2Panel DateRangePanel { get; set; }
        private static void ConstructTimeRangePanel()
        {
            DateRange_Form dateRange_Form = new();
            DateRangePanel = dateRange_Form.Main_Panel;
        }
        private void TimeRange_Button_Click(object sender, EventArgs e)
        {
            if (Controls.Contains(DateRangePanel))
            {
                CloseDateRangePanel();
            }
            else
            {
                ClosePanels();

                // Set the location for the panel
                DateRangePanel.Location = new Point(
                    TimeRange_Button.Right - DateRangePanel.Width,
                    TimeRange_Button.Bottom);

                Controls.Add(DateRangePanel);
                DateRangePanel.BringToFront();
            }
        }
        public void CloseDateRangePanel()
        {
            Controls.Remove(DateRangePanel);
        }

        // Search timer
        private Timer _search_Timer;
        private bool _timerRunning = false;
        private void InitiateSearchTimer()
        {
            _search_Timer = new()
            {
                Interval = 300
            };
            _search_Timer.Tick += SearchTimer_Tick;
        }
        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            _search_Timer.Stop();
            _timerRunning = false;
            RefreshDataGridViewAndCharts();
        }

        // Methods for Event handlers
        private void SelectButton(Guna2Button button)
        {
            UnselectButtons();
            button.BorderThickness = 2;
            button.BorderColor = CustomColors.AccentBlue;
        }
        private static bool IsButtonSelected(Guna2Button button)
        {
            return button.BorderThickness == 2;
        }
        private void UnselectButtons()
        {
            Purchases_Button.BorderThickness = 1;
            Sales_Button.BorderThickness = 1;
            Rentals_Button.BorderThickness = 1;
            Analytics_Button.BorderThickness = 1;
            Purchases_Button.BorderColor = CustomColors.ControlBorder;
            Sales_Button.BorderColor = CustomColors.ControlBorder;
            Rentals_Button.BorderColor = CustomColors.ControlBorder;
            Analytics_Button.BorderColor = CustomColors.ControlBorder;
        }

        // Company label
        public void RenameCompany()
        {
            if (!Controls.Contains(CustomControls.Rename_TextBox))
            {
                return;
            }

            // If the name did not change
            if (CustomControls.Rename_TextBox.Text == CompanyName_Label.Text)
            {
                CloseRenameCompany();
                return;
            }

            // If the company name already exists in this directory
            string parentDir = Directory.GetParent(Directories.ArgoCompany_file).FullName;
            string filePath = parentDir + @"\" + CustomControls.Rename_TextBox.Text + ArgoFiles.ArgoCompanyFileExtension;
            string fileName = Path.GetFileName(filePath);
            List<string> files = Directory.GetFiles(parentDir).ToList();

            bool fileExists = files.Any(f => Path.GetFileName(f).Equals(fileName, StringComparison.OrdinalIgnoreCase));

            if (fileExists)
            {
                // Get a new name for the file
                string name = Path.GetFileNameWithoutExtension(filePath);
                List<string> fileNames = Directories.GetListOfAllFilesWithoutExtensionInDirectory(parentDir);

                string suggestedCompanyName = Tools.AddNumberForAStringThatAlreadyExists(name, fileNames);

                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                    "Rename company",
                    "Do you want to rename '{0}' to '{1}'? There is already a company with the same name.",
                    CustomMessageBoxIcon.Question,
                    CustomMessageBoxButtons.OkCancel,
                    name, suggestedCompanyName);

                if (result == CustomMessageBoxResult.Ok)
                {
                    CustomControls.Rename_TextBox.Text = suggestedCompanyName;
                }
                else { return; }
            }

            CloseRenameCompany();

            CompanyName_Label.Text = CustomControls.Rename_TextBox.Text;
            ArgoCompany.Rename(CustomControls.Rename_TextBox.Text);
            CustomControls.Rename_TextBox.Clear();
            SetEditButtonLocation();
            CenterAndResizeControls();
            UpdateMainMenuFormText();

            string message = $"Renamed program to: {CompanyName_Label.Text}";
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(ThingsThatHaveChangedInFile, 3, message);
        }
        private void CloseRenameCompany()
        {
            Controls.Remove(CustomControls.Rename_TextBox);
            MainTop_Panel.Controls.Add(Edit_Button);
            MainTop_Panel.Controls.Add(CompanyName_Label);
        }
        public void SetCompanyLabel()
        {
            CompanyName_Label.Text = Directories.CompanyName;
            SetEditButtonLocation();
        }
        public void SetEditButtonLocation()
        {
            Edit_Button.Left = CompanyName_Label.Right + 5;
        }

        // Search DataGridView getters and setters
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime? SortFromDate { get; set; } = null;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime? SortToDate { get; set; } = null;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TimeSpan? SortTimeSpan { get; set; } = TimeSpan.MaxValue;

        // Search DataGridView methods
        public void RefreshDataGridViewAndCharts()
        {
            if (Selected == SelectedOption.Analytics)
            {
                ApplyFiltersToDataGridView(Purchase_DataGridView);
                ApplyFiltersToDataGridView(Sale_DataGridView);
                LoadOrRefreshAnalyticsCharts();
            }
            else
            {
                ApplyFiltersToDataGridView(SelectedDataGridView);
                LoadOrRefreshMainCharts();
                SelectedDataGridView.ClearSelection();
            }
        }
        private void ApplyFiltersToDataGridView(Guna2DataGridView dataGridView)
        {
            // Suspend layout updates during filtering for better performance
            dataGridView.SuspendLayout();

            bool usingCustomDateRange = SortFromDate != null && SortToDate != null;
            string displayedSearchText = Search_TextBox.Text.Trim();
            string effectiveSearchText = AISearchExtensions.GetEffectiveSearchQuery(displayedSearchText);

            bool filterExists = (SortTimeSpan != null && SortTimeSpan != TimeSpan.MaxValue) ||
                usingCustomDateRange ||
                !string.IsNullOrEmpty(effectiveSearchText);

            if (filterExists)
            {
                ShowShowingResultsForLabel();

                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    // Try to parse the date from the correct column depending on which filter is used
                    if (!DateTime.TryParse(row.Cells[SalesColumnHeaders[Column.Date]].Value.ToString(), out DateTime rowDate) &&
                        !DateTime.TryParse(row.Cells[PurchaseColumnHeaders[Column.Date]].Value.ToString(), out rowDate))
                    {
                        row.Visible = false;
                        continue;
                    }

                    bool isVisibleByDate = true;

                    if (usingCustomDateRange)
                    {
                        isVisibleByDate = rowDate >= SortFromDate && rowDate <= SortToDate;
                    }
                    else if (SortTimeSpan != null && SortTimeSpan != TimeSpan.MaxValue)
                    {
                        isVisibleByDate = rowDate >= DateTime.Now - SortTimeSpan;
                    }

                    bool isVisibleBySearch = string.IsNullOrEmpty(effectiveSearchText) ||
                        SearchDataGridView.FilterRowByAdvancedSearch(row, effectiveSearchText);

                    row.Visible = isVisibleByDate && isVisibleBySearch;
                }
            }
            else
            {
                HideShowingResultsForLabel();

                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    row.Visible = true;
                }
            }

            bool hasVisibleRows = DataGridViewManager.HasVisibleRowsExcludingReturnedOrLost(dataGridView);

            LabelManager.ManageNoDataLabelOnControl(hasVisibleRows, dataGridView);
            DataGridViewManager.UpdateRowColors(dataGridView);
            UpdateTotalLabels();

            dataGridView.ResumeLayout(true);
        }
        private void ShowShowingResultsForLabel()
        {
            string text = BuildShowingResultsText();
            ShowingResultsFor_Label.Text = text;
            CenterShowingResultsLabel();
        }
        private string BuildShowingResultsText()
        {
            string displayedSearchText = Search_TextBox.Text.Trim();
            string searchDisplay = AISearchExtensions.GetDisplayQuery(displayedSearchText);

            // Helper methods to check if filters are active
            bool hasSearchFilter = !string.IsNullOrEmpty(searchDisplay);
            bool hasTimeSpanFilter = SortTimeSpan != null && SortTimeSpan != TimeSpan.MaxValue;
            bool hasDateRangeFilter = SortFromDate != null && SortToDate != null;

            // Base case: no search, no time filter, no date range
            if (!hasSearchFilter && !hasTimeSpanFilter && !hasDateRangeFilter)
            {
                return LanguageManager.TranslateString("Showing all results");
            }

            // Case 1: Search term only
            if (hasSearchFilter && !hasTimeSpanFilter && !hasDateRangeFilter)
            {
                string template = LanguageManager.TranslateString("Showing results for '{0}'");
                return string.Format(template, searchDisplay);
            }

            // Case 2: Time span only
            if (!hasSearchFilter && hasTimeSpanFilter && !hasDateRangeFilter)
            {
                string timeSpanText = GetTimeSpanText(SortTimeSpan!.Value);
                string template = LanguageManager.TranslateString("Showing results for\nthe last {0}");
                return string.Format(template, timeSpanText);
            }

            // Case 3: Custom date range only
            if (!hasSearchFilter && !hasTimeSpanFilter && hasDateRangeFilter)
            {
                string fromDate = Tools.FormatDate(SortFromDate!.Value);
                string toDate = Tools.FormatDate(SortToDate!.Value);
                string template = LanguageManager.TranslateString("Showing results from\n{0} to {1}");
                return string.Format(template, fromDate, toDate);
            }

            // Case 4: Search term + time span
            if (hasSearchFilter && hasTimeSpanFilter && !hasDateRangeFilter)
            {
                string timeSpanText = GetTimeSpanText(SortTimeSpan!.Value);
                string template = LanguageManager.TranslateString("Showing results for '{0}'\nin the last {1}");
                return string.Format(template, searchDisplay, timeSpanText);
            }

            // Case 5: Search term + custom date range
            if (hasSearchFilter && !hasTimeSpanFilter && hasDateRangeFilter)
            {
                string fromDate = Tools.FormatDate((DateTime)SortFromDate!);
                string toDate = Tools.FormatDate((DateTime)SortToDate!);
                string template = LanguageManager.TranslateString("Showing results for '{0}'\nfrom {1} to {2}");
                return string.Format(template, searchDisplay, fromDate, toDate);
            }

            // Fallback
            return LanguageManager.TranslateString("Showing results");
        }
        private static string GetTimeSpanText(TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 365)
            {
                int years = (int)(timeSpan.TotalDays / 365);
                return years == 1
                    ? LanguageManager.TranslateString("year")
                    : string.Format(LanguageManager.TranslateString("{0} years"), years);
            }
            else
            {
                int days = (int)timeSpan.TotalDays;
                return days == 1
                    ? LanguageManager.TranslateString("day")
                    : string.Format(LanguageManager.TranslateString("{0} days"), days);
            }
        }
        private void CenterShowingResultsLabel()
        {
            bool visible = ShowingResultsFor_Label.Right < LineChart_Label.Left - 5;
            ShowingResultsFor_Label.Visible = visible;

            if (visible)
            {
                ShowingResultsFor_Label.Location = new Point(
                    (ClientSize.Width - ShowingResultsFor_Label.Width) / 2,
                    MainTop_Panel.Bottom + (_chartTop - MainTop_Panel.Bottom - ShowingResultsFor_Label.Height) / 2);
            }
        }
        public void HideShowingResultsForLabel()
        {
            ShowingResultsFor_Label.Visible = false;
        }
        private void SortDataGridViewsByDate()
        {
            string dateColumnHeader = SalesColumnHeaders[Column.Date];
            Sale_DataGridView.Sort(Sale_DataGridView.Columns[dateColumnHeader], ListSortDirection.Ascending);

            dateColumnHeader = PurchaseColumnHeaders[Column.Date];
            Purchase_DataGridView.Sort(Purchase_DataGridView.Columns[dateColumnHeader], ListSortDirection.Ascending);
        }

        // List getters
        public List<Category> CategoryPurchaseList { get; } = [];
        public List<Category> CategorySaleList { get; } = [];
        public List<Category> CategoryRentalList { get; } = [];
        public List<string> AccountantList { get; private set; } = [];
        public List<string> CompanyList { get; private set; } = [];
        public List<Customer> CustomerList { get; private set; } = [];

        // List methods
        public static void AddProductToCategoryByName(List<Category> categoryList, string categoryName, Product product)
        {
            foreach (Category category in categoryList)
            {
                if (category.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
                {
                    category.AddProduct(product);
                    return;
                }
            }
        }
        public static string? GetCountryProductIsFrom(List<Category> categoryList, string productName)
        {
            foreach (Category category in categoryList)
            {
                foreach (Product product in category.ProductList)
                {
                    if (product.Name.Equals(productName, StringComparison.OrdinalIgnoreCase))
                    {
                        return product.CountryOfOrigin;
                    }
                }
            }
            return null;
        }
        public static string? GetCompanyProductIsFrom(List<Category> categoryList, string productName)
        {
            foreach (Category category in categoryList)
            {
                foreach (Product product in category.ProductList)
                {
                    if (product.Name.Equals(productName, StringComparison.OrdinalIgnoreCase))
                    {
                        return product.CompanyOfOrigin;
                    }
                }
            }
            return null;
        }
        public static string? GetCategoryNameProductIsFrom(List<Category> categoryList, string productName, string companyName)
        {
            foreach (Category category in categoryList)
            {
                foreach (Product product in category.ProductList)
                {
                    if (product.Name.Equals(productName, StringComparison.OrdinalIgnoreCase) &&
                        product.CompanyOfOrigin.Equals(companyName, StringComparison.OrdinalIgnoreCase))
                    {
                        return category.Name;
                    }
                }
            }
            return null;
        }
        public static bool DoesProductExistInCategory(string productName, string companyName, List<Category> categories, string categoryName)
        {
            foreach (Category category in categories)
            {
                if (category.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (Product product in category.ProductList)
                    {
                        if (product.Name.Equals(productName, StringComparison.OrdinalIgnoreCase) &&
                           product.CompanyOfOrigin.Equals(companyName, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                    // Return since we found and checked the category
                    return false;
                }
            }
            return false;
        }
        public static Category? GetCategoryCategoryNameIsFrom(List<Category> categoryList, string categoryName)
        {
            foreach (Category category in categoryList)
            {
                if (category.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
                {
                    return category;
                }
            }
            return null;
        }
        public static Category? GetCategoryProductNameIsFrom(List<Category> categoryList, string productName, string companyName)
        {
            foreach (Category category in categoryList)
            {
                foreach (Product product in category.ProductList)
                {
                    if (product.Name.Equals(productName, StringComparison.OrdinalIgnoreCase) &&
                        product.CompanyOfOrigin.Equals(companyName, StringComparison.OrdinalIgnoreCase))
                    {
                        return category;
                    }
                }
            }
            return null;
        }
        public static Product? GetProductProductNameIsFrom(List<Category> categoryList, string productName, string companyName)
        {
            foreach (Category category in categoryList)
            {
                foreach (Product product in category.ProductList)
                {
                    if (product.Name.Equals(productName, StringComparison.OrdinalIgnoreCase) &&
                        product.CompanyOfOrigin.Equals(companyName, StringComparison.OrdinalIgnoreCase))
                    {
                        return product;
                    }
                }
            }
            return null;
        }
        public static bool DoesCategoryHaveProducts(string categoryName, List<Category> sourceList)
        {
            Category? category = GetCategoryCategoryNameIsFrom(sourceList, categoryName);
            return category != null && category.ProductList.Count > 0;
        }

        /// <summary>
        /// Gets a list of formatted product names for items available for sale.
        /// Format: "CompanyOfOrigin > CategoryName > ProductName"
        /// </summary>
        public List<string> GetProductSaleNames() => GetFormattedProductNames(CategorySaleList);

        /// <summary>
        /// Gets a list of formatted product names for items available for purchase.
        /// Format: "CompanyOfOrigin > CategoryName > ProductName"
        /// </summary>
        public List<string> GetProductPurchaseNames() => GetFormattedProductNames(CategoryPurchaseList);

        /// <summary>
        /// Gets a list of formatted rentable items.
        /// Format: "CompanyOfOrigin > CategoryName > ProductName"
        /// </summary>
        public List<string> GetFormattedRentableProductNames()
        {
            List<string> names = [];
            foreach (Category category in CategoryRentalList)
            {
                foreach (Product product in category.ProductList)
                {
                    names.Add($"{product.CompanyOfOrigin} > {category.Name} > {product.Name}");
                }
            }
            return names;
        }

        /// <summary>
        /// Helper method to format product names from a category list.
        /// </summary>
        private static List<string> GetFormattedProductNames(List<Category> categories)
        {
            List<string> names = [];
            foreach (Category category in categories)
            {
                foreach (Product product in category.ProductList)
                {
                    names.Add($"{product.CompanyOfOrigin} > {category.Name} > {product.Name}");
                }
            }
            return names;
        }

        // DataGridView properties
        public SelectedOption Selected { get => GetSelectedOption(); }
        private SelectedOption GetSelectedOption()
        {
            return ActiveForm switch
            {
                Accountants_Form => SelectedOption.Accountants,
                Companies_Form => SelectedOption.Companies,
                Categories_Form => Categories_Form.Instance.Purchase_RadioButton.Checked ? SelectedOption.CategoryPurchases :
                                   Categories_Form.Instance.Rent_RadioButton.Checked ? SelectedOption.CategoryRentals : SelectedOption.CategorySales,
                Products_Form => Products_Form.Instance.Purchase_RadioButton.Checked ? SelectedOption.ProductPurchases : SelectedOption.ProductSales,
                Receipts_Form => SelectedOption.Receipts,
                ItemsInTransaction_Form => IsButtonSelected(Purchases_Button) ? SelectedOption.ItemsInPurchase : SelectedOption.ItemsInSale,
                Customers_Form => SelectedOption.Customers,
                AddCustomer_Form => SelectedOption.Customers,
                Rentals_Form => SelectedOption.Rentals,
                _ => GetButtonBasedOption()
            };
        }
        private SelectedOption GetButtonBasedOption()
        {
            if (IsButtonSelected(Purchases_Button)) { return SelectedOption.Purchases; }
            if (IsButtonSelected(Sales_Button)) { return SelectedOption.Sales; }
            if (IsButtonSelected(Rentals_Button)) { return SelectedOption.Rentals; }
            if (IsButtonSelected(Analytics_Button)) { return SelectedOption.Analytics; }

            return SelectedOption.Analytics;  // Default fallback
        }

        // DataGridView getters
        public Guna2DataGridView Purchase_DataGridView { get; private set; } = new();
        public Guna2DataGridView Sale_DataGridView { get; private set; } = new();
        public Guna2DataGridView Rental_DataGridView { get; private set; } = new();
        public Guna2DataGridView SelectedDataGridView { get; private set; }
        public CartesianChart Profits_Chart { get; private set; }
        public CartesianChart TotalRentals_Chart { get; private set; }
        public PieChart DistributionOfRentals_Chart { get; private set; }

        // DataGridView enums
        public enum SelectedOption
        {
            Purchases,
            Sales,
            Rentals,
            ProductPurchases,
            ProductSales,
            CategoryPurchases,
            CategorySales,
            CategoryRentals,
            Accountants,
            Receipts,
            Companies,
            Analytics,
            ItemsInPurchase,
            ItemsInSale,
            Customers
        }
        public enum Column
        {
            ID,
            Accountant,
            Customer,
            Product,
            Category,
            Country,
            Company,
            Date,
            TotalItems,
            PricePerUnit,
            Shipping,
            Tax,
            Fee,
            Discount,
            ChargedDifference,
            Total,
            Note,
            HasReceipt,
            // These are used for importing & exporting spreadsheets
            Receipt,
            IsReturned,
            ReturnDate,
            ReturnReason,
            ReturnedBy,
            ReturnedItems,

            IsLost,
            LostDate,
            LostReason,
            LostBy,
            LostItems
        }
        public readonly Dictionary<Column, string> PurchaseColumnHeaders = new()
        {
            { Column.ID, "Order #" },
            { Column.Accountant, "Accountant" },
            { Column.Product, "Product / Service" },
            { Column.Category, "Category" },
            { Column.Country, "Country of origin" },
            { Column.Company, "Company of origin" },
            { Column.Date, "Date" },
            { Column.TotalItems, "Quantity" },
            { Column.PricePerUnit, "Price per unit" },
            { Column.Shipping, "Shipping" },
            { Column.Tax, "Tax" },
            { Column.Fee, "Fee" },
            { Column.Discount, "Discount" },
            { Column.ChargedDifference, "Charged difference" },
            { Column.Total, "Total expenses" },
            { Column.Note, "Notes" },
            { Column.HasReceipt, "Has receipt" }
        };
        public readonly Dictionary<Column, string> SalesColumnHeaders = new()
        {
            { Column.ID, "Sale #" },
            { Column.Accountant, "Accountant" },
            { Column.Customer, "Customer" },
            { Column.Product, "Product / Service" },
            { Column.Category, "Category" },
            { Column.Country, "Country of destination" },
            { Column.Company, "Company of origin" },
            { Column.Date, "Date" },
            { Column.TotalItems, "Quantity" },
            { Column.PricePerUnit, "Price per unit" },
            { Column.Shipping, "Shipping" },
            { Column.Tax, "Tax" },
            { Column.Fee, "Fee" },
            { Column.Discount, "Discount" },
            { Column.ChargedDifference, "Charged difference" },
            { Column.Total, "Total revenue" },
            { Column.Note, "Notes" },
            { Column.HasReceipt, "Has receipt" }
        };

        public readonly Dictionary<Column, string> RentalColumnHeaders = new()
        {
            { Column.ID, "Rental #" },
            { Column.Accountant, "Accountant" },
            { Column.Product, "Product / Service" },
            { Column.Category, "Category" },
            { Column.Country, "Country of destination" },
            { Column.Company, "Company of origin" },
            { Column.Date, "Date" },
            { Column.TotalItems, "Total items" },
            { Column.PricePerUnit, "Price per unit" },
            { Column.Shipping, "Shipping" },
            { Column.Tax, "Tax" },
            { Column.Fee, "Fee" },
            { Column.Discount, "Discount" },
            { Column.ChargedDifference, "Charged difference" },
            { Column.Total, "Total rental revenue" },
            { Column.Note, "Notes" },
            { Column.HasReceipt, "Has receipt" }
        };
        public enum DataGridViewTag
        {
            SaleOrPurchase,
            Category,
            Company,
            Product,
            Accountant,
            ItemsInPurchase,
            Customer,
            RentalInventory
        }

        // DataGridView methods
        public IEnumerable<DataGridViewRow> GetAllRows()
        {
            return Purchase_DataGridView.Rows.Cast<DataGridViewRow>()
                .Concat(Sale_DataGridView.Rows.Cast<DataGridViewRow>());
        }

        // Total labels
        public void AlignTotalLabels()
        {
            string quantityColumn = Column.TotalItems.ToString();
            string taxColumn = Column.Tax.ToString();
            string shippingColumn = Column.Shipping.ToString();
            string feeColumn = Column.Fee.ToString();
            string discountColumn = Column.Discount.ToString();
            string chargedDifference = Column.ChargedDifference.ToString();
            string totalPriceColumn = Column.Total.ToString();

            Quantity_Label.Left = SelectedDataGridView.GetCellDisplayRectangle(SelectedDataGridView.Columns[quantityColumn].Index, -1, true).Left;
            Quantity_Label.Width = SelectedDataGridView.Columns[quantityColumn].Width;

            Tax_Label.Left = SelectedDataGridView.GetCellDisplayRectangle(SelectedDataGridView.Columns[taxColumn].Index, -1, true).Left;
            Tax_Label.Width = SelectedDataGridView.Columns[taxColumn].Width;

            Shipping_Label.Left = SelectedDataGridView.GetCellDisplayRectangle(SelectedDataGridView.Columns[shippingColumn].Index, -1, true).Left;
            Shipping_Label.Width = SelectedDataGridView.Columns[shippingColumn].Width;

            Fee_Label.Left = SelectedDataGridView.GetCellDisplayRectangle(SelectedDataGridView.Columns[feeColumn].Index, -1, true).Left;
            Fee_Label.Width = SelectedDataGridView.Columns[feeColumn].Width;

            Discount_Label.Left = SelectedDataGridView.GetCellDisplayRectangle(SelectedDataGridView.Columns[discountColumn].Index, -1, true).Left;
            Discount_Label.Width = SelectedDataGridView.Columns[feeColumn].Width;

            ChargedDifference_Label.Left = SelectedDataGridView.GetCellDisplayRectangle(SelectedDataGridView.Columns[chargedDifference].Index, -1, true).Left;
            ChargedDifference_Label.Width = SelectedDataGridView.Columns[chargedDifference].Width;

            Price_Label.Left = SelectedDataGridView.GetCellDisplayRectangle(SelectedDataGridView.Columns[totalPriceColumn].Index, -1, true).Left;
            Price_Label.Width = SelectedDataGridView.Columns[totalPriceColumn].Width;
        }
        public void UpdateTotalLabels()
        {
            if (Selected != SelectedOption.Purchases && Selected != SelectedOption.Sales && Selected != SelectedOption.Rentals)
            {
                return;
            }

            Total_Panel.Visible = DataGridViewManager.HasVisibleRowsExcludingReturnedOrLost(SelectedDataGridView);

            int totalQuantity = 0;

            decimal totalTax = 0,
                totalShipping = 0,
                fee = 0,
                discount = 0,
                chargedDifference = 0,
                totalPrice = 0;

            foreach (DataGridViewRow row in SelectedDataGridView.Rows)
            {
                // Skip invalid rows (includes fully returned and fully lost items)
                if (!LoadChart.IsRowValid(row))
                {
                    continue;
                }

                // For partially returned or lost items, calculate only the non-affected portion
                int rowQuantity = Convert.ToInt32(row.Cells[Column.TotalItems.ToString()].Value);
                decimal rowTax = Convert.ToDecimal(row.Cells[Column.Tax.ToString()].Value);
                decimal rowShipping = Convert.ToDecimal(row.Cells[Column.Shipping.ToString()].Value);
                decimal rowFee = Convert.ToDecimal(row.Cells[Column.Fee.ToString()].Value);
                decimal rowDiscount = Convert.ToDecimal(row.Cells[Column.Discount.ToString()].Value);
                decimal rowChargedDifference = Convert.ToDecimal(row.Cells[Column.ChargedDifference.ToString()].Value);
                decimal rowTotalPrice = Convert.ToDecimal(row.Cells[Column.Total.ToString()].Value);

                // Calculate the percentage of items that are still valid (not returned or lost)
                decimal validPercentage = CalculateValidItemsPercentage(row);

                totalQuantity += (int)(rowQuantity * validPercentage);
                totalTax += rowTax * validPercentage;
                totalShipping += rowShipping * validPercentage;
                fee += rowFee * validPercentage;
                discount += rowDiscount * validPercentage;
                chargedDifference += rowChargedDifference * validPercentage;
                totalPrice += rowTotalPrice * validPercentage;
            }

            LabelManager.ShowTotalsWithTransactions(Total_Label, SelectedDataGridView);
            Quantity_Label.Text = totalQuantity.ToString();
            Tax_Label.Text = $"{CurrencySymbol}{totalTax:N2}";
            Shipping_Label.Text = $"{CurrencySymbol}{totalShipping:N2}";
            Fee_Label.Text = $"{CurrencySymbol}{fee:N2}";
            Discount_Label.Text = $"{CurrencySymbol}{discount:N2}";
            ChargedDifference_Label.Text = $"{CurrencySymbol}{chargedDifference:N2}";
            Price_Label.Text = $"{CurrencySymbol}{totalPrice:N2}";
        }
        private static decimal CalculateValidItemsPercentage(DataGridViewRow row)
        {
            if (row.Tag is not (List<string> items, TagData tagData))
            {
                // Single item transaction
                bool isPartiallyReturned = ReturnManager.IsTransactionPartiallyReturned(row);
                bool isPartiallyLost = LostManager.IsTransactionPartiallyLost(row);

                // For single items, partial return/loss doesn't make sense, so return full value
                return (isPartiallyReturned || isPartiallyLost) ? 0m : 1m;
            }

            // Multi-item transaction
            int totalItems = items.Count;
            if (items.Count > 0 && items[^1].StartsWith(ReadOnlyVariables.Receipt_text))
            {
                totalItems--;  // Don't count receipt
            }

            if (totalItems == 0) { return 0m; }

            int affectedItems = 0;

            // Count returned items
            if (tagData.ReturnedItems != null)
            {
                affectedItems += tagData.ReturnedItems.Count;
            }

            // Count lost items
            if (tagData.LostItems != null)
            {
                affectedItems += tagData.LostItems.Count;
            }

            // Calculate percentage of valid (non-affected) items
            int validItems = Math.Max(0, totalItems - affectedItems);
            return totalItems > 0 ? (decimal)validItems / totalItems : 0m;
        }

        // Save to file
        public static void SaveDataGridViewToFileAsJson(Guna2DataGridView dataGridView, SelectedOption selected)
        {
            string filePath = DataGridViewManager.GetFilePathForDataGridView(selected);
            List<Dictionary<string, object>> rowsData = [];

            // Collect data from the DataGridView
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                Dictionary<string, object> rowData = [];

                // Iterate through all cells except the last one ("Has receipt" column)
                for (int i = 0; i < row.Cells.Count - 1; i++)
                {
                    DataGridViewCell cell = row.Cells[i];
                    if (cell.Value?.ToString() == ReadOnlyVariables.Show_text && cell.Tag != null)
                    {
                        rowData[_noteTextKey] = cell.Tag;
                    }
                    else
                    {
                        rowData[cell.OwningColumn.Name] = cell.Value;
                    }
                }

                // Add the row tag
                if (row.Tag is (List<string> tagList, TagData purchaseData))
                {
                    rowData[_rowTagKey] = new
                    {
                        Items = tagList,
                        PurchaseData = purchaseData
                    };
                }
                else if (row.Tag is (string tagString, TagData purchaseData1))
                {
                    rowData[_rowTagKey] = new
                    {
                        Tag = tagString,
                        PurchaseData = purchaseData1
                    };
                }
                else if (row.Tag is TagData purchaseData2)
                {
                    rowData[_rowTagKey] = new
                    {
                        PurchaseData = purchaseData2
                    };
                }

                rowsData.Add(rowData);
            }

            string json = JsonConvert.SerializeObject(rowsData, Formatting.Indented);
            Directories.WriteTextToFile(filePath, json);
        }
        public void SaveCategoriesToFile(SelectedOption option)
        {
            if (IsProgramLoading) { return; }

            string filePath = DataGridViewManager.GetFilePathForDataGridView(option);

            List<Category> categoryList;
            if (option == SelectedOption.CategoryPurchases || option == SelectedOption.ProductPurchases)
            {
                categoryList = CategoryPurchaseList;
            }
            else if (option == SelectedOption.CategoryRentals)
            {
                categoryList = CategoryRentalList;
            }
            else
            {
                categoryList = CategorySaleList;
            }

            string json = JsonConvert.SerializeObject(categoryList, Formatting.Indented);
            Directories.WriteTextToFile(filePath, json);
        }
        public void SaveCustomersToFile()
        {
            if (IsProgramLoading) { return; }

            string json = JsonConvert.SerializeObject(CustomerList, Formatting.Indented);
            Directories.WriteTextToFile(Directories.Customers_file, json);
        }
        public static void SaveDataGridViewToFile(Guna2DataGridView dataGridView, SelectedOption selected)
        {
            string filePath = DataGridViewManager.GetFilePathForDataGridView(selected);
            List<string> linesInDataGridView = [];

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    string cellValue = row.Cells[0].Value.ToString();
                    linesInDataGridView.Add(cellValue);
                }
            }

            Directories.WriteLinesToFile(filePath, linesInDataGridView);
        }
        public static void SaveListToFile(List<string> list, SelectedOption selected)
        {
            string filePath = DataGridViewManager.GetFilePathForDataGridView(selected);
            Directories.WriteLinesToFile(filePath, list);
        }

        // Analytic chart properties
        private List<Control> _analyticsControls;
        private Label _includeFreeShipping_Label;
        private Guna2Panel _analyticsTabButtons_Panel;
        private List<Guna2Button> _tabButtons;
        private AnalyticsTab _selectedTabKey = AnalyticsTab.Overview;
        private readonly Dictionary<AnalyticsTab, bool> _tabChartsLoaded = [];
        private readonly Dictionary<AnalyticsTab, List<Control>> _tabControls = [];

        public enum AnalyticsTab
        {
            Overview,
            Geographic,
            Financial,
            Performance,
            Operational,
            Returns,
            LostProducts,
            Customers
        }
        public enum ChartDataType
        {
            TotalRevenue,
            TotalExpenses,
            RevenueDistribution,
            ExpensesDistribution,
            TotalProfits,
            CountriesOfOrigin,
            CompaniesOfOrigin,
            CountriesOfDestination,
            AccountantsTransactions,
            SalesVsExpenses,
            AverageTransactionValue,
            TotalTransactions,
            AverageShippingCosts,
            GrowthRates,
            ReturnsOverTime,
            ReturnReasons,
            ReturnFinancialImpact,
            ReturnsByCategory,
            ReturnsByProduct,
            PurchaseVsSaleReturns,
            WorldMap,
            LossesOverTime,
            LossReasons,
            LossFinancialImpact,
            LossesByCategory,
            LossesByProduct,
            PurchaseVsSaleLosses
        }
        public enum GeoMapDataType
        {
            Combined,
            PurchasesOnly,
            SalesOnly
        }
        public PieChart CountriesOfOrigin_Chart { get; private set; }
        public PieChart CountriesOfDestination_Chart { get; private set; }
        public PieChart CompaniesOfOrigin_Chart { get; private set; }
        public PieChart Accountants_Chart { get; private set; }
        public CartesianChart GrowthRates_Chart { get; private set; }
        public CartesianChart TotalExpensesVsSales_Chart { get; private set; }
        public CartesianChart AverageTransactionValue_Chart { get; private set; }
        public CartesianChart TotalTransactions_Chart { get; private set; }
        public CartesianChart AverageShippingCosts_Chart { get; private set; }
        public Guna2CustomCheckBox IncludeFreeShipping_CheckBox { get; private set; }
        public CartesianChart ReturnsOverTime_Chart { get; private set; }
        public PieChart ReturnReasons_Chart { get; private set; }
        public CartesianChart ReturnFinancialImpact_Chart { get; private set; }
        public PieChart ReturnsByCategory_Chart { get; private set; }
        public PieChart ReturnsByProduct_Chart { get; private set; }
        public PieChart PurchaseVsSaleReturns_Chart { get; private set; }
        public CartesianChart TotalPurchases_Chart { get; private set; }
        public PieChart DistributionOfPurchases_Chart { get; private set; }
        public CartesianChart TotalSales_Chart { get; private set; }
        public PieChart DistributionOfSales_Chart { get; private set; }
        public CartesianChart LossesOverTime_Chart { get; private set; }
        public PieChart LossReasons_Chart { get; private set; }
        public CartesianChart LossFinancialImpact_Chart { get; private set; }
        public PieChart LossesByCategory_Chart { get; private set; }
        public PieChart LossesByProduct_Chart { get; private set; }
        public PieChart PurchaseVsSaleLosses_Chart { get; private set; }
        public CartesianChart TopCustomersByRevenue_Chart { get; private set; }
        public PieChart CustomerPaymentStatus_Chart { get; private set; }
        public CartesianChart CustomerGrowth_Chart { get; private set; }
        public PieChart ActiveVsInactiveCustomers_Chart { get; private set; }
        public CartesianChart CustomerLifetimeValue_Chart { get; private set; }
        public CartesianChart AverageRentalsPerCustomer_Chart { get; private set; }
        public GeoMap WorldMap_GeoMap { get; private set; }

        // GeoMap properties
        private Guna2Panel _worldMapControls_Panel;
        private Guna2CustomRadioButton _combinedData_RadioButton;
        private Guna2CustomRadioButton _purchasesOnly_RadioButton;
        private Guna2CustomRadioButton _salesOnly_RadioButton;
        private Label _worldMapDataType_Label;
        private Label _combinedData_Label;
        private Label _purchasesOnly_Label;
        private Label _salesOnly_Label;

        // Analytic chart methods
        public List<Control> GetMainControls()
        {
            return [
                Sale_DataGridView,
                Purchase_DataGridView,
                TotalPurchases_Chart,
                DistributionOfPurchases_Chart,
                TotalSales_Chart,
                DistributionOfSales_Chart,
                TotalRentals_Chart,
                DistributionOfRentals_Chart,
                Rental_DataGridView,
                Profits_Chart,
                Total_Panel
            ];
        }
        public List<Control> GetAnalyticsControls() => _analyticsControls;
        public IEnumerable<Control> GetAllCharts()
        {
            return [TotalPurchases_Chart,
                DistributionOfPurchases_Chart,
                TotalSales_Chart,
                DistributionOfSales_Chart,
                Profits_Chart,
                CountriesOfOrigin_Chart,
                CompaniesOfOrigin_Chart,
                CountriesOfDestination_Chart,
                WorldMap_GeoMap,
                Accountants_Chart,
                TotalExpensesVsSales_Chart,
                AverageTransactionValue_Chart,
                TotalTransactions_Chart,
                AverageShippingCosts_Chart,
                GrowthRates_Chart,
                ReturnsOverTime_Chart,
                ReturnReasons_Chart,
                ReturnFinancialImpact_Chart,
                ReturnsByCategory_Chart,
                ReturnsByProduct_Chart,
                PurchaseVsSaleReturns_Chart];
        }
        private void ShowMainControls()
        {
            if (_analyticsControls == null) { return; }

            foreach (Control control in GetMainControls())
            {
                control.Visible = true;
            }
            foreach (Control control in _analyticsControls)
            {
                control.Visible = false;
            }

            // Reset chart positions in case returning from analytics
            TotalPurchases_Chart.Top = _chartTop;
            DistributionOfPurchases_Chart.Top = _chartTop;
            TotalSales_Chart.Top = _chartTop;
            DistributionOfSales_Chart.Top = _chartTop;
            TotalRentals_Chart.Top = _chartTop;
            DistributionOfRentals_Chart.Top = _chartTop;
            Profits_Chart.Top = _chartTop;

            // Set which charts should actually be visible based on current view
            if (Selected == SelectedOption.Purchases)
            {
                TotalPurchases_Chart.Visible = true;
                DistributionOfPurchases_Chart.Visible = true;
                TotalSales_Chart.Visible = false;
                DistributionOfSales_Chart.Visible = false;
                TotalRentals_Chart.Visible = false;
                DistributionOfRentals_Chart.Visible = false;
            }
            else if (Selected == SelectedOption.Sales)
            {
                TotalPurchases_Chart.Visible = false;
                DistributionOfPurchases_Chart.Visible = false;
                TotalSales_Chart.Visible = true;
                DistributionOfSales_Chart.Visible = true;
                TotalRentals_Chart.Visible = false;
                DistributionOfRentals_Chart.Visible = false;
            }
            else if (Selected == SelectedOption.Rentals)
            {
                TotalPurchases_Chart.Visible = false;
                DistributionOfPurchases_Chart.Visible = false;
                TotalSales_Chart.Visible = false;
                DistributionOfSales_Chart.Visible = false;
                TotalRentals_Chart.Visible = true;
                DistributionOfRentals_Chart.Visible = true;
            }
        }
        private void ConstructControlsForAnalytics()
        {
            CountriesOfOrigin_Chart = ConstructAnalyticsChart("countriesOfOrigin_Chart", false) as PieChart;
            CompaniesOfOrigin_Chart = ConstructAnalyticsChart("companiesOfOrigin_Chart", false) as PieChart;
            CountriesOfDestination_Chart = ConstructAnalyticsChart("countriesOfDestination_Chart", false) as PieChart;
            Accountants_Chart = ConstructAnalyticsChart("accountants_Chart", false) as PieChart;
            TotalExpensesVsSales_Chart = ConstructAnalyticsChart("salesVsExpenses_Chart", true) as CartesianChart;
            AverageTransactionValue_Chart = ConstructAnalyticsChart("averageOrderValue_Chart", true) as CartesianChart;
            TotalTransactions_Chart = ConstructAnalyticsChart("totalTransactions_Chart", true) as CartesianChart;
            AverageShippingCosts_Chart = ConstructAnalyticsChart("averageShippingCosts_Chart", true) as CartesianChart;
            GrowthRates_Chart = ConstructAnalyticsChart("growthRates_Chart", true) as CartesianChart;
            ReturnsOverTime_Chart = ConstructAnalyticsChart("returnsOverTime_Chart", true) as CartesianChart;
            ReturnReasons_Chart = ConstructAnalyticsChart("returnReasons_Chart", false) as PieChart;
            ReturnFinancialImpact_Chart = ConstructAnalyticsChart("returnFinancialImpact_Chart", true) as CartesianChart;
            ReturnsByCategory_Chart = ConstructAnalyticsChart("returnsByCategory_Chart", false) as PieChart;
            ReturnsByProduct_Chart = ConstructAnalyticsChart("returnsByProduct_Chart", false) as PieChart;
            PurchaseVsSaleReturns_Chart = ConstructAnalyticsChart("purchaseVsSaleReturns_Chart", false) as PieChart;
            LossesOverTime_Chart = ConstructAnalyticsChart("lossesOverTime_Chart", true) as CartesianChart;
            LossReasons_Chart = ConstructAnalyticsChart("lossReasons_Chart", false) as PieChart;
            LossFinancialImpact_Chart = ConstructAnalyticsChart("lossFinancialImpact_Chart", true) as CartesianChart;
            LossesByCategory_Chart = ConstructAnalyticsChart("lossesByCategory_Chart", false) as PieChart;
            LossesByProduct_Chart = ConstructAnalyticsChart("lossesByProduct_Chart", false) as PieChart;
            PurchaseVsSaleLosses_Chart = ConstructAnalyticsChart("purchaseVsSaleLosses_Chart", false) as PieChart;
            TopCustomersByRevenue_Chart = ConstructAnalyticsChart("topCustomersByRevenue_Chart", true) as CartesianChart;
            CustomerPaymentStatus_Chart = ConstructAnalyticsChart("customerPaymentStatus_Chart", false) as PieChart;
            CustomerGrowth_Chart = ConstructAnalyticsChart("customerGrowth_Chart", true) as CartesianChart;
            ActiveVsInactiveCustomers_Chart = ConstructAnalyticsChart("activeVsInactiveCustomers_Chart", false) as PieChart;
            CustomerLifetimeValue_Chart = ConstructAnalyticsChart("customerLifetimeValue_Chart", true) as CartesianChart;
            AverageRentalsPerCustomer_Chart = ConstructAnalyticsChart("averageRentalsPerCustomer_Chart", true) as CartesianChart;

            WorldMap_GeoMap = ConstructGeoMap();
            ConstructWorldMapDataControls();

            IncludeFreeShipping_CheckBox = new Guna2CustomCheckBox
            {
                Size = new Size(20, 20),
                Checked = Properties.Settings.Default.IncludeFreeShipping,
                Animated = true
            };
            IncludeFreeShipping_CheckBox.CheckedChanged += IncludeFreeShippingCheckBox_CheckedChanged;
            Controls.Add(IncludeFreeShipping_CheckBox);

            _includeFreeShipping_Label = new Label
            {
                Text = "Include transactions with free shipping",
                Name = "IncludeFreeShipping_Label",  // This is needed for the language translation
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                Padding = new Padding(5),
                AccessibleDescription = AccessibleDescriptionManager.AlignLeft
            };
            _includeFreeShipping_Label.Click += IncludeFreeShipping_Label_Click;
            Controls.Add(_includeFreeShipping_Label);

            CreateAnalyticsTabControl();
            OrganizeChartsIntoTabs();

            Control[] chartsAndMaps = _analyticsControls
                .Where(c => c is CartesianChart || c is PieChart || c is GeoMap)
                .ToArray();

            MouseClickChartManager.InitCharts(chartsAndMaps);
        }
        private void CreateAnalyticsTabControl()
        {
            // Initialize the dictionaries with enum values
            foreach (AnalyticsTab tab in Enum.GetValues<AnalyticsTab>())
            {
                _tabChartsLoaded[tab] = false;
                _tabControls[tab] = [];
            }

            // Create a panel to hold all the tab buttons
            Guna2Panel tabButtonsPanel = new()
            {
                Height = 60,
                FillColor = CustomColors.ContentPanelBackground,
                BorderColor = CustomColors.ControlBorder,
                BorderThickness = 1,
                BorderRadius = 8,
                Anchor = AnchorStyles.Top
            };

            // Create tab buttons
            List<Guna2Button> tabButtons = [];

            Guna2Button overviewButton = CreateTabButton("Overview", AnalyticsTab.Overview, Resources.Overview);
            tabButtons.Add(overviewButton);

            Guna2Button geographicButton = CreateTabButton("Geographic", AnalyticsTab.Geographic, Resources.Earth);
            tabButtons.Add(geographicButton);

            Guna2Button financialButton = CreateTabButton("Financial", AnalyticsTab.Financial, Resources.Financial);
            tabButtons.Add(financialButton);

            Guna2Button performanceButton = CreateTabButton("Performance", AnalyticsTab.Performance, Resources.Performance);
            tabButtons.Add(performanceButton);

            Guna2Button operationalButton = CreateTabButton("Operational", AnalyticsTab.Operational, Resources.Gear);
            tabButtons.Add(operationalButton);

            Guna2Button returnsButton = CreateTabButton("Returns", AnalyticsTab.Returns, Resources.Return);
            tabButtons.Add(returnsButton);

            Guna2Button lostProductsButton = CreateTabButton("Lost Products", AnalyticsTab.LostProducts, Resources.Loss);
            tabButtons.Add(lostProductsButton);

            Guna2Button customersButton = CreateTabButton("Customers", AnalyticsTab.Customers, Resources.User);
            tabButtons.Add(customersButton);

            _tabButtons = tabButtons;

            // Add buttons to panel (positioning will be done in PositionTabButtons)
            foreach (Guna2Button button in tabButtons)
            {
                tabButtonsPanel.Controls.Add(button);
            }

            _analyticsTabButtons_Panel = tabButtonsPanel;

            ThemeManager.SetThemeForControls([tabButtonsPanel]);

            // Select the first tab by default
            SelectTabButton(AnalyticsTab.Overview);

            Controls.Add(tabButtonsPanel);
        }
        private Guna2Button CreateTabButton(string title, AnalyticsTab tabKey, Image icon)
        {
            Guna2Button button = new()
            {
                Name = $"{tabKey}Tab_Button",
                Text = title,
                Tag = tabKey,
                Image = icon,
                ImageOffset = new Point(-5, 0),
                ImageSize = new Size(25, 25),
                BorderRadius = 6,
            };

            button.Click += TabButton_Click;
            return button;
        }
        private void TabButton_Click(object sender, EventArgs e)
        {
            if (sender is Guna2Button clickedButton && clickedButton.Tag is AnalyticsTab tabKey)
            {
                SelectTabButton(tabKey);

                if (!_tabChartsLoaded[tabKey])
                {
                    LoadChartsForTab(tabKey);
                    _tabChartsLoaded[tabKey] = true;
                }

                ShowControlsForTab(tabKey);
                CenterAndResizeControls();
            }
        }
        private void SelectTabButton(AnalyticsTab selectedTabKey)
        {
            _selectedTabKey = selectedTabKey;

            foreach (Guna2Button button in _tabButtons)
            {
                if (button.Tag is AnalyticsTab buttonTab && buttonTab == selectedTabKey)
                {
                    // Selected state
                    button.FillColor = CustomColors.AccentBlue;
                    button.ForeColor = Color.White;
                }
                else
                {
                    // Unselected state
                    button.FillColor = Color.Transparent;
                    button.ForeColor = CustomColors.Text;
                }
            }
        }
        private void OrganizeChartsIntoTabs()
        {
            _tabControls[AnalyticsTab.Overview].AddRange(
            [
                TotalExpensesVsSales_Chart,
                Profits_Chart,
                TotalTransactions_Chart,
                AverageTransactionValue_Chart
            ]);

            _tabControls[AnalyticsTab.Geographic].AddRange(
            [
                WorldMap_GeoMap,
                CountriesOfOrigin_Chart,
                CountriesOfDestination_Chart,
                CompaniesOfOrigin_Chart,
                _worldMapControls_Panel
            ]);

            _tabControls[AnalyticsTab.Financial].AddRange(
            [
                TotalSales_Chart,
                TotalPurchases_Chart,
                DistributionOfSales_Chart,
                DistributionOfPurchases_Chart
            ]);

            _tabControls[AnalyticsTab.Performance].AddRange(
            [
                GrowthRates_Chart,
                AverageTransactionValue_Chart,
                TotalTransactions_Chart
            ]);

            _tabControls[AnalyticsTab.Operational].AddRange(
            [
                Accountants_Chart,
                AverageShippingCosts_Chart,
                IncludeFreeShipping_CheckBox,
                _includeFreeShipping_Label
            ]);

            _tabControls[AnalyticsTab.Returns].AddRange(
            [
                ReturnsOverTime_Chart,
                ReturnReasons_Chart,
                ReturnFinancialImpact_Chart,
                ReturnsByCategory_Chart,
                ReturnsByProduct_Chart,
                PurchaseVsSaleReturns_Chart
            ]);

            _tabControls[AnalyticsTab.LostProducts].AddRange(
            [
                LossesOverTime_Chart,
                LossReasons_Chart,
                LossFinancialImpact_Chart,
                LossesByCategory_Chart,
                LossesByProduct_Chart,
                PurchaseVsSaleLosses_Chart
            ]);

            _tabControls[AnalyticsTab.Customers].AddRange(
            [
                TopCustomersByRevenue_Chart,
                CustomerPaymentStatus_Chart,
                CustomerGrowth_Chart,
                ActiveVsInactiveCustomers_Chart,
                CustomerLifetimeValue_Chart,
                AverageRentalsPerCustomer_Chart
            ]);

            _analyticsControls =
            [
                CountriesOfOrigin_Chart,
                CompaniesOfOrigin_Chart,
                CountriesOfDestination_Chart,
                WorldMap_GeoMap,
                _worldMapControls_Panel,
                Accountants_Chart,
                TotalExpensesVsSales_Chart,
                AverageTransactionValue_Chart,
                TotalTransactions_Chart,
                AverageShippingCosts_Chart,
                GrowthRates_Chart,
                ReturnsOverTime_Chart,
                ReturnReasons_Chart,
                ReturnFinancialImpact_Chart,
                ReturnsByCategory_Chart,
                ReturnsByProduct_Chart,
                PurchaseVsSaleReturns_Chart,
                IncludeFreeShipping_CheckBox,
                _includeFreeShipping_Label,
                _analyticsTabButtons_Panel,
                LossesOverTime_Chart,
                LossReasons_Chart,
                LossFinancialImpact_Chart,
                LossesByCategory_Chart,
                LossesByProduct_Chart,
                PurchaseVsSaleLosses_Chart,
                TopCustomersByRevenue_Chart,
                CustomerPaymentStatus_Chart,
                CustomerGrowth_Chart,
                ActiveVsInactiveCustomers_Chart,
                CustomerLifetimeValue_Chart,
                AverageRentalsPerCustomer_Chart
            ];
        }
        private void LoadChartsForTab(AnalyticsTab tabKey)
        {
            bool isLine = LineChart_ToggleSwitch.Checked;

            switch (tabKey)
            {
                case AnalyticsTab.Overview:
                    LoadChart.LoadSalesVsExpensesChart(TotalExpensesVsSales_Chart, isLine);
                    SetChartTitle(TotalExpensesVsSales_Chart, TranslatedChartTitles.SalesVsExpenses);

                    LoadChart.LoadTotalTransactionsChart(TotalTransactions_Chart, isLine);
                    SetChartTitle(TotalTransactions_Chart, TranslatedChartTitles.TotalTransactions);

                    LoadChart.LoadAverageTransactionValueChart(AverageTransactionValue_Chart, isLine);
                    SetChartTitle(AverageTransactionValue_Chart, TranslatedChartTitles.AverageTransactionValue);

                    ChartData profitsData = LoadChart.LoadProfitsIntoChart(Profits_Chart, isLine);
                    SetProfitsChartTitle(profitsData.Total);
                    break;

                case AnalyticsTab.Geographic:
                    GeoMapDataType dataType = GetSelectedGeoMapDataType();
                    LoadChart.LoadWorldMapChart(WorldMap_GeoMap, dataType);

                    LoadChart.LoadCountriesOfOriginChart(CountriesOfOrigin_Chart, PieChartGrouping.Top8);
                    SetChartTitle(CountriesOfOrigin_Chart, TranslatedChartTitles.CountriesOfOrigin);

                    LoadChart.LoadCountriesOfDestinationChart(CountriesOfDestination_Chart, PieChartGrouping.Top8);
                    SetChartTitle(CountriesOfDestination_Chart, TranslatedChartTitles.CountriesOfDestination);

                    LoadChart.LoadCompaniesOfOriginChart(CompaniesOfOrigin_Chart, PieChartGrouping.Top8);
                    SetChartTitle(CompaniesOfOrigin_Chart, TranslatedChartTitles.CompaniesOfOrigin);
                    break;

                case AnalyticsTab.Financial:
                    LoadOrRefreshMainCharts();
                    break;

                case AnalyticsTab.Performance:
                    LoadChart.LoadGrowthRateChart(GrowthRates_Chart);
                    SetChartTitle(GrowthRates_Chart, TranslatedChartTitles.GrowthRates);

                    // Load shared charts if not already loaded
                    if (!_tabChartsLoaded[AnalyticsTab.Overview])
                    {
                        LoadChart.LoadAverageTransactionValueChart(AverageTransactionValue_Chart, isLine);
                        SetChartTitle(AverageTransactionValue_Chart, TranslatedChartTitles.AverageTransactionValue);

                        LoadChart.LoadTotalTransactionsChart(TotalTransactions_Chart, isLine);
                        SetChartTitle(TotalTransactions_Chart, TranslatedChartTitles.TotalTransactions);
                    }
                    break;

                case AnalyticsTab.Operational:
                    LoadChart.LoadAccountantsIntoChart(Accountants_Chart, PieChartGrouping.Top8);
                    SetChartTitle(Accountants_Chart, TranslatedChartTitles.AccountantsTransactions);

                    LoadChart.LoadAverageShippingCostsChart(AverageShippingCosts_Chart, isLine,
                        includeZeroShipping: IncludeFreeShipping_CheckBox.Checked);
                    SetChartTitle(AverageShippingCosts_Chart, TranslatedChartTitles.AverageShippingCosts);
                    break;

                case AnalyticsTab.Returns:
                    LoadChart.LoadReturnsOverTimeChart(ReturnsOverTime_Chart, isLine);
                    SetChartTitle(ReturnsOverTime_Chart, TranslatedChartTitles.ReturnsOverTime);

                    LoadChart.LoadReturnReasonsChart(ReturnReasons_Chart, PieChartGrouping.Top8);
                    SetChartTitle(ReturnReasons_Chart, TranslatedChartTitles.ReturnReasons);

                    LoadChart.LoadReturnFinancialImpactChart(ReturnFinancialImpact_Chart, isLine);
                    SetChartTitle(ReturnFinancialImpact_Chart, TranslatedChartTitles.ReturnFinancialImpact);

                    LoadChart.LoadReturnsByCategoryChart(ReturnsByCategory_Chart, PieChartGrouping.Top8);
                    SetChartTitle(ReturnsByCategory_Chart, TranslatedChartTitles.ReturnsByCategory);

                    LoadChart.LoadReturnsByProductChart(ReturnsByProduct_Chart, PieChartGrouping.Top8);
                    SetChartTitle(ReturnsByProduct_Chart, TranslatedChartTitles.ReturnsByProduct);

                    LoadChart.LoadPurchaseVsSaleReturnsChart(PurchaseVsSaleReturns_Chart);
                    SetChartTitle(PurchaseVsSaleReturns_Chart, TranslatedChartTitles.PurchaseVsSaleReturns);
                    break;

                case AnalyticsTab.LostProducts:
                    LoadChart.LoadLossesOverTimeChart(LossesOverTime_Chart, isLine);
                    SetChartTitle(LossesOverTime_Chart, TranslatedChartTitles.LossesOverTime);

                    LoadChart.LoadLossReasonsChart(LossReasons_Chart, PieChartGrouping.Top8);
                    SetChartTitle(LossReasons_Chart, TranslatedChartTitles.LossReasons);

                    LoadChart.LoadLossFinancialImpactChart(LossFinancialImpact_Chart, isLine);
                    SetChartTitle(LossFinancialImpact_Chart, TranslatedChartTitles.LossFinancialImpact);

                    LoadChart.LoadLossesByCategoryChart(LossesByCategory_Chart, PieChartGrouping.Top8);
                    SetChartTitle(LossesByCategory_Chart, TranslatedChartTitles.LossesByCategory);

                    LoadChart.LoadLossesByProductChart(LossesByProduct_Chart, PieChartGrouping.Top8);
                    SetChartTitle(LossesByProduct_Chart, TranslatedChartTitles.LossesByProduct);

                    LoadChart.LoadPurchaseVsSaleLossesChart(PurchaseVsSaleLosses_Chart);
                    SetChartTitle(PurchaseVsSaleLosses_Chart, TranslatedChartTitles.PurchaseVsSaleLosses);
                    break;

                case AnalyticsTab.Customers:
                    LoadChart.LoadTopCustomersByRevenueChart(TopCustomersByRevenue_Chart);
                    SetChartTitle(TopCustomersByRevenue_Chart, "Top Customers by Revenue");

                    LoadChart.LoadCustomerPaymentStatusChart(CustomerPaymentStatus_Chart);
                    SetChartTitle(CustomerPaymentStatus_Chart, "Customer Payment Status");

                    LoadChart.LoadCustomerGrowthChart(CustomerGrowth_Chart, isLine);
                    SetChartTitle(CustomerGrowth_Chart, "Customer Growth Over Time");

                    LoadChart.LoadActiveVsInactiveCustomersChart(ActiveVsInactiveCustomers_Chart);
                    SetChartTitle(ActiveVsInactiveCustomers_Chart, "Active vs Inactive Customers");

                    LoadChart.LoadCustomerLifetimeValueChart(CustomerLifetimeValue_Chart);
                    SetChartTitle(CustomerLifetimeValue_Chart, "Customer Lifetime Value");

                    LoadChart.LoadAverageRentalsPerCustomerChart(AverageRentalsPerCustomer_Chart, isLine);
                    SetChartTitle(AverageRentalsPerCustomer_Chart, "Average Rentals per Customer");
                    break;
            }
        }
        private void ShowControlsForTab(AnalyticsTab tabKey)
        {
            // Hide all analytics charts first
            foreach (List<Control> controlList in _tabControls.Values)
            {
                foreach (Control control in controlList)
                {
                    control.Visible = false;
                }
            }

            // Show controls for the selected tab
            foreach (Control control in _tabControls[tabKey])
            {
                control.Visible = true;
            }
        }
        private void IncludeFreeShipping_Label_Click(object? sender, EventArgs e)
        {
            IncludeFreeShipping_CheckBox.Checked = !IncludeFreeShipping_CheckBox.Checked;
        }
        private void IncludeFreeShippingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool isLineChart = LineChart_ToggleSwitch.Checked;
            bool zeroShipping = IncludeFreeShipping_CheckBox.Checked;

            UserSettings.UpdateSetting("Include free shipping in chart", Properties.Settings.Default.IncludeFreeShipping, zeroShipping,
                value => Properties.Settings.Default.IncludeFreeShipping = value);

            LoadChart.LoadAverageShippingCostsChart(AverageShippingCosts_Chart, isLineChart, includeZeroShipping: zeroShipping);
        }
        private Control ConstructAnalyticsChart(string name, bool isCartesian)
        {
            Control chart;

            if (isCartesian)
            {
                CartesianChart cartesianChart = new()
                {
                    Name = name
                };

                chart = cartesianChart;
            }
            else
            {
                PieChart pieChart = new()
                {
                    Name = name
                };

                LoadChart.ConfigurePieChart(pieChart);
                chart = pieChart;
            }

            Controls.Add(chart);
            return chart;
        }
        private void ShowAnalyticsControls()
        {
            // Hide main controls
            foreach (Control control in GetMainControls())
            {
                control.Visible = false;
            }

            _analyticsTabButtons_Panel.Visible = true;

            // Load overview tab by default if not loaded
            if (!_tabChartsLoaded[AnalyticsTab.Overview])
            {
                LoadChartsForTab(AnalyticsTab.Overview);
                _tabChartsLoaded[AnalyticsTab.Overview] = true;
            }

            ShowControlsForTab(_selectedTabKey);
        }
        public void LoadOrRefreshAnalyticsCharts(bool onlyRefreshForLineCharts = false)
        {
            if (Selected != SelectedOption.Analytics) { return; }

            AnalyticsTab currentTabKey = _selectedTabKey;
            bool isLine = LineChart_ToggleSwitch.Checked;

            if (onlyRefreshForLineCharts)
            {
                // Only reload line/bar charts when toggle switches
                switch (currentTabKey)
                {
                    case AnalyticsTab.Overview:
                    case AnalyticsTab.Performance:
                        LoadChart.LoadSalesVsExpensesChart(TotalExpensesVsSales_Chart, isLine);
                        LoadChart.LoadTotalTransactionsChart(TotalTransactions_Chart, isLine);
                        LoadChart.LoadAverageTransactionValueChart(AverageTransactionValue_Chart, isLine);

                        ChartData profitsData = LoadChart.LoadProfitsIntoChart(Profits_Chart, isLine);
                        SetProfitsChartTitle(profitsData.Total);
                        break;

                    case AnalyticsTab.Financial:
                        LoadOrRefreshMainCharts(true);
                        break;

                    case AnalyticsTab.Operational:
                        LoadChart.LoadAverageShippingCostsChart(AverageShippingCosts_Chart, isLine,
                            includeZeroShipping: IncludeFreeShipping_CheckBox.Checked);
                        break;

                    case AnalyticsTab.Returns:
                        LoadChart.LoadReturnsOverTimeChart(ReturnsOverTime_Chart, isLine);
                        LoadChart.LoadReturnFinancialImpactChart(ReturnFinancialImpact_Chart, isLine);
                        break;

                    case AnalyticsTab.LostProducts:
                        LoadChart.LoadLossesOverTimeChart(LossesOverTime_Chart, isLine);
                        LoadChart.LoadLossFinancialImpactChart(LossFinancialImpact_Chart, isLine);
                        break;

                    case AnalyticsTab.Customers:
                        LoadChart.LoadCustomerGrowthChart(CustomerGrowth_Chart, isLine);
                        LoadChart.LoadAverageRentalsPerCustomerChart(AverageRentalsPerCustomer_Chart, isLine);
                        break;
                }
            }
            else
            {
                // Full reload
                LoadChartsForTab(currentTabKey);
            }
        }
        public void SetAllAnalyticTabsAsNotLoaded()
        {
            foreach (AnalyticsTab tab in Enum.GetValues<AnalyticsTab>())
            {
                _tabChartsLoaded[tab] = false;
            }
        }
        public void ReloadCurrentAnalyticTab()
        {
            if (Selected == SelectedOption.Analytics)
            {
                LoadChartsForTab(_selectedTabKey);
                _tabChartsLoaded[_selectedTabKey] = true;
            }
        }

        // GeoMap methods
        private GeoMap ConstructGeoMap()
        {
            GeoMap geoMap = new()
            {
                Visible = false
            };

            Controls.Add(geoMap);
            return geoMap;
        }
        private void ConstructWorldMapDataControls()
        {
            const int radioButtonSize = 20;

            // Create main label
            _worldMapDataType_Label = new Label
            {
                Text = "Map Data Source:",
                Name = "WorldMapDataType_Label",
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = CustomColors.Text,
                AccessibleDescription = AccessibleDescriptionManager.AlignLeft
            };
            LanguageManager.UpdateLanguageForControl(_worldMapDataType_Label);

            // Combined data option
            _combinedData_RadioButton = new Guna2CustomRadioButton
            {
                Size = new Size(radioButtonSize, radioButtonSize),
                Animated = true
            };

            _combinedData_Label = new Label
            {
                Text = "Purchases + sales",
                Name = "CombinedData_Label",
                AutoSize = true,
                Padding = new Padding(5),
                Font = new Font("Segoe UI", 11),
                ForeColor = CustomColors.Text,
                AccessibleDescription = AccessibleDescriptionManager.AlignLeft
            };
            _combinedData_Label.Click += (s, e) =>
            {
                _combinedData_RadioButton.Checked = true;
            };
            LanguageManager.UpdateLanguageForControl(_combinedData_Label);

            // Purchases only option
            _purchasesOnly_RadioButton = new Guna2CustomRadioButton
            {
                Size = new Size(radioButtonSize, radioButtonSize),
                Animated = true
            };

            _purchasesOnly_Label = new Label
            {
                Text = "Purchases",
                Name = "Purchases_Label",
                AutoSize = true,
                Padding = new Padding(5),
                Font = new Font("Segoe UI", 11),
                ForeColor = CustomColors.Text,
                AccessibleDescription = AccessibleDescriptionManager.AlignLeft
            };
            _purchasesOnly_Label.Click += (s, e) =>
            {
                _purchasesOnly_RadioButton.Checked = true;
            };
            LanguageManager.UpdateLanguageForControl(_purchasesOnly_Label);

            // Sales only option
            _salesOnly_RadioButton = new Guna2CustomRadioButton
            {
                Size = new Size(radioButtonSize, radioButtonSize),
                Animated = true
            };

            _salesOnly_Label = new Label
            {
                Text = "Sales",
                Name = "Sales_Label",
                AutoSize = true,
                Padding = new Padding(5),
                Font = new Font("Segoe UI", 11),
                ForeColor = CustomColors.Text,
                AccessibleDescription = AccessibleDescriptionManager.AlignLeft
            };
            _salesOnly_Label.Click += (s, e) =>
            {
                _salesOnly_RadioButton.Checked = true;
            };
            LanguageManager.UpdateLanguageForControl(_salesOnly_Label);

            // Create panel (size will be calculated in RecalculateWorldMapControlsLayout)
            _worldMapControls_Panel = new Guna2Panel
            {
                FillColor = Color.Transparent,
                BorderThickness = 0,
                Visible = false
            };

            // Add all controls to panel
            _worldMapControls_Panel.Controls.AddRange([
                _worldMapDataType_Label,
                _combinedData_RadioButton,
                _combinedData_Label,
                _purchasesOnly_RadioButton,
                _purchasesOnly_Label,
                _salesOnly_RadioButton,
                _salesOnly_Label
            ]);

            Controls.Add(_worldMapControls_Panel);
            _combinedData_RadioButton.Checked = true;  // Check it after the control is added

            // Set up event handlers
            _salesOnly_RadioButton.CheckedChanged += WorldMapDataType_CheckedChanged;
            _combinedData_RadioButton.CheckedChanged += WorldMapDataType_CheckedChanged;
            _purchasesOnly_RadioButton.CheckedChanged += WorldMapDataType_CheckedChanged;

            RecalculateWorldMapControlsLayout();
        }
        public void RecalculateWorldMapControlsLayout()
        {
            if (_worldMapControls_Panel == null || _worldMapDataType_Label == null)
            {
                return;
            }

            // Constants
            const int radioButtonSize = 20;
            const int radioLabelSpacing = -2;
            const int optionSpacing = 30;
            const int panelHeight = 55;
            int currentX = 0;

            // Recalculate main label position
            int mainLabelY = (panelHeight - _worldMapDataType_Label.PreferredHeight) / 2;
            _worldMapDataType_Label.Location = new Point(currentX, mainLabelY);
            currentX += _worldMapDataType_Label.PreferredWidth + optionSpacing;

            int radioButtonY = (panelHeight - radioButtonSize) / 2;
            int labelY = (panelHeight - _combinedData_Label.PreferredHeight) / 2;

            // Reposition Combined data option
            _combinedData_RadioButton.Location = new Point(currentX, radioButtonY);
            _combinedData_Label.Location = new Point(currentX + radioButtonSize + radioLabelSpacing, labelY);
            currentX += radioButtonSize + radioLabelSpacing + _combinedData_Label.PreferredWidth + optionSpacing;

            // Reposition Purchases only option
            _purchasesOnly_RadioButton.Location = new Point(currentX, radioButtonY);
            _purchasesOnly_Label.Location = new Point(currentX + radioButtonSize + radioLabelSpacing, labelY);
            currentX += radioButtonSize + radioLabelSpacing + _purchasesOnly_Label.PreferredWidth + optionSpacing;

            // Reposition Sales only option
            _salesOnly_RadioButton.Location = new Point(currentX, radioButtonY);
            _salesOnly_Label.Location = new Point(currentX + radioButtonSize + radioLabelSpacing, labelY);

            // Recalculate total panel width
            int totalWidth = currentX + radioButtonSize + radioLabelSpacing + _salesOnly_Label.PreferredWidth;

            // Update panel size
            _worldMapControls_Panel.Size = new Size(totalWidth, panelHeight);

            // Position the panel itself
            if (_analyticsTabButtons_Panel != null)
            {
                _worldMapControls_Panel.Location = new Point((ClientSize.Width - _worldMapControls_Panel.Width) / 2,
                    _analyticsTabButtons_Panel.Bottom + 10);
            }
        }
        private void WorldMapDataType_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is Guna2CustomRadioButton radioButton && radioButton.Checked)
            {
                GeoMapDataType dataType = GetSelectedGeoMapDataType();
                LoadChart.LoadWorldMapChart(WorldMap_GeoMap, dataType);
            }
        }
        private GeoMapDataType GetSelectedGeoMapDataType()
        {
            if (_purchasesOnly_RadioButton.Checked)
            {
                return GeoMapDataType.PurchasesOnly;
            }
            else if (_salesOnly_RadioButton.Checked)
            {
                return GeoMapDataType.SalesOnly;
            }
            else
            {
                return GeoMapDataType.Combined;
            }
        }

        // Misc.
        public static List<Guna2Panel> GetMenus()
        {
            return new List<Guna2Panel>
            {
                CustomControls.FileMenu,
                CustomControls.RecentlyOpenedMenu,
                CustomControls.HelpMenu,
                CustomControls.ControlDropDown_Panel,
                CompanyLogo.CompanyLogoRightClick_Panel,
                GetStarted_Form.RightClickOpenRecent_Panel,
                RightClickDataGridViewRowMenu.Panel,
                RightClickChartMenu.Panel,
                TextBoxManager.RightClickTextBox_Panel,
                RightClickElementMenu.Panel,
                UndoRedoHistoryDropdown.Panel
            }.Where(panel => panel != null).ToList();
        }
        public void UpdateMainMenuFormText()
        {
            Text = $"Argo Sales Tracker {Tools.GetVersionNumber()} - {Directories.CompanyName}";
        }
        public static void CloseRightClickPanels()
        {
            TextBoxManager.HideRightClickPanel();
            RightClickDataGridViewRowMenu.Hide();
            RightClickChartMenu.Hide();
        }
        public void ClosePanels()
        {
            RenameCompany();
            CloseRightClickPanels();

            CompanyLogo.Hide();
            ColumnVisibilityPanel.Hide();
            CloseDateRangePanel();
            Controls.Remove(CustomControls.ControlDropDown_Panel);

            Controls.Remove(CustomControls.FileMenu);
            Controls.Remove(CustomControls.RecentlyOpenedMenu);
            Controls.Remove(CustomControls.HelpMenu);

            CustomControls.DeselectAllMenuButtons(CustomControls.FileMenu);
            CustomControls.DeselectAllMenuButtons(CustomControls.RecentlyOpenedMenu);
            CustomControls.DeselectAllMenuButtons(CustomControls.HelpMenu);

            File_Button.Image = Resources.FileGray;
            Help_Button.Image = Resources.HelpGray;

            DataGridViewManager.DoNotDeleteRows = false;
            MenuKeyShortcutManager.SelectedPanel = null;
        }
    }
}
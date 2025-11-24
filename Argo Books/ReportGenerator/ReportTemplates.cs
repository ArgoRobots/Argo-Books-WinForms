using Argo_Books;
using Argo_Books.Language;
using Argo_Books.ReportGenerator.Elements;

namespace Argo_Books.ReportGenerator
{
    /// <summary>
    /// Provides predefined report templates for common business reporting scenarios.
    /// </summary>
    public static class ReportTemplates
    {
        // Template creation methods
        public static ReportConfiguration CreateMonthlySalesTemplate()
        {
            ReportConfiguration config = new()
            {
                Title = LanguageManager.TranslateString("Monthly Sales Report"),
                PageSize = PageSize.A4,
                PageOrientation = PageOrientation.Landscape,
                ShowHeader = true,
                ShowFooter = true,
                ShowPageNumbers = true
            };

            // Configure filters for sales data
            config.Filters.TransactionType = TransactionType.Revenue;
            config.Filters.DatePresetName = DatePresetNames.ThisMonth;
            config.Filters.SelectedChartTypes.AddRange(
            [
                MainMenu_Form.ChartDataType.TotalRevenue,
                MainMenu_Form.ChartDataType.RevenueDistribution,
                MainMenu_Form.ChartDataType.GrowthRates,
                MainMenu_Form.ChartDataType.AverageTransactionValue
            ]);

            AddSalesReportElements(config);

            return config;
        }
        public static ReportConfiguration CreateFinancialOverviewTemplate()
        {
            ReportConfiguration config = new()
            {
                Title = LanguageManager.TranslateString("Financial Overview"),
                PageSize = PageSize.A4,
                PageOrientation = PageOrientation.Landscape,
                ShowHeader = true,
                ShowFooter = true,
                ShowPageNumbers = true
            };

            // Configure filters for financial data
            config.Filters.TransactionType = TransactionType.Revenue;
            config.Filters.DatePresetName = DatePresetNames.ThisQuarter;
            config.Filters.SelectedChartTypes.AddRange(
            [
                MainMenu_Form.ChartDataType.TotalRevenue,
                MainMenu_Form.ChartDataType.TotalExpenses,
                MainMenu_Form.ChartDataType.SalesVsExpenses,
                MainMenu_Form.ChartDataType.TotalProfits
            ]);

            AddFinancialOverviewElements(config);

            return config;
        }
        public static ReportConfiguration CreatePerformanceAnalysisTemplate()
        {
            ReportConfiguration config = new()
            {
                Title = LanguageManager.TranslateString("Performance Analysis"),
                PageSize = PageSize.A4,
                PageOrientation = PageOrientation.Portrait,
                ShowHeader = true,
                ShowFooter = true,
                ShowPageNumbers = true
            };

            // Configure filters for performance data
            config.Filters.TransactionType = TransactionType.Revenue;
            config.Filters.DatePresetName = DatePresetNames.Last30Days;
            config.Filters.SelectedChartTypes.AddRange(
            [
                MainMenu_Form.ChartDataType.GrowthRates,
                MainMenu_Form.ChartDataType.AverageTransactionValue,
                MainMenu_Form.ChartDataType.TotalTransactions,
                MainMenu_Form.ChartDataType.ReturnsOverTime
            ]);

            AddPerformanceAnalysisElements(config);

            return config;
        }
        public static ReportConfiguration CreateReturnsAnalysisTemplate()
        {
            ReportConfiguration config = new()
            {
                Title = LanguageManager.TranslateString("Returns Analysis"),
                PageSize = PageSize.A4,
                PageOrientation = PageOrientation.Portrait,
                ShowHeader = true,
                ShowFooter = true,
                ShowPageNumbers = true
            };

            // Configure filters for returns data only
            config.Filters.TransactionType = TransactionType.Revenue;
            config.Filters.IncludeReturns = true;
            config.Filters.IncludeLosses = false;
            config.Filters.DatePresetName = DatePresetNames.YearToDate;
            config.Filters.SelectedChartTypes.AddRange(
            [
                MainMenu_Form.ChartDataType.ReturnsOverTime,
                MainMenu_Form.ChartDataType.ReturnReasons,
                MainMenu_Form.ChartDataType.ReturnFinancialImpact,
                MainMenu_Form.ChartDataType.ReturnsByCategory,
                MainMenu_Form.ChartDataType.ReturnsByProduct
            ]);

            AddReturnsAnalysisElements(config);

            return config;
        }
        public static ReportConfiguration CreateLossesAnalysisTemplate()
        {
            ReportConfiguration config = new()
            {
                Title = LanguageManager.TranslateString("Losses Analysis"),
                PageSize = PageSize.A4,
                PageOrientation = PageOrientation.Portrait,
                ShowHeader = true,
                ShowFooter = true,
                ShowPageNumbers = true
            };

            // Configure filters for losses data only
            config.Filters.TransactionType = TransactionType.Revenue;
            config.Filters.IncludeReturns = false;
            config.Filters.IncludeLosses = true;
            config.Filters.DatePresetName = DatePresetNames.YearToDate;
            config.Filters.SelectedChartTypes.AddRange(
            [
                MainMenu_Form.ChartDataType.LossesOverTime,
                MainMenu_Form.ChartDataType.LossReasons,
                MainMenu_Form.ChartDataType.LossFinancialImpact,
                MainMenu_Form.ChartDataType.LossesByCategory,
                MainMenu_Form.ChartDataType.LossesByProduct
            ]);

            AddLossesAnalysisElements(config);

            return config;
        }
        public static ReportConfiguration CreateGeographicAnalysisTemplate()
        {
            ReportConfiguration config = new()
            {
                Title = LanguageManager.TranslateString("Geographic Analysis"),
                PageSize = PageSize.A4,
                PageOrientation = PageOrientation.Landscape,
                ShowHeader = true,
                ShowFooter = true,
                ShowPageNumbers = true
            };

            // Configure filters for geographic data
            config.Filters.TransactionType = TransactionType.Revenue;
            config.Filters.DatePresetName = DatePresetNames.Last30Days;
            config.Filters.SelectedChartTypes.AddRange(
            [
                MainMenu_Form.ChartDataType.WorldMap,
                MainMenu_Form.ChartDataType.CountriesOfOrigin,
                MainMenu_Form.ChartDataType.CountriesOfDestination,
                MainMenu_Form.ChartDataType.CompaniesOfOrigin
            ]);

            AddGeographicAnalysisElements(config);

            return config;
        }

        // Element addition methods
        private static void AddSalesReportElements(ReportConfiguration config)
        {
            TemplateLayoutHelper.LayoutContext context = new()
            {
                PageSize = PageDimensions.GetDimensions(config.PageSize, config.PageOrientation)
            };

            // Date range element
            config.AddElement(new DateRangeElement
            {
                Bounds = TemplateLayoutHelper.GetDateRangeBounds(context),
                ZOrder = 0
            });

            // Create a 2x2 grid for charts
            Rectangle[,] grid = TemplateLayoutHelper.CreateGrid(context, 2, 2);

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.TotalRevenue,
                Bounds = grid[0, 0],
                ZOrder = 1
            });

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.RevenueDistribution,
                Bounds = grid[0, 1],
                ZOrder = 2
            });

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.GrowthRates,
                Bounds = grid[1, 0],
                ZOrder = 3
            });

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.AverageTransactionValue,
                Bounds = grid[1, 1],
                ZOrder = 4
            });
        }
        private static void AddFinancialOverviewElements(ReportConfiguration config)
        {
            TemplateLayoutHelper.LayoutContext context = new()
            {
                PageSize = PageDimensions.GetDimensions(config.PageSize, config.PageOrientation)
            };

            // Date range
            config.AddElement(new DateRangeElement
            {
                Bounds = TemplateLayoutHelper.GetDateRangeBounds(context),
                ZOrder = 0
            });

            // Create 2x2 grid for charts
            Rectangle[,] grid = TemplateLayoutHelper.CreateGrid(context, 2, 2);

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.SalesVsExpenses,
                Bounds = grid[0, 0],
                ZOrder = 1
            });

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.TotalProfits,
                Bounds = grid[0, 1],
                ZOrder = 2
            });

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.TotalRevenue,
                Bounds = grid[1, 0],
                ZOrder = 3
            });

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.TotalExpenses,
                Bounds = grid[1, 1],
                ZOrder = 4
            });
        }
        private static void AddPerformanceAnalysisElements(ReportConfiguration config)
        {
            TemplateLayoutHelper.LayoutContext context = new()
            {
                PageSize = PageDimensions.GetDimensions(config.PageSize, config.PageOrientation)
            };

            // Date range
            config.AddElement(new DateRangeElement
            {
                Bounds = TemplateLayoutHelper.GetDateRangeBounds(context),
                ZOrder = 0
            });

            // Create vertical stack: summary (15%), then 3 equal charts
            Rectangle[] stack = TemplateLayoutHelper.CreateVerticalStack(context, 0.15f, 0.28f, 0.28f, 0.29f);

            config.AddElement(new SummaryElement
            {
                Bounds = stack[0],
                ZOrder = 1
            });

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.GrowthRates,
                Bounds = stack[1],
                ZOrder = 2
            });

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.AverageTransactionValue,
                Bounds = stack[2],
                ZOrder = 3
            });

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.ReturnsOverTime,
                Bounds = stack[3],
                ZOrder = 4
            });
        }
        private static void AddReturnsAnalysisElements(ReportConfiguration config)
        {
            TemplateLayoutHelper.LayoutContext context = new()
            {
                PageSize = PageDimensions.GetDimensions(config.PageSize, config.PageOrientation)
            };

            // Date range
            config.AddElement(new DateRangeElement
            {
                Bounds = TemplateLayoutHelper.GetDateRangeBounds(context),
                ZOrder = 0
            });

            // Mixed layout: full-width chart at top, then 2x2 grid
            Rectangle[] topStack = TemplateLayoutHelper.CreateVerticalStack(context, 0.33f, 0.67f);

            // Use the top portion for the full-width chart
            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.ReturnsOverTime,
                Bounds = topStack[0],
                ZOrder = 1
            });

            // Create a 2x2 grid in the bottom portion
            Rectangle bottomArea = topStack[1];
            int gridWidth = (bottomArea.Width - context.ElementSpacing) / 2;
            int gridHeight = (bottomArea.Height - context.ElementSpacing) / 2;

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.ReturnReasons,
                Bounds = new Rectangle(bottomArea.X, bottomArea.Y, gridWidth, gridHeight),
                ZOrder = 2
            });

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.ReturnFinancialImpact,
                Bounds = new Rectangle(bottomArea.X + gridWidth + context.ElementSpacing, bottomArea.Y, gridWidth, gridHeight),
                ZOrder = 3
            });

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.ReturnsByCategory,
                Bounds = new Rectangle(bottomArea.X, bottomArea.Y + gridHeight + context.ElementSpacing, gridWidth, gridHeight),
                ZOrder = 4
            });

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.ReturnsByProduct,
                Bounds = new Rectangle(bottomArea.X + gridWidth + context.ElementSpacing, bottomArea.Y + gridHeight + context.ElementSpacing, gridWidth, gridHeight),
                ZOrder = 5
            });
        }
        private static void AddLossesAnalysisElements(ReportConfiguration config)
        {
            TemplateLayoutHelper.LayoutContext context = new()
            {
                PageSize = PageDimensions.GetDimensions(config.PageSize, config.PageOrientation)
            };

            // Date range
            config.AddElement(new DateRangeElement
            {
                Bounds = TemplateLayoutHelper.GetDateRangeBounds(context),
                ZOrder = 0
            });

            // Mixed layout: full-width chart at top, then 2x2 grid
            Rectangle[] topStack = TemplateLayoutHelper.CreateVerticalStack(context, 0.33f, 0.67f);

            // Use the top portion for the full-width chart
            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.LossesOverTime,
                Bounds = topStack[0],
                ZOrder = 1
            });

            // Create a 2x2 grid in the bottom portion
            Rectangle bottomArea = topStack[1];
            int gridWidth = (bottomArea.Width - context.ElementSpacing) / 2;
            int gridHeight = (bottomArea.Height - context.ElementSpacing) / 2;

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.LossReasons,
                Bounds = new Rectangle(bottomArea.X, bottomArea.Y, gridWidth, gridHeight),
                ZOrder = 2
            });

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.LossFinancialImpact,
                Bounds = new Rectangle(bottomArea.X + gridWidth + context.ElementSpacing, bottomArea.Y, gridWidth, gridHeight),
                ZOrder = 3
            });

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.LossesByCategory,
                Bounds = new Rectangle(bottomArea.X, bottomArea.Y + gridHeight + context.ElementSpacing, gridWidth, gridHeight),
                ZOrder = 4
            });

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.LossesByProduct,
                Bounds = new Rectangle(bottomArea.X + gridWidth + context.ElementSpacing, bottomArea.Y + gridHeight + context.ElementSpacing, gridWidth, gridHeight),
                ZOrder = 5
            });
        }
        private static void AddGeographicAnalysisElements(ReportConfiguration config)
        {
            TemplateLayoutHelper.LayoutContext context = new()
            {
                PageSize = PageDimensions.GetDimensions(config.PageSize, config.PageOrientation)
            };

            // Date range
            config.AddElement(new DateRangeElement
            {
                Bounds = TemplateLayoutHelper.GetDateRangeBounds(context),
                ZOrder = 0
            });

            // Create vertical split: map (50%), then bottom grid (50%)
            Rectangle[] stack = TemplateLayoutHelper.CreateVerticalStack(context, 0.5f, 0.5f);

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.WorldMap,
                Bounds = stack[0],
                ZOrder = 1
            });

            // Split bottom area into 2 columns
            Rectangle bottomArea = stack[1];
            int colWidth = (bottomArea.Width - context.ElementSpacing) / 2;

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.CountriesOfOrigin,
                Bounds = new Rectangle(bottomArea.X, bottomArea.Y, colWidth, bottomArea.Height),
                ZOrder = 2
            });

            config.AddElement(new ChartElement
            {
                ChartType = MainMenu_Form.ChartDataType.CountriesOfDestination,
                Bounds = new Rectangle(bottomArea.X + colWidth + context.ElementSpacing, bottomArea.Y, colWidth, bottomArea.Height),
                ZOrder = 3
            });
        }

        // Template management
        public static class TemplateNames
        {
            public const string Custom = "Custom Report";
            public const string MonthlySales = "Monthly Sales Report";
            public const string FinancialOverview = "Financial Overview";
            public const string PerformanceAnalysis = "Performance Analysis";
            public const string ReturnsAnalysis = "Returns Analysis";
            public const string LossesAnalysis = "Losses Analysis";
            public const string GeographicAnalysis = "Geographic Analysis";
        }
        public static class DatePresetNames
        {
            public const string Today = "Today";
            public const string Yesterday = "Yesterday";
            public const string Last7Days = "Last 7 days";
            public const string Last30Days = "Last 30 days";
            public const string ThisMonth = "This month";
            public const string LastMonth = "Last month";
            public const string ThisQuarter = "This quarter";
            public const string LastQuarter = "Last quarter";
            public const string YearToDate = "Year to date";
            public const string LastYear = "Last year";
            public const string AllTime = "All time";
            public const string Custom = "Custom";
        }
        public static List<string> GetAvailableTemplates()
        {
            List<string> templates =
            [
                TemplateNames.Custom,
                TemplateNames.MonthlySales,
                TemplateNames.FinancialOverview,
                TemplateNames.PerformanceAnalysis,
                TemplateNames.ReturnsAnalysis,
                TemplateNames.LossesAnalysis,
                TemplateNames.GeographicAnalysis
            ];

            // Add custom templates
            List<string> customTemplates = CustomTemplateStorage.GetCustomTemplateNames();
            templates.AddRange(customTemplates);

            return templates;
        }

        /// <summary>
        /// Checks if the given template name is a built-in template.
        /// </summary>
        public static bool IsBuiltInTemplate(string templateName)
        {
            if (string.IsNullOrWhiteSpace(templateName))
            {
                return false;
            }

            return templateName == TemplateNames.Custom ||
                   templateName == TemplateNames.MonthlySales ||
                   templateName == TemplateNames.FinancialOverview ||
                   templateName == TemplateNames.PerformanceAnalysis ||
                   templateName == TemplateNames.ReturnsAnalysis ||
                   templateName == TemplateNames.LossesAnalysis ||
                   templateName == TemplateNames.GeographicAnalysis;
        }
        public static ReportConfiguration CreateFromTemplateIndex(int index)
        {
            // First 7 templates are built-in
            if (index == 0) { return new ReportConfiguration(); }  // Custom report
            if (index == 1) { return CreateMonthlySalesTemplate(); }
            if (index == 2) { return CreateFinancialOverviewTemplate(); }
            if (index == 3) { return CreatePerformanceAnalysisTemplate(); }
            if (index == 4) { return CreateReturnsAnalysisTemplate(); }
            if (index == 5) { return CreateLossesAnalysisTemplate(); }
            if (index == 6) { return CreateGeographicAnalysisTemplate(); }

            // Custom templates start at index 7
            List<string> customTemplates = CustomTemplateStorage.GetCustomTemplateNames();
            int customIndex = index - 7;

            if (customIndex >= 0 && customIndex < customTemplates.Count)
            {
                ReportConfiguration config = CustomTemplateStorage.LoadTemplate(customTemplates[customIndex]);
                return config ?? new ReportConfiguration();
            }

            return new ReportConfiguration();
        }
    }
}
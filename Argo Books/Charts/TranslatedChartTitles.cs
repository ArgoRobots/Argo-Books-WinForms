using Argo_Books;
using Argo_Books.Language;

namespace Argo_Books.Charts
{
    /// <summary>
    /// Provides translated chart titles for LiveCharts.
    /// </summary>
    public static class TranslatedChartTitles
    {
        private static string Translate(string str) => LanguageManager.TranslateString(str);

        // Main Revenue and Expense Charts
        public static string TotalRevenue => Translate("Total revenue");
        public static string TotalExpenses => Translate("Total expenses");
        public static string RevenueDistribution => Translate("Distribution of revenue");
        public static string ExpensesDistribution => Translate("Distribution of expenses");
        public static string TotalProfits => Translate("Total profits");

        // Geographic Analysis Charts
        public static string CountriesOfOrigin => Translate("Countries of origin for purchases");
        public static string CompaniesOfOrigin => Translate("Companies of origin for purchases");
        public static string CountriesOfDestination => Translate("Countries of destination for sales");
        public static string WorldMap => Translate("World map");

        // Operational Charts
        public static string AccountantsTransactions => Translate("Transactions managed by accountants");

        // Performance and Growth Charts
        public static string SalesVsExpenses => Translate("Total expenses vs. total revenue");
        public static string AverageTransactionValue => Translate("Average transaction value");
        public static string TotalTransactions => Translate("Total transactions");
        public static string AverageShippingCosts => Translate("Average shipping costs");
        public static string GrowthRates => Translate("Expenses and revenue growth rates");

        // Returns Analysis Charts
        public static string ReturnsOverTime => Translate("Returns over time");
        public static string ReturnReasons => Translate("Return reasons");
        public static string ReturnFinancialImpact => Translate("Financial impact of returns");
        public static string ReturnsByCategory => Translate("Returns by category");
        public static string ReturnsByProduct => Translate("Returns by product");
        public static string PurchaseVsSaleReturns => Translate("Purchase vs. sale returns");

        // Losses Analysis Charts
        public static string LossesOverTime => LanguageManager.TranslateString("Losses over time");
        public static string LossReasons => LanguageManager.TranslateString("Loss reasons");
        public static string LossFinancialImpact => LanguageManager.TranslateString("Financial impact of losses");
        public static string LossesByCategory => LanguageManager.TranslateString("Losses by category");
        public static string LossesByProduct => LanguageManager.TranslateString("Losses by product");
        public static string PurchaseVsSaleLosses => LanguageManager.TranslateString("Purchase vs. sale losses");

        public static string GetChartDisplayName(MainMenu_Form.ChartDataType chartType)
        {
            return chartType switch
            {
                MainMenu_Form.ChartDataType.TotalRevenue => TotalRevenue,
                MainMenu_Form.ChartDataType.TotalExpenses => TotalExpenses,
                MainMenu_Form.ChartDataType.RevenueDistribution => RevenueDistribution,
                MainMenu_Form.ChartDataType.ExpensesDistribution => ExpensesDistribution,
                MainMenu_Form.ChartDataType.TotalProfits => TotalProfits,
                MainMenu_Form.ChartDataType.CountriesOfOrigin => CountriesOfOrigin,
                MainMenu_Form.ChartDataType.CompaniesOfOrigin => CompaniesOfOrigin,
                MainMenu_Form.ChartDataType.CountriesOfDestination => CountriesOfDestination,
                MainMenu_Form.ChartDataType.WorldMap => WorldMap,
                MainMenu_Form.ChartDataType.AccountantsTransactions => AccountantsTransactions,
                MainMenu_Form.ChartDataType.SalesVsExpenses => SalesVsExpenses,
                MainMenu_Form.ChartDataType.AverageTransactionValue => AverageTransactionValue,
                MainMenu_Form.ChartDataType.TotalTransactions => TotalTransactions,
                MainMenu_Form.ChartDataType.AverageShippingCosts => AverageShippingCosts,
                MainMenu_Form.ChartDataType.GrowthRates => GrowthRates,
                MainMenu_Form.ChartDataType.ReturnsOverTime => ReturnsOverTime,
                MainMenu_Form.ChartDataType.ReturnReasons => ReturnReasons,
                MainMenu_Form.ChartDataType.ReturnFinancialImpact => ReturnFinancialImpact,
                MainMenu_Form.ChartDataType.ReturnsByCategory => ReturnsByCategory,
                MainMenu_Form.ChartDataType.ReturnsByProduct => ReturnsByProduct,
                MainMenu_Form.ChartDataType.PurchaseVsSaleReturns => PurchaseVsSaleReturns,
                MainMenu_Form.ChartDataType.LossesOverTime => LossesOverTime,
                MainMenu_Form.ChartDataType.LossReasons => LossReasons,
                MainMenu_Form.ChartDataType.LossFinancialImpact => LossFinancialImpact,
                MainMenu_Form.ChartDataType.LossesByCategory => LossesByCategory,
                MainMenu_Form.ChartDataType.LossesByProduct => LossesByProduct,
                MainMenu_Form.ChartDataType.PurchaseVsSaleLosses => PurchaseVsSaleLosses,
                _ => throw new ArgumentOutOfRangeException(nameof(chartType), chartType, "Unknown chart type")
            };
        }
    }
}
using Argo_Books.AnonymousData;
using Guna.UI2.WinForms;
using Argo_Books;
using Argo_Books.Classes;
using System.Diagnostics;

namespace Argo_Books.AISearch
{
    /// <summary>
    /// Extension to the existing search functionality to add AI capabilities.
    /// </summary>
    public static class AISearchExtensions
    {
        // Properties
        private static AIQueryTranslator _queryTranslator;
        private static string _originalQuery = "";
        private static string _translatedQuery = "";

        // Getter
        public static bool IsUsingAIQuery { get; private set; } = false;

        // Init.
        public static void InitializeAISearch(string apiKey)
        {
            _queryTranslator = new AIQueryTranslator(apiKey);
            Log.Write(2, "AI Search functionality initialized");
        }

        /// <summary>
        /// Enhances the search box with AI query translation without replacing the input text.
        /// </summary>
        public static async Task EnhanceSearchAsync(this Guna2TextBox searchBox)
        {
            try
            {
                string userQuery = searchBox.Text;
                string naturalLanguageQuery = userQuery.Substring(1).Trim();
                _originalQuery = naturalLanguageQuery;

                // Calculate dynamic thresholds for all numerical fields based on current data
                DynamicThresholds thresholds = null;
                try
                {
                    var selectedDataGridView = MainMenu_Form.Instance?.SelectedDataGridView;
                    if (selectedDataGridView != null)
                    {
                        thresholds = PercentileCalculator.CalculateAllThresholds(selectedDataGridView);
                        Log.WriteWithFormat(2, "Calculated dynamic thresholds: {0}", thresholds.ToString());
                    }
                }
                catch (Exception thresholdEx)
                {
                    Log.WriteWithFormat(1, "Failed to calculate dynamic thresholds, using defaults: {0}", thresholdEx.Message);
                    thresholds = null;  // Will use defaults in translator
                }

                Stopwatch stopwatch = Stopwatch.StartNew();
                _translatedQuery = await _queryTranslator.TranslateQueryAsync(naturalLanguageQuery, thresholds);
                stopwatch.Stop();

                // Track the API usage data
                AnonymousDataManager.AddOpenAIUsageData(AIQueryTranslator.Model, stopwatch.ElapsedMilliseconds, _queryTranslator.LastTokenUsage);

                IsUsingAIQuery = true;
                Log.WriteWithFormat(2, "AI translated '{0}' to '{1}' (Used {2} tokens)", naturalLanguageQuery, _translatedQuery, _queryTranslator.LastTokenUsage);
                MainMenu_Form.Instance.RefreshDataGridViewAndCharts();
            }
            catch (Exception ex)
            {
                Log.Error_EnhancingSearch(ex.Message);
                IsUsingAIQuery = false;
            }
        }
        public static void ResetQuery()
        {
            // Reset the AI query flag if the user is doing a normal search
            IsUsingAIQuery = false;
            _originalQuery = "";
            _translatedQuery = "";
        }

        /// <summary>
        /// Returns the translated query if using AI search, otherwise returns the original query.
        /// </summary>
        public static string GetEffectiveSearchQuery(string displayedQuery)
        {
            if (IsUsingAIQuery && displayedQuery.StartsWith('!'))
            {
                return _translatedQuery;
            }
            return displayedQuery;
        }

        /// <summary>
        /// Returns the query that should be displayed in the "Showing results for" label.
        /// </summary>
        public static string GetDisplayQuery(string displayedQuery)
        {
            if (IsUsingAIQuery && displayedQuery.StartsWith('!'))
            {
                return _originalQuery;
            }
            return displayedQuery;
        }
    }
}
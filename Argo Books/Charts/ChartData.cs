namespace Argo_Books.Charts
{
    /// <summary>
    /// Represents chart data containing a total value and associated data points for visualization.
    /// </summary>
    public record ChartData
    {
        // Getters
        public double Total { get; }
        public IReadOnlyDictionary<string, double> Data { get; }

        // Constructor
        public ChartData(double total, Dictionary<string, double> data)
        {
            Total = total;
            Data = new Dictionary<string, double>(data ?? []).AsReadOnly();  // Defensive copy
        }

        // Static factory method for creating empty ChartData
        public static ChartData Empty => new(0, []);
    }

    /// <summary>
    /// Represents chart data containing a total value as an integer and associated data points for visualization.
    /// </summary>
    internal class ChartCountData(Dictionary<string, int> data)
    {
        // Getters
        public IReadOnlyDictionary<string, int> Data { get; } = data ?? [];

        // Static factory method for creating empty ChartData
        public static ChartCountData Empty => new([]);
    }

    /// <summary>
    /// Represents chart data specifically for sales vs expenses comparisons, containing separate datasets
    /// for expenses and sales along with their chronological ordering.
    /// </summary>
    public record SalesExpensesChartData
    {
        // Properties
        private readonly Dictionary<string, double> _expensesData;
        private readonly Dictionary<string, double> _salesData;
        private readonly IReadOnlyList<string> _dateOrder;

        // Getter
        public IReadOnlyList<string> GetDateOrder() => _dateOrder;

        // Constructor
        public SalesExpensesChartData(
            Dictionary<string, double> expensesData,
            Dictionary<string, double> salesData,
            IEnumerable<string> dateOrder)
        {
            _expensesData = new Dictionary<string, double>(expensesData ?? []);
            _salesData = new Dictionary<string, double>(salesData ?? []);
            _dateOrder = (dateOrder?.ToList() ?? []).AsReadOnly();
        }

        // Static factory method for creating empty data
        public static SalesExpensesChartData Empty => new([], [], []);

        // Query methods
        public double GetExpensesForDate(string date) =>
            _expensesData.TryGetValue(date, out double value) ? value : 0;
        public double GetSalesForDate(string date) =>
            _salesData.TryGetValue(date, out double value) ? value : 0;
    }
}
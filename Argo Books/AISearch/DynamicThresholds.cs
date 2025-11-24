namespace Argo_Books.AISearch
{
    /// <summary>
    /// Holds dynamically calculated thresholds for various numerical fields.
    /// These thresholds are used to translate qualitative terms (like "expensive", "high discount")
    /// into quantitative search criteria based on actual data.
    /// </summary>
    public class DynamicThresholds
    {
        // Total/Cost thresholds
        public decimal? HighTotal { get; set; }
        public decimal? LowTotal { get; set; }

        // Price per unit thresholds
        public decimal? HighPrice { get; set; }
        public decimal? LowPrice { get; set; }

        // Discount thresholds
        public decimal? HighDiscount { get; set; }
        public decimal? LowDiscount { get; set; }

        // Shipping thresholds
        public decimal? HighShipping { get; set; }
        public decimal? LowShipping { get; set; }

        // Tax thresholds
        public decimal? HighTax { get; set; }
        public decimal? LowTax { get; set; }

        // Fee thresholds
        public decimal? HighFee { get; set; }
        public decimal? LowFee { get; set; }

        // Quantity thresholds
        public decimal? HighQuantity { get; set; }
        public decimal? LowQuantity { get; set; }

        // Charged difference thresholds
        public decimal? HighChargedDifference { get; set; }
        public decimal? LowChargedDifference { get; set; }

        /// <summary>
        /// Returns a formatted string representation of the thresholds for logging.
        /// </summary>
        public override string ToString()
        {
            return $"Total: {FormatRange(LowTotal, HighTotal)}, " +
                   $"Price: {FormatRange(LowPrice, HighPrice)}, " +
                   $"Discount: {FormatRange(LowDiscount, HighDiscount)}, " +
                   $"Shipping: {FormatRange(LowShipping, HighShipping)}";
        }

        private static string FormatRange(decimal? low, decimal? high)
        {
            if (low.HasValue && high.HasValue)
                return $"<{low:F2} / >{high:F2}";
            else if (high.HasValue)
                return $">{high:F2}";
            else if (low.HasValue)
                return $"<{low:F2}";
            else
                return "N/A";
        }

        /// <summary>
        /// Creates default thresholds with hardcoded values (used as fallback).
        /// </summary>
        public static DynamicThresholds CreateDefault()
        {
            return new DynamicThresholds
            {
                HighTotal = 200,
                LowTotal = 50,
                HighPrice = 100,
                LowPrice = 50,
                HighDiscount = 5,
                LowDiscount = 0,
                HighShipping = 20,
                LowShipping = 0,
                HighTax = 20,
                LowTax = 0,
                HighFee = 10,
                LowFee = 0,
                HighQuantity = 10,
                LowQuantity = 1,
                HighChargedDifference = 50,
                LowChargedDifference = -50
            };
        }
    }
}

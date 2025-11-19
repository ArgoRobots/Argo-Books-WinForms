using Sales_Tracker.AISearch;
using System.Windows.Forms;

namespace Tests
{
    [TestClass]
    public class PercentileCalculator_UnitTest
    {
        [TestMethod]
        public void CalculatePercentile_WithValidData_ReturnsCorrectValue()
        {
            // Arrange
            List<decimal> values = [10, 20, 30, 40, 50, 60, 70, 80, 90, 100];

            // Act
            decimal percentile50 = PercentileCalculator.CalculatePercentile(values, 50);
            decimal percentile75 = PercentileCalculator.CalculatePercentile(values, 75);
            decimal percentile90 = PercentileCalculator.CalculatePercentile(values, 90);

            // Assert
            Assert.AreEqual(55, percentile50, "50th percentile should be 55");
            Assert.AreEqual(77.5m, percentile75, "75th percentile should be 77.5");
            Assert.AreEqual(91, percentile90, "90th percentile should be 91");
        }

        [TestMethod]
        public void CalculatePercentile_WithSingleValue_ReturnsThatValue()
        {
            // Arrange
            List<decimal> values = [42];

            // Act
            decimal result = PercentileCalculator.CalculatePercentile(values, 50);

            // Assert
            Assert.AreEqual(42, result);
        }

        [TestMethod]
        public void CalculatePercentile_WithEmptyList_ReturnsZero()
        {
            // Arrange
            List<decimal> values = [];

            // Act
            decimal result = PercentileCalculator.CalculatePercentile(values, 50);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void CalculatePercentile_WithInvalidPercentile_ThrowsException()
        {
            // Arrange
            List<decimal> values = [10, 20, 30];

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => PercentileCalculator.CalculatePercentile(values, 150));  // Invalid percentile
        }

        [TestMethod]
        public void GetDynamicPercentileThreshold_WithVeryFewTransactions_Returns50()
        {
            // Arrange & Act
            double threshold10 = PercentileCalculator.GetDynamicPercentileThreshold(10);
            double threshold20 = PercentileCalculator.GetDynamicPercentileThreshold(20);

            // Assert
            Assert.AreEqual(50, threshold10, "10 transactions should use top 50% (percentile 50)");
            Assert.AreEqual(50, threshold20, "20 transactions should use top 50% (percentile 50)");
        }

        [TestMethod]
        public void GetDynamicPercentileThreshold_WithFewTransactions_Returns75()
        {
            // Arrange & Act
            double threshold30 = PercentileCalculator.GetDynamicPercentileThreshold(30);
            double threshold50 = PercentileCalculator.GetDynamicPercentileThreshold(50);

            // Assert
            Assert.AreEqual(75, threshold30, "30 transactions should use top 25% (percentile 75)");
            Assert.AreEqual(75, threshold50, "50 transactions should use top 25% (percentile 75)");
        }

        [TestMethod]
        public void GetDynamicPercentileThreshold_WithMediumTransactions_Returns90()
        {
            // Arrange & Act
            double threshold100 = PercentileCalculator.GetDynamicPercentileThreshold(100);
            double threshold200 = PercentileCalculator.GetDynamicPercentileThreshold(200);

            // Assert
            Assert.AreEqual(90, threshold100, "100 transactions should use top 10% (percentile 90)");
            Assert.AreEqual(90, threshold200, "200 transactions should use top 10% (percentile 90)");
        }

        [TestMethod]
        public void GetDynamicPercentileThreshold_WithManyTransactions_Returns95()
        {
            // Arrange & Act
            double threshold300 = PercentileCalculator.GetDynamicPercentileThreshold(300);
            double threshold500 = PercentileCalculator.GetDynamicPercentileThreshold(500);

            // Assert
            Assert.AreEqual(95, threshold300, "300 transactions should use top 5% (percentile 95)");
            Assert.AreEqual(95, threshold500, "500 transactions should use top 5% (percentile 95)");
        }

        [TestMethod]
        public void GetDynamicPercentileThreshold_WithLotsOfTransactions_Returns98()
        {
            // Arrange & Act
            double threshold1000 = PercentileCalculator.GetDynamicPercentileThreshold(1000);

            // Assert
            Assert.AreEqual(98, threshold1000, "1000 transactions should use top 2% (percentile 98)");
        }

        [TestMethod]
        public void GetColumnDecimalValues_WithValidColumn_ReturnsValues()
        {
            // Arrange
            DataGridView dataGridView = new();
            dataGridView.Columns.Add("Total", "Total");

            dataGridView.Rows.Add();
            dataGridView.Rows.Add();
            dataGridView.Rows.Add();

            dataGridView.Rows[0].Cells["Total"].Value = "100.50";
            dataGridView.Rows[1].Cells["Total"].Value = "200.75";
            dataGridView.Rows[2].Cells["Total"].Value = "300.25";

            // Act
            List<decimal> values = PercentileCalculator.GetColumnDecimalValues(dataGridView, "Total");

            // Assert
            Assert.AreEqual(3, values.Count);
            Assert.AreEqual(100.50m, values[0]);
            Assert.AreEqual(200.75m, values[1]);
            Assert.AreEqual(300.25m, values[2]);
        }

        [TestMethod]
        public void GetColumnDecimalValues_WithInvalidColumn_ReturnsEmptyList()
        {
            // Arrange
            DataGridView dataGridView = new();
            dataGridView.Columns.Add("Total", "Total");

            // Act
            List<decimal> values = PercentileCalculator.GetColumnDecimalValues(dataGridView, "NonExistentColumn");

            // Assert
            Assert.AreEqual(0, values.Count);
        }

        [TestMethod]
        public void GetColumnDecimalValues_WithNullValues_SkipsThem()
        {
            // Arrange
            DataGridView dataGridView = new();
            dataGridView.Columns.Add("Total", "Total");

            dataGridView.Rows.Add();
            dataGridView.Rows.Add();
            dataGridView.Rows.Add();

            dataGridView.Rows[0].Cells["Total"].Value = "100";
            dataGridView.Rows[1].Cells["Total"].Value = null;
            dataGridView.Rows[2].Cells["Total"].Value = "200";

            // Act
            List<decimal> values = PercentileCalculator.GetColumnDecimalValues(dataGridView, "Total");

            // Assert
            Assert.AreEqual(2, values.Count);
            Assert.AreEqual(100m, values[0]);
            Assert.AreEqual(200m, values[1]);
        }

        [TestMethod]
        public void GetExpensiveItemThreshold_WithNoData_ReturnsDefault()
        {
            // Arrange
            DataGridView dataGridView = new();
            dataGridView.Columns.Add("Total", "Total");

            // Act
            decimal threshold = PercentileCalculator.GetExpensiveItemThreshold(dataGridView);

            // Assert
            Assert.AreEqual(200, threshold, "Should return default 200 when no data");
        }

        [TestMethod]
        public void GetExpensiveItemThreshold_WithData_ReturnsPercentileValue()
        {
            // Arrange
            DataGridView dataGridView = new();
            dataGridView.Columns.Add("Total", "Total");

            // Add 10 rows (which should trigger 50th percentile threshold)
            for (int i = 1; i <= 10; i++)
            {
                dataGridView.Rows.Add();
                dataGridView.Rows[i - 1].Cells["Total"].Value = (i * 100).ToString();
            }

            // Act
            decimal threshold = PercentileCalculator.GetExpensiveItemThreshold(dataGridView);

            // Assert - With 10 items, we use 50th percentile, which should be around 550
            Assert.IsTrue(threshold > 400 && threshold < 700,
                $"Expected threshold around 550 for 50th percentile of values 100-1000, got {threshold}");
        }

        [TestMethod]
        public void GetExpensiveItemThreshold_WithManyTransactions_UsesHigherPercentile()
        {
            // Arrange
            DataGridView dataGridView = new();
            dataGridView.Columns.Add("Total", "Total");

            // Add 1000 rows (which should trigger 98th percentile threshold)
            for (int i = 1; i <= 1000; i++)
            {
                dataGridView.Rows.Add();
                dataGridView.Rows[i - 1].Cells["Total"].Value = i.ToString();
            }

            // Act
            decimal threshold = PercentileCalculator.GetExpensiveItemThreshold(dataGridView);

            // Assert - With 1000 items, we use 98th percentile, which should be around 980
            Assert.IsTrue(threshold > 970 && threshold < 990,
                $"Expected threshold around 980 for 98th percentile of values 1-1000, got {threshold}");
        }

        [TestMethod]
        public void CalculateAllThresholds_WithNullDataGridView_ReturnsDefaults()
        {
            // Act
            DynamicThresholds thresholds = PercentileCalculator.CalculateAllThresholds(null);

            // Assert
            Assert.AreEqual(200, thresholds.HighTotal);
            Assert.AreEqual(50, thresholds.LowTotal);
            Assert.AreEqual(100, thresholds.HighPrice);
            Assert.AreEqual(50, thresholds.LowPrice);
        }

        [TestMethod]
        public void CalculateAllThresholds_WithValidData_CalculatesAllFields()
        {
            // Arrange
            DataGridView dataGridView = new();
            dataGridView.Columns.Add("Total", "Total");
            dataGridView.Columns.Add("Price per unit", "Price per unit");
            dataGridView.Columns.Add("Discount", "Discount");
            dataGridView.Columns.Add("Shipping", "Shipping");

            // Add 100 rows with various values
            for (int i = 1; i <= 100; i++)
            {
                dataGridView.Rows.Add();
                dataGridView.Rows[i - 1].Cells["Total"].Value = (i * 10).ToString();
                dataGridView.Rows[i - 1].Cells["Price per unit"].Value = (i * 2).ToString();
                dataGridView.Rows[i - 1].Cells["Discount"].Value = (i * 0.5m).ToString();
                dataGridView.Rows[i - 1].Cells["Shipping"].Value = (i * 0.3m).ToString();
            }

            // Act
            DynamicThresholds thresholds = PercentileCalculator.CalculateAllThresholds(dataGridView);

            // High thresholds should be around 90% of max
            Assert.IsTrue(thresholds.HighTotal > 800 && thresholds.HighTotal < 1000,
                $"Expected HighTotal around 900, got {thresholds.HighTotal}");
            Assert.IsTrue(thresholds.HighPrice > 160 && thresholds.HighPrice < 200,
                $"Expected HighPrice around 180, got {thresholds.HighPrice}");

            // Low thresholds should be around 10% of max
            Assert.IsTrue(thresholds.LowTotal > 0 && thresholds.LowTotal < 200,
                $"Expected LowTotal around 100, got {thresholds.LowTotal}");
            Assert.IsTrue(thresholds.LowPrice > 0 && thresholds.LowPrice < 40,
                $"Expected LowPrice around 20, got {thresholds.LowPrice}");
        }

        [TestMethod]
        public void CalculateAllThresholds_WithMissingColumns_UsesFallbackDefaults()
        {
            // Arrange
            DataGridView dataGridView = new();
            dataGridView.Columns.Add("Total", "Total");
            // Only add Total column, other columns missing

            for (int i = 1; i <= 100; i++)
            {
                dataGridView.Rows.Add();
                dataGridView.Rows[i - 1].Cells["Total"].Value = (i * 10).ToString();
            }

            // Act
            DynamicThresholds thresholds = PercentileCalculator.CalculateAllThresholds(dataGridView);

            // Assert
            Assert.IsTrue(thresholds.HighTotal > 0);
            Assert.AreEqual(100, thresholds.HighPrice);
            Assert.AreEqual(5, thresholds.HighDiscount);
        }

        [TestMethod]
        public void DynamicThresholds_CreateDefault_ReturnsExpectedValues()
        {
            // Act
            DynamicThresholds defaults = DynamicThresholds.CreateDefault();

            // Assert
            Assert.AreEqual(200, defaults.HighTotal);
            Assert.AreEqual(50, defaults.LowTotal);
            Assert.AreEqual(100, defaults.HighPrice);
            Assert.AreEqual(50, defaults.LowPrice);
            Assert.AreEqual(5, defaults.HighDiscount);
            Assert.AreEqual(0, defaults.LowDiscount);
            Assert.AreEqual(20, defaults.HighShipping);
            Assert.AreEqual(0, defaults.LowShipping);
        }

        [TestMethod]
        public void DynamicThresholds_ToString_ReturnsFormattedString()
        {
            // Arrange
            DynamicThresholds thresholds = new()
            {
                HighTotal = 500,
                LowTotal = 100,
                HighPrice = 200,
                LowPrice = 50
            };

            // Act
            string result = thresholds.ToString();

            // Assert
            Assert.IsTrue(result.Contains("500.00"));
            Assert.IsTrue(result.Contains("100.00"));
            Assert.IsTrue(result.Contains("Total:"));
            Assert.IsTrue(result.Contains("Price:"));
        }
    }
}

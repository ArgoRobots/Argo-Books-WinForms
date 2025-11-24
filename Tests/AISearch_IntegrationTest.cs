using Guna.UI2.WinForms;
using Argo_Books.GridView;
using System.Drawing;
using System.Windows.Forms;

namespace Tests
{
    [TestClass]
    public class AISearch_IntegrationTest
    {
        private Guna2DataGridView _dataGridView;
        private Form _testForm;

        [TestInitialize]
        public void Setup()
        {
            // Create a test form to host the DataGridView (needed for proper visibility handling)
            _testForm = new Form
            {
                Size = new Size(800, 600)
            };

            // Create a test DataGridView
            _dataGridView = new Guna2DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false  // Prevent "new row" placeholder
            };

            // Add columns
            _dataGridView.Columns.Add("Order #", "Order #");
            _dataGridView.Columns.Add("Product", "Product");
            _dataGridView.Columns.Add("Country of origin", "Country of origin");
            _dataGridView.Columns.Add("Company of origin", "Company of origin");
            _dataGridView.Columns.Add("Date", "Date");
            _dataGridView.Columns.Add("Total items", "Total items");
            _dataGridView.Columns.Add("Discount", "Discount");
            _dataGridView.Columns.Add("Total", "Total");
            _dataGridView.Columns.Add("Notes", "Notes");
            _dataGridView.Columns.Add("Has receipt", "Has receipt");

            // Add to form and prepare
            _testForm.Controls.Add(_dataGridView);
            Application.EnableVisualStyles();
            _testForm.Show();

            // Add test rows after the DataGridView is initialized
            AddTestDataToDataGridView();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Close and dispose of the form (this will also dispose the DataGridView)
            if (_testForm != null)
            {
                if (_testForm.Visible)
                {
                    _testForm.Close();
                }
                _testForm.Dispose();
                _testForm = null;
            }

            // Ensure DataGridView is also explicitly disposed
            if (_dataGridView != null)
            {
                _dataGridView.Dispose();
                _dataGridView = null;
            }
        }

        [TestMethod]
        public void SearchDataGridView_SimpleStructuredQuery_FiltersCorrectly()
        {
            // Arrange - Setup a simple structured query that would be produced by the AI
            string structuredQuery = "+Country of origin:United States";

            // Act - Apply filtering through SearchDataGridView
            ApplySearchFilterToAllRows(structuredQuery);

            // Assert - Check only US rows are visible
            Assert.AreEqual(2, CountVisibleRows());
            foreach (DataGridViewRow row in _dataGridView.Rows)
            {
                if (row.Visible)
                {
                    Assert.AreEqual("United States", row.Cells["Country of origin"].Value);
                }
            }
        }

        [TestMethod]
        public void SearchDataGridView_ComplexStructuredQuery_FiltersCorrectly()
        {
            // Arrange - Setup a complex query with multiple conditions
            string structuredQuery = "+Country of origin:United States +Discount:>5";

            // Act - Apply filtering through SearchDataGridView
            ApplySearchFilterToAllRows(structuredQuery);

            // Assert - Check only rows matching both conditions are visible
            Assert.AreEqual(1, CountVisibleRows());
            foreach (DataGridViewRow row in _dataGridView.Rows)
            {
                if (row.Visible)
                {
                    Assert.AreEqual("United States", row.Cells["Country of origin"].Value);
                    Assert.IsTrue(Convert.ToDecimal(row.Cells["Discount"].Value) > 5);
                }
            }
        }

        [TestMethod]
        public void SearchDataGridView_OrCondition_FiltersCorrectly()
        {
            // Arrange - Setup a query with OR condition
            string structuredQuery = "+Country of origin:United States|United Kingdom";

            // Act - Apply filtering through SearchDataGridView
            ApplySearchFilterToAllRows(structuredQuery);

            // Assert - Check only US and UK rows are visible
            Assert.AreEqual(3, CountVisibleRows());
            foreach (DataGridViewRow row in _dataGridView.Rows)
            {
                if (row.Visible)
                {
                    Assert.IsTrue(
                        row.Cells["Country of origin"].Value.ToString() == "United States" ||
                        row.Cells["Country of origin"].Value.ToString() == "United Kingdom"
                    );
                }
            }
        }

        [TestMethod]
        public void SearchDataGridView_DateComparison_FiltersCorrectly()
        {
            // Arrange - Setup a query with date comparison
            string structuredQuery = "+Date:>2023-06-01";

            // Act - Apply filtering through SearchDataGridView
            ApplySearchFilterToAllRows(structuredQuery);

            // Assert - Check only rows with dates after June 1, 2023 are visible
            foreach (DataGridViewRow row in _dataGridView.Rows)
            {
                if (row.Visible)
                {
                    DateTime rowDate = DateTime.Parse(row.Cells["Date"].Value.ToString());
                    Assert.IsTrue(rowDate > new DateTime(2023, 6, 1));
                }
            }
        }

        [TestMethod]
        public void SearchDataGridView_ExclusionCondition_FiltersCorrectly()
        {
            // Arrange - Setup a query that excludes certain rows
            string structuredQuery = "-Country of origin:China";

            // Act - Apply filtering through SearchDataGridView
            ApplySearchFilterToAllRows(structuredQuery);

            // Assert - Check no China rows are visible
            foreach (DataGridViewRow row in _dataGridView.Rows)
            {
                if (row.Visible)
                {
                    Assert.AreNotEqual("China", row.Cells["Country of origin"].Value);
                }
            }
        }

        [TestMethod]
        public void SearchDataGridView_RegionalMapping_FiltersCorrectly()
        {
            // Arrange - Create a query that would result from a regional term like "Europe"
            string structuredQuery = "+Country of origin:United Kingdom|Germany|France";

            // Act - Apply filtering through SearchDataGridView
            ApplySearchFilterToAllRows(structuredQuery);

            // Assert - Check only European countries are visible
            Assert.AreEqual(2, CountVisibleRows());
            foreach (DataGridViewRow row in _dataGridView.Rows)
            {
                if (row.Visible)
                {
                    string country = row.Cells["Country of origin"].Value.ToString();
                    Assert.IsTrue(
                        country == "United Kingdom" ||
                        country == "Germany" ||
                        country == "France"
                    );
                }
            }
        }

        [TestMethod]
        public void SearchDataGridView_QualitativeTerms_FiltersCorrectly()
        {
            // Arrange - Create a query that would result from a term like "expensive purchases"
            string structuredQuery = "+Total:>200";

            // Act - Apply filtering through SearchDataGridView
            ApplySearchFilterToAllRows(structuredQuery);

            // Assert - Check only expensive items are visible
            foreach (DataGridViewRow row in _dataGridView.Rows)
            {
                if (row.Visible)
                {
                    decimal total = Convert.ToDecimal(row.Cells["Total"].Value);
                    Assert.IsTrue(total > 200);
                }
            }
        }

        // Helper methods
        private void AddTestDataToDataGridView()
        {
            // Add rows directly with values rather than using CreateRow
            _dataGridView.Rows.Add("1001", "Laptop", "United States", "TechCorp", "2023-04-15", 2, 10, 1500, "", "✗");
            _dataGridView.Rows.Add("1002", "Phone", "China", "MobileInc", "2023-05-20", 1, 5, 800, "", "✗");
            _dataGridView.Rows.Add("1003", "Tablet", "United Kingdom", "TabletCo", "2023-07-05", 3, 15, 1200, "", "✗");
            _dataGridView.Rows.Add("1004", "Camera", "Japan", "CameraCo", "2023-08-10", 1, 3, 600, "", "✗");
            _dataGridView.Rows.Add("1005", "Headphones", "United States", "AudioTech", "2023-02-25", 4, 2, 300, "", "✗");
            _dataGridView.Rows.Add("1006", "Keyboard", "Germany", "InputCo", "2023-09-30", 2, 8, 180, "", "✗");
        }
        private void ApplySearchFilterToAllRows(string searchQuery)
        {
            foreach (DataGridViewRow row in _dataGridView.Rows)
            {
                // Skip new/uncommitted rows and null rows
                if (row.IsNewRow || row.Cells[0].Value == null)
                {
                    continue;
                }

                bool shouldBeVisible = SearchDataGridView.FilterRowByAdvancedSearch(row, searchQuery);

                // Only update visibility if the row is already part of the grid
                if (row.DataGridView != null && !row.IsNewRow)
                {
                    row.Visible = shouldBeVisible;
                }
            }
        }
        private int CountVisibleRows()
        {
            int count = 0;
            foreach (DataGridViewRow row in _dataGridView.Rows)
            {
                if (row.Visible)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
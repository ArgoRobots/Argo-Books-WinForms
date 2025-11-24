using Argo_Books.UI;
using System.Drawing;
using System.Windows.Forms;

namespace Tests
{
    [TestClass]
    public class LabelManager_Tests
    {
        private Label label;
        private Control parentControl;
        private DataGridView dataGridView;

        [TestInitialize]
        public void Setup()
        {
            // Initialize test components
            label = new Label
            {
                Text = "Showing Results: ",
                AutoSize = true
            };

            parentControl = new Panel
            {
                Width = 500,
                Height = 400
            };
            parentControl.Controls.Add(label);

            dataGridView = new DataGridView
            {
                Width = 400,
                Height = 300,
                Location = new Point(50, 50)
            };

            // Add test columns to DataGridView
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TestColumn1",
                HeaderText = "Test Column 1"
            });
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TestColumn2",
                HeaderText = "Test Column 2"
            });
        }

        [TestMethod]
        public void TestShowShowingResultsLabel_TextFormatting()
        {
            // Arrange
            string initialText = "Showing Results: ";
            string newResults = "1-10 of 100";
            label.Text = initialText;

            // Act
            LabelManager.ShowLabelWithBaseText(label, newResults);

            // Assert
            Assert.AreEqual($"Showing Results: {newResults}", label.Text);
            Assert.IsTrue(label.Visible);
        }

        [TestMethod]
        public void TestShowShowingResultsLabel_Positioning()
        {
            // Arrange
            string results = "1-10 of 100";

            // Act
            LabelManager.ShowLabelWithBaseText(label, results);

            // Assert
            int expectedLeft = (parentControl.ClientSize.Width - label.Width) / 2;
            Assert.AreEqual(expectedLeft, label.Left);
        }

        [TestMethod]
        public void TestShowShowingResultsLabel_PreservesTranslatedText()
        {
            // Arrange
            string translatedPrefix = "Montrer les résultats";  // French example
            label.Text = $"{translatedPrefix}: ";
            string results = "1-10 of 100";

            // Act
            LabelManager.ShowLabelWithBaseText(label, results);

            // Assert
            Assert.AreEqual($"{translatedPrefix}: {results}", label.Text);
        }

        [TestMethod]
        public void TestShowTotalLabel_TextFormatting()
        {
            // Arrange
            string initialText = "Total Records: ";
            label.Text = initialText;

            // Add some rows to the DataGridView
            dataGridView.Rows.Add("Test 1", "Data 1");
            dataGridView.Rows.Add("Test 2", "Data 2");
            dataGridView.Rows.Add("Test 3", "Data 3");

            // Act
            LabelManager.ShowTotalLabel(label, dataGridView);

            // Assert
            Assert.AreEqual($"Total Records: {dataGridView.Rows.Count}", label.Text);
        }

        [TestMethod]
        public void TestShowTotalLabel_Positioning()
        {
            // Arrange
            label.Text = "Total Records: ";

            // Add a row to ensure proper sizing
            dataGridView.Rows.Add("Test", "Data");

            // Act
            LabelManager.ShowTotalLabel(label, dataGridView);

            // Assert
            Point expectedLocation = new(dataGridView.Right - label.Width, dataGridView.Bottom + 10);
            Assert.AreEqual(expectedLocation, label.Location);
        }

        [TestMethod]
        public void TestShowTotalLabel_PreservesTranslatedText()
        {
            // Arrange
            string translatedPrefix = "Total des enregistrements";  // French example
            label.Text = $"{translatedPrefix}: ";

            // Add some rows with data
            dataGridView.Rows.Add("Test 1", "Data 1");
            dataGridView.Rows.Add("Test 2", "Data 2");

            // Act
            LabelManager.ShowTotalLabel(label, dataGridView);

            // Assert
            Assert.AreEqual($"{translatedPrefix}: {dataGridView.Rows.Count}", label.Text);
        }

        [TestCleanup]
        public void Cleanup()
        {
            label.Dispose();
            parentControl.Dispose();
            dataGridView.Dispose();
        }
    }
}
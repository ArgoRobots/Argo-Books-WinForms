using Argo_Books.Classes;
using System.Drawing;
using System.Windows.Forms;

namespace Tests
{
    [TestClass]
    public class Tools_UnitTest
    {
        [TestMethod]
        public void TestFormatDateAndTime()
        {
            DateTime testDate = new(2024, 8, 10, 14, 30, 45);

            string formattedDate = Tools.FormatDate(testDate);
            string formattedTime = Tools.FormatTime(testDate);

            Assert.AreEqual("2024-08-10", formattedDate);
            Assert.AreEqual("14:30:45.00", formattedTime);
        }

        [TestMethod]
        public void TestAddNumberForAStringThatAlreadyExists()
        {
            List<string> existingNames = ["Item", "Item (2)", "Item (3)"];
            string newName = Tools.AddNumberForAStringThatAlreadyExists("Item", existingNames);
            Assert.AreEqual("Item (4)", newName);
        }

        [TestMethod]
        public void TestRemoveNumAfterString()
        {
            string name = "Item (3)";
            string result = Tools.RemoveNumAfterString(name);
            Assert.AreEqual("Item", result);

            string nameWithoutNumber = "Item";
            string resultWithoutNumber = Tools.RemoveNumAfterString(nameWithoutNumber);
            Assert.AreEqual("Item", resultWithoutNumber);
        }

        [TestMethod]
        public void TestIsFormOpen()
        {
            Form testForm = new();
            testForm.Show();

            bool isOpen = Tools.IsFormOpen<Form>();
            Assert.IsTrue(isOpen);

            testForm.Close();
            isOpen = Tools.IsFormOpen<Form>();
            Assert.IsFalse(isOpen);
        }

        [TestMethod]
        public void TestFormatDateTime()
        {
            DateTime testDate = new(2024, 8, 10, 14, 30, 45);
            string formattedDateTime = Tools.FormatDateTime(testDate);
            Assert.AreEqual("2024-08-10 14:30:45.00", formattedDateTime);
        }

        [TestMethod]
        public void TestFormatDuration()
        {
            // Test milliseconds format
            long smallDuration = 500;
            string smallResult = Tools.FormatDuration(smallDuration);
            Assert.AreEqual("500ms", smallResult);

            // Test seconds format
            long mediumDuration = 2500;
            string mediumResult = Tools.FormatDuration(mediumDuration);
            Assert.AreEqual("2.50s", mediumResult);

            // Test minutes and seconds format
            long longDuration = 125000;  // 2 minutes and 5 seconds
            string longResult = Tools.FormatDuration(longDuration);
            Assert.AreEqual("2m 5s", longResult);
        }

        [TestMethod]
        public void TestFormatDurationHours()
        {
            long duration = 3725000;  // 1 hour, 2 minutes and 5 seconds
            string result = Tools.FormatDuration(duration);
            Assert.AreEqual("1h 2m 5s", result);
        }

        [TestMethod]
        public void TestConvertBytesToReadableSize()
        {
            // Test bytes
            long bytes = 500;
            string bytesResult = Tools.ConvertBytesToReadableSize(bytes);
            Assert.AreEqual("500 Bytes", bytesResult);

            // Test kilobytes
            long kilobytes = 1500;
            string kbResult = Tools.ConvertBytesToReadableSize(kilobytes);
            Assert.AreEqual("1.46 KB", kbResult);

            // Test megabytes
            long megabytes = 1500000;
            string mbResult = Tools.ConvertBytesToReadableSize(megabytes);
            Assert.AreEqual("1.43 MB", mbResult);

            // Test gigabytes
            long gigabytes = 1500000000;
            string gbResult = Tools.ConvertBytesToReadableSize(gigabytes);
            Assert.AreEqual("1.4 GB", gbResult);
        }

        [TestMethod]
        public void TestAddEllipsisToString()
        {
            // Create a test string and font
            string longText = "This is a very long text that should be truncated with ellipsis";
            Font testFont = new("Arial", 10);

            // Test with sufficient width (should return original text)
            int largeWidth = 500;
            string resultWithLargeWidth = Tools.AddEllipsisToString(longText, testFont, largeWidth);
            Assert.AreEqual(longText, resultWithLargeWidth);

            // Test with limited width (should return truncated text)
            int smallWidth = 50;
            string resultWithSmallWidth = Tools.AddEllipsisToString(longText, testFont, smallWidth);
            Assert.IsTrue(resultWithSmallWidth.Length < longText.Length);
            Assert.IsTrue(resultWithSmallWidth.EndsWith("..."));
        }

        [TestMethod]
        public void TestGetVersionNumber()
        {
            string version = Tools.GetVersionNumber();

            // Check that the version follows the expected format (e.g., x.x.x.x)
            Assert.IsTrue(version.Count(c => c == '.') >= 2, "Version should have at least 3 segments");

            // Ensure each segment can be parsed as an integer
            string[] segments = version.Split('.');
            foreach (string segment in segments)
            {
                Assert.IsTrue(int.TryParse(segment, out _), $"Version segment '{segment}' should be a number");
            }
        }

        [TestMethod]
        public void TestRemoveNumAfterStringEdgeCases()
        {
            // Test with no number in parentheses
            string nameWithoutNumber = "Item";
            string resultWithoutNumber = Tools.RemoveNumAfterString(nameWithoutNumber);
            Assert.AreEqual("Item", resultWithoutNumber);

            // Test with text in parentheses (not a number)
            string nameWithText = "Item (text)";
            string resultWithText = Tools.RemoveNumAfterString(nameWithText);
            Assert.AreEqual("Item (text)", resultWithText);

            // Test with multiple parentheses
            string nameWithMultipleParentheses = "Item (1) (2)";
            string resultWithMultipleParentheses = Tools.RemoveNumAfterString(nameWithMultipleParentheses);
            Assert.AreEqual("Item (1)", resultWithMultipleParentheses);
        }

        [TestMethod]
        public void TestAddNumberForAStringThatAlreadyExistsWithEmptyList()
        {
            // Test with empty list
            List<string> emptyList = [];
            string result = Tools.AddNumberForAStringThatAlreadyExists("Item", emptyList);
            Assert.AreEqual("Item (2)", result);
        }

        [TestMethod]
        public void TestAddNumberForAStringThatAlreadyExistsWithContinuousNumbering()
        {
            // Test with a list that has entries with consecutive numbers
            List<string> list = ["Item", "Item (2)", "Item (3)", "Item (4)"];
            string result = Tools.AddNumberForAStringThatAlreadyExists("Item", list);
            Assert.AreEqual("Item (5)", result);
        }

        [TestMethod]
        public void TestAddNumberForAStringThatAlreadyExistsWithGaps()
        {
            // Test with a list that has entries with gaps in numbering
            List<string> list = ["Item", "Item (2)", "Item (4)"];  // Missing "Item (3)"
            string result = Tools.AddNumberForAStringThatAlreadyExists("Item", list);
            Assert.AreEqual("Item (3)", result);  // Should find and use the first available number
        }

        [TestMethod]
        public void TestCloseOpenForm()
        {
            // Create and show a test form
            Form testForm = new();
            testForm.Show();

            // Verify the form is open
            Assert.IsTrue(Application.OpenForms.Cast<Form>().Any(f => f.GetType() == typeof(Form)));

            // Close the form
            Tools.CloseOpenForm<Form>();

            // Verify the form is closed
            Assert.IsFalse(Application.OpenForms.Cast<Form>().Any(f => f.GetType() == typeof(Form)));
        }

        [TestMethod]
        public void TestAddEllipsisToStringEmptyText()
        {
            // Test with empty string
            string emptyText = "";
            Font testFont = new("Arial", 10);
            int width = 100;

            string result = Tools.AddEllipsisToString(emptyText, testFont, width);
            Assert.AreEqual("", result, "Empty string should remain empty");
        }

        [TestMethod]
        public void TestAddEllipsisToStringShortText()
        {
            // Test with text that's already shorter than the available width
            string shortText = "Short";
            Font testFont = new("Arial", 10);
            int width = 200;

            string result = Tools.AddEllipsisToString(shortText, testFont, width);
            Assert.AreEqual(shortText, result, "Short text should remain unchanged");
        }

        [TestMethod]
        public void TestFormatDateTimeNull()
        {
            try
            {
                // Test with the default value of DateTime (should not throw an exception)
                DateTime defaultDate = default;
                string result = Tools.FormatDateTime(defaultDate);
                Assert.AreEqual("0001-01-01 00:00:00.00", result);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Should not throw an exception for default DateTime: {ex.Message}");
            }
        }

        [TestMethod]
        public void TestConvertBytesToReadableSizeZeroBytes()
        {
            // Test with zero bytes
            long bytes = 0;
            string result = Tools.ConvertBytesToReadableSize(bytes);
            Assert.AreEqual("0 Bytes", result);
        }

        [TestMethod]
        public void TestConvertBytesToReadableSizeTerabytes()
        {
            // Test with a value that should be displayed in TB
            long terabytes = 2L * 1024 * 1024 * 1024 * 1024;  // 2 TB
            string result = Tools.ConvertBytesToReadableSize(terabytes);
            Assert.AreEqual("2 TB", result);
        }
    }
}

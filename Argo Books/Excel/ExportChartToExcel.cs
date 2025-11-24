using Argo_Books.AnonymousData;
using Argo_Books.Classes;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using Argo_Books.Language;
using System.Diagnostics;

namespace Argo_Books.Excel
{
    /// <summary>
    /// Exports chart data and visualizations to Excel files with various chart types and formatting options.
    /// </summary>
    internal class ExportChartToExcel
    {
        // Constants
        private const string _currencyFormatPattern = "\"$\"#,##0.00";
        private const string _numberFormatPattern = "0";

        // Public methods
        public static void ExportChart(Dictionary<string, double> data, string filePath, eChartType chartType, string chartTitle, string column1Text, string column2Text, bool isSpline = false)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ExcelPackage.License.SetNonCommercialPersonal("Argo");
            using ExcelPackage package = new();
            string worksheetName = LanguageManager.TranslateString("Chart Data");
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetName);

            // Add headers
            worksheet.Cells["A1"].Value = LanguageManager.TranslateString(column1Text);
            worksheet.Cells["B1"].Value = LanguageManager.TranslateString(column2Text);

            // Format headers
            ExcelRange headerRange = worksheet.Cells["A1:B1"];
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);

            // Add data
            int row = 2;
            foreach (KeyValuePair<string, double> item in data.OrderBy(x => x.Key))
            {
                worksheet.Cells[row, 1].Value = item.Key;
                worksheet.Cells[row, 2].Value = item.Value;
                worksheet.Cells[row, 2].Style.Numberformat.Format = _currencyFormatPattern;
                row++;
            }

            ExcelChart chart = CreateChart(worksheet, chartTitle, chartType, false);

            // Configure chart
            ExcelChartSerie series = chart.Series.Add(worksheet.Cells[$"B2:B{row - 1}"], worksheet.Cells[$"A2:A{row - 1}"]);
            if (isSpline && chartType == eChartType.Line)
            {
                ((ExcelLineChartSerie)series).Smooth = true;
            }
            chart.Legend.Remove();

            worksheet.Columns[1, 2].AutoFit();
            package.SaveAs(new FileInfo(filePath));

            TrackChartExport(stopwatch, filePath);
        }
        public static void ExportCountChart(Dictionary<string, int> data, string filePath, eChartType chartType, string chartTitle, string column1Header, string column2Header)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            // Convert int data to double for existing export logic
            Dictionary<string, double> doubleData = data.ToDictionary(kvp => kvp.Key, kvp => (double)kvp.Value);

            ExcelPackage.License.SetNonCommercialPersonal("Argo");
            using ExcelPackage package = new();
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Chart Data");

            // Set headers
            worksheet.Cells[1, 1].Value = column1Header;
            worksheet.Cells[1, 2].Value = column2Header;

            // Format headers
            using (ExcelRange headerRange = worksheet.Cells[1, 1, 1, 2])
            {
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);
            }

            // Add data
            int row = 2;
            foreach (KeyValuePair<string, double> kvp in doubleData.OrderBy(x => x.Key))
            {
                worksheet.Cells[row, 1].Value = kvp.Key;
                worksheet.Cells[row, 2].Value = kvp.Value;

                // Format as whole numbers (no decimals, no currency)
                worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";

                row++;
            }

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // Add chart
            ExcelChart chart = worksheet.Drawings.AddChart(chartTitle, chartType);
            chart.Title.Text = chartTitle;
            chart.SetPosition(0, 0, 3, 0);
            chart.SetSize(600, 400);

            // Set chart data range
            chart.Series.Add(worksheet.Cells[2, 2, row - 1, 2], worksheet.Cells[2, 1, row - 1, 1]);
            chart.Series[0].Header = column2Header;

            // Save the file
            FileInfo fileInfo = new(filePath);
            package.SaveAs(fileInfo);

            TrackChartExport(stopwatch, filePath);
        }
        public static void ExportMultiDataSetChart(Dictionary<string, Dictionary<string, double>> data, string filePath, eChartType chartType, string chartTitle, bool isSpline = false)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ExcelPackage.License.SetNonCommercialPersonal("Argo");
            using ExcelPackage package = new();
            string worksheetName = LanguageManager.TranslateString("Chart Data");
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetName);

            // Get all series names
            List<string> seriesNames = data.First().Value.Keys.ToList();

            // Add headers
            worksheet.Cells[1, 1].Value = LanguageManager.TranslateString("Date");
            for (int i = 0; i < seriesNames.Count; i++)
            {
                worksheet.Cells[1, i + 2].Value = seriesNames[i];
            }

            // Format headers
            ExcelRange headerRange = worksheet.Cells[1, 1, 1, seriesNames.Count + 1];
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);

            // Add data
            int row = 2;
            foreach (KeyValuePair<string, Dictionary<string, double>> dateEntry in data.OrderBy(x => x.Key))
            {
                worksheet.Cells[row, 1].Value = dateEntry.Key;

                for (int i = 0; i < seriesNames.Count; i++)
                {
                    worksheet.Cells[row, i + 2].Value = dateEntry.Value[seriesNames[i]];

                    // Use currency formatting for money values
                    worksheet.Cells[row, i + 2].Style.Numberformat.Format = _currencyFormatPattern;
                }
                row++;
            }

            // Create chart
            ExcelChart chart = CreateChart(worksheet, chartTitle, chartType, true);
            chart.Legend.Position = eLegendPosition.Top;

            // Add series to chart
            for (int i = 0; i < seriesNames.Count; i++)
            {
                ExcelChartSerie series = chart.Series.Add(
                    worksheet.Cells[2, i + 2, row - 1, i + 2], // Y values
                    worksheet.Cells[2, 1, row - 1, 1]          // X values
                );
                if (isSpline && chartType == eChartType.Line)
                {
                    ((ExcelLineChartSerie)series).Smooth = true;
                }
                series.Header = seriesNames[i];
            }

            worksheet.Columns.AutoFit();
            package.SaveAs(new FileInfo(filePath));

            TrackChartExport(stopwatch, filePath);
        }
        public static void ExportMultiDataSetCountChart(Dictionary<string, Dictionary<string, double>> data, string filePath, eChartType chartType, string chartTitle)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ExcelPackage.License.SetNonCommercialPersonal("Argo");
            using ExcelPackage package = new();
            string worksheetName = LanguageManager.TranslateString("Chart Data");
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetName);

            // Get all series names
            List<string> seriesNames = data.First().Value.Keys.ToList();

            // Add headers
            worksheet.Cells[1, 1].Value = LanguageManager.TranslateString("Date");
            for (int i = 0; i < seriesNames.Count; i++)
            {
                worksheet.Cells[1, i + 2].Value = seriesNames[i];
            }

            // Format headers
            ExcelRange headerRange = worksheet.Cells[1, 1, 1, seriesNames.Count + 1];
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);

            // Add data
            int row = 2;
            foreach (KeyValuePair<string, Dictionary<string, double>> dateEntry in data.OrderBy(x => x.Key))
            {
                worksheet.Cells[row, 1].Value = dateEntry.Key;

                for (int i = 0; i < seriesNames.Count; i++)
                {
                    worksheet.Cells[row, i + 2].Value = dateEntry.Value[seriesNames[i]];

                    // Use number formatting for counts (no decimals, no currency)
                    worksheet.Cells[row, i + 2].Style.Numberformat.Format = _numberFormatPattern;
                }
                row++;
            }

            // Create chart
            ExcelChart chart = CreateChart(worksheet, chartTitle, chartType, true);
            chart.Legend.Position = eLegendPosition.Top;

            // Add series to chart
            for (int i = 0; i < seriesNames.Count; i++)
            {
                ExcelChartSerie series = chart.Series.Add(
                    worksheet.Cells[2, i + 2, row - 1, i + 2], // Y values
                    worksheet.Cells[2, 1, row - 1, 1]          // X values
                );
                series.Header = seriesNames[i];
            }

            worksheet.Columns.AutoFit();
            package.SaveAs(new FileInfo(filePath));

            TrackChartExport(stopwatch, filePath);
        }

        // Private method
        /// <summary>
        /// Tracks chart export performance metrics including duration, file size, and export type.
        /// </summary>
        private static void TrackChartExport(Stopwatch stopwatch, string filePath)
        {
            stopwatch.Stop();
            string readableSize = "0 Bytes";

            if (File.Exists(filePath))
            {
                FileInfo fileInfo = new(filePath);
                long fileSizeBytes = fileInfo.Length;
                readableSize = Tools.ConvertBytesToReadableSize(fileSizeBytes);
            }

            Dictionary<ExportDataField, object> exportData = new()
            {
                { ExportDataField.ExportType, ExportType.ExcelSheetsChart },
                { ExportDataField.DurationMS, stopwatch.ElapsedMilliseconds },
                { ExportDataField.FileSize, readableSize }
            };

            AnonymousDataManager.AddExportData(exportData);
        }

        /// <summary>
        /// Creates and configures an Excel chart with default position and size.
        /// </summary>
        /// <returns>The created Excel chart.</returns>
        private static ExcelChart CreateChart(ExcelWorksheet worksheet, string chartTitle, eChartType chartType, bool isMultiDataset)
        {
            ExcelChart chart = worksheet.Drawings.AddChart(chartTitle, chartType);
            chart.SetPosition(0, 0, isMultiDataset ? 4 : 3, 0);
            chart.SetSize(800, 400);
            chart.Title.Text = chartTitle;

            return chart;
        }
    }
}
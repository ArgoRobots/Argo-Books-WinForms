using Argo_Books.AnonymousData;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Argo_Books;
using Argo_Books.Language;
using System.Diagnostics;

namespace Argo_Books.Classes
{
    /// <summary>
    /// Handles the export of chart data to Google Sheets, including creating new spreadsheets and formatting cells.
    /// </summary>
    public class GoogleSheetManager
    {
        // Properties
        private static SheetsService? _sheetsService;
        public enum ChartType
        {
            Line,
            Spline,
            Column,
            Pie
        }

        // Initialize the Google Sheets service
        private static async Task<bool> InitializeServiceAsync()
        {
            if (_sheetsService != null) { return true; }

            try
            {
                UserCredential credential = await GoogleCredentialsManager.GetUserCredentialAsync();

                _sheetsService = new SheetsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Argo Books"
                });

                return true;
            }
            catch
            {
                MainMenu_Form.Instance.InvokeIfRequired(() =>
                    CustomMessageBox.ShowWithFormat("Service Initialization Error",
                    "Failed to initialize Google Sheets service.",
                    CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok)
                );
                return false;
            }
        }

        // Export single dataset chart
        public static async Task ExportChartToGoogleSheetsAsync(
            IReadOnlyDictionary<string, double> data,
            string chartTitle,
            ChartType chartType,
            string column1Text,
            string column2Text,
            CancellationToken cancellationToken = default)
        {
            if (!await InitializeServiceAsync()) { return; }

            Stopwatch stopwatch = Stopwatch.StartNew();
            string operationMessage = "Exporting chart to Google Sheets...";

            // Create a new cancellation token source that combines with the provided token
            using CancellationTokenSource combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            CancellationToken activeCancellationToken = combinedCts.Token;

            Loading_Form.ShowLoading(operationMessage, combinedCts);

            try
            {
                activeCancellationToken.ThrowIfCancellationRequested();

                // Create a new spreadsheet
                Spreadsheet spreadsheet = new()
                {
                    Properties = new SpreadsheetProperties
                    {
                        Title = $"{Directories.CompanyName} - {chartTitle} - {Tools.FormatDate(DateTime.Today)}"
                    },
                    Sheets = [
                        new Sheet
                        {
                            Properties = new SheetProperties
                            {
                                Title = LanguageManager.TranslateString("Chart Data"),
                                SheetId = 0
                            }
                        }
                    ]
                };

                activeCancellationToken.ThrowIfCancellationRequested();

                spreadsheet = await _sheetsService.Spreadsheets
                    .Create(spreadsheet)
                    .ExecuteAsync(activeCancellationToken);

                string spreadsheetId = spreadsheet.SpreadsheetId;
                string sheetName = LanguageManager.TranslateString("Chart Data");

                activeCancellationToken.ThrowIfCancellationRequested();

                // Prepare the data
                List<IList<object>> values =
                [
                    [
                        LanguageManager.TranslateString(column1Text),
                        LanguageManager.TranslateString(column2Text)
                    ]
                ];

                foreach (KeyValuePair<string, double> item in data.OrderBy(x => x.Key))
                {
                    activeCancellationToken.ThrowIfCancellationRequested();
                    values.Add([item.Key, item.Value]);
                }

                activeCancellationToken.ThrowIfCancellationRequested();

                // Write data to sheet
                string range = $"'{sheetName}'!A1:B{values.Count}";
                ValueRange valueRange = new() { Values = values };

                SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest =
                    _sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

                await updateRequest.ExecuteAsync(activeCancellationToken);

                activeCancellationToken.ThrowIfCancellationRequested();

                // Format headers
                List<Request> requests =
                [
                    CreateHeaderFormatRequest(0, 0, 0, 1),
                    CreateNumberFormatRequest(1, values.Count - 1, 1, 1, "#,##0.00")
                ];

                // Add chart
                Request chartRequest = CreateChartRequest(
                    chartType,
                    chartTitle,
                    0, values.Count - 1,
                    [("A", "B")]
                );
                requests.Add(chartRequest);

                activeCancellationToken.ThrowIfCancellationRequested();

                // Execute all formatting requests
                BatchUpdateSpreadsheetRequest batchUpdateRequest = new()
                {
                    Requests = requests
                };

                await _sheetsService.Spreadsheets
                    .BatchUpdate(batchUpdateRequest, spreadsheetId)
                    .ExecuteAsync(activeCancellationToken);

                activeCancellationToken.ThrowIfCancellationRequested();

                // Auto-resize columns
                DimensionRange dimensionRange = new()
                {
                    SheetId = 0,
                    Dimension = "COLUMNS",
                    StartIndex = 0,
                    EndIndex = 2
                };

                Request autoResizeRequest = new()
                {
                    AutoResizeDimensions = new AutoResizeDimensionsRequest
                    {
                        Dimensions = dimensionRange
                    }
                };

                await _sheetsService.Spreadsheets
                    .BatchUpdate(new BatchUpdateSpreadsheetRequest
                    {
                        Requests = [autoResizeRequest]
                    }, spreadsheetId)
                    .ExecuteAsync(activeCancellationToken);

                activeCancellationToken.ThrowIfCancellationRequested();

                OpenGoogleSheet(spreadsheetId);
                TrackGoogleSheetsExport(stopwatch);
            }
            catch (OperationCanceledException)
            {
                // Don't show error message for cancellation
                throw;
            }
            catch
            {
                MainMenu_Form.Instance.InvokeIfRequired(() =>
                    CustomMessageBox.ShowWithFormat("Export Error", "Failed to export chart to Google Sheets.",
                    CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok)
                );
                throw;
            }
            finally
            {
                Loading_Form.CompleteOperation(operationMessage);
            }
        }

        // Export single dataset chart with integer as value
        public static async Task ExportCountChartToGoogleSheetsAsync(
            IReadOnlyDictionary<string, int> data,
            string chartTitle,
            ChartType chartType,
            string column1Text,
            string column2Text,
            CancellationToken cancellationToken = default)
        {
            // Convert int data to double for existing export logic
            Dictionary<string, double> doubleData = data.ToDictionary(kvp => kvp.Key, kvp => (double)kvp.Value);

            if (!await InitializeServiceAsync()) { return; }

            Stopwatch stopwatch = Stopwatch.StartNew();
            string operationMessage = "Exporting chart to Google Sheets...";

            using CancellationTokenSource combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            CancellationToken activeCancellationToken = combinedCts.Token;

            Loading_Form.ShowLoading(operationMessage, combinedCts);

            try
            {
                activeCancellationToken.ThrowIfCancellationRequested();

                // Create a new spreadsheet
                Spreadsheet spreadsheet = new()
                {
                    Properties = new SpreadsheetProperties
                    {
                        Title = $"{Directories.CompanyName} - {chartTitle} - {Tools.FormatDate(DateTime.Today)}"
                    },
                    Sheets = [
                        new Sheet
                        {
                            Properties = new SheetProperties
                            {
                                Title = LanguageManager.TranslateString("Chart Data"),
                                SheetId = 0
                            }
                        }
                    ]
                };

                activeCancellationToken.ThrowIfCancellationRequested();

                spreadsheet = await _sheetsService.Spreadsheets
                    .Create(spreadsheet)
                    .ExecuteAsync(activeCancellationToken);

                string spreadsheetId = spreadsheet.SpreadsheetId;
                string sheetName = LanguageManager.TranslateString("Chart Data");

                activeCancellationToken.ThrowIfCancellationRequested();

                // Prepare the data
                List<IList<object>> values =
                [
                    [
                        LanguageManager.TranslateString(column1Text),
                        LanguageManager.TranslateString(column2Text)
                    ]
                ];

                foreach (KeyValuePair<string, int> item in data.OrderBy(x => x.Key))
                {
                    activeCancellationToken.ThrowIfCancellationRequested();
                    values.Add([item.Key, item.Value]);
                }

                activeCancellationToken.ThrowIfCancellationRequested();

                // Write data to sheet
                string range = $"'{sheetName}'!A1:B{values.Count}";
                ValueRange valueRange = new() { Values = values };

                SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest =
                    _sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

                await updateRequest.ExecuteAsync(activeCancellationToken);

                activeCancellationToken.ThrowIfCancellationRequested();

                // Format headers - use number format for counts, not currency
                List<Request> requests =
                [
                    CreateHeaderFormatRequest(0, 0, 0, 1),
                    CreateNumberFormatRequest(1, values.Count - 1, 1, 1, "#,##0") // No decimal places for counts
                ];

                // Add chart
                Request chartRequest = CreateChartRequest(
                    chartType,
                    chartTitle,
                    0, values.Count - 1,
                    [("A", "B")]
                );
                requests.Add(chartRequest);

                activeCancellationToken.ThrowIfCancellationRequested();

                // Execute all formatting requests
                BatchUpdateSpreadsheetRequest batchUpdateRequest = new()
                {
                    Requests = requests
                };

                await _sheetsService.Spreadsheets
                    .BatchUpdate(batchUpdateRequest, spreadsheetId)
                    .ExecuteAsync(activeCancellationToken);

                activeCancellationToken.ThrowIfCancellationRequested();

                // Auto-resize columns
                DimensionRange dimensionRange = new()
                {
                    SheetId = 0,
                    Dimension = "COLUMNS",
                    StartIndex = 0,
                    EndIndex = 2
                };

                Request autoResizeRequest = new()
                {
                    AutoResizeDimensions = new AutoResizeDimensionsRequest
                    {
                        Dimensions = dimensionRange
                    }
                };

                await _sheetsService.Spreadsheets
                    .BatchUpdate(new BatchUpdateSpreadsheetRequest
                    {
                        Requests = [autoResizeRequest]
                    }, spreadsheetId)
                    .ExecuteAsync(activeCancellationToken);

                activeCancellationToken.ThrowIfCancellationRequested();

                OpenGoogleSheet(spreadsheetId);
                TrackGoogleSheetsExport(stopwatch);
            }
            catch (OperationCanceledException)
            {
                // Don't show error message for cancellation
                throw;
            }
            catch
            {
                MainMenu_Form.Instance.InvokeIfRequired(() =>
                    CustomMessageBox.ShowWithFormat("Export Error", "Failed to export chart to Google Sheets.",
                    CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok)
                );
                throw;
            }
            finally
            {
                Loading_Form.CompleteOperation(operationMessage);
            }
        }

        // Export multiple dataset chart
        public static async Task ExportMultiDataSetChartToGoogleSheetsAsync(
            Dictionary<string, Dictionary<string, double>> data,
            string chartTitle,
            ChartType chartType,
            CancellationToken cancellationToken = default)
        {
            if (!await InitializeServiceAsync()) { return; }

            Stopwatch stopwatch = Stopwatch.StartNew();
            string operationMessage = LanguageManager.TranslateString("Exporting multi-dataset chart to Google Sheets...");

            // Create a new cancellation token source that combines with the provided token
            using CancellationTokenSource combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            CancellationToken activeCancellationToken = combinedCts.Token;

            Loading_Form.ShowLoading(operationMessage, combinedCts);

            try
            {
                activeCancellationToken.ThrowIfCancellationRequested();

                string sheetName = LanguageManager.TranslateString("Chart Data");

                // Create a new spreadsheet
                Spreadsheet spreadsheet = new()
                {
                    Properties = new SpreadsheetProperties
                    {
                        Title = $"{Directories.CompanyName} - {chartTitle} - {Tools.FormatDate(DateTime.Today)}"
                    },
                    Sheets = [
                        new Sheet
                        {
                            Properties = new SheetProperties
                            {
                                Title = sheetName,
                                SheetId = 0
                            }
                        }
                    ]
                };

                activeCancellationToken.ThrowIfCancellationRequested();

                spreadsheet = await _sheetsService.Spreadsheets
                    .Create(spreadsheet)
                    .ExecuteAsync(activeCancellationToken);

                string spreadsheetId = spreadsheet.SpreadsheetId;

                activeCancellationToken.ThrowIfCancellationRequested();

                // Create drive service
                DriveService driveService = new(new BaseClientService.Initializer
                {
                    HttpClientInitializer = _sheetsService.HttpClientInitializer,
                    ApplicationName = "Argo Books"
                });

                // Set file permissions to be accessible by anyone with the link
                Permission permission = new()
                {
                    Type = "anyone",
                    Role = "writer",
                    AllowFileDiscovery = false
                };

                await driveService.Permissions
                    .Create(permission, spreadsheetId)
                    .ExecuteAsync(activeCancellationToken);

                activeCancellationToken.ThrowIfCancellationRequested();

                // Get series names and prepare headers
                List<string> seriesNames = data.First().Value.Keys.ToList();
                List<string> orderedSeriesNames = seriesNames
                    .OrderBy(x => x.Contains("Sales"))  // This puts "Total Sales" last
                    .ToList();
                List<object> headers = [LanguageManager.TranslateString("Date"), .. orderedSeriesNames];

                // Prepare the data
                List<IList<object>> values = [headers];

                foreach (KeyValuePair<string, Dictionary<string, double>> dateEntry in data.OrderBy(x => x.Key))
                {
                    activeCancellationToken.ThrowIfCancellationRequested();

                    List<object> row = [dateEntry.Key];
                    foreach (string seriesName in orderedSeriesNames)
                    {
                        row.Add(dateEntry.Value[seriesName]);
                    }
                    values.Add(row);
                }

                activeCancellationToken.ThrowIfCancellationRequested();

                // Write data to sheet
                string range = $"{sheetName}!A1:{(char)('A' + seriesNames.Count)}{values.Count}";
                ValueRange valueRange = new() { Values = values };

                SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                await updateRequest.ExecuteAsync(activeCancellationToken);

                activeCancellationToken.ThrowIfCancellationRequested();

                // Format headers and numbers
                List<Request> requests =
                [
                    CreateHeaderFormatRequest(0, 0, 0, seriesNames.Count),
                ];

                // Format number columns
                for (int i = 1; i <= seriesNames.Count; i++)
                {
                    activeCancellationToken.ThrowIfCancellationRequested();
                    requests.Add(CreateNumberFormatRequest(1, values.Count - 1, i, i, "#,##0.00"));
                }

                // Add chart with multiple series
                List<(string, string)> seriesRanges = [];
                for (int i = 0; i < seriesNames.Count; i++)
                {
                    seriesRanges.Add(($"A", $"{(char)('B' + i)}"));
                }

                Request chartRequest = CreateChartRequest(
                    chartType,
                    chartTitle,
                    0,
                    values.Count - 1,
                    seriesRanges.ToArray()
                );
                requests.Add(chartRequest);

                activeCancellationToken.ThrowIfCancellationRequested();

                // Execute all formatting requests
                BatchUpdateSpreadsheetRequest batchUpdateRequest = new()
                {
                    Requests = requests
                };

                await _sheetsService.Spreadsheets
                    .BatchUpdate(batchUpdateRequest, spreadsheetId)
                    .ExecuteAsync(activeCancellationToken);

                activeCancellationToken.ThrowIfCancellationRequested();

                // Auto-resize columns
                DimensionRange dimensionRange = new()
                {
                    SheetId = 0,
                    Dimension = "COLUMNS",
                    StartIndex = 0,
                    EndIndex = seriesNames.Count + 1
                };

                Request autoResizeRequest = new()
                {
                    AutoResizeDimensions = new AutoResizeDimensionsRequest
                    {
                        Dimensions = dimensionRange
                    }
                };

                await _sheetsService.Spreadsheets
                    .BatchUpdate(new BatchUpdateSpreadsheetRequest
                    {
                        Requests = [autoResizeRequest]
                    }, spreadsheetId)
                    .ExecuteAsync(activeCancellationToken);

                activeCancellationToken.ThrowIfCancellationRequested();

                OpenGoogleSheet(spreadsheetId);
                TrackGoogleSheetsExport(stopwatch);
            }
            catch (OperationCanceledException)
            {
                // Don't show error message for cancellation
                throw;
            }
            catch
            {
                MainMenu_Form.Instance.InvokeIfRequired(() =>
                    CustomMessageBox.ShowWithFormat("Export Error", "Failed to export multi-dataset chart to Google Sheets.",
                    CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok)
                );
                throw;
            }
            finally
            {
                Loading_Form.CompleteOperation(operationMessage);
            }
        }

        // Export multiple dataset chart with integer as value
        public static async Task ExportMultiDataSetCountChartToGoogleSheetsAsync(
            Dictionary<string, Dictionary<string, double>> data,
            string chartTitle,
            ChartType chartType,
            CancellationToken cancellationToken = default)
        {
            if (!await InitializeServiceAsync()) { return; }

            Stopwatch stopwatch = Stopwatch.StartNew();
            string operationMessage = LanguageManager.TranslateString("Exporting multi-dataset chart to Google Sheets...");

            using CancellationTokenSource combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            CancellationToken activeCancellationToken = combinedCts.Token;

            Loading_Form.ShowLoading(operationMessage, combinedCts);

            try
            {
                activeCancellationToken.ThrowIfCancellationRequested();

                string sheetName = LanguageManager.TranslateString("Chart Data");

                // Create a new spreadsheet
                Spreadsheet spreadsheet = new()
                {
                    Properties = new SpreadsheetProperties
                    {
                        Title = $"{Directories.CompanyName} - {chartTitle} - {Tools.FormatDate(DateTime.Today)}"
                    },
                    Sheets = [
                        new Sheet
                        {
                            Properties = new SheetProperties
                            {
                                Title = sheetName,
                                SheetId = 0
                            }
                        }
                    ]
                };

                activeCancellationToken.ThrowIfCancellationRequested();

                spreadsheet = await _sheetsService.Spreadsheets
                    .Create(spreadsheet)
                    .ExecuteAsync(activeCancellationToken);

                string spreadsheetId = spreadsheet.SpreadsheetId;

                activeCancellationToken.ThrowIfCancellationRequested();

                // Create drive service
                DriveService driveService = new(new BaseClientService.Initializer
                {
                    HttpClientInitializer = _sheetsService.HttpClientInitializer,
                    ApplicationName = "Argo Books"
                });

                // Set file permissions to be accessible by anyone with the link
                Permission permission = new()
                {
                    Type = "anyone",
                    Role = "writer",
                    AllowFileDiscovery = false
                };

                await driveService.Permissions
                    .Create(permission, spreadsheetId)
                    .ExecuteAsync(activeCancellationToken);

                activeCancellationToken.ThrowIfCancellationRequested();

                // Get series names and prepare headers
                List<string> seriesNames = data.First().Value.Keys.ToList();
                List<string> orderedSeriesNames = seriesNames
                    .OrderBy(x => x.Contains("Sales"))  // This puts "Total Sales" last
                    .ToList();
                List<object> headers = [LanguageManager.TranslateString("Date"), .. orderedSeriesNames];

                // Prepare the data
                List<IList<object>> values = [headers];

                foreach (KeyValuePair<string, Dictionary<string, double>> dateEntry in data.OrderBy(x => x.Key))
                {
                    activeCancellationToken.ThrowIfCancellationRequested();

                    List<object> row = [dateEntry.Key];
                    foreach (string seriesName in orderedSeriesNames)
                    {
                        row.Add(dateEntry.Value[seriesName]);
                    }
                    values.Add(row);
                }

                activeCancellationToken.ThrowIfCancellationRequested();

                // Write data to sheet
                string range = $"{sheetName}!A1:{(char)('A' + seriesNames.Count)}{values.Count}";
                ValueRange valueRange = new() { Values = values };

                SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                await updateRequest.ExecuteAsync(activeCancellationToken);

                activeCancellationToken.ThrowIfCancellationRequested();

                // Format headers and numbers
                List<Request> requests =
                [
                    CreateHeaderFormatRequest(0, 0, 0, seriesNames.Count),
                ];

                // Format number columns (no decimals for counts)
                for (int i = 1; i <= seriesNames.Count; i++)
                {
                    activeCancellationToken.ThrowIfCancellationRequested();
                    requests.Add(CreateNumberFormatRequest(1, values.Count - 1, i, i, "#,##0"));  // No decimals
                }

                // Add chart with multiple series
                List<(string, string)> seriesRanges = [];
                for (int i = 0; i < seriesNames.Count; i++)
                {
                    seriesRanges.Add(($"A", $"{(char)('B' + i)}"));
                }

                Request chartRequest = CreateChartRequest(
                    chartType,
                    chartTitle,
                    0,
                    values.Count - 1,
                    seriesRanges.ToArray()
                );
                requests.Add(chartRequest);

                activeCancellationToken.ThrowIfCancellationRequested();

                // Execute all formatting requests
                BatchUpdateSpreadsheetRequest batchUpdateRequest = new()
                {
                    Requests = requests
                };

                await _sheetsService.Spreadsheets
                    .BatchUpdate(batchUpdateRequest, spreadsheetId)
                    .ExecuteAsync(activeCancellationToken);

                activeCancellationToken.ThrowIfCancellationRequested();

                // Auto-resize columns
                DimensionRange dimensionRange = new()
                {
                    SheetId = 0,
                    Dimension = "COLUMNS",
                    StartIndex = 0,
                    EndIndex = seriesNames.Count + 1
                };

                Request autoResizeRequest = new()
                {
                    AutoResizeDimensions = new AutoResizeDimensionsRequest
                    {
                        Dimensions = dimensionRange
                    }
                };

                await _sheetsService.Spreadsheets
                    .BatchUpdate(new BatchUpdateSpreadsheetRequest
                    {
                        Requests = [autoResizeRequest]
                    }, spreadsheetId)
                    .ExecuteAsync(activeCancellationToken);

                activeCancellationToken.ThrowIfCancellationRequested();

                OpenGoogleSheet(spreadsheetId);
                TrackGoogleSheetsExport(stopwatch);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                MainMenu_Form.Instance.InvokeIfRequired(() =>
                    CustomMessageBox.ShowWithFormat("Export Error", "Failed to export multi-dataset count chart to Google Sheets.",
                    CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok)
                );
                throw;
            }
            finally
            {
                Loading_Form.CompleteOperation(operationMessage);
            }
        }

        private static void TrackGoogleSheetsExport(Stopwatch stopwatch)
        {
            stopwatch.Stop();

            Dictionary<ExportDataField, object> exportData = new()
            {
                { ExportDataField.ExportType, ExportType.GoogleSheetsChart },
                { ExportDataField.DurationMS, stopwatch.ElapsedMilliseconds },
                { ExportDataField.FileSize, "N/A" }
            };

            AnonymousDataManager.AddExportData(exportData);
        }

        /// <summary>
        /// Opens the specified Google Sheets spreadsheet in the default web browser.
        /// </summary>
        private static void OpenGoogleSheet(string spreadsheetId)
        {
            Tools.OpenLink($"https://docs.google.com/spreadsheets/d/{spreadsheetId}");
        }

        // Helper methods for creating format requests
        private static Request CreateHeaderFormatRequest(
            int startRowIndex,
            int endRowIndex,
            int startColumnIndex,
            int endColumnIndex)
        {
            return new Request
            {
                RepeatCell = new RepeatCellRequest
                {
                    Range = new GridRange
                    {
                        SheetId = 0,
                        StartRowIndex = startRowIndex,
                        EndRowIndex = endRowIndex + 1,
                        StartColumnIndex = startColumnIndex,
                        EndColumnIndex = endColumnIndex + 1
                    },
                    Cell = new CellData
                    {
                        UserEnteredFormat = new CellFormat
                        {
                            TextFormat = new TextFormat
                            {
                                Bold = true
                            },
                            BackgroundColor = new Google.Apis.Sheets.v4.Data.Color
                            {
                                Red = 0.678f,
                                Green = 0.847f,
                                Blue = 0.902f
                            }
                        }
                    },
                    Fields = "userEnteredFormat(textFormat,backgroundColor)"
                }
            };
        }

        private static Request CreateNumberFormatRequest(
            int startRowIndex,
            int endRowIndex,
            int startColumnIndex,
            int endColumnIndex,
            string numberFormat)
        {
            return new Request
            {
                RepeatCell = new RepeatCellRequest
                {
                    Range = new GridRange
                    {
                        SheetId = 0,
                        StartRowIndex = startRowIndex,
                        EndRowIndex = endRowIndex + 1,
                        StartColumnIndex = startColumnIndex,
                        EndColumnIndex = endColumnIndex + 1
                    },
                    Cell = new CellData
                    {
                        UserEnteredFormat = new CellFormat
                        {
                            NumberFormat = new NumberFormat
                            {
                                Type = "NUMBER",
                                Pattern = numberFormat
                            }
                        }
                    },
                    Fields = "userEnteredFormat.numberFormat"
                }
            };
        }

        private static Request CreateChartRequest(
            ChartType chartType,
            string chartTitle,
            int startRowIndex,
            int endRowIndex,
            (string XColumn, string YColumn)[] seriesRanges)
        {
            ChartSpec chartSpec = new()
            {
                Title = chartTitle
            };

            switch (chartType)
            {
                case ChartType.Line:
                case ChartType.Spline:
                    chartSpec.BasicChart = CreateBasicChartSpec(
                        seriesRanges,
                        startRowIndex,
                        endRowIndex,
                        "LINE",
                        chartType == ChartType.Spline
                    );
                    break;

                case ChartType.Column:
                    chartSpec.BasicChart = CreateBasicChartSpec(
                        seriesRanges,
                        startRowIndex,
                        endRowIndex,
                        "COLUMN",
                        false
                    );
                    break;

                case ChartType.Pie:
                    chartSpec.PieChart = CreatePieChartSpec(
                        seriesRanges[0],
                        startRowIndex,
                        endRowIndex
                    );
                    break;
            }

            return new Request
            {
                AddChart = new AddChartRequest
                {
                    Chart = new EmbeddedChart
                    {
                        Spec = chartSpec,
                        Position = new EmbeddedObjectPosition
                        {
                            OverlayPosition = new OverlayPosition
                            {
                                AnchorCell = new GridCoordinate
                                {
                                    SheetId = 0,
                                    RowIndex = 0,
                                    ColumnIndex = seriesRanges.Length + 2
                                }
                            }
                        }
                    }
                }
            };
        }

        private static BasicChartSpec CreateBasicChartSpec(
            (string XColumn, string YColumn)[] seriesRanges,
            int startRowIndex,
            int endRowIndex,
            string chartType,
            bool isSpline = false)
        {
            List<BasicChartSeries> series = [];

            foreach ((string _, string yColumn) in seriesRanges)
            {
                series.Add(new BasicChartSeries
                {
                    Series = new ChartData
                    {
                        SourceRange = new ChartSourceRange
                        {
                            Sources =
                            [
                                new GridRange
                                {
                                    SheetId = 0,
                                    StartRowIndex = startRowIndex,
                                    EndRowIndex = endRowIndex + 1,
                                    StartColumnIndex = yColumn[0] - 'A',
                                    EndColumnIndex = yColumn[0] - 'A' + 1
                                }
                            ]
                        }
                    },
                    TargetAxis = "LEFT_AXIS"
                });
            }

            return new BasicChartSpec
            {
                ChartType = chartType,
                LineSmoothing = isSpline,
                LegendPosition = "TOP_LEGEND",
                Domains =
                [
                    new BasicChartDomain
                    {
                        Domain = new ChartData
                        {
                            SourceRange = new ChartSourceRange
                            {
                                Sources =
                                [
                                    new() {
                                        SheetId = 0,
                                        StartRowIndex = startRowIndex,
                                        EndRowIndex = endRowIndex + 1,
                                        StartColumnIndex = 0,
                                        EndColumnIndex = 1
                                    }
                                ]
                            }
                        },
                        Reversed = false
                    }
                ],
                Series = series,
                HeaderCount = 1
            };
        }

        private static PieChartSpec CreatePieChartSpec(
            (string XColumn, string YColumn) range,
            int startRowIndex,
            int endRowIndex)
        {
            return new PieChartSpec
            {
                LegendPosition = "RIGHT_LEGEND",
                Domain = new ChartData
                {
                    SourceRange = new ChartSourceRange
                    {
                        Sources =
                        [
                            new() {
                                SheetId = 0,
                                StartRowIndex = startRowIndex,
                                EndRowIndex = endRowIndex + 1,
                                StartColumnIndex = 0,
                                EndColumnIndex = 1
                            }
                        ]
                    }
                },
                Series = new ChartData
                {
                    SourceRange = new ChartSourceRange
                    {
                        Sources =
                        [
                            new GridRange
                            {
                                SheetId = 0,
                                StartRowIndex = startRowIndex,
                                EndRowIndex = endRowIndex + 1,
                                StartColumnIndex = range.YColumn[0] - 'A',
                                EndColumnIndex = range.YColumn[0] - 'A' + 1
                            }
                        ]
                    }
                },
                PieHole = 0,
                ThreeDimensional = false
            };
        }
    }
}
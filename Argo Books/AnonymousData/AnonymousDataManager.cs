using Argo_Books.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Argo_Books;
using Argo_Books.Classes;
using Argo_Books.Language;
using System.Security.Cryptography;

namespace Argo_Books.AnonymousData
{
    public enum DataPointType
    {
        Export,
        OpenAI,
        OpenExchangeRates,
        GoogleSheets,
        Session,
        Error
    }
    public enum ExportDataField
    {
        ExportType,
        DurationMS,
        FileSize,
    }
    public enum ExportType
    {
        ExcelSheetsChart,
        GoogleSheetsChart,
        Backup,
        XLSX,
        Receipts
    }

    /// <summary>
    /// Manages collection and storage of anonymous data points for analytics.
    /// </summary>
    public static class AnonymousDataManager
    {
        private static GeoLocationData? _sessionLocation;
        private static DateTime? _sessionStartTime;
        private static string? _sessionHashedIP;

        // Configuration constants
        private const long MAX_FILE_SIZE_BYTES = 10485760;  // 10MB
        private const int MAX_LINES_BEFORE_ROTATION = 10000;
        private const int UPLOAD_RETRY_ATTEMPTS = 3;

        /// <summary>
        /// Initializes the session with geo-location data.
        /// </summary>
        public static async Task InitializeSessionAsync()
        {
            try
            {
                _sessionLocation = await GeoLocationService.GetLocationAsync();
                _sessionHashedIP = await GeoLocationService.GetHashedIPAsync();
            }
            catch (Exception ex)
            {
                Log.Error_AnonymousDataCollection($"Failed to initialize session geo-location: {ex.Message}");
                _sessionLocation = new GeoLocationData();
                _sessionHashedIP = "InitFailed";
            }
        }
        private static Dictionary<string, object> GetBaseDataPoint(DataPointType dataType)
        {
            Dictionary<string, object> dataPoint = new()
            {
                ["timestamp"] = Tools.FormatDateTime(DateTime.Now),
                ["dataType"] = dataType.ToString(),
                ["country"] = _sessionLocation?.Country ?? "Unknown",
                ["countryCode"] = _sessionLocation?.CountryCode ?? "Unknown",
                ["region"] = _sessionLocation?.Region ?? "Unknown",
                ["city"] = _sessionLocation?.City ?? "Unknown",
                ["timezone"] = _sessionLocation?.Timezone ?? "Unknown",
                ["dataId"] = GenerateDataPointId()  // Unique ID for deduplication
            };

            if (_sessionLocation?.IsVPN == true)
            {
                dataPoint["isVPN"] = true;
            }

            if (!string.IsNullOrEmpty(_sessionHashedIP) && _sessionHashedIP != "IPUnavailable")
            {
                dataPoint["hashedIP"] = _sessionHashedIP;
            }

            return dataPoint;
        }

        /// <summary>
        /// Generates a unique ID for each data point to prevent duplication.
        /// </summary>
        private static string GenerateDataPointId()
        {
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[8];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Adds export operation data to the anonymous data log.
        /// </summary>
        public static void AddExportData(Dictionary<ExportDataField, object> data)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await AddExportDataAsync(data);
                }
                catch (Exception ex)
                {
                    Log.Error_AnonymousDataCollection($"Failed to track export data: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// Adds export operation data to the anonymous data log.
        /// </summary>
        public static async Task AddExportDataAsync(Dictionary<ExportDataField, object> data)
        {
            if (_sessionLocation == null)
            {
                await InitializeSessionAsync();
            }

            ExportType exportType = (ExportType)data[ExportDataField.ExportType];
            long durationMs = (long)data[ExportDataField.DurationMS];

            Dictionary<string, object> dataPoint = GetBaseDataPoint(DataPointType.Export);
            dataPoint["ExportType"] = exportType.ToString();
            dataPoint["DurationMS"] = Tools.FormatDuration(durationMs);
            dataPoint["FileSize"] = data[ExportDataField.FileSize];

            await AppendToDataFileAsync(dataPoint);
        }

        /// <summary>
        /// Adds OpenAI API usage data to the anonymous data log.
        /// </summary>
        public static void AddOpenAIUsageData(string model, long durationMs, int tokensUsed)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await AddOpenAIUsageDataAsync(model, durationMs, tokensUsed);
                }
                catch (Exception ex)
                {
                    Log.Error_AnonymousDataCollection($"Failed to track OpenAI usage: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// Adds OpenAI API usage data to the anonymous data log.
        /// </summary>
        public static async Task AddOpenAIUsageDataAsync(string model, long durationMs, int tokensUsed)
        {
            if (_sessionLocation == null)
            {
                await InitializeSessionAsync();
            }

            Dictionary<string, object> dataPoint = GetBaseDataPoint(DataPointType.OpenAI);
            dataPoint["Model"] = model;
            dataPoint["DurationMS"] = durationMs;
            dataPoint["TokensUsed"] = tokensUsed;

            await AppendToDataFileAsync(dataPoint);
        }

        /// <summary>
        /// Adds Open Exchange Rates API usage data to the anonymous data log.
        /// </summary>
        public static void AddOpenExchangeRatesData(long durationMs)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await AddOpenExchangeRatesDataAsync(durationMs);
                }
                catch (Exception ex)
                {
                    Log.Error_AnonymousDataCollection($"Failed to track exchange rates usage: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// Adds Open Exchange Rates API usage data to the anonymous data log.
        /// </summary>
        public static async Task AddOpenExchangeRatesDataAsync(long durationMs)
        {
            if (_sessionLocation == null)
            {
                await InitializeSessionAsync();
            }

            Dictionary<string, object> dataPoint = GetBaseDataPoint(DataPointType.OpenExchangeRates);
            dataPoint["DurationMS"] = durationMs;

            await AppendToDataFileAsync(dataPoint);
        }

        /// <summary>
        /// Adds error occurrence data to the anonymous data log.
        /// </summary>
        public static void AddErrorData(string errorCode, string errorCategory, int lineNumber)
        {
            _ = Task.Run(async () =>
            {
                if (_sessionLocation == null)
                {
                    await InitializeSessionAsync();
                }

                Dictionary<string, object> dataPoint = GetBaseDataPoint(DataPointType.Error);
                dataPoint["ErrorCode"] = errorCode;
                dataPoint["ErrorCategory"] = errorCategory;
                dataPoint["LineNumber"] = lineNumber;

                await AppendToDataFileAsync(dataPoint, false);  // Don't log errors to prevent recursion
            });
        }
        public static void TrackSessionStart()
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    _sessionStartTime = DateTime.Now;

                    if (_sessionLocation == null)
                    {
                        await InitializeSessionAsync();
                    }

                    Dictionary<string, object> dataPoint = GetBaseDataPoint(DataPointType.Session);
                    dataPoint["action"] = "SessionStart";
                    dataPoint["userAgent"] = Environment.OSVersion.ToString();
                    dataPoint["appVersion"] = Tools.GetVersionNumber();

                    await AppendToDataFileAsync(dataPoint);
                }
                catch (Exception ex)
                {
                    Log.Error_AnonymousDataCollection($"Failed to track session start: {ex.Message}");
                }
            });
        }
        public static void TrackSessionEnd()
        {
            if (_sessionStartTime == null) { return; }

            _ = Task.Run(async () =>
            {
                TimeSpan duration = DateTime.Now - _sessionStartTime.Value;

                Dictionary<string, object> dataPoint = GetBaseDataPoint(DataPointType.Session);
                dataPoint["action"] = "SessionEnd";
                dataPoint["duration"] = duration.TotalSeconds;

                await AppendToDataFileAsync(dataPoint);
            });
        }

        /// <summary>
        /// Appends a data point to the anonymous data cache file with async file operations
        /// and automatic file rotation when size limits are reached.
        /// </summary>
        private static async Task AppendToDataFileAsync(Dictionary<string, object> dataPoint, bool logErrors = true)
        {
            try
            {
                // Ensure directory exists
                string directory = Path.GetDirectoryName(Directories.AnonymousUserData_file);
                Directories.CreateDirectory(directory);

                // Check and handle file rotation
                await RotateFileIfNeededAsync();

                // Add the data point to file
                string jsonLine = JsonConvert.SerializeObject(dataPoint, Formatting.None) + Environment.NewLine;

                // Use async file operations to avoid blocking
                await File.AppendAllTextAsync(Directories.AnonymousUserData_file, jsonLine);
            }
            catch (Exception ex)
            {
                if (logErrors)
                {
                    Log.Error_AnonymousDataCollection($"Failed to log anonymous data: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Rotates the data file if it exceeds size or line limits
        /// </summary>
        private static async Task RotateFileIfNeededAsync()
        {
            if (!File.Exists(Directories.AnonymousUserData_file))
            {
                return;
            }

            FileInfo fileInfo = new(Directories.AnonymousUserData_file);

            // Check file size limit
            if (fileInfo.Length > MAX_FILE_SIZE_BYTES)
            {
                await RotateDataFileAsync("Size limit exceeded");
                return;
            }

            // Check line count limit (more expensive operation, so do it less frequently)
            if (fileInfo.Length > MAX_FILE_SIZE_BYTES / 2)  // Only check line count when file is getting large
            {
                string[] lines = await File.ReadAllLinesAsync(Directories.AnonymousUserData_file);
                if (lines.Length > MAX_LINES_BEFORE_ROTATION)
                {
                    await RotateDataFileAsync("Line limit exceeded");
                }
            }
        }

        /// <summary>
        /// Rotates the current data file by uploading it and clearing it
        /// </summary>
        private static async Task RotateDataFileAsync(string reason)
        {
            Log.Write(1, $"Rotating anonymous data file: {reason}");

            // Try to upload current data before rotation
            bool uploadSuccess = false;
            for (int attempt = 1; attempt <= UPLOAD_RETRY_ATTEMPTS; attempt++)
            {
                try
                {
                    await UploadAnonymousDataAsync();
                    uploadSuccess = true;
                    break;
                }
                catch (Exception ex)
                {
                    Log.Error_AnonymousDataCollection($"Upload attempt {attempt} failed during rotation: {ex.Message}");
                    if (attempt < UPLOAD_RETRY_ATTEMPTS)
                    {
                        await Task.Delay(1000 * attempt);  // Exponential backoff
                    }
                }
            }

            if (uploadSuccess)
            {
                // Clear the file after successful upload
                try
                {
                    await File.WriteAllTextAsync(Directories.AnonymousUserData_file, "");
                    ResetUploadTracking();
                    Log.Write(1, "Anonymous data file rotated successfully");
                }
                catch (Exception ex)
                {
                    Log.Error_AnonymousDataCollection($"Failed to clear data file after rotation: {ex.Message}");
                }
            }
            else
            {
                // If upload failed, archive the file to prevent data loss
                try
                {
                    string archiveFile = $"{Directories.AnonymousUserData_file}.archive.{DateTime.Now:yyyyMMdd_HHmmss}";
                    File.Move(Directories.AnonymousUserData_file, archiveFile);
                    Log.Write(1, $"Data file archived to {archiveFile} due to upload failure");
                }
                catch (Exception ex)
                {
                    Log.Error_AnonymousDataCollection($"Failed to archive data file: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Exports only new data that hasn't been uploaded yet.
        /// </summary>
        public static async Task<string?> ExportIncrementalDataAsync(string outputFilePath, bool indent)
        {
            if (string.IsNullOrEmpty(outputFilePath) || !File.Exists(Directories.AnonymousUserData_file))
            {
                return null;
            }

            try
            {
                string[] allLines = await File.ReadAllLinesAsync(Directories.AnonymousUserData_file);
                long lastUploadedLine = GetLastUploadedLine();

                // Get only new lines since last upload
                IEnumerable<string> newLines = allLines.Skip((int)lastUploadedLine).Where(line => !string.IsNullOrWhiteSpace(line));

                if (!newLines.Any())
                {
                    return null;  // No new data to upload
                }

                List<JObject> newDataPoints = [];

                foreach (string line in newLines)
                {
                    try
                    {
                        JObject dataPoint = JObject.Parse(line);
                        newDataPoints.Add(dataPoint);
                    }
                    catch
                    {
                        // Skip invalid JSON lines
                    }
                }

                if (newDataPoints.Count == 0)
                {
                    return null;
                }

                // Group new data by type
                JObject organizedData = new()
                {
                    ["exportTime"] = Tools.FormatDateTime(DateTime.Now),
                    ["isIncremental"] = true,
                    ["previousUploadLine"] = lastUploadedLine,
                    ["newDataPointsCount"] = newDataPoints.Count,
                    ["geoLocationEnabled"] = true,
                    ["dataPoints"] = new JObject
                    {
                        ["Export"] = new JArray(newDataPoints.Where(d => d["dataType"]?.ToString() == DataPointType.Export.ToString())),
                        ["OpenAI"] = new JArray(newDataPoints.Where(d => d["dataType"]?.ToString() == DataPointType.OpenAI.ToString())),
                        ["OpenExchangeRates"] = new JArray(newDataPoints.Where(d => d["dataType"]?.ToString() == DataPointType.OpenExchangeRates.ToString())),
                        ["GoogleSheets"] = new JArray(newDataPoints.Where(d => d["dataType"]?.ToString() == DataPointType.GoogleSheets.ToString())),
                        ["Session"] = new JArray(newDataPoints.Where(d => d["dataType"]?.ToString() == DataPointType.Session.ToString())),
                        ["Error"] = new JArray(newDataPoints.Where(d => d["dataType"]?.ToString() == DataPointType.Error.ToString()))
                    }
                };

                // Write incremental data to file
                Formatting format = indent ? Formatting.Indented : Formatting.None;
                string organizedJson = JsonConvert.SerializeObject(organizedData, format);
                await File.WriteAllTextAsync(outputFilePath, organizedJson);

                return organizedJson;
            }
            catch (Exception ex)
            {
                Log.Error_AnonymousDataCollection($"Failed to export incremental data: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Exports the anonymous data in an organized format by data type (legacy method for full export).
        /// </summary>
        public static async Task ExportOrganizedDataAsync(string outputFilePath, bool indent)
        {
            if (string.IsNullOrEmpty(outputFilePath) || !File.Exists(Directories.AnonymousUserData_file))
            {
                return;
            }

            try
            {
                // Read all data points
                string[] lines = await File.ReadAllLinesAsync(Directories.AnonymousUserData_file);
                List<JObject> allDataPoints = [];

                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        try
                        {
                            JObject dataPoint = JObject.Parse(line);
                            allDataPoints.Add(dataPoint);
                        }
                        catch { /* Skip invalid JSON lines */ }
                    }
                }

                // Group data by type
                JObject organizedData = new()
                {
                    ["exportTime"] = Tools.FormatDateTime(DateTime.Now),
                    ["isIncremental"] = false,
                    ["totalDataPoints"] = allDataPoints.Count,
                    ["geoLocationEnabled"] = true,
                    ["dataPoints"] = new JObject
                    {
                        ["Export"] = new JArray(allDataPoints.Where(d => d["dataType"]?.ToString() == DataPointType.Export.ToString())),
                        ["OpenAI"] = new JArray(allDataPoints.Where(d => d["dataType"]?.ToString() == DataPointType.OpenAI.ToString())),
                        ["OpenExchangeRates"] = new JArray(allDataPoints.Where(d => d["dataType"]?.ToString() == DataPointType.OpenExchangeRates.ToString())),
                        ["GoogleSheets"] = new JArray(allDataPoints.Where(d => d["dataType"]?.ToString() == DataPointType.GoogleSheets.ToString())),
                        ["Session"] = new JArray(allDataPoints.Where(d => d["dataType"]?.ToString() == DataPointType.Session.ToString())),
                        ["Error"] = new JArray(allDataPoints.Where(d => d["dataType"]?.ToString() == DataPointType.Error.ToString()))
                    }
                };

                // Write data to file
                Formatting format = indent ? Formatting.Indented : Formatting.None;
                string organizedJson = JsonConvert.SerializeObject(organizedData, format);
                await File.WriteAllTextAsync(outputFilePath, organizedJson);
            }
            catch (Exception ex)
            {
                Log.Error_AnonymousDataCollection($"Failed to export organized data: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears the anonymous user data file.
        /// </summary>
        public static void ClearUserData()
        {
            Directories.DeleteFile(Directories.AnonymousUserData_file);
            LanguageManager.TranslationCache = [];
            ResetUploadTracking();
            _sessionLocation = null;
            _sessionHashedIP = null;
        }

        /// <summary>
        /// Gets the size of the anonymous data cache file in bytes.
        /// </summary>
        public static long GetUserDataCacheSize()
        {
            if (File.Exists(Directories.AnonymousUserData_file))
            {
                FileInfo fileInfo = new(Directories.AnonymousUserData_file);
                return fileInfo.Length;
            }
            return 0;
        }

        /// <summary>
        /// Uploads only new anonymous data since the last successful upload.
        /// </summary>
        private static async Task UploadAnonymousDataAsync()
        {
            if (MainMenu_Form.IsAdminMode) { return; }

            if (!File.Exists(Directories.AnonymousUserData_file)) { return; }

            string tempFile = "";
            try
            {
                // Check if API key is available
                string? apiKey = ApiConfig.ApiKey;
                if (string.IsNullOrEmpty(apiKey))
                {
                    Log.Error_AnonymousDataCollection("Upload API key not found in environment variables");
                    return;
                }

                tempFile = Path.Combine(Path.GetTempPath(), "incremental_anonymous_data.json");

                // Export only new data since last upload
                string incrementalData = await ExportIncrementalDataAsync(tempFile, false);

                if (string.IsNullOrEmpty(incrementalData))
                {
                    Log.Write(1, "No new anonymous data to upload");
                    return;
                }

                using HttpClient client = new();
                client.Timeout = TimeSpan.FromMinutes(5);

                // Set authentication headers
                client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
                client.DefaultRequestHeaders.Add("User-Agent", ApiConfig.UserAgent);

                // Properly manage the file stream
                using FileStream fileStream = File.OpenRead(tempFile);
                using StreamContent streamContent = new(fileStream);
                using MultipartFormDataContent form = new()
                {
                    { streamContent, "file", "anonymous_data.json" }
                };

                HttpResponseMessage response = await client.PostAsync(ApiConfig.ServerUrl, form);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Log.Write(1, "Successfully uploaded incremental anonymous data");

                    // Update the last uploaded position only after successful upload
                    await UpdateLastUploadedLineAsync();
                    SaveUploadTime(DateTime.Now);
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    Log.Error_AnonymousDataCollection($"Upload failed with status {response.StatusCode}: {error}");
                }
            }
            catch (HttpRequestException ex)
            {
                Log.Error_AnonymousDataCollection($"Network error during upload: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Log.Error_AnonymousDataCollection($"Upload timeout: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Error_AnonymousDataCollection($"Upload failed: {ex.Message}");
            }
            finally
            {
                // Clean up - file handles are now closed
                Directories.DeleteFile(tempFile);
            }
        }
        public static async Task TryUploadDataOnStartupAsync()
        {
            DateTime? lastUpload = GetLastUploadTime();
            if (lastUpload == null || (DateTime.Now - lastUpload.Value).TotalHours >= 24)
            {
                await UploadAnonymousDataAsync();
            }
        }

        /// <summary>
        /// Gets the line number of the last successfully uploaded data.
        /// </summary>
        private static long GetLastUploadedLine()
        {
            string? val = DataFileManager.GetValue(GlobalAppDataSettings.LastUploadedLine_AnonymousData);
            return long.TryParse(val, out long line) ? line : 0;
        }

        /// <summary>
        /// Updates the last uploaded line position after successful upload.
        /// </summary>
        private static async Task UpdateLastUploadedLineAsync()
        {
            if (!File.Exists(Directories.AnonymousUserData_file))
            {
                return;
            }

            try
            {
                string[] lines = await File.ReadAllLinesAsync(Directories.AnonymousUserData_file);
                long currentLineCount = lines.Length;
                DataFileManager.SetValue(GlobalAppDataSettings.LastUploadedLine_AnonymousData, currentLineCount.ToString());
            }
            catch (Exception ex)
            {
                Log.Error_AnonymousDataCollection($"Failed to update last uploaded line: {ex.Message}");
            }
        }

        /// <summary>
        /// Resets upload tracking (used when file is cleared or rotated).
        /// </summary>
        private static void ResetUploadTracking()
        {
            DataFileManager.SetValue(GlobalAppDataSettings.LastUploadedLine_AnonymousData, "0");
        }
        private static DateTime? GetLastUploadTime()
        {
            string? val = DataFileManager.GetValue(GlobalAppDataSettings.UploadTime_AnonymousData);
            return DateTime.TryParse(val, out DateTime dt) ? dt : null;
        }
        private static void SaveUploadTime(DateTime time)
        {
            DataFileManager.SetValue(GlobalAppDataSettings.UploadTime_AnonymousData, time.ToString("o"));
        }
    }
}
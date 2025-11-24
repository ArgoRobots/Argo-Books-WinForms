using Argo_Books.AnonymousData;
using Argo_Books.Classes;
using Argo_Books.Language;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Argo_Books.Classes
{
    /// <summary>
    /// Defines the categories of errors that can occur in the application.
    /// </summary>
    public enum ErrorCategory
    {
        General,
        File,
        DataGridView,
        Encryption,
        API,
        Translation,
        FileAssociation,
        Environment
    }

    public enum LogCategory
    {
        Error = 0,
        Debug = 1,
        General = 2,
        ProductManager = 3,
        PasswordManager = 4
    }

    /// <summary>
    /// Provides logging functionality with different log levels and error handling mechanisms.
    /// </summary>
    internal partial class Log
    {
        // Properties
        public static string LogText { get; set; }
        private static readonly List<LogEntry> _logEntries = [];
        private static readonly Lock _logLock = new();
        private static readonly int MaxInMemoryEntries = 1000;  // Keep last 1000 in memory

        [GeneratedRegex(@"Error-[a-zA-Z0-9]+")]
        private static partial Regex ErrorCodeRegex();

        // Get formatted log text for current language
        public static string GetFormattedLogText(string languageCode = null)
        {
            lock (_logLock)
            {
                StringBuilder sb = new();
                foreach (LogEntry entry in _logEntries)
                {
                    sb.Append(entry.GetDisplayText(languageCode));
                }
                return sb.ToString();
            }
        }

        // Clear all logs
        public static void ClearLogs()
        {
            lock (_logLock)
            {
                _logEntries.Clear();
                LogText = "";
            }

            // Update UI if form is open
            if (Tools.IsFormOpen<Log_Form>())
            {
                Log_Form.Instance.Log_RichTextBox.InvokeIfRequired(() =>
                    Log_Form.Instance.Log_RichTextBox.Clear()
                );
            }
        }

        // Save logs
        public static void SaveLogs()
        {
            // Then create timestamped backup as before
            Directories.CreateDirectory(Directories.Logs_dir);

            DateTime time = DateTime.Now;
            int count = 0;
            string directory;

            while (true)
            {
                if (count == 0)
                {
                    directory = $@"{Directories.Logs_dir}\{time.Year}-{time.Month}-{time.Day}-{time.Hour}-{time.Minute}{ArgoFiles.TxtFileExtension}";
                }
                else
                {
                    directory = $@"{Directories.Logs_dir}\{time.Year}-{time.Month}-{time.Day}-{time.Hour}-{time.Minute}-{count}{ArgoFiles.TxtFileExtension}";
                }

                if (!File.Exists(directory))
                {
                    // Save in English for the backup file
                    string englishLogText = GetFormattedLogText("en");
                    Directories.WriteTextToFile(directory, englishLogText);
                    break;
                }
                count++;
            }

            CleanupOldLogFiles();
        }

        private static void CleanupOldLogFiles()
        {
            byte maxFiles = 30;

            try
            {
                List<FileInfo> logFiles = Directory.GetFiles(Directories.Logs_dir, $"*{ArgoFiles.TxtFileExtension}")
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.CreationTime)
                    .ToList();

                if (logFiles.Count > maxFiles)
                {
                    // Remove the oldest files
                    foreach (FileInfo? file in logFiles.Skip(maxFiles))
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception ex)
                        {
                            WriteLogEntry(LogCategory.Error, "Failed to delete old log file {0}: {1}", file.FullName, ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLogEntry(LogCategory.Error, "Error during log cleanup: {0}", ex.Message);
            }
        }

        // Writing to log methods
        /// <summary>
        /// Writes a log entry with a specified log level and message.
        /// Log level index: 0 = [Error], 1 = [Debug], 2 = [General], 3 = [Product manager], 4 = [Password manager].
        /// </summary>
        public static void Write(byte index, string text)
        {
            if (!Properties.Settings.Default.ShowDebugInfo && index == 1)
            {
                return;
            }

            WriteLogEntry((LogCategory)index, text);
        }

        /// <summary>
        /// Writes a log entry with formatting support for translation.
        /// Log level index: 0 = [Error], 1 = [Debug], 2 = [General], 3 = [Product manager], 4 = [Password manager].
        /// </summary>
        public static void WriteWithFormat(byte index, string messageTemplate, params object[] args)
        {
            if (!Properties.Settings.Default.ShowDebugInfo && index == 1)
            {
                return;
            }

            WriteLogEntry((LogCategory)index, messageTemplate, args);
        }

        /// <summary>
        /// Core method that handles storing and displaying log entries.
        /// </summary>
        private static void WriteLogEntry(LogCategory category, string template, params object[] args)
        {
            LogEntry entry = new()
            {
                Timestamp = DateTime.Now,
                Category = category,
                Template = template,
                Arguments = args?.Length > 0 ? args : null
            };

            lock (_logLock)
            {
                _logEntries.Add(entry);

                // Update LogText
                string englishMessage = args?.Length > 0 ? string.Format(template, args) : template;
                string timestamp = "<" + Tools.FormatTime(entry.Timestamp) + "> ";
                LogText += timestamp + GetEnglishFormattedCategory(category) + " " + englishMessage + "\n";

                // Maintain memory limit
                if (_logEntries.Count > MaxInMemoryEntries)
                {
                    // Remove oldest entries but keep the essential ones
                    List<LogEntry> entriesToKeep = _logEntries.Skip(_logEntries.Count - MaxInMemoryEntries).ToList();
                    _logEntries.Clear();
                    _logEntries.AddRange(entriesToKeep);

                    RebuilLogText();
                }
            }

            // Update UI if form is open
            if (Tools.IsFormOpen<Log_Form>())
            {
                string currentLanguage = LanguageManager.GetDefaultLanguageAbbreviation() ?? "en";
                string displayText = entry.GetDisplayText(currentLanguage);

                Log_Form.Instance.Log_RichTextBox.InvokeIfRequired(() =>
                    Log_Form.Instance.Log_RichTextBox.AppendText(displayText)
                );
            }
        }
        private static void RebuilLogText()
        {
            StringBuilder sb = new();
            foreach (LogEntry entry in _logEntries)
            {
                string englishMessage = entry.Arguments?.Length > 0 ?
                    string.Format(entry.Template, entry.Arguments) : entry.Template;
                string timestamp = "<" + Tools.FormatTime(entry.Timestamp) + "> ";
                sb.AppendLine(timestamp + GetEnglishFormattedCategory(entry.Category) + " " + englishMessage);
            }
            LogText = sb.ToString();
        }

        /// <summary>
        /// Gets the English formatted category (always returns English regardless of current language).
        /// </summary>
        public static string GetEnglishFormattedCategory(LogCategory category)
        {
            return category switch
            {
                LogCategory.Error => "[Error]",
                LogCategory.Debug => "[Debug]",
                LogCategory.General => "[General]",
                LogCategory.ProductManager => "[Product manager]",
                LogCategory.PasswordManager => "[Password manager]",
                _ => ""
            };
        }

        /// <summary>
        /// Extracts error code from error message (e.g., "Error-3vknm9" from the message).
        /// </summary>
        private static string ExtractErrorCode(string message)
        {
            Match match = ErrorCodeRegex().Match(message);
            return match.Success ? match.Value : "Unknown";
        }
        public static string GetTranslatedFormattedCategory(LogCategory category)
        {
            return category switch
            {
                LogCategory.Error => "[" + LanguageManager.TranslateString("Error") + "]",
                LogCategory.Debug => "[" + LanguageManager.TranslateString("Debug") + "]",
                LogCategory.General => "[" + LanguageManager.TranslateString("General") + "]",
                LogCategory.ProductManager => "[" + LanguageManager.TranslateString("Product manager") + "]",
                LogCategory.PasswordManager => "[" + LanguageManager.TranslateString("Password manager") + "]",
                _ => ""
            };
        }

        private static void Error(
            string messageTemplate,
            string link,
            ErrorCategory category,
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            // Build display message template
            string displayTemplate = messageTemplate;
            if (!string.IsNullOrEmpty(link))
            {
                displayTemplate += "\nMore information: " + link;
                CustomMessageBoxVariables.LinkStart = messageTemplate.Length + 19;
                CustomMessageBoxVariables.Link = link;
                CustomMessageBoxVariables.LinkLength = link.Length;
            }

            // Build log template (includes debug info for admins)
            string logTemplate = displayTemplate;
            if (MainMenu_Form.IsAdminMode)
            {
                logTemplate += "\nDebug info:\nLine: '{" + args.Length + "}'.\nStack trace\n: '{" + (args.Length + 1) + "}'.";
            }

            // Prepare args
            List<object> allArgs = [];
            if (args != null) { allArgs.AddRange(args); }
            allArgs.Add(lineNumber);
            allArgs.Add(Environment.StackTrace);

            // Log with debug info
            WriteLogEntry(LogCategory.Error, logTemplate, allArgs.ToArray());

            // Add to anonymous data
            try
            {
                string errorCode = ExtractErrorCode(messageTemplate);
                AnonymousDataManager.AddErrorData(errorCode, category.ToString(), lineNumber);
            }
            catch
            {
                // Silently fail
            }

            // Show message box without debug info (only original args)
            string currentLanguage = LanguageManager.GetDefaultLanguageAbbreviation() ?? "en";
            string displayMessage;
            if (currentLanguage == "en")
            {
                displayMessage = string.Format(displayTemplate, args ?? []);
            }
            else
            {
                string translatedTemplate = LanguageManager.TranslateString(displayTemplate);
                displayMessage = string.Format(translatedTemplate, args ?? []);
            }
            CustomMessageBox.Show("Error", displayMessage, CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok);
        }

        // File errors
        public static void Error_FileDoesNotExist(
            string filePath,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-3vknm9: File does not exist:\n'{0}'.",
                "",
                ErrorCategory.File,
                lineNumber,
                filePath);
        }
        public static void Error_FileAlreadyExists(
            string filePath,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-djrr3r: File already exists:\n'{0}'.",
                "",
                ErrorCategory.File,
                lineNumber,
                filePath);
        }
        public static void Error_DestinationFileAlreadyExists(
            string filePath,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-8g8we7: The destination file already exists:\n'{0}'.",
                "",
                ErrorCategory.File,
                lineNumber,
                filePath);
        }
        public static void Error_TheSourceAndDestinationAreTheSame(
            string source, string destination,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-h88tzd: The source and destination files are the same.\nSource: '{0}'.\nDestination: '{1}'.",
                "",
                ErrorCategory.File,
                lineNumber,
                source, destination);
        }
        public static void Error_WriteToFile(
            string filePath,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-w7f3k2: Failed to write to file:\n'{0}'.",
                "",
                ErrorCategory.File,
                lineNumber,
                filePath);
        }
        public static void Error_ReadFile(
            string filePath,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-r9d5m1: Failed to read the file:\n'{0}'.",
                "",
                ErrorCategory.File,
                lineNumber,
                filePath);
        }
        public static void Error_Save(
            string info, string filePath,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-s4v8n6: Failed to save. {0}.\n'{1}'.",
                "",
                ErrorCategory.File,
                lineNumber,
                info, filePath);
        }

        // DataGridView errors
        public static void Error_DataGridViewCellIsEmpty(
            DataGridViewCell cell,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-d3g7c4: Cell is empty in '{0}' in column '{1}'.",
                "",
                ErrorCategory.DataGridView,
                lineNumber,
                cell.DataGridView.Name, cell.OwningColumn.Name);
        }
        public static void Error_RowIsOutOfRange(
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-r6w9o2: Row is out of range.",
                "",
                ErrorCategory.DataGridView,
                lineNumber);
        }

        // Encryption errors
        public static void Error_InitEncryptionHelper(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-i2e5h8: Error initializing EncryptionHelper. {0}.",
                "",
                ErrorCategory.Encryption,
                lineNumber,
                info);
        }
        public static void Error_Encryption(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-e7n9c1: Error during encryption. {0}.",
                "",
                ErrorCategory.Encryption,
                lineNumber,
                info);
        }
        public static void Error_Decryption(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-d4c8r3: Error during decryption. {0}.",
                "",
                ErrorCategory.Encryption,
                lineNumber,
                info);
        }

        // API errors
        public static void Error_GetExchangeRate(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-g8x2r5: Error getting exchange rates. {0}.",
                "",
                ErrorCategory.API,
                lineNumber,
                info);
        }
        public static void Error_TranslationAPIRequestFailed(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-t5a7p9: API request failed: {0}.",
                "",
                ErrorCategory.API,
                lineNumber,
                info);
        }
        public static void Error_EnhancingSearch(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-e3n6s1: Error enhancing search: {0}.",
                "",
                ErrorCategory.API,
                lineNumber,
                info);
        }
        public static void Error_ExportingChart(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-x4p8c2: Failed to export chart: {0}.",
                "",
                ErrorCategory.General,
                lineNumber,
                info);
        }

        // Language translation errors
        public static void Error_GetTranslation(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-g9t3l7: Error getting the translation. {0}.",
                "",
                ErrorCategory.Translation,
                lineNumber,
                info);
        }
        public static void Error_Translation(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-t2l8n4: AI Query Translation failed: {0}.",
                "",
                ErrorCategory.Translation,
                lineNumber,
                info);
        }
        public static void Error_SaveTranslationCache(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-t7c4s2: Failed to save translation cache. {0}.",
                "",
                ErrorCategory.Translation,
                lineNumber,
                info);
        }

        // Anonymous usage data errors
        public static void Error_AnonymousDataCollection(string message)
        {
            WriteWithFormat(0, "Error-a6d5c9: Error collecting anonymous usage data: {0}.",
                message);
        }

        // File association errors
        public static void Error_RegisterFileAssociations(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-r7f2a8: Error registering file associations: {0}.",
                "",
                ErrorCategory.FileAssociation,
                lineNumber,
                info);
        }

        // ENV errors
        public static void Error_ENVFileNotFound(
            string fileName,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-v3f6n1: ENV file '{0}' not found relative to solution.",
                "",
                ErrorCategory.Environment,
                lineNumber,
                fileName);
        }
        public static void Error_ENVKeyNotFound(
            string key,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-v8k4n5: ENV key {0} not found.",
                "",
                ErrorCategory.Environment,
                lineNumber,
                key);
        }

        // Auto update errors
        public static void Error_InitializeUpdateManager(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-u1i8m3: Error initializing NetSparkleUpdateManager. {0}.",
                "",
                ErrorCategory.API,
                lineNumber,
                info);
        }
        public static void Error_CheckForUpdates(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-u2c7k9: Error checking for updates. {0}.",
                "",
                ErrorCategory.API,
                lineNumber,
                info);
        }
        public static void Error_StartUpdate(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-u3s5n2: Error starting update. {0}.",
                "",
                ErrorCategory.API,
                lineNumber,
                info);
        }
        public static void Error_DownloadUpdate(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-u4d6w8: Error downloading update. {0}.",
                "",
                ErrorCategory.API,
                lineNumber,
                info);
        }
        public static void Error_ApplyUpdate(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-u5a4r1: Error applying update and restarting. {0}.",
                "",
                ErrorCategory.API,
                lineNumber,
                info);
        }

        // Secrets Manager errors
        public static void Error_CreateEncryptedSecretsFile(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-s1c9e4: Error creating encrypted secrets file. {0}.",
                "",
                ErrorCategory.Encryption,
                lineNumber,
                info);
        }
        public static void Error_SecretsFileNotFound(
            string filePath,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-s2f7n8: Encrypted secrets file not found: '{0}'.",
                "",
                ErrorCategory.File,
                lineNumber,
                filePath);
        }
        public static void Error_DecryptSecretsFile(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-s3d2r6: Failed to decrypt secrets file. {0}.",
                "",
                ErrorCategory.Encryption,
                lineNumber,
                info);
        }
        public static void Error_LoadEncryptedSecrets(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-s4l5k1: Error loading encrypted secrets. {0}.",
                "",
                ErrorCategory.Encryption,
                lineNumber,
                info);
        }

        // Application Startup errors
        public static void Error_EnsureDefaultLanguageTranslations(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-a1l7t9: Error ensuring default language translations. {0}.",
                "",
                ErrorCategory.Translation,
                lineNumber,
                info);
        }
        public static void Error_OpenCompanyFromCommandLine(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-a2o5c8: Error opening company from command line. {0}.",
                "",
                ErrorCategory.File,
                lineNumber,
                info);
        }
        public static void Error_AutoOpenRecentCompanyAfterUpdate(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-a3u6r4: Error auto-opening recent company after update. {0}.",
                "",
                ErrorCategory.File,
                lineNumber,
                info);
        }

        // Theme Change Detector error
        public static void Error_StartThemeListener(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-t1s8l5: Error starting theme listener. {0}.",
                "",
                ErrorCategory.General,
                lineNumber,
                info);
        }

        // Registry Watcher error
        public static void Error_RegistryWatcher(
            string info,
            [CallerLineNumber] int lineNumber = 0)
        {
            Error("Error-r9w3k7: Registry watcher error. {0}.",
                "",
                ErrorCategory.General,
                lineNumber,
                info);
        }
    }
}
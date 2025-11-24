using Argo_Books.Classes;
using System.Formats.Tar;

namespace Argo_Books.Encryption
{
    /// <summary>
    /// Manages footer operations for company files, handling version, accountant list, and password data.
    /// All operations work entirely in memory without creating temporary files.
    /// </summary>
    public static class FooterManager
    {
        /// <summary>
        /// Reads the complete footer from a company file.
        /// </summary>
        /// <returns>FooterData object containing all footer information, or null if reading fails</returns>
        public static FooterData? ReadFooter(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Log.WriteWithFormat(1, "Company file not found: {0}", filePath);
                    return null;
                }

                // Read all lines from the file
                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length < 2)
                {
                    Log.Write(1, "Company file does not have minimum footer structure");
                    return null;
                }

                // Determine if file is encrypted by checking second-to-last line
                string secondLastLine = lines[^2];
                bool isEncrypted = secondLastLine.Contains(EncryptionManager.encryptedTag) &&
                                 secondLastLine.Split(':')[1] == EncryptionManager.encryptedValue;

                FooterData footer = new()
                {
                    IsEncrypted = isEncrypted
                };

                // Extract password (always last line)
                string passwordLine = lines[^1];
                footer.Password = ExtractPassword(passwordLine, isEncrypted);

                // Extract version if present (third from last)
                if (lines.Length >= 3)
                {
                    string versionLine = lines[^3];
                    footer.Version = ExtractVersion(versionLine, isEncrypted);
                }

                // Extract accountants if present (fourth from last)
                if (lines.Length >= 4)
                {
                    string accountantsLine = lines[^4];
                    footer.Accountants = ExtractAccountants(accountantsLine, isEncrypted);
                }

                return footer;
            }
            catch (Exception ex)
            {
                Log.WriteWithFormat(1, "Error reading footer from company file: {0}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Writes footer data to a company file with proper formatting.
        /// </summary>
        public static void WriteFooterToStream(MemoryStream stream, FooterData footer)
        {
            try
            {
                List<string> footerLines = [];

                if (footer.Accountants?.Count > 0)
                {
                    string accountantList = string.Join(",", footer.Accountants);
                    string accountantLine = $"accountants:{accountantList}";

                    if (footer.IsEncrypted)
                    {
                        accountantLine = EncryptionManager.EncryptString(accountantLine, EncryptionManager.AesKey, EncryptionManager.AesIV);
                    }

                    footerLines.Add(accountantLine);
                }

                if (!string.IsNullOrEmpty(footer.Version))
                {
                    string versionLine = $"version:{footer.Version}";

                    if (footer.IsEncrypted)
                    {
                        versionLine = EncryptionManager.EncryptString(versionLine, EncryptionManager.AesKey, EncryptionManager.AesIV);
                    }

                    footerLines.Add(versionLine);
                }

                // Always add encryption marker
                if (footer.IsEncrypted)
                {
                    footerLines.Add(EncryptionManager.encryptedTag + EncryptionManager.encryptedValue);
                }
                else
                {
                    footerLines.Add(EncryptionManager.encryptedTag);
                }

                // Always add password (always encrypted)
                if (!string.IsNullOrEmpty(footer.Password))
                {
                    string passwordLine = EncryptionManager.EncryptString(EncryptionManager.passwordTag + footer.Password, EncryptionManager.AesKey, EncryptionManager.AesIV);
                    footerLines.Add(passwordLine);
                }
                else
                {
                    // Even if no password, add an encrypted empty password line to maintain structure
                    string passwordLine = EncryptionManager.EncryptString(EncryptionManager.passwordTag, EncryptionManager.AesKey, EncryptionManager.AesIV);
                    footerLines.Add(passwordLine);
                }

                // Write footer lines to stream with consistent line endings
                using StreamWriter writer = new(stream, System.Text.Encoding.UTF8, leaveOpen: true);

                foreach (string line in footerLines)
                {
                    writer.Write(Environment.NewLine);
                    writer.Write(line);
                }

                writer.Flush();
            }
            catch (Exception ex)
            {
                Log.WriteWithFormat(1, "Error writing footer to company file: {0}", ex.Message);
            }
        }

        // Private methods
        /// <summary>
        /// Extracts accountant list from footer line, decrypting if necessary.
        /// </summary>
        private static List<string> ExtractAccountants(string accountantLine, bool isEncrypted)
        {
            List<string> accountants = [];

            try
            {
                string processedLine = accountantLine;

                if (isEncrypted)
                {
                    processedLine = EncryptionManager.DecryptString(accountantLine, EncryptionManager.AesKey, EncryptionManager.AesIV);
                    if (string.IsNullOrEmpty(processedLine))
                    {
                        return accountants;
                    }
                }

                if (processedLine.StartsWith("accountants:"))
                {
                    string accountantList = processedLine.Split(':')[1];
                    if (!string.IsNullOrEmpty(accountantList))
                    {
                        accountants = accountantList.Split(',')
                            .Select(a => a.Trim())
                            .Where(a => !string.IsNullOrEmpty(a))
                            .ToList();
                    }
                }

                return accountants;
            }
            catch (Exception ex)
            {
                Log.WriteWithFormat(1, "Error extracting accountants: {0}", ex.Message);
                return accountants;
            }
        }

        /// <summary>
        /// Extracts version from footer line, decrypting if necessary.
        /// </summary>
        private static string? ExtractVersion(string versionLine, bool isEncrypted)
        {
            try
            {
                string processedLine = versionLine;

                if (isEncrypted)
                {
                    processedLine = EncryptionManager.DecryptString(versionLine, EncryptionManager.AesKey, EncryptionManager.AesIV);
                    if (string.IsNullOrEmpty(processedLine))
                    {
                        return null;
                    }
                }

                if (processedLine.StartsWith("version:"))
                {
                    return processedLine.Split(':')[1];
                }

                return null;
            }
            catch (Exception ex)
            {
                Log.WriteWithFormat(1, "Error extracting version: {0}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Extracts password from footer line, decrypting if necessary.
        /// </summary>
        private static string? ExtractPassword(string passwordLine, bool isEncrypted)
        {
            try
            {
                if (isEncrypted)
                {
                    string decryptedPassword = EncryptionManager.DecryptString(passwordLine, EncryptionManager.AesKey, EncryptionManager.AesIV);
                    if (decryptedPassword?.StartsWith(EncryptionManager.passwordTag) == true)
                    {
                        return decryptedPassword.Split(':')[1];
                    }
                }
                else if (passwordLine.StartsWith(EncryptionManager.passwordTag))
                {
                    return EncryptionManager.DecryptString(passwordLine, EncryptionManager.AesKey, EncryptionManager.AesIV)?.Split(':')[1];
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.WriteWithFormat(1, "Error extracting password: {0}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets the footer line count for a file (for use with DecryptFileToMemoryStream).
        /// </summary>
        public static int GetFooterLineCount(FooterData footer)
        {
            if (footer == null) { return 2; }  // Default minimum

            int count = 2;  // Always has encryption marker and password line

            if (!string.IsNullOrEmpty(footer.Version)) { count++; }
            if (footer.Accountants?.Count > 0) { count++; }

            return count;
        }

        // Public getter methods
        /// <summary>
        /// Gets the accountant list from a company file.
        /// </summary>
        /// <summary>
        /// Gets the accountant list from a company file, with fallback for older file formats.
        /// </summary>
        public static List<string> GetAccountants(string filePath)
        {
            // First try to get accountants from footer V.1.0.7+
            List<string> footerAccountants = ReadFooter(filePath)?.Accountants ?? [];

            if (footerAccountants.Count > 0)
            {
                return footerAccountants;
            }

            // Fallback for older files: extract and read from internal accountants file
            return GetAccountantsFromLegacyFile(filePath);
        }

        /// <summary>
        /// Gets accountants from files made in V.1.0.6 or earlier by temporarily extracting the accountants file.
        /// </summary>
        private static List<string> GetAccountantsFromLegacyFile(string filePath)
        {
            string tempDir = null;
            string decryptedTempFile = null;

            try
            {
                // Check if file is encrypted
                bool isEncrypted = IsEncrypted(filePath);
                string sourceFile = filePath;

                if (isEncrypted)
                {
                    // Create a temporary decrypted file
                    decryptedTempFile = Path.GetTempFileName();
                    EncryptionManager.DecryptAndWriteToFile(filePath, decryptedTempFile, EncryptionManager.AesKey, EncryptionManager.AesIV);
                    sourceFile = decryptedTempFile;
                }

                // Create a temporary directory for extraction
                tempDir = Path.Combine(Path.GetTempPath(), "ArgoAccountants_" + Guid.NewGuid().ToString("N")[..8]);
                Directory.CreateDirectory(tempDir);

                // Extract the tar file to temp directory
                TarFile.ExtractToDirectory(sourceFile, tempDir, overwriteFiles: true);

                // Look for the accountants file in the extracted directory
                string[] directories = Directory.GetDirectories(tempDir);

                foreach (string dir in directories)
                {
                    string accountantsFile = Path.Combine(dir, "accountants.txt");
                    if (File.Exists(accountantsFile))
                    {
                        // Read accountants from the file
                        string[] accountantLines = File.ReadAllLines(accountantsFile);
                        List<string> accountants = accountantLines
                            .Where(line => !string.IsNullOrWhiteSpace(line))
                            .ToList();

                        Log.WriteWithFormat(1, "Found {0} accountants in legacy file format", accountants.Count);
                        return accountants;
                    }
                }

                Log.Write(1, "No accountants file found in legacy company file");
                return [];
            }
            catch (Exception ex)
            {
                Log.WriteWithFormat(1, "Error reading accountants from legacy file: {0}", ex.Message);
                return [];
            }
            finally
            {
                // Clean up temporary files
                if (decryptedTempFile != null && File.Exists(decryptedTempFile))
                {
                    try { File.Delete(decryptedTempFile); } catch { }
                }

                if (tempDir != null && Directory.Exists(tempDir))
                {
                    try { Directory.Delete(tempDir, true); } catch { }
                }
            }
        }

        /// <summary>
        /// Gets the version from a company file.
        /// </summary>
        public static string? GetVersion(string filePath)
        {
            return ReadFooter(filePath)?.Version;
        }

        /// <summary>
        /// Gets the password from a company file.
        /// </summary>
        public static bool IsEncrypted(string filePath)
        {
            return ReadFooter(filePath)?.IsEncrypted ?? false;
        }

        /// <summary>
        /// Gets the password from a company file.
        /// </summary>
        public static string? GetPassword(string filePath)
        {
            return ReadFooter(filePath)?.Password;
        }
    }
}
using Argo_Books.Encryption;
using Argo_Books;
using Argo_Books.Classes;

namespace Argo_Books.Classes
{
    /// <summary>
    /// Handles version comparison and compatibility checking for application files.
    /// </summary>
    public static class VersionCompatibilityChecker
    {
        /// <summary>
        /// Validates file version compatibility before proceeding with file operations.
        /// Call this before any file opening operation to ensure compatibility.
        /// This method extracts and reads the version entirely in memory without creating temporary files.
        /// </summary>
        /// <returns>True if compatible and should proceed, false if incompatible</returns>
        public static bool HandleFileVersionCompatibility(string filePath)
        {
            try
            {
                string fileVersion = FooterManager.GetVersion(filePath);
                string currentVersion = Tools.GetVersionNumber();

                if (!IsFileVersionCompatible(fileVersion, currentVersion))
                {
                    HandleIncompatibleVersion(
                        fileVersion ?? "Unknown",
                        currentVersion,
                        filePath
                    );
                    return true;
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Write(1, $"Error validating file version: {ex.Message}");
                return true;  // Default to compatible on error to avoid blocking access
            }
        }

        /// <summary>
        /// Checks if a file version is compatible with the current application version.
        /// </summary>
        /// <returns>True if compatible, false if file version is newer.</returns>
        private static bool IsFileVersionCompatible(string fileVersion, string currentVersion)
        {
            if (string.IsNullOrEmpty(fileVersion))
            {
                // If no version info in file, assume it's an older format and compatible
                return true;
            }

            if (string.IsNullOrEmpty(currentVersion))
            {
                Log.Write(1, "Current application version is null or empty");
                return false;
            }

            try
            {
                Version fileVer = new(fileVersion);
                Version currentVer = new(currentVersion);

                // File is compatible if its version is less than or equal to current version
                return fileVer <= currentVer;
            }
            catch (Exception ex)
            {
                Log.Write(1, $"Error parsing version strings. FileVersion: {fileVersion}, CurrentVersion: {currentVersion}, Error: {ex.Message}");
                // If we can't parse versions, assume compatible to avoid blocking access
                return true;
            }
        }

        /// <summary>
        /// Displays an incompatible version warning.
        /// </summary>
        private static void HandleIncompatibleVersion(string fileVersion, string currentVersion, string filePath)
        {
            string company = Path.GetFileNameWithoutExtension(filePath);

            CustomMessageBox.Show(
                "Incompatible File Version",
                $"Cannot Open This File\n\n" +
                $"The company '{company}' was created with a newer version of Argo Books than what you currently have installed.\n\n" +

                $"📄 File was saved with: Version {fileVersion}\n" +
                $"💻 You currently have: Version {currentVersion}\n\n" +

                $"What does this mean?\n" +
                $"• Opening it could possibly damage or corrupt your data\n" +
                $"• Your file is completely safe - it just needs a newer version to open\n\n" +

                $"What should you do?\n" +
                $"• Download and install the latest version of Argo Books\n" +
                $"• Then try opening this file again\n" +

                $"The application will now close as a safety precaution to protect your data.",
                CustomMessageBoxIcon.Exclamation,
                CustomMessageBoxButtons.Ok
            );

            Log.Write(1, $"File version incompatibility detected. File: {filePath}, FileVersion: {fileVersion}, CurrentVersion: {currentVersion}");
        }
    }
}
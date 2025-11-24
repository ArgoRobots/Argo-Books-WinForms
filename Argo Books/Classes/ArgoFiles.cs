using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Argo_Books.Classes
{
    /// <summary>
    /// Handles file associations and icon registration for Argo Books file types.
    /// </summary>
    internal partial class ArgoFiles
    {
        public static readonly string
            ArgoCompanyFileExtension = ".ArgoSales",
            ArgoTemplateFileExtension = ".ArgoSalesTemplate",
            TxtFileExtension = ".txt",
            JsonFileExtension = ".json",
            XlsxFileExtension = ".xlsx",
            PngFileExtension = ".png",
            ZipExtension = ".zip";

        /// <summary>
        /// Import for the Windows Shell32 API function to notify the system of association changes.
        /// </summary>
        [LibraryImport("shell32.dll", SetLastError = true)]
        private static partial void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        // Shell change notification constants
        const uint SHCNE_ASSOCCHANGED = 0x8000000;  // Notifies system of association change
        const uint SHCNF_IDLIST = 0x0;              // No additional flags needed

        /// <summary>
        /// Registers a file extension with Windows and associates it with an icon and the current application.
        /// </summary>
        public static void RegisterFileIcon(string extension, Icon icon, int iconIndex)
        {
            if (icon != null)
            {
                // Create a persistent copy of the icon in local app data
                string tempIconPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "ArgoSalesTracker",
                    $"{extension.Replace(".", "")}.ico"
                );

                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(tempIconPath));

                // Save a persistent copy of the icon
                using (FileStream fs = new(tempIconPath, FileMode.Create))
                {
                    icon.Save(fs);
                }

                // Generate a unique class name for this file type
                string className = $"ArgoSalesTracker{extension.Replace(".", "")}";

                // Registry path for user-specific file associations
                string userClassesRoot = @"Software\Classes";

                // Create file extension association
                using (RegistryKey extensionKey = Registry.CurrentUser.CreateSubKey($@"{userClassesRoot}\{extension}"))
                {
                    extensionKey.SetValue("", className);
                }

                // Create file type information
                using (RegistryKey classKey = Registry.CurrentUser.CreateSubKey($@"{userClassesRoot}\{className}"))
                {
                    classKey.SetValue("", "Argo Books File");
                }

                // Associate icon with file type
                using (RegistryKey defaultIconKey = Registry.CurrentUser.CreateSubKey($@"{userClassesRoot}\{className}\DefaultIcon"))
                {
                    defaultIconKey.SetValue("", $"{tempIconPath},{iconIndex}");
                }

                // Set up command to open files with this application
                using (RegistryKey commandKey = Registry.CurrentUser.CreateSubKey($@"{userClassesRoot}\{className}\shell\open\command"))
                {
                    commandKey.SetValue("", $"\"{Application.ExecutablePath}\" \"%1\"");
                }

                // Notify Windows to refresh icon cache and file associations
                SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
            }
        }
    }
}
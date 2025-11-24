using Argo_Books;
using Argo_Books.Classes;
using System.Diagnostics;
using System.Reflection;

namespace Argo_Books.Classes
{
    /// <summary>
    /// Provides a collection of utility methods for various formatting, file management, and input validation tasks.
    /// </summary>
    public static class Tools
    {
        // Formatting
        /// <summary>
        /// Formats a DateTime into a readable time format (HH:mm:ss.ff).
        /// </summary>
        public static string FormatTime(DateTime dateTime) => dateTime.ToString("HH:mm:ss.ff");

        /// <summary>
        /// Formats a DateTime into a readable date format (yyyy-MM-dd).
        /// </summary>
        public static string FormatDate(DateTime dateTime) => dateTime.ToString("yyyy-MM-dd");

        /// <summary>
        /// Formats a DateTime into a readable date and time format (yyyy-MM-dd HH:mm:ss.ff).
        /// </summary>
        public static string FormatDateTime(DateTime dateTime) => dateTime.ToString("yyyy-MM-dd HH:mm:ss.ff");

        /// <summary>
        /// Formats milliseconds into a readable duration string
        /// </summary>
        public static string FormatDuration(long milliseconds)
        {
            TimeSpan timespan = TimeSpan.FromMilliseconds(milliseconds);

            // For durations less than 1 second
            if (timespan.TotalSeconds < 1)
            {
                return $"{milliseconds}ms";
            }

            // For durations less than 1 minute
            if (timespan.TotalMinutes < 1)
            {
                return $"{timespan.TotalSeconds:F2}s";
            }

            // For durations less than 1 hour
            if (timespan.TotalHours < 1)
            {
                return $"{timespan.Minutes}m {timespan.Seconds}s";
            }

            // For durations of 1 hour or more
            int hours = (int)timespan.TotalHours;
            return $"{hours}h {timespan.Minutes}m {timespan.Seconds}s";
        }
        public static DateTime ParseDateOrToday(string dateValue)
        {
            return DateTime.TryParse(dateValue, out DateTime parsedDate)
                ? parsedDate
                : DateTime.Today;
        }

        // General
        /// <summary>
        /// Opens the specified URL in the default web browser.
        /// </summary>
        public static void OpenLink(string URL)
        {
            if (string.IsNullOrEmpty(URL))
            {
                Log.Write(0, "URL is empty");
                return;
            }

            Process.Start(new ProcessStartInfo(URL) { UseShellExecute = true });
        }

        /// <summary>
        /// Retrieves the version number of the currently executing assembly.
        /// </summary>
        public static string GetVersionNumber()
        {
            Version? version = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        /// <summary>
        /// Opens Windows File Explorer and selects the specified file in its folder.
        /// </summary>
        public static void ShowFileInFolder(string filePath)
        {
            Process.Start("explorer.exe", $"/select,\"{filePath}\"");
        }

        /// <summary>
        /// Determines if the application is running in Visual Studio development environment.
        /// </summary>
        public static bool IsRunningInVisualStudio()
        {
            // Check for Visual Studio specific environment variables
            return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("VisualStudioVersion")) ||
                   !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("VSAPPIDDIR")) ||
                   Debugger.IsAttached;
        }

        // Strings
        public static string AddNumberForAStringThatAlreadyExists(string name, List<string> list)
        {
            name = RemoveNumAfterString(name);

            // Use HashSet for faster lookups
            HashSet<string> existingNames = [.. list];

            int count = 2;
            string newName;

            while (existingNames.Contains(newName = $"{name} ({count})"))
            {
                count++;
            }

            return newName;
        }

        /// <summary>
        /// Check if the string has " (num)" at the end and remove it.
        /// </summary>
        public static string RemoveNumAfterString(string name)
        {
            if (name.EndsWith(')'))
            {
                int lastOpenParenthesisIndex = name.LastIndexOf('(');
                if (lastOpenParenthesisIndex > 0
                    && int.TryParse(name.AsSpan(lastOpenParenthesisIndex + 1, name.Length - lastOpenParenthesisIndex - 2), out _))
                {
                    return name[..lastOpenParenthesisIndex].TrimEnd();  // Remove the " (num)"
                }
            }
            return name;
        }

        /// <summary>
        /// Truncates text and adds ellipsis (...) to fit within the specified width for controls that don't support AutoEllipsis.
        /// For labels, consider using the AutoEllipsis property instead as it's more efficient and handles resizing automatically.
        /// </summary>
        public static string AddEllipsisToString(string text, Font font, int availableWidth)
        {
            Size textSize = TextRenderer.MeasureText(text, font);

            // If text fits, return it as-is
            if (textSize.Width <= availableWidth)
            {
                return text;
            }

            // Binary search for the optimal truncation point
            int low = 0;
            int high = text.Length;
            const string ellipsis = "...";

            while (low < high)
            {
                int mid = (low + high + 1) / 2;
                string truncated = text[..mid] + ellipsis;
                Size truncatedSize = TextRenderer.MeasureText(truncated, font);

                if (truncatedSize.Width <= availableWidth)
                {
                    low = mid;
                }
                else
                {
                    high = mid - 1;
                }
            }

            return text[..low] + ellipsis;
        }

        // Bytes
        /// <summary>
        /// Converts a size in bytes to a human-readable string with appropriate units (Bytes, KB, MB, GB, or TB).
        /// </summary>
        /// <returns>
        /// A string representing the size converted to the largest possible unit that results in a value greater than or equal to 1,
        /// formatted to two decimal places.
        /// </returns>
        public static string ConvertBytesToReadableSize(long bytes)
        {
            string[] sizes = ["Bytes", "KB", "MB", "GB", "TB"];
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }

        // Forms
        /// <summary>
        /// Returns true if a form of a specified type is already open.
        /// </summary>
        public static bool IsFormOpen<T>() where T : Form
        {
            return Application.OpenForms.OfType<T>().Any();
        }

        /// <summary>
        /// Opens a form or brings it to the front if it's already open. 
        /// Ensures only one instance of each form type is displayed at a time.
        /// </summary>
        public static void OpenForm(Form form)
        {
            Form existingForm = Application.OpenForms.OfType<Form>()
                .FirstOrDefault(f => f.GetType() == form.GetType());

            if (existingForm != null)
            {
                existingForm.BringToFront();
            }
            else
            {
                // Set the Owner property to ensure the new form stays on top of the current form
                // without using TopMost. This creates a parent-child relationship between forms
                form.Owner = MainMenu_Form.Instance;
                form.Show();
            }
        }
        public static void CloseOpenForm<T>() where T : Form
        {
            Application.OpenForms.OfType<T>().FirstOrDefault()?.Close();
        }

        /// <summary>
        /// Close all open forms except for MainMenu_Form.
        /// </summary>
        public static void CloseAllOpenForms()
        {
            List<Form> formsToClose = Application.OpenForms.OfType<Form>()
               .Where(form => form != MainMenu_Form.Instance &&
                              !form.Text.Contains("DateTimeDialog"))  // Exclude these because Guna2DateTimePickers use Forms
               .ToList();

            foreach (Form form in formsToClose)
            {
                form.Close();
            }
        }

        /// <summary>
        /// Forces handle creation for a control if it hasn't been created yet.
        /// </summary>
        public static void EnsureHandleCreated(Control control)
        {
            if (!control.IsHandleCreated)
            {
                nint _ = control.Handle;  // Accessing Handle property forces creation
            }
        }
    }
}
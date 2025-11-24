using Argo_Books.Classes;
using Argo_Books.Language;

namespace Argo_Books.Classes
{
    /// <summary>
    /// Represents a single log entry with template and arguments for translation support.
    /// </summary>
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogCategory Category { get; set; }
        public string Template { get; set; }  // Original English template with placeholders
        public object[] Arguments { get; set; }  // Arguments for string.Format

        public string GetFormattedMessage(string languageCode = null)
        {
            languageCode ??= LanguageManager.GetDefaultLanguageAbbreviation() ?? "en";

            if (languageCode == "en")
            {
                // For English, use the original template
                return Arguments?.Length > 0 ? string.Format(Template, Arguments) : Template;
            }

            // For other languages, translate the template first
            string translatedTemplate = LanguageManager.TranslateString(Template);
            return Arguments?.Length > 0 ? string.Format(translatedTemplate, Arguments) : translatedTemplate;
        }
        public string GetDisplayText(string languageCode = null)
        {
            string timestamp = "<" + Tools.FormatTime(Timestamp) + "> ";
            string categoryText = GetTranslatedCategory(Category, languageCode);
            string message = GetFormattedMessage(languageCode);

            return timestamp + categoryText + " " + message + "\n";
        }
        private static string GetTranslatedCategory(LogCategory category, string languageCode = null)
        {
            languageCode ??= LanguageManager.GetDefaultLanguageAbbreviation() ?? "en";

            if (languageCode == "en")
            {
                return Log.GetEnglishFormattedCategory(category);
            }
            else
            {
                return Log.GetTranslatedFormattedCategory(category);
            }
        }
    }
}
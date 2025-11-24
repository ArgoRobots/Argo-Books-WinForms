namespace Argo_Books.Language
{
    /// <summary>
    /// Result of translation generation operation.
    /// </summary>
    public class TranslationResult
    {
        public bool Success { get; set; }
        public int TotalSourceTexts { get; set; }
        public int TotalNewTranslations { get; set; }
        public int LanguagesProcessed { get; set; }
        public Dictionary<string, int> TranslationsByLanguage { get; set; } = [];
    }
}
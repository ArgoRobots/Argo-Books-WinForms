using Argo_Books.Language;

namespace Argo_Books.ReturnProduct
{
    public static class ReturnReasons
    {
        public static List<string> GetReturnReasons() => [
            LanguageManager.TranslateString("Defective/Damaged Product"),
            LanguageManager.TranslateString("Wrong Item Received"),
            LanguageManager.TranslateString("Customer Changed Mind"),
            LanguageManager.TranslateString("Quality Issues"),
            LanguageManager.TranslateString("Product Not as Described"),
            LanguageManager.TranslateString("Duplicate Order"),
            LanguageManager.TranslateString("Expired Product"),
            LanguageManager.TranslateString("Missing Parts/Accessories"),
            LanguageManager.TranslateString("Size/Fit Issues"),
            LanguageManager.TranslateString("Other")
        ];
    }
}
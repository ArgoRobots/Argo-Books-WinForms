using Argo_Books.Language;

namespace Argo_Books.LostProduct
{
    public static class LostReasons
    {
        public static List<string> GetPurchaseLostReasons() =>
        [
            LanguageManager.TranslateString("Lost/Misplaced"),
            LanguageManager.TranslateString("Stolen/Theft"),
            LanguageManager.TranslateString("Damaged Beyond Repair"),
            LanguageManager.TranslateString("Defective Product"),
            LanguageManager.TranslateString("Shipping Incident"),
            LanguageManager.TranslateString("Fire/Natural Disaster"),
            LanguageManager.TranslateString("Expired/Spoiled"),
            LanguageManager.TranslateString("Equipment Failure"),
            LanguageManager.TranslateString("Human Error"),
            LanguageManager.TranslateString("Inventory Shrinkage"),
            LanguageManager.TranslateString("Accidental Disposal"),
            LanguageManager.TranslateString("Other")
        ];

        public static List<string> GetSalesLostReasons() =>
        [
            LanguageManager.TranslateString("Lost in Transit"),
            LanguageManager.TranslateString("Damaged During Shipping"),
            LanguageManager.TranslateString("Customer Rejection/Refusal"),
            LanguageManager.TranslateString("Delivery Failure"),
            LanguageManager.TranslateString("Lost by Carrier/Logistics"),
            LanguageManager.TranslateString("Warehouse Loss Before Shipping"),
            LanguageManager.TranslateString("Quality Control Rejection"),
            LanguageManager.TranslateString("Address/Delivery Issues"),
            LanguageManager.TranslateString("Customs/Border Issues"),
            LanguageManager.TranslateString("Weather/Natural Disaster"),
            LanguageManager.TranslateString("Packaging Failure"),
            LanguageManager.TranslateString("Other")
        ];
    }
}
using Argo_Books;

namespace Argo_Books.DataClasses
{
    /// <summary>
    /// Contains a set of read-only variables used for various UI and data-related text and formatting values.
    /// </summary>
    public static class ReadOnlyVariables
    {
        public static string EmptyCell => "-";
        public static string MultipleItems_text => "Multiple items";
        public static string Receipt_text => "receipt:";
        public static string Show_text => "show";
        public static string CompanyName_text => "CompanyName";
        public static byte OffsetRightClickPanel => 30;
        public static byte PaddingRightClickPanel => 5;

        public static readonly string
            ID_column = MainMenu_Form.Column.ID.ToString(),
            Accountant_column = MainMenu_Form.Column.Accountant.ToString(),
            Product_column = MainMenu_Form.Column.Product.ToString(),
            Category_column = MainMenu_Form.Column.Category.ToString(),
            Country_column = MainMenu_Form.Column.Country.ToString(),
            Company_column = MainMenu_Form.Column.Company.ToString(),
            Date_column = MainMenu_Form.Column.Date.ToString(),
            TotalItems_column = MainMenu_Form.Column.TotalItems.ToString(),
            PricePerUnit_column = MainMenu_Form.Column.PricePerUnit.ToString(),
            Shipping_column = MainMenu_Form.Column.Shipping.ToString(),
            Tax_column = MainMenu_Form.Column.Tax.ToString(),
            Fee_column = MainMenu_Form.Column.Fee.ToString(),
            Discount_column = MainMenu_Form.Column.Discount.ToString(),
            ChargedDifference_column = MainMenu_Form.Column.ChargedDifference.ToString(),
            Total_column = MainMenu_Form.Column.Total.ToString(),
            Note_column = MainMenu_Form.Column.Note.ToString(),
            HasReceipt_column = MainMenu_Form.Column.HasReceipt.ToString();
    }
}
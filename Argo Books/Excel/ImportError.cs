namespace Argo_Books.Excel
{
    /// <summary>
    /// Data container for tracking import errors with transaction details, field names, and invalid values.
    /// </summary>
    public class ImportError
    {
        public string TransactionId { get; set; }
        public string FieldName { get; set; }
        public string InvalidValue { get; set; }
        public int RowNumber { get; set; }
        public string WorksheetName { get; set; }
    }
}
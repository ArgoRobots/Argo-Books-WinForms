namespace Argo_Books.Excel
{
    /// <summary>
    /// Contains statistics and results from import operations including counts, errors, and cancellation status.
    /// </summary>
    public class ImportSummary
    {
        public int AccountantsImported { get; set; }
        public int CompaniesImported { get; set; }
        public int PurchaseProductsImported { get; set; }
        public int SaleProductsImported { get; set; }
        public int PurchaseTransactionsImported { get; set; }
        public int SaleTransactionsImported { get; set; }
        public int ReceiptsImported { get; set; }
        public int SkippedRows { get; set; }
        public int ItemRowsProcessed { get; set; }
        public List<ImportError> Errors { get; set; } = [];
        public bool WasCancelled { get; set; }

        public int TotalSuccessfulImports => AccountantsImported + CompaniesImported +
            PurchaseProductsImported + SaleProductsImported + PurchaseTransactionsImported +
            SaleTransactionsImported + ReceiptsImported;

        public bool HasAnyImports => TotalSuccessfulImports > 0;
    }
}
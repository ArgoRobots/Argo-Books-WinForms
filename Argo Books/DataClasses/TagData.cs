namespace Argo_Books.DataClasses
{
    /// <summary>
    /// Represents cost-related data associated with a transaction.
    /// This object is stored in the Tag property of each row in the DataGridView.
    /// USD values are used to convert to other currencies, because it's the world reserve currency.
    /// </summary>
    public class TagData
    {
        // USD finance related properties
        public decimal PricePerUnitUSD { get; set; }
        public decimal ShippingUSD { get; set; }
        public decimal TaxUSD { get; set; }
        public decimal FeeUSD { get; set; }
        public decimal DiscountUSD { get; set; }
        public decimal ChargedDifferenceUSD { get; set; }
        public decimal ChargedOrCreditedUSD { get; set; }
        public string OriginalCurrency { get; set; }

        // Finance related properties
        public decimal OriginalPricePerUnit { get; set; }
        public decimal OriginalShipping { get; set; }
        public decimal OriginalTax { get; set; }
        public decimal OriginalFee { get; set; }
        public decimal OriginalDiscount { get; set; }
        public decimal OriginalChargedDifference { get; set; }
        public decimal OriginalChargedOrCredited { get; set; }

        // Return related properties
        public bool IsReturned { get; set; } = false;
        public bool IsPartiallyReturned { get; set; } = false;
        public DateTime? ReturnDate { get; set; }
        public string ReturnReason { get; set; }
        public string ReturnedBy { get; set; }
        public List<int> ReturnedItems { get; set; }
        public string ReturnedItemsText { get; set; }

        // Loss related properties
        public bool IsLost { get; set; } = false;
        public bool IsPartiallyLost { get; set; } = false;
        public DateTime? LostDate { get; set; }
        public string LostReason { get; set; }
        public string LostBy { get; set; }
        public List<int> LostItems { get; set; }
        public string LostItemsText { get; set; }

        // Rental related properties
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string RentalRecordID { get; set; }
    }
}
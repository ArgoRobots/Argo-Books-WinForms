namespace Sales_Tracker.Rentals
{
    /// <summary>
    /// Defines the rental rate period types.
    /// </summary>
    public enum RentalRateType
    {
        Daily,
        Weekly,
        Monthly
    }

    /// <summary>
    /// Represents a single rental transaction for a customer.
    /// </summary>
    public class RentalRecord
    {
        public string RentalRecordID { get; set; }
        public string CustomerID { get; set; }
        public string RentalItemID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public RentalRateType RateType { get; set; }
        public decimal Rate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal SecurityDeposit { get; set; }
        public decimal TotalCost { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Tax { get; set; }
        public decimal Fee { get; set; }
        public decimal Shipping { get; set; }
        public decimal Discount { get; set; }
        public string Notes { get; set; } = "";
        public bool IsActive { get; set; } = true;
        public bool IsOverdue { get; set; }

        public RentalRecord()
        {
            RentalRecordID = Guid.NewGuid().ToString();
            StartDate = DateTime.Now;
        }

        public RentalRecord(
            string customerID,
            string rentalItemID,
            string productName,
            int quantity,
            RentalRateType rateType,
            decimal rate,
            DateTime startDate,
            decimal securityDeposit,
            string notes = "")
        {
            RentalRecordID = Guid.NewGuid().ToString();
            CustomerID = customerID;
            RentalItemID = rentalItemID;
            ProductName = productName;
            Quantity = quantity;
            RateType = rateType;
            Rate = rate;
            StartDate = startDate;
            SecurityDeposit = securityDeposit;
            Notes = notes;

            // Calculate total cost (rate * quantity + deposit)
            TotalCost = rate * quantity + securityDeposit;
            AmountPaid = 0;

            // Set due date based on rate type
            DueDate = rateType switch
            {
                RentalRateType.Daily => startDate.AddDays(1),
                RentalRateType.Weekly => startDate.AddDays(7),
                RentalRateType.Monthly => startDate.AddMonths(1),
                _ => startDate.AddDays(1)
            };
        }

        /// <summary>
        /// Gets the formatted rental rate display (e.g., "$25.00/day").
        /// </summary>
        public string FormattedRateDisplay
        {
            get
            {
                string period = RateType switch
                {
                    RentalRateType.Daily => "day",
                    RentalRateType.Weekly => "week",
                    RentalRateType.Monthly => "month",
                    _ => "day"
                };
                return $"{MainMenu_Form.CurrencySymbol}{Rate:N2}/{period}";
            }
        }

        /// <summary>
        /// Gets the remaining balance (total cost - amount paid).
        /// </summary>
        public decimal RemainingBalance => TotalCost - AmountPaid;

        /// <summary>
        /// Checks if the rental is overdue.
        /// </summary>
        public void CheckOverdueStatus()
        {
            IsOverdue = DueDate.HasValue && DateTime.Now > DueDate.Value && IsActive;
        }

        /// <summary>
        /// Marks the rental as returned.
        /// </summary>
        public void MarkAsReturned()
        {
            ReturnDate = DateTime.Now;
            IsActive = false;
            IsOverdue = false;
        }

        /// <summary>
        /// Records a payment for this rental.
        /// </summary>
        public void RecordPayment(decimal amount)
        {
            AmountPaid += amount;
        }
    }
}
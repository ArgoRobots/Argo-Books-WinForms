using Sales_Tracker.Rentals;

namespace Sales_Tracker.DataClasses
{
    /// <summary>
    /// Represents a customer with contact information, rental history, payment status, and notes.
    /// </summary>
    public class Customer
    {
        // Getters and setters
        public string CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public bool IsBanned { get; set; } = false;
        public string BanReason { get; set; } = "";
        public DateTime? BanDate { get; set; } = null;
        public string Notes { get; set; } = "";
        public List<RentalRecord> RentalRecords { get; set; } = [];
        public PaymentStatus CurrentPaymentStatus { get; set; } = PaymentStatus.Current;
        public decimal OutstandingBalance { get; set; } = 0m;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? LastRentalDate { get; set; } = null;

        public enum PaymentStatus
        {
            Current,
            Overdue,
            Delinquent
        }

        // Default constructor required for deserialization
        public Customer() { }

        public Customer(string customerID, string firstName, string lastName, string email, string phoneNumber, string address, string notes)
        {
            CustomerID = customerID;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            Notes = notes;
        }

        /// <summary>
        /// Adds a rental record to the customer's rental history.
        /// </summary>
        public void AddRentalRecord(RentalRecord record)
        {
            RentalRecords.Add(record);
            LastRentalDate = record.StartDate;
            UpdateOutstandingBalance();
        }

        /// <summary>
        /// Bans the customer with a specified reason.
        /// </summary>
        public void Ban(string reason)
        {
            IsBanned = true;
            BanReason = reason;
            BanDate = DateTime.Now;
        }

        /// <summary>
        /// Unbans the customer.
        /// </summary>
        public void Unban()
        {
            IsBanned = false;
            BanReason = "";
            BanDate = null;
        }

        /// <summary>
        /// Updates the customer's payment status based on outstanding balance and overdue rentals.
        /// </summary>
        public void UpdatePaymentStatus()
        {
            // Update overdue status for all active rentals
            foreach (RentalRecord record in RentalRecords.Where(r => r.IsActive))
            {
                record.CheckOverdueStatus();
            }

            UpdateOutstandingBalance();

            if (OutstandingBalance <= 0)
            {
                CurrentPaymentStatus = PaymentStatus.Current;
            }
            else
            {
                // Check if any rental is overdue
                bool hasOverdueRentals = RentalRecords.Any(r => r.IsOverdue);

                // Check if severely overdue (more than 30 days)
                bool hasSeverelyOverdueRentals = RentalRecords.Any(r =>
                    r.IsOverdue &&
                    r.DueDate.HasValue &&
                    (DateTime.Now - r.DueDate.Value).TotalDays > 30);

                if (hasSeverelyOverdueRentals)
                {
                    CurrentPaymentStatus = PaymentStatus.Delinquent;
                }
                else if (hasOverdueRentals)
                {
                    CurrentPaymentStatus = PaymentStatus.Overdue;
                }
                else
                {
                    CurrentPaymentStatus = PaymentStatus.Current;
                }
            }
        }

        /// <summary>
        /// Updates the outstanding balance based on active rental records.
        /// </summary>
        private void UpdateOutstandingBalance()
        {
            OutstandingBalance = RentalRecords
                .Where(r => r.IsActive)
                .Sum(r => r.RemainingBalance);
        }

        /// <summary>
        /// Gets the total outstanding rental balance across all active rentals.
        /// </summary>
        public decimal GetTotalOutstandingRentalBalance()
        {
            return RentalRecords.Where(r => r.IsActive).Sum(r => r.RemainingBalance);
        }

        /// <summary>
        /// Gets all active rental records for this customer.
        /// </summary>
        public List<RentalRecord> GetActiveRentals()
        {
            return RentalRecords.Where(r => r.IsActive).ToList();
        }

        /// <summary>
        /// Gets all overdue rental records for this customer.
        /// </summary>
        public List<RentalRecord> GetOverdueRentals()
        {
            return RentalRecords.Where(r => r.IsOverdue).ToList();
        }

        /// <summary>
        /// Gets all completed (returned) rental records for this customer.
        /// </summary>
        public List<RentalRecord> GetCompletedRentals()
        {
            return RentalRecords.Where(r => !r.IsActive && r.ReturnDate.HasValue).ToList();
        }

        /// <summary>
        /// Records a payment for a specific rental.
        /// </summary>
        public void RecordPayment(string rentalRecordID, decimal amount)
        {
            RentalRecord record = RentalRecords.FirstOrDefault(r => r.RentalRecordID == rentalRecordID);
            if (record != null)
            {
                record.RecordPayment(amount);
                UpdatePaymentStatus();
            }
        }

        /// <summary>
        /// Marks a specific rental as returned.
        /// </summary>
        public void ReturnRental(string rentalRecordID)
        {
            RentalRecord record = RentalRecords.FirstOrDefault(r => r.RentalRecordID == rentalRecordID);
            if (record != null)
            {
                record.MarkAsReturned();
                UpdatePaymentStatus();
            }
        }

        /// <summary>
        /// Gets a summary of the customer's rental statistics.
        /// </summary>
        public string GetRentalSummary()
        {
            int totalRentals = RentalRecords.Count;
            int activeRentals = GetActiveRentals().Count;
            int overdueRentals = GetOverdueRentals().Count;
            int completedRentals = GetCompletedRentals().Count;

            return $"Total: {totalRentals} | Active: {activeRentals} | Overdue: {overdueRentals} | Completed: {completedRentals}";
        }
    }
}
namespace Argo_Books.Rentals
{
    /// <summary>
    /// Represents a rental inventory item with availability tracking and quantity management.
    /// </summary>
    public class RentalItem
    {
        public enum AvailabilityStatus
        {
            Available,
            Rented,
            Maintenance,
            Retired
        }

        // Getters and setters
        public string RentalItemID { get; set; }
        public string ProductName { get; set; }
        public string CompanyName { get; set; }
        public AvailabilityStatus Status { get; set; } = AvailabilityStatus.Available;
        public int TotalQuantity { get; set; }
        public int QuantityRented { get; set; }
        public int QuantityInMaintenance { get; set; }
        public decimal DailyRate { get; set; }
        public decimal? WeeklyRate { get; set; }
        public decimal? MonthlyRate { get; set; }
        public decimal SecurityDeposit { get; set; }
        public string Notes { get; set; } = "";
        public DateTime DateAdded { get; set; }
        public DateTime? LastStatusUpdate { get; set; }
        public DateTime? LastRentalDate { get; set; }
        public string CurrentRenterID { get; set; } = "";
        public List<RentalRecord> RentalRecords { get; set; } = [];

        /// <summary>
        /// Number of units currently available for rent.
        /// </summary>
        public int QuantityAvailable => TotalQuantity - QuantityRented - QuantityInMaintenance;

        // Init.
        public RentalItem()
        {
            DateAdded = DateTime.Now;
        }

        // Methods
        /// <summary>
        /// Updates the status of the rental item and records the timestamp.
        /// </summary>
        public void UpdateStatus(AvailabilityStatus newStatus)
        {
            Status = newStatus;
            LastStatusUpdate = DateTime.Now;
        }

        /// <summary>
        /// Rents out a specified quantity of this item.
        /// </summary>
        public bool RentOut(int quantity, string customerID)
        {
            if (QuantityAvailable >= quantity)
            {
                QuantityRented += quantity;
                CurrentRenterID = customerID;
                LastRentalDate = DateTime.Now;

                // Update status if all units are now rented
                if (QuantityAvailable == 0)
                {
                    UpdateStatus(AvailabilityStatus.Rented);
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a specified quantity of this item.
        /// </summary>
        public bool ReturnItem(int quantity)
        {
            if (QuantityRented >= quantity)
            {
                QuantityRented -= quantity;

                // Update status if units are now available
                if (QuantityAvailable > 0 && Status == AvailabilityStatus.Rented)
                {
                    UpdateStatus(AvailabilityStatus.Available);
                }

                if (QuantityRented == 0)
                {
                    CurrentRenterID = "";
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Moves a specified quantity to maintenance.
        /// </summary>
        public bool MoveToMaintenance(int quantity)
        {
            if (QuantityAvailable >= quantity)
            {
                QuantityInMaintenance += quantity;
                UpdateStatus(AvailabilityStatus.Maintenance);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Moves a specified quantity out of maintenance.
        /// </summary>
        public bool ReturnFromMaintenance(int quantity)
        {
            if (QuantityInMaintenance >= quantity)
            {
                QuantityInMaintenance -= quantity;

                if (QuantityAvailable > 0 && QuantityInMaintenance == 0)
                {
                    UpdateStatus(AvailabilityStatus.Available);
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Adjusts the total quantity (for inventory updates).
        /// </summary>
        public void AdjustTotalQuantity(int newTotal)
        {
            TotalQuantity = newTotal;
            LastStatusUpdate = DateTime.Now;
        }
    }
}
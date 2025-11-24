using Argo_Books.Classes;
using Newtonsoft.Json;
using Argo_Books.Classes;

namespace Argo_Books.Rentals
{
    /// <summary>
    /// Manages rental inventory operations including tracking availability and quantities.
    /// </summary>
    public static class RentalInventoryManager
    {
        /// <summary>
        /// Gets the current rental inventory list.
        /// </summary>
        public static List<RentalItem> RentalInventory { get; private set; } = [];

        /// <summary>
        /// Loads rental inventory from file.
        /// </summary>
        public static void LoadInventory()
        {
            try
            {
                if (File.Exists(Directories.RentalInventory_file))
                {
                    string json = File.ReadAllText(Directories.RentalInventory_file);
                    RentalInventory = JsonConvert.DeserializeObject<List<RentalItem>>(json) ?? [];
                }
                else
                {
                    RentalInventory = [];
                }
            }
            catch (Exception ex)
            {
                Log.Error_ReadFile($"Failed to load rental inventory: {ex.Message}");
                RentalInventory = [];
            }
        }

        /// <summary>
        /// Saves rental inventory to file.
        /// </summary>
        public static void SaveInventory()
        {
            try
            {
                string json = JsonConvert.SerializeObject(RentalInventory, Formatting.Indented);
                File.WriteAllText(Directories.RentalInventory_file, json);
            }
            catch (Exception ex)
            {
                Log.Error_WriteToFile($"Failed to save rental inventory: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a new rental item to inventory.
        /// </summary>
        public static bool AddRentalItem(RentalItem item)
        {
            if (RentalInventory.Any(r => r.RentalItemID == item.RentalItemID))
            {
                return false;
            }

            RentalInventory.Add(item);
            SaveInventory();
            return true;
        }

        /// <summary>
        /// Removes a rental item from inventory.
        /// </summary>
        public static bool RemoveRentalItem(string rentalItemID)
        {
            RentalItem? item = RentalInventory.FirstOrDefault(r => r.RentalItemID == rentalItemID);
            if (item != null)
            {
                RentalInventory.Remove(item);
                SaveInventory();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a rental item by ID.
        /// </summary>
        public static RentalItem? GetRentalItem(string rentalItemID)
        {
            return RentalInventory.FirstOrDefault(r => r.RentalItemID == rentalItemID);
        }

        /// <summary>
        /// Gets all available rental items.
        /// </summary>
        public static List<RentalItem> GetAvailableItems()
        {
            return RentalInventory.Where(r => r.QuantityAvailable > 0).ToList();
        }

        /// <summary>
        /// Gets all rented rental items.
        /// </summary>
        public static List<RentalItem> GetRentedItems()
        {
            return RentalInventory.Where(r => r.QuantityRented > 0).ToList();
        }

        /// <summary>
        /// Gets all items in maintenance.
        /// </summary>
        public static List<RentalItem> GetMaintenanceItems()
        {
            return RentalInventory.Where(r => r.QuantityInMaintenance > 0).ToList();
        }

        /// <summary>
        /// Updates rental item status.
        /// </summary>
        public static bool UpdateItemStatus(string rentalItemID, RentalItem.AvailabilityStatus newStatus)
        {
            RentalItem? item = GetRentalItem(rentalItemID);
            if (item != null)
            {
                item.UpdateStatus(newStatus);
                SaveInventory();
                return true;
            }
            return false;
        }
    }
}
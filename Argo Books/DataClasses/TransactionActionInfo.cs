using Argo_Books.Language;

namespace Argo_Books.DataClasses
{
    /// <summary>
    /// Contains information about a return or loss action performed on a transaction.
    /// </summary>
    public class TransactionActionInfo
    {
        public DateTime? ActionDate { get; set; }
        public string Reason { get; set; } = "";
        public string ActionBy { get; set; } = "";
        public List<int> AffectedItems { get; set; } = [];

        /// <summary>
        /// Gets a formatted date string for display purposes.
        /// </summary>
        public string FormattedDate => ActionDate?.ToString("yyyy-MM-dd HH:mm") ?? LanguageManager.TranslateString("Unknown");

        /// <summary>
        /// Gets the reason or a default message if none provided.
        /// </summary>
        public string DisplayReason => string.IsNullOrEmpty(Reason) ? LanguageManager.TranslateString("No reason provided") : Reason;

        /// <summary>
        /// Gets the person who performed the action or "Unknown" if not specified.
        /// </summary>
        public string DisplayActionBy => string.IsNullOrEmpty(ActionBy) ? LanguageManager.TranslateString("Unknown") : ActionBy;

        /// <summary>
        /// Indicates whether specific items are affected (for multi-item transactions).
        /// </summary>
        public bool HasAffectedItems => AffectedItems != null && AffectedItems.Count > 0;
    }

    /// <summary>
    /// Specific implementation for return information.
    /// </summary>
    public class ReturnInfo : TransactionActionInfo
    {
        /// <summary>
        /// Alias for ActionDate for better semantic clarity.
        /// </summary>
        public DateTime? ReturnDate
        {
            get => ActionDate;
            set => ActionDate = value;
        }

        /// <summary>
        /// Alias for Reason for better semantic clarity.
        /// </summary>
        public string ReturnReason
        {
            get => Reason;
            set => Reason = value;
        }

        /// <summary>
        /// Alias for ActionBy for better semantic clarity.
        /// </summary>
        public string ReturnedBy
        {
            get => ActionBy;
            set => ActionBy = value;
        }

        /// <summary>
        /// Alias for AffectedItems for better semantic clarity.
        /// </summary>
        public List<int> ReturnedItems
        {
            get => AffectedItems;
            set => AffectedItems = value;
        }
    }

    /// <summary>
    /// Specific implementation for loss information.
    /// </summary>
    public class LossInfo : TransactionActionInfo
    {
        /// <summary>
        /// Alias for ActionDate for better semantic clarity.
        /// </summary>
        public DateTime? LostDate
        {
            get => ActionDate;
            set => ActionDate = value;
        }

        /// <summary>
        /// Alias for Reason for better semantic clarity.
        /// </summary>
        public string LostReason
        {
            get => Reason;
            set => Reason = value;
        }

        /// <summary>
        /// Alias for ActionBy for better semantic clarity.
        /// </summary>
        public string LostBy
        {
            get => ActionBy;
            set => ActionBy = value;
        }

        /// <summary>
        /// Alias for AffectedItems for better semantic clarity.
        /// </summary>
        public List<int> LostItems
        {
            get => AffectedItems;
            set => AffectedItems = value;
        }
    }
}
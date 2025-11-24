namespace Argo_Books.GridView
{
    /// <summary>
    /// Configuration options for DataGridView search operations.
    /// </summary>
    public sealed class SearchOptions
    {
        // Properties
        private readonly StringComparison _stringComparison;
        private readonly byte _maxLevenshteinDistance;

        public SearchOptions()
        {
            _stringComparison = StringComparison.OrdinalIgnoreCase;
            _maxLevenshteinDistance = 2;
        }
        public StringComparison GetStringComparison() => _stringComparison;
        public int GetMaxLevenshteinDistance() => _maxLevenshteinDistance;
    }
}
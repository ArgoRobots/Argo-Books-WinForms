namespace Argo_Books.DataClasses
{
    /// <summary>
    /// Represents a search result with a name, flag image, and score.
    /// </summary>
    public class SearchResult(string name, Image flag, int score)
    {
        public string Name { get; set; } = name;
        public string DisplayName { get; set; } = name;  // For when it's translated into other languages
        public Image Flag { get; set; } = flag;
        public int Score { get; set; } = score;
    }
}
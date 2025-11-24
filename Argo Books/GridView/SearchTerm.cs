namespace Argo_Books.GridView
{
    /// <summary>
    /// Represents a search term with support for exact phrases, exclusions, and required matching.
    /// </summary>
    public sealed class SearchTerm
    {
        private readonly SearchTermType _type;

        private SearchTerm(string text, SearchTermType type)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Search term cannot be empty or whitespace.", nameof(text));
            }

            if (type.HasFlag(SearchTermType.Exclusion) && type.HasFlag(SearchTermType.Required))
            {
                throw new ArgumentException("Search term cannot be both excluded and required.");
            }

            Text = text.Trim();
            _type = type;
        }

        // Getters
        public string Text { get; }
        public bool IsExactPhrase => _type.HasFlag(SearchTermType.ExactPhrase);
        public bool IsExclusion => _type.HasFlag(SearchTermType.Exclusion);
        public bool IsRequired => _type.HasFlag(SearchTermType.Required);

        public static SearchTerm Create(string text, bool isExactPhrase = false, bool isExclusion = false, bool isRequired = false)
        {
            SearchTermType type = SearchTermType.Normal;

            if (isExactPhrase)
            {
                type |= SearchTermType.ExactPhrase;
            }

            if (isExclusion)
            {
                type |= SearchTermType.Exclusion;
            }

            if (isRequired)
            {
                type |= SearchTermType.Required;
            }

            return new SearchTerm(text, type);
        }

        [Flags]
        private enum SearchTermType
        {
            Normal,
            ExactPhrase,
            Exclusion,
            Required
        }
    }
}
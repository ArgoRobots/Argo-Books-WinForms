namespace Argo_Books.GridView
{
    /// <summary>
    /// Represents a structured search term with field-specific targeting and comparison operators.
    /// </summary>
    public class StructuredSearchTerm
    {
        public string Field { get; }
        public string Value { get; }
        public bool IsRequired { get; }
        public bool IsExclusion { get; }
        public string ComparisonOperator { get; }
        public bool HasOrCondition { get; }
        public List<string> OrValues { get; }

        public StructuredSearchTerm(string field, string value, bool isRequired, bool isExclusion, string comparisonOperator)
        {
            Field = field;
            Value = value;
            IsRequired = isRequired;
            IsExclusion = isExclusion;
            ComparisonOperator = comparisonOperator;

            // Check if this is an OR condition (contains pipe character)
            HasOrCondition = value.Contains('|');
            if (HasOrCondition)
            {
                // Split the value by pipe character to get individual values
                OrValues = value.Split('|').Select(v => v.Trim()).Where(v => !string.IsNullOrEmpty(v)).ToList();
            }
            else
            {
                OrValues = [];
            }
        }
    }
}
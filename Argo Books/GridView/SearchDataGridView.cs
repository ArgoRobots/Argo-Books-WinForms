using Argo_Books.Classes;
using Argo_Books.DataClasses;
using Argo_Books.GridView;
using System.Text;
using System.Text.RegularExpressions;

namespace Argo_Books.GridView
{
    /// <summary>
    /// Provides advanced search functionality for DataGridView controls with support for exact phrases,
    /// exclusions, required terms, and fuzzy matching.
    /// </summary>
    public static partial class SearchDataGridView
    {
        /// <summary>
        /// Filters a DataGridView row based on advanced search criteria.
        /// - "exact phrase" for exact phrase matching
        /// - -term for excluding terms
        /// - +term for required terms
        /// - Regular terms for fuzzy matching
        /// </summary>
        /// <returns>True if the row matches the search criteria, otherwise false.</returns>
        public static bool FilterRowByAdvancedSearch(DataGridViewRow row, string searchText, SearchOptions searchOptions = null)
        {
            ArgumentNullException.ThrowIfNull(row);

            if (string.IsNullOrWhiteSpace(searchText))
            {
                return true;
            }

            if (searchText.Contains(':'))
            {
                return FilterRowByStructuredSearch(row, searchText, searchOptions);
            }

            SearchOptions options = searchOptions ?? new SearchOptions();

            List<SearchTerm> searchTerms;
            try
            {
                searchTerms = ParseSearchTerms(searchText);
            }
            catch (ArgumentException)
            {
                // If parsing fails, treat it as no match
                return false;
            }

            List<string> searchableContent = GetSearchableContent(row);

            return searchTerms.All(term =>
            {
                if (term.IsExactPhrase)
                {
                    return searchableContent.Any(content =>
                    {
                        return content.Contains(term.Text, options.GetStringComparison());
                    });
                }

                if (term.IsExclusion)
                {
                    return !searchableContent.Any(content =>
                    {
                        return content.Contains(term.Text, options.GetStringComparison());
                    });
                }

                if (term.IsRequired)
                {
                    return searchableContent.Any(content =>
                    {
                        return content.Contains(term.Text, options.GetStringComparison());
                    });
                }

                return searchableContent.Any(content =>
                {
                    return LevenshteinDistance.IsFuzzyMatch(content, term.Text, options.GetMaxLevenshteinDistance());
                });
            });
        }

        // Normal search
        private static List<SearchTerm> ParseSearchTerms(string searchText)
        {
            List<SearchTerm> terms = [];
            StringBuilder currentTerm = new();
            bool inQuotes = false;

            for (int i = 0; i < searchText.Length; i++)
            {
                char c = searchText[i];
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    if (!inQuotes && currentTerm.Length > 0)
                    {
                        terms.Add(SearchTerm.Create(currentTerm.ToString(), isExactPhrase: true));
                        currentTerm.Clear();
                    }
                }
                else if (!inQuotes && char.IsWhiteSpace(c))
                {
                    if (currentTerm.Length > 0)
                    {
                        AddSearchTerm(terms, currentTerm.ToString());
                        currentTerm.Clear();
                    }
                }
                else
                {
                    currentTerm.Append(c);
                }
            }

            if (currentTerm.Length > 0)
            {
                AddSearchTerm(terms, currentTerm.ToString());
            }

            return terms;
        }
        private static void AddSearchTerm(List<SearchTerm> terms, string term)
        {
            if (term.Length == 1 && (term[0] == '+' || term[0] == '-'))
            {
                return;  // Skip single character modifiers
            }

            try
            {
                if (term.StartsWith('-'))
                {
                    string termText = term[1..];
                    if (!string.IsNullOrWhiteSpace(termText))
                    {
                        terms.Add(SearchTerm.Create(termText, isExclusion: true));
                    }
                }
                else if (term.StartsWith('+'))
                {
                    string termText = term[1..];
                    if (!string.IsNullOrWhiteSpace(termText))
                    {
                        terms.Add(SearchTerm.Create(termText, isRequired: true));
                    }
                }
                else
                {
                    terms.Add(SearchTerm.Create(term));
                }
            }
            catch (ArgumentException)
            {
                // Skip invalid terms
            }
        }
        private static List<string> GetSearchableContent(DataGridViewRow row)
        {
            List<string> content = [];

            // Add visible cell values
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (cell.Value != null)
                {
                    content.Add(cell.Value.ToString());
                }
            }

            // Add tag data if available
            if (row.Tag is (List<string> items, TagData))
            {
                int itemsToTake = items.Count;
                if (items.Count > 0 && items[^1].StartsWith("Receipt:"))
                {
                    itemsToTake--;
                }

                for (int i = 0; i < itemsToTake; i++)
                {
                    string[] parts = items[i].Split(',');
                    for (int j = 0; j < Math.Min(4, parts.Length); j++)
                    {
                        content.Add(parts[j]);
                    }
                }
            }

            return content;
        }

        // AI search
        private static bool FilterRowByStructuredSearch(DataGridViewRow row, string searchText, SearchOptions searchOptions = null)
        {
            List<StructuredSearchTerm> structuredTerms = ParseStructuredSearchTerms(searchText);

            // Parse any remaining free-text terms that aren't in field:value format
            string remainingText = ExtractRemainingText(searchText, structuredTerms);
            List<SearchTerm> freeTextTerms = !string.IsNullOrWhiteSpace(remainingText)
                ? ParseSearchTerms(remainingText)
                : [];

            // If no terms found, consider it a match
            if (structuredTerms.Count == 0 && freeTextTerms.Count == 0)
            {
                return true;
            }

            List<string> searchableContent = GetSearchableContent(row);

            // Check structured field-specific terms
            bool structuredTermsMatch = structuredTerms.All(term =>
            {
                // Match against specific column/field
                DataGridViewColumn column = DataGridViewManager.FindColumnByDisplayName(row.DataGridView, term.Field);
                if (column == null)
                {
                    // Field not found, can't be a match unless it's an exclusion
                    return term.IsExclusion;
                }

                // Get cell value
                object cellValue = row.Cells[column.Index].Value;
                if (cellValue == null)
                {
                    // No value means exclusion matches, required fails
                    return term.IsExclusion;
                }

                string stringValue = cellValue.ToString();

                // Handle OR conditions
                if (term.HasOrCondition)
                {
                    bool anyMatch = term.OrValues.Any(orValue =>
                    {
                        if (!string.IsNullOrEmpty(term.ComparisonOperator))
                        {
                            // Handle numeric comparisons with OR
                            if (IsNumericComparison(stringValue, orValue, term.ComparisonOperator, out bool comparisonResult))
                            {
                                return comparisonResult;
                            }

                            // Handle date comparisons with OR
                            if (IsDateField(column) && IsDateComparison(stringValue, orValue, term.ComparisonOperator, out bool dateComparisonResult))
                            {
                                return dateComparisonResult;
                            }
                        }

                        // Regular string comparison for OR values
                        StringComparison comparison = searchOptions?.GetStringComparison() ?? StringComparison.OrdinalIgnoreCase;
                        return stringValue.Contains(orValue, comparison);
                    });

                    // Flip result if it's an exclusion
                    return term.IsExclusion ? !anyMatch : anyMatch;
                }

                // Handle numeric comparisons
                if (!string.IsNullOrEmpty(term.ComparisonOperator))
                {
                    if (IsNumericComparison(stringValue, term.Value, term.ComparisonOperator, out bool comparisonResult))
                    {
                        // Flip result if it's an exclusion
                        return term.IsExclusion ? !comparisonResult : comparisonResult;
                    }
                }

                // Handle date comparisons
                if (IsDateField(column) && !string.IsNullOrEmpty(term.ComparisonOperator))
                {
                    if (IsDateComparison(stringValue, term.Value, term.ComparisonOperator, out bool dateComparisonResult))
                    {
                        return term.IsExclusion ? !dateComparisonResult : dateComparisonResult;
                    }
                }

                // Handle string matching with exact phrases
                StringComparison comparison = searchOptions?.GetStringComparison() ?? StringComparison.OrdinalIgnoreCase;
                bool isExactMatch = term.Value.StartsWith('\"') && term.Value.EndsWith('\"');

                if (isExactMatch)
                {
                    string exactValue = term.Value.Trim('"');
                    bool exactMatch = stringValue.Equals(exactValue, comparison);
                    return term.IsExclusion ? !exactMatch : exactMatch;
                }

                // Regular string contains
                bool match = stringValue.Contains(term.Value, comparison);
                return term.IsExclusion ? !match : match;
            });

            // Check free text terms against all content
            bool freeTextTermsMatch = freeTextTerms.All(term =>
            {
                if (term.IsExactPhrase)
                {
                    return searchableContent.Any(content =>
                        content.Contains(term.Text, searchOptions?.GetStringComparison() ?? StringComparison.OrdinalIgnoreCase));
                }

                if (term.IsExclusion)
                {
                    return !searchableContent.Any(content =>
                        content.Contains(term.Text, searchOptions?.GetStringComparison() ?? StringComparison.OrdinalIgnoreCase));
                }

                if (term.IsRequired)
                {
                    return searchableContent.Any(content =>
                        content.Contains(term.Text, searchOptions?.GetStringComparison() ?? StringComparison.OrdinalIgnoreCase));
                }

                // Fuzzy match for normal terms
                return searchableContent.Any(content =>
                    LevenshteinDistance.IsFuzzyMatch(content, term.Text, searchOptions?.GetMaxLevenshteinDistance() ?? 2));
            });

            // Both structured and free-text terms must match
            return structuredTermsMatch && freeTextTermsMatch;
        }
        private static string ExtractRemainingText(string searchText, List<StructuredSearchTerm> structuredTerms)
        {
            // Remove all structured terms from the original search text to get remaining free text
            string remainingText = searchText;

            foreach (StructuredSearchTerm term in structuredTerms)
            {
                string valuePattern = term.HasOrCondition
                    ? string.Join("\\|", term.OrValues.Select(v => Regex.Escape(v)))
                    : Regex.Escape(term.Value);

                string termPattern = $"{(term.IsRequired ? "\\+" : "")}{(term.IsExclusion ? "-" : "")}{Regex.Escape(term.Field)}:{Regex.Escape(term.ComparisonOperator)}{valuePattern}";
                remainingText = Regex.Replace(remainingText, termPattern, " ", RegexOptions.IgnoreCase);
            }

            return remainingText.Trim();
        }
        private static bool IsNumericComparison(string cellValue, string compareValue, string comparisonOperator, out bool result)
        {
            result = false;

            // Clean up the comparison value - remove any quote characters
            compareValue = compareValue.Trim('"', '\'');

            if (decimal.TryParse(cellValue, out decimal numericCellValue) &&
                decimal.TryParse(compareValue, out decimal numericCompareValue))
            {
                if (comparisonOperator == ">")
                {
                    result = numericCellValue > numericCompareValue;
                    return true;
                }
                else if (comparisonOperator == "<")
                {
                    result = numericCellValue < numericCompareValue;
                    return true;
                }
                else if (comparisonOperator == "=")
                {
                    result = numericCellValue == numericCompareValue;
                    return true;
                }
            }

            return false;
        }
        private static bool IsDateField(DataGridViewColumn column)
        {
            return column.Name.Equals("Date", StringComparison.OrdinalIgnoreCase) ||
                   column.HeaderText.Equals("Date", StringComparison.OrdinalIgnoreCase);
        }
        private static bool IsDateComparison(string cellValue, string compareValue, string comparisonOperator, out bool result)
        {
            result = false;

            if (DateTime.TryParse(cellValue, out DateTime dateCell))
            {
                // Handle relative date values like "2025-12-01" from AI translations
                if (DateTime.TryParse(compareValue, out DateTime dateCompare))
                {
                    if (comparisonOperator == ">")
                    {
                        result = dateCell > dateCompare;
                        return true;
                    }
                    else if (comparisonOperator == "<")
                    {
                        result = dateCell < dateCompare;
                        return true;
                    }
                }

                // Handle special date keywords
                DateTime now = DateTime.Now;
                if (compareValue.Equals("today", StringComparison.OrdinalIgnoreCase))
                {
                    dateCompare = now.Date;
                }
                else if (compareValue.Equals("yesterday", StringComparison.OrdinalIgnoreCase))
                {
                    dateCompare = now.Date.AddDays(-1);
                }
                else if (compareValue.Equals("this month", StringComparison.OrdinalIgnoreCase) ||
                        compareValue.Equals("thismonth", StringComparison.OrdinalIgnoreCase))
                {
                    dateCompare = new DateTime(now.Year, now.Month, 1);
                }
                else if (compareValue.Equals("last month", StringComparison.OrdinalIgnoreCase) ||
                        compareValue.Equals("lastmonth", StringComparison.OrdinalIgnoreCase))
                {
                    dateCompare = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
                }
                else
                {
                    // Couldn't parse as a date
                    return false;
                }

                if (comparisonOperator == ">")
                {
                    result = dateCell > dateCompare;
                    return true;
                }
                else if (comparisonOperator == "<")
                {
                    result = dateCell < dateCompare;
                    return true;
                }
            }

            return false;
        }
        [GeneratedRegex(@"([\+\-])?(\w+[\s\w]*?):([<>=]*)([^:]+?)(?=\s+[\+\-]?\w+:|\s*$)", RegexOptions.Compiled)]
        private static partial Regex StructuredSearchTermRegex();
        private static List<StructuredSearchTerm> ParseStructuredSearchTerms(string searchText)
        {
            List<StructuredSearchTerm> terms = [];

            // Match patterns like:
            // +Field:Value, -Field:Value, Field:Value
            // Field:>Value, Field:<Value
            // +Field:>Value, -Field:<Value, etc.
            // Field:Value1|Value2|Value3 (OR condition)
            MatchCollection matches = StructuredSearchTermRegex().Matches(searchText);

            foreach (Match match in matches)
            {
                string prefix = match.Groups[1].Value;
                string field = match.Groups[2].Value.Trim();
                string comparisonOp = match.Groups[3].Value;
                string value = match.Groups[4].Value;

                bool isRequired = prefix == "+";
                bool isExclusion = prefix == "-";

                terms.Add(new StructuredSearchTerm(field, value, isRequired, isExclusion, comparisonOp));
            }

            return terms;
        }
    }
}
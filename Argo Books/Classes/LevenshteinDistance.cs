namespace Argo_Books.Classes
{
    /// <summary>
    /// Provides fuzzy string matching using the Levenshtein distance algorithm.
    /// </summary>
    public class LevenshteinDistance
    {
        /// <summary>
        /// Determines if two strings are a fuzzy match within the specified distance threshold.
        /// </summary>
        public static bool IsFuzzyMatch(string source, string target, int maxDistance)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            {
                return false;
            }

            // Normalize strings
            string normalizedSource = source.ToLowerInvariant().Trim();
            string normalizedTarget = target.ToLowerInvariant().Trim();

            // Exact match check
            if (normalizedSource.Contains(normalizedTarget))
            {
                return true;
            }

            // If target is longer than source, no need to check
            if (normalizedTarget.Length > normalizedSource.Length + maxDistance)
            {
                return false;
            }

            // Check each word in source
            string[] sourceWords = normalizedSource.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string[] targetWords = normalizedTarget.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (string targetWord in targetWords)
            {
                bool wordMatched = false;
                foreach (string sourceWord in sourceWords)
                {
                    // Skip words with too much length difference
                    if (Math.Abs(sourceWord.Length - targetWord.Length) > maxDistance)
                    {
                        continue;
                    }

                    double similarity = CalculateNormalizedSimilarity(sourceWord, targetWord);
                    if (similarity >= 0.6)  // Using the threshold of 0.6
                    {
                        wordMatched = true;
                        break;
                    }
                }

                if (!wordMatched)
                {
                    return false;
                }
            }

            return true;
        }
        private static double CalculateNormalizedSimilarity(string source, string target)
        {
            // Handle empty strings
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target) ? 1.0 : 0.0;
            }

            if (string.IsNullOrEmpty(target))
            {
                return 0.0;
            }

            int distance = CalculateLevenshteinDistance(source, target);
            int maxDistance = Math.Max(source.Length, target.Length);

            // Normalize to similarity (1.0 means identical, 0.0 means completely different)
            return 1.0 - ((double)distance / maxDistance);
        }
        private static int CalculateLevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target) ? 0 : target.Length;
            }

            if (string.IsNullOrEmpty(target))
            {
                return source.Length;
            }

            // Create matrix
            int[,] matrix = new int[source.Length + 1, target.Length + 1];

            // Initialize first row and column
            for (int i = 0; i <= source.Length; i++)
            {
                matrix[i, 0] = i;
            }

            for (int j = 0; j <= target.Length; j++)
            {
                matrix[0, j] = j;
            }

            // Fill matrix
            for (int i = 1; i <= source.Length; i++)
            {
                for (int j = 1; j <= target.Length; j++)
                {
                    int substitutionCost = (source[i - 1] == target[j - 1]) ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(
                            matrix[i - 1, j] + 1,   // Deletion
                            matrix[i, j - 1] + 1),  // Insertion
                        matrix[i - 1, j - 1] + substitutionCost  // Substitution
                    );

                    // Transposition check (for swapped characters)
                    if (i > 1 && j > 1 &&
                        source[i - 1] == target[j - 2] &&
                        source[i - 2] == target[j - 1])
                    {
                        matrix[i, j] = Math.Min(matrix[i, j], matrix[i - 2, j - 2] + 1);
                    }
                }
            }

            return matrix[source.Length, target.Length];
        }
    }
}
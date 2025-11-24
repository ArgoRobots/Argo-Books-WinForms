using Argo_Books.DataClasses;
using Newtonsoft.Json;
using Argo_Books.Classes;
using System.Text;

namespace Argo_Books.AISearch
{
    /// <summary>
    /// Translates natural language queries into structured search parameters for the SearchDataGridView functionality.
    /// </summary>
    public class AIQueryTranslator
    {
        // Properties
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiEndpoint;

        // Getter
        public int LastTokenUsage { get; private set; }
        public static string Model { get; private set; } = "gpt-3.5-turbo";

        // Methods
        public AIQueryTranslator(string apiKey, string apiEndpoint = "https://api.openai.com/v1/chat/completions")
        {
            _apiKey = apiKey;
            _apiEndpoint = apiEndpoint;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        /// <summary>
        /// Translates a natural language query into structured search parameters.
        /// </summary>
        /// <param name="naturalLanguageQuery">The user's search query in natural language</param>
        /// <param name="thresholds">Optional dynamic thresholds for numerical fields. If null, uses default values.</param>
        /// <returns>A structured search query that can be used with SearchDataGridView.</returns>
        public async Task<string> TranslateQueryAsync(string naturalLanguageQuery, DynamicThresholds thresholds = null)
        {
            try
            {
                // Only process if there's an actual query
                if (string.IsNullOrWhiteSpace(naturalLanguageQuery))
                {
                    return "";
                }

                // Create the prompt for the AI
                string prompt = BuildPrompt(naturalLanguageQuery, thresholds);

                // Send request to AI API
                string response = await SendApiRequestAsync(prompt);

                // Extract the formatted search query
                string formattedQuery = ParseResponse(response);

                return formattedQuery;
            }
            catch (Exception ex)
            {
                Log.Error_Translation(ex.Message);

                // Return the original query as fallback
                return naturalLanguageQuery;
            }
        }
        private static string BuildPrompt(string naturalLanguageQuery, DynamicThresholds thresholds = null)
        {
            // Create a detailed prompt that explains the search system and what we need
            StringBuilder promptBuilder = new();

            // Use provided thresholds or fall back to defaults
            thresholds ??= DynamicThresholds.CreateDefault();

            promptBuilder.AppendLine("You are a search query translator for a sales tracking application. Your role is to convert natural language queries into structured search syntax.");
            promptBuilder.AppendLine("\nThe application supports the following search syntax:");
            promptBuilder.AppendLine("- Regular terms for fuzzy matching");
            promptBuilder.AppendLine("- \"exact phrase\" for exact phrase matching");
            promptBuilder.AppendLine("- -term for excluding terms");
            promptBuilder.AppendLine("- +term for required terms");
            promptBuilder.AppendLine("- Field:value for field-specific searches (e.g., Company:AliExpress)");
            promptBuilder.AppendLine("- Field:>value for greater than comparisons (e.g., Discount:>10)");
            promptBuilder.AppendLine("- Field:<value for less than comparisons (e.g., Total:<50)");
            promptBuilder.AppendLine("- Use OR operator with pipe character '|' for multiple options (e.g., +Country of origin::France|Germany|Italy, and NOT +Country of origin:France|Germany +Country of origin:Italy).");

            // Add information about the data schema
            promptBuilder.AppendLine("\nData includes the following fields:");

            promptBuilder.AppendLine("  When the user refers to a country, map it to one of these exact country names:");
            // Include the first 10 countries to give an idea
            promptBuilder.AppendLine($"\n{string.Join(", ", Country.GetAllCountryNames())}");
            promptBuilder.AppendLine("\n For example, 'America' and 'USA' should be mapped to 'United States', 'UK' should be mapped to 'United Kingdom'");

            // Regional Groupings for OR conditions
            promptBuilder.AppendLine("\nREGIONAL GROUPINGS INSTRUCTIONS:");
            promptBuilder.AppendLine("When a user refers to a region or continent, translate it to an OR condition of all relevant countries.");
            promptBuilder.AppendLine("Examples of regional groupings:");
            promptBuilder.AppendLine("1. Europe: Use 'Country:France|Germany|United Kingdom|Italy|Spain|Netherlands|Sweden|Switzerland|Norway|Finland|Belgium|Denmark|Austria|Portugal|Greece|Ireland'");
            promptBuilder.AppendLine("2. Asia: Use 'Country:China|Japan|South Korea|India|Taiwan|Singapore|Vietnam|Thailand|Malaysia|Indonesia|Philippines'");
            promptBuilder.AppendLine("3. North America: Use 'Country:United States|Canada|Mexico'");
            promptBuilder.AppendLine("4. South America: Use 'Country:Brazil|Argentina|Colombia|Chile|Peru|Ecuador|Venezuela'");
            promptBuilder.AppendLine("5. Africa: Use 'Country:South Africa|Nigeria|Egypt|Kenya|Morocco|Ghana|Ethiopia|Tanzania'");
            promptBuilder.AppendLine("6. Middle East: Use 'Country:United Arab Emirates|Saudi Arabia|Israel|Turkey|Qatar|Kuwait|Bahrain|Oman'");
            promptBuilder.AppendLine("7. Oceania: Use 'Country:Australia|New Zealand|Fiji|Papua New Guinea'");

            // Explanation of "high" and "low" value thresholds - now using ALL dynamic thresholds
            promptBuilder.AppendLine("\nThresholds for qualitative terms (dynamically calculated based on actual data):");
            promptBuilder.AppendLine($"- \"expensive\" OR \"high cost\" OR \"high total\" means Total > {thresholds.HighTotal:F2}");
            promptBuilder.AppendLine($"- \"cheap\" OR \"low cost\" OR \"low total\" means Total < {thresholds.LowTotal:F2}");
            promptBuilder.AppendLine($"- \"high price\" OR \"expensive price\" means Price per unit > {thresholds.HighPrice:F2}");
            promptBuilder.AppendLine($"- \"low price\" OR \"cheap price\" means Price per unit < {thresholds.LowPrice:F2}");
            promptBuilder.AppendLine($"- \"high discount\" OR \"large discount\" means Discount > {thresholds.HighDiscount:F2}");
            promptBuilder.AppendLine($"- \"low discount\" OR \"small discount\" means Discount < {thresholds.LowDiscount:F2}");
            promptBuilder.AppendLine($"- \"high shipping\" OR \"expensive shipping\" means Shipping > {thresholds.HighShipping:F2}");
            promptBuilder.AppendLine($"- \"low shipping\" OR \"cheap shipping\" means Shipping < {thresholds.LowShipping:F2}");
            promptBuilder.AppendLine($"- \"free shipping\" means Shipping = 0");
            promptBuilder.AppendLine($"- \"high tax\" means Tax > {thresholds.HighTax:F2}");
            promptBuilder.AppendLine($"- \"low tax\" means Tax < {thresholds.LowTax:F2}");
            promptBuilder.AppendLine($"- \"high fee\" means Fee > {thresholds.HighFee:F2}");
            promptBuilder.AppendLine($"- \"low fee\" means Fee < {thresholds.LowFee:F2}");
            promptBuilder.AppendLine($"- \"high quantity\" OR \"large order\" OR \"bulk\" means Total items > {thresholds.HighQuantity:F2}");
            promptBuilder.AppendLine($"- \"low quantity\" OR \"small order\" means Total items < {thresholds.LowQuantity:F2}");

            // Add rules for using country fields
            promptBuilder.AppendLine("\nIMPORTANT RULES FOR COUNTRY FIELDS:");
            promptBuilder.AppendLine("1. Use 'Country of origin' for queries about where products come from");
            promptBuilder.AppendLine("2. Use 'Country of destination' for queries about where products are sent to");
            promptBuilder.AppendLine("3. If the query mentions 'purchases' or 'orders', use 'Country of origin'");
            promptBuilder.AppendLine("4. If the query mentions 'sales', use 'Country of destination'");
            promptBuilder.AppendLine("5. If unsure, default to 'Country of origin'");

            // Add examples to guide the AI
            promptBuilder.AppendLine("\nExamples:");

            promptBuilder.AppendLine("Query: \"products from america\"");
            promptBuilder.AppendLine("Translation: \"+Country of origin:United States\"");

            promptBuilder.AppendLine("\nQuery: \"sales to UK last month\"");
            promptBuilder.AppendLine("Translation: \"+Country of destination:United Kingdom +Date:>2023-12-01\"");

            promptBuilder.AppendLine("\nQuery: \"aliexpress orders with high discount\"");
            promptBuilder.AppendLine($"Translation: \"+Company of origin:aliexpress +Discount:>{thresholds.HighDiscount:F2}\"");

            promptBuilder.AppendLine("\nQuery: \"expensive purchases from last month\"");
            promptBuilder.AppendLine($"Translation: \"+Total:>{thresholds.HighTotal:F2} +Date:>2023-12-01\"");

            promptBuilder.AppendLine("\nQuery: \"sales with free shipping that are not from Germany\"");
            promptBuilder.AppendLine("Translation: \"+Shipping:0 -Company of origin:Germany\"");

            promptBuilder.AppendLine("\nQuery: \"orders from germany with discount greater than 10\"");
            promptBuilder.AppendLine("Translation: \"+Country of origin:Germany +Discount:>10\"");

            promptBuilder.AppendLine("\nQuery: \"orders from germany with discount of 10 or more\"");
            promptBuilder.AppendLine("Translation: \"+Country of origin:Germany +Discount:>9.9\"");

            promptBuilder.AppendLine("\nQuery: \"orders from germany made by John\"");
            promptBuilder.AppendLine("Translation: \"+Country of origin:Germany \"John\"");

            promptBuilder.AppendLine("\nQuery: \"orders from germany and Italy\"");
            promptBuilder.AppendLine("Translation: \"+Country of origin:Germany|Italy");

            promptBuilder.AppendLine("\nQuery: \"orders from europe\"");
            promptBuilder.AppendLine("Translation: \"+Country of origin:France|Germany|United Kingdom|Italy|Spain|Netherlands|Sweden|Switzerland|Norway|Finland|Belgium|Denmark|Austria|Portugal|Greece|Ireland\"");

            promptBuilder.AppendLine("\nQuery: \"orders from europe but not france\"");
            promptBuilder.AppendLine("Translation: \"+Country of origin:Germany|United Kingdom|Italy|Spain|Netherlands|Sweden|Switzerland|Norway|Finland|Belgium|Denmark|Austria|Portugal|Greece|Ireland\"");

            promptBuilder.AppendLine("\nQuery: \"sales to asian countries last month\"");
            promptBuilder.AppendLine("Translation: \"+Country of destination:China|Japan|South Korea|India|Taiwan|Singapore|Vietnam|Thailand|Malaysia|Indonesia|Philippines +Date:>2023-12-01\"");

            promptBuilder.AppendLine("\nQuery: \"cheap items with low shipping\"");
            promptBuilder.AppendLine($"Translation: \"+Total:<{thresholds.LowTotal:F2} +Shipping:<{thresholds.LowShipping:F2}\"");

            promptBuilder.AppendLine("\nQuery: \"bulk orders with high discounts\"");
            promptBuilder.AppendLine($"Translation: \"+Total items:>{thresholds.HighQuantity:F2} +Discount:>{thresholds.HighDiscount:F2}\"");

            promptBuilder.AppendLine("\nQuery: \"expensive items with low price per unit\"");
            promptBuilder.AppendLine($"Translation: \"+Total:>{thresholds.HighTotal:F2} +Price per unit:<{thresholds.LowPrice:F2}\"");

            promptBuilder.AppendLine("\nQuery: \"small orders with high shipping costs\"");
            promptBuilder.AppendLine($"Translation: \"+Total items:<{thresholds.LowQuantity:F2} +Shipping:>{thresholds.HighShipping:F2}\"");

            // Finally, add the user's query
            promptBuilder.AppendLine("\nUser Query: \"" + naturalLanguageQuery + "\"");
            promptBuilder.AppendLine("\nTranslation:");

            // Emphasize output format
            promptBuilder.AppendLine("\nIMPORTANT: Return ONLY the structured search query without any explanation or additional text.");
            promptBuilder.AppendLine("Use the exact field names as provided above and ensure proper syntax with + for required terms.");
            promptBuilder.AppendLine("Remember to use the pipe character '|' for OR conditions when translating regional terms.");
            promptBuilder.AppendLine("Do NOT add any single or double quotes around your response.");

            return promptBuilder.ToString();
        }
        private async Task<string> SendApiRequestAsync(string prompt)
        {
            try
            {
                // Create the API request payload
                var requestBody = new
                {
                    model = Model,
                    messages = new[]
                    {
                        new { role = "system", content = "You are a search query translator." },
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.3,
                    max_tokens = 150
                };

                StringContent content = new(
                    JsonConvert.SerializeObject(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                // Send the request
                HttpResponseMessage response = await _httpClient.PostAsync(_apiEndpoint, content);
                string responseBody = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                // Parse the response
                dynamic? responseObject = JsonConvert.DeserializeObject<dynamic>(responseBody);

                // Extract token usage
                if (responseObject?.usage != null)
                {
                    LastTokenUsage = responseObject.usage.total_tokens;
                    Log.WriteWithFormat(2, "Token usage for query translation: {0}", LastTokenUsage);
                }
                else
                {
                    LastTokenUsage = 0;
                }

                return responseObject.choices[0].message.content.ToString();
            }
            catch (Exception ex)
            {
                Log.Error_TranslationAPIRequestFailed(ex.Message);
                LastTokenUsage = 0;
                throw;
            }
        }

        /// <summary>
        /// The model is instructed not to include explanations, but handle it just in case.
        /// </summary>
        /// <returns></returns>
        private static string ParseResponse(string response)
        {
            // Extract just the query part (strip explanations)
            string[] lines = response.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);

            // Look for the first line that contains a likely search query
            foreach (string line in lines)
            {
                string trimmed = line.Trim();

                // Heuristics to identify the actual translation:
                // - Contains search syntax characters (+, -, :, >, <)
                // - Not too long
                // - Doesn't contain full sentences
                if ((trimmed.Contains('+') || trimmed.Contains('-') ||
                     trimmed.Contains(':') || trimmed.Contains('>') ||
                     trimmed.Contains('<')) &&
                     trimmed.Length < 100 &&
                     !trimmed.EndsWith('.'))
                {
                    return StripQuotes(trimmed);
                }
            }

            // If we can't find a good translation, return a cleaned-up version of the first non-empty line
            return lines.Length > 0 ? StripQuotes(lines[0].Trim()) : "";
        }
        private static string StripQuotes(string input)
        {
            if (string.IsNullOrEmpty(input)) { return input; }

            // Remove leading and trailing " or ' if present
            if (input.StartsWith('\"') && input.EndsWith('\"') ||
                input.StartsWith('\'') && input.EndsWith('\''))
            {
                return input.Substring(1, input.Length - 2);
            }

            return input;
        }
    }
}
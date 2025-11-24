using Argo_Books.UI;
using System.Text;

namespace Argo_Books.DataClasses
{
    /// <summary>
    /// Represents a country with its dialing code and phone number formatting rules.
    /// </summary>
    public class CountryCode(string countryName, string code, string format = "")
    {
        public string CountryName { get; set; } = countryName;
        public string Code { get; set; } = code;
        public string Format { get; set; } = format;

        /// <summary>
        /// Gets the search result display text with country code.
        /// </summary>
        public string ToSearchResultText()
        {
            return $"{Code} - {CountryName}";
        }

        /// <summary>
        /// Gets a list of all country codes with their formatting patterns.
        /// Matches all countries from Country.cs
        /// </summary>
        public static List<CountryCode> GetCountryCodes()
        {
            return
            [
                // Popular countries (will be shown at top)
                new("United States", "+1", "(XXX) XXX-XXXX"),
                new("China", "+86", "XXX XXXX XXXX"),
                new("Germany", "+49", "XXX XXXXXXXX"),
                new("Japan", "+81", "XX XXXX XXXX"),
                new("United Kingdom", "+44", "XXXX XXX XXXX"),
                new("France", "+33", "X XX XX XX XX"),
                new("Italy", "+39", "XXX XXX XXXX"),
                new("Canada", "+1", "(XXX) XXX-XXXX"),

                // All other countries alphabetically
                new("Afghanistan", "+93", "XX XXX XXXX"),
                new("Albania", "+355", "XX XXX XXXX"),
                new("Algeria", "+213", "XXX XX XX XX"),
                new("Andorra", "+376", "XXX XXX"),
                new("Angola", "+244", "XXX XXX XXX"),
                new("Antigua and Barbuda", "+1-268", "XXX-XXXX"),
                new("Argentina", "+54", "XX XXXX-XXXX"),
                new("Armenia", "+374", "XX XXX XXX"),
                new("Australia", "+61", "XXXX XXX XXX"),
                new("Austria", "+43", "XXX XXXXXXX"),
                new("Azerbaijan", "+994", "XX XXX XX XX"),
                new("Bahamas", "+1-242", "XXX-XXXX"),
                new("Bahrain", "+973", "XXXX XXXX"),
                new("Bangladesh", "+880", "XXXX-XXXXXX"),
                new("Barbados", "+1-246", "XXX-XXXX"),
                new("Belarus", "+375", "XX XXX-XX-XX"),
                new("Belgium", "+32", "XXX XX XX XX"),
                new("Belize", "+501", "XXX-XXXX"),
                new("Benin", "+229", "XX XX XX XX"),
                new("Bhutan", "+975", "XX XXX XXX"),
                new("Bolivia", "+591", "X XXX XXXX"),
                new("Bosnia and Herzegovina", "+387", "XX XXX-XXX"),
                new("Botswana", "+267", "XX XXX XXX"),
                new("Brazil", "+55", "(XX) XXXXX-XXXX"),
                new("Brunei", "+673", "XXX XXXX"),
                new("Bulgaria", "+359", "XX XXX XXXX"),
                new("Burkina Faso", "+226", "XX XX XX XX"),
                new("Burundi", "+257", "XX XX XXXX"),
                new("Cabo Verde", "+238", "XXX XX XX"),
                new("Cambodia", "+855", "XX XXX XXX"),
                new("Cameroon", "+237", "X XX XX XX XX"),
                new("Central African Republic", "+236", "XX XX XX XX"),
                new("Chad", "+235", "XX XX XX XX"),
                new("Chile", "+56", "X XXXX XXXX"),
                new("Colombia", "+57", "XXX XXX XXXX"),
                new("Comoros", "+269", "XXX XX XX"),
                new("Costa Rica", "+506", "XXXX XXXX"),
                new("Croatia", "+385", "XX XXX XXXX"),
                new("Cuba", "+53", "X XXX XXXX"),
                new("Cyprus", "+357", "XX XXXXXX"),
                new("Czechia", "+420", "XXX XXX XXX"),
                new("Denmark", "+45", "XX XX XX XX"),
                new("Djibouti", "+253", "XX XX XX XX"),
                new("Dominica", "+1-767", "XXX-XXXX"),
                new("Dominican Republic", "+1-809", "XXX-XXXX"),
                new("Ecuador", "+593", "XX XXX XXXX"),
                new("Egypt", "+20", "XXX XXX XXXX"),
                new("El Salvador", "+503", "XXXX XXXX"),
                new("Equatorial Guinea", "+240", "XXX XXX XXX"),
                new("Eritrea", "+291", "X XXX XXX"),
                new("Estonia", "+372", "XXXX XXXX"),
                new("Eswatini", "+268", "XXXX XXXX"),
                new("Ethiopia", "+251", "XX XXX XXXX"),
                new("Fiji", "+679", "XXX XXXX"),
                new("Finland", "+358", "XX XXX XXXX"),
                new("Gabon", "+241", "X XX XX XX"),
                new("Gambia", "+220", "XXX XXXX"),
                new("Georgia", "+995", "XXX XXX XXX"),
                new("Ghana", "+233", "XX XXX XXXX"),
                new("Greece", "+30", "XXX XXX XXXX"),
                new("Grenada", "+1-473", "XXX-XXXX"),
                new("Guatemala", "+502", "XXXX XXXX"),
                new("Guinea", "+224", "XXX XX XX XX"),
                new("Guinea-Bissau", "+245", "XXX XXXX"),
                new("Guyana", "+592", "XXX XXXX"),
                new("Haiti", "+509", "XXXX XXXX"),
                new("Honduras", "+504", "XXXX XXXX"),
                new("Hungary", "+36", "XX XXX XXXX"),
                new("Iceland", "+354", "XXX XXXX"),
                new("India", "+91", "XXXXX XXXXX"),
                new("Indonesia", "+62", "XXX XXXX XXXX"),
                new("Iran", "+98", "XXX XXX XXXX"),
                new("Iraq", "+964", "XXX XXX XXXX"),
                new("Ireland", "+353", "XX XXX XXXX"),
                new("Israel", "+972", "XX XXX XXXX"),
                new("Ivory Coast", "+225", "XX XX XX XXXX"),
                new("Jamaica", "+1-876", "XXX-XXXX"),
                new("Jordan", "+962", "X XXXX XXXX"),
                new("Kazakhstan", "+7", "XXX XXX XX XX"),
                new("Kenya", "+254", "XXX XXXXXX"),
                new("Kiribati", "+686", "XXXX XXXX"),
                new("Kuwait", "+965", "XXXX XXXX"),
                new("Kyrgyzstan", "+996", "XXX XXXXXX"),
                new("Lao", "+856", "XX XX XXX XXX"),
                new("Latvia", "+371", "XX XXX XXX"),
                new("Lebanon", "+961", "XX XXX XXX"),
                new("Lesotho", "+266", "XX XXX XXX"),
                new("Liberia", "+231", "XX XXX XXXX"),
                new("Libya", "+218", "XX XXX XXXX"),
                new("Liechtenstein", "+423", "XXX XXXX"),
                new("Lithuania", "+370", "XXX XXXXX"),
                new("Luxembourg", "+352", "XXX XXX XXX"),
                new("Madagascar", "+261", "XX XX XXX XX"),
                new("Malawi", "+265", "X XXXX XXXX"),
                new("Malaysia", "+60", "XX XXXX XXXX"),
                new("Maldives", "+960", "XXX XXXX"),
                new("Mali", "+223", "XX XX XX XX"),
                new("Malta", "+356", "XXXX XXXX"),
                new("Marshall Islands", "+692", "XXX-XXXX"),
                new("Mauritania", "+222", "XX XX XX XX"),
                new("Mauritius", "+230", "XXXX XXXX"),
                new("Mexico", "+52", "XXX XXX XXXX"),
                new("Micronesia", "+691", "XXX XXXX"),
                new("Moldova", "+373", "XX XXX XXX"),
                new("Monaco", "+377", "XX XX XX XX"),
                new("Mongolia", "+976", "XX XX XXXX"),
                new("Montenegro", "+382", "XX XXX XXX"),
                new("Morocco", "+212", "XX XXX XXXX"),
                new("Mozambique", "+258", "XX XXX XXXX"),
                new("Myanmar", "+95", "XX XXX XXXX"),
                new("Namibia", "+264", "XX XXX XXXX"),
                new("Nauru", "+674", "XXX XXXX"),
                new("Nepal", "+977", "XX XXX XXXX"),
                new("Netherlands", "+31", "XX XXXXXXXX"),
                new("New Zealand", "+64", "XX XXX XXXX"),
                new("Nicaragua", "+505", "XXXX XXXX"),
                new("Niger", "+227", "XX XX XX XX"),
                new("Nigeria", "+234", "XXX XXX XXXX"),
                new("North Korea", "+850", "XXX XXXX XXXX"),
                new("North Macedonia", "+389", "XX XXX XXX"),
                new("Norway", "+47", "XXX XX XXX"),
                new("Oman", "+968", "XXXX XXXX"),
                new("Pakistan", "+92", "XXX XXX XXXX"),
                new("Palau", "+680", "XXX XXXX"),
                new("Panama", "+507", "XXXX XXXX"),
                new("Papua New Guinea", "+675", "XXX XXXX"),
                new("Paraguay", "+595", "XXX XXX XXX"),
                new("Peru", "+51", "XXX XXX XXX"),
                new("Philippines", "+63", "XXX XXX XXXX"),
                new("Poland", "+48", "XXX XXX XXX"),
                new("Portugal", "+351", "XXX XXX XXX"),
                new("Qatar", "+974", "XXXX XXXX"),
                new("Romania", "+40", "XXX XXX XXX"),
                new("Russia", "+7", "XXX XXX-XX-XX"),
                new("Rwanda", "+250", "XXX XXX XXX"),
                new("Saint Kitts and Nevis", "+1-869", "XXX-XXXX"),
                new("Saint Lucia", "+1-758", "XXX-XXXX"),
                new("Saint Vincent and the Grenadines", "+1-784", "XXX-XXXX"),
                new("Samoa", "+685", "XX XXXX"),
                new("San Marino", "+378", "XXXX XXXXXX"),
                new("Sao Tome and Principe", "+239", "XXX XXXX"),
                new("Saudi Arabia", "+966", "XX XXX XXXX"),
                new("Senegal", "+221", "XX XXX XX XX"),
                new("Serbia", "+381", "XX XXX XXXX"),
                new("Seychelles", "+248", "X XXX XXX"),
                new("Sierra Leone", "+232", "XX XXXXXX"),
                new("Singapore", "+65", "XXXX XXXX"),
                new("Slovakia", "+421", "XXX XXX XXX"),
                new("Slovenia", "+386", "XX XXX XXX"),
                new("Solomon Islands", "+677", "XXXXX"),
                new("Somalia", "+252", "XX XXX XXX"),
                new("South Africa", "+27", "XX XXX XXXX"),
                new("South Korea", "+82", "XX XXXX XXXX"),
                new("South Sudan", "+211", "XX XXX XXXX"),
                new("Spain", "+34", "XXX XXX XXX"),
                new("Sri Lanka", "+94", "XX XXX XXXX"),
                new("Sudan", "+249", "XX XXX XXXX"),
                new("Suriname", "+597", "XXX XXXX"),
                new("Sweden", "+46", "XX XXX XX XX"),
                new("Switzerland", "+41", "XX XXX XX XX"),
                new("Syria", "+963", "XX XXXX XXXX"),
                new("Taiwan", "+886", "XXXX XXXX"),
                new("Tajikistan", "+992", "XX XXX XXXX"),
                new("Tanzania", "+255", "XXX XXX XXX"),
                new("Thailand", "+66", "XX XXX XXXX"),
                new("The Democratic Republic of the Congo", "+243", "XXX XXX XXX"),
                new("The Republic of the Congo", "+242", "XX XXX XXXX"),
                new("Timor-Leste", "+670", "XXX XXXX"),
                new("Togo", "+228", "XX XX XX XX"),
                new("Tonga", "+676", "XXXXX"),
                new("Trinidad and Tobago", "+1-868", "XXX-XXXX"),
                new("Tunisia", "+216", "XX XXX XXX"),
                new("Turkey", "+90", "XXX XXX XX XX"),
                new("Turkmenistan", "+993", "XX XXXXXX"),
                new("Tuvalu", "+688", "XXXXX"),
                new("Uganda", "+256", "XXX XXXXXX"),
                new("Ukraine", "+380", "XX XXX XX XX"),
                new("United Arab Emirates", "+971", "XX XXX XXXX"),
                new("Uruguay", "+598", "X XXX XXXX"),
                new("Uzbekistan", "+998", "XX XXX XX XX"),
                new("Vanuatu", "+678", "XXXXX"),
                new("Venezuela", "+58", "XXX XXX XXXX"),
                new("Vietnam", "+84", "XX XXXX XXXX"),
                new("Western Sahara", "+212", "XX XXX XXXX"),
                new("Yemen", "+967", "XXX XXX XXX"),
                new("Zambia", "+260", "XX XXX XXXX"),
                new("Zimbabwe", "+263", "XX XXX XXXX")
            ];
        }

        /// <summary>
        /// Formats a phone number according to the country's format pattern.
        /// </summary>
        public string FormatPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(Format) || string.IsNullOrEmpty(phoneNumber))
                return phoneNumber;

            // Remove all non-digit characters
            string digitsOnly = new(phoneNumber.Where(char.IsDigit).ToArray());

            if (string.IsNullOrEmpty(digitsOnly))
                return phoneNumber;

            StringBuilder formatted = new();
            int digitIndex = 0;

            foreach (char c in Format)
            {
                if (c == 'X')
                {
                    if (digitIndex < digitsOnly.Length)
                    {
                        formatted.Append(digitsOnly[digitIndex]);
                        digitIndex++;
                    }
                    else
                    {
                        break;  // No more digits to format
                    }
                }
                else
                {
                    // Add formatting character if there are still digits to be formatted
                    if (digitIndex < digitsOnly.Length)
                    {
                        formatted.Append(c);
                    }
                }
            }

            // If there are remaining digits, append them
            if (digitIndex < digitsOnly.Length)
            {
                formatted.Append(digitsOnly.AsSpan(digitIndex));
            }

            return formatted.ToString();
        }

        /// <summary>
        /// Gets SearchResults for country codes with flags for use in SearchBox.
        /// </summary>
        public static List<SearchResult> GetCountryCodeSearchResults()
        {
            List<SearchResult> results = [];
            List<CountryCode> countryCodes = GetCountryCodes();

            // Create a dictionary to map country names to their flags from the Country class
            Dictionary<string, Image> countryFlags = Country.CountrySearchResults
                .Where(sr => sr.Name != SearchBox.AddLine && !string.IsNullOrEmpty(sr.Name))
                .ToDictionary(sr => sr.Name, sr => sr.Flag, StringComparer.OrdinalIgnoreCase);

            // Add popular countries first (matching Country.cs order)
            string[] popularCountries = ["United States", "China", "Germany", "Japan", "United Kingdom", "France", "Italy", "Canada"];
            foreach (string popularCountry in popularCountries)
            {
                CountryCode? country = countryCodes.FirstOrDefault(c => c.CountryName == popularCountry);
                if (country != null)
                {
                    countryFlags.TryGetValue(country.CountryName, out Image? flag);
                    results.Add(new SearchResult(country.ToSearchResultText(), flag, 0));
                }
            }

            // Add separator line
            results.Add(new SearchResult(SearchBox.AddLine, null, 0));

            // Add all other countries
            foreach (CountryCode country in countryCodes)
            {
                if (!popularCountries.Contains(country.CountryName))
                {
                    countryFlags.TryGetValue(country.CountryName, out Image? flag);
                    results.Add(new SearchResult(country.ToSearchResultText(), flag, 0));
                }
            }

            return results;
        }

        /// <summary>
        /// Parses a search result text to extract the country code.
        /// </summary>
        public static string ParseCountryCodeFromSearchResult(string searchResultText)
        {
            if (string.IsNullOrWhiteSpace(searchResultText))
                return "+1";

            // Format is "+X - CountryName" or "+XX - CountryName"
            int dashIndex = searchResultText.IndexOf(" - ");
            if (dashIndex > 0)
            {
                return searchResultText.Substring(0, dashIndex).Trim();
            }

            return searchResultText;
        }

        /// <summary>
        /// Gets a CountryCode object from a search result text or country code string.
        /// </summary>
        public static CountryCode? GetCountryCodeFromText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return GetCountryCodes().FirstOrDefault(c => c.Code == "+1");

            List<CountryCode> codes = GetCountryCodes();

            // Try to parse as search result text ("+X - CountryName")
            string code = ParseCountryCodeFromSearchResult(text);

            // Find by code
            return codes.FirstOrDefault(c => c.Code == code) ?? codes.FirstOrDefault(c => c.Code == "+1");
        }
    }
}

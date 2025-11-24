using Argo_Books.UI;

namespace Argo_Books.DataClasses
{
    /// <summary>
    /// Provides a list of all countries with associated flags.
    /// This list is used to support search functionalities within the application.
    /// </summary>
    public static class Country
    {
        public static readonly List<SearchResult> CountrySearchResults =
        [
            new SearchResult("Unkown", null, 0),
            new SearchResult("United States", Properties.Flags.United_States_of_America, 0),
            new SearchResult("China", Properties.Flags.China, 0),
            new SearchResult("Germany", Properties.Flags.Germany, 0),
            new SearchResult("Japan", Properties.Flags.Japan, 0),
            new SearchResult("United Kingdom", Properties.Flags.United_Kingdom_of_Great_Britain_and_Northern_Ireland, 0),
            new SearchResult("France", Properties.Flags.France, 0),
            new SearchResult("Italy", Properties.Flags.Italy, 0),
            new SearchResult("Canada", Properties.Flags.Canada, 0),

            new SearchResult(SearchBox.AddLine, null, 0),

            new SearchResult("Afghanistan", Properties.Flags.Afghanistan, 0),
            new SearchResult("Albania", Properties.Flags.Albania, 0),
            new SearchResult("Algeria", Properties.Flags.Algeria, 0),
            new SearchResult("Andorra", Properties.Flags.Andorra, 0),
            new SearchResult("Angola", Properties.Flags.Angola, 0),
            new SearchResult("Antigua and Barbuda", Properties.Flags.Antigua_and_Barbuda, 0),
            new SearchResult("Argentina", Properties.Flags.Argentina, 0),
            new SearchResult("Armenia", Properties.Flags.Armenia, 0),
            new SearchResult("Australia", Properties.Flags.Australia, 0),
            new SearchResult("Austria", Properties.Flags.Austria, 0),
            new SearchResult("Azerbaijan", Properties.Flags.Azerbaijan, 0),
            new SearchResult("Bahamas", Properties.Flags.Bahamas, 0),
            new SearchResult("Bahrain", Properties.Flags.Bahrain, 0),
            new SearchResult("Bangladesh", Properties.Flags.Bangladesh, 0),
            new SearchResult("Barbados", Properties.Flags.Barbados, 0),
            new SearchResult("Belarus", Properties.Flags.Belarus, 0),
            new SearchResult("Belgium", Properties.Flags.Belgium, 0),
            new SearchResult("Belize", Properties.Flags.Belize, 0),
            new SearchResult("Benin", Properties.Flags.Benin, 0),
            new SearchResult("Bhutan", Properties.Flags.Bhutan, 0),
            new SearchResult("Bolivia", Properties.Flags.Bolivia, 0),
            new SearchResult("Bosnia and Herzegovina", Properties.Flags.Bosnia_and_Herzegovina, 0),
            new SearchResult("Botswana", Properties.Flags.Botswana, 0),
            new SearchResult("Brazil", Properties.Flags.Brazil, 0),
            new SearchResult("Brunei", Properties.Flags.Brunei, 0),
            new SearchResult("Bulgaria", Properties.Flags.Bulgaria, 0),
            new SearchResult("Burkina Faso", Properties.Flags.Burkina_Faso, 0),
            new SearchResult("Burundi", Properties.Flags.Burundi, 0),
            new SearchResult("Cabo Verde", Properties.Flags.Cabo_Verde, 0),
            new SearchResult("Cambodia", Properties.Flags.Cambodia, 0),
            new SearchResult("Cameroon", Properties.Flags.Cameroon, 0),
            new SearchResult("Central African Republic", Properties.Flags.Central_African_Republic, 0),
            new SearchResult("Chad", Properties.Flags.Chad, 0),
            new SearchResult("Chile", Properties.Flags.Chile, 0),
            new SearchResult("Colombia", Properties.Flags.Colombia, 0),
            new SearchResult("Comoros", Properties.Flags.Comoros, 0),
            new SearchResult("Costa Rica", Properties.Flags.Costa_Rica, 0),
            new SearchResult("Croatia", Properties.Flags.Croatia, 0),
            new SearchResult("Cuba", Properties.Flags.Cuba, 0),
            new SearchResult("Cyprus", Properties.Flags.Cyprus, 0),
            new SearchResult("Czechia", Properties.Flags.Czechia, 0),
            new SearchResult("Denmark", Properties.Flags.Denmark, 0),
            new SearchResult("Djibouti", Properties.Flags.Djibouti, 0),
            new SearchResult("Dominica", Properties.Flags.Dominica, 0),
            new SearchResult("Dominican Republic", Properties.Flags.Dominican_Republic, 0),
            new SearchResult("Ecuador", Properties.Flags.Ecuador, 0),
            new SearchResult("Egypt", Properties.Flags.Egypt, 0),
            new SearchResult("El Salvador", Properties.Flags.El_Salvador, 0),
            new SearchResult("Equatorial Guinea", Properties.Flags.Equatorial_Guinea, 0),
            new SearchResult("Eritrea", Properties.Flags.Eritrea, 0),
            new SearchResult("Estonia", Properties.Flags.Estonia, 0),
            new SearchResult("Eswatini", Properties.Flags.Eswatini, 0),
            new SearchResult("Ethiopia", Properties.Flags.Ethiopia, 0),
            new SearchResult("Fiji", Properties.Flags.Fiji, 0),
            new SearchResult("Finland", Properties.Flags.Finland, 0),
            new SearchResult("Gabon", Properties.Flags.Gabon, 0),
            new SearchResult("Gambia", Properties.Flags.Gambia, 0),
            new SearchResult("Georgia", Properties.Flags.Georgia, 0),
            new SearchResult("Ghana", Properties.Flags.Ghana, 0),
            new SearchResult("Greece", Properties.Flags.Greece, 0),
            new SearchResult("Grenada", Properties.Flags.Grenada, 0),
            new SearchResult("Guatemala", Properties.Flags.Guatemala, 0),
            new SearchResult("Guinea", Properties.Flags.Guinea, 0),
            new SearchResult("Guinea-Bissau", Properties.Flags.Guinea_Bissau, 0),
            new SearchResult("Guyana", Properties.Flags.Guyana, 0),
            new SearchResult("Haiti", Properties.Flags.Haiti, 0),
            new SearchResult("Honduras", Properties.Flags.Honduras, 0),
            new SearchResult("Hungary", Properties.Flags.Hungary, 0),
            new SearchResult("Iceland", Properties.Flags.Iceland, 0),
            new SearchResult("India", Properties.Flags.India, 0),
            new SearchResult("Indonesia", Properties.Flags.Indonesia, 0),
            new SearchResult("Iran", Properties.Flags.Iran, 0),
            new SearchResult("Iraq", Properties.Flags.Iraq, 0),
            new SearchResult("Ireland", Properties.Flags.Ireland, 0),
            new SearchResult("Israel", Properties.Flags.Israel, 0),
            new SearchResult("Ivory Coast", Properties.Flags.Ivory_Coast, 0),
            new SearchResult("Jamaica", Properties.Flags.Jamaica, 0),
            new SearchResult("Jordan", Properties.Flags.Jordan, 0),
            new SearchResult("Kazakhstan", Properties.Flags.Kazakhstan, 0),
            new SearchResult("Kenya", Properties.Flags.Kenya, 0),
            new SearchResult("Kiribati", Properties.Flags.Kiribati, 0),
            new SearchResult("Kuwait", Properties.Flags.Kuwait, 0),
            new SearchResult("Kyrgyzstan", Properties.Flags.Kyrgyzstan, 0),
            new SearchResult("Lao", Properties.Flags.Lao, 0),
            new SearchResult("Latvia", Properties.Flags.Latvia, 0),
            new SearchResult("Lebanon", Properties.Flags.Lebanon, 0),
            new SearchResult("Lesotho", Properties.Flags.Lesotho, 0),
            new SearchResult("Liberia", Properties.Flags.Liberia, 0),
            new SearchResult("Libya", Properties.Flags.Libya, 0),
            new SearchResult("Liechtenstein", Properties.Flags.Liechtenstein, 0),
            new SearchResult("Lithuania", Properties.Flags.Lithuania, 0),
            new SearchResult("Luxembourg", Properties.Flags.Luxembourg, 0),
            new SearchResult("Madagascar", Properties.Flags.Madagascar, 0),
            new SearchResult("Malawi", Properties.Flags.Malawi, 0),
            new SearchResult("Malaysia", Properties.Flags.Malaysia, 0),
            new SearchResult("Maldives", Properties.Flags.Maldives, 0),
            new SearchResult("Mali", Properties.Flags.Mali, 0),
            new SearchResult("Malta", Properties.Flags.Malta, 0),
            new SearchResult("Marshall Islands", Properties.Flags.Marshall_Islands, 0),
            new SearchResult("Mauritania", Properties.Flags.Mauritania, 0),
            new SearchResult("Mauritius", Properties.Flags.Mauritius, 0),
            new SearchResult("Mexico", Properties.Flags.Mexico, 0),
            new SearchResult("Micronesia", Properties.Flags.Micronesia, 0),
            new SearchResult("Moldova", Properties.Flags.Moldova, 0),
            new SearchResult("Monaco", Properties.Flags.Monaco, 0),
            new SearchResult("Mongolia", Properties.Flags.Mongolia, 0),
            new SearchResult("Montenegro", Properties.Flags.Montenegro, 0),
            new SearchResult("Morocco", Properties.Flags.Morocco, 0),
            new SearchResult("Mozambique", Properties.Flags.Mozambique, 0),
            new SearchResult("Myanmar", Properties.Flags.Myanmar, 0),
            new SearchResult("Namibia", Properties.Flags.Namibia, 0),
            new SearchResult("Nauru", Properties.Flags.Nauru, 0),
            new SearchResult("Nepal", Properties.Flags.Nepal, 0),
            new SearchResult("Netherlands", Properties.Flags.Netherlands, 0),
            new SearchResult("New Zealand", Properties.Flags.New_Zealand, 0),
            new SearchResult("Nicaragua", Properties.Flags.Nicaragua, 0),
            new SearchResult("Niger", Properties.Flags.Niger, 0),
            new SearchResult("Nigeria", Properties.Flags.Nigeria, 0),
            new SearchResult("North Korea", Properties.Flags.North_Korea, 0),
            new SearchResult("North Macedonia", Properties.Flags.North_Macedonia, 0),
            new SearchResult("Norway", Properties.Flags.Norway, 0),
            new SearchResult("Oman", Properties.Flags.Oman, 0),
            new SearchResult("Pakistan", Properties.Flags.Pakistan, 0),
            new SearchResult("Palau", Properties.Flags.Palau, 0),
            new SearchResult("Panama", Properties.Flags.Panama, 0),
            new SearchResult("Papua New Guinea", Properties.Flags.Papua_New_Guinea, 0),
            new SearchResult("Paraguay", Properties.Flags.Paraguay, 0),
            new SearchResult("Peru", Properties.Flags.Peru, 0),
            new SearchResult("Philippines", Properties.Flags.Philippines, 0),
            new SearchResult("Poland", Properties.Flags.Poland, 0),
            new SearchResult("Portugal", Properties.Flags.Portugal, 0),
            new SearchResult("Qatar", Properties.Flags.Qatar, 0),
            new SearchResult("Romania", Properties.Flags.Romania, 0),
            new SearchResult("Russia", Properties.Flags.Russia, 0),
            new SearchResult("Rwanda", Properties.Flags.Rwanda, 0),
            new SearchResult("Saint Kitts and Nevis", Properties.Flags.Saint_Kitts_and_Nevis, 0),
            new SearchResult("Saint Lucia", Properties.Flags.Saint_Lucia, 0),
            new SearchResult("Saint Vincent and the Grenadines", Properties.Flags.Saint_Vincent_and_the_Grenadines, 0),
            new SearchResult("Samoa", Properties.Flags.Samoa, 0),
            new SearchResult("San Marino", Properties.Flags.San_Marino, 0),
            new SearchResult("Sao Tome and Principe", Properties.Flags.Sao_Tome_and_Principe, 0),
            new SearchResult("Saudi Arabia", Properties.Flags.Saudi_Arabia, 0),
            new SearchResult("Senegal", Properties.Flags.Senegal, 0),
            new SearchResult("Serbia", Properties.Flags.Serbia, 0),
            new SearchResult("Seychelles", Properties.Flags.Seychelles, 0),
            new SearchResult("Sierra Leone", Properties.Flags.Sierra_Leone, 0),
            new SearchResult("Singapore", Properties.Flags.Singapore, 0),
            new SearchResult("Slovakia", Properties.Flags.Slovakia, 0),
            new SearchResult("Slovenia", Properties.Flags.Slovenia, 0),
            new SearchResult("Solomon Islands", Properties.Flags.Solomon_Islands, 0),
            new SearchResult("Somalia", Properties.Flags.Somalia, 0),
            new SearchResult("South Africa", Properties.Flags.South_Africa, 0),
            new SearchResult("South Korea", Properties.Flags.South_Korea, 0),
            new SearchResult("South Sudan", Properties.Flags.South_Sudan, 0),
            new SearchResult("Spain", Properties.Flags.Spain, 0),
            new SearchResult("Sri Lanka", Properties.Flags.Sri_Lanka, 0),
            new SearchResult("Sudan", Properties.Flags.Sudan, 0),
            new SearchResult("Suriname", Properties.Flags.Suriname, 0),
            new SearchResult("Sweden", Properties.Flags.Sweden, 0),
            new SearchResult("Switzerland", Properties.Flags.Switzerland, 0),
            new SearchResult("Syria", Properties.Flags.Syria, 0),
            new SearchResult("Taiwan", Properties.Flags.Taiwan, 0),
            new SearchResult("Tajikistan", Properties.Flags.Tajikistan, 0),
            new SearchResult("Tanzania", Properties.Flags.Tanzania, 0),
            new SearchResult("Thailand", Properties.Flags.Thailand, 0),
            new SearchResult("The Democratic Republic of the Congo", Properties.Flags.The_Democratic_Republic_of_the_Congo, 0),
            new SearchResult("The Republic of the Congo", Properties.Flags.The_Republic_of_the_Congo, 0),
            new SearchResult("Timor-Leste", Properties.Flags.Timor_Leste, 0),
            new SearchResult("Togo", Properties.Flags.Togo, 0),
            new SearchResult("Tonga", Properties.Flags.Tonga, 0),
            new SearchResult("Trinidad and Tobago", Properties.Flags.Trinidad_and_Tobago, 0),
            new SearchResult("Tunisia", Properties.Flags.Tunisia, 0),
            new SearchResult("Turkey", Properties.Flags.Turkey, 0),
            new SearchResult("Turkmenistan", Properties.Flags.Turkmenistan, 0),
            new SearchResult("Tuvalu", Properties.Flags.Tuvalu, 0),
            new SearchResult("Uganda", Properties.Flags.Uganda, 0),
            new SearchResult("Ukraine", Properties.Flags.Ukraine, 0),
            new SearchResult("United Arab Emirates", Properties.Flags.United_Arab_Emirates, 0),
            new SearchResult("Uruguay", Properties.Flags.Uruguay, 0),
            new SearchResult("Uzbekistan", Properties.Flags.Uzbekistan, 0),
            new SearchResult("Vanuatu", Properties.Flags.Vanuatu, 0),
            new SearchResult("Venezuela", Properties.Flags.Venezuela, 0),
            new SearchResult("Vietnam", Properties.Flags.Vietnam, 0),
            new SearchResult("Western Sahara", Properties.Flags.Western_Sahara, 0),
            new SearchResult("Yemen", Properties.Flags.Yemen, 0),
            new SearchResult("Zambia", Properties.Flags.Zambia, 0),
            new SearchResult("Zimbabwe", Properties.Flags.Zimbabwe ,0)
        ];

        /// <summary>
        /// Returns an array of all country names.
        /// </summary>
        /// <returns>String array containing all country names</returns>
        public static string[] GetAllCountryNames()
        {
            return CountrySearchResults
                .Where(result => !string.IsNullOrEmpty(result.Name) &&
                                result.Name != "Unkown" &&
                                result.Name != SearchBox.AddLine)
                .Select(result => result.Name)
                .ToArray();
        }

        public static string NormalizeCountryName(string countryOfOrigin)
        {
            if (string.IsNullOrWhiteSpace(countryOfOrigin))
            {
                return countryOfOrigin;
            }

            // Create a case-insensitive comparison dictionary for country variants
            Dictionary<string, string> countryVariants = new(StringComparer.OrdinalIgnoreCase)
            {
                // United States variants
                ["US"] = "United States",
                ["USA"] = "United States",
                ["U.S."] = "United States",
                ["U.S.A."] = "United States",
                ["United States of America"] = "United States",
                ["America"] = "United States",
                ["States"] = "United States",
                ["The States"] = "United States",

                // China variants
                ["CN"] = "China",
                ["CHN"] = "China",
                ["PRC"] = "China",
                ["People's Republic of China"] = "China",
                ["Mainland China"] = "China",

                // Germany variants
                ["DE"] = "Germany",
                ["DEU"] = "Germany",
                ["Deutschland"] = "Germany",
                ["Federal Republic of Germany"] = "Germany",

                // Japan variants
                ["JP"] = "Japan",
                ["JPN"] = "Japan",
                ["Nippon"] = "Japan",

                // United Kingdom variants
                ["GB"] = "United Kingdom",
                ["GBR"] = "United Kingdom",
                ["UK"] = "United Kingdom",
                ["U.K."] = "United Kingdom",
                ["Great Britain"] = "United Kingdom",
                ["Britain"] = "United Kingdom",
                ["England"] = "United Kingdom",
                ["Scotland"] = "United Kingdom",
                ["Wales"] = "United Kingdom",
                ["Northern Ireland"] = "United Kingdom",

                // France variants
                ["FR"] = "France",
                ["FRA"] = "France",
                ["French Republic"] = "France",

                // Italy variants
                ["IT"] = "Italy",
                ["ITA"] = "Italy",
                ["Italian Republic"] = "Italy",

                // Canada variants
                ["CA"] = "Canada",
                ["CAN"] = "Canada",

                // Afghanistan variants
                ["AF"] = "Afghanistan",
                ["AFG"] = "Afghanistan",

                // Albania variants
                ["AL"] = "Albania",
                ["ALB"] = "Albania",

                // Algeria variants
                ["DZ"] = "Algeria",
                ["DZA"] = "Algeria",

                // Andorra variants
                ["AD"] = "Andorra",
                ["AND"] = "Andorra",

                // Angola variants
                ["AO"] = "Angola",
                ["AGO"] = "Angola",

                // Antigua and Barbuda variants
                ["AG"] = "Antigua and Barbuda",
                ["ATG"] = "Antigua and Barbuda",

                // Argentina variants
                ["AR"] = "Argentina",
                ["ARG"] = "Argentina",
                ["Argentine Republic"] = "Argentina",

                // Armenia variants
                ["AM"] = "Armenia",
                ["ARM"] = "Armenia",

                // Australia variants
                ["AU"] = "Australia",
                ["AUS"] = "Australia",
                ["Commonwealth of Australia"] = "Australia",

                // Austria variants
                ["AT"] = "Austria",
                ["AUT"] = "Austria",
                ["Republic of Austria"] = "Austria",

                // Azerbaijan variants
                ["AZ"] = "Azerbaijan",
                ["AZE"] = "Azerbaijan",

                // Bahamas variants
                ["BS"] = "Bahamas",
                ["BHS"] = "Bahamas",

                // Bahrain variants
                ["BH"] = "Bahrain",
                ["BHR"] = "Bahrain",

                // Bangladesh variants
                ["BD"] = "Bangladesh",
                ["BGD"] = "Bangladesh",

                // Barbados variants
                ["BB"] = "Barbados",
                ["BRB"] = "Barbados",

                // Belarus variants
                ["BY"] = "Belarus",
                ["BLR"] = "Belarus",
                ["Republic of Belarus"] = "Belarus",

                // Belgium variants
                ["BE"] = "Belgium",
                ["BEL"] = "Belgium",
                ["Kingdom of Belgium"] = "Belgium",

                // Belize variants
                ["BZ"] = "Belize",
                ["BLZ"] = "Belize",

                // Benin variants
                ["BJ"] = "Benin",
                ["BEN"] = "Benin",

                // Bhutan variants
                ["BT"] = "Bhutan",
                ["BTN"] = "Bhutan",

                // Bolivia variants
                ["BO"] = "Bolivia",
                ["BOL"] = "Bolivia",

                // Bosnia and Herzegovina variants
                ["BA"] = "Bosnia and Herzegovina",
                ["BIH"] = "Bosnia and Herzegovina",

                // Botswana variants
                ["BW"] = "Botswana",
                ["BWA"] = "Botswana",

                // Brazil variants
                ["BR"] = "Brazil",
                ["BRA"] = "Brazil",
                ["Federative Republic of Brazil"] = "Brazil",

                // Brunei variants
                ["BN"] = "Brunei",
                ["BRN"] = "Brunei",

                // Bulgaria variants
                ["BG"] = "Bulgaria",
                ["BGR"] = "Bulgaria",
                ["Republic of Bulgaria"] = "Bulgaria",

                // Burkina Faso variants
                ["BF"] = "Burkina Faso",
                ["BFA"] = "Burkina Faso",

                // Burundi variants
                ["BI"] = "Burundi",
                ["BDI"] = "Burundi",

                // Cabo Verde variants
                ["CV"] = "Cabo Verde",
                ["CPV"] = "Cabo Verde",
                ["Cape Verde"] = "Cabo Verde",

                // Cambodia variants
                ["KH"] = "Cambodia",
                ["KHM"] = "Cambodia",

                // Cameroon variants
                ["CM"] = "Cameroon",
                ["CMR"] = "Cameroon",

                // Central African Republic variants
                ["CF"] = "Central African Republic",
                ["CAF"] = "Central African Republic",

                // Chad variants
                ["TD"] = "Chad",
                ["TCD"] = "Chad",

                // Chile variants
                ["CL"] = "Chile",
                ["CHL"] = "Chile",

                // Colombia variants
                ["CO"] = "Colombia",
                ["COL"] = "Colombia",

                // Comoros variants
                ["KM"] = "Comoros",
                ["COM"] = "Comoros",

                // Costa Rica variants
                ["CR"] = "Costa Rica",
                ["CRI"] = "Costa Rica",

                // Croatia variants
                ["HR"] = "Croatia",
                ["HRV"] = "Croatia",
                ["Republic of Croatia"] = "Croatia",

                // Cuba variants
                ["CU"] = "Cuba",
                ["CUB"] = "Cuba",

                // Cyprus variants
                ["CY"] = "Cyprus",
                ["CYP"] = "Cyprus",

                // Czechia variants
                ["CZ"] = "Czechia",
                ["CZE"] = "Czechia",
                ["Czech Republic"] = "Czechia",

                // Denmark variants
                ["DK"] = "Denmark",
                ["DNK"] = "Denmark",
                ["Kingdom of Denmark"] = "Denmark",

                // Djibouti variants
                ["DJ"] = "Djibouti",
                ["DJI"] = "Djibouti",

                // Dominica variants
                ["DM"] = "Dominica",
                ["DMA"] = "Dominica",

                // Dominican Republic variants
                ["DO"] = "Dominican Republic",
                ["DOM"] = "Dominican Republic",

                // Ecuador variants
                ["EC"] = "Ecuador",
                ["ECU"] = "Ecuador",

                // Egypt variants
                ["EG"] = "Egypt",
                ["EGY"] = "Egypt",
                ["Arab Republic of Egypt"] = "Egypt",

                // El Salvador variants
                ["SV"] = "El Salvador",
                ["SLV"] = "El Salvador",

                // Equatorial Guinea variants
                ["GQ"] = "Equatorial Guinea",
                ["GNQ"] = "Equatorial Guinea",

                // Eritrea variants
                ["ER"] = "Eritrea",
                ["ERI"] = "Eritrea",

                // Estonia variants
                ["EE"] = "Estonia",
                ["EST"] = "Estonia",
                ["Republic of Estonia"] = "Estonia",

                // Eswatini variants
                ["SZ"] = "Eswatini",
                ["SWZ"] = "Eswatini",
                ["Swaziland"] = "Eswatini",

                // Ethiopia variants
                ["ET"] = "Ethiopia",
                ["ETH"] = "Ethiopia",

                // Fiji variants
                ["FJ"] = "Fiji",
                ["FJI"] = "Fiji",

                // Finland variants
                ["FI"] = "Finland",
                ["FIN"] = "Finland",
                ["Republic of Finland"] = "Finland",

                // Gabon variants
                ["GA"] = "Gabon",
                ["GAB"] = "Gabon",

                // Gambia variants
                ["GM"] = "Gambia",
                ["GMB"] = "Gambia",

                // Georgia variants
                ["GE"] = "Georgia",
                ["GEO"] = "Georgia",

                // Ghana variants
                ["GH"] = "Ghana",
                ["GHA"] = "Ghana",

                // Greece variants
                ["GR"] = "Greece",
                ["GRC"] = "Greece",
                ["Hellenic Republic"] = "Greece",

                // Grenada variants
                ["GD"] = "Grenada",
                ["GRD"] = "Grenada",

                // Guatemala variants
                ["GT"] = "Guatemala",
                ["GTM"] = "Guatemala",

                // Guinea variants
                ["GN"] = "Guinea",
                ["GIN"] = "Guinea",

                // Guinea-Bissau variants
                ["GW"] = "Guinea-Bissau",
                ["GNB"] = "Guinea-Bissau",

                // Guyana variants
                ["GY"] = "Guyana",
                ["GUY"] = "Guyana",

                // Haiti variants
                ["HT"] = "Haiti",
                ["HTI"] = "Haiti",

                // Honduras variants
                ["HN"] = "Honduras",
                ["HND"] = "Honduras",

                // Hungary variants
                ["HU"] = "Hungary",
                ["HUN"] = "Hungary",

                // Iceland variants
                ["IS"] = "Iceland",
                ["ISL"] = "Iceland",

                // India variants
                ["IN"] = "India",
                ["IND"] = "India",
                ["Republic of India"] = "India",
                ["Bharat"] = "India",

                // Indonesia variants
                ["ID"] = "Indonesia",
                ["IDN"] = "Indonesia",

                // Iran variants
                ["IR"] = "Iran",
                ["IRN"] = "Iran",

                // Iraq variants
                ["IQ"] = "Iraq",
                ["IRQ"] = "Iraq",

                // Ireland variants
                ["IE"] = "Ireland",
                ["IRL"] = "Ireland",
                ["Republic of Ireland"] = "Ireland",

                // Israel variants
                ["IL"] = "Israel",
                ["ISR"] = "Israel",
                ["State of Israel"] = "Israel",

                // Ivory Coast variants
                ["CI"] = "Ivory Coast",
                ["CIV"] = "Ivory Coast",
                ["Côte d'Ivoire"] = "Ivory Coast",

                // Jamaica variants
                ["JM"] = "Jamaica",
                ["JAM"] = "Jamaica",

                // Jordan variants
                ["JO"] = "Jordan",
                ["JOR"] = "Jordan",

                // Kazakhstan variants
                ["KZ"] = "Kazakhstan",
                ["KAZ"] = "Kazakhstan",

                // Kenya variants
                ["KE"] = "Kenya",
                ["KEN"] = "Kenya",

                // Kiribati variants
                ["KI"] = "Kiribati",
                ["KIR"] = "Kiribati",

                // Kuwait variants
                ["KW"] = "Kuwait",
                ["KWT"] = "Kuwait",

                // Kyrgyzstan variants
                ["KG"] = "Kyrgyzstan",
                ["KGZ"] = "Kyrgyzstan",

                // Lao variants
                ["LA"] = "Lao",
                ["LAO"] = "Lao",
                ["Laos"] = "Lao",

                // Latvia variants
                ["LV"] = "Latvia",
                ["LVA"] = "Latvia",
                ["Republic of Latvia"] = "Latvia",

                // Lebanon variants
                ["LB"] = "Lebanon",
                ["LBN"] = "Lebanon",

                // Lesotho variants
                ["LS"] = "Lesotho",
                ["LSO"] = "Lesotho",

                // Liberia variants
                ["LR"] = "Liberia",
                ["LBR"] = "Liberia",

                // Libya variants
                ["LY"] = "Libya",
                ["LBY"] = "Libya",

                // Liechtenstein variants
                ["LI"] = "Liechtenstein",
                ["LIE"] = "Liechtenstein",

                // Lithuania variants
                ["LT"] = "Lithuania",
                ["LTU"] = "Lithuania",
                ["Republic of Lithuania"] = "Lithuania",

                // Luxembourg variants
                ["LU"] = "Luxembourg",
                ["LUX"] = "Luxembourg",

                // Madagascar variants
                ["MG"] = "Madagascar",
                ["MDG"] = "Madagascar",

                // Malawi variants
                ["MW"] = "Malawi",
                ["MWI"] = "Malawi",

                // Malaysia variants
                ["MY"] = "Malaysia",
                ["MYS"] = "Malaysia",

                // Maldives variants
                ["MV"] = "Maldives",
                ["MDV"] = "Maldives",

                // Mali variants
                ["ML"] = "Mali",
                ["MLI"] = "Mali",

                // Malta variants
                ["MT"] = "Malta",
                ["MLT"] = "Malta",

                // Marshall Islands variants
                ["MH"] = "Marshall Islands",
                ["MHL"] = "Marshall Islands",

                // Mauritania variants
                ["MR"] = "Mauritania",
                ["MRT"] = "Mauritania",

                // Mauritius variants
                ["MU"] = "Mauritius",
                ["MUS"] = "Mauritius",

                // Mexico variants
                ["MX"] = "Mexico",
                ["MEX"] = "Mexico",
                ["United Mexican States"] = "Mexico",

                // Micronesia variants
                ["FM"] = "Micronesia",
                ["FSM"] = "Micronesia",

                // Moldova variants
                ["MD"] = "Moldova",
                ["MDA"] = "Moldova",
                ["Republic of Moldova"] = "Moldova",

                // Monaco variants
                ["MC"] = "Monaco",
                ["MCO"] = "Monaco",

                // Mongolia variants
                ["MN"] = "Mongolia",
                ["MNG"] = "Mongolia",

                // Montenegro variants
                ["ME"] = "Montenegro",
                ["MNE"] = "Montenegro",

                // Morocco variants
                ["MA"] = "Morocco",
                ["MAR"] = "Morocco",

                // Mozambique variants
                ["MZ"] = "Mozambique",
                ["MOZ"] = "Mozambique",

                // Myanmar variants
                ["MM"] = "Myanmar",
                ["MMR"] = "Myanmar",
                ["Burma"] = "Myanmar",

                // Namibia variants
                ["NA"] = "Namibia",
                ["NAM"] = "Namibia",

                // Nauru variants
                ["NR"] = "Nauru",
                ["NRU"] = "Nauru",

                // Nepal variants
                ["NP"] = "Nepal",
                ["NPL"] = "Nepal",

                // Netherlands variants
                ["NL"] = "Netherlands",
                ["NLD"] = "Netherlands",
                ["Holland"] = "Netherlands",
                ["Kingdom of the Netherlands"] = "Netherlands",

                // New Zealand variants
                ["NZ"] = "New Zealand",
                ["NZL"] = "New Zealand",
                ["Aotearoa"] = "New Zealand",

                // Nicaragua variants
                ["NI"] = "Nicaragua",
                ["NIC"] = "Nicaragua",

                // Niger variants
                ["NE"] = "Niger",
                ["NER"] = "Niger",

                // Nigeria variants
                ["NG"] = "Nigeria",
                ["NGA"] = "Nigeria",

                // North Korea variants
                ["KP"] = "North Korea",
                ["PRK"] = "North Korea",
                ["Democratic People's Republic of Korea"] = "North Korea",
                ["DPRK"] = "North Korea",

                // North Macedonia variants
                ["MK"] = "North Macedonia",
                ["MKD"] = "North Macedonia",
                ["Macedonia"] = "North Macedonia",

                // Norway variants
                ["NO"] = "Norway",
                ["NOR"] = "Norway",
                ["Kingdom of Norway"] = "Norway",

                // Oman variants
                ["OM"] = "Oman",
                ["OMN"] = "Oman",

                // Pakistan variants
                ["PK"] = "Pakistan",
                ["PAK"] = "Pakistan",

                // Palau variants
                ["PW"] = "Palau",
                ["PLW"] = "Palau",

                // Panama variants
                ["PA"] = "Panama",
                ["PAN"] = "Panama",

                // Papua New Guinea variants
                ["PG"] = "Papua New Guinea",
                ["PNG"] = "Papua New Guinea",

                // Paraguay variants
                ["PY"] = "Paraguay",
                ["PRY"] = "Paraguay",

                // Peru variants
                ["PE"] = "Peru",
                ["PER"] = "Peru",

                // Philippines variants
                ["PH"] = "Philippines",
                ["PHL"] = "Philippines",

                // Poland variants
                ["PL"] = "Poland",
                ["POL"] = "Poland",
                ["Republic of Poland"] = "Poland",

                // Portugal variants
                ["PT"] = "Portugal",
                ["PRT"] = "Portugal",
                ["Portuguese Republic"] = "Portugal",

                // Qatar variants
                ["QA"] = "Qatar",
                ["QAT"] = "Qatar",

                // Romania variants
                ["RO"] = "Romania",
                ["ROU"] = "Romania",

                // Russia variants
                ["RU"] = "Russia",
                ["RUS"] = "Russia",
                ["Russian Federation"] = "Russia",
                ["USSR"] = "Russia",
                ["Soviet Union"] = "Russia",

                // Rwanda variants
                ["RW"] = "Rwanda",
                ["RWA"] = "Rwanda",

                // Saint Kitts and Nevis variants
                ["KN"] = "Saint Kitts and Nevis",
                ["KNA"] = "Saint Kitts and Nevis",

                // Saint Lucia variants
                ["LC"] = "Saint Lucia",
                ["LCA"] = "Saint Lucia",

                // Saint Vincent and the Grenadines variants
                ["VC"] = "Saint Vincent and the Grenadines",
                ["VCT"] = "Saint Vincent and the Grenadines",

                // Samoa variants
                ["WS"] = "Samoa",
                ["WSM"] = "Samoa",

                // San Marino variants
                ["SM"] = "San Marino",
                ["SMR"] = "San Marino",

                // Sao Tome and Principe variants
                ["ST"] = "Sao Tome and Principe",
                ["STP"] = "Sao Tome and Principe",

                // Saudi Arabia variants
                ["SA"] = "Saudi Arabia",
                ["SAU"] = "Saudi Arabia",

                // Senegal variants
                ["SN"] = "Senegal",
                ["SEN"] = "Senegal",

                // Serbia variants
                ["RS"] = "Serbia",
                ["SRB"] = "Serbia",

                // Seychelles variants
                ["SC"] = "Seychelles",
                ["SYC"] = "Seychelles",

                // Sierra Leone variants
                ["SL"] = "Sierra Leone",
                ["SLE"] = "Sierra Leone",

                // Singapore variants
                ["SG"] = "Singapore",
                ["SGP"] = "Singapore",
                ["Republic of Singapore"] = "Singapore",

                // Slovakia variants
                ["SK"] = "Slovakia",
                ["SVK"] = "Slovakia",
                ["Slovak Republic"] = "Slovakia",

                // Slovenia variants
                ["SI"] = "Slovenia",
                ["SVN"] = "Slovenia",
                ["Republic of Slovenia"] = "Slovenia",

                // Solomon Islands variants
                ["SB"] = "Solomon Islands",
                ["SLB"] = "Solomon Islands",

                // Somalia variants
                ["SO"] = "Somalia",
                ["SOM"] = "Somalia",

                // South Africa variants
                ["ZA"] = "South Africa",
                ["ZAF"] = "South Africa",
                ["Republic of South Africa"] = "South Africa",

                // South Korea variants
                ["KR"] = "South Korea",
                ["KOR"] = "South Korea",
                ["Korea"] = "South Korea",
                ["Republic of Korea"] = "South Korea",
                ["ROK"] = "South Korea",

                // South Sudan variants
                ["SS"] = "South Sudan",
                ["SSD"] = "South Sudan",

                // Spain variants
                ["ES"] = "Spain",
                ["ESP"] = "Spain",
                ["Kingdom of Spain"] = "Spain",

                // Sri Lanka variants
                ["LK"] = "Sri Lanka",
                ["LKA"] = "Sri Lanka",

                // Sudan variants
                ["SD"] = "Sudan",
                ["SDN"] = "Sudan",

                // Suriname variants
                ["SR"] = "Suriname",
                ["SUR"] = "Suriname",

                // Sweden variants
                ["SE"] = "Sweden",
                ["SWE"] = "Sweden",
                ["Kingdom of Sweden"] = "Sweden",

                // Switzerland variants
                ["CH"] = "Switzerland",
                ["CHE"] = "Switzerland",
                ["Swiss Confederation"] = "Switzerland",

                // Syria variants
                ["SY"] = "Syria",
                ["SYR"] = "Syria",

                // Taiwan variants
                ["TW"] = "Taiwan",
                ["TWN"] = "Taiwan",
                ["ROC"] = "Taiwan",
                ["Republic of China"] = "Taiwan",

                // Tajikistan variants
                ["TJ"] = "Tajikistan",
                ["TJK"] = "Tajikistan",

                // Tanzania variants
                ["TZ"] = "Tanzania",
                ["TZA"] = "Tanzania",

                // Thailand variants
                ["TH"] = "Thailand",
                ["THA"] = "Thailand",
                ["Kingdom of Thailand"] = "Thailand",

                // The Democratic Republic of the Congo variants
                ["CD"] = "The Democratic Republic of the Congo",
                ["COD"] = "The Democratic Republic of the Congo",
                ["DRC"] = "The Democratic Republic of the Congo",
                ["Congo-Kinshasa"] = "The Democratic Republic of the Congo",

                // The Republic of the Congo variants
                ["CG"] = "The Republic of the Congo",
                ["COG"] = "The Republic of the Congo",
                ["Congo-Brazzaville"] = "The Republic of the Congo",

                // Timor-Leste variants
                ["TL"] = "Timor-Leste",
                ["TLS"] = "Timor-Leste",
                ["East Timor"] = "Timor-Leste",

                // Togo variants
                ["TG"] = "Togo",
                ["TGO"] = "Togo",

                // Tonga variants
                ["TO"] = "Tonga",
                ["TON"] = "Tonga",

                // Trinidad and Tobago variants
                ["TT"] = "Trinidad and Tobago",
                ["TTO"] = "Trinidad and Tobago",

                // Tunisia variants
                ["TN"] = "Tunisia",
                ["TUN"] = "Tunisia",

                // Turkey variants
                ["TR"] = "Turkey",
                ["TUR"] = "Turkey",
                ["Republic of Turkey"] = "Turkey",

                // Turkmenistan variants
                ["TM"] = "Turkmenistan",
                ["TKM"] = "Turkmenistan",

                // Tuvalu variants
                ["TV"] = "Tuvalu",
                ["TUV"] = "Tuvalu",

                // Uganda variants
                ["UG"] = "Uganda",
                ["UGA"] = "Uganda",

                // Ukraine variants
                ["UA"] = "Ukraine",
                ["UKR"] = "Ukraine",

                // United Arab Emirates variants
                ["AE"] = "United Arab Emirates",
                ["ARE"] = "United Arab Emirates",
                ["UAE"] = "United Arab Emirates",

                // Uruguay variants
                ["UY"] = "Uruguay",
                ["URY"] = "Uruguay",

                // Uzbekistan variants
                ["UZ"] = "Uzbekistan",
                ["UZB"] = "Uzbekistan",

                // Vanuatu variants
                ["VU"] = "Vanuatu",
                ["VUT"] = "Vanuatu",

                // Venezuela variants
                ["VE"] = "Venezuela",
                ["VEN"] = "Venezuela",

                // Vietnam variants
                ["VN"] = "Vietnam",
                ["VNM"] = "Vietnam",
                ["Socialist Republic of Vietnam"] = "Vietnam",

                // Western Sahara variants
                ["EH"] = "Western Sahara",
                ["ESH"] = "Western Sahara",

                // Yemen variants
                ["YE"] = "Yemen",
                ["YEM"] = "Yemen",

                // Zambia variants
                ["ZM"] = "Zambia",
                ["ZMB"] = "Zambia",

                // Zimbabwe variants
                ["ZW"] = "Zimbabwe",
                ["ZWE"] = "Zimbabwe"
            };

            // Check if the country name is in our variants dictionary
            if (countryVariants.TryGetValue(countryOfOrigin, out string normalizedName))
            {
                return normalizedName;
            }

            // If no variant found, return the original name
            return countryOfOrigin;
        }
        public static string MapCountryNameToCode(string countryName)
        {
            Dictionary<string, string> countryMappings = new(StringComparer.OrdinalIgnoreCase)
            {
                { "Afghanistan", "afg" },
                { "Albania", "alb" },
                { "Algeria", "dza" },
                { "Andorra", "and" },
                { "Angola", "ago" },
                { "Antigua and Barbuda", "atg" },
                { "Argentina", "arg" },
                { "Armenia", "arm" },
                { "Australia", "aus" },
                { "Austria", "aut" },
                { "Azerbaijan", "aze" },
                { "Bahamas", "bhs" },
                { "Bahrain", "bhr" },
                { "Bangladesh", "bgd" },
                { "Barbados", "brb" },
                { "Belarus", "blr" },
                { "Belgium", "bel" },
                { "Belize", "blz" },
                { "Benin", "ben" },
                { "Bhutan", "btn" },
                { "Bolivia", "bol" },
                { "Bosnia and Herzegovina", "bih" },
                { "Botswana", "bwa" },
                { "Brazil", "bra" },
                { "Brunei", "brn" },
                { "Bulgaria", "bgr" },
                { "Burkina Faso", "bfa" },
                { "Burundi", "bdi" },
                { "Cabo Verde", "cpv" },
                { "Cambodia", "khm" },
                { "Cameroon", "cmr" },
                { "Canada", "can" },
                { "Central African Republic", "caf" },
                { "Chad", "tcd" },
                { "Chile", "chl" },
                { "China", "chn" },
                { "Colombia", "col" },
                { "Comoros", "com" },
                { "Costa Rica", "cri" },
                { "Croatia", "hrv" },
                { "Cuba", "cub" },
                { "Cyprus", "cyp" },
                { "Czechia", "cze" },
                { "Denmark", "dnk" },
                { "Djibouti", "dji" },
                { "Dominica", "dma" },
                { "Dominican Republic", "dom" },
                { "Ecuador", "ecu" },
                { "Egypt", "egy" },
                { "El Salvador", "slv" },
                { "Equatorial Guinea", "gnq" },
                { "Eritrea", "eri" },
                { "Estonia", "est" },
                { "Eswatini", "swz" },
                { "Ethiopia", "eth" },
                { "Fiji", "fji" },
                { "Finland", "fin" },
                { "France", "fra" },
                { "Gabon", "gab" },
                { "Gambia", "gmb" },
                { "Georgia", "geo" },
                { "Germany", "deu" },
                { "Ghana", "gha" },
                { "Greece", "grc" },
                { "Grenada", "grd" },
                { "Guatemala", "gtm" },
                { "Guinea", "gin" },
                { "Guinea-Bissau", "gnb" },
                { "Guyana", "guy" },
                { "Haiti", "hti" },
                { "Honduras", "hnd" },
                { "Hungary", "hun" },
                { "Iceland", "isl" },
                { "India", "ind" },
                { "Indonesia", "idn" },
                { "Iran", "irn" },
                { "Iraq", "irq" },
                { "Ireland", "irl" },
                { "Israel", "isr" },
                { "Italy", "ita" },
                { "Ivory Coast", "civ" },
                { "Jamaica", "jam" },
                { "Japan", "jpn" },
                { "Jordan", "jor" },
                { "Kazakhstan", "kaz" },
                { "Kenya", "ken" },
                { "Kiribati", "kir" },
                { "Kuwait", "kwt" },
                { "Kyrgyzstan", "kgz" },
                { "Lao", "lao" },
                { "Latvia", "lva" },
                { "Lebanon", "lbn" },
                { "Lesotho", "lso" },
                { "Liberia", "lbr" },
                { "Libya", "lby" },
                { "Liechtenstein", "lie" },
                { "Lithuania", "ltu" },
                { "Luxembourg", "lux" },
                { "Madagascar", "mdg" },
                { "Malawi", "mwi" },
                { "Malaysia", "mys" },
                { "Maldives", "mdv" },
                { "Mali", "mli" },
                { "Malta", "mlt" },
                { "Marshall Islands", "mhl" },
                { "Mauritania", "mrt" },
                { "Mauritius", "mus" },
                { "Mexico", "mex" },
                { "Micronesia", "fsm" },
                { "Moldova", "mda" },
                { "Monaco", "mco" },
                { "Mongolia", "mng" },
                { "Montenegro", "mne" },
                { "Morocco", "mar" },
                { "Mozambique", "moz" },
                { "Myanmar", "mmr" },
                { "Namibia", "nam" },
                { "Nauru", "nru" },
                { "Nepal", "npl" },
                { "Netherlands", "nld" },
                { "New Zealand", "nzl" },
                { "Nicaragua", "nic" },
                { "Niger", "ner" },
                { "Nigeria", "nga" },
                { "North Korea", "prk" },
                { "North Macedonia", "mkd" },
                { "Norway", "nor" },
                { "Oman", "omn" },
                { "Pakistan", "pak" },
                { "Palau", "plw" },
                { "Panama", "pan" },
                { "Papua New Guinea", "png" },
                { "Paraguay", "pry" },
                { "Peru", "per" },
                { "Philippines", "phl" },
                { "Poland", "pol" },
                { "Portugal", "prt" },
                { "Qatar", "qat" },
                { "Romania", "rou" },
                { "Russia", "rus" },
                { "Rwanda", "rwa" },
                { "Saint Kitts and Nevis", "kna" },
                { "Saint Lucia", "lca" },
                { "Saint Vincent and the Grenadines", "vct" },
                { "Samoa", "wsm" },
                { "San Marino", "smr" },
                { "Sao Tome and Principe", "stp" },
                { "Saudi Arabia", "sau" },
                { "Senegal", "sen" },
                { "Serbia", "srb" },
                { "Seychelles", "syc" },
                { "Sierra Leone", "sle" },
                { "Singapore", "sgp" },
                { "Slovakia", "svk" },
                { "Slovenia", "svn" },
                { "Solomon Islands", "slb" },
                { "Somalia", "som" },
                { "South Africa", "zaf" },
                { "South Korea", "kor" },
                { "South Sudan", "ssd" },
                { "Spain", "esp" },
                { "Sri Lanka", "lka" },
                { "Sudan", "sdn" },
                { "Suriname", "sur" },
                { "Sweden", "swe" },
                { "Switzerland", "che" },
                { "Syria", "syr" },
                { "Taiwan", "twn" },
                { "Tajikistan", "tjk" },
                { "Tanzania", "tza" },
                { "Thailand", "tha" },
                { "The Democratic Republic of the Congo", "cod" },
                { "The Republic of the Congo", "cog" },
                { "Timor-Leste", "tls" },
                { "Togo", "tgo" },
                { "Tonga", "ton" },
                { "Trinidad and Tobago", "tto" },
                { "Tunisia", "tun" },
                { "Turkey", "tur" },
                { "Turkmenistan", "tkm" },
                { "Tuvalu", "tuv" },
                { "Uganda", "uga" },
                { "Ukraine", "ukr" },
                { "United Arab Emirates", "are" },
                { "United Kingdom", "gbr" },
                { "United States", "usa" },
                { "Uruguay", "ury" },
                { "Uzbekistan", "uzb" },
                { "Vanuatu", "vut" },
                { "Venezuela", "ven" },
                { "Vietnam", "vnm" },
                { "Western Sahara", "esh" },
                { "Yemen", "yem" },
                { "Zambia", "zmb" },
                { "Zimbabwe", "zwe" }
            };

            return countryMappings.TryGetValue(countryName, out string? code) ? code : "";
        }
    }
}
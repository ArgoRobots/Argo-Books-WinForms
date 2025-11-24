using Newtonsoft.Json;
using Argo_Books.Classes;
using System.Security.Cryptography;
using System.Text;

namespace Argo_Books.AnonymousData
{
    public static class GeoLocationService
    {
        private static GeoLocationData? _cachedLocation;
        private static DateTime _lastFetched = DateTime.MinValue;
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromHours(12);
        private static readonly string[] GeoApiEndpoints = [
            "http://ip-api.com/json/?fields=status,country,countryCode,region,city,timezone,proxy",
            "https://ipapi.co/json/",
            "https://ipinfo.io/json"
        ];

        public static async Task<GeoLocationData> GetLocationAsync()
        {
            // Return cached data if still valid
            if (_cachedLocation != null && DateTime.Now - _lastFetched < CacheExpiry)
            {
                return _cachedLocation;
            }

            // Try multiple geo-location services for reliability
            foreach (string endpoint in GeoApiEndpoints)
            {
                try
                {
                    using HttpClient client = new();
                    client.Timeout = TimeSpan.FromSeconds(8);
                    client.DefaultRequestHeaders.Add("User-Agent", ApiConfig.UserAgent);

                    string response = await client.GetStringAsync(endpoint);
                    dynamic? locationData = JsonConvert.DeserializeObject<dynamic>(response);

                    GeoLocationData geoData = new();

                    // Parse different API response formats
                    if (endpoint.Contains("ip-api.com"))
                        if (locationData?.status == "success")
                        {
                            geoData = ParseIpApiResponse(locationData);
                        }
                        else if (endpoint.Contains("ipapi.co"))
                        {
                            geoData = ParseIpApiCoResponse(locationData);
                        }
                        else if (endpoint.Contains("ipinfo.io"))
                        {
                            geoData = ParseIpInfoResponse(locationData);
                        }

                    if (geoData.Country != "Unknown")
                    {
                        _cachedLocation = geoData;
                        _lastFetched = DateTime.Now;
                        return _cachedLocation;
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteWithFormat(1, "Geo-location service {0} failed: {1}", endpoint, ex.Message);
                    continue;  // Try next service
                }
            }

            Log.Error_AnonymousDataCollection("All geo-location services failed, using default location");
            _cachedLocation = new GeoLocationData();
            return _cachedLocation;
        }
        private static GeoLocationData ParseIpApiResponse(dynamic data)
        {
            return new GeoLocationData
            {
                Country = data.country?.ToString() ?? "Unknown",
                CountryCode = data.countryCode?.ToString() ?? "Unknown",
                Region = data.region?.ToString() ?? "Unknown",
                City = data.city?.ToString() ?? "Unknown",
                Timezone = data.timezone?.ToString() ?? "Unknown",
                IsVPN = data.proxy == true
            };
        }
        private static GeoLocationData ParseIpApiCoResponse(dynamic data)
        {
            return new GeoLocationData
            {
                Country = data.country_name?.ToString() ?? "Unknown",
                CountryCode = data.country_code?.ToString() ?? "Unknown",
                Region = data.region?.ToString() ?? "Unknown",
                City = data.city?.ToString() ?? "Unknown",
                Timezone = data.timezone?.ToString() ?? "Unknown",
                IsVPN = false  // ipapi.co doesn't provide VPN detection in free tier
            };
        }
        private static GeoLocationData ParseIpInfoResponse(dynamic data)
        {
            return new GeoLocationData
            {
                Country = data.country?.ToString() ?? "Unknown",
                CountryCode = data.country?.ToString() ?? "Unknown",
                Region = data.region?.ToString() ?? "Unknown",
                City = data.city?.ToString() ?? "Unknown",
                Timezone = data.timezone?.ToString() ?? "Unknown",
                IsVPN = false  // ipinfo.io doesn't provide VPN detection in free tier
            };
        }
        public static string HashIP(string ip, string salt = "AnonymousTracker2025")
        {
            try
            {
                byte[] hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(ip + salt));
                return Convert.ToBase64String(hashedBytes)[..16];  // Truncate for storage
            }
            catch
            {
                return "HashFailed";
            }
        }
        public static async Task<string> GetHashedIPAsync()
        {
            try
            {
                using HttpClient client = new();
                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestHeaders.Add("User-Agent", ApiConfig.UserAgent);
                string ip = await client.GetStringAsync("https://api.ipify.org");
                return HashIP(ip.Trim());
            }
            catch
            {
                return "IPUnavailable";
            }
        }
    }
}
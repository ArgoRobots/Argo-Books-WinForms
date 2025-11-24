using Argo_Books;
using Argo_Books.Classes;
using System.Net;
using System.Net.NetworkInformation;

namespace Argo_Books.Classes
{
    /// <summary>
    /// Utility class for checking internet connectivity using multiple methods.
    /// </summary>
    public static class InternetConnectionManager
    {
        private static readonly HttpClient _httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(5)
        };

        private static readonly string[] httpTestUrls = [
            "https://www.google.com/generate_204",              // Google connectivity endpoint
            "https://www.msftconnecttest.com/connecttest.txt",  // Microsoft connectivity endpoint
            "https://detectportal.firefox.com/success.txt",     // Mozilla connectivity endpoint
            "https://captive.apple.com/hotspot-detect.html"     // Apple connectivity endpoint
       ];

        /// <summary>
        /// Checks internet connectivity using multiple methods for maximum reliability.
        /// </summary>
        public static async Task<bool> IsInternetAvailableAsync()
        {
            try
            {
                // First check if any network interface is available
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    return false;
                }

                // Method 1: Try HTTP connectivity checks (most reliable)
                if (await CheckHttpConnectivityAsync())
                {
                    return true;
                }

                // Method 2: Try DNS resolution (works through proxies)
                if (await CheckDnsConnectivityAsync())
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.WriteWithFormat(1, "Internet connectivity check failed: {0}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Checks connectivity using HTTP requests to known endpoints.
        /// </summary>
        private static async Task<bool> CheckHttpConnectivityAsync()
        {
            try
            {
                IEnumerable<Task<bool>> tasks = httpTestUrls.Select(CheckSingleHttpEndpointAsync);
                bool[] results = await Task.WhenAll(tasks);
                return results.Any(success => success);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks a single HTTP endpoint for connectivity.
        /// </summary>
        private static async Task<bool> CheckSingleHttpEndpointAsync(string url)
        {
            try
            {
                using HttpResponseMessage response = await _httpClient.GetAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks connectivity by attempting DNS resolution.
        /// </summary>
        private static async Task<bool> CheckDnsConnectivityAsync()
        {
            try
            {
                IPHostEntry hostEntry = await Dns.GetHostEntryAsync("www.google.com");
                return hostEntry.AddressList.Length > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Shows a message box when no internet connection is detected.
        /// </summary>
        private static CustomMessageBoxResult ShowNoInternetMessage(string operationName = "this operation", bool showRetryOption = true)
        {
            return CustomMessageBox.ShowWithFormat("No Internet Connection",
                "An internet connection is required for {0}. Please check your connection and try again.",
                CustomMessageBoxIcon.Exclamation,
                showRetryOption ? CustomMessageBoxButtons.RetryCancel : CustomMessageBoxButtons.Ok,
                operationName);
        }

        /// <summary>
        /// Checks internet connectivity and shows appropriate message if unavailable.
        /// </summary>
        public static async Task<bool> CheckInternetAndShowMessageAsync(string operationName = "this operation", bool showRetryOption = true)
        {
            while (true)
            {
                if (await IsInternetAvailableAsync())
                {
                    return true;
                }

                CustomMessageBoxResult result = ShowNoInternetMessage(operationName, showRetryOption);
                if (result != CustomMessageBoxResult.Retry)
                {
                    return false;
                }
            }
        }
    }
}
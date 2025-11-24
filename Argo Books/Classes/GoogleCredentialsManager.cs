using Argo_Books.Encryption;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Argo_Books;

namespace Argo_Books.Classes
{
    public class GoogleCredentialsManager
    {
        private static readonly string[] Scopes =
        [
            "https://www.googleapis.com/auth/spreadsheets",
            "https://www.googleapis.com/auth/drive.file"
        ];

        /// <summary>
        /// Gets user credentials using OAuth from environment variables.
        /// First time will prompt user to authenticate in browser.
        /// </summary>
        public static async Task<UserCredential> GetUserCredentialAsync()
        {
            try
            {
                string clientId = DotEnv.Get("GOOGLE_CLIENT_ID");
                string clientSecret = DotEnv.Get("GOOGLE_CLIENT_SECRET");

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    throw new Exception("Missing Google OAuth credentials in .env file");
                }

                ClientSecrets secrets = new()
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                };

                // Store token in AppData
                string credPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "ArgoSalesTracker",
                    "google_token"
                );

                return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)
                );
            }
            catch
            {
                CustomMessageBox.Show(
                    "Authentication Error",
                    "Failed to authenticate with Google. Please check your credentials.",
                    CustomMessageBoxIcon.Error,
                    CustomMessageBoxButtons.Ok
                );
                throw;
            }
        }
    }
}
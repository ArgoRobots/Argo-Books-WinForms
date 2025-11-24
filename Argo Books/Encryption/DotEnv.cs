using Argo_Books.Classes;
using Argo_Books.Classes;

namespace Argo_Books.Encryption
{
    /// <summary>
    /// Provides functionality to load environment variables from encrypted secrets file
    /// and access them throughout the application.
    /// </summary>
    public static class DotEnv
    {
        private static readonly Dictionary<string, string> _envVars = [];

        /// <summary>
        /// Loads environment variables from encrypted secrets file.
        /// In development (Visual Studio) it, automatically creates an encrypted secrets file from the .env.
        /// </summary>
        public static void Load()
        {
            if (Tools.IsRunningInVisualStudio())
            {
                string envFilePath = FindEnvRelativeToSolution();

                if (envFilePath != null && File.Exists(envFilePath))
                {
                    ProductionSecretsManager.CreateEncryptedSecretsFile(envFilePath);
                }
            }

            if (File.Exists(Directories.SecretsFilePath))
            {
                LoadFromEncryptedSecrets();
            }
            else
            {
                Log.Error_ENVFileNotFound(Path.GetFileName(Directories.SecretsFilePath));
            }
        }

        /// <summary>
        /// In production, this file is included in the installation folder.
        /// </summary>
        private static void LoadFromEncryptedSecrets()
        {
            Dictionary<string, string> secrets = ProductionSecretsManager.LoadEncryptedSecrets();

            foreach (KeyValuePair<string, string> kvp in secrets)
            {
                _envVars[kvp.Key] = kvp.Value;
            }
        }

        /// <summary>
        /// Retrieves the value of an environment variable by its key.
        /// </summary>
        public static string? Get(string key)
        {
            if (_envVars.TryGetValue(key, out string? value))
            {
                return value;
            }

            Log.Error_ENVKeyNotFound(key);
            return null;
        }

        /// <summary>
        /// Locates a file by searching up through the directory hierarchy from the current directory.
        /// </summary>
        private static string? FindEnvRelativeToSolution()
        {
            string currentDir = Directory.GetCurrentDirectory();
            string fileName = ".env";

            // Navigate up until we find either the solution file or the .env file
            while (!string.IsNullOrEmpty(currentDir))
            {
                // Check if the .env file exists in the current directory
                string envFilePath = Path.Combine(currentDir, fileName);
                if (File.Exists(envFilePath))
                {
                    return envFilePath;
                }

                // Check if any .sln file exists in the current directory
                if (Directory.GetFiles(currentDir, "*.sln").Length != 0)
                {
                    // We found the solution directory, check for .env file
                    envFilePath = Path.Combine(currentDir, fileName);
                    if (File.Exists(envFilePath))
                    {
                        return envFilePath;
                    }
                    else
                    {
                        // .env doesn't exist in solution directory
                        return null;
                    }
                }

                // Move up one directory
                DirectoryInfo? parentDir = Directory.GetParent(currentDir);
                if (parentDir == null)
                {
                    break;  // We've reached the root directory
                }

                currentDir = parentDir.FullName;
            }

            return null;  // File not found
        }
    }
}
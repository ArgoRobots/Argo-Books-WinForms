using Argo_Books.Classes;
using Argo_Books.Classes;

namespace Argo_Books.Encryption
{
    /// <summary>
    /// Creates and reads an encrypted secrets file next to the executable.
    /// </summary>
    public static class ProductionSecretsManager
    {
        /// <summary>
        /// Creates an encrypted secrets file from the current .env file.
        /// This should be run during the build process to generate the encrypted file.
        /// </summary>
        public static void CreateEncryptedSecretsFile(string envFilePath)
        {
            if (!File.Exists(envFilePath))
            {
                Log.Error_FileDoesNotExist(envFilePath);
                throw new FileNotFoundException($"Environment file not found: {envFilePath}");
            }

            try
            {
                string envContent = File.ReadAllText(envFilePath);
                string encryptedContent = EncryptionManager.EncryptString(envContent, EncryptionManager.AesKey, EncryptionManager.AesIV);
                File.WriteAllText(Directories.SecretsFilePath, encryptedContent);
            }
            catch (Exception ex)
            {
                Log.Error_CreateEncryptedSecretsFile($"{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Loads and decrypts the secrets file, returning the environment variables as a dictionary.
        /// In production, this file is included in the installation folder.
        /// </summary>
        public static Dictionary<string, string> LoadEncryptedSecrets()
        {
            if (!File.Exists(Directories.SecretsFilePath))
            {
                Log.Error_SecretsFileNotFound(Directories.SecretsFilePath);
                return [];
            }

            try
            {
                string encryptedContent = File.ReadAllText(Directories.SecretsFilePath);
                string decryptedContent = EncryptionManager.DecryptString(encryptedContent, EncryptionManager.AesKey, EncryptionManager.AesIV);

                if (string.IsNullOrEmpty(decryptedContent))
                {
                    Log.Error_DecryptSecretsFile("Content is null or empty after decryption");
                    return [];
                }

                return ParseEnvironmentContent(decryptedContent);
            }
            catch (Exception ex)
            {
                Log.Error_LoadEncryptedSecrets($"{ex.Message}");
                return [];
            }
        }

        /// <summary>
        /// Parses environment file content into a key-value dictionary.
        /// </summary>
        private static Dictionary<string, string> ParseEnvironmentContent(string content)
        {
            Dictionary<string, string> variables = [];

            foreach (string line in content.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
            {
                // Skip comments and empty lines
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith('#'))
                {
                    continue;
                }

                string[] parts = line.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                {
                    continue;
                }

                string key = parts[0].Trim();
                string value = parts[1].Trim();

                variables[key] = value;
            }

            return variables;
        }
    }
}
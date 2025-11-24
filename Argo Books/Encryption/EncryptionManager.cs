using Argo_Books.Classes;
using System.Security.Cryptography;
using System.Text;

namespace Argo_Books.Encryption
{
    /// <summary>
    /// The EncryptionManager class provides functionality for encrypting and decrypting data, including
    /// streams and strings, using AES encryption. This class derives encryption keys from hardcoded secrets.
    /// </summary>
    public static class EncryptionManager
    {
        public const string encryptedTag = "encrypted:", encryptedValue = "true", passwordTag = "key:";

        // Getters
        public static byte[] AesKey { get; private set; }
        public static byte[] AesIV { get; private set; }

        /// <summary>
        /// Initializes the encryption manager by deriving encryption keys from hardcoded secrets.
        /// </summary>
        public static void Initialize()
        {
            try
            {
                (AesKey, AesIV) = DeriveKeysFromSecrets();
            }
            catch (Exception ex)
            {
                Log.Error_InitEncryptionHelper(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Encrypts the given input stream using AES encryption with the specified key and IV.
        /// </summary>
        /// <returns>An encrypted MemoryStream containing the encrypted data.</returns>
        public static MemoryStream EncryptStream(Stream inputStream, byte[] key, byte[] iv)
        {
            MemoryStream outputStream = new();

            try
            {
                using Aes aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;

                // Create the CryptoStream for encryption
                CryptoStream cryptoStream = new(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write, leaveOpen: true);

                // Copy the inputStream to the CryptoStream
                inputStream.CopyTo(cryptoStream);
                cryptoStream.FlushFinalBlock();
            }
            catch (Exception ex)
            {
                Log.Error_Encryption(ex.Message);
            }

            outputStream.Position = 0;
            return outputStream;
        }

        /// <summary>
        /// Decrypts the specified encrypted file into a MemoryStream.
        /// </summary>
        /// <returns>A tuple containing the decrypted MemoryStream and any footer lines.</returns>
        public static (MemoryStream?, string[]) DecryptFileToMemoryStream(string inputFile, byte[] key, byte[] iv)
        {
            try
            {
                // Read footer information to get the count
                FooterData? footerData = FooterManager.ReadFooter(inputFile);
                if (footerData == null)
                {
                    Log.Error_Decryption("Could not read footer information");
                    return (null, []);
                }

                int footerLineCount = FooterManager.GetFooterLineCount(footerData);

                // Read the file content as bytes and as text lines
                byte[] fileContentBytes = File.ReadAllBytes(inputFile);
                string[] allLines = File.ReadAllLines(inputFile);

                if (allLines.Length < footerLineCount)
                {
                    Log.Error_Decryption("File does not have expected footer structure");
                    return (null, []);
                }

                // Extract footer lines for return
                string[] footerLines = allLines[^footerLineCount..];

                // Calculate the encrypted content size by reconstructing just the footer portion
                // and subtracting it from the total file size
                string[] contentLines = allLines[..^footerLineCount];
                string contentPortion = string.Join(Environment.NewLine, contentLines);

                // The encrypted content should be exactly the size of the content portion
                byte[] expectedContentBytes = Encoding.UTF8.GetBytes(contentPortion);

                // But we need to work with the actual bytes to avoid encoding issues
                // So let's find where the first footer line starts in the byte array

                string firstFooterLine = footerLines[0];
                byte[] footerLineBytes = Encoding.UTF8.GetBytes(firstFooterLine);

                // Search for the first footer line in the file bytes
                int footerStartByte = FindByteSequence(fileContentBytes, footerLineBytes);

                if (footerStartByte == -1)
                {
                    Log.Error_Decryption($"Could not find footer line '{firstFooterLine}' in file bytes");
                    return (null, footerLines);
                }

                // The encrypted content ends at the newline before the footer
                // Work backwards from footerStartByte to find the preceding newline
                int encryptedContentEnd = footerStartByte;

                // Check if there's a newline just before the footer line
                if (encryptedContentEnd > 0 && fileContentBytes[encryptedContentEnd - 1] == '\n')
                {
                    encryptedContentEnd--;

                    // Check for \r\n (Windows line endings)
                    if (encryptedContentEnd > 0 && fileContentBytes[encryptedContentEnd - 1] == '\r')
                    {
                        encryptedContentEnd--;
                    }
                }

                // Extract encrypted content bytes
                byte[] encryptedContentBytes = new byte[encryptedContentEnd];
                Array.Copy(fileContentBytes, 0, encryptedContentBytes, 0, encryptedContentEnd);

                // Validate the encrypted content length
                if (encryptedContentBytes.Length % 16 != 0)
                {
                    Log.Error_Decryption($"The encrypted content length {encryptedContentBytes.Length} is not a multiple of the AES block size (16 bytes)");

                    // Show some context around the boundary for debugging
                    int contextStart = Math.Max(0, encryptedContentEnd - 32);
                    int contextLength = Math.Min(64, fileContentBytes.Length - contextStart);
                    byte[] contextBytes = new byte[contextLength];
                    Array.Copy(fileContentBytes, contextStart, contextBytes, 0, contextLength);

                    // Convert to hex for easier debugging
                    string hexContext = Convert.ToHexString(contextBytes);
                    Log.WriteWithFormat(1, "Hex context around boundary: {0}", hexContext);

                    return (null, footerLines);
                }

                // Create a MemoryStream from the encrypted content bytes
                using MemoryStream inputMemoryStream = new(encryptedContentBytes);

                // Decrypt the content
                MemoryStream decryptedStream = new();
                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    using CryptoStream cryptoStream = new(inputMemoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
                    cryptoStream.CopyTo(decryptedStream);
                }

                decryptedStream.Position = 0;
                return (decryptedStream, footerLines);
            }
            catch (Exception ex)
            {
                Log.Error_Decryption(ex.Message);
                return (null, []);
            }
        }

        /// <summary>
        /// Finds a byte sequence within a byte array and returns the starting index.
        /// </summary>
        private static int FindByteSequence(byte[] haystack, byte[] needle)
        {
            for (int i = 0; i <= haystack.Length - needle.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < needle.Length; j++)
                {
                    if (haystack[i + j] != needle[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Encrypts the specified plain text string using AES encryption with the provided key and IV.
        /// </summary>
        /// <returns>A Base64-encoded string containing the encrypted text.</returns>
        public static string EncryptString(string plainText, byte[] key, byte[] iv)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msEncrypt = new();
            using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
            using StreamWriter swEncrypt = new(csEncrypt);

            swEncrypt.Write(plainText);
            swEncrypt.Flush();
            csEncrypt.FlushFinalBlock();

            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        /// <summary>
        /// Decrypts the specified Base64-encoded cipher text string using AES decryption with the provided key and IV.
        /// </summary>
        /// <returns>The decrypted plain text, or null if decryption fails.</returns>
        public static string? DecryptString(string cipherText, byte[] key, byte[] iv)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            using MemoryStream msDecrypt = new(cipherTextBytes);
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);

            try
            {
                return srDecrypt.ReadToEnd();
            }
            catch (Exception ex)
            {
                Log.Error_Decryption(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Decrypts the specified encrypted input file and writes the decrypted content to the output file.
        /// </summary>
        public static void DecryptAndWriteToFile(string inputFile, string outputFile, byte[] key, byte[] iv)
        {
            (MemoryStream decryptedStream, string[] footerLines) = DecryptFileToMemoryStream(inputFile, key, iv);
            if (decryptedStream == null)
            {
                return;
            }

            using FileStream outputFileStream = new(outputFile, FileMode.Create, FileAccess.Write);
            decryptedStream.CopyTo(outputFileStream);

            // Write the footer lines to the output file
            using StreamWriter writer = new(outputFileStream, Encoding.UTF8, leaveOpen: true);
            foreach (string line in footerLines)
            {
                writer.WriteLine(line);
            }
        }

        /// <summary>
        /// Derives encryption keys from hardcoded secrets.
        /// </summary>
        /// <returns>A tuple containing the derived key and IV.</returns>
        private static (byte[] Key, byte[] IV) DeriveKeysFromSecrets()
        {
            // Get hardcoded secrets
            byte[] salt = SecretConstants.GetSalt();
            byte[] pepper = SecretConstants.GetPepper();
            string appSpecificString = SecretConstants.GetAppSpecificString();

            // Use only application-specific constants to ensure keys are the same across installations
            using HMACSHA256 hmac = new(pepper);
            byte[] combinedData = Encoding.UTF8.GetBytes(appSpecificString);
            byte[] keyMaterial = hmac.ComputeHash(combinedData);

            // Use a key derivation function to generate the key and IV
            byte[] derivedKey;
            byte[] derivedIV;

            using (Rfc2898DeriveBytes pbkdf2 = new(keyMaterial, salt, 10000, HashAlgorithmName.SHA256))
            {
                derivedKey = pbkdf2.GetBytes(32);  // 256 bits for AES-256
                derivedIV = pbkdf2.GetBytes(16);   // 128 bits for AES IV
            }

            return (derivedKey, derivedIV);
        }
    }
}
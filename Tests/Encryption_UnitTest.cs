using Argo_Books.Classes;
using Argo_Books.Encryption;
using System.Security.Cryptography;
using System.Text;

namespace Tests
{
    [TestClass]
    public class Encryption_Tests
    {
        [TestInitialize]
        public void Setup()
        {
            Directories.SetUniversalDirectories();
            ArgoCompany.InitCacheFiles();
            EncryptionManager.Initialize();
        }

        [TestMethod]
        public void TestEncryptionDecryption()
        {
            string originalText = "MySecretPassword";
            byte[] key = EncryptionManager.AesKey;
            byte[] iv = EncryptionManager.AesIV;

            // Encrypt the string
            string encryptedText = EncryptionManager.EncryptString(originalText, key, iv);

            // Decrypt the string
            string? decryptedText = EncryptionManager.DecryptString(encryptedText, key, iv);

            // Validate the result
            Assert.AreEqual(originalText, decryptedText, "Encryption and decryption failed.");
        }

        [TestMethod]
        public void TestEncryptDecryptStream()
        {
            string originalText = "This is a stream test.";
            byte[] key = EncryptionManager.AesKey;
            byte[] iv = EncryptionManager.AesIV;

            // Convert string to stream
            MemoryStream inputStream = new(Encoding.UTF8.GetBytes(originalText));

            // Encrypt the stream
            MemoryStream encryptedStream = EncryptionManager.EncryptStream(inputStream, key, iv);

            // Reset position for reading the encrypted stream
            encryptedStream.Position = 0;

            // Decrypt the stream
            MemoryStream decryptedStream = new();
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                using CryptoStream cryptoStream = new(encryptedStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
                cryptoStream.CopyTo(decryptedStream);
            }

            // Reset position for reading the decrypted stream
            decryptedStream.Position = 0;

            // Convert decrypted stream back to string
            StreamReader reader = new(decryptedStream);
            string decryptedText = reader.ReadToEnd();

            // Validate the result
            Assert.AreEqual(originalText, decryptedText, "Stream encryption and decryption failed.");
        }

        [TestMethod]
        public void TestDerivedKeyConsistency()
        {
            // Initialize EncryptionManager twice and check if keys remain consistent
            EncryptionManager.Initialize();
            byte[] firstKey = EncryptionManager.AesKey;
            byte[] firstIV = EncryptionManager.AesIV;

            // Re-initialize
            EncryptionManager.Initialize();
            byte[] secondKey = EncryptionManager.AesKey;
            byte[] secondIV = EncryptionManager.AesIV;

            // Keys should be the same after re-initialization on the same machine
            CollectionAssert.AreEqual(firstKey, secondKey, "Key derivation is not consistent.");
            CollectionAssert.AreEqual(firstIV, secondIV, "IV derivation is not consistent.");
        }
    }
}
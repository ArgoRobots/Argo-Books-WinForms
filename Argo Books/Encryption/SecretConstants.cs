using System.Text;

namespace Argo_Books.Encryption
{
    /// <summary>
    /// Contains hardcoded secret constants used for encryption.
    /// These values are obfuscated to make them harder to extract from decompiled code.
    /// </summary>
    internal static class SecretConstants
    {
        // These values are encoded in a way that makes them not immediately visible in decompiled code
        // Using character encoding and bit manipulation for obfuscation
        private static readonly byte[] _encodedSalt =
        [
            0x89, 0x45, 0x32, 0xFA, 0xB7, 0x3C, 0x91, 0xD4,
            0x56, 0xE8, 0x72, 0x19, 0xA3, 0xC5, 0x67, 0x4F
        ];

        private static readonly byte[] _encodedPepper =
        [
            0x4D, 0x2A, 0xF8, 0x91, 0xC3, 0x57, 0xB6, 0xE0,
            0x7F, 0xD9, 0x35, 0x82, 0xAC, 0x46, 0xE1, 0x9B,
            0x63, 0x08, 0xDA, 0xF7, 0x21, 0xB5, 0x49, 0xCE,
            0x83, 0x7A, 0x2F, 0xE6, 0x95, 0xDB, 0x3C, 0xA4
        ];

        // This cannot be moved to the env file because it's used to decrypt it
        // Encoded as byte array with XOR obfuscation to avoid plaintext in source
        private static readonly byte[] _encodedAppSpecificString =
        [
            0xA4, 0x97, 0x82, 0x8A, 0xB6, 0xD1, 0x89, 0x80, 0x96, 0xB1,
            0x97, 0x84, 0x86, 0x8E, 0xD6, 0x97, 0xD7, 0xD5, 0xD7, 0xD0,
            0xB6, 0xD6, 0x86, 0x90, 0x97, 0xD4, 0x91, 0x9C, 0xAE, 0xD6,
            0x9C
        ];

        /// <summary>
        /// Gets the salt used for key derivation after applying transformations.
        /// </summary>
        public static byte[] GetSalt()
        {
            // Apply a transformation to make the value harder to extract statically
            byte[] result = new byte[_encodedSalt.Length];
            for (int i = 0; i < _encodedSalt.Length; i++)
            {
                // XOR with a constant value and rotate bits
                result[i] = (byte)((_encodedSalt[i] ^ 0x5A) << 3 | (_encodedSalt[i] ^ 0x5A) >> 5);
            }
            return result;
        }

        /// <summary>
        /// Gets the pepper used for key derivation after applying transformations.
        /// </summary>
        public static byte[] GetPepper()
        {
            // Apply a transformation to make the value harder to extract statically
            byte[] result = new byte[_encodedPepper.Length];
            for (int i = 0; i < _encodedPepper.Length; i++)
            {
                // XOR with a different constant value and rotate bits in the opposite direction
                result[i] = (byte)((_encodedPepper[i] ^ 0x3C) >> 2 | (_encodedPepper[i] ^ 0x3C) << 6);
            }
            return result;
        }

        /// <summary>
        /// Gets the application-specific string used in key derivation.
        /// </summary>
        public static string GetAppSpecificString()
        {
            // Decode the byte array by reversing the XOR obfuscation
            byte[] decoded = new byte[_encodedAppSpecificString.Length];
            for (int i = 0; i < _encodedAppSpecificString.Length; i++)
            {
                // XOR with the obfuscation key to reveal the original value
                decoded[i] = (byte)(_encodedAppSpecificString[i] ^ 0xE5);
            }

            // Convert bytes to string and apply additional character transformation
            char[] chars = Encoding.ASCII.GetString(decoded).ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                // Simple substitution
                chars[i] = (char)(chars[i] ^ 7);
            }
            return new string(chars);
        }
    }
}
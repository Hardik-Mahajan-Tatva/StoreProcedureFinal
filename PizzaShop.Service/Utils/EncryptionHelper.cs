using System.Security.Cryptography;
using System.Text;

namespace PizzaShop.Service.Utils
{
    public static class EncryptionHelper
    {
        private static readonly string EncryptionKey = "gjawei9gjmaweirhjwcvirjgmnfvioerjmg9iawrjmegvbmne".PadRight(32).Substring(0, 32);

        /// <summary>
        /// Encrypts a plain text string using AES encryption and returns a Base64-encoded string.
        /// </summary>
        /// <param name="plainText">The plain text string to encrypt.</param>
        /// <returns>A Base64-encoded string representing the encrypted data.</returns>
        public static string Encrypt(string plainText)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aes.IV = new byte[16];

                using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream();
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        /// <summary>
        /// Decrypts a Base64-encoded string encrypted using AES back into plain text.
        /// </summary>
        /// <param name="encryptedText">The Base64-encoded string to decrypt.</param>
        /// <returns>The decrypted plain text string.</returns>
        public static string Decrypt(string encryptedText)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
            aes.IV = new byte[16]; // Initialization vector (all zeros)

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(encryptedBytes);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            byte[] plainBytes = new byte[encryptedBytes.Length];
            int decryptedByteCount = cs.Read(plainBytes, 0, plainBytes.Length);
            return Encoding.UTF8.GetString(plainBytes, 0, decryptedByteCount);
        }
    }
}
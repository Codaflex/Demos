using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EncryptDecryptBase64;

internal class Program
{
    // Common Initialization Vector and Encryption Key, shared between Encryptor and Decryptor.
    private static readonly byte[] _InitializationVector = Encoding.ASCII.GetBytes("MySpecialVector1");          // ToDo - Change this (16 characters)
    private static readonly byte[] _EncryptionKey = Encoding.ASCII.GetBytes("MyLongEncryptionKey32Characters2"); // ToDo - Change this (32 characters)

    public static void Main()
    {
        var originalText = "My very secret text that I want to hide from the World!"; // Make sure the text you want to encrypt is longer than the Initialization Vector you use.

        // Encrypt the string to an array of bytes...
        var encrypted = Encrypt(originalText);

        // ... and then Decrypt it to see if it is still the same.
        var resultText = Decrypt(encrypted);

        // Display the original data and the decrypted data.
        Console.WriteLine($"Original:  {originalText}");
        Console.WriteLine($"Encrypted: {encrypted}");
        Console.WriteLine($"Decrypted: {resultText}");
        Console.WriteLine($"Same?      {originalText == resultText}");
    }

    private static string Encrypt(string plainText)
    {
        byte[] encrypted;

        // Create an Aes object with the specified key and IV.
        using (var aesCrypt = Aes.Create())
        {
            // Create a decrytor to perform the stream transform.
            ICryptoTransform encryptor = aesCrypt.CreateEncryptor(_EncryptionKey, _InitializationVector);

            // Create the streams used for encryption.
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                // Write all data to the stream.
                swEncrypt.Write(plainText);
            }

            encrypted = msEncrypt.ToArray();
        }

        // Convert the encrypted bytes Base64 to string.
        var result = Convert.ToBase64String(encrypted);

        // Return the encrypted string
        return result;
    }

    private static string Decrypt(string encryptedText)
    {
        // Declare the string used to hold the decrypted text.
        string plainText = null;

        try
        {
            // Create an Aes object with the specified key and IV.
            using (var aesAlg = Aes.Create())
            {
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(_EncryptionKey, _InitializationVector);

                // Convert the encrypted Base64 string to bytes.
                var encryptedBytes = Convert.FromBase64String(encryptedText);

                // Create the streams used for decryption.
                using var msDecrypt = new MemoryStream(encryptedBytes);
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);
                // Read the decrypted bytes from the decrypting stream and place them in a string.
                plainText = srDecrypt.ReadToEnd();
            }

            return plainText;
        }
        catch (Exception) // The Decryption failed.
        {
            return string.Empty;
        }
    }
}

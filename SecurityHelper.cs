using System.Security.Cryptography;
using System.Text;

namespace RJ_VC_Bypass;

/// <summary>
/// Security Helper class for encryption, decryption, and license verification
/// </summary>
public static class SecurityHelper
{
    private const string SALT = "RJ_BYPASS_DEFAULT_SALT";

    /// <summary>
    /// Verify license key and return new expiry timestamp
    /// </summary>
    public static long VerifyAndGetNewExpiry(string deviceId, string encryptedKeyBase64, long currentExpiryMs)
    {
        // Master key bypass
        if (encryptedKeyBase64.Trim().Equals("HTLWIN-MASTER-KEY", StringComparison.OrdinalIgnoreCase))
        {
            return long.MaxValue;
        }

        try
        {
            string rawData = deviceId + SALT;
            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));
            byte[] aesKeyBytes = hashBytes.Take(16).ToArray();

            string decryptedStr = DecryptAES(encryptedKeyBase64.Trim(), aesKeyBytes);

            // Parse response format: TYPE:VALUE
            string[] parts = decryptedStr.Split(':');

            if (parts.Length >= 2)
            {
                if (parts[0].Equals("VIP", StringComparison.OrdinalIgnoreCase) &&
                    parts[1].Equals("LIFETIME", StringComparison.OrdinalIgnoreCase))
                {
                    return long.MaxValue;
                }
                else if (parts[0].Equals("ADD", StringComparison.OrdinalIgnoreCase))
                {
                    long daysToAdd = long.Parse(parts[1]);
                    long msToAdd = daysToAdd * 24L * 60L * 60L * 1000L;
                    if (currentExpiryMs == long.MaxValue) return long.MaxValue;
                    long baseTime = Math.Max(DateTimeOffset.Now.ToUnixTimeMilliseconds(), currentExpiryMs);
                    return baseTime + msToAdd;
                }
                else if (parts[0].Equals("EXP", StringComparison.OrdinalIgnoreCase))
                {
                    return long.Parse(parts[1]);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Decryption error: {ex.Message}");
        }

        return -1L;
    }

    /// <summary>
    /// AES decrypt (ECB mode, PKCS7 padding)
    /// </summary>
    private static string DecryptAES(string base64Data, byte[] key)
    {
        byte[] data = Convert.FromBase64String(base64Data);

        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.PKCS7;

        using ICryptoTransform decryptor = aes.CreateDecryptor();
        byte[] decrypted = decryptor.TransformFinalBlock(data, 0, data.Length);

        return Encoding.UTF8.GetString(decrypted);
    }

    /// <summary>
    /// Base64 encode for secure storage
    /// </summary>
    public static string EncodeForStorage(string value)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
    }

    /// <summary>
    /// Base64 decode from secure storage
    /// </summary>
    public static string DecodeFromStorage(string encoded)
    {
        try
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Generate a simple hash for data integrity
    /// </summary>
    public static string HashData(string data)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}

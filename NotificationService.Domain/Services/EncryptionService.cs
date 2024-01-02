using System.Security.Cryptography;
using System.Text;
using NLog;
using ILogger = NLog.ILogger;

namespace NotificationService.Domain.Services;

public static class EncryptionService
{
    private const string ENCRYPTION_KEY = "BHTEU/EEHcA8RBbWI0r/7zgPZsn6xtHqRj7X8qkeosA=";
    private const string INITIALIZATION_VECTOR = "UF6avPUNcDxnVBYg+MbSPw==";

    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public static string Encrypt(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            _logger.Error($"{nameof(data)} is null or empty");
            return null;
        }

        try
        {
            using (Aes aes = Aes.Create())
            {
                aes.IV = Convert.FromBase64String(INITIALIZATION_VECTOR);
                aes.Key = Convert.FromBase64String(ENCRYPTION_KEY);
                ICryptoTransform encryptor = aes.CreateEncryptor();

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cryptoStream, Encoding.Unicode))
                        {
                            writer.Write(data);
                        }
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
        catch (CryptographicException ex)
        {
            _logger.Error(ex, ex.InnerException.Message);
            return null;
        }
    }

    public static string Decrypt(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            _logger.Error($"{nameof(data)} is null or empty");
            return null;
        }

        try
        {
            using (Aes aes = Aes.Create())
            {
                aes.IV = Convert.FromBase64String(INITIALIZATION_VECTOR);
                aes.Key = Convert.FromBase64String(ENCRYPTION_KEY);
                ICryptoTransform decryptor = aes.CreateDecryptor();

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(data)))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryptoStream, Encoding.Unicode))
                        {
                            return reader.ReadLine();
                        }
                    }
                }
            }
        }
        catch (CryptographicException ex)
        {
            _logger.Error(ex, ex.InnerException.Message);

            return null;
        }
    }
}
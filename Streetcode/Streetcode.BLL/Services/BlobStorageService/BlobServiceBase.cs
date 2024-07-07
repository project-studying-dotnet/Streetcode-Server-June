using System.Security.Cryptography;
using System.Text;
namespace Streetcode.BLL.Services.BlobStorageService
{
    public class BlobServiceBase
    {
        public string GetMimeType(string extension)
        {
            var ext = extension.ToLower().Replace(".", string.Empty);

            var mimeTypes = new Dictionary<string, string>
            {
                { "jpg",  "image/jpeg" },
                { "jpeg", "image/jpeg" },
                { "png",  "image/png"  },
                { "gif",  "image/gif"  },
                { "bmp",  "image/bmp"  },
                { "txt",  "text/plain" },
                { "mp3",  "audio/mpeg" },
                { "wav",  "audio/wav"  },
                { "ogg",  "audio/ogg"  },
                { "m4a",  "audio/mp4"  },
                { "flac", "audio/flac" }
            };

            return mimeTypes.TryGetValue(ext, out var mimeType) ? mimeType : "application/octet-stream";
        }

        public string HashFunction(string createdFileName)
        {
            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(createdFileName));
                return Convert.ToBase64String(result).Replace('/', '_');
            }
        }

        public void EncryptFile(byte[] imageBytes, string type, string name, string keyCrypt, string blobPath)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(keyCrypt);

            byte[] iv = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(iv);
            }

            byte[] encryptedBytes;
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Key = keyBytes;
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor();
                encryptedBytes = encryptor.TransformFinalBlock(imageBytes, 0, imageBytes.Length);
            }

            byte[] encryptedData = new byte[encryptedBytes.Length + iv.Length];
            Buffer.BlockCopy(iv, 0, encryptedData, 0, iv.Length);
            Buffer.BlockCopy(encryptedBytes, 0, encryptedData, iv.Length, encryptedBytes.Length);
            File.WriteAllBytes($"{blobPath}{name}.{type}", encryptedData);
        }

        public byte[] DecryptFile(string fileName, string type, string keyCrypt, string blobPath)
        {
            byte[] encryptedData = File.ReadAllBytes($"{blobPath}{fileName}.{type}");
            byte[] keyBytes = Encoding.UTF8.GetBytes(keyCrypt);

            byte[] iv = new byte[16];
            Buffer.BlockCopy(encryptedData, 0, iv, 0, iv.Length);

            byte[] decryptedBytes;
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Key = keyBytes;
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor();
                decryptedBytes = decryptor.TransformFinalBlock(encryptedData, iv.Length, encryptedData.Length - iv.Length);
            }

            return decryptedBytes;
        }
    }
}

using System.Security.Cryptography;
using System.Text;

namespace api.Utils
{
    public static class HashUtil
    {
        public static string ComputeSHA256Digest(string input)
        {
            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder hexString= new StringBuilder(hash.Length * 2);
            foreach (byte b in hash) 
            {
                hexString.AppendFormat("{0:x2}", b);
            }
            return Base64Encoder.Encode(hexString.ToString());
        }
    }
}
using System.Security.Cryptography;
using System.Text;

namespace api.Utils
{
    public static class HashUtil
    {
        public static string ComputeSHA256(string input)
        {
            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }
    }
}
using System.Text;

namespace api.Utils
{
    public static class Base64Encoder
    {
        public static string Encode(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }
        public static string Decode(string base64Text)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64Text));
        }
    }
}
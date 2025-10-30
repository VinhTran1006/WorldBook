using System.Security.Cryptography;
using System.Text;

namespace WorldBook.Config
{
    public static class MomoSecurity
    {
        public static string SignSHA256(string rawData, string secretKey)
        {
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
        }
    }
}

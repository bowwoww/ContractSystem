using System.Security.Cryptography;
using System.Text;


namespace DataModel.Security
{
    public class QrHelper
    {
        public static string GenerateQrKey(string contractId, string memberId, string signTime, string expires, string secret)
        {
            var data = $"{contractId}|{memberId}|{signTime}|{expires}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash);
        }
    }
}

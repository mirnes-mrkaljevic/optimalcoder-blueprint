using System.Security.Cryptography;

namespace OptimalCoder.Blueprint.Domain.Identity
{
    public interface ICryptoService
    {
        string GenerateHmacCode(int userId);
        int VerifyUserHmacCode(string codeBase64Url);


    }
    public class CryptoService : ICryptoService
    {
        private static readonly byte[] _privateKey = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF, 0xCF, 0xCA, 0xFE , 0xEC, 0xEB, 0xAF, 0xDE }; 
        private static readonly TimeSpan _passwordResetExpiry = TimeSpan.FromMinutes(5);
        private  static byte _version = 1;

        public string GenerateHmacCode(int userId)
        {
            var message = Enumerable.Empty<byte>()
                .Append(_version)
                .Concat(BitConverter.GetBytes(userId))
                .Concat(BitConverter.GetBytes(DateTime.UtcNow.ToBinary()))
                .ToArray();

            using (HMACSHA256 hmacSha256 = new HMACSHA256(key: _privateKey))
            {
                var hash = hmacSha256.ComputeHash(buffer: message, offset: 0, count: message.Length);

                var outputMessage = message.Concat(hash).ToArray();
                var outputCodeB64 = Convert.ToBase64String(outputMessage);
                var outputCode = outputCodeB64.Replace('+', '-').Replace('/', '_');
                return outputCode;
            }
        }

        public int VerifyUserHmacCode(string codeBase64Url)
        {
            var base64 = codeBase64Url.Replace('-', '+').Replace('_', '/');
            var message = Convert.FromBase64String(base64);

            var version = message[0];
            if (version < _version)
                throw new Exception("User verification fails. Versions are not the same.");

            var userId = BitConverter.ToInt32(message, startIndex: 1);
            var createdUtcBinary = BitConverter.ToInt64(message, startIndex: 1 + sizeof(int));

            DateTime createdUtc = DateTime.FromBinary(createdUtcBinary);
            if (createdUtc.Add(_passwordResetExpiry) < DateTime.UtcNow)
                throw new Exception("User verification fails. Expired date time.");

            var messageLength = 1 + sizeof(Int32) + sizeof(Int64);

            using (HMACSHA256 hmacSha256 = new HMACSHA256(key: _privateKey))
            {
                var hash = hmacSha256.ComputeHash(message, offset: 0, count: messageLength);

                var messageHash = message.Skip(messageLength).ToArray();

                if(Enumerable.SequenceEqual(hash, messageHash)){
                    return userId;
                }
                throw new Exception("User verification fails. Hashes are not the same.");
            }
        }
    }
}

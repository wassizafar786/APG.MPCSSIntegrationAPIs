using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace APGMPCSSIntegration.Constant.Helpers.Services
{
    public static class SHA256Hashing
    {
        static readonly SHA256 sha256 = SHA256.Create();
        public static Task<string> constructHash(string hashData)
        {
                byte[] byteData = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashData));
                var builder = new StringBuilder();
                foreach (byte bytes in byteData)
                {
                    builder.Append(bytes.ToString("x2"));
                }
                return Task.FromResult(builder.ToString());
            
        }
    }
}

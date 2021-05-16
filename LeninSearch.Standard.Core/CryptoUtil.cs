using System;
using System.IO;
using System.Security.Cryptography;

namespace LeninSearch.Standard.Core
{
    public static class CryptoUtil
    {
        public static string GetMD5(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hashBytes = md5.ComputeHash(stream);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
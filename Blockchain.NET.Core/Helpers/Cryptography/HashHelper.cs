using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain.NET.Core.Helpers.Cryptography
{
    public static class HashHelper
    {
        public static string Sha256(string randomString)
        {
            if (string.IsNullOrEmpty(randomString))
                return string.Empty;
            return Sha256(Encoding.UTF8.GetBytes(randomString));
        }

        public static string Sha256(byte[] data)
        {
            if (data == null)
                return string.Empty;
            var hash = new System.Text.StringBuilder();
            var crypt = new System.Security.Cryptography.SHA256Managed();
            byte[] crypto = crypt.ComputeHash(data);
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        public static string ByteArrayToHexString(byte[] array)
        {
            var hash = new System.Text.StringBuilder();
            foreach (byte theByte in array)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        public static string RIPEMD160(string randomString)
        {
            // create a ripemd160 object
            var r160 = Blockchain.NET.Core.Helpers.Cryptography.RIPEMD160.RIPEMD160.Create();
            // convert the string to byte
            byte[] myByte = System.Text.Encoding.ASCII.GetBytes(randomString);
            // compute the byte to RIPEMD160 hash
            byte[] encrypted = r160.ComputeHash(myByte);
            // create a new StringBuilder process the hash byte
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encrypted.Length; i++)
            {
                sb.Append(encrypted[i].ToString("X2"));
            }
            // convert the StringBuilder to String and convert it to lower case and return it.
            return sb.ToString().ToLower();
        }
    }
}

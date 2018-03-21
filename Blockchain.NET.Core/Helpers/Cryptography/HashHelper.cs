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
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
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

        public static string RIPEMD160(string password)
        {
            // create a ripemd160 object
            var r160 = Blockchain.NET.Core.Helpers.Cryptography.RIPEMD160.RIPEMD160.Create();
            // convert the string to byte
            byte[] myByte = System.Text.Encoding.ASCII.GetBytes(password);
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

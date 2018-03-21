using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain.NET.Core.Helpers
{
    public class Encryptable
    {
        public static byte[] GetFrontLockSha(string str1, string str2)
        {
            string saltString = str1 + str2;

            // convert text to bytes to get hash
            ASCIIEncoding AE = new ASCIIEncoding();

            byte[] saltBuffer = AE.GetBytes(saltString);
            return GetSHA512(saltBuffer);
        }

        private static byte[] GetSHA512(byte[] plainBuf)
        {
            byte[] hash;
            using (SHA512Managed hashVal = new SHA512Managed())
            {
                hash = hashVal.ComputeHash(plainBuf);
            }
            return hash;
        }

        public byte[] AESEncryptString(string clearText, string passText, byte[] saltBytes)
        {
            byte[] clearBytes = Encoding.UTF8.GetBytes(clearText);
            byte[] passBytes = Encoding.UTF8.GetBytes(passText);

            // set the global value, which will be used by the Save button
            return AESEncryptBytes(clearBytes, passBytes, saltBytes);
        }

        private byte[] AESEncryptBytes(byte[] clearBytes, byte[] passBytes, byte[] saltBytes)
        {
            byte[] encryptedBytes = null;

            // create a key from the password and salt, use 32K iterations
            var key = new Rfc2898DeriveBytes(passBytes, saltBytes, 32768);

            // create an AES object
            using (Aes aes = new AesManaged())
            {
                // set the key size to 256
                aes.KeySize = 256;
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }
            return encryptedBytes;
        }

        public static string AESDecryptBytes(byte[] cryptBytes, string passPhrase, byte[] saltBytes)
        {
            byte[] clearBytes = null;
            byte[] passBytes = Encoding.UTF8.GetBytes(passPhrase);

            // create a key from the password and salt, use 32K iterations
            var key = new Rfc2898DeriveBytes(passBytes, saltBytes, 32768);

            using (Aes aes = new AesManaged())
            {
                // set the key size to 256
                aes.KeySize = 256;
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cryptBytes, 0, cryptBytes.Length);
                        cs.Close();
                    }
                    clearBytes = ms.ToArray();
                }
            }
            return Encoding.UTF8.GetString(clearBytes);
        }

    }
}

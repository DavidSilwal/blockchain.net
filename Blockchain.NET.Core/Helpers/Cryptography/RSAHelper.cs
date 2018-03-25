using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain.NET.Core.Helpers.Cryptography
{
    public static class RSAHelper
    {
        public const int RSAKeySize = 2048;
        public static string SignData(string message, string privateKey)
        {
            var encoding = new ASCIIEncoding();
            byte[] signedBytes;
            using (RSA rsa = RSA.Create(RSAKeySize))
            {
                byte[] originalData = encoding.GetBytes(message);

                try
                {
                    rsa.ImportParameters(ToRSAParameters(privateKey));

                    signedBytes = rsa.SignData(originalData, HashAlgorithmName.SHA512, RSASignaturePadding.Pss);
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
                finally
                {
                    rsa.Clear();
                }
            }
            return Convert.ToBase64String(signedBytes);
        }

        public static bool VerifyData(string originalMessage, string signedMessage, string publicKey)
        {
            var encoding = new ASCIIEncoding();
            bool success = false;
            using (RSA rsa = RSA.Create(RSAKeySize))
            {
                byte[] bytesToVerify = encoding.GetBytes(originalMessage);
                byte[] signedBytes = Convert.FromBase64String(signedMessage);
                try
                {
                    rsa.ImportParameters(ToRSAParameters(publicKey));

                    success = rsa.VerifyData(bytesToVerify, signedBytes, HashAlgorithmName.SHA512, RSASignaturePadding.Pss);
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    rsa.Clear();
                }
            }
            return success;
        }

        public static Tuple<string, string> CreateKeyPair()
        {
            using (RSA rsa = RSA.Create(RSAKeySize))
            {
                string privateKey = JsonConvert.SerializeObject(new RSAParametersSerializable(rsa.ExportParameters(true)));
                string publicKey = JsonConvert.SerializeObject(rsa.ExportParameters(false));

                return new Tuple<string, string>(privateKey, publicKey);
            }
        }

        public static byte[] Encrypt(string publicKey, string data)
        {
            using (RSA rsa = RSA.Create(RSAKeySize))
            {
                try
                {
                    rsa.ImportParameters(ToRSAParameters(publicKey));

                    byte[] plainBytes = Encoding.UTF8.GetBytes(data);

                    byte[] encryptedBytes = rsa.Encrypt(plainBytes, RSAEncryptionPadding.OaepSHA512);

                    return encryptedBytes;
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
                finally
                {
                    rsa.Clear();
                }
            }
        }

        public static string Decrypt(string privateKey, byte[] encryptedBytes)
        {
            using (RSA rsa = RSA.Create(RSAKeySize))
            {
                try
                {
                    rsa.ImportParameters(ToRSAParameters(privateKey));

                    byte[] plainBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA512);

                    return Encoding.UTF8.GetString(plainBytes, 0, plainBytes.Length);
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
                finally
                {
                    rsa.Clear();
                }
            }
        }

        public static RSAParameters ToRSAParameters(string key)
        {
            using (RSA rsa = RSA.Create(RSAKeySize))
            {
                var rsaParameters = JsonConvert.DeserializeObject<RSAParametersSerializable>(key).RSAParameters;
                rsa.ImportParameters(rsaParameters);

                if (rsaParameters.P == null)
                    return rsa.ExportParameters(false);

                return rsa.ExportParameters(true);
            }
        }
    }
}

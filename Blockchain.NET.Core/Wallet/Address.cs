﻿using Blockchain.NET.Core.Helpers.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.NET.Core.Wallet
{
    public class Address
    {
        public string Key { get; set; }

        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }

        public static Address New()
        {
            var newAddress = new Address();
            var keyValuePair = RSAHelper.CreateKeyPair();
            newAddress.PrivateKey = keyValuePair.Item1;
            newAddress.PublicKey = keyValuePair.Item2;
            newAddress.Key = HashHelper.GenerateAddress(newAddress.PublicKey);
            return newAddress;
        }
    }
}

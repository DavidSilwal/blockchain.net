using Blockchain.NET.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Blockchain.NET.Core
{
    public class Wallet : Serializable<Wallet>
    {
        public List<Address> Addresses = new List<Address>();

        private string _password;

        private static string _walletName = "wallet.sec";
        private static string _rootPath = "Wallet";

        public Wallet(string password)
        {
            _password = password;
        }

        public void Save()
        {
            if (!Directory.Exists(_rootPath))
                Directory.CreateDirectory(_rootPath);
            File.WriteAllBytes(Path.Combine(_rootPath, _walletName), Serialize(_password));
        }

        public static Wallet Load(string password)
        {
            if (!Directory.Exists(_rootPath))
                Directory.CreateDirectory(_rootPath);
            if (!File.Exists(Path.Combine(_rootPath, _walletName)))
            {
                new Wallet(password).Save();
            }
            var wallet = Deserialize(File.ReadAllBytes(Path.Combine(_rootPath, _walletName)), password);
            wallet._password = password;
            return wallet;
        }

        public Address GetNewAddress()
        {
            var newAddress = Address.New();
            Addresses.Add(newAddress);
            Save();
            return newAddress;
        }
    }
}

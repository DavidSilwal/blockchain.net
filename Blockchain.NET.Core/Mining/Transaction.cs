using Blockchain.NET.Core.Helpers.Cryptography;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Blockchain.NET.Core.Mining
{
    public class Transaction
    {
        [Key]
        public string Address { get; set; }

        public string Input { get; set; }

        public string Signature { get; set; }

        public decimal Amount { get; set; }

        public byte[] Data { get; set; }

        public string Output { get; set; }

        public Transaction() { }

        public Transaction(string input, string privateKey, string publicKey, decimal amount, string output, byte[] data = null)
        {
            Input = input;
            Address = HashHelper.RIPEMD160(HashHelper.Sha256(publicKey));
            Amount = amount;
            Data = data;
            Output = output;
            Signature = RSAHelper.SignData(GenerateHash(), privateKey);
        }

        public string GenerateHash()
        {
            return HashHelper.Sha256(Address + Input + Amount + HashHelper.Sha256(Data) + Output);
        }

        public bool Verify(string publicKey)
        {
            return RSAHelper.VerifyData(GenerateHash(), Signature, publicKey) && Input == HashHelper.RIPEMD160(HashHelper.Sha256(publicKey));
        }
    }
}

using Blockchain.NET.Core.Helpers.Cryptography;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Blockchain.NET.Core
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Address { get; set; }

        public string PublicKey { get; set; }

        public byte[] Data { get; set; }

        public decimal Amount { get; set; }

        public Transaction()
        {

        }

        public Transaction(string[] input, string publicKey, decimal amount, byte[] data = null)
        {
            PublicKey = publicKey;
            Address = HashHelper.RIPEMD160(HashHelper.Sha256(PublicKey));
            Data = data;
            Amount = amount;
        }

        public string GenerateHash()
        {
            return HashHelper.Sha256(PublicKey);
        }
    }
}

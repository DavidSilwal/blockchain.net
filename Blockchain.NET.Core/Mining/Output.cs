using Blockchain.NET.Core.Helpers.Cryptography;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Blockchain.NET.Core.Mining
{
    public class Output
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long TransactionId { get; set; }

        public string Key { get; set; }

        public long Amount { get; set; }

        public Transaction Transaction { get; set; }

        public Output() { }

        public Output(string key, long amount)
        {
            Key = key;
            Amount = amount;
        }

        public string GenerateHash()
        {
            return HashHelper.Sha256(Key + Amount);
        }
    }
}

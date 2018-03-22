using Blockchain.NET.Core.Helpers.Cryptography;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Blockchain.NET.Core.Mining
{
    public class Input
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long TransactionId { get; set; }

        public string Key { get; set; }

        public string Signature { get; set; }

        public string PublicKey { get; set; }

        public Transaction Transaction { get; set; }

        public Input() { }

        public Input(string key)
        {
            Key = key;
        }

        public string GenerateHash()
        {
            return HashHelper.Sha256(Key);
        }
    }
}

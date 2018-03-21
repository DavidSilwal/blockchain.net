using Blockchain.NET.Core.Helpers.Cryptography;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Blockchain.NET.Core.Mining
{
    public class IO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long TransactionId { get; set; }

        public string Key { get; set; }

        public Transaction Transaction { get; set; }

        public IO() { }

        public IO(string key)
        {
            Key = key;
        }

        public string GenerateHash()
        {
            return HashHelper.Sha256(Key);
        }
    }
}

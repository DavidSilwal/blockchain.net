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

        public string Signature { get; set; }

        public Transaction()
        {

        }

        public Transaction(string[] input, string[] output, decimal amount, decimal confirmationFee)
        {
            //Input = input;
            //Output = output;
            //Amount = amount;
            //Signature = GenerateHash();
        }

        public string GenerateHash()
        {
            return HashHelper.Sha256(Signature);
        }
    }
}

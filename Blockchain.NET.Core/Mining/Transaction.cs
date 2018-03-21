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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long BlockHeight { get; set; }

        public List<IO> Inputs { get; set; }

        public string PublicKey { get; set; }

        public string Signature { get; set; }

        public decimal Amount { get; set; }

        public byte[] Data { get; set; }

        public string Output { get; set; }

        [ForeignKey(nameof(BlockHeight))]
        public Block Block { get; set; }

        public Transaction() { }

        public Transaction(List<IO> inputs, string privateKey, string publicKey, decimal amount, string[] output, byte[] data = null)
        {
            Inputs = inputs;
            PublicKey = publicKey;
            Amount = amount;
            Data = data;
            Output = string.Join(",", output);
            Signature = RSAHelper.SignData(GenerateHash(), privateKey);
        }

        public string GenerateHash()
        {
            var inputsHash = HashHelper.Sha256(string Inputs.Select(i => i.GenerateHash())
            return HashHelper.Sha256( + Amount + HashHelper.Sha256(Data) + Output);
        }

        public bool Verify(string[] unlockScripts)
        {
            var inputList = string.IsNullOrEmpty(Input) ? new List<string>() : Input.Split(',').OrderBy(s => s).ToList();
            var unlockScriptsList = unlockScripts.Select(s => HashHelper.GenerateAddress(PublicKey)).OrderBy(s => s).ToList();
            return RSAHelper.VerifyData(GenerateHash(), Signature, PublicKey) && inputList.SequenceEqual(unlockScriptsList);
        }
    }
}

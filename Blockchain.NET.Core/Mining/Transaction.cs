using Blockchain.NET.Core.Helpers;
using Blockchain.NET.Core.Helpers.Calculations;
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

        public int BlockHeight { get; set; }

        public string Hash { get; set; }

        public virtual ICollection<Input> Inputs { get; set; }

        public byte[] Data { get; set; }

        public virtual ICollection<Output> Outputs { get; set; }

        [ForeignKey(nameof(BlockHeight))]
        public Block Block { get; set; }

        public Transaction() { }

        public Transaction(List<Input> inputs, Wallet.Wallet wallet, List<Output> outputs, byte[] data = null)
        {
            Inputs = inputs;
            Data = data;
            Outputs = outputs;
            calculateOutputs(wallet);
            signInputs(wallet);
        }

        private void signInputs(Wallet.Wallet wallet)
        {
            if (Inputs != null)
            {
                foreach (var input in Inputs)
                {
                    var foundAddress = wallet.Addresses.FirstOrDefault(a => a.Key == input.Key);
                    
                    if (foundAddress != null)
                    {
                        input.Signature = RSAHelper.SignData(GenerateHash(), foundAddress.PrivateKey);
                        input.PublicKey = foundAddress.PublicKey;
                    }
                }
            }
        }

        private void calculateOutputs(Wallet.Wallet wallet)
        {
            if (Inputs != null)
            {
                decimal balance = BalanceHelper.GetBalanceOfAddresses(Inputs.Select(i => i.Key).ToArray());
                Outputs.Add(new Output(wallet.NewAddress().Key, balance - Outputs.Select(o => o.Amount).Sum()));
            }
        }

        public string GenerateHash()
        {
            if (string.IsNullOrEmpty(Hash))
            {
                var inputsHash = Inputs == null ? string.Empty : HashHelper.Sha256(string.Join("", Inputs.Select(i => i.GenerateHash())));
                var outputsHash = Outputs == null ? string.Empty : HashHelper.Sha256(string.Join("", Outputs.Select(i => i.GenerateHash())));
                Hash = HashHelper.Sha256(inputsHash + HashHelper.Sha256(Data) + outputsHash);
            }
            return Hash;
        }

        public bool Verify()
        {
            if (Inputs != null)
                foreach (var input in Inputs)
                {
                    if (!RSAHelper.VerifyData(GenerateHash(), input.Signature, input.PublicKey) || input.Key != HashHelper.GenerateAddress(input.PublicKey))
                        return false;
                }
            return true;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }
    }
}

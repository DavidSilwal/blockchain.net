using Blockchain.NET.Core.Store;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blockchain.NET.Core.Helpers.Calculations
{
    public static class BalanceHelper
    {
        public static decimal GetBalanceOfAddress(string address)
        {
            decimal balance = 0;
            if (!string.IsNullOrEmpty(address))
                using (BlockchainDbContext db = new BlockchainDbContext())
                {
                    balance = db.Inputs.Any(i => i.Key == address) ? 0 : db.Outputs.Where(o => o.Key == address).Select(t => t.Amount).Sum();
                }
            return balance;
        }

        public static decimal GetBalanceOfAddresses(string[] addresses)
        {
            decimal balance = 0;
            if (addresses != null)
                using (BlockchainDbContext db = new BlockchainDbContext())
                {
                    var usedOutputs = db.Outputs.Where(o => addresses.Contains(o.Key)).ToList();
                    var usedInputs = db.Inputs.Where(o => addresses.Contains(o.Key)).Select(i => i.Key).ToList();
                    foreach(var output in usedOutputs)
                    {
                        if (!usedInputs.Contains(output.Key))
                            balance += output.Amount;
                    }
                }
            return balance;
        }

        public static bool EverUsedAsInput(string address)
        {
            if (!string.IsNullOrEmpty(address))
                using (BlockchainDbContext db = new BlockchainDbContext())
                {
                    return db.Transactions.Any(t => t.Inputs.Select(i => i.Key).Contains(address));
                }
            return false;
        }

        public static bool EverUsedAsInput(string[] addresses)
        {
            if (addresses != null)
                using (BlockchainDbContext db = new BlockchainDbContext())
                {
                    foreach(var address in addresses)
                    {
                        if (db.Transactions.Any(t => t.Inputs.Select(i => i.Key).Contains(address)))
                            return true;
                    }
                }
            return false;
        }
    }
}

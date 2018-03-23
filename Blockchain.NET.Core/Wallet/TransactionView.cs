using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.NET.Core.Wallet
{
    public class TransactionView
    {
        public IOView[] Inputs { get; set; }

        public string Message { get; set; }

        public int BlockHeight { get; set; }

        public IOView[] Outputs { get; set; }

        public decimal Amount { get; set; }

        public bool IsIncome { get; set; }
    }
}

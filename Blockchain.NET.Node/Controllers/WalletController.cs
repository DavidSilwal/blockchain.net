using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Blockchain.NET.Node.Controllers
{
    [Route("api/[controller]")]
    public class WalletController : Controller
    {
        [HttpGet("[action]")]
        public WalletBalance WalletBalance()
        {
            return new WalletBalance() { Balance = Program.Wallet.GetBalance() };
        }
    }

    public class WalletBalance
    {
        public decimal Balance { get; set; }
    }
}

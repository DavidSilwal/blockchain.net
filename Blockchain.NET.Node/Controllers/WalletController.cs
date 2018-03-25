using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockchain.NET.Core.Wallet;
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

        [HttpPost("[action]")]
        public IActionResult AddTransaction([FromBody] TransactionRequest request)
        {
            try
            {
                Program.BlockChain.AddTransaction(Program.Wallet.CreateTransaction(request.Address, request.Amount, request.Message));

                return Ok();
            }
            catch (Exception exc)
            {
                return BadRequest(new { error = exc.Message });
            }
        }

        [HttpGet("[action]")]
        public List<TransactionView> ActualTransactions([FromQuery]int blockHeight)
        {
            var transactions = Program.Wallet.GetTransactions(blockHeight);

            return transactions;
        }

        [HttpGet("[action]")]
        public AddressView GenerateAddress()
        {
            return new AddressView() { Key = Program.Wallet.NewAddress().Key };
        }
    }

    public class AddressView
    {
        public string Key { get; set; }
    }

    public class WalletBalance
    {
        public decimal Balance { get; set; }
    }

    public class TransactionRequest
    {
        public string Address { get; set; }

        public decimal Amount { get; set; }

        public string Message { get; set; }
    }
}

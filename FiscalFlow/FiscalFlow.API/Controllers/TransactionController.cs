using FiscalFlow.Application.Core.Abstractions.Services;
using FiscalFlow.Contracts.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiscalFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<IActionResult> AddTransaction(AddTransactionRequest payload)
        {
            var transaction = await _transactionService.AddTransaction(payload);
            if(transaction.IsSuccess)
            {
                return Created();
            }
            return NotFound(transaction.Errors[0]);
        }
    }
}

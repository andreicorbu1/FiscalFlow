using Ardalis.Result.AspNetCore;
using FiscalFlow.Application.Core.Abstractions.Services;
using FiscalFlow.Contracts.Accounts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiscalFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public IActionResult CreateAccount(CreateAccountRequest payload)
        {
            var result = _accountService.CreateAccount(payload);
            if(result.IsSuccess)
            {
                return Created();
            }
            else
            {
                return NotFound(result.Errors[0]);
            }
        }

        [HttpGet]
        [Produces("text/csv")]
        public async Task<FileResult> GetTransactionsAsCsv(Guid accountId)
        {
            await _accountService.ExportTransactionsAsCsvAsync(accountId);
            string fileName = $"{accountId}.csv";
            string directoryPath = "C:\\Users\\Andrei\\Dev\\licenta\\FiscalFlow\\FiscalFlow\\FiscalFlow.API\\CSV"; // This should be the directory where your CSV files are stored

            // Get the full path of the file
            string filePath = Path.Combine(directoryPath, fileName);
            return PhysicalFile(filePath, "text/csv", fileName);
        }

        [HttpGet("userid ={userId}")]
        public async Task<ActionResult<IReadOnlyCollection<Domain.Entities.Account>>> GetUsersAccountsAsync(string userId)
        {
            var accounts = await _accountService.GetAccountsOfOwnerAsync(userId);
            if(accounts.IsSuccess)
            {
                return Ok(accounts.Value);
            }
            else
            {
                return this.ToActionResult(accounts);
            }
        }
    }
}

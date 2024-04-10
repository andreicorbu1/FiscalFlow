using System.Security.Claims;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using FiscalFlow.Application.Core.Abstractions.Services;
using FiscalFlow.Contracts;
using FiscalFlow.Contracts.Accounts;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpPost]
        public IActionResult CreateAccount(CreateAccountRequest payload)
        {
            payload.OwnerId = ExtractUserIdFromClaims().Value;
            var result = _accountService.CreateAccount(payload);
            if (result.IsSuccess)
            {
                return Created();
            }
            else
            {
                return NotFound(result.Errors[0]);
            }
        }

        [Authorize]
        [HttpGet("me/accountId={accountId}")]
        [Produces("text/csv")]
        public async Task<IActionResult> GetTransactionsAsCsv(Guid accountId)
        {
            await _accountService.ExportTransactionsAsCsvAsync(accountId);
            string fileName = $"{accountId}.csv";
            string directoryPath =
                "C:\\Users\\Andrei\\Dev\\licenta\\FiscalFlow\\FiscalFlow\\FiscalFlow.API\\CSV"; // This should be the directory where your CSV files are stored

            // Get the full path of the file
            string filePath = Path.Combine(directoryPath, fileName);
            return PhysicalFile(filePath, "text/csv", fileName);
        }

        [Authorize]
        [HttpPut("me/update/account={accountId}")]
        public async Task<IActionResult> UpdateUserAccount([FromRoute] Guid accountId, [FromBody] UpdateAccountRequest updateAccount)
        {
            var id = ExtractUserIdFromClaims();
            if (!id.IsSuccess)
            {
                return Unauthorized();
            }

            updateAccount.AccountId = accountId;
            var result = await _accountService.UpdateAccount(id.Value, updateAccount);
            return this.ToActionResult(result);
        }
        
        [Authorize]
        [HttpGet("me/accounts")]
        public async Task<ActionResult<IReadOnlyCollection<Domain.Entities.Account>>> GetUsersAccountsAsync()
        {
            var idResult = ExtractUserIdFromClaims();
            if (!idResult.IsSuccess)
                return Unauthorized();
            var ownerId = idResult.Value;
            var accounts = await _accountService.GetAccountsOfOwnerAsync(ownerId);
            if (accounts.IsSuccess)
            {
                return Ok(accounts.Value);
            }
            else
            {
                return this.ToActionResult(accounts);
            }
        }

        private Result<string> ExtractUserIdFromClaims()
        {
            Claim? ownerId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (ownerId is null)
            {
                return Result.Unauthorized();
            }

            return Result.Success(ownerId.Value);
        }
    }
}
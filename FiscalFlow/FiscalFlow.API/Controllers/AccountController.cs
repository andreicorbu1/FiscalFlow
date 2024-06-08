using Ardalis.Result.AspNetCore;
using FiscalFlow.Application.Core.Abstractions.Services;
using FiscalFlow.Application.Core.Extensions;
using FiscalFlow.Application.Tools.Csv;
using FiscalFlow.Contracts;
using FiscalFlow.Contracts.Accounts;
using FiscalFlow.Domain.Entities;
using FiscalFlow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static FiscalFlow.API.Utils.Utils;

namespace FiscalFlow.API.Controllers;

[Route("api/v1/[controller]")]
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
        payload.OwnerId = ExtractUserIdFromClaims(User);
        var result = _accountService.CreateAccount(payload);
        return this.ToActionResult(result);
    }

    [Authorize]
    [HttpGet("me/account/last-{numberOfTransactions}-transactions")]
    public async Task<IActionResult> GetLastTransactionsOfUser(int numberOfTransactions)
    {
        var ownerId = ExtractUserIdFromClaims(User);
        if (!ownerId.IsSuccess)
        {
            return Unauthorized();
        }

        var transactions = await _accountService.GetLastTransactions(ownerId, numberOfTransactions);
        if (!transactions.IsSuccess)
        {
            return BadRequest();
        }

        return Ok(transactions.Value);
    }

    [Authorize]
    [HttpGet("me/category-expenses")]
    public async Task<ActionResult<Dictionary<Category, decimal>>> GetCategoryExpenses()
    {
        var userId = ExtractUserIdFromClaims(User);
        if (!userId.IsSuccess)
        {
            return Unauthorized();
        }

        var dict = await _accountService.GetCategoryReportsFromAllAccounts(userId);
        return this.ToActionResult(dict);
    }

    [Authorize]
    [HttpGet("me/csv/export/{accountId}")]
    [Produces("text/csv")]
    public async Task<IActionResult> GetAccountAsCsv(Guid accountId)
    {
        var ownerId = ExtractUserIdFromClaims(User);
        if (!ownerId.IsSuccess) return Unauthorized();
        var stream = await _accountService.ExportTransactionsAsCsvAsync(accountId, ownerId.Value);
        if (!stream.IsSuccess)
            return BadRequest();
        var fileName = $"{accountId}.csv";
        // Get the full path of the file
        return File(stream.Value, "text/csv", fileName);
    }

    [Authorize]
    [HttpPost("me/csv/import/{accountId}")]
    public async Task<IActionResult> ImportTransactionsFromCsv(Guid accountId, IFormFile file)
    {
        if(file == null || file.Length == 0)
        {
            return BadRequest("File is empty!");
        }
        IList<Transaction> transactions = new List<Transaction>();
        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            stream.Position = 0;
            transactions = CsvImporter.Import<Transaction>(stream);
        }
        var ownerId = ExtractUserIdFromClaims(User);
        var result = await _accountService.ImportTransactionsFromCsv(transactions, ownerId, accountId);
        return this.ToActionResult(result);
    }

    [HttpGet("me/account/{accountId}")]
    [Authorize]
    public async Task<ActionResult<AccountDto>> GetUserAccount(Guid accountId)
    {
        var id = ExtractUserIdFromClaims(User);
        if (!id.IsSuccess) return Unauthorized();

        var account = await _accountService.GetAccountFromIdAsync(accountId, id.Value);
        return account.Value.ToAccountDto();
    }

    [Authorize]
    [HttpPut("me/update/account={accountId}")]
    public async Task<IActionResult> UpdateUserAccount([FromRoute] Guid accountId,
        [FromBody] UpdateAccountRequest updateAccount)
    {
        var id = ExtractUserIdFromClaims(User);
        if (!id.IsSuccess) return Unauthorized();

        updateAccount.AccountId = accountId;
        var result = await _accountService.UpdateAccount(id.Value, updateAccount);
        return this.ToActionResult(result);
    }

    [Authorize]
    [HttpGet("me/accounts")]
    public async Task<ActionResult<List<AccountDto>>> GetUsersAccounts()
    {
        var idResult = ExtractUserIdFromClaims(User);
        if (!idResult.IsSuccess)
            return Unauthorized();
        var ownerId = idResult.Value;
        var accounts = await _accountService.GetAccountsOfOwnerAsync(ownerId);
        if (accounts.IsSuccess) return Ok(accounts.Value);

        return this.ToActionResult(accounts);
    }

    [Authorize]
    [HttpDelete("me/delete/{accountId:guid}")]
    public async Task<IActionResult> DeleteAccount([FromRoute] Guid accountId)
    {
        var id = ExtractUserIdFromClaims(User);
        if (!id.IsSuccess) return Unauthorized();

        var result = await Task.Run(() => _accountService.DeleteAccount(accountId, id.Value));
        return this.ToActionResult(result);
    }
}
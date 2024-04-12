﻿using Ardalis.Result.AspNetCore;
using FiscalFlow.Application.Core.Abstractions.Services;
using FiscalFlow.Contracts;
using FiscalFlow.Contracts.Accounts;
using FiscalFlow.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static FiscalFlow.API.Utils.Utils;

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
            payload.OwnerId = ExtractUserIdFromClaims(User);
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
        [HttpGet("me/csv/export/{accountId}")]
        [Produces("text/csv")]
        public async Task<IActionResult> GetAccountAsCsv(Guid accountId)
        {
            var ownerId = ExtractUserIdFromClaims(User);
            if (!ownerId.IsSuccess) return Unauthorized();
            await _accountService.ExportTransactionsAsCsvAsync(accountId, ownerId.Value);
            string fileName = $"{accountId}.csv";
            string directoryPath =
                "C:\\Users\\Andrei\\Dev\\licenta\\FiscalFlow\\FiscalFlow\\FiscalFlow.API\\CSV"; // This should be the directory where your CSV files are stored

            // Get the full path of the file
            string filePath = Path.Combine(directoryPath, fileName);
            return PhysicalFile(filePath, "text/csv", fileName);
        }

        [HttpGet("me/account/{accountId}")]
        [Authorize]
        public async Task<ActionResult<Account>> GetUserAccount(Guid accountId)
        {
            var id = ExtractUserIdFromClaims(User);
            if (!id.IsSuccess)
            {
                return Unauthorized();
            }

            var account = await _accountService.GetAccountFromIdAsync(accountId, id.Value);
            return this.ToActionResult(account);
        }
        
        [Authorize]
        [HttpPut("me/update/account={accountId}")]
        public async Task<IActionResult> UpdateUserAccount([FromRoute] Guid accountId,
            [FromBody] UpdateAccountRequest updateAccount)
        {
            var id = ExtractUserIdFromClaims(User);
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
        public async Task<ActionResult<IReadOnlyCollection<Account>>> GetUsersAccounts()
        {
            var idResult = ExtractUserIdFromClaims(User);
            if (!idResult.IsSuccess)
                return Unauthorized();
            var ownerId = idResult.Value;
            var accounts = await _accountService.GetAccountsOfOwnerAsync(ownerId);
            if (accounts.IsSuccess)
            {
                return Ok(accounts.Value);
            }

            return this.ToActionResult(accounts);
        }

        [Authorize]
        [HttpDelete("me/delete/{accountId:guid}")]
        public async Task<IActionResult> DeleteAccount([FromRoute] Guid accountId)
        {
            var id = ExtractUserIdFromClaims(User);
            if (!id.IsSuccess)
            {
                return Unauthorized();
            }

            var result = await Task.Run(() => _accountService.DeleteAccount(accountId, id.Value));
            return this.ToActionResult(result);
        }
    }
}
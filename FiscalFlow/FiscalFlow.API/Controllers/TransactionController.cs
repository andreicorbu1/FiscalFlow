using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using FiscalFlow.Application.Core.Abstractions.Services;
using FiscalFlow.Contracts.Accounts;
using FiscalFlow.Contracts.Transactions;
using FiscalFlow.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static FiscalFlow.API.Utils.Utils;

namespace FiscalFlow.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Transaction>> AddTransaction(AddTransactionRequest payload)
    {
        var id = ExtractUserIdFromClaims(User);
        if (!id.IsSuccess) return Unauthorized();
        var transaction = await _transactionService.AddTransaction(payload, id.Value);
        if (transaction.IsSuccess)
        {
            return Created();
        }
        return this.ToActionResult(transaction);
    }

    [Authorize]
    [HttpGet("me/{accountId:guid}/periodOfTime")]
    public async Task<ActionResult<IList<Transaction>>> GetTransactionsPeriod(Guid accountId,
        PeriodOfTimeRequest period)
    {
        var id = ExtractUserIdFromClaims(User);
        if (!id.IsSuccess) return Unauthorized();
        return this.ToActionResult(await _transactionService.GetTransactionsFromAccountPeriodOfTime(id.Value, accountId, period));
    }

    [HttpDelete("me/delete/{transactionId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteTransaction(Guid transactionId)
    {
        var id = ExtractUserIdFromClaims(User);
        if (!id.IsSuccess)
            return Unauthorized();

        var result = await Task.Run(() => _transactionService.DeleteTransaction(id.Value, transactionId));

        return this.ToActionResult(result);
    }

    [HttpPut("me/edit")]
    [Authorize]
    public async Task<IActionResult> EditTransaction(UpdateTransaction payload)
    {
        var id = ExtractUserIdFromClaims(User);
        if (!id.IsSuccess)
            return Unauthorized();
        var result = await _transactionService.UpdateTransaction(payload, id);
        return this.ToActionResult(result);
    }
}
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using FiscalFlow.Application.Core.Abstractions.Services;
using FiscalFlow.Contracts.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static FiscalFlow.API.Utils.Utils;

namespace FiscalFlow.API.Controllers;

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
    [Authorize]
    public async Task<IActionResult> AddTransaction(AddTransactionRequest payload)
    {
        var id = ExtractUserIdFromClaims(User);
        if (!id.IsSuccess) return Unauthorized();
        var transaction = await _transactionService.AddTransaction(payload, id.Value);
        if (transaction.IsSuccess) return Created();
        return NotFound(transaction.Errors[0]);
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
}
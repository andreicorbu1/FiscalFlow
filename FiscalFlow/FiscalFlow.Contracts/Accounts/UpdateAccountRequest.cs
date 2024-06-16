using FiscalFlow.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FiscalFlow.Contracts;

public class UpdateAccountRequest
{
    [JsonIgnore] public Guid AccountId { get; set; }
    [MaxLength(50)] public string? Name { get; set; }
    public decimal? MoneyBalance { get; set; }
    public MyCurrency MoneyCurrency { get; set; }
    public AccountType AccountType { get; set; }
}
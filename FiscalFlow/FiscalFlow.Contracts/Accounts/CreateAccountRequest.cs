﻿using FiscalFlow.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FiscalFlow.Contracts.Accounts;

public class CreateAccountRequest
{
    [MaxLength(50)] public string Name { get; set; } = string.Empty;

    public decimal Balance { get; set; }
    public string Currency { get; set; }
    public AccountType AccountType { get; set; }
    [JsonIgnore] public string OwnerId { get; set; } = string.Empty;
}
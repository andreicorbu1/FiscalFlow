﻿namespace FiscalFlow.Contracts.Authentication
{
    public sealed class ResetPasswordRequest
    {
        public ResetPasswordRequest()
        {

        }
        public required string Email { get; init; }
        public required string ResetCode { get; init; }
        public required string NewPassword { get; init; }
    }
}
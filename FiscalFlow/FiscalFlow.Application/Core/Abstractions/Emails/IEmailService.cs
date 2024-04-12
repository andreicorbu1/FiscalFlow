using FiscalFlow.Contracts.Emails;

namespace FiscalFlow.Application.Core.Abstractions.Emails;

public interface IEmailService
{
    public Task<bool> SendEmailAsync(MailRequest mailRequest);
}
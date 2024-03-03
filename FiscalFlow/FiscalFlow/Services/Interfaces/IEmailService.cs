using FiscalFlow.Dto.Dto;

namespace FiscalFlow.Services.Interfaces;

public interface IEmailService
{
    public Task<bool> SendEmailAsync(EmailSendDto emailSend);
}
using FiscalFlow.Application.Core.Abstractions.Emails;
using FiscalFlow.Contracts.Emails;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Microsoft.Extensions.Configuration;

namespace FiscalFlow.Application.Services;

internal sealed class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<bool> SendEmailAsync(MailRequest emailSend)
    {
        var client = new MailjetClient(_config["MailJet:ApiKey"], _config["MailJet:SecretKey"]);

        var email = new TransactionalEmailBuilder()
            .WithFrom(new SendContact(_config["Email:From"], _config["Email:ApplicationName"]))
            .WithSubject(emailSend.Subject)
            .WithHtmlPart(emailSend.Body)
            .WithTo(new SendContact(emailSend.To))
            .Build();

        var response = await client.SendTransactionalEmailAsync(email);
        if (response.Messages != null && response.Messages[0].Status == "success")
            return true;

        return false;
    }
}
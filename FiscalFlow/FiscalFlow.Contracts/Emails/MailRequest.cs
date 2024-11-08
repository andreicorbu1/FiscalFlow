﻿namespace FiscalFlow.Contracts.Emails;

public sealed class MailRequest
{
    public MailRequest(string to, string subject, string body)
    {
        To = to;
        Subject = subject;
        Body = body;
    }

    public string To { get; }
    public string Subject { get; }
    public string Body { get; }
}
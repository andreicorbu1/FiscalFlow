namespace FiscalFlow.Contracts.Authentication;

public class ExternalAuthRequest
{
    public string? Provider { get; set; }
    public string? IdToken { get; set; }
}

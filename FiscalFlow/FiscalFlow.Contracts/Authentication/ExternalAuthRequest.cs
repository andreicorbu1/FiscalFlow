namespace FiscalFlow.Contracts.Authentication;

public class ExternalAuthRequest
{
    public string? Provider { get; set; }
    public string? IdToken { get; set; }
    public string? AccessToken { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
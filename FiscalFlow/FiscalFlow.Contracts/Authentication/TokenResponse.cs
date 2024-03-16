namespace FiscalFlow.Contracts.Authentication;

public class TokenResponse
{
    public string Token { get; }
    public string FirstName { get; }
    public string LastName { get; }

    public TokenResponse(string token, string firstName, string lastName)
    {
        Token = token;
        FirstName = firstName;
        LastName = lastName;
    }
}


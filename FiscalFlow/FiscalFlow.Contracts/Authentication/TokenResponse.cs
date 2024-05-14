namespace FiscalFlow.Contracts.Authentication;

public class TokenResponse
{
    public TokenResponse(string token, string refreshToken, string firstName, string lastName)
    {
        Token = token;
        FirstName = firstName;
        LastName = lastName;
        RefreshToken = refreshToken;
    }

    public string RefreshToken { get; }
    public string Token { get; }
    public string FirstName { get; }
    public string LastName { get; }
}
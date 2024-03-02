using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FiscalFlow.Model;
using FiscalFlow.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace FiscalFlow.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _jwtKey;

    public JwtService(IConfiguration config)
    {
        _config = config;
        _jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]!));
    }

    public string CreateJwt(AppUser user)
    {
        var userClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
        };

        var credentials = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(userClaims),
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(_config["JWT:ExpiresInMinutes"]!)),
            SigningCredentials = credentials,
            Issuer = _config["JWT:Issuer"],
            Audience = _config["JWT:Audience"]
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = tokenHandler.CreateToken(tokenDescriptor);
        
        return tokenHandler.WriteToken(jwt);

    }
}
using FiscalFlow.Model;

namespace FiscalFlow.Services.Interfaces;
public interface IJwtService
{ 
    string CreateJwt(AppUser user);
}

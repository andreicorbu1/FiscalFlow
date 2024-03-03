using FiscalFlow.Model;

namespace FiscalFlow.Dto.Response;

public class UserResponse
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string JWT { get; set; }
}

public static class AppUserExtensions
{
    public static UserResponse ToUserResponse(this AppUser user)
    {
        return new UserResponse
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
        };
    }
}

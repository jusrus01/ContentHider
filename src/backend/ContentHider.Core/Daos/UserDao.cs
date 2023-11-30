using Microsoft.AspNetCore.Identity;

namespace ContentHider.Core.Daos;

public class UserDao : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}
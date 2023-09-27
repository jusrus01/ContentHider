using ContentHider.Core.Daos;
using ContentHider.Core.Enums;

namespace ContentHider.Core.Entities;

public class UserDao : Dao
{
    public string? Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? UserName { get; init; }
    public string? Password { get; set; }
    public Roles Role { get; init; }
}
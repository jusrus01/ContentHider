using ContentHider.Core.Daos;

namespace ContentHider.Core.Entities;

public class UserDao : Dao
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}
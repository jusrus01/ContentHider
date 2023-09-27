using ContentHider.Core.Daos;

namespace ContentHider.Core.Entities;

public class OrganizationDao : Dao
{
    public string? Id { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public string? OwnerId { get; init; }
}
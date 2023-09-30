namespace ContentHider.Core.Daos;

public class OrganizationDao : Dao
{
    public string? Id { get; init; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? OwnerId { get; set; }
}
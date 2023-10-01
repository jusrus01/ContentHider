namespace ContentHider.Core.Daos;

public class FormatDao : Dao
{
    public string? Id { get; init; }
    public string? OrganizationId { get; init; }
    public OrganizationDao Organization { get; init; }
    public string? Title { get; set; }
}
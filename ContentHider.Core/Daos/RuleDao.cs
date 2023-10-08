namespace ContentHider.Core.Daos;

public class RuleDao : Dao
{
    public string? Id { get; init; }
    public string? FormatId { get; init; }
    public FormatDao? Format { get; init; }
    public string? Title { get; set; }
}
using ContentHider.Core.Enums;

namespace ContentHider.Core.Daos;

public class FormatDao : Dao
{
    public string? Id { get; init; }
    public string? OrganizationId { get; init; }
    public OrganizationDao? Organization { get; init; }
    public string? Title { get; set; }

    public FormatType Type { get; set; }
    public string? Description { get; set; }

    public string? FormatDefinition { get; set; }

    public List<RuleDao>? Rules { get; init; }
}
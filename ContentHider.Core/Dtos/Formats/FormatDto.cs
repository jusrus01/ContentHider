using ContentHider.Core.Dtos.Rules;
using ContentHider.Core.Enums;

namespace ContentHider.Core.Dtos.Formats;

public record FormatDto(string? Id, string? OrganizationId, string? Title, List<RuleDto>? Rules, FormatType Type,
    string? FormatDefinition);
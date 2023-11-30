using ContentHider.Core.Dtos.Formats;

namespace ContentHider.Core.Dtos.Organizations;

public record OrgDto(string? Id, string? Title, string? Description, List<FormatDto>? Formats);
using ContentHider.Core.Enums;

namespace ContentHider.Core.Dtos.Formats;

public record UpdateFormatDto(string? Title, string? Description, FormatType Type, string? FormatDefinition);
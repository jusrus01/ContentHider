using ContentHider.Core.Enums;

namespace ContentHider.Core.Dtos.Formats;

public record CreateFormatDto(string? Title, string? Description, FormatType Type, string? FormatDefinition);
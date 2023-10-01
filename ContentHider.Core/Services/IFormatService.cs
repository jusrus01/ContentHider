using ContentHider.Core.Dtos.Formats;

namespace ContentHider.Core.Services;

public interface IFormatService
{
    Task<OrgFormatDto> CreateAsync(string orgId, OrgCreateFormatDto createDto, CancellationToken token);
}
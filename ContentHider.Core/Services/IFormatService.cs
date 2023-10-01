using ContentHider.Core.Dtos.Formats;

namespace ContentHider.Core.Services;

// TODO: clarify about mapping to dto from middleware (.net core)
public interface IFormatService
{
    Task<OrgFormatDto> CreateAsync(string orgId, OrgCreateFormatDto createDto, CancellationToken token);
    Task<OrgFormatDto> UpdateAsync(string orgId, string id, OrgUpdateFormatDto updateDto, CancellationToken token);
    Task<OrgFormatDto> DeleteAsync(string orgId, string id, CancellationToken token);
    Task<OrgFormatDto> GetByIdAsync(string orgId, string id, CancellationToken token);
    Task<IEnumerable<OrgFormatDto>> GetAllAsync(string orgId, CancellationToken token);
}
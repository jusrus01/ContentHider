using ContentHider.Core.Dtos.Formats;

namespace ContentHider.Core.Services;

public interface IFormatService
{
    Task<FormatDto> CreateAsync(string orgId, CreateFormatDto createDto, CancellationToken token);
    Task<FormatDto> UpdateAsync(string orgId, string id, UpdateFormatDto updateDto, CancellationToken token);
    Task<FormatDto> DeleteAsync(string orgId, string id, CancellationToken token);
    Task<FormatDto> GetByIdAsync(string orgId, string id, CancellationToken token);
    Task<IEnumerable<FormatDto>> GetAllAsync(string orgId, CancellationToken token);
}
using ContentHider.Core.Dtos.Organizations;

namespace ContentHider.Core.Services;

public interface IOrganizationService
{
    Task<OrgDto> CreateAsync(CreateOrgDto dto, CancellationToken token);

    Task<OrgDto> UpdateAsync(string id, UpdateOrgDto dto, CancellationToken token);

    Task<OrgDto> DeleteAsync(string id, CancellationToken token);

    Task<IEnumerable<OrgDto>> GetAllAsync(CancellationToken token);
    Task<IEnumerable<OrgPreviewDto>> GetAllPreviewAsync(CancellationToken token);

    Task<OrgDto> GetByIdAsync(string id, CancellationToken token);
}
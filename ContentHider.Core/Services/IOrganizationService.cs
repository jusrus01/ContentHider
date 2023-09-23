using ContentHider.Core.Dtos;

namespace ContentHider.Core.Services;

public interface IOrganizationService
{
    Task<OrgDto> CreateAsync(CreateOrgDto dto, CancellationToken token);
}
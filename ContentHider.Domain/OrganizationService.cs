using ContentHider.Core.Dtos;
using ContentHider.Core.Entities;
using ContentHider.Core.Exceptions;
using ContentHider.Core.Repositories;
using ContentHider.Core.Services;

namespace ContentHider.Domain;

public class OrganizationService : IOrganizationService
{
    private readonly IUnitOfWork _uow;

    public OrganizationService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<OrgDto> CreateAsync(CreateOrgDto dto, CancellationToken token)
    {
        EnsureValidCreateArgs(dto);

        var orgs = await _uow
            .GetAsync<OrganizationDao>(o => o.Title == dto.Title, token)
            .ConfigureAwait(false);
        EnsureEmpty(orgs);

        // TODO: assign current user id
        var newOrg = new OrganizationDao
        {
            Description = dto.Description,
            Title = dto.Title,
            Id = Guid.NewGuid().ToString(),
            OwnerId = Guid.NewGuid().ToString()
        };

        await _uow.SaveAsync(newOrg, token).ConfigureAwait(false);

        return new OrgDto(newOrg.Title, newOrg.Description);
    }

    private static void EnsureEmpty(List<OrganizationDao> orgs)
    {
        if (orgs.Any())
        {
            throw new InvalidInputHttpException(null, "Such org already exists");
        }
    }

    private static void EnsureValidCreateArgs(CreateOrgDto dto)
    {
        var isValid =
            !string.IsNullOrWhiteSpace(dto.Title) &&
            !string.IsNullOrWhiteSpace(dto.Description);

        if (!isValid)
        {
            throw new InvalidInputHttpException(null, "Invalid input");
        }
    }
}
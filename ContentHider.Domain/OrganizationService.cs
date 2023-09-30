using ContentHider.Core.Daos;
using ContentHider.Core.Dtos;
using ContentHider.Core.Exceptions;
using ContentHider.Core.Repositories;
using ContentHider.Core.Services;

namespace ContentHider.Domain;

public class OrganizationService : IOrganizationService
{
    private readonly ICallerAccessor _callerAccessor;
    private readonly IUnitOfWork _uow;

    public OrganizationService(ICallerAccessor callerAccessor, IUnitOfWork uow)
    {
        _uow = uow;
        _callerAccessor = callerAccessor;
    }

    public async Task<OrgDto> CreateAsync(CreateOrgDto dto, CancellationToken token)
    {
        EnsureValidArgs(dto.Title, dto.Description);

        var orgs = await _uow
            .GetAsync<OrganizationDao>(o => o.Title == dto.Title, token)
            .ConfigureAwait(false);
        EnsureEmpty(orgs);

        var newOrg = new OrganizationDao
        {
            Description = dto.Description,
            Title = dto.Title,
            Id = Guid.NewGuid().ToString(),
            OwnerId = _callerAccessor.UserId
        };

        await _uow.SaveAsync(newOrg, token).ConfigureAwait(false);

        return ToDto(newOrg);
    }

    public async Task<OrgDto> UpdateAsync(string id, UpdateOrgDto dto, CancellationToken token)
    {
        EnsureValidId(id);
        EnsureValidArgs(dto.Title, dto.Description);

        var orgs = await _uow
            .GetAsync<OrganizationDao>(o => o.Id == id, token)
            .ConfigureAwait(false);

        var org = orgs.SingleOrDefault();
        EnsureOrgFound(org);

        org!.Description = dto.Description;
        org.Title = dto.Title;

        await _uow.UpdateAsync(org, token)
            .ConfigureAwait(false);

        return ToDto(org);
    }

    public async Task<OrgDto> DeleteAsync(string id, CancellationToken token)
    {
        EnsureValidId(id);
        var orgs = await _uow
            .GetAsync<OrganizationDao>(o => o.Id == id, token)
            .ConfigureAwait(false);

        var org = orgs.SingleOrDefault();
        EnsureOrgFound(org);

        var dto = ToDto(org);
#pragma warning disable CS8631
        await _uow.DeleteAsync(org, token).ConfigureAwait(false);
#pragma warning restore CS8631
        return dto;
    }

    public async Task<IEnumerable<OrgDto>> GetAllAsync(CancellationToken token)
    {
        var orgs = await _uow
            .GetAsync<OrganizationDao>(token: token)
            .ConfigureAwait(false);
        return orgs.Select(ToDto);
    }

    public async Task<OrgDto> GetByIdAsync(string id, CancellationToken token)
    {
        EnsureValidId(id);
        var orgs = await _uow
            .GetAsync<OrganizationDao>(o => o.Id == id, token)
            .ConfigureAwait(false);

        var org = orgs.SingleOrDefault();
        EnsureOrgFound(org);

        return ToDto(org);
    }

    private static OrgDto ToDto(OrganizationDao? org)
    {
        ArgumentNullException.ThrowIfNull(org);
        ArgumentNullException.ThrowIfNull(org.Title);
        ArgumentNullException.ThrowIfNull(org.Description);
        ArgumentNullException.ThrowIfNull(org.Id);

        return new OrgDto(org.Id, org.Title, org.Description);
    }

    private static void EnsureEmpty(List<OrganizationDao> orgs)
    {
        if (orgs.Any())
        {
            throw new InvalidInputHttpException(null, "Org already exists");
        }
    }

    private static void EnsureValidArgs(string title, string description)
    {
        var isValid =
            !string.IsNullOrWhiteSpace(title) &&
            !string.IsNullOrWhiteSpace(description);

        if (!isValid)
        {
            throw new InvalidInputHttpException(null, "Invalid input");
        }
    }

    private static void EnsureValidId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new InvalidInputHttpException(null, "Invalid id");
        }
    }

    private static void EnsureOrgFound(OrganizationDao? org)
    {
        if (org == null)
        {
            throw new InvalidInputHttpException(null, "Org not found");
        }
    }
}
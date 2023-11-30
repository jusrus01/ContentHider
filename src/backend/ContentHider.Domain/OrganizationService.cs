using ContentHider.Core.Daos;
using ContentHider.Core.Dtos.Organizations;
using ContentHider.Core.Exceptions;
using ContentHider.Core.Extensions;
using ContentHider.Core.Repositories;
using ContentHider.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace ContentHider.Domain;

/// <summary>
///     Logic operates under the assumption that the administrator will be able to do modifications.
///     If that changes, will need to update validation
/// </summary>
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
            .GetAsync(SearchPatterns.Org.SearchOrgByTitle(dto), token: token)
            .ConfigureAwait(false);
        orgs.EnsureEmpty();

        var newOrg = new OrganizationDao
        {
            Description = dto.Description,
            Title = dto.Title,
            Id = Guid.NewGuid().ToString(),
            OwnerId = _callerAccessor.UserId
        };

        await _uow.SaveAsync(newOrg, token).ConfigureAwait(false);

        return Mapper.ToDto(newOrg);
    }

    public async Task<OrgDto> UpdateAsync(string id, UpdateOrgDto dto, CancellationToken token)
    {
        EnsureValidId(id);
        EnsureValidArgs(dto.Title, dto.Description);

        var orgs = await _uow
            .GetAsync(SearchPatterns.Org.SelectOrgById(id), token: token)
            .ConfigureAwait(false);

        orgs.EnsureSingle();
        var org = orgs.SingleOrDefault();

        org!.Description = dto.Description;
        org.Title = dto.Title;

        await _uow.UpdateAsync(org, token)
            .ConfigureAwait(false);

        return Mapper.ToDto(org);
    }

    public async Task<OrgDto> DeleteAsync(string id, CancellationToken token)
    {
        EnsureValidId(id);
        var orgs = await _uow
            .GetAsync(SearchPatterns.Org.SelectOrgById(id), token: token)
            .ConfigureAwait(false);

        orgs.EnsureSingle();
        var org = orgs.SingleOrDefault();

        var dto = Mapper.ToDto(org!);
#pragma warning disable CS8631
        await _uow.DeleteAsync(org, token).ConfigureAwait(false);
#pragma warning restore CS8631
        return dto;
    }

    public async Task<IEnumerable<OrgDto>> GetAllAsync(CancellationToken token)
    {
        IIncludableQueryable<OrganizationDao, object> Include(IQueryable<OrganizationDao> func) =>
            func
                .Include(i => i.Formats!)
                .ThenInclude(i => i.Rules!);

        var orgs = await _uow
            .GetAsync<OrganizationDao>(token: token, include: Include)
            .ConfigureAwait(false);
        return orgs.Select(Mapper.ToDto);
    }

    public async Task<IEnumerable<OrgPreviewDto>> GetAllPreviewAsync(CancellationToken token)
    {
        var orgs = await _uow
            .GetAsync<OrganizationDao>(token: token)
            .ConfigureAwait(false);
        return orgs.Select(ToPreviewDto);
    }

    public async Task<OrgDto> GetByIdAsync(string id, CancellationToken token)
    {
        IIncludableQueryable<OrganizationDao, object> Include(IQueryable<OrganizationDao> func) =>
            func
                .Include(i => i.Formats!)
                .ThenInclude(i => i.Rules!);

        EnsureValidId(id);
        var orgs = await _uow
            .GetAsync(SearchPatterns.Org.SelectOrgById(id), token: token, include: Include)
            .ConfigureAwait(false);

        orgs.EnsureSingle();
        var org = orgs.SingleOrDefault();

        return Mapper.ToDto(org!);
    }

    private static OrgPreviewDto ToPreviewDto(OrganizationDao? org)
    {
        ArgumentNullException.ThrowIfNull(org);
        ArgumentNullException.ThrowIfNull(org.Title);
        ArgumentNullException.ThrowIfNull(org.Description);
        ArgumentNullException.ThrowIfNull(org.Id);

        return new OrgPreviewDto(org.Title, org.Description);
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
}
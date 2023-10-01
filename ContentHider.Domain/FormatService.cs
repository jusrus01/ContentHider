using ContentHider.Core.Daos;
using ContentHider.Core.Dtos.Formats;
using ContentHider.Core.Exceptions;
using ContentHider.Core.Extensions;
using ContentHider.Core.Repositories;
using ContentHider.Core.Services;

namespace ContentHider.Domain;

/// <summary>
///     Logic operates under the assumption that the administrator will be able to do modifications.
///     If that changes, will need to update validation
/// </summary>
public class FormatService : IFormatService
{
    private readonly IUnitOfWork _uow;

    public FormatService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<OrgFormatDto> CreateAsync(string orgId, OrgCreateFormatDto createDto, CancellationToken token)
    {
        EnsureValidId(orgId);
        EnsureValidArgs(createDto.Title);

        var orgs = await _uow
            .GetAsync(i => i.Formats!, SearchPatterns.Org.SelectOrgById(orgId), token)
            .ConfigureAwait(false);

        orgs.EnsureSingle();
        var org = orgs.SingleOrDefault();

        var format = new FormatDao
        {
            Id = Guid.NewGuid().ToString(),
            OrganizationId = org.Id,
            Title = createDto.Title
        };

        org.EnsureFormatDoesNotExist(format);

        await _uow.SaveAsync(format, token).ConfigureAwait(false);

        return ToDto(format);
    }

    public async Task<OrgFormatDto> UpdateAsync(
        string orgId,
        string id,
        OrgUpdateFormatDto updateDto,
        CancellationToken token)
    {
        EnsureValidId(orgId);
        EnsureValidArgs(updateDto.Title);

        var orgs = await _uow
            .GetAsync(i => i.Formats!, SearchPatterns.Org.SelectOrgById(orgId), token)
            .ConfigureAwait(false);

        orgs.EnsureSingle();
        var org = orgs.SingleOrDefault();

        var format = org.Formats.SingleOrDefault(i => i.Id == id);
        if (format == null)
        {
            throw new InvalidInputHttpException(null, $"Cannot find format with id '{id}'");
        }

        format.Title = updateDto.Title;

        await _uow.UpdateAsync(format, token).ConfigureAwait(false);

        return ToDto(format);
    }

    public Task<OrgFormatDto> DeleteAsync(string orgId, string id, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<OrgFormatDto> GetByIdAsync(string orgId, string id, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<OrgFormatDto>> GetAllAsync(string orgId, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    private static OrgFormatDto ToDto(FormatDao format)
    {
        return new OrgFormatDto(format.Title);
    }

    private static void EnsureValidId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new InvalidInputHttpException(null, "Invalid id");
        }
    }

    private static void EnsureValidArgs(string title)
    {
        var isValid = !string.IsNullOrWhiteSpace(title);

        if (!isValid)
        {
            throw new InvalidInputHttpException(null, "Invalid input");
        }
    }
}
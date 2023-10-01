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
        EnsureValidArgs(createDto);

        var orgs = await _uow
            .GetAsync(i => i.Formats, SearchPatterns.Org.SelectOrgById(orgId), token)
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

        return new OrgFormatDto(format.Title);
    }

    private static void EnsureValidId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new InvalidInputHttpException(null, "Invalid id");
        }
    }

    private static void EnsureValidArgs(OrgCreateFormatDto createDto)
    {
        var isValid = !string.IsNullOrWhiteSpace(createDto.Title);

        if (!isValid)
        {
            throw new InvalidInputHttpException(null, "Invalid input");
        }
    }
}
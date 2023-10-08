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
            .GetAsync(i => i.Formats!, SearchPatterns.Org.SelectOrgById(orgId), token: token)
            .ConfigureAwait(false);

        orgs.EnsureSingle();
        var org = orgs.SingleOrDefault() ?? throw new InvalidOperationException();

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
        EnsureValidId(id);
        EnsureValidArgs(updateDto.Title);

        var orgs = await _uow
            .GetAsync(i => i.Formats!, SearchPatterns.Org.SelectOrgById(orgId), token: token)
            .ConfigureAwait(false);

        orgs.EnsureSingle();
        var org = orgs.SingleOrDefault() ?? throw new Exception();
        org.EnsureSingleFormat(id);

        var format = org.Formats?.SingleOrDefault(i => i.Id == id) ?? throw new Exception();
        format.Title = updateDto.Title;

        await _uow.UpdateAsync(format, token).ConfigureAwait(false);

        return ToDto(format);
    }

    public async Task<OrgFormatDto> DeleteAsync(string orgId, string id, CancellationToken token)
    {
        EnsureValidId(orgId);
        EnsureValidId(id);

        var orgs = await _uow
            .GetAsync(i => i.Formats!, SearchPatterns.Org.SelectOrgById(orgId), token: token)
            .ConfigureAwait(false);

        orgs.EnsureSingle();
        var org = orgs.SingleOrDefault() ?? throw new Exception();
        org.EnsureSingleFormat(id);

        var format = org.Formats?.SingleOrDefault(i => i.Id == id) ?? throw new Exception();
        var dto = ToDto(format);


        await _uow.DeleteAsync(format, token);

        return dto;
    }

    public async Task<OrgFormatDto> GetByIdAsync(string orgId, string id, CancellationToken token)
    {
        EnsureValidId(orgId);
        EnsureValidId(id);

        var orgs = await _uow
            .GetAsync(i => i.Formats!, SearchPatterns.Org.SelectOrgById(orgId), token: token)
            .ConfigureAwait(false);

        orgs.EnsureSingle();
        var org = orgs.SingleOrDefault() ?? throw new Exception();
        org.EnsureSingleFormat(id);

        var format = org.Formats?.SingleOrDefault(i => i.Id == id) ?? throw new Exception();

        return ToDto(format);
    }

    public async Task<IEnumerable<OrgFormatDto>> GetAllAsync(string orgId, CancellationToken token)
    {
        EnsureValidId(orgId);

        var orgs = await _uow
            .GetAsync(i => i.Formats!, SearchPatterns.Org.SelectOrgById(orgId), token: token)
            .ConfigureAwait(false);

        orgs.EnsureSingle();
        var org = orgs.SingleOrDefault();

        return org!.Formats!.Select(ToDto);
    }

    private static OrgFormatDto ToDto(FormatDao format)
    {
        return new OrgFormatDto(format.Id, format.OrganizationId, format.Title);
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
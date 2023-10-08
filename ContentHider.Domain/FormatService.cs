using System.Net;
using System.Text.Json;
using System.Xml.Linq;
using ContentHider.Core.Daos;
using ContentHider.Core.Dtos.Formats;
using ContentHider.Core.Enums;
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

    public async Task<FormatDto> CreateAsync(string orgId, CreateFormatDto createDto, CancellationToken token)
    {
        EnsureValidId(orgId);
        EnsureValidArgs(createDto.Title);

        var orgs = await _uow
            .GetDeprecatedAsync(i => i.Formats!, SearchPatterns.Org.SelectOrgById(orgId), token: token)
            .ConfigureAwait(false);

        orgs.EnsureSingle();
        var org = orgs.SingleOrDefault() ?? throw new InvalidOperationException();

        EnsureFormatDefinitionIsCorrect(createDto.FormatDefinition, createDto.Type);
        var format = new FormatDao
        {
            Id = Guid.NewGuid().ToString(),
            OrganizationId = org.Id,
            Title = createDto.Title,
            Description = createDto.Description,
            Type = createDto.Type,
            FormatDefinition = createDto.FormatDefinition
        };

        org.EnsureFormatDoesNotExist(format);

        await _uow.SaveAsync(format, token).ConfigureAwait(false);

        return Mapper.ToDto(format);
    }

    public async Task<FormatDto> UpdateAsync(
        string orgId,
        string id,
        UpdateFormatDto updateDto,
        CancellationToken token)
    {
        EnsureValidId(orgId);
        EnsureValidId(id);
        EnsureValidArgs(updateDto.Title);

        var orgs = await _uow
            .GetDeprecatedAsync(i => i.Formats!, SearchPatterns.Org.SelectOrgById(orgId), token: token,
                includeExpr2: i => i.Formats!.Select(format => format.Rules))
            .ConfigureAwait(false);

        orgs.EnsureSingle();
        var org = orgs.SingleOrDefault() ?? throw new Exception();
        org.EnsureSingleFormat(id);

        var format = org.Formats?.SingleOrDefault(i => i.Id == id) ?? throw new Exception();

        format.Title = updateDto.Title;
        format.Description = updateDto.Description;

        if (format.Type != updateDto.Type)
        {
            EnsureTypeCanChange(format);
            EnsureFormatDefinitionIsCorrect(updateDto.FormatDefinition, updateDto.Type);
            format.Type = updateDto.Type;
        }

        await _uow.UpdateAsync(format, token).ConfigureAwait(false);

        return Mapper.ToDto(format);
    }

    public async Task<FormatDto> DeleteAsync(string orgId, string id, CancellationToken token)
    {
        EnsureValidId(orgId);
        EnsureValidId(id);

        var orgs = await _uow
            .GetDeprecatedAsync(i => i.Formats!, SearchPatterns.Org.SelectOrgById(orgId), token: token)
            .ConfigureAwait(false);

        orgs.EnsureSingle();
        var org = orgs.SingleOrDefault() ?? throw new Exception();
        org.EnsureSingleFormat(id);

        var format = org.Formats?.SingleOrDefault(i => i.Id == id) ?? throw new Exception();
        var dto = Mapper.ToDto(format);


        await _uow.DeleteAsync(format, token);

        return dto;
    }

    public async Task<FormatDto> GetByIdAsync(string orgId, string id, CancellationToken token)
    {
        EnsureValidId(orgId);
        EnsureValidId(id);

        var orgs = await _uow
            .GetDeprecatedAsync(i => i.Formats!, SearchPatterns.Org.SelectOrgById(orgId), token: token)
            .ConfigureAwait(false);

        orgs.EnsureSingle();
        var org = orgs.SingleOrDefault() ?? throw new Exception();
        org.EnsureSingleFormat(id);

        var format = org.Formats?.SingleOrDefault(i => i.Id == id) ?? throw new Exception();

        return Mapper.ToDto(format);
    }

    public async Task<IEnumerable<FormatDto>> GetAllAsync(string orgId, CancellationToken token)
    {
        EnsureValidId(orgId);

        var orgs = await _uow
            .GetDeprecatedAsync(i => i.Formats!, SearchPatterns.Org.SelectOrgById(orgId), token: token)
            .ConfigureAwait(false);

        orgs.EnsureSingle();
        var org = orgs.SingleOrDefault();

        return org!.Formats!.Select(Mapper.ToDto);
    }

    private static void EnsureFormatDefinitionIsCorrect(string? formatDefinition, FormatType type)
    {
        var validationException = new HttpException(null, $"Invalid format definition for '{type}' type",
            HttpStatusCode.BadRequest);

        if (string.IsNullOrWhiteSpace(formatDefinition))
        {
            throw validationException;
        }

        switch (type)
        {
            case FormatType.Json:
                EnsureJsonIsParsable(formatDefinition, validationException);
                break;
            case FormatType.Xml:
                EnsureXmlIsParsable(formatDefinition, validationException);
                break;
            default:
                throw validationException;
        }
    }

    private static void EnsureJsonIsParsable(string formatDefinition, Exception validationException)
    {
        try
        {
            JsonSerializer.Deserialize<object>(formatDefinition);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw validationException;
        }
    }

    private static void EnsureXmlIsParsable(string formatDefinition, Exception validationException)
    {
        try
        {
            XDocument.Parse(formatDefinition);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw validationException;
        }
    }

    private static void EnsureTypeCanChange(FormatDao format)
    {
        if (format.Rules!.Any())
        {
            throw new InvalidInputHttpException(null,
                "Cannot change format type that already has rules assigned to it. Delete all rules if you want to change type.");
        }
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
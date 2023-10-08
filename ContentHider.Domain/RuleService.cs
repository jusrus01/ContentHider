using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using ContentHider.Core.Daos;
using ContentHider.Core.Dtos.Rules;
using ContentHider.Core.Enums;
using ContentHider.Core.Exceptions;
using ContentHider.Core.Extensions;
using ContentHider.Core.Repositories;
using ContentHider.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace ContentHider.Domain;

/// <summary>
///     Logic operates under the assumption that the administrator will be able to do modifications.
///     If that changes, will need to update validation
/// </summary>
public class RuleService : IRuleService
{
    private readonly IUnitOfWork _uow;

    public RuleService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<RuleDto> CreateAsync(string orgId, string formatId, CreateRuleDto createDto,
        CancellationToken token)
    {
        EnsureValidId(orgId);
        EnsureValidId(formatId);
        EnsureValidArgs(createDto.Title, createDto.AnonymizedField);

        var format = await ResolveFormatAsync(orgId, formatId, token);
        EnsureRuleNotCreated(format.Id, format.Rules, createDto);
        EnsureRuleIsValidForFormatDefinition(format, createDto.AnonymizedField);

        var rule = new RuleDao
        {
            Id = Guid.NewGuid().ToString(),
            FormatId = format.Id,
            Title = createDto.Title,
            AnonymizedField = createDto.AnonymizedField
        };

        await _uow.SaveAsync(rule, token);

        return Mapper.ToDto(rule);
    }


    public async Task<RuleDto> UpdateAsync(string orgId, string formatId, string id, UpdateRuleDto updateDto,
        CancellationToken token)
    {
        EnsureValidId(orgId);
        EnsureValidId(formatId);
        EnsureValidArgs(updateDto.Title, updateDto.AnonymizedField);
        EnsureValidId(id);

        var format = await ResolveFormatAsync(orgId, formatId, token);
        EnsureRuleCreated(format.Id, format.Rules, id);
        EnsureRuleIsValidForFormatDefinition(format, updateDto.AnonymizedField);

        var rule = format.Rules!.Single(rule => rule.Id == id);

        rule.Title = updateDto.Title;
        rule.AnonymizedField = updateDto.AnonymizedField;

        await _uow.UpdateAsync(rule, token);

        return Mapper.ToDto(rule);
    }

    public async Task<RuleDto> DeleteAsync(string orgId, string formatId, string id, CancellationToken token)
    {
        EnsureValidId(orgId);
        EnsureValidId(formatId);
        EnsureValidId(id);

        var format = await ResolveFormatAsync(orgId, formatId, token);
        EnsureRuleCreated(format.Id, format.Rules, id);

        var rule = format.Rules!.Single(rule => rule.Id == id);
        var dto = Mapper.ToDto(rule);

        await _uow.DeleteAsync(rule, token);

        return dto;
    }

    public async Task<RuleDto> GetByIdAsync(string orgId, string formatId, string id, CancellationToken token)
    {
        EnsureValidId(orgId);
        EnsureValidId(formatId);
        EnsureValidId(id);

        var format = await ResolveFormatAsync(orgId, formatId, token);
        EnsureRuleCreated(format.Id, format.Rules, id);

        var rule = format.Rules!.Single(rule => rule.Id == id);

        return Mapper.ToDto(rule);
    }

    public async Task<IEnumerable<RuleDto>> GetAllAsync(string orgId, string formatId, CancellationToken token)
    {
        EnsureValidId(orgId);
        EnsureValidId(formatId);

        var format = await ResolveFormatAsync(orgId, formatId, token);
        return format.Rules!.Select(Mapper.ToDto);
    }

    private static void EnsureRuleIsValidForFormatDefinition(FormatDao format, string anonymizedField)
    {
        var definition = format.FormatDefinition ?? throw new ArgumentException();
        var keyWordPresent = definition.Contains(anonymizedField);
        if (!keyWordPresent)
        {
            throw new InvalidInputHttpException(null,
                $"Invalid anonymized field. Make sure tag that you want to anonymize exists. Format definition '{definition}'");
        }

        switch (format.Type)
        {
            case FormatType.Json:
                var obj = JsonSerializer.Deserialize<object>(definition);
                var fields = obj?.GetType().GetFields();
                foreach (var field in fields?.ToList() ?? new List<FieldInfo>())
                {
                    if (field.Name == anonymizedField)
                    {
                        return;
                    }
                }

                var properties = obj?.GetType().GetProperties();
                foreach (var property in properties?.ToList() ?? new List<PropertyInfo>())
                {
                    if (property.Name == anonymizedField)
                    {
                        return;
                    }
                }

                throw new InvalidInputHttpException(null,
                    $"Invalid anonymized field. Make sure tag that you want to anonymize exists and does not exist as a value in your format definition. Format definition '{definition}'");
            case FormatType.Xml:
                var doc = XDocument.Parse(definition);
                if (doc.Descendants().Select(i => i.Name).All(i => i != anonymizedField))
                {
                    throw new InvalidInputHttpException(null,
                        $"Invalid anonymized field. Make sure tag that you want to anonymize exists and does not exist as a value in your format definition. Format definition '{definition}'");
                }

                break;
            default:
                throw new InvalidOperationException();
        }
    }

    private static void EnsureRuleCreated(string? formatId, List<RuleDao>? formatRules, string? id)
    {
        ArgumentNullException.ThrowIfNull(formatRules);

        if (formatRules.All(rule => rule.Id != id))
        {
            throw new InvalidInputHttpException(null,
                $"Rule '{id}' is not found for '{formatId}' format");
        }
    }

    private static void EnsureRuleNotCreated(string? formatId, List<RuleDao>? formatRules, CreateRuleDto createDto)
    {
        ArgumentNullException.ThrowIfNull(formatRules);

        if (formatRules.Any(rule => rule.Title == createDto.Title))
        {
            throw new InvalidInputHttpException(null,
                $"Rule '{createDto.Title}' already exists for '{formatId}' format");
        }
    }

    private static void EnsureValidId(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new InvalidInputHttpException(null, "Invalid id");
        }
    }

    private static void EnsureValidArgs(params string[]? args)
    {
        var isValid = !args?.Any(string.IsNullOrWhiteSpace) ?? false;
        if (!isValid)
        {
            throw new InvalidInputHttpException(null, "Invalid input");
        }
    }

    private async Task<FormatDao> ResolveFormatAsync(string orgId, string formatId, CancellationToken token)
    {
        var orgs = await _uow
            .GetAsync(SearchPatterns.Org.SelectOrgById(orgId),
                i => i.Include(j => j.Formats!).ThenInclude(j => j.Rules!), token)
            .ConfigureAwait(false);

        orgs.EnsureSingle();
        var org = orgs.SingleOrDefault() ?? throw new Exception();
        org.EnsureSingleFormat(formatId);
        var format = org.Formats?.SingleOrDefault(i => i.Id == formatId) ?? throw new Exception();
        return format;
    }
}
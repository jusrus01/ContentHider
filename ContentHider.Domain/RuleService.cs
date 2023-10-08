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
    private const string HiddenContent = "HIDDEN";
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

    public async Task<TransformedTextDto> ApplyAsync(string orgId, string formatId, string text,
        CancellationToken token)
    {
        EnsureValidId(orgId);
        EnsureValidId(formatId);
        EnsureValidId(text);

        var format = await ResolveFormatAsync(orgId, formatId, token);
        var formatDefinition = format.FormatDefinition ?? throw new ArgumentException();

        switch (format.Type)
        {
            case FormatType.Json:
                var formatDocument = JsonDocument.Parse(formatDefinition);
                JsonDocument? textDocument = null;
                try
                {
                    textDocument = JsonDocument.Parse(text);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw new InvalidInputHttpException(null,
                        $"Received text does not comply with this format definition '{formatDefinition}'");
                }

                var formatObjects = formatDocument.RootElement
                    .EnumerateObject()
                    .Select(i => i)
                    .ToList();
                var textObjects = textDocument.RootElement
                    .EnumerateObject()
                    .Select(i => i)
                    .ToList();
                EnsureMatchesFormat(formatObjects, textObjects, formatDefinition);

                var rules = format.Rules ?? new List<RuleDao>();
                var dictionary = new Dictionary<string, object>();

                foreach (var property in textObjects)
                {
                    if (rules.Any(rule => rule.AnonymizedField == property.Name))
                    {
                        dictionary.Add(property.Name, HiddenContent);
                    }
                    else
                    {
                        dictionary.Add(property.Name, property.Value);
                    }
                }

                return new TransformedTextDto(JsonSerializer.Serialize(dictionary));
            case FormatType.Xml:
                var formatElements = XElement.Parse(formatDefinition).Elements().ToList();
                XElement? textRootElement = null;
                List<XElement>? textElements = null;
                try
                {
                    textRootElement = XElement.Parse(text);
                    textElements = textRootElement.Elements().ToList() ?? new List<XElement>();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw new InvalidInputHttpException(null,
                        $"Received text does not comply with this format definition '{formatDefinition}'");
                }

                EnsureMatchesFormat(formatElements, textElements, formatDefinition);

                var elementRules = format.Rules ?? new List<RuleDao>();
                foreach (var property in textElements)
                {
                    if (elementRules.Any(rule => rule.AnonymizedField == property.Name))
                    {
                        property.SetValue(HiddenContent);
                    }
                }

                return new TransformedTextDto(textRootElement.ToString());

            default:
                throw new NotSupportedException();
        }
    }

    private static void EnsureMatchesFormat(List<XElement> formatElements, List<XElement> textElements,
        string formatDefinition)
    {
        if (formatElements.Count != textElements.Count)
        {
            throw new InvalidInputHttpException(null,
                $"Received text does not comply with this format definition '{formatDefinition}'");
        }

        if (formatElements.Any(property => textElements.All(i => i.Name != property.Name)))
        {
            throw new InvalidInputHttpException(null,
                $"Received text does not comply with this format definition '{formatDefinition}'");
        }
    }

    private static void EnsureMatchesFormat(List<JsonProperty> formatObjects, List<JsonProperty> textObjects,
        string formatDefinition)
    {
        if (formatObjects.Count != textObjects.Count)
        {
            throw new InvalidInputHttpException(null,
                $"Received text does not comply with this format definition '{formatDefinition}'");
        }

        if (formatObjects.Any(property => textObjects.All(i => i.Name != property.Name)))
        {
            throw new InvalidInputHttpException(null,
                $"Received text does not comply with this format definition '{formatDefinition}'");
        }
    }

    private static void EnsureRuleIsValidForFormatDefinition(FormatDao format, string anonymizedField)
    {
        var formatDefinition = format.FormatDefinition ?? throw new ArgumentException();
        switch (format.Type)
        {
            case FormatType.Json:
                var document = JsonDocument.Parse(formatDefinition);
                var root = document.RootElement;

                foreach (var property in root.EnumerateObject())
                {
                    if (property.Name == anonymizedField)
                    {
                        return;
                    }
                }

                throw new InvalidInputHttpException(null,
                    $"Invalid anonymized field. Make sure tag that you want to anonymize exists and does not exist as a value in your format definition. Format definition '{formatDefinition}'");
            case FormatType.Xml:
                var doc = XDocument.Parse(formatDefinition);
                if (doc.Descendants().Select(i => i.Name).All(i => i != anonymizedField))
                {
                    throw new InvalidInputHttpException(null,
                        $"Invalid anonymized field. Make sure tag that you want to anonymize exists and does not exist as a value in your format definition. Format definition '{formatDefinition}'");
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
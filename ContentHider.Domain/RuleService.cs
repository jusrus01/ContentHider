using ContentHider.Core.Daos;
using ContentHider.Core.Dtos.Rules;
using ContentHider.Core.Exceptions;
using ContentHider.Core.Extensions;
using ContentHider.Core.Repositories;
using ContentHider.Core.Services;

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
        EnsureValidArgs(createDto.Title);

        var format = await ResolveFormatAsync(orgId, formatId, token);
        EnsureRuleNotCreated(format.Id, format.Rules, createDto);

        var rule = new RuleDao
        {
            Id = Guid.NewGuid().ToString(),
            FormatId = format.Id,
            Title = createDto.Title
        };

        await _uow.SaveAsync(rule, token);

        return ToDto(rule);
    }


    public async Task<RuleDto> UpdateAsync(string orgId, string formatId, string id, UpdateRuleDto updateDto,
        CancellationToken token)
    {
        EnsureValidId(orgId);
        EnsureValidId(formatId);
        EnsureValidArgs(updateDto.Title);
        EnsureValidId(id);

        var format = await ResolveFormatAsync(orgId, formatId, token);
        EnsureRuleCreated(format.Id, format.Rules, id);

        var rule = format.Rules!.Single(rule => rule.Id == id);

        rule.Title = updateDto.Title;

        await _uow.UpdateAsync(rule, token);

        return ToDto(rule);
    }

    public async Task<RuleDto> DeleteAsync(string orgId, string formatId, string id, CancellationToken token)
    {
        EnsureValidId(orgId);
        EnsureValidId(formatId);
        EnsureValidId(id);

        var format = await ResolveFormatAsync(orgId, formatId, token);
        EnsureRuleCreated(format.Id, format.Rules, id);

        var rule = format.Rules!.Single(rule => rule.Id == id);
        var dto = ToDto(rule);

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

        return ToDto(rule);
    }

    public async Task<IEnumerable<RuleDto>> GetAllAsync(string orgId, string formatId, CancellationToken token)
    {
        EnsureValidId(orgId);
        EnsureValidId(formatId);

        var format = await ResolveFormatAsync(orgId, formatId, token);
        return format.Rules!.Select(ToDto);
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

    private static void EnsureValidArgs(string? title)
    {
        var isValid = !string.IsNullOrWhiteSpace(title);

        if (!isValid)
        {
            throw new InvalidInputHttpException(null, "Invalid input");
        }
    }

    private static RuleDto ToDto(RuleDao dao)
    {
        return new RuleDto(dao.Id, dao.Title);
    }

    private async Task<FormatDao> ResolveFormatAsync(string orgId, string formatId, CancellationToken token)
    {
        var orgs = await _uow
            .GetAsync(i => i.Formats!, SearchPatterns.Org.SelectOrgById(orgId),
                i => i.Formats!.Select(format => format.Rules), token)
            .ConfigureAwait(false);

        orgs.EnsureSingle();
        var org = orgs.SingleOrDefault() ?? throw new Exception();
        org.EnsureSingleFormat(formatId);
        var format = org.Formats?.SingleOrDefault(i => i.Id == formatId) ?? throw new Exception();
        return format;
    }
}
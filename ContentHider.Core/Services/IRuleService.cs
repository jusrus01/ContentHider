using ContentHider.Core.Dtos.Rules;

namespace ContentHider.Core.Services;

public interface IRuleService
{
    Task<RuleDto> CreateAsync(string orgId, string formatId, CreateRuleDto createDto, CancellationToken token);

    Task<RuleDto> UpdateAsync(string orgId, string formatId, string id, UpdateRuleDto updateDto,
        CancellationToken token);

    Task<RuleDto> DeleteAsync(string orgId, string formatId, string id, CancellationToken token);
    Task<RuleDto> GetByIdAsync(string orgId, string formatId, string id, CancellationToken token);
    Task<IEnumerable<RuleDto>> GetAllAsync(string orgId, string formatId, CancellationToken token);

    Task<string> ApplyAsync(string orgId, string formatId, string text, CancellationToken token);
}
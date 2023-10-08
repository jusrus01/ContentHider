using ContentHider.Core.Dtos.Rules;
using ContentHider.Core.Services;

namespace ContentHider.Domain;

public class RuleService : IRuleService
{
    public Task<RuleDto> CreateAsync(string orgId, string formatId, CreateRuleDto createDto, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<RuleDto> UpdateAsync(string orgId, string formatId, string id, UpdateRuleDto updateDto,
        CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<RuleDto> DeleteAsync(string orgId, string formatId, string id, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<RuleDto> GetByIdAsync(string orgId, string formatId, string id, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<RuleDto>> GetAllAsync(string orgId, string formatId, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
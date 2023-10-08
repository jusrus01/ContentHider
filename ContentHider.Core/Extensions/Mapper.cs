using ContentHider.Core.Daos;
using ContentHider.Core.Dtos.Formats;
using ContentHider.Core.Dtos.Organizations;
using ContentHider.Core.Dtos.Rules;

namespace ContentHider.Core.Extensions;

public static class Mapper
{
    public static RuleDto ToDto(RuleDao dao)
    {
        return new RuleDto(dao.Id, dao.Title);
    }

    public static FormatDto ToDto(FormatDao format)
    {
        var rules = format.Rules?.Select(ToDto).ToList();

        return new FormatDto(format.Id, format.OrganizationId, format.Title, rules, format.Type,
            format.FormatDefinition);
    }

    public static OrgDto ToDto(OrganizationDao org)
    {
        var formats = org.Formats?.Select(ToDto)
            .ToList();

        return new OrgDto(org.Id, org.Title, org.Description, formats);
    }
}
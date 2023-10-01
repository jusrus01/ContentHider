using ContentHider.Core.Daos;
using ContentHider.Core.Exceptions;

namespace ContentHider.Core.Extensions;

public static class OrganizationExtensions
{
    public static void EnsureSingle(this List<OrganizationDao> orgs)
    {
        if (orgs.Count != 1)
        {
            throw new InvalidInputHttpException(null, "Org not found");
        }
    }

    public static void EnsureEmpty(this List<OrganizationDao> orgs)
    {
        if (orgs.Any())
        {
            throw new InvalidInputHttpException(null, "Org already exists");
        }
    }

    public static void EnsureFormatDoesNotExist(this OrganizationDao org, FormatDao format)
    {
        ArgumentNullException.ThrowIfNull(org.Formats);

        if (org.Formats.Any(assignedFormat => assignedFormat.Id == format.Id))
        {
            throw new InvalidInputHttpException(null, $"Format for '{org.Id}' org already exists");
        }
    }
}
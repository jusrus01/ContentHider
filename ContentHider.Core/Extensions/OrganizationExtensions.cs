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

        if (org.Formats.Any(assignedFormat => assignedFormat.Id == format.Id || assignedFormat.Title == format.Title))
        {
            throw new InvalidInputHttpException(null, $"Format for '{org.Id}' org already exists");
        }
    }

    public static void EnsureSingleFormat(this OrganizationDao org, string id)
    {
        ArgumentNullException.ThrowIfNull(org.Formats);

        var orgFormat = org.Formats.SingleOrDefault(assignedFormat =>
            assignedFormat.Id == id);
        if (orgFormat == null)
        {
            throw new InvalidInputHttpException(null, $"Cannot find format with id '{id}'");
        }
    }
}
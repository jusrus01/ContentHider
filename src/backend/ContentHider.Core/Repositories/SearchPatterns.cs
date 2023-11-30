using System.Linq.Expressions;
using ContentHider.Core.Daos;
using ContentHider.Core.Dtos.Organizations;

namespace ContentHider.Core.Repositories;

public static class SearchPatterns
{
    public static class Org
    {
        public static Expression<Func<OrganizationDao, bool>> SelectOrgById(string id)
        {
            return o => o.Id == id;
        }

        public static Expression<Func<OrganizationDao, bool>> SearchOrgByTitle(CreateOrgDto dto)
        {
            return o => o.Title == dto.Title;
        }
    }
}
using System;
using System.Threading.Tasks;
using ContentHider.Core.Entities;

namespace ContentHider.Core.Services;

public interface IOrganizationService
{
    OrganizationDao Create(Guid userId);

    Task SaveAsync(OrganizationDao dao);
}
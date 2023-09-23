using System;
using ContentHider.Core.Daos;

namespace ContentHider.Core.Entities;

public class OrganizationDao : Dao
{
    public Guid Id { get; init; }
    public string? Title { get; init; }

    public string? Description { get; init; }

    public Guid OwnerId { get; init; }
}
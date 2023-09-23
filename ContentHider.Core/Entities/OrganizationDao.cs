using System;

namespace ContentHider.Core.Entities;

public class OrganizationDao : Dao
{
    public Guid Id { get; init; }
    public string? Name { get; init; }

    public Guid OwnerId { get; init; }
}
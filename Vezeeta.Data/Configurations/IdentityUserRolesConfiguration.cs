﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vezeeta.Data.Configurations;

internal class IdentityUserRolesConfiguration : IEntityTypeConfiguration<IdentityUserRole<int>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<int>> builder)
    {
        builder.HasData(new IdentityUserRole<int>() { RoleId = 1, UserId = 1});
    }
}

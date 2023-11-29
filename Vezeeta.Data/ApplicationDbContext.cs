﻿using Microsoft.EntityFrameworkCore;
using Vezeeta.Core.Models;
using Vezeeta.Data.Configurations;

namespace Vezeeta.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new SpecializationConfiguration());

    }

    public DbSet<User> Users { get; set; }
    public DbSet<Specialization> Specializations { get; set; }
}

﻿using Vezeeta.Core.Models;
using Vezeeta.Data.Configurations;
using Vezeeta.Core.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Vezeeta.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CouponConfiguration());
        modelBuilder.ApplyConfiguration(new DoctorConfiguration());
        modelBuilder.ApplyConfiguration(new BookingsConfiguration());
        modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
        modelBuilder.ApplyConfiguration(new SpecializationConfiguration());
        modelBuilder.ApplyConfiguration(new AppointmentTimeConfiguration());
        modelBuilder.ApplyConfiguration(new ApplicationRoleConfiguration());
        modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
        modelBuilder.ApplyConfiguration(new IdentityUserRolesConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Specialization> Specializations { get; set; }
    public DbSet<AppointmentTime> AppointmentTimes { get; set; }
}

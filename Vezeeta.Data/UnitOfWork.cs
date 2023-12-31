﻿using Vezeeta.Core;
using Vezeeta.Core.Models;
using Vezeeta.Core.Repositories;
using Vezeeta.Data.Repositories;
using Vezeeta.Core.Models.Identity;

namespace Vezeeta.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IBaseRepository<Coupon> Coupons { get; private set; }
    public IBaseRepository<Doctor> Doctors { get; private set; }
    public IBaseRepository<Booking> Bookings { get; private set; }
    public IBaseRepository<Appointment> Appointments { get; private set; }
    public IBaseRepository<Specialization> Specializations { get; private set; }
    public IBaseRepository<ApplicationUser> ApplicationUsers { get; private set; }
    public IBaseRepository<AppointmentTime> AppointmentTimes { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Coupons = new BaseRepository<Coupon>(_context);
        Doctors = new BaseRepository<Doctor>(_context);
        Bookings = new BaseRepository<Booking>(_context);
        Appointments = new BaseRepository<Appointment>(_context);
        Specializations = new BaseRepository<Specialization>(_context);
        AppointmentTimes = new BaseRepository<AppointmentTime>(_context);
        ApplicationUsers = new BaseRepository<ApplicationUser>(_context);
    }
    public void Commit() => _context.SaveChanges();

    public Task CommitAsync() => _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}

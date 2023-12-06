﻿using Vezeeta.Core;
using Vezeeta.Core.Models;
using Vezeeta.Core.Services;

namespace Vezeeta.Services.Local;

public class CouponService : ICouponService
{
    private readonly IUnitOfWork _unitOfWork;

    public CouponService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Coupon?> GetById(int id) => await _unitOfWork.Coupons.GetByIdAsync(id);
    public async Task<Coupon?> GetByDiscountCode(string discountCode)
    {
        return await _unitOfWork.Coupons.FindWithCriteriaAndIncludesAsync(e => e.DiscountCode == discountCode);
    }
    public async Task<Coupon> Create(Coupon coupon)
    {
        coupon.Active = true;
        await _unitOfWork.Coupons.AddAsync(coupon);
        await _unitOfWork.CommitAsync();
        return coupon;
    }
    public async Task Update(Coupon coupon)
    {
        _unitOfWork.Coupons.Update(coupon);
        await _unitOfWork.CommitAsync();
    }
    public async Task Delete(Coupon coupon)
    {
        _unitOfWork.Coupons.Delete(coupon);
        await _unitOfWork.CommitAsync();
    }

    public async Task Deactivate(Coupon coupon)
    {
        coupon.Active = false;
        await _unitOfWork.CommitAsync();
    }

}
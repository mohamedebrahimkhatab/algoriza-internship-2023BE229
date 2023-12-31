﻿using Vezeeta.Core.Models;

namespace Vezeeta.Core.Services;

public interface ISpecializationService
{
    Task<IEnumerable<Specialization>> GetAll();
    Task<Specialization?> GetById(int id);
    Task<Specialization?> GetByName(string name);
    Task<IEnumerable<Specialization>> FindBySearch(string search);
}

﻿using System.Reflection.Metadata.Ecma335;
using Vezeeta.Core.Models.Identity;

namespace Vezeeta.Core.Models;

public class Doctor : BaseEntity
{
    public decimal? Price { get; set; }

    public int ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }

    public int SpecializationId { get; set; }
    public Specialization? Specialization { get; set; }

    public List<Appointment>? Appointments { get; set; }
}

﻿namespace Vezeeta.Core.Contracts.PatientDtos;

public class DoctorGetPatientDto
{
    public string? PhotoPath { get; set; }
    public string? PatientName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Gender { get; set; }
    public int Age { get; set; }
    public string? Appointment { get; set; }
}

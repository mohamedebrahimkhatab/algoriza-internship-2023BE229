﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vezeeta.Core.Consts;
using Vezeeta.Core.Contracts.AppointmentDtos;
using Vezeeta.Core.Models;
using Vezeeta.Core.Services;

namespace Vezeeta.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize(Roles = UserRoles.Doctor)]
public class AppointmentsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IMapper mapper, IAppointmentService appointmentService)
    {
        _mapper = mapper;
        _appointmentService = appointmentService;
    }

    [HttpPost]
    public async Task<IActionResult> Add(DoctorAppointmentDto dto)
    {
        int userId = int.Parse(User.FindFirstValue("Id"));
        int doctorId = await _appointmentService.GetDoctorId(userId);
        var appointments = _mapper.Map<List<Appointment>>(dto.Days);
        await _appointmentService.AddAppointmentsAndPrice(doctorId, dto.Price, appointments);
        return Created();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTime(UpdateTimeDto timeDto)
    {
        try
        {
            AppointmentTime? time = await _appointmentService.GetAppointmentTime(timeDto.Id);
            if (time == null)
            {
                return NotFound("Time not Found");
            }
            time = _mapper.Map(timeDto, time);
            await _appointmentService.UpdateAppointmentTime(time);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTime(int id)
    {
        AppointmentTime? time = await _appointmentService.GetAppointmentTime(id);
        if (time == null)
        {
            return NotFound("Time not Found");
        }
        await _appointmentService.DeleteAppointmentTime(time);
        return NoContent();
    }
}

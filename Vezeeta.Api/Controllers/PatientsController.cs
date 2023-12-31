﻿using AutoMapper;
using Vezeeta.Core.Consts;
using Microsoft.AspNetCore.Mvc;
using Vezeeta.Core.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Vezeeta.Core.Contracts.PatientDtos;
using Vezeeta.Core.Services;
using System.Collections.Generic;
using Vezeeta.Core.Models;
using Vezeeta.Core.Contracts.BookingDtos;
using Microsoft.AspNetCore.Authorization;
using Vezeeta.Api.Validators;
using Vezeeta.Core.Contracts.CouponDtos;

namespace Vezeeta.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class PatientsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPatientService _patientService;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public PatientsController(IMapper mapper, UserManager<ApplicationUser> userManager, IPatientService patientService, IWebHostEnvironment hostingEnvironment)
    {
        _mapper = mapper;
        _userManager = userManager;
        _patientService = patientService;
        _hostingEnvironment = hostingEnvironment;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterPatientDto patientDto)
    {
        try
        {
            var validator = new RegisterPatientDtoValidator();
            var validate = await validator.ValidateAsync(patientDto);
            if (!validate.IsValid)
            {
                return BadRequest(validate.Errors.Select(e => e.ErrorMessage));
            }
            ApplicationUser? user = await _userManager.FindByEmailAsync(patientDto.Email);
            if (user != null)
            {
                return BadRequest("this email is already taken");
            }

            ApplicationUser patient = _mapper.Map<ApplicationUser>(patientDto);
            patient.PhotoPath = ProcessUploadedFile(patientDto.Image);
            IdentityResult result = await _userManager.CreateAsync(patient, patientDto.Password);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors.ToString());
            }

            await _userManager.AddToRoleAsync(patient, UserRoles.Patient);

            return Created();
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    private string ProcessUploadedFile(IFormFile photo)
    {
        string uniqueFileName = null;
        if (photo != null)
        {

            string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                photo.CopyTo(fileStream);
            }
        }

        return uniqueFileName;
    }

    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<IEnumerable<GetPatientDto>>> GetAll(int? page, int? pageSize, string? search)
    {
        IEnumerable<ApplicationUser> result = await _patientService.GetAll(page ?? 1, pageSize ?? 10, search ?? "");
        return Ok(_mapper.Map<IEnumerable<GetPatientDto>>(result));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> GetById(int id)
    {
        ApplicationUser? patient = await _patientService.GetById(id);
        if(patient == null)
        {
            return NotFound("Patient not found");
        }
        GetPatientDto patientDto = _mapper.Map<GetPatientDto>(patient);
        IEnumerable<Booking> bookings = await _patientService.GetPatientBookings(id);
        return Ok(new GetByIdPatientDto { 
            Details = _mapper.Map<GetPatientDto>(patient),
            Bookings = _mapper.Map<List<PatientGetBookingDto>>(bookings)
        });
    }
}

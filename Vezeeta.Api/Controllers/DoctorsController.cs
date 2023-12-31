﻿using AutoMapper;
using Vezeeta.Core.Consts;
using Vezeeta.Core.Models;
using Vezeeta.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Vezeeta.Core.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Vezeeta.Core.Contracts.DoctorDtos;
using Microsoft.AspNetCore.Authorization;
using Vezeeta.Api.Validators;
using Vezeeta.Core.Contracts.CouponDtos;
using FluentValidation;
using Vezeeta.Services.EmailService;

namespace Vezeeta.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISpecializationService _specializationService;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly IEmailSender _emailSender;

    public DoctorsController(IDoctorService doctorService,
                            IMapper mapper,
                            UserManager<ApplicationUser> userManager,
                            ISpecializationService specializationService,
                            IWebHostEnvironment hostingEnvironment,
                            IEmailSender emailSender)
    {
        _doctorService = doctorService;
        _mapper = mapper;
        _userManager = userManager;
        _specializationService = specializationService;
        _hostingEnvironment = hostingEnvironment;
        _emailSender = emailSender;
    }

    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<IEnumerable<AdminGetDoctorDto>>> AdminGetAll(int? page, int? pageSize, string? search)
    {

        IEnumerable<Doctor> result = await _doctorService.AdminGetAll(page ?? 1, pageSize ?? 10, search ?? "");
        return Ok(_mapper.Map<IEnumerable<AdminGetDoctorDto>>(result));
    }

    [HttpGet]
    [Authorize(Roles = UserRoles.Patient)]
    public async Task<ActionResult<IEnumerable<AdminGetDoctorDto>>> PatientGetAll(int? page, int? pageSize, string? search)
    {

        IEnumerable<Doctor> result = await _doctorService.PatientGetAll(page ?? 1, pageSize ?? 10, search ?? "");
        return Ok(_mapper.Map<IEnumerable<PatientGetDoctorDto>>(result));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<GetIdDoctorDto>> GetById(int id)
    {
        Doctor? result = await _doctorService.GetById(id);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<GetIdDoctorDto>(result));
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Add(CreateDoctorDto doctorDto)
    {
        try
        {
            var validator = new CreateDoctorDtoValidator();
            var validate = await validator.ValidateAsync(doctorDto);
            if (!validate.IsValid)
            {
                return BadRequest(validate.Errors.Select(e => e.ErrorMessage));
            }
            ApplicationUser? user = await _userManager.FindByEmailAsync(doctorDto.Email);
            if (user != null)
            {
                return BadRequest("this email is already taken");
            }

            Specialization specialization = await _specializationService.GetById(doctorDto.SpecializationId);

            if (specialization == null)
            {
                return NotFound("Specialization Does not exist");
            }

            Doctor doctor = _mapper.Map<Doctor>(doctorDto);
            doctor.SpecializationId = specialization.Id;
            doctor.ApplicationUser.PhotoPath = ProcessUploadedFile(doctorDto.Image);
            IdentityResult result = await _userManager.CreateAsync(doctor.ApplicationUser, "Doc*1234");

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors.ToString());
            }

            await _userManager.AddToRoleAsync(doctor.ApplicationUser, UserRoles.Doctor);

            await _doctorService.Create(doctor);
            var message = new Message(new string[] { doctorDto.Email }, "Your Pass", "Doc*1234");
            await _emailSender.SendEmailAsync(message);
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

    [HttpPut]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Edit(UpdateDoctorDto doctorDto)
    {
        try
        {
            var validator = new UpdateDoctorDtoValidator();
            var validate = await validator.ValidateAsync(doctorDto);
            if (!validate.IsValid)
            {
                return BadRequest(validate.Errors.Select(e => e.ErrorMessage));
            }
            Doctor? doctor = await _doctorService.GetById(doctorDto.DoctorId);
            if (doctor == null)
            {
                return NotFound("this doctor is not found");
            }

            if (doctorDto.Image != null)
            {
                if (doctorDto.PhotoPath != null)
                {
                    string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", doctorDto.PhotoPath);
                    System.IO.File.Delete(filePath);
                }
                doctorDto.PhotoPath = ProcessUploadedFile(doctorDto.Image);
            }

            _mapper.Map(doctorDto, doctor);

            await _doctorService.Update(doctor);

            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            Doctor? doctor = await _doctorService.GetById(id);
            if (doctor == null)
                return NotFound("Doctor is not exist");
            var user = doctor.ApplicationUser;
            await _doctorService.Delete(doctor);
            await _userManager.DeleteAsync(user);
            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
}

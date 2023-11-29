﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vezeeta.Core.Models;
using Vezeeta.Core.Services;

namespace Vezeeta.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SpecializationsController : ControllerBase
    {
        private readonly ISpecializationService _specializationService;

        public SpecializationsController(ISpecializationService specializationService)
        {
            _specializationService = specializationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Specialization>>> GetAll() => Ok(await _specializationService.GetAll());

        [HttpGet]
        public async Task<ActionResult<Specialization>> GetById(int id)
        {
            var result =  await _specializationService.GetById(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<IEnumerable<Specialization>> GetByName(string name) => await _specializationService.GetByName(name);
    }
}
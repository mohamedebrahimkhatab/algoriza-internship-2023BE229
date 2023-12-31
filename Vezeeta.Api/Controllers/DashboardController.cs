﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vezeeta.Core.Consts;
using Vezeeta.Core.Enums;
using Vezeeta.Core.Services;

namespace Vezeeta.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize(Roles = UserRoles.Admin)]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<ActionResult<int>> NumOfDoctors(SearchBy? search) => Ok(await _dashboardService.GetNumOfDoctors(search));

    [HttpGet]
    public async Task<ActionResult<int>> NumOfPatients(SearchBy? search) => Ok(await _dashboardService.GetNumOfPatients(search));

    [HttpGet]
    public async Task<IActionResult> NumOfRequests(SearchBy? search) => Ok(await _dashboardService.GetNumOfRequests(search));

    [HttpGet]
    public async Task<IActionResult> Top5Specializations() => Ok(await _dashboardService.GetTop5Speializations());
    
    [HttpGet]
    public async Task<IActionResult> Top10Doctors() => Ok(await _dashboardService.GetTop10Doctors());

}

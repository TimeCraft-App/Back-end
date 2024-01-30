using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeCraft.Core.Services.LeaveManager;
using TimeCraft.Domain.Dtos.LeaveManagerDtos;

namespace TimeCraft.Api.Controllers
{
    public class LeaveManagerController : BaseController
    {
        private readonly ILeaveManagerService _leaveManagerService;
        private readonly ILogger<LeaveManagerController> _logger;

        public LeaveManagerController(ILeaveManagerService leaveManagerService, ILogger<LeaveManagerController> logger)
        {
            _leaveManagerService = leaveManagerService;
            _logger = logger;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, User, HR, SecurityManager")]
        [HttpPost("/apply")]
        public async Task<IActionResult> ApplyForTimeoffRequest(TimeoffRequestApplicationDto data)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst("Id").Value;

            var submitted = await _leaveManagerService.ApplyForTimeoffRequest(userId, data);
            if (submitted)
            {
                _logger.LogInformation($"{nameof(LeaveManagerController)}: The timeoff request application is submitted successfully!");
                return Ok("The timeoff request application is submitted successfully!\"");
            }
            else
            {
                return BadRequest("The timeoff request couldn't be made!");
            }
        }
    }
}

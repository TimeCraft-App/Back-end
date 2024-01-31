using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeCraft.Core.Services.LeaveManager;
using TimeCraft.Core.Services.TimeoffBalanceService;
using TimeCraft.Domain.Dtos.LeaveManagerDtos;
using TimeCraft.Domain.Entities;
using TimeCraft.Domain.Enums;
using TimeCraft.Infrastructure.Persistence.UnitOfWork;

namespace TimeCraft.Api.Controllers
{
    public class LeaveManagerController : BaseController
    {
        private readonly ILeaveManagerService _leaveManagerService;
        private readonly ITimeoffBalanceService<TimeoffBalance> _timeoffBalanceService;
        private readonly ILogger<LeaveManagerController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        // These are updated when get balance for employee is requested
        public int VacationDaysLeft { get; set; } = -1;
        public int SickDaysLeft { get; set; } = -1;
        public int PersonalDaysLeft { get; set; } = -1;
        public int OthersDaysLeft { get; set; } = -1;
        public WorkStatusType? WorkStatus { get; set; } = null;

        public LeaveManagerController(ILeaveManagerService leaveManagerService, ILogger<LeaveManagerController> logger, ITimeoffBalanceService<TimeoffBalance> timeoffBalanceService, IUnitOfWork unitOfWork)
        {
            _leaveManagerService = leaveManagerService;
            _logger = logger;
            _timeoffBalanceService = timeoffBalanceService;
            _unitOfWork = unitOfWork;
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


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, HR")]
        [HttpPost("/approve")]
        public async Task<IActionResult> ApproveTimeoffRequest(int id)
        {
            try
            {
                await _leaveManagerService.ApproveTimeoffRequest(id);

                _logger.LogInformation($"{nameof(LeaveManagerController)}: The timeoff request with id: {id} status is changed successfully!");
                return Ok("The timeoff request status is changed successfully!\"");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(LeaveManagerController)}: Sth bad happened during the approval of the timeoff request!");
                return BadRequest("The timeoff request status didn't change!");
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, HR")]
        [HttpPost("/reject")]
        public async Task<IActionResult> RejectTimeoffRequest(int id)
        {
            try
            {
                await _leaveManagerService.RejectTimeoffRequest(id);

                _logger.LogInformation($"{nameof(LeaveManagerController)}: The timeoff request with id: {id} status is changed successfully!");
                return Ok("The timeoff request status is changed successfully!\"");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(LeaveManagerController)}: Sth bad happened during the rejection of the timeoff request!");
                return BadRequest("The timeoff request status didn't change!");
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, HR")]
        [HttpPut("/changeBalance")]
        public async Task<IActionResult> ChangeBalance(int employeeId, int quantity, TimeoffType type)
        {
            try
            {
                await _timeoffBalanceService.ChangeBalance(employeeId, quantity, type);

                _logger.LogInformation($"{nameof(LeaveManagerController)}: The timeoff balance has changed successfully!");
                return Ok("The timeoff balance has changed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(LeaveManagerController)}: Sth bad happened during the changing of timebalance!");
                return BadRequest("The timeoff balance didn't change!" + ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, HR, User")]
        [HttpGet("/getBalance")]
        public async Task<IActionResult> GetBalance(int employeeId, TimeoffType type)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var role = userData.FindFirst(ClaimTypes.Role)?.Value;

            // User has permission to check only his balance
            if (role == "User")
            {
                var userId = userData.FindFirst("Id").Value;
                var existingEmployeeId = _unitOfWork.Repository<Employee>().GetByCondition(x => x.UserId == userId).Select(x => x.Id).FirstOrDefault();

                if (existingEmployeeId != employeeId)
                {
                    return BadRequest("You don't have permission to do this!");
                }
            }

            try
            {
                var balance = _timeoffBalanceService.GetBalance(employeeId, type);

                VacationDaysLeft = _timeoffBalanceService.GetBalance(employeeId, TimeoffType.Vacation);
                PersonalDaysLeft = _timeoffBalanceService.GetBalance(employeeId, TimeoffType.Personal);
                OthersDaysLeft = _timeoffBalanceService.GetBalance(employeeId, TimeoffType.Other);
                SickDaysLeft = _timeoffBalanceService.GetBalance(employeeId, TimeoffType.Sick); 

                var lastTimeoff = _unitOfWork.Repository<TimeoffRequest>().GetByCondition(x => x.EmployeeId == employeeId && x.Status == "Approved").FirstOrDefault();

                if (lastTimeoff is null)
                {
                    throw new Exception("The employee hasn't used any timeoff day!");
                }
                if (lastTimeoff.StartDate <= DateTime.Now && lastTimeoff.EndDate >= DateTime.Now)
                {
                    if (Enum.TryParse<TimeoffType>(lastTimeoff.Type, out var timeoffType))
                    {
                        switch (timeoffType)
                        {
                            case TimeoffType.Sick:
                                WorkStatus = WorkStatusType.SickLeave;
                                break;
                            case TimeoffType.Vacation:
                                WorkStatus = WorkStatusType.OnVacation;
                                break;
                            case TimeoffType.Personal:
                                WorkStatus = WorkStatusType.PersonalLeave;
                                break;
                            case TimeoffType.Other:
                                WorkStatus = WorkStatusType.OtherLeave;
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    WorkStatus = WorkStatusType.Working;
                }
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return BadRequest("An exeption happened: " + ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, HR, User")]
        [HttpGet("/getBalances")]
        public async Task<IActionResult> GetBalances(int employeeId, TimeoffType type)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var role = userData.FindFirst(ClaimTypes.Role)?.Value;

            // User has permission to check only his balance
            if (role == "User")
            {
                var userId = userData.FindFirst("Id").Value;
                var existingEmployeeId = _unitOfWork.Repository<Employee>().GetByCondition(x => x.UserId == userId).Select(x => x.Id).FirstOrDefault();

                if (existingEmployeeId != employeeId)
                {
                    return BadRequest("You don't have permission to do this!");
                }
            }

            var balances = new TimeoffBalance
            {
                VacationDays = VacationDaysLeft,
                PersonalDays = PersonalDaysLeft,
                OtherTimeOffDays = OthersDaysLeft,
                SickDays = SickDaysLeft
            };

            return Ok(balances);
        }
    }
}

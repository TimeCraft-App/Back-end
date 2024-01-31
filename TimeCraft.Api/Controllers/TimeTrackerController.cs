using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using TimeCraft.Core.Services.TimeTracker;
using TimeCraft.Domain.Enums;

namespace TimeCraft.Api.Controllers
{
    public class TimeTrackerController : BaseController
    {
        private const int TOTAL_DAYS_OFF = 40; // Todo: get from settings
        private int TOTAL_USED_DAYS { get; set; } = -1;

        private ITimeTrackerService _timeTrackerService;
        public TimeTrackerController(ITimeTrackerService timeTrackerService)
        {
            _timeTrackerService = timeTrackerService;
        }

        [HttpPost("/increaseUsedDays")]
        public async Task<IActionResult> IncreaseUsedDays(int employeeId, int quantity)
        {
            try
            {
                await _timeTrackerService.IncreaseUsedDays(employeeId, quantity);

                return Ok("The used days are increased succesfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("/timeoffPossible")]
        public async Task<IActionResult> TimeoffPossible(int employeeId, TimeoffType timeoffType, int quantity)
        {
            try
            {
                var timeoffPossible = _timeTrackerService.TimeoffPossible(employeeId, timeoffType, quantity);

                return Ok("TimeoffPossible is: " + timeoffPossible);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/get_info")]
        public async Task<IActionResult> GetInfo()
        {
            return Ok($"Total days off: {TOTAL_DAYS_OFF}. " +
                $"\n +  total used days: {TOTAL_USED_DAYS}");
        }
    }
}

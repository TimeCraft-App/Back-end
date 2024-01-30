using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeCraft.Core.Services.TimeoffBalanceService;
using TimeCraft.Domain.Dtos.TimeoffBalanceDtos;
using TimeCraft.Domain.Entities;
using TimeCraft.Infrastructure.Constants;

namespace TimeCraft.Api.Controllers
{
    public class TimeoffBalanceController : Controller
    {
        private readonly ITimeoffBalanceService<TimeoffBalance> _timeoffBalanceService;
        private readonly ILogger<TimeoffBalanceController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// TimeoffBalanceController constructor
        /// </summary>
        /// <param name="timeoffBalanceService"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public TimeoffBalanceController(ITimeoffBalanceService<TimeoffBalance> timeoffBalanceService, ILogger<TimeoffBalanceController> logger, IMapper mapper)
        {
            _timeoffBalanceService = timeoffBalanceService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a specific timeoffBalance by ID
        /// </summary>
        /// <param name="id">The ID of the timeoffBalance to retrieve</param>
        /// <returns>The specified timeoffBalance</returns>
        /// <response code="200">Returns the specified timeoffBalance</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <response code="404">If the specified timeoffBalance is not found</response>
        /// <tags>TimeoffBalance</tags>
        /// <remarks>
        /// </remarks>
        [HttpGet("timeoffBalance")]
        public async Task<IActionResult> GetTimeoffBalance(int id)
        {
            var timeoffBalance = await _timeoffBalanceService.GetById(id);
            if (timeoffBalance is null)
            {
                return NotFound();
            }

            return Ok(timeoffBalance);
        }

        /// <summary>
        /// Gets all timeoffBalances in paginated form
        /// </summary>
        /// <returns>A list of timeoffBalances</returns>
        /// <response code="200">Returns the list of all timeoffBalances</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>TimeoffBalance</tags>
        /// <remarks>
        /// </remarks>
        [HttpGet("timeoffBalances")]
        public async Task<IActionResult> GetTimeoffBalances(int page = 1, int pageSize = 10)
        {
            var timeoffBalances = await _timeoffBalanceService.GetAllAsync(page, pageSize);

            return Ok(timeoffBalances);
        }

        /// <summary>
        /// Searches timeoffBalances and returns in paginated form
        /// </summary>
        /// <returns>A list of timeoffBalances</returns>
        /// <response code="200">Returns the list of all timeoffBalances</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>TimeoffBalance</tags>
        /// <remarks>
        /// </remarks>
        [HttpGet("searchTimeoffBalances")]
        public async Task<IActionResult> SearchTimeoffBalances(int employeeId = 0, int page = CommonConstants.DefaultPageNumber, int pageSize = CommonConstants.DefaultPageSize)
        {
            var result = await _timeoffBalanceService.SearchTimeoffBalances(employeeId, page, pageSize).ToListAsync();

            return Ok(result);
        }

        /// <summary>
        /// Creates a timeoffBalance!
        /// </summary>
        /// <param name="timeoffBalanceToCreate"></param>
        /// <returns></returns>
        [HttpPost("timeoffBalance")]
        public async Task<IActionResult> CreateTimeoffBalance(TimeoffBalanceCreateDto timeoffBalanceToCreate)
        {
            var timeoffBalance = _mapper.Map<TimeoffBalance>(timeoffBalanceToCreate);
            var createdId = await _timeoffBalanceService.Create(timeoffBalance);

            if (createdId <= 0)
            {
                _logger.LogInformation($"{nameof(TimeoffBalanceController)} - Couldn't create the timeoffBalance.");
                return BadRequest("Couldn't create the timeoffBalance, something went wrong");
            }

            _logger.LogInformation($"{nameof(TimeoffBalanceController)} - Created timeoffBalance successfully.");
            return Ok("TimeoffBalance is created successfully!");
        }

        /// <summary>
        /// Updates a timeoffBalance by id!
        /// </summary>
        /// <param name="timeoffBalanceToUpdate"></param>
        /// <returns></returns>
        [HttpPut("timeoffBalance")]
        public async Task<IActionResult> UpdateTimeoffBalance(TimeoffBalanceUpdateDto timeoffBalanceToUpdate)
        {
            try
            {
                var timeoffBalance = _mapper.Map<TimeoffBalance>(timeoffBalanceToUpdate);
                await _timeoffBalanceService.Update(_mapper.Map<TimeoffBalance>(timeoffBalance));

                _logger.LogInformation($"{nameof(TimeoffBalanceController)} - Updated timeoffBalance successfully, Id: {timeoffBalanceToUpdate.Id}.");
                return Ok("TimeoffBalance is updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(TimeoffBalanceController)} - Error when updating timeoffBalance, Id: {timeoffBalanceToUpdate.Id}.");
                return BadRequest("Couldn't update the timeoffBalance, something went wrong");
            }
        }

        /// <summary>
        /// Deletes a timeoffBalance by id!
        /// </summary>
        /// <param name="timeoffBalanceId"></param>
        /// <param name="softDelete"></param>
        /// <returns></returns>
        [HttpDelete("timeoffBalance")]
        public async Task<IActionResult> DeleteTimeoffBalance(int timeoffBalanceId, bool softDelete = true)
        {
            try
            {
                await _timeoffBalanceService.Delete(timeoffBalanceId, softDelete);

                _logger.LogInformation($"{nameof(TimeoffBalanceController)} - Deleted timeoffBalance successfully, Id: {timeoffBalanceId}.");
                return Ok("TimeoffBalance has been deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(TimeoffBalanceController)} - Error when deleting timeoffBalance, Id: {timeoffBalanceId}.");
                return BadRequest("Couldn't delete the timeoffBalance, something went wrong");
            }
        }
    }
}

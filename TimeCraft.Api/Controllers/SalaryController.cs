using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TimeCraft.Core.Services.SalaryService;
using TimeCraft.Domain.Dtos.SalaryDtos;
using TimeCraft.Domain.Entities;

namespace TimeCraft.Api.Controllers
{
    public class SalaryController : BaseController
    {
        private readonly ISalaryService<Salary> _salaryService;
        private readonly ILogger<SalaryController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// SalaryController constructor
        /// </summary>
        /// <param name="salaryService"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public SalaryController(ISalaryService<Salary> salaryService, ILogger<SalaryController> logger, IMapper mapper)
        {
            _salaryService = salaryService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a specific salary by ID
        /// </summary>
        /// <param name="id">The ID of the salary to retrieve</param>
        /// <returns>The specified salary</returns>
        /// <response code="200">Returns the specified salary</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <response code="404">If the specified salary is not found</response>
        /// <tags>Salary</tags>
        /// <remarks>
        /// </remarks>
        [HttpGet("salary")]
        public async Task<IActionResult> GetSalary(int id)
        {
            var salary = await _salaryService.GetById(id);
            if (salary is null)
            {
                return NotFound();
            }

            return Ok(salary);
        }

        /// <summary>
        /// Gets all salaries in paginated form
        /// </summary>
        /// <returns>A list of salaries</returns>
        /// <response code="200">Returns the list of all salaries</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Salary</tags>
        /// <remarks>
        /// </remarks>
        [HttpGet("salaries")]
        public async Task<IActionResult> GetSalarys(int page = 1, int pageSize = 10)
        {
            var salaries = await _salaryService.GetAllAsync(page, pageSize);

            return Ok(salaries);
        }

        /// <summary>
        /// Creates a salary!
        /// </summary>
        /// <param name="salaryToCreate"></param>
        /// <returns></returns>
        [HttpPost("salary")]
        public async Task<IActionResult> CreateSalary(SalaryCreateDto salaryToCreate)
        {
            var salary = _mapper.Map<Salary>(salaryToCreate);
            var createdId = await _salaryService.Create(salary);

            if (createdId <= 0)
            {
                _logger.LogInformation($"{nameof(SalaryController)} - Couldn't create the salary.");
                return BadRequest("Couldn't create the salary, something went wrong");
            }

            _logger.LogInformation($"{nameof(SalaryController)} - Created salary successfully.");
            return Ok("Salary is created successfully!");
        }

        /// <summary>
        /// Updates a salary by id!
        /// </summary>
        /// <param name="salaryToUpdate"></param>
        /// <returns></returns>
        [HttpPut("salary")]
        public async Task<IActionResult> UpdateSalary(SalaryUpdateDto salaryToUpdate)
        {
            try
            {
                var salary = _mapper.Map<Salary>(salaryToUpdate);
                await _salaryService.Update(_mapper.Map<Salary>(salary));

                _logger.LogInformation($"{nameof(SalaryController)} - Updated salary successfully, Id: {salaryToUpdate.Id}.");
                return Ok("Salary is updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(SalaryController)} - Error when updating salary, Id: {salaryToUpdate.Id}.");
                return BadRequest("Couldn't update the salary, something went wrong");
            }
        }

        /// <summary>
        /// Deletes a salary by id!
        /// </summary>
        /// <param name="salaryId"></param>
        /// <param name="softDelete"></param>
        /// <returns></returns>
        [HttpDelete("salary")]
        public async Task<IActionResult> DeleteSalary(int salaryId, bool softDelete = true)
        {
            try
            {
                await _salaryService.Delete(salaryId, softDelete);
                
                _logger.LogInformation($"{nameof(SalaryController)} - Deleted salary successfully, Id: {salaryId}.");
                return Ok("Salary has been deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(SalaryController)} - Error when deleting salary, Id: {salaryId}.");
                return BadRequest("Couldn't delete the salary, something went wrong");
            }
        }
    }
}

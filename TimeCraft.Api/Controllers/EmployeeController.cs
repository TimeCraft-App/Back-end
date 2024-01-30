using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeCraft.Core.Services.EmployeeService;
using TimeCraft.Domain.Dtos.EmployeeDtos;
using TimeCraft.Domain.Entities;
using TimeCraft.Infrastructure.Constants;

namespace TimeCraft.Api.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService<Employee> _employeeService;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// EmployeeController constructor
        /// </summary>
        /// <param name="employeeService"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public EmployeeController(IEmployeeService<Employee> employeeService, ILogger<EmployeeController> logger, IMapper mapper)
        {
            _employeeService = employeeService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a specific employee by ID
        /// </summary>
        /// <param name="id">The ID of the employee to retrieve</param>
        /// <returns>The specified employee</returns>
        /// <response code="200">Returns the specified employee</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <response code="404">If the specified employee is not found</response>
        /// <tags>Employee</tags>
        /// <remarks>
        /// </remarks>
        [HttpGet("employee")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _employeeService.GetById(id);
            if (employee is null)
            {
                return NotFound();
            }

            return Ok(employee);
        }
        
        /// <summary>
        /// Gets all employees in paginated form
        /// </summary>
        /// <returns>A list of employees</returns>
        /// <response code="200">Returns the list of all employees</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Employee</tags>
        /// <remarks>
        /// </remarks>
        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees(int page = 1, int pageSize = 10)
        {
            var employees = await _employeeService.GetAllAsync(page, pageSize);

            return Ok(employees);
        }

        /// <summary>
        /// Searches employees and returns in paginated form
        /// </summary>
        /// <returns>A list of employees</returns>
        /// <response code="200">Returns the list of all employees</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Employee</tags>
        /// <remarks>
        /// </remarks>
        [HttpGet("searchEmployees")]
        public async Task<IActionResult> SearchEmployees(int page = CommonConstants.DefaultPageNumber, int pageSize = CommonConstants.DefaultPageSize)
        {
            var result = await _employeeService.SearchEmployees(page, pageSize).ToListAsync();

            return Ok(result);
        }

        /// <summary>
        /// Creates a employee!
        /// </summary>
        /// <param name="employeeToCreate"></param>
        /// <returns></returns>
        [HttpPost("employee")]
        public async Task<IActionResult> CreateEmployee(EmployeeCreateDto employeeToCreate)
        {
            var employee = _mapper.Map<Employee>(employeeToCreate);
            var createdId = await _employeeService.Create(employee);

            if (createdId <= 0)
            {
                _logger.LogInformation($"{nameof(EmployeeController)} - Couldn't create the employee.");
                return BadRequest("Couldn't create the employee, something went wrong");
            }

            _logger.LogInformation($"{nameof(EmployeeController)} - Created employee successfully.");
            return Ok("Employee is created successfully!");
        }

        /// <summary>
        /// Updates a employee by id!
        /// </summary>
        /// <param name="employeeToUpdate"></param>
        /// <returns></returns>
        [HttpPut("employee")]
        public async Task<IActionResult> UpdateEmployee(EmployeeUpdateDto employeeToUpdate)
        {
            try
            {
                var employee = _mapper.Map<Employee>(employeeToUpdate);
                await _employeeService.Update(_mapper.Map<Employee>(employee));

                _logger.LogInformation($"{nameof(EmployeeController)} - Updated employee successfully, Id: {employeeToUpdate.Id}.");
                return Ok("Employee is updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(EmployeeController)} - Error when updating employee, Id: {employeeToUpdate.Id}.");
                return BadRequest("Couldn't update the employee, something went wrong");
            }
        }

        /// <summary>
        /// Deletes a employee by id!
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="softDelete"></param>
        /// <returns></returns>
        [HttpDelete("employee")]
        public async Task<IActionResult> DeleteEmployee(int employeeId, bool softDelete = true)
        {
            try
            {
                await _employeeService.Delete(employeeId, softDelete);
                
                _logger.LogInformation($"{nameof(EmployeeController)} - Deleted employee successfully, Id: {employeeId}.");
                return Ok("Employee has been deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(EmployeeController)} - Error when deleting employee, Id: {employeeId}.");
                return BadRequest("Couldn't delete the employee, something went wrong");
            }
        }
    }
}

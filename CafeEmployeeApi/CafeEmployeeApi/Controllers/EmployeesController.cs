using CafeEmployeeApi.DTOs;
using CafeEmployeeApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CafeEmployeeApi.Controllers
{
    /// <summary>
    /// API endpoints for managing employees.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Gets a list of employees, optionally filtered by cafe name.
        /// </summary>
        /// <param name="cafe">The name of the cafe to filter by (e.g., "The Grind House").</param>
        /// <returns>A list of employees sorted by their days worked.</returns>
        /// <response code="200">Returns the list of employees.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees([FromQuery] string? cafe)
        {
            var employees = await _employeeService.GetEmployeesAsync(cafe);
            return Ok(employees);
        }

        /// <summary>
        /// Creates a new employee.
        /// </summary>
        /// <param name="employeeDto">The data for the new employee.</param>
        /// <returns>The newly created employee.</returns>
        /// <response code="201">Returns the newly created employee.</response>
        /// <response code="400">If the request is invalid (e.g., email is already in use).</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeDto>> PostEmployee(CreateOrUpdateEmployeeDto employeeDto)
        {
            var (newEmployee,error) = await _employeeService.CreateEmployeeAsync(employeeDto);

            // The service returns null if a business rule (e.g., unique email) is violated.
            if (error != null)
            {
                // Differentiate between conflict error and other validation errors.
                return error.Contains("same email already exists") ? Conflict(new { message = error }) : BadRequest(new { message = error });
            }

            // Return a 201 Created status with a location header pointing to the new resource.
            return CreatedAtAction(nameof(GetEmployees), new { id = newEmployee.Id }, newEmployee);
        }

        /// <summary>
        /// Updates an existing employee.
        /// </summary>
        /// <param name="id">The unique ID of the employee to update.</param>
        /// <param name="employeeDto">The updated data for the employee.</param>
        /// <response code="204">If the update was successful.</response>
        /// <response code="404">If the employee with the specified ID was not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutEmployee(string id, CreateOrUpdateEmployeeDto employeeDto)
        {
            var (updatedEmployee, error) = await _employeeService.UpdateEmployeeAsync(id, employeeDto);
            if (error != null)
            {
                // Differentiate between a "not found" error and other validation errors.
                return error.Contains("not found") ? NotFound(new { message = error }) : BadRequest(new { message = error });
            }
            return Ok(updatedEmployee);
        }

        /// <summary>
        /// Deletes an employee.
        /// </summary>
        /// <param name="id">The unique ID of the employee to delete.</param>
        /// <response code="204">If the deletion was successful.</response>
        /// <response code="404">If the employee with the specified ID was not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            var success = await _employeeService.DeleteEmployeeAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}

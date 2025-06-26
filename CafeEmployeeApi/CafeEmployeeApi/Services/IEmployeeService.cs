using CafeEmployeeApi.DTOs;

namespace CafeEmployeeApi.Services
{
    /// <summary>
    /// Interface for business logic operations related to employees.
    /// This layer handles mapping, validation, and orchestration.
    /// </summary>
    public interface IEmployeeService
    {
        /// <summary>
        /// Gets a list of employees, optionally filtered by cafe name, and maps them to EmployeeDto.
        /// </summary>
        /// <param name="cafeName">The name of the cafe to filter by.</param>
        /// <returns>A collection of EmployeeDto, sorted by the number of days worked in descending order.</returns>
        Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(string? cafeName);

        /// <summary>
        /// Creates a new employee based on the provided DTO.
        /// </summary>
        /// <param name="employeeDto">The DTO containing data for the new employee.</param>
        /// <returns>The created employee as an EmployeeDto, or null if the email address is already in use.</returns>
        Task<(EmployeeDto? EmployeeDto, string? Error)> CreateEmployeeAsync(CreateOrUpdateEmployeeDto employeeDto);

        /// <summary>
        /// Updates an existing employee's details.
        /// </summary>
        /// <param name="id">The unique ID of the employee to update.</param>
        /// <param name="employeeDto">The DTO containing the updated data.</param>
        /// <returns>The updated employee as an EmployeeDto, or null if the employee was not found.</returns>
        Task<(EmployeeDto? EmployeeDto, string? Error)> UpdateEmployeeAsync(string id, CreateOrUpdateEmployeeDto employeeDto);

        /// <summary>
        /// Deletes an employee by their ID.
        /// </summary>
        /// <param name="id">The unique ID of the employee to delete.</param>
        /// <returns>True if deletion was successful, otherwise false.</returns>
        Task<bool> DeleteEmployeeAsync(string id);
    }
}

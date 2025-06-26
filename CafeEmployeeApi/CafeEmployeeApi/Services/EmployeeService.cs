using CafeEmployeeApi.DTOs;
using CafeEmployeeApi.Models;
using CafeEmployeeApi.Repositories;

namespace CafeEmployeeApi.Services
{
    /// <summary>
    /// Implements the IEmployeeService interface to provide business logic for employee operations.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICafeRepository _cafeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository, ICafeRepository cafeRepository)
        {
            _employeeRepository = employeeRepository;
            _cafeRepository = cafeRepository;
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(string? cafeName)
        {
            var employees = await _employeeRepository.GetAllAsync(cafeName);

            // Map the Employee entities to EmployeeDto objects for the API response.
            return employees
                .Select(e => new EmployeeDto(
                    e.Id,
                    e.Name,
                    e.EmailAddress,
                    e.PhoneNumber,
                    e.Gender,
                    e.EmploymentDate,
                    // Calculate DaysWorked: if EmploymentDate has a value, find the difference in days. Otherwise, it's 0.
                    e.EmploymentDate.HasValue ? (DateTime.UtcNow.Date - e.EmploymentDate.Value.Date).Days : 0,
                    // Safely access the cafe's name using the null-conditional operator.
                    e.Cafe?.Name,
                    e.Cafe?.Id
                ))
                // Finally, sort the result by DaysWorked in descending order as per requirements.
                .OrderByDescending(e => e.DaysWorked);
        }

        public async Task<(EmployeeDto? EmployeeDto, string? Error)> CreateEmployeeAsync(CreateOrUpdateEmployeeDto employeeDto)
        {
            // Business Rule: An employee cannot be created with an email address that is already in use.
            if (await _employeeRepository.IsEmailInUse(employeeDto.EmailAddress))
            {
                return (null, "A Employee with the same email already exists.");
            }

            var newEmployee = new Employee
            {
                // Assign a new, unique employee ID.
                Id = GenerateEmployeeId(),
                Name = employeeDto.Name,
                EmailAddress = employeeDto.EmailAddress,
                PhoneNumber = employeeDto.PhoneNumber,
                Gender = employeeDto.Gender,
                CafeId = employeeDto.CafeId,
                EmploymentDate = employeeDto.EmploymentDate
            };

            await _employeeRepository.AddAsync(newEmployee);
            await _employeeRepository.SaveChangesAsync();

            // We need the cafe name for the response DTO, so we fetch it if a CafeId was provided.
            var cafe = newEmployee.CafeId.HasValue ? await _cafeRepository.GetByIdAsync(newEmployee.CafeId.Value) : null;

            var newEmployeeDto = new EmployeeDto(
                newEmployee.Id,
                newEmployee.Name,
                newEmployee.EmailAddress,
                newEmployee.PhoneNumber,
                newEmployee.Gender,
                newEmployee.EmploymentDate,
                newEmployee.EmploymentDate.HasValue ? (DateTime.UtcNow.Date - newEmployee.EmploymentDate.Value.Date).Days : 0,
                cafe?.Name,
                cafe?.Id
            );
            return (newEmployeeDto, null);
        }

        public async Task<(EmployeeDto? EmployeeDto, string? Error)> UpdateEmployeeAsync(string id, CreateOrUpdateEmployeeDto employeeDto)
        {
            var existingEmployee = await _employeeRepository.GetByIdAsync(id);
            if (existingEmployee == null)
            {
                return (null, "Employee not found.");
            }

            // Update basic properties from the DTO.
            existingEmployee.Name = employeeDto.Name;
            existingEmployee.EmailAddress = employeeDto.EmailAddress;
            existingEmployee.PhoneNumber = employeeDto.PhoneNumber;
            existingEmployee.Gender = employeeDto.Gender; 
            existingEmployee.CafeId = employeeDto.CafeId;
            existingEmployee.EmploymentDate = employeeDto.EmploymentDate;          

            _employeeRepository.Update(existingEmployee);
            await _employeeRepository.SaveChangesAsync();

            var cafe = existingEmployee.CafeId.HasValue ? await _cafeRepository.GetByIdAsync(existingEmployee.CafeId.Value) : null;

            var updatedEmployeeDto = new EmployeeDto(
                 existingEmployee.Id, existingEmployee.Name, existingEmployee.EmailAddress, existingEmployee.PhoneNumber,
                 existingEmployee.Gender, existingEmployee.EmploymentDate,
                 existingEmployee.EmploymentDate.HasValue ? (DateTime.UtcNow.Date - existingEmployee.EmploymentDate.Value.Date).Days : 0,
                 cafe?.Name,
                 cafe?.Id
             );
            return (updatedEmployeeDto, null);
        }

        public async Task<bool> DeleteEmployeeAsync(string id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return false;

            _employeeRepository.Delete(employee);
            return await _employeeRepository.SaveChangesAsync();
        }

        /// <summary>
        /// A simple helper method to generate a unique employee ID in the format 'UIXXXXXXX'.
        /// </summary>
        /// <returns>A new, randomized employee ID string.</returns>
        private string GenerateEmployeeId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var randomPart = new string(Enumerable.Repeat(chars, 7)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return $"UI{randomPart}";
        }
    }
}

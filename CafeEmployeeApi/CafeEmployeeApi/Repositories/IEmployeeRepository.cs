using CafeEmployeeApi.Models;

namespace CafeEmployeeApi.Repositories
{
    /// <summary>
    /// Interface for data access operations related to Employee entities.
    /// This defines the contract for our repository.
    /// </summary>
    public interface IEmployeeRepository
    {
        /// <summary>
        /// Retrieves all employees, optionally filtered by the name of the cafe they work at.
        /// </summary>
        /// <param name="cafeName">The name of the cafe to filter by. If null, all employees are returned.</param>
        /// <returns>A collection of Employee entities matching the criteria.</returns>
        Task<IEnumerable<Employee>> GetAllAsync(string? cafeName);

        /// <summary>
        /// Retrieves a single employee by their unique ID.
        /// </summary>
        /// <param name="id">The unique string ID of the employee (e.g., 'UIXXXXXXX').</param>
        /// <returns>The Employee entity or null if not found.</returns>
        Task<Employee?> GetByIdAsync(string id);

        /// <summary>
        /// Adds a new employee to the database context.
        /// </summary>
        /// <param name="employee">The Employee entity to add.</param>
        /// <returns>The added Employee entity, which can be tracked by the context.</returns>
        Task<Employee> AddAsync(Employee employee);

        /// <summary>
        /// Marks an employee entity as modified in the context.
        /// </summary>
        /// <param name="employee">The Employee entity to update.</param>
        void Update(Employee employee);

        /// <summary>
        /// Marks an employee entity for deletion from the context.
        /// </summary>
        /// <param name="employee">The Employee entity to delete.</param>
        void Delete(Employee employee);

        /// <summary>
        /// Checks if an email address is already in use by another employee.
        /// </summary>
        /// <param name="email">The email address to check.</param>
        /// <returns>True if the email is in use, otherwise false.</returns>
        Task<bool> IsEmailInUse(string email);

        /// <summary>
        /// Saves all changes made in the context to the database.
        /// </summary>
        /// <returns>True if any records were changed, otherwise false.</returns>
        Task<bool> SaveChangesAsync();
    }
}

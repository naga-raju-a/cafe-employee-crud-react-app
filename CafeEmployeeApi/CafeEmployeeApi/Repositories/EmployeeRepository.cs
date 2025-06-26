using CafeEmployeeApi.Data;
using CafeEmployeeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeEmployeeApi.Repositories
{
    /// <summary>
    /// Class implementing the IEmployeeRepository for data operations on Employee entities using EF Core.
    /// </summary>
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync(string? cafeName)
        {
            // Start with a base query to get all employees.
            // Eagerly load the related Cafe to access its name without another DB query (avoids N+1 problem).
            var query = _context.Employees.Include(e => e.Cafe).AsQueryable();

            // Apply a case-insensitive filter if the cafe name is provided.
            if (!string.IsNullOrEmpty(cafeName))
            {
                query = query.Where(e => e.Cafe != null && e.Cafe.Name.ToLower() == cafeName.ToLower());
            }

            return await query.ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(string id)
        {
            // Find a single employee by their primary key, including their related cafe.
            return await _context.Employees.Include(e => e.Cafe).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            // Stage the new employee entity to be inserted into the database.
            await _context.Employees.AddAsync(employee);
            return employee;
        }

        public void Update(Employee employee)
        {
            // Stage the existing employee entity to be updated in the database.
            _context.Employees.Update(employee);
        }

        public void Delete(Employee employee)
        {
            // Stage the employee entity to be removed from the database.
            _context.Employees.Remove(employee);
        }

        public async Task<bool> IsEmailInUse(string email)
        {
            // Use AnyAsync for an efficient existence check. It translates to a `SELECT EXISTS(...)` query in SQL.
            return await _context.Employees.AnyAsync(e => e.EmailAddress.ToLower() == email.ToLower());
        }

        public async Task<bool> SaveChangesAsync()
        {
            // Commits all staged changes to the database.
            // SaveChangesAsync returns the number of state entries written to the database. We return true if at least one was changed.
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

using CafeEmployeeApi.Data;
using CafeEmployeeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeEmployeeApi.Repositories
{
    public class CafeRepository : ICafeRepository
    {
        private readonly AppDbContext _context;

        public CafeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cafe>> GetAllAsync(string? location)
        {
            // Start with a base query including related employees for employee count.
            var query = _context.Cafes.Include(c => c.Employees).AsQueryable();

            // If a location is provided, apply a filter (case-insensitive).
            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(c => c.Location.ToLower() == location.ToLower());
            }

            return await query.ToListAsync();
        }

        public async Task<Cafe?> GetByIdAsync(Guid id)
        {
            // Eagerly load employees to ensure the count is available.
            return await _context.Cafes.Include(c => c.Employees).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cafe> AddAsync(Cafe cafe)
        {
            await _context.Cafes.AddAsync(cafe);
            return cafe;
        }

        public void Update(Cafe cafe)
        {
            _context.Cafes.Update(cafe);
        }

        public void Delete(Cafe cafe)
        {
            _context.Cafes.Remove(cafe);
        }

        public async Task<bool> SaveChangesAsync()
        {
            // SaveChangesAsync returns the number of state entries written to the database.
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

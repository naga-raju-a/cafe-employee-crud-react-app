using CafeEmployeeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeEmployeeApi.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                // Look for any cafes.
                if (context.Cafes.Any())
                {
                    return; // DB has been seeded
                }

                // --- Seed Cafes ---
                var cafe1 = new Cafe { Name = "The Grind House", Description = "Artisanal coffee and pastries", Location = "Downtown", Logo = "logo1.png" };
                var cafe2 = new Cafe { Name = "Java Stop", Description = "Quick and friendly service", Location = "Uptown" };
                var cafe3 = new Cafe { Name = "Book & Bean", Description = "A quiet place to read and sip", Location = "Downtown" };

                await context.Cafes.AddRangeAsync(cafe1, cafe2, cafe3);
                await context.SaveChangesAsync();

                // --- Seed Employees ---
                // Helper to generate IDs for seeding
                string GenerateId() => "UI" + Guid.NewGuid().ToString("N").Substring(0, 7).ToUpper();

                await context.Employees.AddRangeAsync(
                    new Employee { Id = GenerateId(), Name = "John Doe", EmailAddress = "john.d@test.com", PhoneNumber = "91234567", Gender = "Male", CafeId = cafe1.Id, EmploymentDate = DateTime.UtcNow.AddDays(-100) },
                    new Employee { Id = GenerateId(), Name = "Jane Smith", EmailAddress = "jane.s@test.com", PhoneNumber = "98765432", Gender = "Female", CafeId = cafe1.Id, EmploymentDate = DateTime.UtcNow.AddDays(-50) },
                    new Employee { Id = GenerateId(), Name = "Peter Jones", EmailAddress = "peter.j@test.com", PhoneNumber = "81112222", Gender = "Male", CafeId = cafe1.Id, EmploymentDate = DateTime.UtcNow.AddDays(-200) },
                    new Employee { Id = GenerateId(), Name = "Mary Williams", EmailAddress = "mary.w@test.com", PhoneNumber = "83334444", Gender = "Female", CafeId = cafe2.Id, EmploymentDate = DateTime.UtcNow.AddDays(-30) },
                    new Employee { Id = GenerateId(), Name = "Sam Brown", EmailAddress = "sam.b@test.com", PhoneNumber = "95556666", Gender = "Male", CafeId = cafe2.Id, EmploymentDate = DateTime.UtcNow.AddDays(-400) },
                    new Employee { Id = GenerateId(), Name = "Chris Wilson", EmailAddress = "chris.w@test.com", PhoneNumber = "91212121", Gender = "Male" } // Unassigned
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
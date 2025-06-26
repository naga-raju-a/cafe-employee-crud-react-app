using CafeEmployeeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeEmployeeApi.Data
{
    /// <summary>
    /// The Entity Framework database context for the application.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cafe> Cafes { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the one-to-many relationship between Cafe and Employee.
            modelBuilder.Entity<Cafe>()
                .HasMany(c => c.Employees)       // A Cafe has many Employees
                .WithOne(e => e.Cafe)            // An Employee has one Cafe
                .HasForeignKey(e => e.CafeId)    // The foreign key is CafeId
                                                 // CRITICAL: When a cafe is deleted, all its employees are also deleted.
                                                 // This enforces the business rule directly in the database.
                .OnDelete(DeleteBehavior.Cascade);

            // Configure a unique constraint on the Employee's EmailAddress.
            // This prevents duplicate employees based on email.
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.EmailAddress)
                .IsUnique();
        }
    }
}

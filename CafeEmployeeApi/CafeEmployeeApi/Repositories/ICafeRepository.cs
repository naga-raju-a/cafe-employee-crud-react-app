using CafeEmployeeApi.Models;

namespace CafeEmployeeApi.Repositories
{
    /// <summary>
    /// Interface for data access operations related to Cafe entities.
    /// </summary>
    public interface ICafeRepository
    {
        /// <summary>
        /// Retrieves all cafes, optionally filtered by location.
        /// </summary>
        /// <param name="location">The location to filter cafes by. If null, all cafes are returned.</param>
        /// <returns>A collection of Cafe entities.</returns>
        Task<IEnumerable<Cafe>> GetAllAsync(string? location);

        /// <summary>
        /// Retrieves a single cafe by its unique ID.
        /// </summary>
        /// <param name="id">The GUID of the cafe.</param>
        /// <returns>The Cafe entity or null if not found.</returns>
        Task<Cafe?> GetByIdAsync(Guid id);

        /// <summary>
        /// Adds a new cafe to the database.
        /// </summary>
        /// <param name="cafe">The Cafe entity to add.</param>
        /// <returns>The added Cafe entity.</returns>
        Task<Cafe> AddAsync(Cafe cafe);

        /// <summary>
        /// Marks a cafe entity as modified.
        /// </summary>
        /// <param name="cafe">The Cafe entity to update.</param>
        void Update(Cafe cafe);

        /// <summary>
        /// Marks a cafe entity for deletion.
        /// </summary>
        /// <param name="cafe">The Cafe entity to delete.</param>
        void Delete(Cafe cafe);

        /// <summary>
        /// Saves all changes made in the context to the database.
        /// </summary>
        /// <returns>True if any records were changed, otherwise false.</returns>
        Task<bool> SaveChangesAsync();
    }
}

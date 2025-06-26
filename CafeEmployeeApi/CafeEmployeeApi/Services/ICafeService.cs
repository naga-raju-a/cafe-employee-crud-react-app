using CafeEmployeeApi.DTOs;

namespace CafeEmployeeApi.Services
{
    /// <summary>
    /// Interface for business logic operations related to cafes.
    /// </summary>
    public interface ICafeService
    {
        /// <summary>
        /// Gets a list of cafes, optionally filtered by location, and maps them to CafeDto.
        /// </summary>
        /// <param name="location">The location to filter by.</param>
        /// <returns>A collection of CafeDto.</returns>
        Task<IEnumerable<CafeDto>> GetCafesAsync(string? location);

        /// <summary>
        /// Creates a new cafe from a DTO.
        /// </summary>
        /// <param name="cafeDto">The DTO containing the data for the new cafe.</param>
        /// <returns>The created cafe as a CafeDto.</returns>
        Task<(CafeDto? CafeDto, string? Error)> CreateCafeAsync(CreateOrUpdateCafeDto cafeDto);

        /// <summary>
        /// Updates an existing cafe.
        /// </summary>
        /// <param name="id">The ID of the cafe to update.</param>
        /// <param name="cafeDto">The DTO containing the updated data.</param>
        /// <returns>The updated cafe as a CafeDto, or null if the cafe was not found.</returns>
        Task<(CafeDto? CafeDto, string? Error)> UpdateCafeAsync(Guid id, CreateOrUpdateCafeDto cafeDto);

        /// <summary>
        /// Deletes a cafe by its ID.
        /// </summary>
        /// <param name="id">The ID of the cafe to delete.</param>
        /// <returns>True if deletion was successful, otherwise false.</returns>
        Task<bool> DeleteCafeAsync(Guid id);
    }
}

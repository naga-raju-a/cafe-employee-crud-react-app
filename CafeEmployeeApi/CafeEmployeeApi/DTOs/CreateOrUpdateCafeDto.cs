using System.ComponentModel.DataAnnotations;

namespace CafeEmployeeApi.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating or updating a cafe.
    /// </summary>
    /// <param name="Name">The name of the cafe.</param>
    /// <param name="Description">A short description of the cafe.</param>
    /// <param name="Logo">The URL to the cafe's logo.</param>
    /// <param name="Location">The physical location of the cafe.</param>
    public record CreateOrUpdateCafeDto(
        [Required] string Name,
        [Required] string Description,
        string? Logo,
        [Required] string Location
    );
}

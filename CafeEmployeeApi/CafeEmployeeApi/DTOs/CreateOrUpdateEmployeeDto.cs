using System.ComponentModel.DataAnnotations;

namespace CafeEmployeeApi.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating or updating an employee.
    /// </summary>
    /// <param name="Name">The name of the employee.</param>
    /// <param name="EmailAddress">The email address of the employee.</param>
    /// <param name="PhoneNumber">The phone number of the employee.</param>
    /// <param name="Gender">The gender of the employee.</param>
    /// <param name="CafeId">The unique ID of the cafe to assign the employee to. Null for unassigned.</param>
    public record CreateOrUpdateEmployeeDto(
        [Required] string Name,
        [Required][EmailAddress] string EmailAddress,
        [Required][RegularExpression(@"^[89]\d{7}$")] string PhoneNumber,
        [Required] string Gender,
        Guid? CafeId,
        DateTime? EmploymentDate
    );
}

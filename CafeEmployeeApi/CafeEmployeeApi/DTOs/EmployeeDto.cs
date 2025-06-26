namespace CafeEmployeeApi.DTOs
{
    /// <summary>
    /// Data Transfer Object for returning employee information.
    /// </summary>
    /// <param name="Id">The unique ID of the employee (UIXXXXXXX).</param>
    /// <param name="Name">The name of the employee.</param>
    /// <param name="EmailAddress">The email address of the employee.</param>
    /// <param name="PhoneNumber">The phone number of the employee.</param>
    /// <param name="DaysWorked">The calculated number of days the employee has worked at the cafe.</param>
    /// <param name="Cafe">The name of the cafe the employee works at (null if unassigned).</param>
    public record EmployeeDto(
        string Id,
        string Name,
        string EmailAddress,
        string PhoneNumber,
        string Gender, 
        DateTime? EmploymentDate,
        int DaysWorked,
        string? CafeName,
        Guid? CafeId
    );
}

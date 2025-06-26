namespace CafeEmployeeApi.DTOs
{
    /// <summary>
    /// Data Transfer Object for returning cafe information.
    /// </summary>
    /// <param name="Id">The unique ID of the cafe.</param>
    /// <param name="Name">The name of the cafe.</param>
    /// <param name="Description">A short description of the cafe.</param>
    /// <param name="Logo">The URL to the cafe's logo.</param>
    /// <param name="Location">The physical location of the cafe.</param>
    /// <param name="Employees">The total number of employees working at the cafe.</param>
    public record CafeDto(
        Guid Id,
        string Name,
        string Description,
        string? Logo,
        string Location,
        int EmployeesCount
    );
}

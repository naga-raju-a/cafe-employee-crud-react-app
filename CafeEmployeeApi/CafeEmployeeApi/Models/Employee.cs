using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CafeEmployeeApi.Models
{
    /// <summary>
    /// Represents an Employee entity in the database.
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// The unique employee identifier, in the format 'UIXXXXXXX'.
        /// This is the primary key.
        /// </summary>
        [Key]
        [StringLength(10)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // We generate this ID in our service layer.
        public string Id { get; set; }

        /// <summary>
        /// The required name of the employee.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// The required and unique email address of the employee.
        /// </summary>
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        /// <summary>
        /// The required phone number of the employee.
        /// Must start with 8 or 9 and have exactly 8 digits.
        /// </summary>
        [Required]
        [RegularExpression(@"^[89]\d{7}$", ErrorMessage = "Phone number must start with 8 or 9 and have 8 digits.")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// The gender of the employee (e.g., "Male" or "Female").
        /// </summary>
        [Required]
        public string Gender { get; set; }

        /// <summary>
        /// The date the employee started working at their assigned cafe.
        /// Null if the employee is not assigned to a cafe.
        /// </summary>
        public DateTime? EmploymentDate { get; set; }

        /// <summary>
        /// The foreign key linking to the Cafe this employee works at.
        /// Nullable to allow for unassigned employees.
        /// </summary>
        public Guid? CafeId { get; set; }

        /// <summary>
        /// Navigation property to the Cafe entity.
        /// </summary>
        [ForeignKey("CafeId")]
        public Cafe? Cafe { get; set; }
    }
}

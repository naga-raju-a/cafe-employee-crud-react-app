using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CafeEmployeeApi.Models
{
    /// <summary>
    /// Represents a Cafe entity in the database.
    /// </summary>
    public class Cafe
    {
        /// <summary>
        /// The unique identifier for the cafe (UUID).
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// The required name of the cafe.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// A required short description of the cafe.
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        /// <summary>
        /// An optional URL or path to the cafe's logo.
        /// </summary>
        public string? Logo { get; set; }

        /// <summary>
        /// The required physical location of the cafe.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        /// <summary>
        /// Navigation property representing the collection of employees who work at this cafe.
        /// </summary>
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}

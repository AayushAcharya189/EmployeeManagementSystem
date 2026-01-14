using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public required string FullName { get; set; }

        [Required]
        public required string Department { get; set; }

        [Required]
        public required string Position { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Salary { get; set; }

        public bool IsDeleted { get; set; } = false;

        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }

}

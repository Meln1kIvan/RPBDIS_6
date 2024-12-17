using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPBDIS_6.Models
{
    [Table("Employees")]
    public class Employee
    {
        [Key]
        [Column("EmployeeId")]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(50)]
        public string Position { get; set; }

        // Навигационные свойства
        public ICollection<MaintenanceSchedule> MaintenanceSchedules { get; set; } = new List<MaintenanceSchedule>();
        public ICollection<CompletedWork> CompletedWorks { get; set; } = new List<CompletedWork>();
    }
}

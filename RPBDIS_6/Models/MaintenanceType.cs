using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPBDIS_6.Models
{
    [Table("MaintenanceTypes")]
    public class MaintenanceType
    {
        [Key]
        [Column("MaintenanceTypeId")]
        public int MaintenanceTypeId { get; set; }

        [Required]
        [StringLength(150)]
        public string Description { get; set; }

        // Навигационные свойства
        public ICollection<MaintenanceSchedule> MaintenanceSchedules { get; set; } = new List<MaintenanceSchedule>();
        public ICollection<CompletedWork> CompletedWorks { get; set; } = new List<CompletedWork>();
    }
}

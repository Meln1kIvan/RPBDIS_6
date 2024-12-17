using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPBDIS_6.Models
{
    [Table("MaintenanceSchedules")]
    public class MaintenanceSchedule
    {
        [Key]
        [Column("ScheduleId")]
        public int ScheduleId { get; set; }

        [ForeignKey("Equipment")]
        [Column("EquipmentId")]
        public int EquipmentId { get; set; }

        [ForeignKey("MaintenanceType")]
        [Column("MaintenanceTypeId")]
        public int MaintenanceTypeId { get; set; }

        [ForeignKey("Employee")]
        [Column("ResponsibleEmployeeId")]
        public int ResponsibleEmployeeId { get; set; }

        [Column("ScheduledDate")]
        public DateOnly ScheduledDate { get; set; }

        [Column(TypeName = "money")]
        public decimal EstimatedCost { get; set; }

        // Навигационные свойства
        public Equipment Equipment { get; set; }
        public MaintenanceType MaintenanceType { get; set; }
        public Employee ResponsibleEmployee { get; set; }
    }
}

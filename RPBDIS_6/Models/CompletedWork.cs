using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPBDIS_6.Models
{
    [Table("CompletedWorks")]
    public class CompletedWork
    {
        [Key]
        [Column("CompletedMaintenanceId")]
        public int CompletedMaintenanceId { get; set; }

        [ForeignKey("Equipment")]
        [Column("EquipmentId")]
        public int EquipmentId { get; set; }

        [ForeignKey("MaintenanceType")]
        [Column("MaintenanceTypeId")]
        public int MaintenanceTypeId { get; set; }

        [ForeignKey("Employee")]
        [Column("ResponsibleEmployeeId")]
        public int ResponsibleEmployeeId { get; set; }

        [Column("CompletionDate")]
        public DateOnly CompletionDate { get; set; }

        [Column(TypeName = "money")]
        public decimal ActualCost { get; set; }

        // Навигационные свойства
        public Equipment Equipment { get; set; }
        public MaintenanceType MaintenanceType { get; set; }
        public Employee ResponsibleEmployee { get; set; }
    }
}

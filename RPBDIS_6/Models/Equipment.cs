using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPBDIS_6.Models
{
    [Table("Equipments")]
    public class Equipment
    {
        [Key]
        [Column("EquipmentId")]
        public int EquipmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string InventoryNumber { get; set; }

        [Column("StartDate")]
        public DateOnly StartDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        // Навигационные свойства
        public ICollection<MaintenanceSchedule> MaintenanceSchedules { get; set; } = new List<MaintenanceSchedule>();
        public ICollection<CompletedWork> CompletedWorks { get; set; } = new List<CompletedWork>();
    }
}

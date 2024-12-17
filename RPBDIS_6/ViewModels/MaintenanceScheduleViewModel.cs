namespace RPBDIS_6.ViewModels
{
    public class MaintenanceScheduleViewModel
    {
        public int ScheduleId { get; set; }
        public DateOnly ScheduledDate { get; set; }
        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public int MaintenanceTypeId { get; set; }
        public string MaintenanceTypeDescription { get; set; }
        public int ResponsibleEmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public decimal EstimatedCost { get; set; }
    }
}

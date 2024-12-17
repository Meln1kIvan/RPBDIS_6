using Microsoft.EntityFrameworkCore;
using RPBDIS_6.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RPBDIS_6.Data
{
    public class MonitoringContext : DbContext
    {
        public MonitoringContext(DbContextOptions<MonitoringContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<MaintenanceSchedule> MaintenanceSchedules { get; set; }
        public DbSet<MaintenanceType> MaintenanceTypes { get; set; }
        public DbSet<CompletedWork> CompletedWorks { get; set; }
    }
}

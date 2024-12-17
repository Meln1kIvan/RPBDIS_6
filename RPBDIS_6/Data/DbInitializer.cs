using System;
using System.Linq;
using RPBDIS_6.Models;

namespace RPBDIS_6.Data
{
    public static class DbInitializer
    {
        public static void Initialize(MonitoringContext db)
        {
            db.Database.EnsureCreated();

            // Проверка, есть ли уже записи в базе данных
            if (db.Employees.Any() || db.Equipments.Any() || db.MaintenanceSchedules.Any()
                || db.MaintenanceTypes.Any() || db.CompletedWorks.Any())
            {
                return; // База данных уже инициализирована
            }

            Random rand = new Random();

            // Вставка данных для MaintenanceTypes (типы обслуживания)
            string[] maintenanceTypes = { "Плановое ТО", "Аварийный ремонт", "Замена деталей", "Диагностика" };
            foreach (var type in maintenanceTypes)
            {
                db.MaintenanceTypes.Add(new MaintenanceType
                {
                    Description = type
                });
            }
            db.SaveChanges();

            // Вставка данных для Equipments (оборудование)
            for (int i = 1; i <= 50; i++) // Добавим 50 записей
            {
                db.Equipments.Add(new Equipment
                {
                    Name = "Оборудование " + i,
                    InventoryNumber = "INV" + i.ToString("D3"),
                    StartDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-rand.Next(1, 10))), // Дата начала эксплуатации
                    Location = "Цех №" + rand.Next(1, 5)
                });
            }
            db.SaveChanges();

            // Вставка данных для Employees (сотрудники)
            string[] positions = { "Инженер", "Механик", "Техник", "Оператор" };
            for (int i = 1; i <= 20; i++) // Добавим 20 сотрудников
            {
                db.Employees.Add(new Employee
                {
                    FullName = "Сотрудник " + i,
                    Position = positions[rand.Next(positions.Length)]
                });
            }
            db.SaveChanges();

            // Вставка данных для MaintenanceSchedules (запланированные работы)
            for (int i = 1; i <= 30; i++) // Добавим 30 записей
            {
                db.MaintenanceSchedules.Add(new MaintenanceSchedule
                {
                    EquipmentId = rand.Next(1, 51), // Случайный ID оборудования
                    MaintenanceTypeId = rand.Next(1, maintenanceTypes.Length + 1), // Случайный тип обслуживания
                    ScheduledDate = DateOnly.FromDateTime(DateTime.Now.AddDays(rand.Next(1, 30))), // Запланированная дата
                    ResponsibleEmployeeId = rand.Next(1, 21), // Случайный ID сотрудника
                    EstimatedCost = rand.Next(5000, 20000) // Случайная стоимость от 5000 до 20000
                });
            }
            db.SaveChanges();

            // Вставка данных для CompletedWorks (выполненные работы)
            for (int i = 1; i <= 30; i++) // Добавим 30 записей
            {
                db.CompletedWorks.Add(new CompletedWork
                {
                    EquipmentId = rand.Next(1, 51), // Случайный ID оборудования
                    MaintenanceTypeId = rand.Next(1, maintenanceTypes.Length + 1), // Случайный тип обслуживания
                    CompletionDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-rand.Next(1, 30))), // Дата завершения
                    ResponsibleEmployeeId = rand.Next(1, 21), // Случайный ID сотрудника
                    ActualCost = rand.Next(5000, 25000) // Фактическая стоимость от 5000 до 25000
                });
            }
            db.SaveChanges();
        }
    }
}

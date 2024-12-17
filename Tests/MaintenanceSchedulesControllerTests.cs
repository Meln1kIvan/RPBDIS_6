using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPBDIS_6.Controllers;
using RPBDIS_6.Data;
using RPBDIS_6.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests
{
    public class MaintenanceSchedulesControllerTests
    {
        private MonitoringContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<MonitoringContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString()) // Уникальное имя БД
                .Options;

            var context = new MonitoringContext(options);
            context.Database.EnsureCreated(); // Создаем заново для каждого теста

            // Добавляем тестовые данные
            if (!context.Equipments.Any())
            {
                context.Equipments.AddRange(new List<Equipment>
                {
                    new Equipment { EquipmentId = 1, Name = "Станок 1", InventoryNumber = "INV123", Location = "Цех 1" },
                    new Equipment { EquipmentId = 2, Name = "Конвейер", InventoryNumber = "INV456", Location = "Цех 2" }
                });

                context.Employees.AddRange(new List<Employee>
                {
                    new Employee { EmployeeId = 1, FullName = "Иван Иванов", Position = "Инженер" },
                    new Employee { EmployeeId = 2, FullName = "Мария Смирнова", Position = "Техник" }
                });

                context.MaintenanceTypes.AddRange(new List<MaintenanceType>
                {
                    new MaintenanceType { MaintenanceTypeId = 1, Description = "Замена деталей" },
                    new MaintenanceType { MaintenanceTypeId = 2, Description = "Плановое обслуживание" }
                });

                context.MaintenanceSchedules.Add(new MaintenanceSchedule
                {
                    ScheduleId = 1,
                    EquipmentId = 1,
                    MaintenanceTypeId = 1,
                    ResponsibleEmployeeId = 1,
                    ScheduledDate = new System.DateOnly(2024, 6, 1),
                    EstimatedCost = 5000
                });

                context.SaveChanges();
            }

            return context;
        }

        [Fact]
        public void Get_ReturnsAllSchedules()
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new MaintenanceSchedulesController(context);

            // Act
            var result = controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var schedules = Assert.IsType<List<RPBDIS_6.ViewModels.MaintenanceScheduleViewModel>>(okResult.Value);
            Assert.Single(schedules);
        }

        [Fact]
        public void GetById_ReturnsSchedule_WhenIdIsValid()
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new MaintenanceSchedulesController(context);

            // Act
            var result = controller.Get(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var schedule = Assert.IsType<RPBDIS_6.ViewModels.MaintenanceScheduleViewModel>(okResult.Value);
            Assert.Equal(1, schedule.ScheduleId);
        }

        [Fact]
        public void GetById_ReturnsNotFound_WhenIdIsInvalid()
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new MaintenanceSchedulesController(context);

            // Act
            var result = controller.Get(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Post_AddsNewSchedule()
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new MaintenanceSchedulesController(context);

            var newSchedule = new MaintenanceSchedule
            {
                ScheduleId = 2,
                EquipmentId = 2,
                MaintenanceTypeId = 2,
                ResponsibleEmployeeId = 2,
                ScheduledDate = new System.DateOnly(2024, 7, 1),
                EstimatedCost = 6000
            };

            // Act
            var result = controller.Post(newSchedule);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var addedSchedule = Assert.IsType<MaintenanceSchedule>(createdAtActionResult.Value);
            Assert.Equal(2, addedSchedule.ScheduleId);

            Assert.Equal(2, context.MaintenanceSchedules.Count());
        }

        [Fact]
        public void Put_UpdatesExistingSchedule()
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new MaintenanceSchedulesController(context);

            var updatedSchedule = new MaintenanceSchedule
            {
                ScheduleId = 1,
                EquipmentId = 2, // Изменяем оборудование
                MaintenanceTypeId = 2, // Изменяем тип обслуживания
                ResponsibleEmployeeId = 2,
                ScheduledDate = new System.DateOnly(2024, 8, 1),
                EstimatedCost = 7000
            };

            // Act
            var result = controller.Put(1, updatedSchedule);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var updated = Assert.IsType<MaintenanceSchedule>(okResult.Value);

            Assert.Equal(2, updated.EquipmentId);
            Assert.Equal(7000, updated.EstimatedCost);
        }

        [Fact]
        public void Put_ReturnsNotFound_WhenScheduleDoesNotExist()
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new MaintenanceSchedulesController(context);

            var nonExistingSchedule = new MaintenanceSchedule
            {
                ScheduleId = 999,
                EquipmentId = 2,
                MaintenanceTypeId = 2,
                ResponsibleEmployeeId = 2,
                ScheduledDate = new System.DateOnly(2024, 8, 1),
                EstimatedCost = 7000
            };

            // Act
            var result = controller.Put(999, nonExistingSchedule);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_RemovesSchedule()
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new MaintenanceSchedulesController(context);

            // Act
            var result = controller.Delete(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var deletedSchedule = Assert.IsType<MaintenanceSchedule>(okResult.Value);
            Assert.Equal(1, deletedSchedule.ScheduleId);

            Assert.Empty(context.MaintenanceSchedules);
        }

        [Fact]
        public void Delete_ReturnsNotFound_WhenScheduleDoesNotExist()
        {
            // Arrange
            var context = GetInMemoryContext();
            var controller = new MaintenanceSchedulesController(context);

            // Act
            var result = controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}

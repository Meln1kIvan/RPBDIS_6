using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RPBDIS_6.Data;
using RPBDIS_6.Models;
using RPBDIS_6.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace RPBDIS_6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceSchedulesController : Controller
    {
        private readonly MonitoringContext _context;

        public MaintenanceSchedulesController(MonitoringContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получение списка всех запланированных работ
        /// </summary>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<IEnumerable<MaintenanceScheduleViewModel>> Get()
        {
            var schedules = _context.MaintenanceSchedules
                .Include(ms => ms.Equipment)
                .Include(ms => ms.MaintenanceType)
                .Include(ms => ms.ResponsibleEmployee)
                .Select(ms => new MaintenanceScheduleViewModel
                {
                    ScheduleId = ms.ScheduleId,
                    ScheduledDate = ms.ScheduledDate,
                    EquipmentId = ms.EquipmentId,
                    EquipmentName = ms.Equipment.Name,
                    MaintenanceTypeId = ms.MaintenanceTypeId,
                    MaintenanceTypeDescription = ms.MaintenanceType.Description,
                    ResponsibleEmployeeId = ms.ResponsibleEmployeeId,
                    EmployeeName = ms.ResponsibleEmployee.FullName,
                    EstimatedCost = ms.EstimatedCost
                })
                .Take(500)
                .ToList();

            return Ok(schedules);
        }

        /// <summary>
        /// Получение отфильтрованного списка запланированных работ
        /// </summary>
        [HttpGet("FilteredSchedules")]
        [Produces("application/json")]
        public ActionResult<IEnumerable<MaintenanceScheduleViewModel>> GetFilteredSchedules(int employeeId, int equipmentId)
        {
            var query = _context.MaintenanceSchedules
                .Include(ms => ms.Equipment)
                .Include(ms => ms.MaintenanceType)
                .Include(ms => ms.ResponsibleEmployee)
                .AsQueryable();

            if (employeeId > 0)
                query = query.Where(ms => ms.ResponsibleEmployeeId == employeeId);

            if (equipmentId > 0)
                query = query.Where(ms => ms.EquipmentId == equipmentId);

            var filteredSchedules = query.Select(ms => new MaintenanceScheduleViewModel
            {
                ScheduleId = ms.ScheduleId,
                ScheduledDate = ms.ScheduledDate,
                EquipmentId = ms.EquipmentId,
                EquipmentName = ms.Equipment.Name,
                MaintenanceTypeId = ms.MaintenanceTypeId,
                MaintenanceTypeDescription = ms.MaintenanceType.Description,
                ResponsibleEmployeeId = ms.ResponsibleEmployeeId,
                EmployeeName = ms.ResponsibleEmployee.FullName,
                EstimatedCost = ms.EstimatedCost
            })
            .OrderBy(ms => ms.ScheduleId)
            .Take(500)
            .ToList();

            return Ok(filteredSchedules);
        }

        /// <summary>
        /// Получение данных одного расписания
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var schedule = _context.MaintenanceSchedules
                .Include(ms => ms.Equipment)
                .Include(ms => ms.MaintenanceType)
                .Include(ms => ms.ResponsibleEmployee)
                .Select(ms => new MaintenanceScheduleViewModel
                {
                    ScheduleId = ms.ScheduleId,
                    ScheduledDate = ms.ScheduledDate,
                    EquipmentId = ms.EquipmentId,
                    EquipmentName = ms.Equipment.Name,
                    MaintenanceTypeId = ms.MaintenanceTypeId,
                    MaintenanceTypeDescription = ms.MaintenanceType.Description,
                    ResponsibleEmployeeId = ms.ResponsibleEmployeeId,
                    EmployeeName = ms.ResponsibleEmployee.FullName,
                    EstimatedCost = ms.EstimatedCost
                })
                .FirstOrDefault(ms => ms.ScheduleId == id);

            if (schedule == null)
                return NotFound();

            return Ok(schedule);
        }


        /// <summary>
        /// Добавление нового расписания
        /// </summary>
        [HttpPost]
        public IActionResult Post([FromBody] MaintenanceSchedule schedule)
        {
            if (schedule == null)
                return BadRequest("Данные не были переданы.");

            // Проверка корректности данных
            if (schedule.EquipmentId == 0 ||
                schedule.MaintenanceTypeId == 0 ||
                schedule.ResponsibleEmployeeId == 0 ||
                schedule.ScheduledDate == default ||
                schedule.EstimatedCost <= 0)
            {
                return BadRequest("Заполните все поля корректно.");
            }

            // Добавляем запись в базу данных
            _context.MaintenanceSchedules.Add(schedule);
            _context.SaveChanges();

            return CreatedAtAction(nameof(Get), new { id = schedule.ScheduleId }, schedule);
        }

        /// <summary>
        /// Обновление данных расписания
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] MaintenanceSchedule schedule)
        {
            if (schedule == null || id != schedule.ScheduleId)
                return BadRequest();

            var existingSchedule = _context.MaintenanceSchedules
                .FirstOrDefault(ms => ms.ScheduleId == id);

            if (existingSchedule == null)
                return NotFound();

            // Обновляем поля
            existingSchedule.EquipmentId = schedule.EquipmentId;
            existingSchedule.MaintenanceTypeId = schedule.MaintenanceTypeId;
            existingSchedule.ResponsibleEmployeeId = schedule.ResponsibleEmployeeId;
            existingSchedule.ScheduledDate = schedule.ScheduledDate;
            existingSchedule.EstimatedCost = schedule.EstimatedCost;

            _context.SaveChanges();

            return Ok(existingSchedule);
        }


        /// <summary>
        /// Удаление расписания по ID
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var schedule = _context.MaintenanceSchedules.FirstOrDefault(ms => ms.ScheduleId == id);

            if (schedule == null)
                return NotFound();

            _context.MaintenanceSchedules.Remove(schedule);
            _context.SaveChanges();

            return Ok(schedule);
        }

        /// <summary>
        /// Список оборудования
        /// </summary>
        [HttpGet("equipments")]
        [Produces("application/json")]
        public ActionResult<IEnumerable<Equipment>> GetEquipments()
        {
            return Ok(_context.Equipments.ToList());
        }

        /// <summary>
        /// Список сотрудников
        /// </summary>
        [HttpGet("employees")]
        [Produces("application/json")]
        public ActionResult<IEnumerable<Employee>> GetEmployees()
        {
            return Ok(_context.Employees.ToList());
        }

        [HttpGet("maintenanceTypes")]
        [Produces("application/json")]
        public ActionResult<IEnumerable<MaintenanceType>> GetMaintenanceTypes()
        {
            return Ok(_context.MaintenanceTypes.ToList());
        }
    }
}

using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using RPBDIS_6.Models;
using RPBDIS_6.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Reflection;

namespace RPBDIS_6
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Логирование действий с использованием Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .WriteTo.File("MonitoringWebApiLog-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            // Добавляем Serilog
            builder.Host.UseSerilog();

            // Подключение строки соединения к базе данных
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<MonitoringContext>(options =>
                options.UseSqlServer(connectionString));

            // Добавляем Swagger для документирования API
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Monitoring API",
                    Description = "API для работы с системой мониторинга технического обслуживания",
                    Contact = new OpenApiContact
                    {
                        Name = "Vanya",
                        Email = string.Empty,
                        Url = new Uri("https://github.com/Meln1kIvan")
                    }
                });

                // Путь к XML-документации для Swagger
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Добавляем CORS-политику (если необходимо)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Конфигурация HTTP-запросов
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Monitoring API v1");
                });
            }

            // Обработка исключений
            app.UseDeveloperExceptionPage();

            app.UseRouting();

            // Подключение статических файлов
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Использование CORS
            app.UseCors("AllowAll");

            // Аутентификация и авторизация
            app.UseAuthentication();
            app.UseAuthorization();

            // Инициализация базы данных
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                try
                {
                    var context = serviceProvider.GetRequiredService<MonitoringContext>();
                    DbInitializer.Initialize(context); // Если у вас есть класс для инициализации
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "An error occurred during database initialization");
                }
            }

            // Маршруты контроллеров
            app.MapControllers();

            // Запуск приложения
            app.Run();
        }
    }
}

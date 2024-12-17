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
            // ����������� �������� � �������������� Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .WriteTo.File("MonitoringWebApiLog-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            // ��������� Serilog
            builder.Host.UseSerilog();

            // ����������� ������ ���������� � ���� ������
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<MonitoringContext>(options =>
                options.UseSqlServer(connectionString));

            // ��������� Swagger ��� ���������������� API
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Monitoring API",
                    Description = "API ��� ������ � �������� ����������� ������������ ������������",
                    Contact = new OpenApiContact
                    {
                        Name = "Vanya",
                        Email = string.Empty,
                        Url = new Uri("https://github.com/Meln1kIvan")
                    }
                });

                // ���� � XML-������������ ��� Swagger
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // ��������� CORS-�������� (���� ����������)
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

            // ������������ HTTP-��������
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Monitoring API v1");
                });
            }

            // ��������� ����������
            app.UseDeveloperExceptionPage();

            app.UseRouting();

            // ����������� ����������� ������
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // ������������� CORS
            app.UseCors("AllowAll");

            // �������������� � �����������
            app.UseAuthentication();
            app.UseAuthorization();

            // ������������� ���� ������
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                try
                {
                    var context = serviceProvider.GetRequiredService<MonitoringContext>();
                    DbInitializer.Initialize(context); // ���� � ��� ���� ����� ��� �������������
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "An error occurred during database initialization");
                }
            }

            // �������� ������������
            app.MapControllers();

            // ������ ����������
            app.Run();
        }
    }
}

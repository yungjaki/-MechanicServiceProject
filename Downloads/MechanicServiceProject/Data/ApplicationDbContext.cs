using MechanicService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MechanicService.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<ServiceAppointment> ServiceAppointments { get; set; }
    public DbSet<ServiceType> ServiceTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Many-to-many: Vehicle ↔ ServiceType
        builder.Entity<Vehicle>()
            .HasMany(v => v.ServiceTypes)
            .WithMany(s => s.Vehicles)
            .UsingEntity(j => j.ToTable("VehicleServiceTypes"));

        builder.Entity<ServiceAppointment>()
            .Property(a => a.TotalPrice)
            .HasPrecision(18, 2);

        // 1-to-many: Customer → ServiceAppointments
        builder.Entity<ServiceAppointment>()
            .HasOne(a => a.Customer)
            .WithMany(c => c.ServiceAppointments)
            .HasForeignKey(a => a.CustomerId);

        // 1-to-many: Vehicle → ServiceAppointments
        builder.Entity<ServiceAppointment>()
            .HasOne(a => a.Vehicle)
            .WithMany(v => v.ServiceAppointments)
            .HasForeignKey(a => a.VehicleId);

        // Seed ServiceTypes
        builder.Entity<ServiceType>().HasData(
            new ServiceType { Id = 1, Name = "Смяна на масло", Description = "Пълна смяна на двигателното масло и маслен филтър" },
            new ServiceType { Id = 2, Name = "Смяна на спирачки", Description = "Проверка и смяна на спирачни накладки и дискове" },
            new ServiceType { Id = 3, Name = "Компютърна диагностика", Description = "Пълна диагностика на електронните системи" },
            new ServiceType { Id = 4, Name = "Смяна на гуми", Description = "Смяна и балансиране на гуми" }
        );

        // Seed Vehicles
        builder.Entity<Vehicle>().HasData(
            new Vehicle { Id = 1, LicensePlate = "CB1234AB", Make = "Toyota", Model = "Corolla", YearOfManufacture = 2018, IsAvailable = true },
            new Vehicle { Id = 2, LicensePlate = "CB5678CD", Make = "BMW", Model = "320i", YearOfManufacture = 2020, IsAvailable = true },
            new Vehicle { Id = 3, LicensePlate = "CB9012EF", Make = "Ford", Model = "Focus", YearOfManufacture = 2015, IsAvailable = false }
        );

        // Seed many-to-many join table
        builder.Entity("ServiceTypeVehicle").HasData(
            new { ServiceTypesId = 1, VehiclesId = 1 },
            new { ServiceTypesId = 2, VehiclesId = 1 },
            new { ServiceTypesId = 1, VehiclesId = 2 },
            new { ServiceTypesId = 2, VehiclesId = 2 },
            new { ServiceTypesId = 3, VehiclesId = 2 },
            new { ServiceTypesId = 1, VehiclesId = 3 },
            new { ServiceTypesId = 2, VehiclesId = 3 },
            new { ServiceTypesId = 3, VehiclesId = 3 },
            new { ServiceTypesId = 4, VehiclesId = 3 }
        );

        // Seed Customers
        builder.Entity<Customer>().HasData(
            new Customer { Id = 1, FullName = "Иван Петров", Email = "ivan@example.com", Phone = "+359888111222" },
            new Customer { Id = 2, FullName = "Мария Иванова", Email = "maria@example.com", Phone = "+359888333444" }
        );

        // Seed ServiceAppointments
        builder.Entity<ServiceAppointment>().HasData(
            new ServiceAppointment { Id = 1, AppointmentDate = new DateTime(2026, 6, 1), CompletionDate = new DateTime(2026, 6, 1, 14, 0, 0), TotalPrice = 120m, CustomerId = 1, VehicleId = 1 },
            new ServiceAppointment { Id = 2, AppointmentDate = new DateTime(2026, 7, 10), CompletionDate = new DateTime(2026, 7, 10, 17, 0, 0), TotalPrice = 350m, CustomerId = 2, VehicleId = 2 },
            new ServiceAppointment { Id = 3, AppointmentDate = new DateTime(2026, 8, 1), CompletionDate = new DateTime(2026, 8, 2, 12, 0, 0), TotalPrice = 200m, CustomerId = 1, VehicleId = 3 }
        );
    }
}

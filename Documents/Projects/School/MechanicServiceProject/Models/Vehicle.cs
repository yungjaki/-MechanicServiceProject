using System.ComponentModel.DataAnnotations;

namespace MechanicService.Models;

public class Vehicle
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string LicensePlate { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Make { get; set; } = string.Empty; // Toyota, BMW, Ford

    [Required]
    [MaxLength(50)]
    public string Model { get; set; } = string.Empty;

    public int YearOfManufacture { get; set; }

    public bool IsAvailable { get; set; } = true;

    // 1 Vehicle → Many ServiceAppointments
    public ICollection<ServiceAppointment> ServiceAppointments { get; set; } = new List<ServiceAppointment>();

    // Many Vehicles ↔ Many ServiceTypes
    public ICollection<ServiceType> ServiceTypes { get; set; } = new List<ServiceType>();
}

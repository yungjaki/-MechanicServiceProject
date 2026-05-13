using System.ComponentModel.DataAnnotations;

namespace MechanicService.Models;

public class ServiceType
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(250)]
    public string Description { get; set; } = string.Empty;

    // Many ServiceTypes ↔ Many Vehicles
    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}

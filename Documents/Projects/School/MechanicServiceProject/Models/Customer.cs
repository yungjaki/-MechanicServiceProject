using System.ComponentModel.DataAnnotations;

namespace MechanicService.Models;

public class Customer
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    // 1 Customer → Many ServiceAppointments
    public ICollection<ServiceAppointment> ServiceAppointments { get; set; } = new List<ServiceAppointment>();
}

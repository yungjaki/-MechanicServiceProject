using System.ComponentModel.DataAnnotations;

namespace MechanicService.Models;

public class ServiceAppointment
{
    public int Id { get; set; }

    public DateTime AppointmentDate { get; set; }

    public DateTime CompletionDate { get; set; }

    public decimal TotalPrice { get; set; }

    // Foreign keys
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
}

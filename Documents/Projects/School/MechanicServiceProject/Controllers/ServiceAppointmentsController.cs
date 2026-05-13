using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MechanicService.Data;
using MechanicService.Models;

namespace MechanicService.Controllers;

public class ServiceAppointmentsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ServiceAppointmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: ServiceAppointments
    public async Task<IActionResult> Index()
    {
        var appointments = await _context.ServiceAppointments
            .Include(a => a.Customer)
            .Include(a => a.Vehicle)
            .ToListAsync();
        return View(appointments);
    }

    // GET: ServiceAppointments/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var appointment = await _context.ServiceAppointments
            .Include(a => a.Customer)
            .Include(a => a.Vehicle)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (appointment == null)
            return NotFound();

        return View(appointment);
    }

    // GET: ServiceAppointments/Create
    public IActionResult Create()
    {
        ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FullName");
        ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "LicensePlate");
        return View();
    }

    // POST: ServiceAppointments/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("AppointmentDate,CompletionDate,TotalPrice,CustomerId,VehicleId")] ServiceAppointment appointment)
    {
        if (ModelState.IsValid)
        {
            _context.Add(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FullName", appointment.CustomerId);
        ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "LicensePlate", appointment.VehicleId);
        return View(appointment);
    }

    // GET: ServiceAppointments/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var appointment = await _context.ServiceAppointments.FindAsync(id);
        if (appointment == null)
            return NotFound();

        ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FullName", appointment.CustomerId);
        ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "LicensePlate", appointment.VehicleId);
        return View(appointment);
    }

    // POST: ServiceAppointments/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,AppointmentDate,CompletionDate,TotalPrice,CustomerId,VehicleId")] ServiceAppointment appointment)
    {
        if (id != appointment.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(appointment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceAppointmentExists(appointment.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FullName", appointment.CustomerId);
        ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "LicensePlate", appointment.VehicleId);
        return View(appointment);
    }

    // GET: ServiceAppointments/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var appointment = await _context.ServiceAppointments
            .Include(a => a.Customer)
            .Include(a => a.Vehicle)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (appointment == null)
            return NotFound();

        return View(appointment);
    }

    // POST: ServiceAppointments/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var appointment = await _context.ServiceAppointments.FindAsync(id);
        if (appointment != null)
        {
            _context.ServiceAppointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool ServiceAppointmentExists(int id)
    {
        return _context.ServiceAppointments.Any(e => e.Id == id);
    }
}

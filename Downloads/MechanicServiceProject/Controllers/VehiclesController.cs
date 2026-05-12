using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MechanicService.Data;
using MechanicService.Models;

namespace MechanicService.Controllers;

public class VehiclesController : Controller
{
    private readonly ApplicationDbContext _context;

    public VehiclesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Vehicles
    public async Task<IActionResult> Index()
    {
        return View(await _context.Vehicles.Include(v => v.ServiceTypes).ToListAsync());
    }

    // GET: Vehicles/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var vehicle = await _context.Vehicles
            .Include(v => v.ServiceTypes)
            .Include(v => v.ServiceAppointments)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (vehicle == null)
            return NotFound();

        return View(vehicle);
    }

    // GET: Vehicles/Create
    public IActionResult Create()
    {
        ViewData["ServiceTypes"] = _context.ServiceTypes.ToList();
        return View();
    }

    // POST: Vehicles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("LicensePlate,Make,Model,YearOfManufacture,IsAvailable")] Vehicle vehicle, int[] serviceTypeIds)
    {
        if (ModelState.IsValid)
        {
            _context.Add(vehicle);
            await _context.SaveChangesAsync();

            if (serviceTypeIds.Length > 0)
            {
                foreach (var serviceTypeId in serviceTypeIds)
                {
                    var serviceType = await _context.ServiceTypes.FindAsync(serviceTypeId);
                    if (serviceType != null)
                        vehicle.ServiceTypes.Add(serviceType);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["ServiceTypes"] = _context.ServiceTypes.ToList();
        return View(vehicle);
    }

    // GET: Vehicles/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var vehicle = await _context.Vehicles.Include(v => v.ServiceTypes).FirstOrDefaultAsync(v => v.Id == id);
        if (vehicle == null)
            return NotFound();

        ViewData["ServiceTypes"] = _context.ServiceTypes.ToList();
        ViewData["SelectedServiceTypes"] = vehicle.ServiceTypes.Select(s => s.Id).ToList();
        return View(vehicle);
    }

    // POST: Vehicles/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,LicensePlate,Make,Model,YearOfManufacture,IsAvailable")] Vehicle vehicle, int[] serviceTypeIds)
    {
        if (id != vehicle.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var existing = await _context.Vehicles.Include(v => v.ServiceTypes).FirstOrDefaultAsync(v => v.Id == id);
                if (existing == null)
                    return NotFound();

                existing.LicensePlate = vehicle.LicensePlate;
                existing.Make = vehicle.Make;
                existing.Model = vehicle.Model;
                existing.YearOfManufacture = vehicle.YearOfManufacture;
                existing.IsAvailable = vehicle.IsAvailable;

                existing.ServiceTypes.Clear();
                foreach (var serviceTypeId in serviceTypeIds)
                {
                    var serviceType = await _context.ServiceTypes.FindAsync(serviceTypeId);
                    if (serviceType != null)
                        existing.ServiceTypes.Add(serviceType);
                }

                _context.Update(existing);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleExists(vehicle.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["ServiceTypes"] = _context.ServiceTypes.ToList();
        ViewData["SelectedServiceTypes"] = serviceTypeIds.ToList();
        return View(vehicle);
    }

    // GET: Vehicles/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var vehicle = await _context.Vehicles
            .Include(v => v.ServiceTypes)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (vehicle == null)
            return NotFound();

        return View(vehicle);
    }

    // POST: Vehicles/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle != null)
        {
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool VehicleExists(int id)
    {
        return _context.Vehicles.Any(e => e.Id == id);
    }
}

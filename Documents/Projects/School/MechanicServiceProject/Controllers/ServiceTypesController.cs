using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MechanicService.Data;
using MechanicService.Models;

namespace MechanicService.Controllers;

public class ServiceTypesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ServiceTypesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: ServiceTypes
    public async Task<IActionResult> Index()
    {
        return View(await _context.ServiceTypes.ToListAsync());
    }

    // GET: ServiceTypes/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var serviceType = await _context.ServiceTypes
            .Include(s => s.Vehicles)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (serviceType == null)
            return NotFound();

        return View(serviceType);
    }

    // GET: ServiceTypes/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: ServiceTypes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description")] ServiceType serviceType)
    {
        if (ModelState.IsValid)
        {
            _context.Add(serviceType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(serviceType);
    }

    // GET: ServiceTypes/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var serviceType = await _context.ServiceTypes.FindAsync(id);
        if (serviceType == null)
            return NotFound();

        return View(serviceType);
    }

    // POST: ServiceTypes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] ServiceType serviceType)
    {
        if (id != serviceType.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(serviceType);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceTypeExists(serviceType.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(serviceType);
    }

    // GET: ServiceTypes/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(m => m.Id == id);
        if (serviceType == null)
            return NotFound();

        return View(serviceType);
    }

    // POST: ServiceTypes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var serviceType = await _context.ServiceTypes.FindAsync(id);
        if (serviceType != null)
        {
            _context.ServiceTypes.Remove(serviceType);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool ServiceTypeExists(int id)
    {
        return _context.ServiceTypes.Any(e => e.Id == id);
    }
}

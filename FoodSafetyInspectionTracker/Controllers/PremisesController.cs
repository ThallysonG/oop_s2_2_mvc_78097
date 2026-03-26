using FoodSafetyInspectionTracker.Data;
using FoodSafetyInspectionTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyInspectionTracker.Controllers
{
    [Authorize]
    public class PremisesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PremisesController> _logger;

        public PremisesController(ApplicationDbContext context, ILogger<PremisesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,Inspector,Viewer")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Premises.OrderBy(p => p.Name).ToListAsync());
        }

        [Authorize(Roles = "Admin,Inspector,Viewer")]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.Premises
                .Include(p => p.Inspections)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (item == null) return NotFound();
            return View(item);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Premises model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model while creating premises by user {UserName}", User.Identity?.Name);
                return View(model);
            }

            _context.Premises.Add(model);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Premises created by {UserName}. PremisesId={PremisesId}, Name={Name}, Town={Town}",
                User.Identity?.Name ?? "Anonymous", model.Id, model.Name, model.Town);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.Premises.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Premises model)
        {
            if (!ModelState.IsValid) return View(model);

            _context.Premises.Update(model);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Premises updated by {UserName}. PremisesId={PremisesId}, Name={Name}, Town={Town}, RiskRating={RiskRating}",
                User.Identity?.Name ?? "Anonymous", model.Id, model.Name, model.Town, model.RiskRating);

            return RedirectToAction(nameof(Index));
        }
    }
}
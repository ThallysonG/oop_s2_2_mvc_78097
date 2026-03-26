using FoodSafetyInspectionTracker.Data;
using FoodSafetyInspectionTracker.Models;
using FoodSafetyInspectionTracker.Services.Interfaces;
using FoodSafetyInspectionTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyInspectionTracker.Controllers
{
    [Authorize]
    public class InspectionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IInspectionService _inspectionService;
        private readonly ILogger<InspectionsController> _logger;

        public InspectionsController(
            ApplicationDbContext context,
            IInspectionService inspectionService,
            ILogger<InspectionsController> logger)
        {
            _context = context;
            _inspectionService = inspectionService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,Inspector,Viewer")]
        public async Task<IActionResult> Index()
        {
            var data = await _context.Inspections
                .Include(i => i.Premises)
                .OrderByDescending(i => i.InspectionDate)
                .ToListAsync();

            return View(data);
        }

        [Authorize(Roles = "Admin,Inspector,Viewer")]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.Inspections
                .Include(i => i.Premises)
                .Include(i => i.FollowUps)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null) return NotFound();
            return View(item);
        }

        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Premises = await _context.Premises.OrderBy(p => p.Name).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Create(InspectionCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model while creating inspection by user {UserName}", User.Identity?.Name);
                ViewBag.Premises = await _context.Premises.OrderBy(p => p.Name).ToListAsync();
                return View(vm);
            }

            try
            {
                await _inspectionService.CreateInspectionAsync(vm, User.Identity?.Name);
                TempData["Success"] = "Inspection created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating inspection");
                ModelState.AddModelError(string.Empty, "Unable to create inspection.");
                ViewBag.Premises = await _context.Premises.OrderBy(p => p.Name).ToListAsync();
                return View(vm);
            }
        }
    }
}
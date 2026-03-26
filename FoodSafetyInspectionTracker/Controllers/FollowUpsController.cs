using FoodSafetyInspectionTracker.Data;
using FoodSafetyInspectionTracker.Services.Interfaces;
using FoodSafetyInspectionTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyInspectionTracker.Controllers
{
    [Authorize]
    public class FollowUpsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFollowUpService _followUpService;
        private readonly ILogger<FollowUpsController> _logger;

        public FollowUpsController(
            ApplicationDbContext context,
            IFollowUpService followUpService,
            ILogger<FollowUpsController> logger)
        {
            _context = context;
            _followUpService = followUpService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,Inspector,Viewer")]
        public async Task<IActionResult> Index()
        {
            var items = await _context.FollowUps
                .Include(f => f.Inspection)
                .ThenInclude(i => i!.Premises)
                .OrderBy(f => f.DueDate)
                .ToListAsync();

            return View(items);
        }

        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Inspections = await _context.Inspections
                .Include(i => i.Premises)
                .OrderByDescending(i => i.InspectionDate)
                .ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Create(FollowUpCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model while creating follow-up by user {UserName}", User.Identity?.Name);
                ViewBag.Inspections = await _context.Inspections.Include(i => i.Premises).ToListAsync();
                return View(vm);
            }

            try
            {
                await _followUpService.CreateFollowUpAsync(vm, User.Identity?.Name);
                TempData["Success"] = "Follow-up created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business validation failed while creating follow-up");
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Inspections = await _context.Inspections.Include(i => i.Premises).ToListAsync();
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating follow-up");
                ModelState.AddModelError(string.Empty, "Unable to create follow-up.");
                ViewBag.Inspections = await _context.Inspections.Include(i => i.Premises).ToListAsync();
                return View(vm);
            }
        }

        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Close(int id)
        {
            var item = await _context.FollowUps
                .Include(f => f.Inspection)
                .ThenInclude(i => i!.Premises)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> CloseConfirmed(int id, DateTime closedDate)
        {
            try
            {
                await _followUpService.CloseFollowUpAsync(id, closedDate, User.Identity?.Name);
                TempData["Success"] = "Follow-up closed successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing follow-up {FollowUpId}", id);
                return RedirectToAction("Index", "Error");
            }
        }
    }
}
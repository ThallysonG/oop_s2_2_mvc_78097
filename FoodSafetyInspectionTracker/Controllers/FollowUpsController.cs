using FoodSafetyInspectionTracker.Models;
using FoodSafetyInspectionTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FoodSafetyInspectionTracker.Controllers
{
    [Authorize]
    public class FollowUpsController : Controller
    {
        private readonly FollowUpService _followUpService;

        public FollowUpsController(FollowUpService followUpService)
        {
            _followUpService = followUpService;
        }

        [Authorize(Roles = "Admin,Inspector,Viewer")]
        public async Task<IActionResult> Index()
        {
            var items = await _followUpService.GetAllAsync();
            return View(items);
        }

        [Authorize(Roles = "Admin,Inspector,Viewer")]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _followUpService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Create()
        {
            await LoadInspectionsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Create(FollowUp followUp)
        {
            if (!ModelState.IsValid)
            {
                await LoadInspectionsAsync();
                return View(followUp);
            }

            await _followUpService.CreateAsync(followUp);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _followUpService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            await LoadInspectionsAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Edit(int id, FollowUp followUp)
        {
            if (id != followUp.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadInspectionsAsync();
                return View(followUp);
            }

            await _followUpService.UpdateAsync(followUp);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Close(int id)
        {
            await _followUpService.CloseAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _followUpService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _followUpService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadInspectionsAsync()
        {
            var inspections = await _followUpService.GetInspectionListAsync();

            ViewBag.InspectionId = new SelectList(
                inspections.Select(i => new
                {
                    i.Id,
                    Description = $"{i.Id} - {i.Premise!.Name} - {i.InspectionDate:yyyy-MM-dd}"
                }),
                "Id",
                "Description");
        }
    }
}
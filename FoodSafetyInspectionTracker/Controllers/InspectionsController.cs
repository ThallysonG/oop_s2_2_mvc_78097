using FoodSafetyInspectionTracker.Models;
using FoodSafetyInspectionTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FoodSafetyInspectionTracker.Controllers
{
    [Authorize]
    public class InspectionsController : Controller
    {
        private readonly InspectionService _inspectionService;

        public InspectionsController(InspectionService inspectionService)
        {
            _inspectionService = inspectionService;
        }

        [Authorize(Roles = "Admin,Inspector,Viewer")]
        public async Task<IActionResult> Index()
        {
            var items = await _inspectionService.GetAllAsync();
            return View(items);
        }

        [Authorize(Roles = "Admin,Inspector,Viewer")]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _inspectionService.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Create()
        {
            await LoadPremisesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Create(Inspection inspection)
        {
            if (!ModelState.IsValid)
            {
                await LoadPremisesAsync();
                return View(inspection);
            }

            await _inspectionService.CreateAsync(inspection);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _inspectionService.GetByIdAsync(id);
            if (item == null) return NotFound();

            await LoadPremisesAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Inspector")]
        public async Task<IActionResult> Edit(int id, Inspection inspection)
        {
            if (id != inspection.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadPremisesAsync();
                return View(inspection);
            }

            await _inspectionService.UpdateAsync(inspection);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _inspectionService.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _inspectionService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadPremisesAsync()
        {
            var premises = await _inspectionService.GetPremisesListAsync();
            ViewBag.PremiseId = new SelectList(premises, "Id", "Name");
        }
    }
}
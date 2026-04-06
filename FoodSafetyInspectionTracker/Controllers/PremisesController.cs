using FoodSafetyInspectionTracker.Models;
using FoodSafetyInspectionTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FoodSafetyInspectionTracker.Controllers
{
    [Authorize]
    public class PremisesController : Controller
    {
        private readonly PremisesService _premisesService;

        public PremisesController(PremisesService premisesService)
        {
            _premisesService = premisesService;
        }

        [Authorize(Roles = "Admin,Inspector,Viewer")]
        public async Task<IActionResult> Index()
        {
            var items = await _premisesService.GetAllAsync();
            return View(items);
        }

        [Authorize(Roles = "Admin,Inspector,Viewer")]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _premisesService.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Premise premises)
        {
            if (!ModelState.IsValid) return View(premises);

            await _premisesService.CreateAsync(premises);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _premisesService.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Premise premises)
        {
            if (id != premises.Id) return NotFound();
            if (!ModelState.IsValid) return View(premises);

            await _premisesService.UpdateAsync(premises);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _premisesService.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _premisesService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
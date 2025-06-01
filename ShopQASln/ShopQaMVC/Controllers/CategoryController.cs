using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using ShopQaMVC.Services;
using System.Threading.Tasks;

namespace ShopQaMVC.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CategoryApiService _apiService;

        public CategoryController(CategoryApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _apiService.GetAllAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            bool result = await _apiService.CreateAsync(category);
            if (result)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "Không thể tạo category mới.");
            return View(category);
        }

        
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _apiService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        
        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            bool result = await _apiService.UpdateAsync(category);
            if (result)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "Không thể cập nhật category.");
            return View(category);
        }

        
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            bool result = await _apiService.DeleteAsync(id);
            if (!result)
            {
               
                TempData["Error"] = "Không thể xóa category này.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

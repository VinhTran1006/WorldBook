using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WorldBook.Models;
using WorldBook.Services.Interfaces;

namespace WorldBook.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index()
        {
            var categorys = await _categoryService.GetCategoriesAsync();
            return View("/Views/AdminViews/ManageCategory/Index.cshtml", categorys);
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View("/Views/AdminViews/ManageCategory/Details.cshtml", category);
        }

        public IActionResult Add()
        {
            return PartialView("~/Views/AdminViews/ManageCategory/Add.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Add(Category cate)
        {
            var existingCategory = await _categoryService.GetByNameAsync(cate.CategoryName);
            if (existingCategory != null)
            {
                ModelState.AddModelError("CategoryName", "Category with the same name already exists.");
                return Json(new { error = true });
            }
            var newCategory = new Models.Category
            {
                CategoryName = cate.CategoryName,
                CategoryDescription = cate.CategoryDescription,
                IsActive = true
            };
            await _categoryService.AddAsync(newCategory);
            return Json(new { success = true });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return PartialView("~/Views/AdminViews/ManageCategory/Edit.cshtml", category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category cate)
        {
            var existingCategory = await _categoryService.GetByNameAsync(cate.CategoryName);
            if (existingCategory != null && existingCategory.CategoryId != cate.CategoryId)
            {
                ModelState.AddModelError("CategoryName", "Category with the same name already exists.");
                return Json(new { error = true });
            }
            else await _categoryService.UpdateCategoryAsync(cate);
            return Json(new { success = true });
        }


        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return PartialView("~/Views/AdminViews/ManageCategory/Delete.cshtml", category);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return Json(new { success = true });
        }
    }
    }

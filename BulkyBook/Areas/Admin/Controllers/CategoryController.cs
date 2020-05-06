using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int productPage =1)
        {
            CategoryVM categoryVM = new CategoryVM()
            {
                Categories = await _unitOfWork.Category.GetAllAsync()
            };

            return View(categoryVM);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Category category = new Category();

            if(id == null)
            {
                return View(category);
            }
            category = await _unitOfWork.Category.GetAsync(id.GetValueOrDefault());
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if(category.Id == 0)
                {
                    await _unitOfWork.Category.AddAsync(category);
                }
                else
                {
                    _unitOfWork.Category.Update(category);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        //#region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _unitOfWork.Category.GetAllAsync();
            return Json(new { data = categories });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Category.GetAsync(id);
            if(category == null)
            {
                return Json(new { success = false, message = "Error While Deliting" });
            }

            await _unitOfWork.Category.RemoveAsync(category);
            _unitOfWork.Save();
            return Json(new { success = false, message = "Delete Successful" });
        }

    }
}
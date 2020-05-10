using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            IEnumerable<Category> CatList = await _unitOfWork.Category.GetAllAsync();
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = CatList.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            if(id == null)
            {
                return View(productVM);
            }

            productVM.Product = await _unitOfWork.Product.GetAsync(id.GetValueOrDefault());
            if(productVM.Product == null)
            {
                return NotFound();
            }

            return View(productVM);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Upsert(Product Product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if(Product.Id == 0)
        //        {
        //            await _unitOfWork.Product.AddAsync(Product);
        //        }
        //        else
        //        {
        //            _unitOfWork.Product.Update(Product);
        //        }
        //        _unitOfWork.Save();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(Product);
        //}

        //#region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _unitOfWork.Product.GetAllAsync(includeProperties: "Category,CoverType");
            return Json(new { data = products });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var Product = await _unitOfWork.Product.GetAsync(id);
            if(Product == null)
            {
                return Json(new { success = false, message = "Error While Deliting" });
            }

            await _unitOfWork.Product.RemoveAsync(Product);
            _unitOfWork.Save();
            return Json(new { success = false, message = "Delete Successful" });
        }

    }
}
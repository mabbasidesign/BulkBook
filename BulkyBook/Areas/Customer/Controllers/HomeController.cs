using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.DataAccess.Repository.IRepository;
using System.Runtime.InteropServices.WindowsRuntime;

namespace BulkyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            //IEnumerable<Product> productList = await _unitOfWork.Product.GetAllAsync(includeProperties: "Category,CoverType");

            return View(productList);
        }

        public IActionResult Details(int id)
        {
            var productFromDB = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id, includeProperties: "Category,CoverType");

            ShoppingCart shoppingCat = new ShoppingCart()
            {
                Product = productFromDB,
                ProductId = productFromDB.Id
            };

            return View(shoppingCat);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

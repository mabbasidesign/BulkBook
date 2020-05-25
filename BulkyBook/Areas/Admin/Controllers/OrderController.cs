using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderDetailsVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }



        #region API CALLS

        [HttpGet]
        public IActionResult GetOrderList(string status)
        {
            IEnumerable<OrderHeader> orderHeaderList;

            orderHeaderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");

            return Json(new { data = orderHeaderList });
        }

        #endregion

    }
}
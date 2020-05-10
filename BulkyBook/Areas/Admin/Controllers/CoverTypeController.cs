using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();

            if(id == null)
            {
                return View(coverType);
            }

            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);
            coverType = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter);
            //coverType = _unitOfWork.CoverType.Get(id.GetValueOrDefault());
            if (coverType == null)
            {
                return NotFound();
            }

            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Name", coverType.Name);

                if (coverType.Id == 0)
                {
                    _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Create, parameter);
                    //_unitOfWork.CoverType.Add(coverType);
                }
                else
                {
                    parameter.Add("@Id", coverType.Id);
                    _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Update, parameter);
                    //_unitOfWork.CoverType.Update(coverType);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(coverType);
        }

        //#region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            //var companies = _unitOfWork.CoverType.GetAll();
            var companies = _unitOfWork.SP_Call.List<CoverType>(SD.Proc_CoverType_GetAll, null);
            return Json(new { data = companies });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);
            var coverType = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter);
            //var CoverType = _unitOfWork.CoverType.Get(id);
            if (coverType == null)
            {
                return Json(new { success = false, message = "Error While Deliting" });
            }

            _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Delete, parameter);
            //_unitOfWork.CoverType.Remove(coverType);
            _unitOfWork.Save();
            return Json(new { success = false, message = "Delete Successful" });
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }


    #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _db.ApplicationUsers.Include(u => u.Company).ToList();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }

            return Json(new { data = userList });
        }

       [HttpPost]
       public IActionResult LockUnlock([FromBody] string id)
        {
            var userInDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if(userInDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            if(userInDb.LockoutEnd != null && userInDb.LockoutEnd > DateTime.Now )
            {
                //user is currently locked, we will unlock them
                userInDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                userInDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return Json(new { succes = true, message = "Operation Successful." });
        }

    #endregion

    }
}
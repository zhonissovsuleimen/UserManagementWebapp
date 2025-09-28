using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UserManagementWebapp.Database;
using UserManagementWebapp.Models;

namespace UserManagementWebapp.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UsersDbContext _context;

        public RegisterController(UsersDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Name,Email,Password")] User user)
        {
            user.Status = Data.Status.Unverified;
            //TODO: salt + hashing
            if (ModelState.IsValid)
            {
                _context.Add(user);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    ModelState.AddModelError("Email", "Email is already in use.");
                    return View(user);
                }
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }
    }
}

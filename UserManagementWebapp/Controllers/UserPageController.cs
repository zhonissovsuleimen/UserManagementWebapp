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
    public class UserPageController : Controller
    {
        private readonly UsersDbContext _context;

        public UserPageController(UsersDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            string guidStr = User.Claims.FirstOrDefault(c => c.Type == "Guid")?.Value ?? "";

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Guid.ToString() == guidStr);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}

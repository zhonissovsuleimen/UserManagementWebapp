using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementWebapp.Data;
using UserManagementWebapp.Database;

namespace UserManagementWebapp.Controllers
{
    public class UsersController : Controller
    {
        private readonly UsersDbContext _context;
        public UsersController(UsersDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var users = await _context.Users
                    .OrderBy(u => u.LastLogin == null)
                    .ThenByDescending(u => u.LastLogin)
                    .ToListAsync(); 
                return View(users);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task BlockUser(string guid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Guid.ToString() == guid && u.Status == Status.Active);
            if (user != null)
            {
                user.isBlocked = true;
                await _context.SaveChangesAsync();
            }
            return;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task UnblockUser(string guid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Guid.ToString() == guid && u.Status == Status.Active);
            if (user != null)
            {
                user.isBlocked = false;
                await _context.SaveChangesAsync();
            }
            return;
        }
    }
}

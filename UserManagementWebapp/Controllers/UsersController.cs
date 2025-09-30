using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                return View(await _context.Users.ToListAsync());
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }
}

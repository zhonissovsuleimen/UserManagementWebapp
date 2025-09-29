using Microsoft.AspNetCore.Mvc;
using UserManagementWebapp.Database;
using UserManagementWebapp.Helpers;
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
            var salt = new Salt { User = user };
            user.PasswordHash = PasswordHasher.GenHashedPassword(user.Password, salt.SaltValue);
            if (ModelState.IsValid)
            {
                _context.Add(user);
                _context.Add(salt);

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

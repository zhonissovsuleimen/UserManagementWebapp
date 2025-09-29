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
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegisterViewModel regModel)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    Name = regModel.Name,
                    Email = regModel.Email,
                };

                Salt salt = new Salt { User = user };
                user.PasswordHash = PasswordHasher.GenHashedPassword(regModel.Password, salt.SaltValue);

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

            return View();
        }
    }
}

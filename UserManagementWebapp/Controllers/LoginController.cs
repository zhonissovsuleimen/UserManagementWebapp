using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UserManagementWebapp.Database;
using UserManagementWebapp.Helpers;
using UserManagementWebapp.Models;

namespace UserManagementWebapp.Controllers
{
    public class LoginController : Controller
    {
        private readonly UsersDbContext _context;
        public LoginController(UsersDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Edit", "UserPage");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginModel.Email);
                if (user != null)
                {
                    if(user.Status == Data.Status.Blocked)
                    {
                        ModelState.AddModelError("Email", "Account is blocked.");
                        return View(loginModel);
                    }

                    var salt = await _context.Salts.FirstOrDefaultAsync(s => s.User.Id == user.Id);
                    if (salt != null)
                    {
                        var hashedPassword = Hasher.GetHashedValue(loginModel.Password, salt.SaltValue);
                        if (user.PasswordHash.SequenceEqual(hashedPassword))
                        {
                            await CookiesHelper.PersistentLogin(HttpContext, user);
                            user.LastLogin = DateTime.UtcNow;
                            await _context.SaveChangesAsync();

                            return RedirectToAction("Index", "Users");
                        }
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(loginModel);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementWebapp.Database;
using UserManagementWebapp.Helpers;

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
            if (IsAllowed())
            {
                var users = await _context.Users
                    .OrderBy(u => u.LastLogin == null)
                    .ThenByDescending(u => u.LastLogin)
                    .ToListAsync(); 
                return View(users);
            }
            else
            {
                return RedirectToAction("Index", "UserPage");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockSelected(List<Guid> selectedGuids)
        {
            bool blockedYourself = false;
            if (IsAllowed())
            {
                foreach (Guid guid in selectedGuids)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Guid == guid);

                    if (user != null)
                    {
                        blockedYourself |= CookiesHelper.IsYourself(User, user);
                        user.isBlocked = true;
                    }
                }
                await _context.SaveChangesAsync();
            }

            if (blockedYourself)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnblockSelected(List<Guid> selectedGuids)
        {
            if (IsAllowed())
            {
                foreach (Guid guid in selectedGuids)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Guid == guid);
                    if (user != null)
                    {
                        user.isBlocked = false;
                    }
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSelected(List<Guid> selectedGuids)
        {
            bool deletedYourself = false;
            if (IsAllowed())
            {
                foreach (Guid guid in selectedGuids)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Guid == guid);

                    if (user != null)
                    {
                        deletedYourself |= CookiesHelper.IsYourself(User, user);
                        _context.Users.Remove(user);
                    }
                }
                await _context.SaveChangesAsync();
            }

            if (deletedYourself)
            {
                return RedirectToAction("Logout", "Login");
            } else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSelectedUnverified(List<Guid> selectedGuids)
        {
            bool deletedYourself = false;
            if (IsAllowed())
            {
                foreach (Guid guid in selectedGuids)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Guid == guid && !u.isVerified);

                    if (user != null)
                    {
                        deletedYourself |= CookiesHelper.IsYourself(User, user);
                        _context.Users.Remove(user);
                    }
                }
                await _context.SaveChangesAsync();
            }

            if (deletedYourself)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public bool IsAllowed()
        {
            string guid_claim = User.Claims.FirstOrDefault(c => c.Type == "Guid")?.Value ?? "";
            return User.Identity != null && User.Identity.IsAuthenticated && _context.Users.Any(u => u.Guid.ToString() == guid_claim && !u.isBlocked);
        }
    }
}

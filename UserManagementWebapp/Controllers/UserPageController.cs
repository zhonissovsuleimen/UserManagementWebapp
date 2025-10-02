using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagementWebapp.Data;
using UserManagementWebapp.Database;
using UserManagementWebapp.Helpers;
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
                return RedirectToAction("Index", "Login");
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendVerificationEmail()
        {
            string guid_claim = User.Claims.FirstOrDefault(c => c.Type == "Guid")?.Value ?? "";
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Guid.ToString() == guid_claim);
            if (user == null || user.isVerified)
            {
                return RedirectToAction("Index");
            }

            string token = EmailVerification.GenVerificationToken();

            Salt salt = new Salt { User = user, Purpose = SaltPurpose.EmailVerification };
            byte[] tokenHashed = Hasher.GetHashedValue(token, salt.SaltValue);
            EmailVerification ev = new() { User = user, TokenHash = tokenHashed };

            _context.Add(salt);
            _context.Add(ev);

            await _context.SaveChangesAsync();

            string link = Url.Action("Verify", "Register", new { token = token, guid = user.Guid }, Request.Scheme, Request.Host.ToString()) ?? "";

            await EmailSender.SendVerificationEmail(user.Name, user.Email, link);

            return RedirectToAction("Index");
        }
    }
}

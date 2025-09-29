using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementWebapp.Data;
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

                Salt salt = new Salt { User = user, Purpose = SaltPurpose.Password };
                user.PasswordHash = Hasher.GetHashedValue(regModel.Password, salt.SaltValue);

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

                await SendEmailVerification(user);

                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Verify(string token, string guid)
        {
            var notVerifiedResult = Content("Invalid token or guid");
            if (!Guid.TryParse(guid, out Guid guidParsed))
            {
                return notVerifiedResult;
            }

            if (await _context.Users.AnyAsync(u => u.Guid == guidParsed))
            {
                User user = await _context.Users.FirstAsync(u => u.Guid == guidParsed);
                if (await _context.EmailVerifications.AnyAsync(ev => ev.User.Id == user.Id))
                {
                    EmailVerification ev = await _context.EmailVerifications.FirstAsync(ev => ev.User.Id == user.Id);
                    if (ev.Used || ev.Expiration < DateTime.UtcNow)
                    {
                        return Content("Token expired or already used.");
                    }

                    Salt salt = await _context.Salts.FirstAsync(s => s.User.Id == user.Id && s.Purpose == SaltPurpose.EmailVerification);
                    byte[] tokenHashed = Hasher.GetHashedValue(token, salt.SaltValue);
                    if (ev.TokenHash.SequenceEqual(tokenHashed))
                    {
                        user.Status = Status.Active;
                        ev.Used = true;
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            return notVerifiedResult;
        }

        public async Task SendEmailVerification(User user)
        {
            string token = EmailVerification.GenVerificationToken();

            Salt salt = new Salt { User = user, Purpose = SaltPurpose.EmailVerification };
            byte[] tokenHashed = Hasher.GetHashedValue(token, salt.SaltValue);
            EmailVerification ev = new() { User = user, TokenHash = tokenHashed };

            _context.Add(salt);
            _context.Add(ev);

            await _context.SaveChangesAsync();

            string link = Url.Action("Verify", "Register", new { token = token, guid = user.Guid }, Request.Scheme, Request.Host.ToString()) ?? "";

            await EmailSender.SendVerificationEmail(user.Name, user.Email, link);
        }
    }
}

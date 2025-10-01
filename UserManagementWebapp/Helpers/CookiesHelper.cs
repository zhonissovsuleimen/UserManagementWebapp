using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using UserManagementWebapp.Models;

namespace UserManagementWebapp.Helpers
{
    public class CookiesHelper
    {
        public static async Task PersistentLogin(HttpContext httpContext, User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim("Guid", user.Guid.ToString()),
                //new Claim("Status", user.Status.ToString()),
                new Claim("isVerified", user.isVerified.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }

        public static async Task Logout(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public static bool IsYourself(ClaimsPrincipal identity, User user)
        {
            string guid_claim = identity.Claims.FirstOrDefault(c => c.Type == "Guid")?.Value ?? "";
            return user.Guid.ToString() == guid_claim;
        }
    }
}

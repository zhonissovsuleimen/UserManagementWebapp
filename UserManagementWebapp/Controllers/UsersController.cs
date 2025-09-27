using Microsoft.AspNetCore.Mvc;

namespace UserManagementWebapp.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

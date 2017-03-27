using Microsoft.AspNetCore.Mvc;

namespace TrollChat.Web.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }
    }
}
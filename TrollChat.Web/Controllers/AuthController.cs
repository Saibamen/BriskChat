using Microsoft.AspNetCore.Mvc;

namespace TrollChat.Web.Controllers
{
    public class AuthController : BaseController
    {
        public IActionResult Register()
        {
            return View();
        }
    }
}
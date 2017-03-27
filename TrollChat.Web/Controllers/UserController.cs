using Microsoft.AspNetCore.Mvc;

namespace TrollChat.Web.Controllers
{
    public class UserController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
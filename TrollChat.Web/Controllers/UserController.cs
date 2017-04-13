using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TrollChat.Web.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class UserController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateChannel()
        {
            return View();
        }
    }
}
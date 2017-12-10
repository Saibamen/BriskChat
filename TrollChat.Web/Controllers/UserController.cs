using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BriskChat.Web.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class UserController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
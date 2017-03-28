using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TrollChat.Web.Controllers
{
    [Authorize(Policy = "UserPolicy")]
    public class UserController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
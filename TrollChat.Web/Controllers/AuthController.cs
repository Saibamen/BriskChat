using Microsoft.AspNetCore.Mvc;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.Web.Models.Auth;

namespace TrollChat.Web.Controllers
{
    [Route("auth")]
    public class AuthController : BaseController
    {
        private readonly IAddNewUser addNewUser;

        public AuthController(IAddNewUser addNewUser)
        {
            this.addNewUser = addNewUser;
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterViewModel model)
        {
            var userModel = new User { Email = model.Email, Password = model.Password, Name = model.Name };
            var userAddAction = addNewUser.Invoke(userModel);

            if (userAddAction == 0)
            {
                // TODO: Alert.Danger: User Already exists
                return View();
            }

            // TODO: Alert: Confirmation email has been sent to your email address
            return RedirectToAction("Login", "Auth");
        }
    }
}
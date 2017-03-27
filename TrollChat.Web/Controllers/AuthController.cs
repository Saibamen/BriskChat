using Microsoft.AspNetCore.Mvc;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.Web.Models.Auth;
using TrollChat.BusinessLogic.Models;

namespace TrollChat.Web.Controllers
{
    [Route("auth")]
    public class AuthController : BaseController
    {
        private readonly IAuthorizeUser authorizeUser;
        private readonly IAddNewUser addNewUser;

        public AuthController(IAuthorizeUser authorizeUser, IAddNewUser addNewUser)
        {
            this.authorizeUser = authorizeUser;
            this.addNewUser = addNewUser;
        }
        
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
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

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost("login")]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var access = authorizeUser.Invoke(model.Email, model.Password);

            return View();
        }
    }
}
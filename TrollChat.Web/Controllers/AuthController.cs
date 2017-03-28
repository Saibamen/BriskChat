using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.Web.Models.Auth;

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

        [AllowAnonymous]
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost("register")]
        public IActionResult Register(RegisterViewModel model)
        {
            var userModel = new User { Email = model.Email, Password = model.Password, Name = model.Name };
            var userAddAction = addNewUser.Invoke(userModel);

            if (userAddAction == 0)
            {
                Alert.Danger("User already exists");
                return View();
            }

            // TODO: Alert: Confirmation email
            //Alert.Success("Confirmation email has been sent to your email address");

            // TODO: Authorize User (TC-3) and redirect to sth

            return RedirectToAction("Login", "Auth");
        }

        [AllowAnonymous]
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var access = authorizeUser.Invoke(model.Email, model.Password);

            if (!access) return View();

            //TODO: Create actual claims
            var claims = new List<Claim>
            {
                new Claim("Role", "User"),
            };

            var claimsIdentity = new ClaimsIdentity(claims, "role");
            var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.Authentication.SignInAsync("CookieMiddleware", claimsPrinciple);

            return RedirectToAction("Index", "User");
        }

        [ValidateAntiForgeryToken]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync("CookieMiddleware");
            return RedirectToAction("Login");
        }
    }
}
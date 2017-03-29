using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Helpers.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.Web.Models.Auth;

namespace TrollChat.Web.Controllers
{
    [Route("auth")]
    public class AuthController : BaseController
    {
        private readonly IAuthorizeUser authorizeUser;
        private readonly IAddNewUser addNewUser;
        private readonly IEmailService emailService;
        private readonly IConfirmUserEmail confirmUserEmail;

        public AuthController(IAuthorizeUser authorizeUser,
            IAddNewUser addNewUser,
            IEmailService emailService,
            IConfirmUserEmail confirmUserEmail)
        {
            this.authorizeUser = authorizeUser;
            this.addNewUser = addNewUser;
            this.emailService = emailService;
            this.confirmUserEmail = confirmUserEmail;
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

            if (userAddAction == null)
            {
                Alert.Danger("User already exists");
                return View();
            }

            var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { token = userAddAction.SecretToken }, Request.Scheme);
            var htmlSource = RenderViewToString("ConfirmEmail", "", callbackUrl);
            var result = PreMailer.Net.PreMailer.MoveCssInline(htmlSource);

            var message = emailService.CreateMessage(model.Email, "Confirm your account", result.Html);
            emailService.SendEmailAsync(message).ConfigureAwait(false);

            Alert.Success("Confirmation email has been sent to your email address");

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
                Alert.Warning();

                return View(model);
            }

            //TODO: Check for user email confirmed
            var access = authorizeUser.Invoke(model.Email, model.Password);

            if (!access)
            {
                ModelState.AddModelError("Email", "Invalid email or password");
                Alert.Warning();

                return View();
            }

            //TODO: Create actual claims
            var claims = new List<Claim>
            {
                new Claim("Role", "User"),
            };

            var claimsIdentity = new ClaimsIdentity(claims, "role");
            var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.Authentication.SignInAsync("Cookies", claimsPrinciple);

            Alert.Success("Logged in");

            return RedirectToAction("Index", "User");
        }

        [ValidateAntiForgeryToken]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");

            Alert.Success("Logged out");

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpGet("confirmemail")]
        public IActionResult ConfirmEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                Alert.Danger("Invalid token");
                return View("Error");
            }

            var confirmAction = confirmUserEmail.Invoke(token);

            if (confirmAction)
            {
                Alert.Success("Email confirmed");
                return RedirectToAction("Login", "Auth");
            }

            Alert.Danger("Invalid token");
            return View("Error");
        }
    }
}
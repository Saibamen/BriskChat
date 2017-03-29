using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Actions.UserToken.Interfaces;
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
        private readonly IGetUserByEmail getuserByEmail;
        private readonly IAddUserToken addUserToken;

        public AuthController(IAuthorizeUser authorizeUser,
            IAddNewUser addNewUser,
            IEmailService emailService,
            IConfirmUserEmail confirmUserEmail,
            IGetUserByEmail getUserByEmail,
            IAddUserToken addUserToken)
        {
            this.authorizeUser = authorizeUser;
            this.addNewUser = addNewUser;
            this.emailService = emailService;
            this.confirmUserEmail = confirmUserEmail;
            this.getuserByEmail = getUserByEmail;
            this.addUserToken = addUserToken;
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
            var userModel = new UserModel { Email = model.Email, Password = model.Password, Name = model.Name };
            var userAddAction = addNewUser.Invoke(userModel);

            if (userAddAction == null)
            {
                Alert.Danger("User already exists");
                return View();
            }

            var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { token = userAddAction.Tokens.FirstOrDefault().SecretToken }, Request.Scheme);
            var stringView = RenderViewToString("Email_EmailAccountConfirmation", "", callbackUrl);
            var viewWithInlineCss = PreMailer.Net.PreMailer.MoveCssInline(stringView);

            var message = emailService.CreateMessage(model.Email, "Confirm your account", viewWithInlineCss.Html);
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

        [AllowAnonymous]
        [HttpGet("resendconfirmationemail")]
        public IActionResult ResendConfirmationEmail()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("resendconfirmationemail")]
        public IActionResult ResendConfirmationEmail(ResendConfirmationEmailViewModel model)
        {
            var user = getuserByEmail.Invoke(model.Email);
            if (user == null)
            {
                Alert.Danger("Something went wrong");
                return View();
            }
            if (user.EmailConfirmedOn != null)
            {
                Alert.Danger("Email already confirmed");
                return View();
            }

            var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { token = user.Tokens.FirstOrDefault().SecretToken }, Request.Scheme);
            var stringView = RenderViewToString("Email_EmailAccountConfirmation", "", callbackUrl);
            var viewWithInlineCss = PreMailer.Net.PreMailer.MoveCssInline(stringView);

            var message = emailService.CreateMessage(model.Email, "Confirm your account", viewWithInlineCss.Html);
            emailService.SendEmailAsync(message).ConfigureAwait(false);

            Alert.Success("Check your inbox");
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpGet("resetpassword")]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("resetpassword")]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = getuserByEmail.Invoke(model.Email);

            if (user == null)
            {
                return View();
            }

            var token = addUserToken.Invoke(user.Id);
            var callbackUrl = Url.Action("ResetPasswordByToken", "Auth", new { token }, Request.Scheme);
            var stringView = RenderViewToString("Email_EmailAccountConfirmation", "", callbackUrl);
            var viewWithInlineCss = PreMailer.Net.PreMailer.MoveCssInline(stringView);

            var message = emailService.CreateMessage(model.Email, "Confirm your account", viewWithInlineCss.Html);
            emailService.SendEmailAsync(message).ConfigureAwait(false);

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet("resetpasswordbytoken")]
        public IActionResult ResetPasswordByToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                Alert.Danger("Invalid token");
                return View("Error");
            }

            //TODO: Write reset password action

            Alert.Danger("Invalid token");
            return View("Error");
        }
    }
}
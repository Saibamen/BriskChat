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
        private readonly IGetUserByToken getuserByToken;
        private readonly IEditUserPassword editUserPassword;
        private readonly IDeleteUserTokenyByTokenString deleteUserTokenByTokenString;

        public AuthController(IAuthorizeUser authorizeUser,
            IAddNewUser addNewUser,
            IEmailService emailService,
            IConfirmUserEmail confirmUserEmail,
            IGetUserByEmail getUserByEmail,
            IAddUserToken addUserToken,
            IGetUserByToken getUserByToken,
            IEditUserPassword editIUserPassword,
            IDeleteUserTokenyByTokenString deleteUserTokenyByTokenString)
        {
            this.authorizeUser = authorizeUser;
            this.addNewUser = addNewUser;
            this.emailService = emailService;
            this.confirmUserEmail = confirmUserEmail;
            this.getuserByEmail = getUserByEmail;
            this.addUserToken = addUserToken;
            this.getuserByToken = getUserByToken;
            this.editUserPassword = editIUserPassword;
            this.deleteUserTokenByTokenString = deleteUserTokenyByTokenString;
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
            var stringView = RenderViewToString("ConfirmEmail", "", callbackUrl);

            var message = emailService.CreateMessage(model.Email, "Confirm your account", stringView);
            emailService.SendEmailAsync(message).ConfigureAwait(false);

            Alert.Success("Confirmation email has been sent to your email address");

            return RedirectToAction("Login", "Auth");
        }

        [AllowAnonymous]
        [HttpGet("login")]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                Alert.Warning();
                ViewBag.ReturnUrl = returnUrl;

                return View(model);
            }

            //TODO: Check for user email confirmed
            var access = authorizeUser.Invoke(model.Email, model.Password);

            if (!access)
            {
                ModelState.AddModelError("Email", "Invalid email or password");
                Alert.Warning();
                ViewBag.ReturnUrl = returnUrl;

                return View(model);
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

            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Index", "User");
            }

            return Redirect(returnUrl);
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

            if (!confirmAction)
            {
                Alert.Danger("Couldn't finish this action");
                return RedirectToAction("Login", "Auth");
            }

            Alert.Success("Email confirmed");
            return RedirectToAction("Login", "Auth");
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
                return View(model);
            }
            if (user.EmailConfirmedOn != null)
            {
                Alert.Danger("Email already confirmed");
                return View(model);
            }

            var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { token = user.Tokens.FirstOrDefault().SecretToken }, Request.Scheme);
            var stringView = RenderViewToString("ConfirmEmail", "", callbackUrl);

            var message = emailService.CreateMessage(model.Email, "Confirm your account", stringView);
            emailService.SendEmailAsync(message).ConfigureAwait(false);

            Alert.Success("Check your inbox");
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpGet("resetpassword")]
        public IActionResult ResetPasswordInitiation()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("resetpassword")]
        public IActionResult ResetPasswordInitiation(ResetPasswordInitiationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Alert.Danger("Something went wrong");
                return View(model);
            }

            var user = getuserByEmail.Invoke(model.Email);

            if (user == null)
            {
                Alert.Danger("Something went wrong");
                return View();
            }

            var token = addUserToken.Invoke(user.Id);
            var callbackUrl = Url.Action("ResetPasswordByToken", "Auth", new { token }, Request.Scheme);
            var stringView = RenderViewToString("ConfirmEmail", "", callbackUrl);

            var message = emailService.CreateMessage(model.Email, "Confirm your account", stringView);
            emailService.SendEmailAsync(message).ConfigureAwait(false);

            Alert.Success("Email was sent to your Email account");
            return RedirectToAction("Login");
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

            var user = getuserByToken.Invoke(token);

            if (user == null)
            {
                Alert.Danger("You can't complete this action");
                return View("Error");
            }

            var model = new ResetPasswordNewPasswordViewModel()
            {
                Token = token,
            };

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost("resetpasswordbytoken")]
        public IActionResult ResetPasswordByToken(ResetPasswordNewPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Alert.Danger("Something went wrong");
                return View(model);
            }

            var user = getuserByToken.Invoke(model.Token);

            if (user == null)
            {
                Alert.Danger("You can't complete this action");
                return View("Login");
            }

            var result = editUserPassword.Invoke(user.Id, model.Password);

            if (!result)
            {
                Alert.Danger("Something went wrong");
                return View(model);
            }

            Alert.Success("Your password has been updated");
            return RedirectToAction("Login");
        }
    }
}
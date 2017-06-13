using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TrollChat.BusinessLogic.Actions.Domain.Interfaces;
using TrollChat.BusinessLogic.Actions.Email.Interfaces;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Actions.UserToken.Interfaces;
using TrollChat.BusinessLogic.Helpers.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.Web.Authorization;
using TrollChat.Web.Helpers;
using TrollChat.Web.Models.Auth;
using TrollChat.Web.Models.Common;

namespace TrollChat.Web.Controllers
{
    [Route("auth")]
    public class AuthController : BaseController
    {
        private readonly IAuthenticateUser authenticateUser;
        private readonly IAddNewUser addNewUser;
        private readonly IEmailService emailService;
        private readonly IConfirmUserEmailByToken confirmUserEmailByToken;
        private readonly IGetUserByEmail getuserByEmail;
        private readonly IAddUserTokenToUser addUserTokenToUser;
        private readonly IGetUserByToken getUserByToken;
        private readonly IEditUserPassword editUserPassword;
        private readonly IDeleteUserTokenyByTokenString deleteUserTokenByTokenString;
        private readonly IAddNewEmailMessage addNewEmailMessage;
        private readonly ICheckDomainExistsByName checkDomainExistsByName;

        public AuthController(IAuthenticateUser authenticateUser,
            IAddNewUser addNewUser,
            IEmailService emailService,
            IConfirmUserEmailByToken confirmUserEmailByToken,
            IGetUserByEmail getUserByEmail,
            IAddUserTokenToUser addUserTokenToUser,
            IGetUserByToken getUserByToken,
            IEditUserPassword editIUserPassword,
            IDeleteUserTokenyByTokenString deleteUserTokenyByTokenString,
            IAddNewEmailMessage addNewEmailMessage,
            ICheckDomainExistsByName checkDomainExistsByName
            )
        {
            this.authenticateUser = authenticateUser;
            this.addNewUser = addNewUser;
            this.emailService = emailService;
            this.confirmUserEmailByToken = confirmUserEmailByToken;
            this.getuserByEmail = getUserByEmail;
            this.addUserTokenToUser = addUserTokenToUser;
            this.getUserByToken = getUserByToken;
            this.editUserPassword = editIUserPassword;
            this.deleteUserTokenByTokenString = deleteUserTokenyByTokenString;
            this.addNewEmailMessage = addNewEmailMessage;
            this.checkDomainExistsByName = checkDomainExistsByName;
        }

        [AllowAnonymous]
        [HttpGet("signin")]
        public IActionResult ChooseDomain()
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost("signin")]
        public IActionResult ChooseDomain(ChooseDomainViewModel model)
        {
            if (!checkDomainExistsByName.Invoke(model.DomainName))
            {
                ModelState.AddModelError("Domain", "Domain not found");

                return View(model);
            }

            return RedirectToAction("Login", new { domainName = model.DomainName });
        }

        [AllowAnonymous]
        [HttpGet("register")]
        public IActionResult Register(string domainName)
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

            var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { token = userAddAction.Tokens.FirstOrDefault().SecretToken },
                Request.Scheme);
            var emailinfo = new EmailBodyHelper().GetRegisterEmailBodyModel(callbackUrl);
            var stringView = RenderViewToString<EmailBodyModel>("ConfirmEmail", emailinfo);
            var message = emailService.CreateMessage(model.Email, "Confirm your account", stringView);
            var mappedMessage = AutoMapper.Mapper.Map<EmailMessageModel>(message);
            addNewEmailMessage.Invoke(mappedMessage);

            Alert.Success("Confirmation email has been sent to your email address");

            return RedirectToAction("Login", "Auth");
        }

        [AllowAnonymous]
        [HttpGet("{domainName}/login")]
        public IActionResult Login(string returnUrl, string domainName)
        {
            if (string.IsNullOrEmpty(domainName))
            {
                Alert.Warning("Choose existing domain");
                return RedirectToAction("ChooseDomain", "Auth");
            }

            if (!checkDomainExistsByName.Invoke(domainName))
            {
                Alert.Warning("Choose existing domain");
                return RedirectToAction("ChooseDomain", "Auth");
            }

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.DomainName = domainName;

            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost("{domainName}/login")]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                Alert.Warning();
                ViewBag.ReturnUrl = returnUrl;

                return View(model);
            }

            var access = authenticateUser.Invoke(model.Email, model.Password, model.DomainName);

            if (access == null)
            {
                ModelState.AddModelError("Email", "Invalid email or password");
                Alert.Warning();
                ViewBag.ReturnUrl = returnUrl;

                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, access.Name),
                new Claim(ClaimTypes.Sid, access.Id.ToString()),
                new Claim(ClaimTypes.Role, Role.User),
                // TODO: MOVE TO CONST STRING
                new Claim("DomainName", access.Domain.Name),
                new Claim("DomainId", access.Domain.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Claims");
            var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.Authentication.SignInAsync("Cookies", claimsPrinciple);

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

            return RedirectToAction("Index", "Home");
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

            var confirmAction = confirmUserEmailByToken.Invoke(token);

            if (!confirmAction)
            {
                Alert.Danger("Couldn't finish this action");

                return RedirectToAction("Login", "Auth");
            }

            Alert.Success("Email confirmed");
            return RedirectToAction("Login", "Auth");
        }

        [AllowAnonymous]
        [HttpGet("{domainName}/resendconfirmationemail")]
        public IActionResult ResendConfirmationEmail(string domainName)
        {
            if (!checkDomainExistsByName.Invoke(domainName))
            {
                Alert.Warning("Choose existing domain");
                return RedirectToAction("ChooseDomain", "Auth");
            }

            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost("{domainName}/resendconfirmationemail")]
        public IActionResult ResendConfirmationEmail(ResendConfirmationEmailViewModel model)
        {
            var user = getuserByEmail.Invoke(model.Email, model.DomainName);

            if (user == null)
            {
                Alert.Danger("Something went wrong");

                return View(model);
            }

            if (user.EmailConfirmedOn != null)
            {
                Alert.Danger("Email already confirmed");

                return RedirectToAction("Login");
            }

            var token = addUserTokenToUser.Invoke(user.Id);

            var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { token }, Request.Scheme);
            var emailinfo = new EmailBodyHelper().GetRegisterEmailBodyModel(callbackUrl);
            var stringView = RenderViewToString<EmailBodyModel>("ConfirmEmail", emailinfo);
            var message = emailService.CreateMessage(model.Email, "Confirm your account", stringView);
            var mappedMessage = AutoMapper.Mapper.Map<EmailMessageModel>(message);

            addNewEmailMessage.Invoke(mappedMessage);
            Alert.Success("Check your inbox");

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpGet("{domainName}/resetpassword")]
        public IActionResult ResetPasswordInitiation(string domainName)
        {
            if (!checkDomainExistsByName.Invoke(domainName))
            {
                Alert.Warning("Choose existing domain");
                return RedirectToAction("ChooseDomain", "Auth");
            }

            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost("{domainName}/resetpassword")]
        public IActionResult ResetPasswordInitiation(ResetPasswordInitiationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Alert.Danger("Something went wrong");

                return View(model);
            }

            var user = getuserByEmail.Invoke(model.Email, model.DomainName);

            if (user == null)
            {
                Alert.Danger("We don't have this account in database");

                return View();
            }

            var token = addUserTokenToUser.Invoke(user.Id);
            var callbackUrl = Url.Action("ResetPasswordByToken", "Auth", new { token },
                Request.Scheme);
            var emailinfo = new EmailBodyHelper().GetResetPasswordBodyModel(callbackUrl);
            var stringView = RenderViewToString<EmailBodyModel>("ResetPassword", emailinfo);
            var message = emailService.CreateMessage(model.Email, "Confirm your account", stringView);
            var mappedMessage = AutoMapper.Mapper.Map<EmailMessageModel>(message);

            addNewEmailMessage.Invoke(mappedMessage);
            Alert.Success("Email will be sent to your account shortly");

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpGet("resetpasswordbytoken/{token}")]
        public IActionResult ResetPasswordByToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                Alert.Danger("Invalid token");

                return View("Error");
            }

            var user = getUserByToken.Invoke(token);

            if (user == null)
            {
                Alert.Danger("Invalid token");

                return View("Error");
            }

            var model = new ResetPasswordNewPasswordViewModel()
            {
                Token = token
            };

            return View(model);
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost("resetpasswordbytoken")]
        public IActionResult ResetPasswordByToken(ResetPasswordNewPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Alert.Danger("Something went wrong");

                return View(model);
            }

            var user = getUserByToken.Invoke(model.Token);

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

            return RedirectToAction("ChooseDomain");
        }
    }
}
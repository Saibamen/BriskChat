using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using TrollChat.BusinessLogic.Actions.Domain.Interfaces;
using TrollChat.BusinessLogic.Actions.Email.Interfaces;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Actions.UserToken.Interfaces;
using TrollChat.BusinessLogic.Helpers.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.Web.Authorization;
using TrollChat.Web.Helpers;
using TrollChat.Web.Models.Auth;

namespace TrollChat.Web.Controllers
{
    [Route("auth")]
    public class AuthController : BaseController
    {
        private readonly IAuthenticateUser authenticateUser;
        private readonly IAddNewUser addNewUser;
        private readonly IEmailService emailService;
        private readonly IConfirmUserEmailByToken confirmUserEmailByToken;
        private readonly IGetUserByEmail getUserByEmail;
        private readonly IAddUserTokenToUser addUserTokenToUser;
        private readonly IGetUserByToken getUserByToken;
        private readonly IEditUserPassword editUserPassword;
        private readonly IAddNewEmailMessage addNewEmailMessage;
        private readonly ICheckDomainExistsByName checkDomainExistsByName;
        private readonly IAddNewDomain addNewDomain;
        private readonly ISetDomainOwner setDomainOwner;

        public AuthController(IAuthenticateUser authenticateUser,
            IAddNewUser addNewUser,
            IEmailService emailService,
            IConfirmUserEmailByToken confirmUserEmailByToken,
            IGetUserByEmail getUserByEmail,
            IAddUserTokenToUser addUserTokenToUser,
            IGetUserByToken getUserByToken,
            IEditUserPassword editUserPassword,
            IDeleteUserTokenyByTokenString deleteUserTokenyByTokenString,
            IAddNewEmailMessage addNewEmailMessage,
            ICheckDomainExistsByName checkDomainExistsByName,
            IAddNewDomain addNewDomain,
            ISetDomainOwner setDomainOwner)
        {
            this.authenticateUser = authenticateUser;
            this.addNewUser = addNewUser;
            this.emailService = emailService;
            this.confirmUserEmailByToken = confirmUserEmailByToken;
            this.getUserByEmail = getUserByEmail;
            this.addUserTokenToUser = addUserTokenToUser;
            this.getUserByToken = getUserByToken;
            this.editUserPassword = editUserPassword;
            this.addNewEmailMessage = addNewEmailMessage;
            this.checkDomainExistsByName = checkDomainExistsByName;
            this.addNewDomain = addNewDomain;
            this.setDomainOwner = setDomainOwner;
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToAction("Login", new { domainName = model.DomainName });
        }

        [AllowAnonymous]
        [HttpGet("createdomain")]
        public IActionResult CreateDomain()
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost("createdomain")]
        public IActionResult CreateDomain(CreateDomainViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var domain = addNewDomain.Invoke(new DomainModel { Name = model.DomainName });

            if (domain == Guid.Empty)
            {
                ModelState.AddModelError("DomainName", "This domain already exists in our database");

                return View(model);
            }

            var userModel = new UserModel { Email = model.Email, Password = model.Password, Name = model.Name, Domain = new DomainModel { Id = domain } };
            var userAddAction = addNewUser.Invoke(userModel);

            if (userAddAction == null)
            {
                Alert.Danger("User already exists");

                return View();
            }

            var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { token = userAddAction.Tokens.FirstOrDefault().SecretToken },
                Request.Scheme);
            var emailinfo = new EmailBodyHelper().GetRegisterEmailBodyModel(callbackUrl);
            var stringView = RenderViewToString("EmailTemplate", emailinfo);
            var message = emailService.CreateMessage(model.Email, "Confirm your account", stringView);
            var mappedMessage = AutoMapper.Mapper.Map<EmailMessageModel>(message);
            addNewEmailMessage.Invoke(mappedMessage);

            setDomainOwner.Invoke(userAddAction.Id, domain);

            Alert.Success("Confirmation email has been sent to your email address");

            return RedirectToAction("Login", "Auth", new { domainName = model.DomainName });
        }

        [AllowAnonymous]
        [HttpGet("{domainName}/login")]
        public IActionResult Login(string returnUrl, string domainName)
        {
            if (string.IsNullOrEmpty(domainName) || !checkDomainExistsByName.Invoke(domainName))
            {
                Alert.Warning("Domain not found");
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
                ViewBag.ReturnUrl = returnUrl;

                return View(model);
            }

            var access = authenticateUser.Invoke(model.Email, model.Password, model.DomainName);

            if (access == null)
            {
                ModelState.AddModelError("Email", "Invalid email or password or email not confirmed");
                ViewBag.ReturnUrl = returnUrl;

                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, access.Name),
                new Claim(ClaimTypes.Sid, access.Id.ToString()),
                new Claim(ClaimTypes.Role, Role.User),
                new Claim(Constants.ClaimTypesConstants.DomainName, access.Domain.Name),
                new Claim(Constants.ClaimTypesConstants.DomainId, access.Domain.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Claims");
            var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.Authentication.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrinciple);

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
            await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

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

                return RedirectToAction("ChooseDomain", "Auth");
            }

            Alert.Success("Email confirmed");
            return RedirectToAction("ChooseDomain", "Auth");
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = getUserByEmail.Invoke(model.Email, model.DomainName);

            if (user == null)
            {
                Alert.Danger("We don't have this account in database");

                return View(model);
            }

            if (user.EmailConfirmedOn != null)
            {
                Alert.Info("Email already confirmed");

                return RedirectToAction("Login");
            }

            var token = addUserTokenToUser.Invoke(user.Id);

            var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { token }, Request.Scheme);
            var emailInfo = new EmailBodyHelper().GetRegisterEmailBodyModel(callbackUrl);
            var stringView = RenderViewToString("EmailTemplate", emailInfo);
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
                return View(model);
            }

            var user = getUserByEmail.Invoke(model.Email, model.DomainName);

            if (user == null)
            {
                Alert.Danger("We don't have this account in database");

                return View(model);
            }

            var token = addUserTokenToUser.Invoke(user.Id);
            var callbackUrl = Url.Action("ResetPasswordByToken", "Auth", new { token },
                Request.Scheme);
            var emailInfo = new EmailBodyHelper().GetResetPasswordBodyModel(callbackUrl);
            var stringView = RenderViewToString("EmailTemplate", emailInfo);
            var message = emailService.CreateMessage(model.Email, "Reset your password", stringView);
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

                return View();
            }

            var user = getUserByToken.Invoke(token);

            if (user == null)
            {
                Alert.Danger("User not found or not activated");

                return View();
            }

            var model = new ResetPasswordViewModel
            {
                Token = token
            };

            return View(model);
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost("resetpasswordbytoken/{token}")]
        public IActionResult ResetPasswordByToken(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = getUserByToken.Invoke(model.Token);

            if (user == null)
            {
                Alert.Danger("User not found or not activated");

                return RedirectToAction("ChooseDomain", "Auth");
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
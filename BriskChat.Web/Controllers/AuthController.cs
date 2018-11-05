using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BriskChat.BusinessLogic.Actions.Domain.Interfaces;
using BriskChat.BusinessLogic.Actions.Email.Interfaces;
using BriskChat.BusinessLogic.Actions.Role.Interfaces;
using BriskChat.BusinessLogic.Actions.User.Interfaces;
using BriskChat.BusinessLogic.Actions.UserDomain.Interfaces;
using BriskChat.BusinessLogic.Actions.UserToken.Interfaces;
using BriskChat.BusinessLogic.Helpers.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.Web.Authorization;
using BriskChat.Web.Constants;
using BriskChat.Web.Helpers;
using BriskChat.Web.Models.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BriskChat.Web.Controllers
{
    [Route("auth")]
    public class AuthController : BaseController
    {
        private readonly IAuthenticateUser _authenticateUser;
        private readonly IAddNewUser _addNewUser;
        private readonly IEmailService _emailService;
        private readonly IConfirmUserEmailByToken _confirmUserEmailByToken;
        private readonly IGetUserByEmail _getUserByEmail;
        private readonly IAddUserTokenToUser _addUserTokenToUser;
        private readonly IGetUserByToken _getUserByToken;
        private readonly IEditUserPassword _editUserPassword;
        private readonly IAddNewEmailMessage _addNewEmailMessage;
        private readonly ICheckDomainExistsByName _checkDomainExistsByName;
        private readonly IAddNewDomain _addNewDomain;
        private readonly ISetDomainOwner _setDomainOwner;
        private readonly IAddUserToDomain _addUserToDomain;
        private readonly IGetRoleByName _getRoleByName;

        public AuthController(IAuthenticateUser authenticateUser,
            IAddNewUser addNewUser,
            IEmailService emailService,
            IConfirmUserEmailByToken confirmUserEmailByToken,
            IGetUserByEmail getUserByEmail,
            IAddUserTokenToUser addUserTokenToUser,
            IGetUserByToken getUserByToken,
            IEditUserPassword editUserPassword,
            IDeleteUserTokenByTokenString deleteUserTokenByTokenString,
            IAddNewEmailMessage addNewEmailMessage,
            ICheckDomainExistsByName checkDomainExistsByName,
            IAddNewDomain addNewDomain,
            ISetDomainOwner setDomainOwner,
            IAddUserToDomain addUserToDomain,
            IGetRoleByName getRoleByName)
        {
            _authenticateUser = authenticateUser;
            _addNewUser = addNewUser;
            _emailService = emailService;
            _confirmUserEmailByToken = confirmUserEmailByToken;
            _getUserByEmail = getUserByEmail;
            _addUserTokenToUser = addUserTokenToUser;
            _getUserByToken = getUserByToken;
            _editUserPassword = editUserPassword;
            _addNewEmailMessage = addNewEmailMessage;
            _checkDomainExistsByName = checkDomainExistsByName;
            _addNewDomain = addNewDomain;
            _setDomainOwner = setDomainOwner;
            _addUserToDomain = addUserToDomain;
            _getRoleByName = getRoleByName;
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

            var domain = _addNewDomain.Invoke(new DomainModel { Name = model.DomainName });

            if (domain == Guid.Empty)
            {
                ModelState.AddModelError("DomainName", "This domain already exists in our database");

                return View(model);
            }

            var userModel = new UserModel { Email = model.Email, Password = model.Password, Name = model.Name, Domain = new DomainModel { Id = domain } };
            var userAddAction = _addNewUser.Invoke(userModel);

            if (userAddAction == null)
            {
                Alert.Danger("User already exists");

                return View();
            }

            var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { token = userAddAction.Tokens.FirstOrDefault().SecretToken },
                Request.Scheme);
            var emailInfo = new EmailBodyHelper().GetRegisterEmailBodyModel(callbackUrl);
            var stringView = RenderViewToString("EmailTemplate", emailInfo);
            var message = _emailService.CreateMessage(model.Email, "Confirm your account", stringView);
            var mappedMessage = AutoMapper.Mapper.Map<EmailMessageModel>(message);
            _addNewEmailMessage.Invoke(mappedMessage);

            _setDomainOwner.Invoke(userAddAction.Id, domain);

            var role = _getRoleByName.Invoke(RoleNamesConstants.Owner);
            // Add user to UserDomains table
            _addUserToDomain.Invoke(userAddAction.Id, domain, role.Id);

            Alert.Success("Confirmation email has been sent to your email address");

            return RedirectToAction("Login", "Auth", new { domainName = model.DomainName });
        }

        [AllowAnonymous]
        [HttpGet("{domainName}/login")]
        public IActionResult Login(string returnUrl, string domainName)
        {
            if (string.IsNullOrWhiteSpace(domainName) || !_checkDomainExistsByName.Invoke(domainName))
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

            var access = _authenticateUser.Invoke(model.Email, model.Password, model.DomainName);

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
                new Claim(ClaimTypesConstants.DomainName, access.Domain.Name),
                new Claim(ClaimTypesConstants.DomainId, access.Domain.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Claims");
            var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrinciple);

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return RedirectToAction("Index", "User");
            }

            return Redirect(returnUrl);
        }

        [ValidateAntiForgeryToken]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            Alert.Success("Logged out");

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet("confirmemail")]
        public IActionResult ConfirmEmail(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                Alert.Danger("Invalid token");

                return View("Error");
            }

            var confirmAction = _confirmUserEmailByToken.Invoke(token);

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
            if (_checkDomainExistsByName.Invoke(domainName))
            {
                return View();
            }

            Alert.Warning("Choose existing domain");

            return RedirectToAction("ChooseDomain", "Auth");
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

            var user = _getUserByEmail.Invoke(model.Email, model.DomainName);

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

            var token = _addUserTokenToUser.Invoke(user.Id);

            var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { token }, Request.Scheme);
            var emailInfo = new EmailBodyHelper().GetRegisterEmailBodyModel(callbackUrl);
            var stringView = RenderViewToString("EmailTemplate", emailInfo);
            var message = _emailService.CreateMessage(model.Email, "Confirm your account", stringView);
            var mappedMessage = AutoMapper.Mapper.Map<EmailMessageModel>(message);

            _addNewEmailMessage.Invoke(mappedMessage);
            Alert.Success("Check your inbox");

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpGet("{domainName}/resetpassword")]
        public IActionResult ResetPasswordInitiation(string domainName)
        {
            if (_checkDomainExistsByName.Invoke(domainName))
            {
                return View();
            }

            Alert.Warning("Choose existing domain");

            return RedirectToAction("ChooseDomain", "Auth");
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

            var user = _getUserByEmail.Invoke(model.Email, model.DomainName);

            if (user == null)
            {
                Alert.Danger("We don't have this account in database");

                return View(model);
            }

            var token = _addUserTokenToUser.Invoke(user.Id);
            var callbackUrl = Url.Action("ResetPasswordByToken", "Auth", new { token },
                Request.Scheme);
            var emailInfo = new EmailBodyHelper().GetResetPasswordBodyModel(callbackUrl);
            var stringView = RenderViewToString("EmailTemplate", emailInfo);
            var message = _emailService.CreateMessage(model.Email, "Reset your password", stringView);
            var mappedMessage = AutoMapper.Mapper.Map<EmailMessageModel>(message);

            _addNewEmailMessage.Invoke(mappedMessage);
            Alert.Success("Email will be sent to your account shortly");

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpGet("resetpasswordbytoken/{token}")]
        public IActionResult ResetPasswordByToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                Alert.Danger("Invalid token");

                return View();
            }

            var user = _getUserByToken.Invoke(token);

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

            var user = _getUserByToken.Invoke(model.Token);

            if (user == null)
            {
                Alert.Danger("User not found or not activated");

                return RedirectToAction("ChooseDomain", "Auth");
            }

            var result = _editUserPassword.Invoke(user.Id, model.Password);

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
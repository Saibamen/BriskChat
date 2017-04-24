﻿using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Configuration.Seeder;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TrollChat.BusinessLogic.Actions.Domain.Interfaces;

namespace TrollChat.Web.Controllers
{
    [Route("[controller]")]
    public class SeedController : Controller
    {
        private readonly IAddNewUser addNewUser;
        private readonly IHostingEnvironment env;
        private readonly IConfirmUserEmailByToken ConfirmUserEmailByToken;
        private readonly IAddNewDomain addNewDomain;
        private readonly IGetUserByEmail getUserByEmail;
        private readonly IGetDomainByName getDomainByName;

        public SeedController(IAddNewUser addNewUser,
            IConfirmUserEmailByToken ConfirmUserEmailByToken,
            IHostingEnvironment env,
            IAddNewDomain addNewDomain,
            IGetUserByEmail getUserByEmail,
            IGetDomainByName getDomainByName)
        {
            this.addNewUser = addNewUser;
            this.ConfirmUserEmailByToken = ConfirmUserEmailByToken;
            this.addNewDomain = addNewDomain;
            this.getUserByEmail = getUserByEmail;
            this.getDomainByName = getDomainByName;
            this.env = env;
        }

        [HttpGet("seedall")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            new DbContextSeeder().Seed(addNewUser, ConfirmUserEmailByToken, addNewDomain, getUserByEmail, getDomainByName);

            return Json("Database seeded");
        }
    }
}
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Actions.Domain.Interface;
using TrollChat.BusinessLogic.Configuration.Seeder;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

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

        public SeedController(IAddNewUser addNewUser,
            IConfirmUserEmailByToken ConfirmUserEmailByToken,
            IHostingEnvironment env,
            IAddNewDomain addNewDomain, IGetUserByEmail getUserByEmail)
        {
            this.addNewUser = addNewUser;
            this.ConfirmUserEmailByToken = ConfirmUserEmailByToken;
            this.addNewDomain = addNewDomain;
            this.getUserByEmail = getUserByEmail;
            this.env = env;
        }

        [HttpGet("seedall")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            using (var context = new TrollChatDbContext())
            {
                new DbContextSeeder(context).Seed(addNewUser, ConfirmUserEmailByToken, addNewDomain, getUserByEmail);
            }

            return Json("Database seeded");
        }
    }
}
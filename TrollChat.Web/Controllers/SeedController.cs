using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Configuration.Seeder;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TrollChat.BusinessLogic.Models.Common;

namespace TrollChat.Web.Controllers
{
    [Route("api/[controller]")]
    public class SeedController : Controller
    {
        private readonly IAddNewUser addNewUser;
        private readonly IHostingEnvironment env;
        private readonly IConfirmUserEmailByToken ConfirmUserEmailByToken;

        public SeedController(IAddNewUser addNewUser,
            IConfirmUserEmailByToken ConfirmUserEmailByToken,
            IHostingEnvironment env)
        {
            this.addNewUser = addNewUser;

            this.ConfirmUserEmailByToken = ConfirmUserEmailByToken;
            this.env = env;
        }

        [HttpGet("seedall")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (!env.IsDevelopment())
            {
                var errorResult = new ResultModel<string>("Not authorize", ResultCode.Error);
                return Json(errorResult);
            }

            using (var context = new TrollChatDbContext())
            {
                new DbContextSeeder(context).Seed(addNewUser, ConfirmUserEmailByToken);
            }

            return Json("Database seeded");
        }
    }
}
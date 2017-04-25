using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Configuration.Seeder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrollChat.BusinessLogic.Actions.Domain.Interfaces;

namespace TrollChat.Web.Controllers
{
    [Route("[controller]")]
    public class SeedController : Controller
    {
        private readonly IAddNewUser addNewUser;
        private readonly IConfirmUserEmailByToken confirmUserEmailByToken;
        private readonly IAddNewDomain addNewDomain;
        private readonly IGetDomainByName getDomainByName;

        public SeedController(IAddNewUser addNewUser,
            IConfirmUserEmailByToken confirmUserEmailByToken,
            IAddNewDomain addNewDomain,
            IGetDomainByName getDomainByName)
        {
            this.addNewUser = addNewUser;
            this.confirmUserEmailByToken = confirmUserEmailByToken;
            this.addNewDomain = addNewDomain;
            this.getDomainByName = getDomainByName;
        }

        [HttpGet("seedall")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            new DbContextSeeder().Seed(addNewUser, confirmUserEmailByToken, addNewDomain, getDomainByName);

            return Json("Database seeded");
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrollChat.BusinessLogic.Configuration.Interfaces;

namespace TrollChat.Web.Controllers
{
    [Route("[controller]")]
    public class SeedController : Controller
    {
        private readonly IDbContextSeeder dbContextSeeder;

        public SeedController(IDbContextSeeder dbContextSeeder)
        {
            this.dbContextSeeder = dbContextSeeder;
        }

        [HttpGet("seedall")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            dbContextSeeder.Seed();
            return Json("Database seeded");
        }
    }
}
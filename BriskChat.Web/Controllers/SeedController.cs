using BriskChat.BusinessLogic.Configuration.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BriskChat.Web.Controllers
{
    [Route("[controller]")]
    public class SeedController : Controller
    {
        private readonly IDbContextSeeder _dbContextSeeder;

        public SeedController(IDbContextSeeder dbContextSeeder)
        {
            _dbContextSeeder = dbContextSeeder;
        }

        [HttpGet("seedall")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var isSeeded = _dbContextSeeder.Seed();

            return Json(isSeeded ? "Database seeded" : "Some errors when seeding database :(");
        }
    }
}
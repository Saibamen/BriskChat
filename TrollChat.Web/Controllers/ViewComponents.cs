using Microsoft.AspNetCore.Mvc;

namespace TrollChat.Web.Controllers
{
    public class CreateRoomViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
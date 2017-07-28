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

    public class CreatePrivateConversationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }

    public class BrowseRoomsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
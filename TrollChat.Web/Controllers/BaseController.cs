using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TrollChat.Web.Helpers;

namespace TrollChat.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public AlertHelper Alert = new AlertHelper();

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            if (Alert.GetAlerts().Any())
            {
                TempData.Put("alertMessages", Alert.GetAlerts());
            }
        }
    }
}
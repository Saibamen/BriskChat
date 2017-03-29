using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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

        public string RenderViewToString<T>(string viewName, string masterName, T model, bool customized = false, string controllerName = "")
        {
            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                var engine = HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

                if (engine != null)
                {
                    ViewEngineResult viewResult =
                        engine.FindView(ControllerContext, viewName, true);
                    ViewContext viewContext =
                        new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw, new HtmlHelperOptions());

                    viewResult.View.RenderAsync(viewContext);
                }

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace BMSS.WebUI.Helpers.HtmlHelpers
{
    public static class PartialRender
    {
        public static string RenderToString(this PartialViewResult partialView)
        {
            HttpContext hc = HttpContext.Current;

            if (hc == null)
                throw new NotSupportedException("An HTTP context is required to render the partial view to a string");

            var controllerName = hc.Request.RequestContext.RouteData.Values["controller"].ToString();
            var controller = (ControllerBase)ControllerBuilder.Current.GetControllerFactory().CreateController(hc.Request.RequestContext, controllerName);
            var controllerContext = new ControllerContext(hc.Request.RequestContext, controller);
            var view = ViewEngines.Engines.FindPartialView(controllerContext, partialView.ViewName).View;
            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                using (var tw = new HtmlTextWriter(sw))
                {
                    view.Render(new ViewContext(controllerContext, view, partialView.ViewData, partialView.TempData, tw), tw);
                }
            }

            return sb.ToString();
        }
    }
}
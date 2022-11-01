using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BMSS.WebUI.Helpers.Attributes
{
    public class EditAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            { 
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
               new { action = "UnAuthoziedAjaxRequest", controller = "UnAuthorized" }));
                //new RedirectResult("/UnAuthorized/UnAuthoziedAjaxRequest");
            }
        }

       
    }
}
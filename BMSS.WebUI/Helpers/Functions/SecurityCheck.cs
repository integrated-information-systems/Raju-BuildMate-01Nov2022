using BMSS.WebUI.Helpers.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMSS.WebUI.Helpers.Functions
{
    public static class SecurityCheck
    {
        public static bool ActionIsAuthorized(ActionParameterHelperModel  actionParameterHelperModel)
        {
            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
            ControllerBase controller = factory.CreateController(HttpContext.Current.Request.RequestContext, actionParameterHelperModel.Controller) as ControllerBase;
            var controllerContext = new ControllerContext(HttpContext.Current.Request.RequestContext, controller);
            var controllerDescriptor = new ReflectedControllerDescriptor(controller.GetType());
            var actionDescriptor = controllerDescriptor.FindAction(controllerContext, actionParameterHelperModel.Action);
            AuthorizationContext authContext = new AuthorizationContext(controllerContext, actionDescriptor);
            foreach (var authAttribute in controllerDescriptor.GetFilterAttributes(true).Where(a => a is AuthorizeAttribute).Select(a => a as AuthorizeAttribute))
            {
                authAttribute.OnAuthorization(authContext);
                if (authContext.Result != null)
                    return false;
            }
            foreach (var authAttribute in actionDescriptor.GetFilterAttributes(true).Where(a => a is AuthorizeAttribute).Select(a => a as AuthorizeAttribute))
            {
                authAttribute.OnAuthorization(authContext);
                if (authContext.Result != null)
                    return false;
            }
            return true;
        }
    }
}
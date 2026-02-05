using System.Web.Mvc;
using System.Web.Routing;

public class BaseController : Controller
{
    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        // If user is NOT logged in
        if (Session["LoginId"] == null)
        {
            string controllerName =
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            // Allow Login controller without redirect loop
            if (!controllerName.Equals("Login", System.StringComparison.OrdinalIgnoreCase))
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Login" },  
                        { "action", "Index" }
                    }
                );
                return;
            }
        }

        base.OnActionExecuting(filterContext);
    }
}

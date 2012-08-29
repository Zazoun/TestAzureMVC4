using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Test.Utilities
{
    public class CultureAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // Is it View ?
            var view = filterContext.Result as ViewResultBase;
            if (view == null) // if not exit
                return;

            var cultureName = Thread.CurrentThread.CurrentCulture.Name; 

            // Is it default culture? exit
            if (cultureName == CultureHelper.GetDefaultCulture())
                return;


            // Are views implemented separately for this culture?  if not exit
            var viewImplemented = CultureHelper.IsViewSeparate(cultureName);
            if (viewImplemented == false)
                return;

            var viewName = view.ViewName;

            int i;

            if (string.IsNullOrEmpty(viewName))
                viewName = filterContext.RouteData.Values["action"] + "." + cultureName; // Index.en-US
            else if ((i = viewName.IndexOf('.')) > 0)
            {
                // contains . like "Index.cshtml"                
                viewName = viewName.Substring(0, i + 1) + cultureName + viewName.Substring(i);
            }
            else
                viewName += "." + cultureName; // e.g. "Index" ==> "Index.en-Us"

            view.ViewName = viewName;

            filterContext.Controller.ViewBag._culture = "." + cultureName;

            base.OnActionExecuted(filterContext);
        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Attempt to read the culture cookie from Request
            var cultureCookie = filterContext.HttpContext.Request.Cookies["_culture"];
            var cultureName = cultureCookie != null ? cultureCookie.Value : filterContext.HttpContext.Request.UserLanguages[0];

            // Validate culture name
            cultureName = CultureHelper.GetValidCulture(cultureName); // This is safe
            
            // Modify current thread's culture            
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureName);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(cultureName);

        }
    }
}
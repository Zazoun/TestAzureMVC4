using System.Web.Mvc;

namespace Test.Areas.Front
{
    public class FrontAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Front";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Front_default",
                "Front/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Movie/{action}/{id}",
                "Films/{action}/{id}",
                new { controller = "Movie", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

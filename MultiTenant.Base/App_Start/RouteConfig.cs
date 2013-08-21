using System.Web.Mvc;
using System.Web.Routing;
using MultiTenant.Infrastructure;

namespace MultiTenant.Base
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // base route that gets matched provided none of the parameters 
            // match any tenant names
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new  {controller = new NonTenantRouteConstraint(MvcApplication.Tenants)});

            routes.MapRoute(
                name: "Tenant",
                url: "{tenant}/{controller}/{action}/{id}",
                defaults: new { tenant = "", controller = "Home", action = "Index", id = UrlParameter.Optional });

            
        }
    }
}
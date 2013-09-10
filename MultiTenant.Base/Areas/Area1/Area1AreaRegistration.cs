using System.Web.Mvc;
using MultiTenant.Infrastructure;
using MultiTenant.Infrastructure.Mvc;

namespace MultiTenant.Base.Areas.Area1
{
    public class Area1AreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Area1";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Area1_default",
                "Area1/{controller}/{action}/{id}",
                new {action = "Index", id = UrlParameter.Optional},
                new {controller = new NonTenantRouteConstraint(MvcApplication.Tenants)}
                );

            context.MapRoute(
                name: "Area1_Tenant",
                url: "{tenant}/Area1/{controller}/{action}/{id}",
                defaults: new {tenant = "", controller = "Home", action = "Index", id = UrlParameter.Optional});
        }
    }
}

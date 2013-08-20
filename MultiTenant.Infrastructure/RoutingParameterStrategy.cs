using System.Web;
using System.Web.Routing;
using Autofac.Extras.Multitenant;

public class RoutingParameterStrategy : ITenantIdentificationStrategy
{
    public bool TryIdentifyTenant(out object tenantId)
    {
        tenantId = null;
        try
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(context));
                if (routeData != null)
                    tenantId = routeData.Values.ContainsKey("tenant") ? routeData.Values["tenant"] : null;
            }
        }
        catch (HttpException)
        {
            // Happens at app startup in IIS 7.0
        }
        return !string.IsNullOrWhiteSpace(tenantId as string);
    }
}
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;

namespace MultiTenant.Infrastructure.Mvc
{
    /// <summary>
    /// This constraint casuses a route not to be matched if the route 
    /// matches a tenant name
    /// </summary>
    public class NonTenantRouteConstraint : IRouteConstraint
    {
        private ICollection<string> TenantList { get; set; }

        public NonTenantRouteConstraint(ICollection<string> tenantList)
        {
            TenantList = tenantList;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            var param = values[parameterName].ToString();

            return !TenantList.Contains(param);
        }
    }
}

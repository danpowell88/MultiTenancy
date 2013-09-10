using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MultiTenant.Infrastructure;

namespace MultiTenant.Base
{
    public class MvcApplication : HttpApplication
    {
        private static List<string> _tenants;

        public static ICollection<string> Tenants
        {
            get
            {
                // return a copy of the tenants list to prevent outside modification
                return _tenants.AsReadOnly();
            }
        }

        public static Dictionary<Assembly, PluginAssemblyAttribute> GetAssemblyPluginDetails(
            IEnumerable<Assembly> assemblies)
        {
            var assemblyPluginDetails = new Dictionary<Assembly, PluginAssemblyAttribute>();

            foreach (var assembly in assemblies
                .AsQueryable()
                .Where(a => a.IsDefined(typeof (PluginAssemblyAttribute), false)))
            {
                var pluginSpecification =
                    assembly.GetCustomAttributes(typeof (PluginAssemblyAttribute), false)
                        .OfType<PluginAssemblyAttribute>()
                        .Single();

                assemblyPluginDetails[assembly] = pluginSpecification;
            }

            return assemblyPluginDetails;
        }

        public static ILookup<string, PluginAssembly> GetTenantsAssemblies(IEnumerable<Assembly> assemblies)
        {
            var pluginDetails = GetAssemblyPluginDetails(assemblies);
            return pluginDetails.ToLookup(k => k.Value.Tenant,
                v => new PluginAssembly {Assembly = v.Key, PluginAssemblyType = v.Value.AssemblyType});
        }

        protected void Application_Start()
        {
            var assemblyPluginDetails = GetAssemblyPluginDetails(AppDomain.CurrentDomain.GetAssemblies());
            _tenants = assemblyPluginDetails.Select(a => a.Value.Tenant).Distinct().ToList();

            IocConfig.RegisterDependencies();
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
        }
    }
}
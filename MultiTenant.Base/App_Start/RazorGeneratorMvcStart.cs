using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using MultiTenant.Base;
using MultiTenant.Infrastructure;
using MultiTenant.Infrastructure.Mvc;
using WebActivatorEx;

using System.Collections.Generic;

[assembly: PostApplicationStartMethod(typeof (RazorGeneratorMvcStart), "Start")]

namespace MultiTenant.Base
{
    public static class RazorGeneratorMvcStart
    {
        public static void Start()
        {
            var assemblyPluginDetails = MvcApplication.GetAssemblyPluginDetails(AppDomain.CurrentDomain.GetAssemblies());

            Func<RouteValueDictionary, IEnumerable<Assembly>, Assembly> assemblySelector = (routes, assemblies) =>
                                                                                            {
                                                                                                object tenantId;

                                                                                                var canParse =routes.TryGetValue("tenant",out tenantId);

                                                                                                if (canParse)
                                                                                                {
                                                                                                    var tenantAssemblies=MvcApplication.GetTenantsAssemblies(assemblies);

                                                                                                    var tenantViewAssembly=tenantAssemblies[(string)tenantId].SingleOrDefault
                                                                                                                (p =>p.PluginAssemblyType.HasFlag(PluginAssemblyType.View));

                                                                                                    return tenantViewAssembly.Assembly;
                                                                                                }
                                                                                                return null;
                                                                                            };

            var engine = new MultiTenantPrecompiledViewEngine(assemblySelector,
                assemblyPluginDetails.Where(p => p.Value.AssemblyType.HasFlag(PluginAssemblyType.View))
                    .Select(a => a.Key)
                    .ToArray());

            ViewEngines.Engines.Insert(0, engine);

            // StartPage lookups are done by WebPages. 
            VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);
        }
    }
}
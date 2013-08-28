using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Extras.Multitenant;
using Autofac.Integration.Mvc;
using MultiTenant.Infrastructure;

namespace MultiTenant.Base
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
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

        protected void Application_Start()
        {
            _tenants = new List<string>();

            ViewEngines.Engines.Clear();

            // Set up the tenant ID strategy and application container.
            // The request parameter tenant ID strategy is used here as an example.
            // You should use your own strategy in production.
            var tenantIdStrategy = new RoutingParameterStrategy();
            var builder = new ContainerBuilder();

            //builder.RegisterType<RazorViewEngine>().As<IViewEngine>();
            builder.RegisterType<WebFormViewEngine>().As<IViewEngine>();

            builder.RegisterControllers(typeof (MvcApplication).Assembly)
                .Named<IController>(t => t.Name.Replace("Controller", string.Empty))
                .AsSelf();

            // Create the multitenant container and the tenant overrides.
            var mtc = new MultitenantContainer(tenantIdStrategy, builder.Build());

            foreach (
                var assembly in
                    AppDomain.CurrentDomain.GetAssemblies()
                        .AsQueryable()
                        .Where(a => a.IsDefined(typeof (PluginAssemblyAttribute), false)))
            {
                var pluginSpecification =
                    assembly.GetCustomAttributes(typeof (PluginAssemblyAttribute), false)
                        .OfType<PluginAssemblyAttribute>()
                        .Single();

                _tenants.Add(pluginSpecification.Tenant);

                var assembly1 = assembly;
                mtc.ConfigureTenant(pluginSpecification.Tenant,
                    b =>
                    {
                        b.RegisterControllers(assembly1)
                            .Named<IController>(t => t.Name.Replace("Controller", string.Empty))
                            .As(a =>
                                {
                                    var baseType = a.BaseType;

                                    while (baseType != null && baseType.Assembly !=
                                           typeof (MvcApplication).Assembly)
                                    {
                                        baseType = baseType.BaseType;
                                    }

                                    return baseType ?? a;
                                });

                        b.RegisterType<RazorViewEngine>().As<IViewEngine>();
                    }
                    );
            }

            // Here's the magic line: Set up the DependencyResolver using
            // a multitenant container rather than a regular container.
            DependencyResolver.SetResolver(new AutofacDependencyResolver(mtc));

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            ControllerBuilder.Current.SetControllerFactory(new MultiTenantControllerFactory(mtc));

            //ViewEngines.Engines.Add(new MultiTenantRazorViewEngine(mtc));
            //VirtualPathFactoryManager.RegisterVirtualPathFactory(new MultiTenantVirtualPathFactory(mtc));
        }
    }
}
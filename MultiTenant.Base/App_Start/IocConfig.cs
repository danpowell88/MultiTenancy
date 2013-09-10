using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Extras.Multitenant;
using Autofac.Integration.Mvc;
using MultiTenant.Infrastructure;
using MultiTenant.Infrastructure.Mvc;

namespace MultiTenant.Base
{
    public class IocConfig
    {
        public static void RegisterDependencies()
        {
            var tenantsAssemblies = MvcApplication.GetTenantsAssemblies(AppDomain.CurrentDomain.GetAssemblies());

            var tenantIdStrategy = new RoutingParameterStrategy();
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly).Named<IController>(t => t.Name.Replace("Controller", String.Empty)).AsSelf();

            var mtc = new MultitenantContainer(tenantIdStrategy, builder.Build());

            foreach (var tenantAssembly in tenantsAssemblies.ToList())
            {
                var assembly = tenantAssembly;
                mtc.ConfigureTenant(tenantAssembly.Key, b =>
                {
                    foreach (var pluginAssembly in assembly)
                    {
                        if (
                            pluginAssembly.PluginAssemblyType.HasFlag(
                                PluginAssemblyType.Controller))
                        {
                            RegisterControllers(b, pluginAssembly);
                        }
                    }
                });
            }

            // Here's the magic line: Set up the DependencyResolver using
            // a multitenant container rather than a regular container.
            DependencyResolver.SetResolver(new AutofacDependencyResolver(mtc));
            ControllerBuilder.Current.SetControllerFactory(new MultiTenantControllerFactory(mtc));
        }

        private static void RegisterControllers(ContainerBuilder b, PluginAssembly assembly)
        {
            b.RegisterControllers(assembly.Assembly)
                .Named<IController>(t => t.Name.Replace("Controller", String.Empty))
                .As(a =>
                    {
                        var baseType = a.BaseType;

                        while (baseType != null && baseType.Assembly !=
                               typeof(MvcApplication).Assembly)
                        {
                            baseType = baseType.BaseType;
                        }

                        return baseType ?? a;
                    });
        }
    }
}
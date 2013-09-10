using System;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;

namespace MultiTenant.Infrastructure.Mvc
{
    public class MultiTenantControllerFactory : DefaultControllerFactory
    {
        private readonly IContainer _container;

        public MultiTenantControllerFactory(IContainer container)
        {
            _container = container;
        }

        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            Type controller;
            object x;
            if (_container.TryResolveNamed(controllerName, typeof(IController), out x))
                controller = x.GetType();
            else
            {
                controller = base.GetControllerType(requestContext, controllerName);
            }

            return controller;
        }
    }
}

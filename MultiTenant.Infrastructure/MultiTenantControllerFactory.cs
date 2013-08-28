using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;

namespace MultiTenant.Infrastructure
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

    //public class MultiTenantRazorViewEngine : IViewEngine
    //{
    //    private readonly IContainer _container;

    //    private IViewEngine[] _fallbackViewEngines = {new RazorViewEngine(), new WebFormViewEngine()};

    //    public MultiTenantRazorViewEngine(IContainer container)
    //    {
    //        _container = container;
    //    }

    //    public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
    //    {
    //        IViewEngine engine;
    //        if (_container.TryResolve(out engine))
    //        {
    //            engine.FindPartialView(controllerContext, partialViewName, useCache);
    //        }
    //        else
    //        {
    //            engine = new RazorViewEngine();
    //        }
    //    }

    //    public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
    //    {
    //        var engine = _container.Resolve<IViewEngine>();
    //        engine.FindView(controllerContext, viewName,masterName, useCache);
    //    }

    //    public void ReleaseView(ControllerContext controllerContext, IView view)
    //    {
    //        var engine = _container.Resolve<IViewEngine>();
    //        engine.ReleaseView(controllerContext, view);
    //    }
    //}
}

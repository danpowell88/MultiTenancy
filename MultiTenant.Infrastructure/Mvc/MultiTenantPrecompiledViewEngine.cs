using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using RazorGenerator.Mvc;

namespace MultiTenant.Infrastructure.Mvc
{
    public class MultiTenantPrecompiledViewEngine : VirtualPathProviderViewEngine, IVirtualPathFactory
    {
        private readonly Func<RouteValueDictionary, IEnumerable<Assembly>, Assembly>
            _assemblySelector;

        private readonly IDictionary<Assembly, IDictionary<string, ViewMapping>> _mappings =
            new Dictionary<Assembly, IDictionary<string, ViewMapping>>();

        private readonly IViewPageActivator _viewPageActivator;

        public MultiTenantPrecompiledViewEngine(
            Func<RouteValueDictionary, IEnumerable<Assembly>, Assembly> assemblySelector,
            params Assembly[] viewAssemblies)
            : this(viewAssemblies, assemblySelector, null)
        {
        }

        public MultiTenantPrecompiledViewEngine(IEnumerable<Assembly> viewAssemblies,
            Func<RouteValueDictionary, IEnumerable<Assembly>, Assembly> assemblySelector,
            IViewPageActivator viewPageActivator)
        {
            _assemblySelector = assemblySelector;
            base.AreaViewLocationFormats = new[]
                                           {
                                               "~/Areas/{2}/Views/{1}/{0}.cshtml",
                                               "~/Areas/{2}/Views/Shared/{0}.cshtml",
                                           };

            base.AreaMasterLocationFormats = new[]
                                             {
                                                 "~/Areas/{2}/Views/{1}/{0}.cshtml",
                                                 "~/Areas/{2}/Views/Shared/{0}.cshtml",
                                             };

            base.AreaPartialViewLocationFormats = new[]
                                                  {
                                                      "~/Areas/{2}/Views/{1}/{0}.cshtml",
                                                      "~/Areas/{2}/Views/Shared/{0}.cshtml",
                                                  };
            base.ViewLocationFormats = new[]
                                       {
                                           "~/Views/{1}/{0}.cshtml",
                                           "~/Views/Shared/{0}.cshtml",
                                       };
            base.MasterLocationFormats = new[]
                                         {
                                             "~/Views/{1}/{0}.cshtml",
                                             "~/Views/Shared/{0}.cshtml",
                                         };
            base.PartialViewLocationFormats = new[]
                                              {
                                                  "~/Views/{1}/{0}.cshtml",
                                                  "~/Views/Shared/{0}.cshtml",
                                              };
            base.FileExtensions = new[]
                                  {
                                      "cshtml",
                                  };

            foreach (var viewAssembly in viewAssemblies)
            {
                var precompiledViewAssembly = new PrecompiledViewAssembly(viewAssembly);

                var virtualPathViewMappings = new Dictionary<string, ViewMapping>();

                foreach (var mapping in precompiledViewAssembly.GetTypeMappings())
                {
                    virtualPathViewMappings[mapping.Key] = new ViewMapping
                                                           {
                                                               Type = mapping.Value,
                                                               ViewAssembly = precompiledViewAssembly
                                                           };
                }

                _mappings[viewAssembly] = virtualPathViewMappings;
            }

            _viewPageActivator = viewPageActivator
                                 ??
                                 DependencyResolver.Current.GetService<IViewPageActivator>()
                                     /* For compatibility, remove this line within next version */
                                 ?? DefaultViewPageActivator.Current;
        }

        public object CreateInstance(string virtualPath)
        {
            var routeData = ((MvcHandler) HttpContext.Current.Handler).RequestContext.RouteData;

            ViewMapping mapping;

            if (!_mappings[_assemblySelector(routeData.Values, _mappings.Keys)].TryGetValue(virtualPath, out mapping))
            {
                return null;
            }

            if (!mapping.ViewAssembly.PreemptPhysicalFiles && VirtualPathProvider.FileExists(virtualPath))
            {
                // If we aren't pre-empting physical files, use the BuildManager to create _ViewStart instances if the file exists on disk. 
                return BuildManager.CreateInstanceFromVirtualPath(virtualPath, typeof (WebPageRenderingBase));
            }

            if (mapping.ViewAssembly.UsePhysicalViewsIfNewer && mapping.ViewAssembly.IsPhysicalFileNewer(virtualPath))
            {
                // If the physical file on disk is newer and the user's opted in this behavior, serve it instead.
                return BuildManager.CreateInstanceFromVirtualPath(virtualPath, typeof (WebViewPage));
            }

            return _viewPageActivator.Create((ControllerContext) null, mapping.Type);
        }

        public bool Exists(string virtualPath)
        {
            var routeData = ((MvcHandler) HttpContext.Current.Handler).RequestContext.RouteData;

            var tenantAssembly = _assemblySelector(routeData.Values, _mappings.Keys);

            if (tenantAssembly == null)
                return false;

            var viewMappings = _mappings[tenantAssembly];

            return viewMappings.ContainsKey(virtualPath);
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return CreateViewInternal(controllerContext, partialPath, masterPath: null, runViewStartPages: false);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            return CreateViewInternal(controllerContext, viewPath, masterPath, runViewStartPages: true);
        }

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            ViewMapping mapping;

            var tenantAssembly = _assemblySelector(controllerContext.RouteData.Values, _mappings.Keys);

            if (tenantAssembly == null)
                return false;

            if (!_mappings[tenantAssembly].TryGetValue(virtualPath, out mapping))
            {
                return false;
            }

            if (mapping.ViewAssembly.UsePhysicalViewsIfNewer && mapping.ViewAssembly.IsPhysicalFileNewer(virtualPath))
            {
                // If the physical file on disk is newer and the user's opted in this behavior, serve it instead.
                return false;
            }
            return Exists(virtualPath);
        }

        private IView CreateViewInternal(ControllerContext controllerContext, string viewPath, string masterPath,
            bool runViewStartPages)
        {
            ViewMapping mapping;

            if (_mappings[_assemblySelector(controllerContext.RouteData.Values, _mappings.Keys)].TryGetValue(viewPath,
                out mapping))
            {
                return new PrecompiledMvcView(viewPath, masterPath, mapping.Type, runViewStartPages, base.FileExtensions,
                    _viewPageActivator);
            }
            return null;
        }

        private struct ViewMapping
        {
            public Type Type { get; set; }
            public PrecompiledViewAssembly ViewAssembly { get; set; }
        }
    }
}
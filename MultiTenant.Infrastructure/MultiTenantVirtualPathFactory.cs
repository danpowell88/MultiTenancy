using System.Web.WebPages;
using Autofac;
using Autofac.Extras.Multitenant;

namespace MultiTenant.Infrastructure
{
    public class MultiTenantVirtualPathFactory : IVirtualPathFactory
    {
        private readonly MultitenantContainer _container;

        public MultiTenantVirtualPathFactory(MultitenantContainer container)
        {
            _container = container;
        }

        public bool Exists(string virtualPath)
        {
            var engine = _container.Resolve<IVirtualPathFactory>();
            return engine.Exists(virtualPath);
        }

        public object CreateInstance(string virtualPath)
        {
            var engine = _container.Resolve<IVirtualPathFactory>();
            return engine.CreateInstance(virtualPath);
        }
    }
}
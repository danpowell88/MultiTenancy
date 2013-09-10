using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenant.Infrastructure
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
    public class PluginAssemblyAttribute : Attribute
    {
        public PluginAssemblyAttribute(string tenant, PluginAssemblyType assemblyType)
        {
            Tenant = tenant;
            AssemblyType = assemblyType;
        }

        public String Tenant { get; private set; }
        public PluginAssemblyType AssemblyType { get; set; }
    }
}

using System.Reflection;

namespace MultiTenant.Infrastructure
{
    public struct PluginAssembly
    {
        public PluginAssemblyType PluginAssemblyType { get; set; }
        public Assembly Assembly { get; set; }
    }
}
using System;

namespace MultiTenant.Infrastructure
{
    [Flags]
    public enum PluginAssemblyType
    {
        View,
        Controller
    }
}
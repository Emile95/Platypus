using System.Reflection;
using PlatypusUtils.Plugin;

namespace PlatypusUtils
{
    public static class PluginResolver
    {
        public static List<PluginType> InstanciateImplementationsFromDll<PluginType>(string dllPath, bool exceptionIfNotFound = false)
            where PluginType : class
        {
            List<PluginType> plugins = new List<PluginType>();

            Assembly assembly = LoadAssembly(dllPath);

            Type pluginType = typeof(PluginType);

            foreach (Type type in assembly.GetTypes())
            {
                if (pluginType.IsAssignableFrom(type))
                    plugins.Add(Activator.CreateInstance(type) as PluginType);
            }

            if (exceptionIfNotFound && plugins.Count == 0)
                throw new NoImplementationFoundException<PluginType>();

            return plugins;
        }

        public static PluginType InstanciateImplementationFromDll<PluginType>(string dllPath, bool exceptionIfNotFound = false)
            where PluginType : class
        {
            Assembly assembly = LoadAssembly(dllPath);

            Type pluginType = typeof(PluginType);

            foreach (Type type in assembly.GetTypes())
            {
                if (pluginType.IsAssignableFrom(type))
                    return Activator.CreateInstance(type) as PluginType;
            }

            if (exceptionIfNotFound)
                throw new NoImplementationFoundException<PluginType>();

            return null;
        }

        private static Assembly LoadAssembly(string dllFilePath)
        {
            byte[] assemblyBytes = File.ReadAllBytes(dllFilePath);
            return Assembly.Load(assemblyBytes);
        }
    }
}

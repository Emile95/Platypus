using System.Reflection;
using PlatypusUtils.Plugin;

namespace PlatypusUtils
{
    public static class PluginResolver
    {
        public static PluginType InstanciateImplementationFromFile<PluginType>(string filePath, bool exceptionIfNotFound = false)
            where PluginType : class
        {
            byte[] bytesRaw = File.ReadAllBytes(filePath);
            return InstanciateImplementationFromRawBytes<PluginType>(bytesRaw);
        }

        public static PluginType InstanciateImplementationFromRawBytes<PluginType>(byte[] rawBytes, bool exceptionIfNotFound = false)
            where PluginType : class
        {
            Assembly assembly = Assembly.Load(rawBytes);
            return InstanciateImplementationFromAssembly<PluginType>(assembly);
        }

        public static PluginType InstanciateImplementationFromAssembly<PluginType>(Assembly assembly, bool exceptionIfNotFound = false)
            where PluginType : class
        {
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
    }
}

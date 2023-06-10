namespace Utils.Plugin
{
    public class NoImplementationFoundException<PluginType> : Exception
    {
        public NoImplementationFoundException()
            :base("No implementation of type '" + typeof(PluginType) + "' found in assembly") {}
    }
}

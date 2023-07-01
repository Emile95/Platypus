using Common.Ressource;

namespace Common
{
    public static class Utils
    {
        public static string GetString(string key, params object[] args)
        {
            return String.Format(Strings.ResourceManager.GetString(key), args);
        }
    }
}

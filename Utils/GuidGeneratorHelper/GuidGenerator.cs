namespace Utils.GuidGeneratorHelper
{
    public static class GuidGenerator
    {
        public static string GenerateFromEnumerable(IEnumerable<string> existingGuids, List<string> withoutGuids = null)
        {
            Func<string, bool> predicate = withoutGuids == null ? (guid) => existingGuids.Contains(guid) : (guid) => existingGuids.Contains(guid) || withoutGuids.Contains(guid);
            string guid = null;
            do
            {
                guid = Guid.NewGuid().ToString();
            } while (predicate(guid));
            return guid;
        }
    }
}

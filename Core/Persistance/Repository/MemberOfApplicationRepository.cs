namespace Core.Persistance.Repository
{
    public abstract class MemberOfApplicationRepository
    {
        protected List<string> RemoveByApplicationGuid(string directoryPath, string applicationGuid)
        {
            string regularExpression = $"*{applicationGuid}";
            string[] paths = Directory.GetDirectories(directoryPath, regularExpression);
            List<string> guids = new List<string>();
            foreach (string path in paths)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                guids.Add(directoryInfo.Name);
                Directory.Delete(path, true);
            }
            return guids;
        }
    }
}

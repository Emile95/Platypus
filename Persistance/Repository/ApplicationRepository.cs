using Persistance.Entity;

namespace Persistance.Repository
{
    public class ApplicationRepository : Repository<ApplicationEntity, string>
    {
        public override ApplicationEntity Add(ApplicationEntity entity)
        {
            WriteApplicationDll(entity.Guid, entity.AssemblyRaw);

            return entity;
        }

        public override void Remove(string applicationGuid)
        {
            string applicationDirectoryPath = ApplicationPaths.GetApplicationDirectoryPath(applicationGuid);
            Directory.Delete(applicationDirectoryPath, true);
        }

        public override void Consume(Action<ApplicationEntity> consumer, Predicate<ApplicationEntity> condition = null)
        {
            if (Directory.Exists(ApplicationPaths.APPLICATIONSDIRECTORYPATHS) == false) return;

            string[] applicationDirectoriesPath = Directory.GetDirectories(ApplicationPaths.APPLICATIONSDIRECTORYPATHS);
            foreach (string applicationDirectoryPath in applicationDirectoriesPath)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(applicationDirectoryPath);
                string dllFilePath = ApplicationPaths.GetApplicationDllFilePath(directoryInfo.Name);

                ApplicationEntity entity = new ApplicationEntity()
                {
                    Guid = directoryInfo.Name,
                    AssemblyRaw = File.ReadAllBytes(dllFilePath),
                };

                if (condition != null && condition(entity)) consumer(entity);
            }
        }

        public override ApplicationEntity Update(ApplicationEntity entity)
        {
            if (entity.AssemblyRaw != null && entity.AssemblyRaw.Length != 0)
                WriteApplicationDll(entity.Guid, entity.AssemblyRaw);

            return entity;
        }

        private void WriteApplicationDll(string applicationGuid, byte[] assemblyRaw)
        {
            string newApplicationDllFilePath = ApplicationPaths.GetApplicationDllFilePath(applicationGuid);
            File.WriteAllBytes(newApplicationDllFilePath, assemblyRaw);
        }
    }
}

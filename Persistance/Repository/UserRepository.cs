using Newtonsoft.Json;
using Persistance.Entity;

namespace Persistance.Repository
{
    public class UserRepository
    {
        public void SaveUser(UserEntity entity)
        {
            string userDirectoryPath = ApplicationPaths.GetUserDirectoryPath(entity.ID);
            if(Directory.Exists(userDirectoryPath) == false)
                Directory.CreateDirectory(userDirectoryPath);
            string lines = JsonConvert.SerializeObject(entity);
            File.WriteAllText(ApplicationPaths.GetUserFilePathByBasePath(userDirectoryPath), lines);
        }
    }
}

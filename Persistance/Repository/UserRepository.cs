using Newtonsoft.Json;
using Persistance.Entity;

namespace Persistance.Repository
{
    public class UserRepository : MemberOfApplicationRepository
    {
        public int SaveUser(string connectionMethodGuid, UserEntity entity)
        {
            int lastID = Convert.ToInt32(File.ReadAllText(ApplicationPaths.LASTUSERIDFILEPATH));
            int newID = (lastID + 1);
            File.WriteAllText(ApplicationPaths.LASTUSERIDFILEPATH, newID.ToString());
            entity.ID = lastID;

            string userDirectoryPath = ApplicationPaths.GetUserDirectoryPath(connectionMethodGuid, entity.ID);

            if(Directory.Exists(userDirectoryPath) == false)
                Directory.CreateDirectory(userDirectoryPath);

            string userFilePath = Path.Combine(userDirectoryPath, ApplicationPaths.USERFILENAME);

            string json = JsonConvert.SerializeObject(entity);
            File.WriteAllText(userFilePath, json);

            return newID;
        }

        public UserEntity GetUserByData(string userConnectionMethodGuid, Predicate<UserEntity> predicate)
        {
            string[] userDirectories =  Directory.GetDirectories(ApplicationPaths.GetUsersByConnectionMethodDirectory(userConnectionMethodGuid));
            
            foreach(string userDirectoiresPath in userDirectories)
            {
                string userFilePath = Path.Combine(userDirectoiresPath, ApplicationPaths.USERFILENAME);
                string json = File.ReadAllText(userFilePath);
                UserEntity userEntity = JsonConvert.DeserializeObject<UserEntity>(json);

                if(predicate(userEntity))
                    return userEntity;
            }

            return null;
        }

        public void SaveUserCredentialMethod(UserCredentialMethodEntity entity)
        {
            string credentialMethodDirectoryPath = Path.Combine(ApplicationPaths.USERSDIRECTORYPATH, entity.Guid);
            if (Directory.Exists(credentialMethodDirectoryPath) == false)
                Directory.CreateDirectory(credentialMethodDirectoryPath);
            string definitionFilePath = Path.Combine(credentialMethodDirectoryPath, ApplicationPaths.DEFINITIONFILENAME);
            string json = JsonConvert.SerializeObject(entity);
            File.WriteAllText(definitionFilePath, json);
        }

        public List<string> RemoveUserCredentialMethodOfApplication(string applicationGuid)
        {
            return RemoveByApplicationGuid(ApplicationPaths.USERSDIRECTORYPATH, applicationGuid);
        }
    }
}

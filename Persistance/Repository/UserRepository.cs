using Newtonsoft.Json;
using Persistance.Entity;

namespace Persistance.Repository
{
    public class UserRepository : MemberOfApplicationRepository
    {
        public void SaveUser(UserEntity entity)
        {
            int lastID = Convert.ToInt32(File.ReadAllText(ApplicationPaths.LASTPLATYPUSUSERIDFILEPATH));
            File.WriteAllText(ApplicationPaths.LASTPLATYPUSUSERIDFILEPATH, (lastID + 1).ToString());
            entity.ID = lastID;

            string platypusUserDirectoryPath = ApplicationPaths.GetPlatypusUserDirectoryPath(entity.ID);

            if(Directory.Exists(platypusUserDirectoryPath) == false)
                Directory.CreateDirectory(platypusUserDirectoryPath);

            string platypusUserFilePath = Path.Combine(platypusUserDirectoryPath, ApplicationPaths.USERFILENAME);

            string json = JsonConvert.SerializeObject(entity);
            File.WriteAllText(platypusUserFilePath, json);
        }

        public UserEntity GetUserByCredential(string userName, string password)
        {
            string[] platypusUserDirectoires =  Directory.GetDirectories(ApplicationPaths.PLATYPUSUSERSDIRECTORYPATH);
                
            foreach(string platypusUserDirectoiresPath in platypusUserDirectoires)
            {
                string platypusUserFilePath = Path.Combine(platypusUserDirectoiresPath, ApplicationPaths.USERFILENAME);
                string json = File.ReadAllText(platypusUserFilePath);
                UserEntity userEntity = JsonConvert.DeserializeObject<UserEntity>(json);
                if(userEntity.UserName == userName &&
                   userEntity.Password == password)
                    return userEntity;
            }

            return null;
        }

        public void SaveUserCredential(int userID, Dictionary<string, object> credential)
        {
            string userDirectoryPath = ApplicationPaths.GetUserDirectoryPath(userID);
            SaveUserCredentialByBasePath(userDirectoryPath, credential);
        }

        public void SaveUserCredentialByBasePath(string basePath, Dictionary<string, object> credential)
        {
            string json = JsonConvert.SerializeObject(credential);
            File.WriteAllText(basePath, json);
        }

        public void SaveUserCredentialMethod(UserCredentialMethodEntity entity)
        {
            string credentialMethodDirectoryPath = Path.Combine(ApplicationPaths.USERCREDENTIALMETHODSDIRECTORYPATH, entity.Guid);
            if (Directory.Exists(credentialMethodDirectoryPath) == false)
                Directory.CreateDirectory(credentialMethodDirectoryPath);
            string definitionFilePath = Path.Combine(credentialMethodDirectoryPath, ApplicationPaths.DEFINITIONFILENAME);
            string json = JsonConvert.SerializeObject(entity);
            File.WriteAllText(definitionFilePath, json);
        }

        public List<string> RemoveUserCredentialMethodOfApplication(string applicationGuid)
        {
            return RemoveByApplicationGuid(ApplicationPaths.USERCREDENTIALMETHODSDIRECTORYPATH, applicationGuid);
        }
    }
}

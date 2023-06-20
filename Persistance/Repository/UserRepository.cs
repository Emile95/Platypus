﻿using Newtonsoft.Json;
using Persistance.Entity;

namespace Persistance.Repository
{
    public class UserRepository
    {
        public string SaveUser(UserEntity entity)
        {
            string userDirectoryPath = ApplicationPaths.GetUserDirectoryPath(entity.ID);
            if(Directory.Exists(userDirectoryPath) == false)
                Directory.CreateDirectory(userDirectoryPath);
            string json = JsonConvert.SerializeObject(entity);
            File.WriteAllText(ApplicationPaths.GetUserFilePathByBasePath(userDirectoryPath), json);
            return userDirectoryPath;
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
            string regularExpression = $"*{applicationGuid}";
            string[] actionOfApplicationDirectoryPaths = Directory.GetDirectories(ApplicationPaths.USERCREDENTIALMETHODSDIRECTORYPATH, regularExpression);
            List<string> actionGuidss = new List<string>();
            foreach (string actionOfApplicationDirectoryPath in actionOfApplicationDirectoryPaths)
            {
                DirectoryInfo actionDirectoryInfo = new DirectoryInfo(actionOfApplicationDirectoryPath);
                actionGuidss.Add(actionDirectoryInfo.Name);
                Directory.Delete(actionOfApplicationDirectoryPath, true);
            }
            return actionGuidss;
        }
    }
}

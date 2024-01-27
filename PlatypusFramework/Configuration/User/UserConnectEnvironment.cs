﻿using Persistance.Entity;
namespace PlatypusFramework.Configuration.User
{
    public class UserConnectEnvironment<CredentialType>
    {
        public List<UserEntity> Users { get; set; }
        public UserEntity CorrespondingUser { get; set; }
        public string LoginAttemptMessage { get; set; }
        public CredentialType Credential { get; set; }
    }
}
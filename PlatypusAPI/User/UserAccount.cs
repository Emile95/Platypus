﻿namespace PlatypusAPI.User
{
    public class UserAccount
    {
        public int ID { get; private set; }
        public string Name { get; private set; }

        public UserAccount(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}

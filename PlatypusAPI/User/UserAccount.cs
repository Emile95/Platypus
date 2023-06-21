namespace PlatypusAPI.User
{
    public class UserAccount
    {
        public int ID { get; private set; }

        public UserAccount(int id)
        {
            ID = id;
        }
    }
}

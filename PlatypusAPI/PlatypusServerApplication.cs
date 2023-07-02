using PlatypusAPI.ApplicationAction.Run;
using PlatypusAPI.User;

namespace PlatypusAPI
{
    public class PlatypusServerApplication
    {
        private readonly PlatypusClientSocketHandler _socketHandler;
        public UserAccount ConnectedUser { get; private set; }

        public PlatypusServerApplication(
            PlatypusClientSocketHandler socketHandler,
            UserAccount connectedUser
        )
        {
            _socketHandler = socketHandler;
            ConnectedUser = connectedUser;
        }

        public void Disconnect()
        {

        }

        public ApplicationActionRunResult RunApplicationAction(ApplicationActionRunParameter applicationActionRunparameter)
        {
            _socketHandler.SendToServer(Common.Utils.GetBytesFromObject(applicationActionRunparameter));

            if(applicationActionRunparameter.Async == false)
            {

            }

            return null;
        }
    }
}

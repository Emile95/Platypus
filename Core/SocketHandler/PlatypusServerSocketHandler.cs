using Common.SocketHandler;
using Common.SocketHandler.State;
using Core.ApplicationAction;
using Utils.GuidGeneratorHelper;

namespace Core.Sockethandler
{
    public class PlatypusServerSocketHandler : ServerSocketHandler<string>
    {
        private readonly ApplicationActionsHandler _applicationActionsHandler;
        private readonly int _port;

        public PlatypusServerSocketHandler(
            string protocol,
            int port,
            ApplicationActionsHandler applicationActionsHandler
        ) : base(protocol)
        {
            _applicationActionsHandler = applicationActionsHandler;
            _port = port;
        }

        protected override string GenerateClientKey(List<string> currentKeys)
        {
            return GuidGenerator.GenerateFromEnumerable(currentKeys);
        }

        public override void OnAccept(ClientReceivedState<string> receivedState)
        {
            Console.WriteLine($"new client connected, key='{receivedState.ClientKey}'");
        }

        public override void OnLostSocket(ClientReceivedState<string> receivedState)
        {
            Console.WriteLine($"client lost, key='{receivedState.ClientKey}'");
        }

        public override void OnReceive(ClientReceivedState<string> receivedState)
        {
            
        }

        public void Initialize(string host)
        {
            Initialize(_port, host);
        }
    }
}

using Common.SocketHandler;
using Common.SocketHandler.State;
using Core.ApplicationAction;
using Utils.GuidGeneratorHelper;

namespace Core.Sockethandler
{
    internal class PlatypusTCPClientSocketsHandler : ServerSocketHandler<string>
    {
        private readonly ApplicationActionsHandler _applicationActionsHandler;

        public PlatypusTCPClientSocketsHandler(
            ApplicationActionsHandler applicationActionsHandler
        ) : base("tcp")
        {
            _applicationActionsHandler = applicationActionsHandler;
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
    }
}

﻿using Common.SocketHandler.State;
using Common.SocketHandler.Tcp;
using Core.ApplicationAction;
using Utils.GuidGeneratorHelper;

namespace Core
{
    internal class PlatypusTCPClientSocketsHandler : TCPServerSocketHandler<string>
    {
        private readonly ApplicationActionsHandler _applicationActionsHandler;

        public PlatypusTCPClientSocketsHandler(
            ApplicationActionsHandler applicationActionsHandler
        )
        {
            _applicationActionsHandler = applicationActionsHandler;
        }

        protected override string GenerateClientKey(List<string> currentKeys)
        {
            return GuidGenerator.GenerateFromEnumerable(currentKeys);
        }

        protected override void OnAccept(ClientReceivedState<string> receivedState)
        {
            Console.WriteLine($"new client connected, key='{receivedState.ClientKey}'");
        }

        protected override void OnLostSocket(ClientReceivedState<string> receivedState)
        {
            Console.WriteLine($"client lost, key='{receivedState.ClientKey}'");
        }

        protected override void OnReceive(ClientReceivedState<string> receivedState)
        {
            
        }
    }
}

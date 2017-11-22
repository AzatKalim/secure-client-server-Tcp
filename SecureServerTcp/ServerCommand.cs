using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace SecureServerTcp
{
    class ServerCommand : ServerSocket
    {
        public delegate void ServerCommandHandlerForServer();

        public event ServerCommandHandlerForServer EventHandlersListForServer;

        public ServerCommand(int port = 7777)
        {
            this.port = port;
            Listener = new TcpListener(IPAddress.Any, port);
            clients = new List<ClientSocket>();
            newClientsQueue = new Queue<TcpClient>();
            EventHandlersListForServerCommand += newClientHendler;
            Start();
        }

        public void newClientHendler()
        {
            EventHandlersListForServer();
        }

        public ClientCommand AcceptClientCommand()
        {
            ClientCommand newClientCommand = new ClientCommand(newClientsQueue.Dequeue());
            clients.Add(newClientCommand);
            return newClientCommand;
        }
    }
}

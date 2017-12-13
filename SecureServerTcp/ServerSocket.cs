using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace SecureServerTcp
{
    class ServerSocket
    {
        protected TcpListener Listener;

        protected List<ClientSocket> clients;

        protected int port;

        public delegate void ServerSocketHandlerForServerCommand();

        public event ServerSocketHandlerForServerCommand EventHandlersListForServerCommand;

        protected Queue<TcpClient> newClientsQueue;

        Thread accept_new_clients;

        public ServerSocket()
        {
        }

        public ServerSocket(int port = 2000)
        {
            this.port = port;
            Listener = new TcpListener(IPAddress.Any, port);
            clients = new List<ClientSocket>();
            newClientsQueue = new Queue<TcpClient>();
        }

        ~ServerSocket()
        {
            Listener.Stop();
        }

        public void Start()
        {
            Listener.Start();
            accept_new_clients = new Thread(new ThreadStart(AcceptTcpClient));
            accept_new_clients.Start();
        }

        public void Disconnect()
        {
            accept_new_clients.Abort();
            //accept_new_clients.Join();
          
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Disconnect();
            }
            Listener.Stop();
        }

        public void DeleteClient(ClientSocket cl)
        {
            cl.Disconnect();
            clients.Remove(cl);
        }

        private void AcceptTcpClient()
        {
            while (true)
            {
                TcpClient temp = Listener.AcceptTcpClient();
                lock (newClientsQueue)
                {
                    newClientsQueue.Enqueue(temp);
                }
                EventHandlersListForServerCommand();
            }
        }

        public void ReceiveMessage(ClientSocket cl)
        {
            String temp = cl.ReadBuffer();
            Console.WriteLine(temp);
            cl.SendMessage(temp);
        }
    }
}

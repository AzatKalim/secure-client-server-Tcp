using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commands;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace SecureClientTcp
{
    class ClientCommand : ClientSocket
    {
        public static int ID;

        public int id;

        public int countCommand;

        Queue<BaseCommand> bufferCommand;

        StringBuilder bufferMessage;

        StringBuilder sendBuffer;

        public delegate void ClientCommandHandlerForServer();

        public event ClientCommandHandlerForServer ClientCommandHandlersListForServer;

        public ClientCommand(string ipAdress,int port) : base (ipAdress,port)
        {
            id = ID;
            ID++;
            clientState = ClientState.CONNECTED;
            stream = new NetworkStream(socket.Client);
            bytesCount = 0;
            bufferCommand = new Queue<BaseCommand>();
            bufferMessage = new StringBuilder();
            sendBuffer = new StringBuilder();
            EventHandlersListForClientCommand += NewMessageHandler;
            work_thread = new Thread(new ThreadStart(ReadMessage));
            work_thread.Start();

        }

        public void NewMessageHandler(ClientSocket clientSocket)
        {
            bufferMessage.Append(clientSocket.ReadBuffer());

            if (bufferMessage.Length != 0)
            {
                int id = (int)bufferMessage[0];
                int messageLength = (int)bufferMessage[1];

                if (messageLength <= (bufferMessage.Length - 2))
                {
                    String temp = bufferMessage.ToString(2, messageLength);
                    BaseCommand serializedCommand = BaseCommand.Deserialize(id, temp);
                    lock (bufferCommand)
                    {
                        bufferCommand.Enqueue(serializedCommand);
                    }
                    countCommand++;
                    bufferMessage.Remove(0, messageLength + 2);
                }
            }
            if (ClientCommandHandlersListForServer != null)
            {
                ClientCommandHandlersListForServer();
            }
        }

        public BaseCommand[] GetCommands()
        {
            if (bufferCommand.Count > 0)
            {
                BaseCommand[] temp = new BaseCommand[bufferCommand.Count];
                for (int i = 0; i < bufferCommand.Count; i++)
                {
                    temp[i] = bufferCommand.Dequeue();
                    Console.WriteLine("Get({0}): {1} on {2}", id, temp[i], DateTime.Now.ToShortTimeString());

                }
                return temp;
            }
            else
            {
                return null;
            }
        }

        public bool SendCommand(BaseCommand command)
        {
            Console.WriteLine("Sent({0}): {1} on {2}", id, command, DateTime.Now.ToShortTimeString());
            lock (sendBuffer)
            {
                sendBuffer.Clear();
                string newMessage = command.Serialize();
                sendBuffer.Append((char)command.id);
                sendBuffer.Append((char)newMessage.Length);
                sendBuffer.Append(newMessage);
                countCommand++;
                return SendMessage(sendBuffer.ToString());
            }
        }
    }
}

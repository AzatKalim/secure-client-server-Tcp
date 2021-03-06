﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commands;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Encription;

namespace SecureServerTcp
{
    class ClientCommand:ClientSocket
    {
        public static int ID;

        public int id;

        public int countCommand;

        Queue<BaseCommand> bufferCommand;

        StringBuilder bufferMessage;

        StringBuilder sendBuffer;

        public RC4 encription;

        public delegate void ClientCommandHandlerForServer(ClientCommand client);

        public event ClientCommandHandlerForServer ClientCommandHandlersListForServer;

        public ClientCommand(TcpClient client)
        {
            id = ID;
            ID++;
            clientState = ClientState.CONNECTED;
            this.socket = client;
            buffer = new byte[2048];
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
                    if (id != 10 && id != 5)
                    {
                        temp=encription.Decode(temp);
                    }
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
                ClientCommandHandlersListForServer(this);
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
                if (command.id != 10 && command.id != 5)
                {
                    newMessage=encription.Encode(newMessage);
                }
                sendBuffer.Append((char)command.id);
                sendBuffer.Append((char)newMessage.Length);
                sendBuffer.Append(newMessage);
                countCommand++;
                return SendMessage(sendBuffer.ToString());
            }
        }
    }
}

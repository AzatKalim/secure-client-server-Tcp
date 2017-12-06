using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace SecureClientTcp
{
    delegate void ClientEvent(ClientSocket cl);

    public enum ClientState
    {
        CONNECTED,
        DISCONNECTED,
        ERROR,
        Autorized,
        KeysExchanged
    }
    class ClientSocket
    {
        public delegate void ClientSocketHandler(ClientSocket client);

        protected event ClientSocketHandler EventHandlersListForClientCommand;

        protected NetworkStream stream;

        protected byte[] buffer;

        protected int bytesCount;

        protected Thread work_thread;

        public ClientState clientState
        {
            protected set;
            get;
        }

        protected TcpClient socket;

        public ClientSocket()
        {
        }

        protected  ClientSocket(TcpClient client)
        {
            clientState = ClientState.CONNECTED;
            socket = client;
            socket.NoDelay = true;
            buffer = new byte[2048];
            stream = new NetworkStream(socket.Client);
            bytesCount = 0;
            work_thread = new Thread(new ThreadStart(ReadMessage));
            work_thread.Start();
        }

        protected ClientSocket(string ipAdress, int port)
        {
            socket = new TcpClient();
            // Соединяем сокет с удаленной точкой
            socket.Connect(ipAdress, port);
            socket.NoDelay = true;
            buffer = new byte[2048];
            stream = new NetworkStream(socket.Client);
            bytesCount = 0;
            clientState = ClientState.CONNECTED;
            work_thread = new Thread(new ThreadStart(ReadMessage));
            work_thread.Start();

        }

        public bool SendMessage(string message)
        {
            try
            {
                if (clientState == ClientState.CONNECTED)
                {
                    byte[] myWriteBuffer = Encoding.Unicode.GetBytes(message);
                    stream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            catch (Exception)
            {
                clientState = ClientState.ERROR;
                return false;
            }
        }

        protected void ReadMessage()
        {
            try
            {
                byte[] tempbuffer = new byte[1024];
                while (true && clientState == ClientState.CONNECTED)
                {
                    bytesCount = stream.Read(tempbuffer, 0, tempbuffer.Length);
                    if (bytesCount == 0)
                    {
                        clientState = ClientState.ERROR;
                        return;
                    }
                    lock (buffer)
                    {
                        for (int i = 0; i < bytesCount; i++)
                        {
                            buffer[i] = tempbuffer[i];
                        }
                    }

                    EventHandlersListForClientCommand(this);
                }

            }
            catch (Exception)
            {
                clientState = ClientState.ERROR;
            }
        }

        public string ReadBuffer()
        {
            StringBuilder message = new StringBuilder();
            lock (buffer)
            {
                message.Append(Encoding.Unicode.GetString(buffer, 0, bytesCount));
                bytesCount = 0;
            }

            return message.ToString();
        }

        public void Disconnect()
        {
            clientState = ClientState.DISCONNECTED;
            work_thread.Abort();
            work_thread.Join();
            socket.Close();
        }

        public String getIP()
        {
            try
            {
                if (clientState == ClientState.CONNECTED || clientState == ClientState.ERROR)
                {
                    return this.socket.Client.RemoteEndPoint.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                clientState = ClientState.ERROR;
                return null;
            }
        }

        ~ClientSocket()
        {
        }
    }
}

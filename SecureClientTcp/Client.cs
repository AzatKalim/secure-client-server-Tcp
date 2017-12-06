﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using System.Numerics;
using Commands;



namespace SecureClientTcp
{
    enum state
    {
        AUTHORIZED,
        REGISTERED,
        CONNECTED,
        ERROR,
        DISCONNETED
    }
    class Client
    {
        ClientCommand clientCommand;

        static SHA1 hashFunction;

        Queue<BaseCommand> arrivedCommands;

        Thread workThread;

        state status;

        BigInteger callAnswer;

        ManualResetEvent callGetEvent;

        public delegate void ClientDisconnect();

        public delegate void ClientStatusChanged(bool result);

        public delegate void ClientNewMessageArriver(string sender,string text);

        public event ClientStatusChanged ClientAuthorizedHandlersListForUI;

        public event ClientStatusChanged ClientRegistredHandlerListForUI;

        public event ClientNewMessageArriver ClientMessageHandlerListForUI;

        public event ClientDisconnect ClientDisconnectHandlerListForUI;
      
        public Client(string ipAdress="localhost",int port=11000)
        {
            arrivedCommands = new Queue<BaseCommand>();
            clientCommand = new ClientCommand(ipAdress, port);
            clientCommand.ClientCommandHandlersListForServer += NewCommandReacion;
            hashFunction = SHA1Managed.Create();
            callGetEvent = new ManualResetEvent(false);
            workThread= new Thread(new ThreadStart(ClientRoutine));
            workThread.Start();
        }

        public void ClientRoutine()
        {
            while (status != state.ERROR)
            {
                Thread.Sleep(100);

                //обработка пришедших комманд

                if (arrivedCommands.Count > 0)
                {
                    BaseCommand temp;
                    lock (arrivedCommands)
                    {
                        temp = arrivedCommands.Dequeue();
                    }
                    CommandReacion(temp);
                }
            }
        }

        public void NewCommandReacion()
        {
            BaseCommand[] commands = clientCommand.GetCommands();
            //извлечение команд из буффера
            lock (arrivedCommands)
            {
                foreach (var item in commands)
                {
                    arrivedCommands.Enqueue(item);
                }
                
            }
        }

        void CommandReacion(BaseCommand command)
        {
            switch (command.id)
            {
                case 2://Команда CallAnswer
                    {
                        Reaction(command as RegistratioAnswer);
                    }
                    break;
                case 4://Команда CallAnswer
                    {
                        Reaction(command as CallAnswer);
                    }
                    break;
                case 5://Команда KeyExchange
                    {
                        Reaction(command  as KeysExchange);
                    }
                    break;
                case 6://Команада Chat
                    {
                        Reaction(command  as Chat);
                    }
                    break;
                case 7://Команада Disconnect
                    {
                        Reaction(command  as Stop);
                    }
                    break;
                case 8://Команда Autentification
                    {
                        Reaction(command  as AutentificationAnswer);
                    }
                    break;
                default:
                    Console.WriteLine("Unnown command");
                    break;
            }
        }

        void Reaction(CallAnswer command)
        {
            callAnswer=BigInteger.Parse(command.answer);
            callGetEvent.Set();
        }

        void Reaction(RegistratioAnswer command)
        {
            if(command.answer==true)
            {
                status= state.REGISTERED;
                if(ClientRegistredHandlerListForUI!=null)
                {
                    ClientRegistredHandlerListForUI(true);
                }
            }
            else
            {
                if(ClientRegistredHandlerListForUI!=null)
                {
                    ClientRegistredHandlerListForUI(false);
                }
            }
        }

        void Reaction(AutentificationAnswer command)
        {
            if(command.answer)
            {
                status= state.AUTHORIZED;
                if(ClientAuthorizedHandlersListForUI!=null)
                {
                    ClientAuthorizedHandlersListForUI(true);
                }
            }
            else
            {
                if(ClientAuthorizedHandlersListForUI!=null)
                {
                    ClientAuthorizedHandlersListForUI(false);
                }
            }
        }

        void Reaction (KeysExchange command)
        {

        }

        void Reaction (Chat chatCommand)
        {
            if(ClientMessageHandlerListForUI!=null)
            {
                ClientMessageHandlerListForUI(chatCommand.sender,chatCommand.text);
            }
        }

        void Reaction(Stop stopCommand)
        {
            status= state.DISCONNETED;
            workThread.Abort();
            if(ClientDisconnectHandlerListForUI!=null)
            {
                ClientDisconnectHandlerListForUI();
            }
        }

        public void Autentification(string login,string password)
        {
            clientCommand.SendCommand(new Call(login));
            callGetEvent.WaitOne();
            callGetEvent.Reset();
            clientCommand.SendCommand(new Registration(login,ComputeHash(password + callAnswer.ToString())));
        }

        public void Registration(string login, string password)
        {
            clientCommand.SendCommand(new Registration(login,password));
        }


        private string ComputeHash(string message)
        {
            Byte[] msgByteArr = Encoding.ASCII.GetBytes(message);
            Byte[] hashArray = hashFunction.ComputeHash(msgByteArr);
            return Encoding.ASCII.GetString(hashArray);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using System.Numerics;
using Commands;
using Encription;


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
        #region Params 

        ClientCommand clientCommand;

        static SHA1 hashFunction;

        Queue<BaseCommand> arrivedCommands;

        Thread workThread;

        state status;

        BigInteger callAnswer;

        ManualResetEvent callGetEvent;

        ManualResetEvent keysGetEvent;

        Encoding enc = Encoding.Default;

        #endregion

        #region Delegates

        public delegate void ClientDisconnect();

        public delegate void ClientStatusChanged(bool result);

        public delegate void ClientNewMessageArriver(string sender,string text);

        public event ClientStatusChanged ClientAuthorizedHandlersListForUI;

        public event ClientStatusChanged ClientRegistredHandlerListForUI;

        public event ClientNewMessageArriver ClientMessageHandlerListForUI;

        public event ClientDisconnect ClientDisconnectHandlerListForUI;

        #endregion 

        #region Construtors

        public Client(string ipAdress="localhost",int port=11000)
        {
            arrivedCommands = new Queue<BaseCommand>();
            clientCommand = new ClientCommand(ipAdress, port);
            clientCommand.ClientCommandHandlersListForServer += NewCommandReacion;
            hashFunction = SHA1Managed.Create();
            callGetEvent = new ManualResetEvent(false);
            keysGetEvent = new ManualResetEvent(false);
            workThread= new Thread(new ThreadStart(ClientRoutine));
            workThread.Start();
        }

        public void KeyExchangeStart()
        {
            clientCommand.encription= new RC4();
            clientCommand.SendCommand(new PreKeyExchange(clientCommand.encription.q.ToString(),
                                                        clientCommand.encription.field.ToString(),
                                                        clientCommand.encription.GenerateParam().ToString()));

        }

        #endregion 

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

        #region Reaction on new commands

        void NewCommandReacion()
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
            Type type=BaseCommand.ReturnTypeOfCommand(command.id);
            switch (command.id)
            {
                case 2://Команда RegistrationAnswer
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
                case 9://Команда Autentification
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
            if (command.sender == "server")
            {
                clientCommand.encription.SetKey(command.param);
            }
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

        #endregion 

        #region Public Methods for User

        public void Autentification(string login,string password)
        {
            KeyExchangeStart();
            clientCommand.SendCommand(new Call(login));
            callGetEvent.WaitOne();
            callGetEvent.Reset();
            if (callAnswer == -1)
            {
                ClientAuthorizedHandlersListForUI(false);
                return;
            }
            clientCommand.SendCommand(new Autentification(login,ComputeHash(password,callAnswer)));
        }

        public void Registration(string login, string password)
        {
            KeyExchangeStart();
            clientCommand.SendCommand(new Registration(login,password));
        }

        public bool SendMessage(string name,string message)
        {
            return clientCommand.SendCommand(new Chat(name,message));

        }
        #endregion 

        #region Secondary functions 

        private byte[] ComputeHash(string message, BigInteger salt)
        {
            Byte[] msgByteArr = enc.GetBytes(message + salt.ToString());
            Byte[] hashArray = hashFunction.ComputeHash(msgByteArr);
            return hashArray;
        }

        #endregion 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Commands;
using System.Security.Cryptography;
using System.Numerics;
using Encription;

namespace SecureServerTcp
{
    public enum status
    {
        on,
        off
    };

    class Server
    {
        public struct CommandList
        {
            public ClientCommand client;

            public BaseCommand[] commands;

            public CommandList(ClientCommand clientCommand)
            {
                client = clientCommand;
                commands = clientCommand.GetCommands();
            }
        }

        #region Constatants 

        const int CALL_ANSWER_LENGTH = 128;

        const int SALT_LENGTH = 100;

        #endregion

        Thread workThread;

        ServerCommand serverCommand;

        Queue<CommandList> commandsAndClients;

        bool clientsChanged;

        Dictionary<int, ClientCommand> listOfIDAndClientCommands;

        Dictionary<int, string> listOfIDAndNames;

        public status currentStatus;

        static SHA1 hashFunction;

        static int counter;

        securityEntities context;

        Encoding enc = Encoding.Default;

        public Server(int port = 11000)
        {
            //иницилизация 
            listOfIDAndNames = new Dictionary<int, string>();
            listOfIDAndClientCommands = new Dictionary<int, ClientCommand>();
            commandsAndClients = new Queue<CommandList>();
            currentStatus = status.on;
            clientsChanged = false;
            hashFunction = SHA1Managed.Create();
            //запуск сервера команд
            serverCommand = new ServerCommand(port);
            serverCommand.EventHandlersListForServer += AddClientCommand;
            serverCommand.Start();
            //поток для выполнения рутины
            workThread = new Thread(new ThreadStart(ServerRoutine));
            workThread.Start();
            context = new securityEntities();
        }

        ~Server()
        {
            Stop();
            workThread.Abort();
            workThread.Join();
        }

        //операции выполняемые сервером постоянно
        void ServerRoutine()
        {
            while (currentStatus != status.off)
            {
                //задержка между проходами сервера 
                Thread.Sleep(100);

                //обработка пришедших комманд

                while (commandsAndClients.Count > 0)
                {
                    CommandList temp;
                    lock (commandsAndClients)
                    {
                        temp = commandsAndClients.Dequeue();
                    }
                    CommandReacion(temp);
                }

                lock (listOfIDAndClientCommands)
                {
                    // проверка на отлючившихся клиентов из-за ошибки
                    for (int i = 0; i < listOfIDAndClientCommands.Count; i++)
                    {
                        if (listOfIDAndClientCommands[i].clientState == ClientState.ERROR)
                        {
                            ClientCommand client = listOfIDAndClientCommands[i];
                            DeleteClient(client);
                            i--;
                        }
                    }
                }
            }            
        }

        public void NewCommandReacion(ClientCommand client)
        {
            //извлечение команды из буффера
            lock (commandsAndClients)
            {
                commandsAndClients.Enqueue(new CommandList(client));
            }
        }

        //реакции на команды от игроков
        void CommandReacion(CommandList commandList)
        {
            foreach (BaseCommand baseCommand in commandList.commands)
            {
                switch (baseCommand.id)
                {
                    case 1://Команда Call
                        {
                            Reaction(baseCommand as Registration, commandList.client);
                        }
                        break;
                    case 3://Команда Call
                        {
                            Reaction(baseCommand as Call, commandList.client);
                        }
                        break;
                    case 5://Команда KeyExchange
                        {
                            Reaction(baseCommand as KeysExchange, commandList.client);
                        }
                        break;
                    case 6://Команада Chat
                        {
                            Reaction(baseCommand as Chat, commandList.client);
                        }
                        break;
                    case 7://Команада Disconnect
                        {
                            Reaction(baseCommand as Stop,commandList.client);
                        }
                        break;
                    case 8://Команда Autentification
                        {
                            Reaction(baseCommand as Autentification, commandList.client);
                        }
                        break;
                    case 10:
                        {
                            Reaction(baseCommand as PreKeyExchange, commandList.client);
                        }
                        break;
                    default:
                        Console.WriteLine("Unnown command");
                        break;
                }
            }
        }

        void Reaction(Registration command, ClientCommand client)
        {
            foreach (var user in context.users)
            {
                if (user.login == command.login)
                {
                    client.SendCommand(new RegistratioAnswer(false));
                }
            }
            BigInteger newsalt = GenerateSalt();

            var us = new users()
            {
                id = counter,
                login = command.login,
                password_hash = ComputeHash(command.password, newsalt),
                salt = newsalt.ToString(),
            };
            context.users.Add(us);
            context.SaveChanges();
            Console.WriteLine("Пользователь {0}, зарегистрерирован", command.login);
            client.SendCommand(new RegistratioAnswer(true));
        }

        void Reaction(Call call, ClientCommand client)
        {
            foreach (var user in context.users)
            {
                if (user.login == call.login)
                {
                    Console.WriteLine("Пользователь {0}, получил ответ", call.login);
                    client.SendCommand(new CallAnswer(user.salt));
                    return;
                }
            }
            Console.WriteLine("Пользователь {0},не получил ответ", call.login);
            client.SendCommand(new CallAnswer("-1"));
        }

        void Reaction(Chat command, ClientCommand client)
        {
            Console.WriteLine("Chat: sender {0}, message {1}", command.sender, command.text);
            foreach (var item in listOfIDAndClientCommands)
            {
                item.Value.SendCommand(command);
            }
        }

        void Reaction(Autentification command, ClientCommand client)
        {
            var context = new securityEntities();
            foreach (var user in context.users)
            {
                if (user.login == command.login)
                {
                    Console.WriteLine(enc.GetString(command.passwordHash));
                    Console.WriteLine(user.password_hash);
                }
                if (user.login == command.login && user.password_hash == enc.GetString(command.passwordHash))
                {
                    clientsChanged = true;
                    listOfIDAndNames.Add(client.id, command.login);
                    listOfIDAndClientCommands.Add(client.id, client);
                    Console.WriteLine("Пользователь {0} прошел аутентификацию",command.login);
                    client.SendCommand(new AutentificationAnswer(true));
                    return;
                }
            }
            Console.WriteLine("Пользователь {0}  не прошел аутентификацию", command.login);
            client.SendCommand(new AutentificationAnswer(false));

        }

        void Reaction(Stop command, ClientCommand client)
        {
            client.Disconnect();
            DeleteClient(client);
        }

        void Reaction(KeysExchange command, ClientCommand client)
        {
        }

        void Reaction(PreKeyExchange command, ClientCommand client)
        {
            client.encription = new RC4(command.q, command.n);
            client.SendCommand( new KeysExchange("server",client.encription.GenerateParam().ToString()));
            client.encription.SetKey(command.param);
            Console.WriteLine("Key exchange!");
        }
        //реакция на на отключение
        void PlayerDisconnectReaction(ClientCommand client)
        {
            DeleteClient(client);
        }

        //остановка работы сервера 
        public void Stop()
        {
            currentStatus = status.off;
            serverCommand.Disconnect();
        }

        void AddClientCommand()
        {
            ClientCommand temp = serverCommand.AcceptClientCommand();
            temp.ClientCommandHandlersListForServer += NewCommandReacion;
        }

        //удаление игрока 
        void DeleteClient(ClientCommand client)
        {         
            listOfIDAndClientCommands.Remove(client.id);
            listOfIDAndNames.Remove(client.id);
            serverCommand.DeleteClient(client as ClientSocket);
        }
       
        void SendToAllPlayers(BaseCommand command)
        {
            foreach (KeyValuePair<int, ClientCommand> player in listOfIDAndClientCommands)
            {
                player.Value.SendCommand(command);
            }
        }

        // вспомогательные методы для поиска игроков в списках
        ClientCommand FindClentCommand(int id)
        {
            ClientCommand client = null;
            foreach (KeyValuePair<int, ClientCommand> player in listOfIDAndClientCommands)
            {
                if (id == player.Key)
                {
                    client = player.Value;
                }
            }
            return client;
        }

        String FindName(int id)
        {
            string result = null;
            foreach (KeyValuePair<int, string> player in listOfIDAndNames)
            {
                if (id == player.Key)
                {
                    result = player.Value;
                }
            }
            return result;
        }

        int FindId(string ob)
        {
            int result = -1;

            foreach (KeyValuePair<int, string> player in listOfIDAndNames)
            {

                if (String.Compare(ob as String, player.Value) == 0)
                {
                    result = player.Key;
                }
            }
            return result;
        }

        int FindId(ClientCommand ob)
        {
            int result = -1;

            foreach (KeyValuePair<int, ClientCommand> player in listOfIDAndClientCommands)
            {
                if (ob == player.Value)
                {
                    result = player.Key;
                }
            }
            return result;
        }

        private BigInteger GenerateSalt()
        {
            return RC4.RandomNum(SALT_LENGTH);
        }

        private string ComputeHash(string message, BigInteger salt)
        {
            Byte[] msgByteArr = enc.GetBytes(message + salt.ToString());
            Byte[] hashArray = hashFunction.ComputeHash(msgByteArr);
            return enc.GetString(hashArray);
        }
    }
}

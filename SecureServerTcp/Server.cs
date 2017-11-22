using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Commands;

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

        DateTime StartTime;

        Thread workThread;

        ServerCommand serverCommand;

        Queue<CommandList> commandsAndClients;

        bool clientsChanged;

        Dictionary<int, ClientCommand> listOfIDAndClientCommands;

        Dictionary<int, string> listOfIDAndNames;

        public status currentStatus;

        public Server(int port = 7777)
        {
            //иницилизация 
            listOfIDAndNames = new Dictionary<int, string>();
            listOfIDAndClientCommands = new Dictionary<int, ClientCommand>();
            commandsAndClients = new Queue<CommandList>();
            currentStatus = status.on;
            StartTime = DateTime.Now;
            clientsChanged = false;
            //запуск сервера команд
            serverCommand = new ServerCommand(port);
            serverCommand.EventHandlersListForServer += AddClientCommand;
            serverCommand.Start();
            //поток для выполнения рутины
            workThread = new Thread(new ThreadStart(ServerRoutine));
            workThread.Start();
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
                Thread.Sleep(10);

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
                            IntroReaction(baseCommand as Call, commandList.client);
                        }
                        break;
                    case 4://Команда Chat
                        {
                            Chat chatCommand = baseCommand as Chat;
                            ChatReaction(chatCommand, commandList.client);
                        }
                        break;
                    case 10://Команада Move
                        {
                           
                        }
                        break;
                    case 12://Команада Disconnect
                        {
                            PlayerDisconnectReactio(commandList.client);
                        }
                        break;
                    default:
                        Console.WriteLine("Unnown command");
                        break;
                }
            }
        }


        bool CallReaction(Call call, ClientCommand client)
        {
            var context = new securityEntities();

            foreach (var user in context.users)
            {
                if (user.login == login)
                {
                    Console.WriteLine("Пользователь {0}, получил ответ", login);
                    currentUser = user;
                    return user.salt;
                }
            }
            Console.WriteLine("Пользователь {0},не получил ответ", login);
            return "-1";
        }
        //реакция на Intro
        bool IntroReaction(Intro command, ClientCommand client)
        {
            Response answer;
            //если игра не в фазе ожидания
            if (gameData.currentStatus != GameData.GameStatus.waiting)
            {
                //отправка сообщени об ошибке 
                answer = new Response("notok", "ERROR.Game already starded");
                client.SendCommand(answer as BaseCommand);
                //отсоединение игрока
                client.Disconnect();
                serverCommand.DeleteClient(client);

                return false;
            }

            bool nameAlreadyInGame = false;
            //проверка , есть уже грок с таким именем
            foreach (KeyValuePair<int, string> player in listOfIDAndNames)
            {
                if (player.Value == command.name)
                {
                    nameAlreadyInGame = true;
                    break;
                }
            }
            //если уже есть
            if (nameAlreadyInGame)
            {
                answer = new Response("notok", "ERROR.User with this name have already entered");
                client.SendCommand(answer as BaseCommand);
                //отсоединение игрока
                client.Disconnect();
                serverCommand.DeleteClient(client);
                return false;
            }
            else
            {
                //добавление игрока
                answer = new Response("ok", "welcome, " + command.name);
                client.SendCommand(answer as BaseCommand);
                listOfIDAndNames.Add(client.id, command.name);
                listOfIDAndClientCommands.Add(client.id, client);
                gameData.AddPlayer(command.name);
                playersChanged = true;
            }
            return true;
        }

        //реакция на на отключение
        void PlayerDisconnectReaction(ClientCommand client)
        {
            playersChanged = true;
            DeleteClient(client);
        }

        //реакция на Chat
        void Reaction(Chat chatCommand, ClientCommand client)
        {
            string name = FindName(client.id);
            //если имя не найдено => игрок не авторизован
            if (name == null)
            {
                client.SendCommand(new Chat("Server", "ERROR.Cant use chat without authorization"));
                return;
            }
            SendToAllPlayers(new Chat(name, chatCommand.text));
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
            gameData.DeletePlayer(FindName(client.id));
            listOfIDAndClientCommands.Remove(client.id);
            listOfIDAndNames.Remove(client.id);
            serverCommand.DeleteClient(client as ClientSocket);
            playersChanged = true;
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
    }
}

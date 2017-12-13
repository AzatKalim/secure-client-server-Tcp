using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
namespace SecureServerTcp
{
    class Program
    {
        static void Main(string[] args)
        {
            bool flag = true;
            Server server=null;
            while (flag)
            {
                Console.WriteLine("список команд:");
                Console.WriteLine("start");
                Console.WriteLine("stop");
                Console.WriteLine("send");
                string commnad = Console.ReadLine();
                switch (commnad)
                {
                    case "start":
                    {
                        server = new Server();
                    }
                    break;
                    case "stop":
                    {
                        server.Stop();
                        flag = false;
                    }
                    break;
                    case "send":
                    {
                        string message = Console.ReadLine();
                        server.SendToAll(message);
                    }
                    break;
                }
            }
        }
    }
}

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
            Server server = new Server();
            Console.ReadKey();
            server.Stop();
        }
    }
}

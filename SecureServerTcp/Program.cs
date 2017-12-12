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
            BigInteger te= 22;
           Console.WriteLine( BigInteger.Parse(te.ToString()));
            Server server = new Server();
        }
    }
}

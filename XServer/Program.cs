using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XServer.XSocket;

namespace XServer
{
    class Program
    {
        

        static void Main(string[] args)
        {
            Console.WriteLine("[Server]Program has started...");
            XListener xl = new XListener(1453);
            xl.StartListen();
            Console.ReadLine();
        }
    }
}

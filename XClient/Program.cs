using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XClient.XSocket;

namespace XClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[Client] program has start...");
            OurSocket os = new OurSocket(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1453));

            os.StartConnect();
            Console.WriteLine("[Client] User waiting for chat command...");
            string command = Console.ReadLine();
            while (!command.StartsWith("/exit"))
            {
                
                if (command.StartsWith("/chat "))
                {
                    string payload = command.Substring(6);
                    os.SendChat(payload);
                }
                command = Console.ReadLine();
            }
        }
    }
}

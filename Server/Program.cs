using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IPAddress iP;
            if (IPAddress.TryParse(args[0], out iP))
            {
                int port = int.Parse(args[1]);
                TCP_Server server = new TCP_Server(iP, port);
                await server.Start();
            }
            else
            {
                Console.WriteLine("Invalid IP address");
            }
        }
    }
}

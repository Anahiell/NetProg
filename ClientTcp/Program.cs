using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClientTcp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IPAddress iP;
            if (IPAddress.TryParse(args[0], out iP))
            {
                int port = int.Parse(args[1]);
                Client host = new Client("user", "pass");
                await host.Connect(iP, port);
            }
            else
            {
                Console.WriteLine("Invalid IP address");
                Console.ReadKey();
            }
        }
    }
}

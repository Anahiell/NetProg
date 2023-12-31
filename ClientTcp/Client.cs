﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClientTcp
{
    internal class Client
    {
        private TcpClient tcpClient;
        private NetworkStream clientStream;
        private string clientId;
        private string password;

        public Client(string clientID, string password)
        {
            this.clientId = clientID;
            this.password = password;
            tcpClient = new TcpClient();
        }
        public async Task Connect(IPAddress ipAddress, int port)
        {
            
            await tcpClient.ConnectAsync(ipAddress,port);
            using (clientStream = tcpClient.GetStream())
            {


                //аутендефикация
                await Authenticate();

                //получение цитат
                await GetQuotes();

                //завершение
                await Exit();
            }
        }
        private async Task Authenticate()
        {
            //отправка запроса на аутендификацию
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("подсказка user:user или admin:admin");
            Console.ForegroundColor= ConsoleColor.White;
            Console.WriteLine("Enter username:");
            clientId = Console.ReadLine();
            Console.WriteLine("Enter password:");
            password = Console.ReadLine();
            string loginReq = $"login:{clientId}:{password}";
            await SendMess(loginReq);

            //ответ
            string response = await ReceiveMess();
            if (response == "login:success")
            {
                Console.WriteLine($"Authentication successful");
            }
            else
            {
                Console.WriteLine($"Authentication failed");
            }
        }
        private async Task GetQuotes()
        {
            // Запрос на цитаты
            for (int i = 0; i < 3; i++)
            {
                // Отправка запроса на цитату
                await SendMess("get_quote");

                // Ответ
                string quote = await ReceiveMess();
                Console.WriteLine($"Quote {i + 1}: {quote}");

                // Пауза перед отправкой следующего запроса 
                Console.WriteLine("Press ENTER to get the next quote...");
                Console.ReadLine();
            }
        }
        private async Task Exit()
        {
            //отправка запроса для окончания
            Console.WriteLine("EXIT");
            await SendMess("exit");

            //ответ
            string resp = await ReceiveMess();
            Console.WriteLine($"Server response:{resp}");

            //закрытие
            tcpClient.Close();
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();

            Console.WriteLine("Сервер отключился. Нажмите Enter для выхода...");
            Console.ReadLine();
        }
        private async Task SendMess(string mess)
        {
            byte[] messBytes = Encoding.UTF8.GetBytes(mess);
            await clientStream.WriteAsync(messBytes, 0, messBytes.Length);
        }
    private async Task<string> ReceiveMess()
        {
            byte[] buffer = new byte[4096];
            int bytesRead = await clientStream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer,0,bytesRead);
        }
    }
}

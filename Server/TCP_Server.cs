using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    internal class TCP_Server
    {
        private TcpListener tcpListener; //прослушка
        private string[] quotes = { "Цитата 1", "Цитата 2", "Цитата 3", "Цитата 4" };//цитаты
        private static int MAX_CLIENTS = 10;//макс клиенты
        private int MAX_QUOTES_PER_CLIENT = 5;//макс число запросов
        private SemaphoreSlim semaphore = new SemaphoreSlim(MAX_CLIENTS);//семафор для ограничения кол-во клиентов

        public TCP_Server(IPAddress IP, int port)
        {
            tcpListener = new TcpListener(IP, port);//создание тср листенер
        }
        public async Task Start() //запуск сервера, прослушки и ожидание клиентов
        {
            tcpListener.Start();
            Console.WriteLine($"Server is started! Waiting {tcpListener.LocalEndpoint}...");
            while (true)
            {
                TcpClient client = await tcpListener.AcceptTcpClientAsync();
                _ = HandlClientAs(client);

            }
        }
        private async Task HandlClientAs(TcpClient tcpClient)
        {
            await semaphore.WaitAsync();// разрешение от семафора

            NetworkStream clientStream = tcpClient.GetStream();
            Console.WriteLine($"Client is connected {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address} in {DateTime.Now}");
            string clientName = null;
            string clientPass = null;
            int qCount = 0;

            try
            {
                while (true)
                {
                    byte[] message = new byte[4096];
                    int bytesRead;

                    try
                    {
                        bytesRead = await clientStream.ReadAsync(message, 0, 4096);
                    }
                    catch
                    {
                        break;
                    }
                    if (bytesRead == 0) break;
                    string clientRequest = Encoding.UTF8.GetString(message, 0, bytesRead);

                    if (clientRequest.StartsWith("login:"))
                    {
                        string[] some = clientRequest.Split(':');
                        if (some.Length == 3 && ValidUser(some[1], some[2]))
                        {
                            clientName = some[1];
                            clientPass = some[2];
                            await clientStream.WriteAsync(Encoding.UTF8.GetBytes("login:succes"), 0, 13);
                        }
                        else
                        {
                            await clientStream.WriteAsync(Encoding.UTF8.GetBytes("login:faild"), 0, 12);
                        }
                    }
                    else if (clientName != null && clientPass != null)
                    {
                        if (clientRequest.ToLower() == "get_quote" && qCount < MAX_QUOTES_PER_CLIENT)
                        {
                            string randomQuote = quotes[new Random().Next(quotes.Length)];
                            byte[] quoteB = Encoding.UTF8.GetBytes(randomQuote);
                            await clientStream.WriteAsync(quoteB, 0, quoteB.Length);
                            qCount++;
                        }
                        else if (clientRequest.ToLower() == "exit")
                        {
                            Console.ReadKey();
                            break;
                        }
                    }
                }
            }
            finally
            {
                Console.WriteLine($"Client {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address} disconnected in {DateTime.Now}");
                semaphore.Release();
                tcpClient.Close();
            }
        }
        private bool ValidUser(string username, string password)
        {
            //жестко проверяет что бы это был юзер:пасс
            return username == "user" && password == "pass";
        }
    }
}

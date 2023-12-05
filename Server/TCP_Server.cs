using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using static System.Collections.Specialized.BitVector32;

namespace Server
{
    internal class TCP_Server
    {
        private TcpListener tcpListener; //прослушка
        private string[] quotes = { "Цитата 1", "Цитата 2", "Цитата 3", "Цитата 4" };//цитаты
        private static int MAX_CLIENTS = 10;//макс клиенты
        private int MAX_QUOTES_PER_CLIENT = 5;//макс число запросов
        private SemaphoreSlim semaphore = new SemaphoreSlim(MAX_CLIENTS);//семафор для ограничения кол-во клиентов
        private string pathFileUserName = "C:\\Users\\user\\Desktop\\сетквое\\NetProg\\Server\\usersName.txt";
        private string pathLog = "C:\\Users\\user\\Desktop\\сетквое\\NetProg\\Server\\log.txt";
        private List<string> loadUsers;
        private string action;
        public TCP_Server(IPAddress IP, int port)
        {
            tcpListener = new TcpListener(IP, port);//создание тср листенер
            LoadUsers();
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
            await semaphore.WaitAsync();

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

                    if (clientRequest.ToLower() == "exit")
                    {
                        Console.WriteLine($"Client {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address} requested exit.");
                        break;
                    }

                    if (clientRequest.StartsWith("login:"))
                    {
                        clientRequest = clientRequest.Substring(6); // Удаляем "login:"
                        string[] some = clientRequest.Split(':');

                        if (some.Length == 2 && ValidUser(some[0], some[1]))
                        {
                            clientName = some[0];
                            clientPass = some[1];
                            await clientStream.WriteAsync(Encoding.UTF8.GetBytes("login:success"), 0, 13);
                            action = $"Пользователь {clientName} вошел в систему.";
                            Console.WriteLine(action);
                            WriteLog(clientName, DateTime.Now, action);
                        }
                        else
                        {
                            await clientStream.WriteAsync(Encoding.UTF8.GetBytes("login:faild"), 0, 12);
                        }
                    }
                    else
                    {
                       
                        if (clientName == null || clientPass == null)
                        {
                            Console.WriteLine($"Получен запрос без аутентификации: {clientRequest}");
                        }
                        else
                        {
                            if (clientRequest.ToLower() == "get_quote" && qCount < MAX_QUOTES_PER_CLIENT)
                            {
                                string randomQuote = quotes[new Random().Next(quotes.Length)];
                                byte[] quoteB = Encoding.UTF8.GetBytes(randomQuote);
                                await clientStream.WriteAsync(quoteB, 0, quoteB.Length);
                                qCount++;
                                action = null;
                                action = $"Пользователь {clientName} запросил цитату и получил: {randomQuote}";
                                Console.WriteLine(action);
                                WriteLog(clientName, DateTime.Now, action);
                            }
                            else if (clientRequest.ToLower() == "exit")
                            {
                                Console.ReadKey();
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                action = null ;
                action = $"Клиент {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address} ({clientName}) отключен в {DateTime.Now}";
                Console.WriteLine(action);
                WriteLog(clientName, DateTime.Now, action);
                semaphore.Release();
                tcpClient.Close();
                Console.WriteLine("Нажмите Enter для продолжения...");
                Console.ReadLine();
            }
        }
        private bool ValidUser(string username, string password)
        {
            return loadUsers.Any(u => u == $"{username}:{password}");
        }
        private void LoadUsers()
        {
            loadUsers = new List<string>();
            try
            {
                if (!File.Exists(pathFileUserName))
                {
                    File.Create(pathFileUserName).Close();
                }

                using (StreamReader sr = new StreamReader(pathFileUserName))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                        {
                            line = line.Length > 2 ? line.Substring(2) : line;
                            loadUsers.Add(line);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error loading users: {ex.Message}");
            }
        }
        private void WriteLog(string user, DateTime date, string status)
        {
            try
            {
                if (!File.Exists(pathLog))
                {
                    File.Create(pathLog).Close();
                }

                using (StreamWriter sw = new StreamWriter(pathLog, true))
                {
                    string line = $"{user} : {date} = {status}";
                    sw.WriteLine(line);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error writing logs: {ex.Message}");
            }
        }
    }
}

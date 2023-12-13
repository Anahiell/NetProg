using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace Server
{
    internal class TCP_Server
    {
        private string[] moves = { "rock", "paper", "scissors" };

        private bool player1Ready = false;

        private List<TcpClient> connectedClients = new List<TcpClient>();

        private TcpListener tcpListener;
        private SemaphoreSlim semaphore = new SemaphoreSlim(MAX_CLIENTS);

        private List<string> loadUsers;
        private string action;
        private string player1Move;

        private const int MAX_CLIENTS = 2;

        private string pathFileUserName = "C:\\Users\\user\\Desktop\\сетквое\\NetProg\\Server\\usersName.txt";
        private string pathLog = "C:\\Users\\user\\Desktop\\сетквое\\NetProg\\Server\\log.txt";

        public TCP_Server(IPAddress IP, int port)
        {
            tcpListener = new TcpListener(IP, port);
            LoadUsers();
        }

        public async Task Start()
        {
            tcpListener.Start();
            Console.WriteLine($"Server is started! Waiting {tcpListener.LocalEndpoint}...");

            while (true)
            {
                TcpClient client = await tcpListener.AcceptTcpClientAsync();
                Console.WriteLine($"Player connected in {DateTime.Now}");
                _ = Task.Run(() => HandleClientAsync(client));
            }
        }

        private async Task HandleClientAsync(TcpClient tcpClient)
        {
            await semaphore.WaitAsync();

            NetworkStream clientStream = tcpClient.GetStream();

            try
            {

                await clientStream.WriteAsync(Encoding.UTF8.GetBytes($"login:success: PLayer"), 0, $"login:success:Player".Length);

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
                        Console.WriteLine($"Player  requested exit.");
                        break;
                    }

                    if (clientRequest.StartsWith("login:"))
                    {
                        clientRequest = clientRequest.Substring(6);
                        string[] credentials = clientRequest.Split(':');

                        if (credentials.Length == 2 && ValidUser(credentials[0], credentials[1]))
                        {
                            await clientStream.WriteAsync(Encoding.UTF8.GetBytes($"login:success:Player"), 0, 20);
                            action = $"Player  has logged in.";
                            Console.WriteLine(action);
                            WriteLog($"Player ", DateTime.Now, action);
                        }
                        else
                        {
                            await clientStream.WriteAsync(Encoding.UTF8.GetBytes("login:failed"), 0, 12);
                        }
                    }
                    else
                    {

                        await HandleGameRequest(clientRequest, clientStream);

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling player: {ex.Message}");
            }
            finally
            {
                tcpClient.Close();
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }
        private async Task HandleGameRequest(string request, NetworkStream clientStream)
        {
            switch (request.ToLower())
            {
                case "get_move":
                    string serverMove = GetRandomMove();
                    await SendRequestToPlayer(clientStream, $"move:{serverMove}");
                    break;

                case var moveRequest when moveRequest.StartsWith("move:"):
                    string playerMove = moveRequest.Substring("move:".Length);
                    await HandleMoveRequestAsync(playerMove, clientStream);
                    break;

                case "surrender":
                    await HandleSurrender(clientStream);
                    break;

                default:
                    Console.WriteLine($"Received unknown request: {request}");
                    break;
            }
        }

        private async Task HandleMoveRequestAsync(string playerMove, NetworkStream clientStream)
        {
            string serverMove = GetRandomMove();

            string result = DetermineGameResult(playerMove, serverMove);

            await SendRequestToPlayer(clientStream, $"game_result:{result}:{serverMove}");
        }

        private async Task SendRequestToPlayer(NetworkStream stream, string message)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message to player: {ex.Message}");
            }
        }

        private string DetermineGameResult(string playerMove, string serverMove)
        {
            if (playerMove == serverMove)
            {
                return "draw";
            }
            else if ((playerMove == "rock" && serverMove == "scissors") ||
                     (playerMove == "paper" && serverMove == "rock") ||
                     (playerMove == "scissors" && serverMove == "paper"))
            {
                return "win";
            }
            else
            {
                return "lose";
            }
        }
        private string GetRandomMove()
        {
            Random random = new Random();
            int index = random.Next(moves.Length);
            return moves[index];
        }
        private async Task HandleSurrender(NetworkStream clientStream)
        {
            await SendRequestToPlayer(clientStream, "surrender");

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

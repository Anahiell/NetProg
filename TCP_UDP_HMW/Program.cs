using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TCP_UDP_HMW
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string ipAddress = "127.0.0.1";
            int port = 8888;
            // Запуск сервера
            Process serverProcess = new Process();
            serverProcess.StartInfo.FileName = @"C:\Users\user\Desktop\сетквое\NetProg\Server\bin\Debug\\Server.exe"; // Замените на фактический путь
            serverProcess.StartInfo.Arguments = $"{ipAddress} {port}";
            serverProcess.Start();

            await Task.Delay(2000);

            // Запуск первого клиента
            Process client1Process = new Process();
            client1Process.StartInfo.FileName = @"C:\Users\user\Desktop\сетквое\NetProg\ClientTcp\bin\Debug\\ClientTcp.exe"; // Замените на фактический путь
            client1Process.StartInfo.Arguments = $"{ipAddress} {port}";
            client1Process.Start();

            // Запуск второго клиента
            Process client2Process = new Process();
            client2Process.StartInfo.FileName = @"C:\Users\user\Desktop\сетквое\NetProg\ClientTcp\bin\Debug\\ClientTcp.exe"; // Замените на фактический путь
            client2Process.StartInfo.Arguments = $"{ipAddress} {port}";
            client2Process.Start();

            // Ожидание завершения всех процессов
            serverProcess.WaitForExit();
            client1Process.WaitForExit();
            client2Process.WaitForExit();
        }
    }
}
